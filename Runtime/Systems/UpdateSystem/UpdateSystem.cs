#if UNITY_EDITOR || BUILD_DEBUG
#define ENABLE_UPDATESYSTEM_PROFILING
#endif

using System;
using System.Collections.Generic;
using BioluminescentGames.Utils.StaticUtilities;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace BioluminescentGames.Utils.Systems.UpdateSystem
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class UpdateSystem : SystemBase
    {
        public static UpdateSystem Instance { get; private set; }
        public static bool HasInstance { get; private set; }

        private readonly List<IUpdatable> _updatables = new();

        private readonly Queue<IUpdatable> _updatablesToAdd = new();
        private readonly Queue<IUpdatable> _updatablesToRemove = new();

        public void Register(IUpdatable updatable)
        {
            _updatablesToAdd.Enqueue(updatable);
        }

        public void Unregister(IUpdatable updatable)
        {
            _updatablesToRemove.Enqueue(updatable);
        }

        private void AddUpdatable(IUpdatable updatable)
        {
            Log.Assert(!(_updatables.Contains(updatable) && !_updatablesToRemove.Contains(updatable)), $"Tried to add updatable {updatable} that was already registered.", updatable as Object);

            _updatables.Add(updatable);
        }

        private void RemoveUpdatable(IUpdatable updatable)
        {
            if (!_updatables.Contains(updatable))
                // Some objects may spawn disabled.
                return;
            
            _updatables.RemoveSwapBack(updatable);
        }

        [HideInCallstack]
        protected override void OnUpdate()
        {
            bool shouldSortList = _updatablesToAdd.Count > 0;

            while (_updatablesToAdd.TryDequeue(out IUpdatable updatableToAdd))
                AddUpdatable(updatableToAdd);

            while (_updatablesToRemove.TryDequeue(out IUpdatable updatableToRemove))
                RemoveUpdatable(updatableToRemove);

            if (shouldSortList)
            {
#if ENABLE_UPDATESYSTEM_PROFILING
                Profiler.BeginSample("Sort UpdateList");
#endif

                SortUpdateList();

#if ENABLE_UPDATESYSTEM_PROFILING
                Profiler.EndSample();
#endif
            }

            foreach (IUpdatable updatable in _updatables)
            {
#if ENABLE_UPDATESYSTEM_PROFILING
                if (updatable is MonoBehaviour monoBehaviour)
                    Profiler.BeginSample(updatable.GetType().GetName(), monoBehaviour);
                else
                    Profiler.BeginSample(updatable.GetType().GetName() + " (not MonoBehaviour)");
#endif

                try
                {
                    updatable.OnUpdate();
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }

#if ENABLE_UPDATESYSTEM_PROFILING
                Profiler.EndSample();
#endif
            }
        }

        private void SortUpdateList()
        {
            _updatables.Sort((updatableA, updatableB) =>
            {
                /*MonoBehaviour behaviourA = updatableA as MonoBehaviour;
                MonoBehaviour behaviourB = updatableB as MonoBehaviour;

                Log.Assert(behaviourA != null && behaviourB != null);

                return behaviourA.gameObject.GetInstanceID().CompareTo(behaviourB.gameObject.GetInstanceID());*/

                // ReSharper disable once ConvertToLambdaExpression
                return string.Compare(updatableA.GetType().GetName(), updatableB.GetType().GetName(), StringComparison.InvariantCulture);
            });
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            Instance = null;
            HasInstance = false;
        }
#endif

        protected override void OnCreate()
        {
            Instance = this;
            HasInstance = true;
        }

        protected override void OnDestroy()
        {
            Instance = null;
            HasInstance = false;
        }
    }
}
