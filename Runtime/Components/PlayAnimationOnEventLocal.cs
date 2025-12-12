#if UNITY_NGO

using BioluminescentGames.Utils;
using Unity.Netcode;
using UnityEngine;

namespace BioluminescentGames.Components
{
    public class PlayAnimationOnEventLocal : NetworkBehaviour
    {
        [SerializeField] private ScriptableEvent evt;
        [SerializeField] private Animator animator;
        [SerializeField] private string id;

        private void Awake()
        {
            if (!IsOwner) return;

            evt.EventTriggered += EvtOnEventTriggered;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (!IsOwner) return;

            evt.EventTriggered -= EvtOnEventTriggered;
        }

        private void EvtOnEventTriggered()
        {
            animator.SetTrigger(id);
        }
    }
}

#endif
