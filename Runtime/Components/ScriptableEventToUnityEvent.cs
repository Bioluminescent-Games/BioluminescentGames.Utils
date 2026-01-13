using System;
using BioluminescentGames.Utils.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace BioluminescentGames.Utils.Components
{
    public class ScriptableEventToUnityEvent : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent scriptableEvent;
        [SerializeField] private UnityEvent unityEvent;

        private void Awake()
        {
            scriptableEvent.EventTriggered += ScriptableEventOnEventTriggered;
        }

        private void OnDestroy()
        {
            scriptableEvent.EventTriggered -= ScriptableEventOnEventTriggered;
        }

        private void ScriptableEventOnEventTriggered()
        {
            unityEvent.Invoke();
        }
    }
}
