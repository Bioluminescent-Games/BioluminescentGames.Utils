using System;
using System.Collections;
using BioluminescentGames.Utils.StaticUtilities;
using BioluminescentGames.Utils.Systems.UpdateSystem;
#if UNITY_NGO
using Unity.Netcode;
#endif
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
            Debug.LogWarning($"OnUpdate method on {GetType().GetName()} is empty or you're calling base.OnUpdate()!");
#endif
        }
        
        protected void ExecuteOnNextFrame(Action action) => StartCoroutine(ExecuteOnNextFrameImpl(action));
        private static IEnumerator ExecuteOnNextFrameImpl(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        protected void WaitBeforeExecuting(int frameWait, Action action) => StartCoroutine(WaitBeforeExecutingImpl(frameWait, action));
        private static IEnumerator WaitBeforeExecutingImpl(int frameWait, Action action)
        {
            for (int i = 0; i < frameWait; i++)
                yield return null;

            action?.Invoke();
        }

        protected void WaitForSecondsBeforeExecuting(float delay, Action action) => StartCoroutine(WaitForSecondsBeforeExecutingImpl(delay, action));
        private static IEnumerator WaitForSecondsBeforeExecutingImpl(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action?.Invoke();
        }

        protected void WaitForConditionBeforeExecuting(Func<bool> predicate, Action action) => StartCoroutine(WaitForConditionBeforeExecutingImpl(predicate, action));
        private static IEnumerator WaitForConditionBeforeExecutingImpl(Func<bool> predicate, Action action)
        {
            yield return new WaitUntil(predicate);

            action?.Invoke();
        }

        protected void ExecuteRepeating(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate = null, float delaySeconds = 0) => StartCoroutine(ExecuteRepeatingImpl(action, repeatRateSeconds, continueRunningPredicate, delaySeconds));
        private static IEnumerator ExecuteRepeatingImpl(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate, float delaySeconds)
        {
            if (delaySeconds > 0)
                yield return new WaitForSeconds(delaySeconds);

            while (continueRunningPredicate?.Invoke() ?? true)
            {
                action?.Invoke();
                yield return new WaitForSeconds(repeatRateSeconds);
            }
        }

        protected void ExecuteRepeatingRealtime(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate = null, float delaySeconds = 0) => StartCoroutine(ExecuteRepeatingRealtimeImpl(action, repeatRateSeconds, continueRunningPredicate, delaySeconds));
        private static IEnumerator ExecuteRepeatingRealtimeImpl(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate, float delaySeconds)
        {
            if (delaySeconds > 0)
                yield return new WaitForSecondsRealtime(delaySeconds);

            while (continueRunningPredicate?.Invoke() ?? true)
            {
                action?.Invoke();
                yield return new WaitForSecondsRealtime(repeatRateSeconds);
            }
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
            Debug.LogWarning($"OnUpdate method on {GetType().GetName()} is empty or you're calling base.OnUpdate()!");
#endif
        }
        
        protected void ExecuteOnNextFrame(Action action) => StartCoroutine(ExecuteOnNextFrameImpl(action));
        private static IEnumerator ExecuteOnNextFrameImpl(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        protected void WaitBeforeExecuting(int frameWait, Action action) => StartCoroutine(WaitBeforeExecutingImpl(frameWait, action));
        private static IEnumerator WaitBeforeExecutingImpl(int frameWait, Action action)
        {
            for (int i = 0; i < frameWait; i++)
                yield return null;

            action?.Invoke();
        }

        protected void WaitForSecondsBeforeExecuting(float delay, Action action) => StartCoroutine(WaitForSecondsBeforeExecutingImpl(delay, action));
        private static IEnumerator WaitForSecondsBeforeExecutingImpl(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action?.Invoke();
        }

        protected void WaitForConditionBeforeExecuting(Func<bool> predicate, Action action) => StartCoroutine(WaitForConditionBeforeExecutingImpl(predicate, action));
        private static IEnumerator WaitForConditionBeforeExecutingImpl(Func<bool> predicate, Action action)
        {
            yield return new WaitUntil(predicate);

            action?.Invoke();
        }

        protected void ExecuteRepeating(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate = null, float delaySeconds = 0) => StartCoroutine(ExecuteRepeatingImpl(action, repeatRateSeconds, continueRunningPredicate, delaySeconds));
        private static IEnumerator ExecuteRepeatingImpl(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate, float delaySeconds)
        {
            if (delaySeconds > 0)
                yield return new WaitForSeconds(delaySeconds);

            while (continueRunningPredicate?.Invoke() ?? true)
            {
                action?.Invoke();
                yield return new WaitForSeconds(repeatRateSeconds);
            }
        }

        protected void ExecuteRepeatingRealtime(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate = null, float delaySeconds = 0) => StartCoroutine(ExecuteRepeatingRealtimeImpl(action, repeatRateSeconds, continueRunningPredicate, delaySeconds));
        private static IEnumerator ExecuteRepeatingRealtimeImpl(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate, float delaySeconds)
        {
            if (delaySeconds > 0)
                yield return new WaitForSecondsRealtime(delaySeconds);

            while (continueRunningPredicate?.Invoke() ?? true)
            {
                action?.Invoke();
                yield return new WaitForSecondsRealtime(repeatRateSeconds);
            }
        }
    }
#endif
}
