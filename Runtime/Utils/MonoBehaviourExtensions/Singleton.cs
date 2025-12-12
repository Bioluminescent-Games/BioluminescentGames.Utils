using UnityEngine;

#if UNITY_NGO
using Unity.Netcode;
#endif

namespace BioluminescentGames.Utils.MonoBehaviourExtensions
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable MemberCanBePrivate.Global

    /// <summary>
    /// Template for creating singletons for MonoBehaviours
    /// </summary>
    /// <typeparam name="TSelf">The type inheriting from MonoSingleton</typeparam>
    public abstract class MonoSingleton<TSelf> : MonoBehaviour where TSelf : MonoSingleton<TSelf>
    {
        protected static TSelf instance;

        public static bool HasInstance => instance != null;
        public static TSelf TryGetInstance() => HasInstance ? instance : null;
        public static TSelf Current => instance;

        public static TSelf Instance
        {
            get
            {
                if (instance)
                    return instance;

                instance = FindFirstObjectByType<TSelf>();
                if (instance)
                    return instance;

                Debug.LogWarning($"Auto-creating instance for {typeof(TSelf).Name}");

                GameObject obj = new()
                {
                    name = typeof(TSelf).Name + " AutoCreated"
                };
                return instance = obj.AddComponent<TSelf>();
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        protected virtual void InitializeSingleton()
        {
            if (Application.isPlaying)
                instance = this as TSelf;
        }

        /*[RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad() {
            instance = null;
        }*/
    }

    /// <summary>
    /// Template for creating singletons for regular classes
    /// </summary>
    /// <typeparam name="TSelf">The type inheriting from Singleton</typeparam>
    public abstract class Singleton<TSelf> where TSelf : Singleton<TSelf>, new()
    {
        protected static TSelf instance;
        public static bool HasInstance => instance != null;
        public static TSelf TryGetInstance() => HasInstance ? instance : null;
        public static TSelf Current => Instance;

        public static TSelf Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                return instance = new TSelf();
            }
        }

        /*[RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad() {
            instance = null;
        }*/
    }

    #if UNITY_NGO
    /// <summary>
    /// Template for creating singletons for NetworkObjects
    /// </summary>
    /// <typeparam name="TSelf">The type inheriting from NetworkSingleton</typeparam>
    public abstract class NetworkSingleton<TSelf> : NetworkBehaviour where TSelf : NetworkSingleton<TSelf>
    {
        protected static TSelf instance;
        public static bool HasInstance => instance != null;
        public static TSelf TryGetInstance() => HasInstance ? instance : null;
        public static TSelf Current => instance;

        public static TSelf Instance
        {
            get
            {
                if (instance)
                    return instance;

                instance = FindFirstObjectByType<TSelf>();
                if (instance)
                    return instance;

                Debug.LogError($"FATAL ERROR - {nameof(TSelf)} DOESNT HAVE AN INSTANCE!");

                return null;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (instance == this)
                instance = null;
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            instance = this as TSelf;
        }

        /*[RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad() {
            instance = null;
        }*/
    }

    public abstract class LocalNetworkSingleton<TSelf> : NetworkBehaviour where TSelf : LocalNetworkSingleton<TSelf>
    {
        protected static TSelf local;
        public static bool HasLocalInstance => local != null;
        public static TSelf TryGetInstance() => HasLocalInstance ? local : null;
        public static TSelf Current => local;

        public static TSelf Local
        {
            get
            {
                if (local != null)
                    return local;

                Debug.LogError($"FATAL ERROR - {typeof(TSelf).Name} DOESNT HAVE A LOCAL INSTANCE!");

                return null;
            }
        }

        public override void OnNetworkSpawn()
        {
            InitializeSingleton();

            NetworkSpawn();
        }

        protected virtual void NetworkSpawn() {}

        public override void OnDestroy()
        {
            if (local == this)
                local = null;
        }

        protected virtual void InitializeSingleton()
        {
            if (Application.isPlaying && IsOwner)
                local = this as TSelf;
        }

        /*[RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad() {
            local = null;
        }*/
    }
#endif

    /// <summary>
    /// Template for creating singletons for UI
    /// </summary>
    /// <typeparam name="TSelf">The type inheriting from UISingleton</typeparam>
    public abstract class UISingleton<TSelf> : UIBehaviour where TSelf : UISingleton<TSelf>
    {
        protected static TSelf instance;
        public static bool HasInstance => instance != null;
        public static TSelf TryGetInstance() => HasInstance ? instance : null;
        public static TSelf Current => instance;

        public static TSelf Instance
        {
            get
            {
                if (instance)
                    return instance;

                instance = FindFirstObjectByType<TSelf>();
                if (instance)
                    return instance;

                Debug.LogWarning($"Auto-creating instance for {typeof(TSelf).Name}");

                GameObject obj = new()
                {
                    name = typeof(TSelf).Name + " AutoCreated"
                };
                return instance = obj.AddComponent<TSelf>();
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        protected virtual void InitializeSingleton()
        {
            if (Application.isPlaying)
                instance = this as TSelf;
        }

        /*[RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad() {
            instance = null;
        }*/
    }

    /// <summary>
    /// Template for creating singletons for PublicUI
    /// </summary>
    /// <typeparam name="TSelf">The type inheriting from PublicUISingleton</typeparam>
    public abstract class PublicUISingleton<TSelf> : PublicUIBehaviour where TSelf : PublicUISingleton<TSelf>
    {
        protected static TSelf instance;
        public static bool HasInstance => instance != null;
        public static TSelf TryGetInstance() => HasInstance ? instance : null;
        public static TSelf Current => instance;

        public static TSelf Instance
        {
            get
            {
                if (instance)
                    return instance;

                instance = FindFirstObjectByType<TSelf>();
                if (instance)
                    return instance;

                Debug.LogWarning($"Auto-creating instance for {typeof(TSelf).Name}");

                GameObject obj = new()
                {
                    name = typeof(TSelf).Name + " AutoCreated"
                };
                return instance = obj.AddComponent<TSelf>();
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        protected virtual void InitializeSingleton()
        {
            if (Application.isPlaying)
                instance = this as TSelf;
        }

        /*[RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoad() {
            instance = null;
        }*/
    }
    // ReSharper restore InconsistentNaming
    // ReSharper restore MemberCanBePrivate.Global
}
