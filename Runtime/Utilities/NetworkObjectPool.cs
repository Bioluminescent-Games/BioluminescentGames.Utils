#if UNITY_NGO

using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;

namespace BioluminescentGames.Utils.Utilities
{
    /// <summary>
    /// An ObjectPool that contains NetworkObjects. Correctly handles spawning,
    /// despawning, and more stuff.
    /// </summary>
    public class NetworkObjectPool : INetworkPrefabInstanceHandler, IDisposable
    {
        private readonly ObjectPool<NetworkObject> _pool;

        private readonly NetworkObject _prefab;

        public static NetworkObjectPool CreateAndRegisterFor(NetworkObject prefab)
        {
            NetworkObjectPool handler = new(prefab);
            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, handler);
            return handler;
        }

        /// <summary>
        /// If using this constructor, remember to register it with the <see cref="NetworkPrefabHandler"/> inside the <see cref="NetworkManager"/>.
        /// This can be done with
        /// <code>
        /// NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, handler);
        /// </code>
        /// </summary>
        /// <param name="prefab"></param>
        public NetworkObjectPool(NetworkObject prefab)
        {
            _prefab = prefab;

            _pool = new ObjectPool<NetworkObject>(ObjectPoolCreateInstance, ObjectPoolOnGet, ObjectPoolOnRelease, ObjectPoolOnDestroy);
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            _pool.Get(out NetworkObject networkObject);
            networkObject.transform.position = position;
            networkObject.transform.rotation = rotation;
            return networkObject;
        }

        public void Destroy(NetworkObject networkObject)
        {
            _pool.Release(networkObject);
        }

        private NetworkObject ObjectPoolCreateInstance()
        {
            return UnityEngine.Object.Instantiate(_prefab);
        }

        private static void ObjectPoolOnGet(NetworkObject networkObject)
        {
            networkObject.gameObject.SetActive(true);
        }

        private static void ObjectPoolOnRelease(NetworkObject networkObject)
        {
            networkObject.gameObject.SetActive(false);
        }

        private static void ObjectPoolOnDestroy(NetworkObject networkObject)
        {
            if (networkObject)
                UnityEngine.Object.Destroy(networkObject.gameObject);
        }

        public void Dispose()
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(_prefab);
            _pool?.Dispose();
        }
    }
}

#endif
