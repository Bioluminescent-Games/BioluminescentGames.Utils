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
                SortUpdateList();
                _listIsDirty = false;
            }

            foreach (IUpdatable updatable in _updatables)
            {
#if UNITY_EDITOR || BUILD_DEBUG
                Profiler.BeginSample(updatable.GetType().Name);
#endif
                updatable.OnUpdate();
#if UNITY_EDITOR || BUILD_DEBUG
                Profiler.EndSample();
#endif
            }
        }

        private void SortUpdateList()
        {
            _updatables.Sort((updatableA, updatableB) =>
            {
                MonoBehaviour behaviourA = updatableA as MonoBehaviour;
                MonoBehaviour behaviourB = updatableB as MonoBehaviour;

                Debug.Assert(behaviourA != null && behaviourB != null);

                return behaviourA.gameObject.GetInstanceID().CompareTo(behaviourB.gameObject.GetInstanceID());
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
