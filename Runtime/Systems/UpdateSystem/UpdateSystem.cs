#if UNITY_EDITOR || BUILD_DEBUG
#define ENABLE_UPDATESYSTEM_PROFILING
#endif

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;

namespace BioluminescentGames.Systems.UpdateSystem
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class UpdateSystem : SystemBase
    {
        public static UpdateSystem Instance { get; private set; }
        public static bool HasInstance { get; private set; }

        private readonly List<IUpdatable> _updatables = new();

        private readonly Queue<IUpdatable> _updatablesToAdd = new();
        private readonly Queue<IUpdatable> _updatablesToRemove = new();

        #if ENABLE_UPDATESYSTEM_PROFILING
        private readonly Dictionary<Type, string> _typenameCache = new();
        #endif


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
            Debug.Assert(!_updatables.Contains(updatable));

            _updatables.Add(updatable);
        }

        private void RemoveUpdatable(IUpdatable updatable)
        {
            Debug.Assert(_updatables.Contains(updatable));

            _updatables.RemoveSwapBack(updatable);
        }

        protected override void OnUpdate()
        {
            bool shouldSortList = _updatablesToAdd.Count > 0;

            while (_updatablesToAdd.TryDequeue(out IUpdatable updatableToAdd))
                _updatables.Add(updatableToAdd);

            while (_updatablesToRemove.TryDequeue(out IUpdatable updatableToRemove))
                _updatables.RemoveSwapBack(updatableToRemove);

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
                Type type = updatable.GetType();
                if (!_typenameCache.TryGetValue(type, out string sampleName))
                {
                    // failure to get typename from the cache.
                    sampleName = type.Name;
                    _typenameCache[type] = sampleName;
                }
                Profiler.BeginSample(sampleName);
#endif

                updatable.OnUpdate();

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

                Debug.Assert(behaviourA != null && behaviourB != null);

                return behaviourA.gameObject.GetInstanceID().CompareTo(behaviourB.gameObject.GetInstanceID());*/
                return string.Compare(updatableA.GetType().Name, updatableB.GetType().Name, StringComparison.InvariantCulture);
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
