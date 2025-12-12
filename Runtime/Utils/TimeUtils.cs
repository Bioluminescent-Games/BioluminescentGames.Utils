using BioluminescentGames.Utils.DependencyInjection;
using System;
using System.Collections;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using UnityEngine;

namespace BioluminescentGames.Utils
{
    public class TimeUtils : MonoSingleton<TimeUtils>, IDependencyProvider
    {
        public event Action OnUpdate;
        public event Action OnLateUpdate;
        public event Action OnFixedUpdate;
        
        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            base.Awake();
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        public void ExecuteOnNextFrame(Action action) => StartCoroutine(ExecuteOnNextFrameImpl(action));
        private static IEnumerator ExecuteOnNextFrameImpl(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        public void WaitBeforeExecuting(int frameWait, Action action) => StartCoroutine(WaitBeforeExecutingImpl(frameWait, action));
        private static IEnumerator WaitBeforeExecutingImpl(int frameWait, Action action)
        {
            for (int i = 0; i < frameWait; i++)
                yield return null;

            action?.Invoke();
        }

        public void WaitForSecondsBeforeExecuting(float delay, Action action) => StartCoroutine(WaitForSecondsBeforeExecutingImpl(delay, action));
        private static IEnumerator WaitForSecondsBeforeExecutingImpl(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action?.Invoke();
        }

        public void WaitForConditionBeforeExecuting(Func<bool> predicate, Action action) => StartCoroutine(WaitForConditionBeforeExecutingImpl(predicate, action));
        private static IEnumerator WaitForConditionBeforeExecutingImpl(Func<bool> predicate, Action action)
        {
            yield return new WaitUntil(predicate);

            action?.Invoke();
        }

        public void ExecuteRepeating(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate, float delaySeconds = 0) => StartCoroutine(ExecuteRepeatingImpl(action, repeatRateSeconds, continueRunningPredicate, delaySeconds));
        private static IEnumerator ExecuteRepeatingImpl(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate, float delaySeconds)
        {
            if (delaySeconds > 0)
                yield return new WaitForSeconds(delaySeconds);

            while (continueRunningPredicate())
            {
                action?.Invoke();
                yield return new WaitForSeconds(repeatRateSeconds);
            }
        }

        [Provide] private TimeUtils ProvideTimeUtils() => this;
    }
}