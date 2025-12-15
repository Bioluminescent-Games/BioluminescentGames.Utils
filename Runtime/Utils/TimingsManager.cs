using System;
using System.Collections.Generic;
using BioluminescentGames.Systems.UpdateSystem;
using UnityEngine;
using UnityEngine.Events;

namespace BioluminescentGames.Utils
{
    public class TimingsManager : BioluminescentBehaviour
    {
        private readonly Dictionary<int, TickDelay> _tickDelayDictionary = new();

        public event Action Tick;
        public int GameTicks { get; private set; }
        public float TotalTimePassed { get; private set; }
        public float TotalTimePassedUnscaled { get; private set; }

        private const float k_TickDuration = 0.05f;

        public override void OnUpdate()
        {
            UpdateTimings();
        }

        private void UpdateTimings()
        {
            TotalTimePassed += Time.deltaTime;
            TotalTimePassedUnscaled += Time.unscaledDeltaTime;

            int totalTicksPassed = Mathf.FloorToInt(TotalTimePassed / k_TickDuration);

            while (totalTicksPassed < GameTicks)
            {
                GameTicks++;
                // A tick has passed!
                Tick?.Invoke();

                if (_tickDelayDictionary.ContainsKey(GameTicks))
                    HandleInvokeTickDelayEvent(GameTicks);
            }
        }

        private void HandleInvokeTickDelayEvent(int gameTicks)
        {
            if (!_tickDelayDictionary.TryGetValue(gameTicks, out TickDelay tickDelay)) return;

            tickDelay.Invoke();
            _tickDelayDictionary.Remove(gameTicks);
        }

        public TickDelay GetEventAfterTicks(int ticks)
        {
            int targetTimeTicks = GameTicks + ticks;

            if (!_tickDelayDictionary.ContainsKey(targetTimeTicks))
            {
                TickDelay tickDelay = new TickDelay(targetTimeTicks);
                tickDelay.Discard += TickDelay_Discard;
                _tickDelayDictionary.Add(targetTimeTicks, tickDelay);
            }

            return _tickDelayDictionary[targetTimeTicks];
        }

        private void TickDelay_Discard(TickDelay tickDelay)
        {
            if (_tickDelayDictionary.ContainsValue(tickDelay))
                _tickDelayDictionary.Remove(tickDelay.Delay);
        }

        public struct TickDelay
        {
            public int Delay { get; }
            private readonly UnityEvent Event;
            private readonly List<UnityAction> listeners;

            internal Action<TickDelay> Discard;

            public TickDelay(int delay)
            {
                Event = new UnityEvent();
                listeners = new List<UnityAction>();
                Discard = null;
                Delay = delay;
            }

            internal readonly void Invoke()
            {
                Event?.Invoke();
            }

            public readonly void AddListener(UnityAction call)
            {
                Event.AddListener(call);
                listeners.Add(call);
            }

            public readonly void RemoveListener(UnityAction call)
            {
                if (listeners.Contains(call))
                {
                    Event.RemoveListener(call);
                    listeners.Remove(call);

                    if (listeners.Count < 1)
                        // No more listeners
                        Discard?.Invoke(this);
                }
            }
        }
    }
}
