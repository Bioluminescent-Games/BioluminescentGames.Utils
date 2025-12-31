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

        private bool _listIsDirty;

        #if ENABLE_UPDATESYSTEM_PROFILING
        private readonly Dictionary<Type, string> _typenameCache = new();
        #endif


        public void Register(IUpdatable updatable)
        {
            Debug.Assert(!_updatables.Contains(updatable));

            _listIsDirty = true;

            _updatables.Add(updatable);
        }

        public void Unregister(IUpdatable updatable)
        {
            Debug.Assert(_updatables.Contains(updatable));

            _listIsDirty = true;

            _updatables.RemoveSwapBack(updatable);
        }

        protected override void OnUpdate()
        {
            if (_listIsDirty)
            {
#if ENABLE_UPDATESYSTEM_PROFILING
                Profiler.BeginSample("Sort UpdateList");
#endif

                SortUpdateList();
                _listIsDirty = false;

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
                    // failure to get typename from cache.
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
