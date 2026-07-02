using System;
using BioluminescentGames.Utils.MonoBehaviourExtensions;

namespace BioluminescentGames.Utils.Utilities
{
    public class TimeUtils : BioluminescentSingleton<TimeUtils>
    {
        public event Action Updated;
        public event Action LateUpdated;
        public event Action FixedUpdated;

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);
        }

        public override void OnUpdate()
        {
            Updated?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdated?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdated?.Invoke();
        }

        public new void ExecuteOnNextFrame(Action action) => base.ExecuteOnNextFrame(action);

        public new void WaitBeforeExecuting(int frameWait, Action action) => base.WaitBeforeExecuting(frameWait, action);

        public new void WaitForSecondsBeforeExecuting(float delay, Action action) => base.WaitForSecondsBeforeExecuting(delay, action);

        public new void WaitForConditionBeforeExecuting(Func<bool> predicate, Action action) => base.WaitForConditionBeforeExecuting(predicate, action);

        public new void ExecuteRepeating(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate = null, float delaySeconds = 0) => base.ExecuteRepeating(action, repeatRateSeconds, continueRunningPredicate, delaySeconds);

        public new void ExecuteRepeatingRealtime(Action action, float repeatRateSeconds, Func<bool> continueRunningPredicate, float delaySeconds = 0) => base.ExecuteRepeatingRealtime(action, repeatRateSeconds, continueRunningPredicate, delaySeconds);
    }
}
