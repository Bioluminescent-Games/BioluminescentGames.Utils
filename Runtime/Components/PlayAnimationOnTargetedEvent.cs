#if UNITY_NGO

using BioluminescentGames.Utils;
using UnityEngine;

namespace BioluminescentGames.Components
{
    public class PlayAnimationOnTargetedEvent : NetworkBehaviour
    {
        [SerializeField] private TargetedScriptableEvent evt;
        [SerializeField] private Animator animator;
        [SerializeField] private string id;

        private void Awake()
        {
            evt.EventTriggered += EvtOnEventTriggered;
        }

        public override void OnDestroy()
        {
            evt.EventTriggered -= EvtOnEventTriggered;

            base.OnDestroy();
        }

        private void EvtOnEventTriggered(ulong clientId)
        {
            if (clientId != OwnerClientId)
                return;

            animator.SetTrigger(id);
        }
    }
}

#endif
