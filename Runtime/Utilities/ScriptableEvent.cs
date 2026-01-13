using System;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    public abstract class ScriptableEvent<T> : ScriptableObject
    {
        public event Action<T> EventTriggered;

        public void Trigger(T value)
        {
            EventTriggered?.Invoke(value);
        }
    }

    [CreateAssetMenu(fileName = "New Scriptable Event", menuName = "Scriptable Objects/Events/Scriptable Event")]
    public class ScriptableEvent : ScriptableObject
    {
        public event Action EventTriggered;

        public void Trigger()
        {
            EventTriggered?.Invoke();
        }
    }
}
