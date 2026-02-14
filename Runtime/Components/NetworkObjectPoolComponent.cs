#if UNITY_NGO

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace BioluminescentGames.Utils.Components
{
    /// <summary>
    /// Registers NetworkObject Prefabs to be used for pooling instead of spawned with a classic Instantiate and despawned with Object.Destroy.
    /// Based on https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/blob/main/Assets/Scripts/Infrastructure/NetworkObjectPool.cs
    /// </summary>
    public class NetworkObjectPoolComponent : MonoBehaviour
    {
        [SerializeField] private PoolConfigObject[] pooledPrefabs;

        private readonly HashSet<NetworkObject> _registeredPrefabs = new();

        private readonly Dictionary<NetworkObject, ObjectPool<NetworkObject>> _pools = new();

        private void Awake()
        {
            foreach (PoolConfigObject prefab in pooledPrefabs)
                RegisterPrefab(prefab.Prefab, prefab.PrewarmCount);
        }

        private void OnDestroy()
        {
            foreach (NetworkObject prefab in _registeredPrefabs)
            {
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
                _pools[prefab].Clear();
            }

            _pools.Clear();
            _registeredPrefabs.Clear();
        }

        public NetworkObject GetNetworkObject(NetworkObject prefab, Vector3 position, Quaternion rotation)
        {
            NetworkObject networkObject = _pools[prefab].Get();
            networkObject.transform.SetPositionAndRotation(position, rotation);
            return networkObject;
        }

        public void ReturnNetworkObject(NetworkObject networkObject, NetworkObject prefab)
        {
            _pools[prefab].Release(networkObject);
        }

        private void RegisterPrefab(NetworkObject prefab, int prewarmCount)
        {
            _registeredPrefabs.Add(prefab);

            // NOTE: defaultCapacity only allocates said number of places in the list. We need to actually create the objects manually.
            _pools[prefab] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: prewarmCount);

            // Create the objects manually
            List<NetworkObject> objectsCreatedForPrewarm = new();
            for (int i = 0; i < prewarmCount; i++)
                objectsCreatedForPrewarm.Add(_pools[prefab].Get()); // This creates an object
            foreach (NetworkObject networkObject in objectsCreatedForPrewarm)
                _pools[prefab].Release(networkObject); // Put it back

            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));


            return;

            void ActionOnGet(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(true);
            }

            void ActionOnRelease(NetworkObject networkObject)
            {
                networkObject.gameObject.SetActive(false);
            }

            void ActionOnDestroy(NetworkObject networkObject)
            {
                if (networkObject)
                    Destroy(networkObject.gameObject);
            }

            NetworkObject CreateFunc()
            {
                return Instantiate(prefab);
            }
        }
    }

    [Serializable]
    internal struct PoolConfigObject
    {
        [field: SerializeField] public NetworkObject Prefab { get; private set; }
        [field: SerializeField] public int PrewarmCount { get; private set; }
    }

    internal class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly NetworkObject _prefab;
        private readonly NetworkObjectPoolComponent _pool;

        public PooledPrefabInstanceHandler(NetworkObject prefab, NetworkObjectPoolComponent pool)
        {
            _prefab = prefab;
            _pool = pool;
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return _pool.GetNetworkObject(_prefab, position, rotation);
        }

        public void Destroy(NetworkObject networkObject)
        {
            _pool.ReturnNetworkObject(networkObject, _prefab);
        }
    }
}

#endif
