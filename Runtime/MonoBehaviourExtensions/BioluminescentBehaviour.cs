using BioluminescentGames.Utils.Systems.UpdateSystem;
using Unity.Netcode;
using UnityEngine;

namespace BioluminescentGames.Utils.MonoBehaviourExtensions
{
    public abstract class BioluminescentBehaviour : MonoBehaviour, IUpdatable
    {
        protected virtual void OnEnable()
        {
            if (UpdateSystem.HasInstance)
                UpdateSystem.Instance.Register(this);
        }

        protected virtual void OnDisable()
        {
            if (UpdateSystem.HasInstance)
                UpdateSystem.Instance.Unregister(this);
        }

        public virtual void OnUpdate()
        {
#if UNITY_EDITOR || BUILD_DEBUG
            Debug.LogWarning($"OnUpdate method on {gameObject.name} is empty or you're calling base.OnUpdate()!");
#endif
        }
    }

#if UNITY_NGO
    public abstract class BioluminescentNetworkBehaviour : NetworkBehaviour, IUpdatable
    {
        protected virtual void OnEnable()
        {
            UpdateSystem.Instance.Register(this);
        }

        protected virtual void OnDisable()
        {
            if (UpdateSystem.HasInstance)
                UpdateSystem.Instance.Unregister(this);
        }

        public virtual void OnUpdate()
        {
#if UNITY_EDITOR || BUILD_DEBUG
            Debug.LogWarning($"OnUpdate method on {gameObject.name} is empty or you're calling base.OnUpdate()!");
#endif
        }
    }
#endif
}
