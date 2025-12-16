using UnityEngine;

namespace BioluminescentGames.Utils.MonoBehaviourExtensions
{
    public abstract class BioluminescentBehaviour : MonoBehaviour, IUpdatable
    {
        protected virtual void Awake()
        {
            UpdateSystem.Instance.Register(this);
        }

        protected virtual void OnDestroy()
        {
            if (UpdateSystem.HasInstance)
                UpdateSystem.Instance.Unregister(this);
        }

        public virtual void OnUpdate() {}
    }
}
