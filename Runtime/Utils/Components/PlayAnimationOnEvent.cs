using System;
using BioluminescentGames.Utils.Utilities;
using UnityEngine;

namespace BioluminescentGames.Utils.Components
{
    public class PlayAnimationOnEvent : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent evt;
        [SerializeField] private Animator animator;
        [SerializeField] private string id;

        private void Awake()
        {
            evt.EventTriggered += EvtOnEventTriggered;
        }

        private void OnDestroy()
        {
            evt.EventTriggered -= EvtOnEventTriggered;
        }

        private void EvtOnEventTriggered()
        {
            animator.SetTrigger(id);
        }
    }
}
