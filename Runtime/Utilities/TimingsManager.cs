using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.MonoBehaviourExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace BioluminescentGames.Utils.Utilities
{
    public class TimingsManager : BioluminescentBehaviour
    {
        internal readonly Dictionary<int, TickDelay> TickDelayDictionary = new();

        private int _currentId;

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

                if (TickDelayDictionary.ContainsKey(GameTicks))
                    HandleInvokeTickDelayEvent(GameTicks);
            }
        }

        private void HandleInvokeTickDelayEvent(int gameTicks)
        {
            if (!TickDelayDictionary.TryGetValue(gameTicks, out TickDelay tickDelay)) return;

            tickDelay.Invoke();
            TickDelayDictionary.Remove(gameTicks);
        }

        public TickDelay GetEventAfterTicks(int ticks)
        {
            int targetTimeTicks = GameTicks + ticks;

            if (TickDelayDictionary.TryGetValue(targetTimeTicks, out TickDelay delay)) return delay;

            TickDelay tickDelay = new(targetTimeTicks, _currentId++);
            tickDelay.Discard += TickDelay_Discard;
            TickDelayDictionary.Add(targetTimeTicks, tickDelay);

            return TickDelayDictionary[targetTimeTicks];
        }

        private void TickDelay_Discard(TickDelay tickDelay)
        {
            if (TickDelayDictionary.ContainsValue(tickDelay))
                TickDelayDictionary.Remove(tickDelay.Delay);
        }

        public struct TickDelay : IEquatable<TickDelay>
        {
            public int Delay { get; }
            private readonly UnityEvent _event;
            private readonly List<UnityAction> _listeners;

            private int Id { get; }

            internal Action<TickDelay> Discard;

            internal TickDelay(int delay, int id)
            {
                _event = new UnityEvent();
                _listeners = new List<UnityAction>();
                Discard = null;
                Delay = delay;
                Id = id;
            }

            internal readonly void Invoke()
            {
                _event?.Invoke();
            }

            public readonly void AddListener(UnityAction call)
            {
                _event.AddListener(call);
                _listeners.Add(call);
            }

            public readonly void RemoveListener(UnityAction call)
            {
                if (_listeners.Contains(call))
                {
                    _event.RemoveListener(call);
                    _listeners.Remove(call);

                    if (_listeners.Count < 1)
                        // No more listeners
                        Discard?.Invoke(this);
                }
            }

            public bool Equals(TickDelay other)
            {
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                return obj is TickDelay other && Equals(other);
            }

            public override int GetHashCode()
            {
                return Id;
            }

            public static bool operator ==(TickDelay left, TickDelay right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(TickDelay left, TickDelay right)
            {
                return !left.Equals(right);
            }
        }
    }
}
