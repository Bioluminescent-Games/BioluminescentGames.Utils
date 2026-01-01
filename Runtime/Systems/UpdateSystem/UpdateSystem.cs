#if UNITY_EDITOR || BUILD_DEBUG
#define ENABLE_UPDATESYSTEM_PROFILING
#endif

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

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

        private readonly Dictionary<Type, string> _typenameCache = new();

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
            Debug.Assert(!_updatables.Contains(updatable), $"Tried to add updatable {updatable} that was already registered.", updatable as Object);

            _updatables.Add(updatable);
        }

        private void RemoveUpdatable(IUpdatable updatable)
        {
            if (!_updatables.Contains(updatable))
                // Some objects may spawn disabled.
                return;

            _updatables.RemoveSwapBack(updatable);
        }

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

                Profiler.BeginSample(GetTypeName(updatable.GetType()));
#endif

                try
                {
                    updatable.OnUpdate();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
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

                Debug.Assert(behaviourA != null && behaviourB != null);

                return behaviourA.gameObject.GetInstanceID().CompareTo(behaviourB.gameObject.GetInstanceID());*/

                // ReSharper disable once ConvertToLambdaExpression
                return string.Compare(GetTypeName(updatableA.GetType()), GetTypeName(updatableB.GetType()), StringComparison.InvariantCulture);
            });
        }

        private string GetTypeName(Type type)
        {
            if (_typenameCache.TryGetValue(type, out string typeName)) return typeName;

            // failure to get typename from the cache.
            typeName = type.Name;
            _typenameCache[type] = typeName;

            return typeName;
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
