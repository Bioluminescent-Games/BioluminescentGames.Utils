using System;
using BioluminescentGames.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace BioluminescentGames.Components
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
