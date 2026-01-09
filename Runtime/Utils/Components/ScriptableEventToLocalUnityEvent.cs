#if UNITY_NGO

using BioluminescentGames.Utils.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BioluminescentGames.Utils.Components
{
    public class ScriptableEventToLocalUnityEvent : NetworkBehaviour
    {
        [SerializeField] private ScriptableEvent scriptableEvent;
        [SerializeField] private UnityEvent unityEvent;

        private void Awake()
        {
            if (!IsOwner) return;

            scriptableEvent.EventTriggered += ScriptableEventOnEventTriggered;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (!IsOwner) return;

            scriptableEvent.EventTriggered -= ScriptableEventOnEventTriggered;
        }

        private void ScriptableEventOnEventTriggered()
        {
            unityEvent.Invoke();
        }
    }
}

#endif
