#if (SERIALIZED_COLLECTIONS || UNITY_6000_6_OR_NEWER) && SCENE_REFERENCE

#region

using System;
using System.Collections.Generic;
#if !UNITY_6000_6_OR_NEWER
using AYellowpaper.SerializedCollections;
#endif
using Eflatun.SceneReference;
using UnityEngine;
#if ZLINQ
using ZLinq;
#else
using System.Linq;
#endif

#endregion

namespace BioluminescentGames.Utils.Utilities
{
    [Serializable]
    public class SceneDictionary<T>
    {
#if UNITY_6000_6_OR_NEWER
#pragma warning disable UAC1016
        [SerializeField] private Dictionary<SceneReference, T> dictionary;
#pragma warning restore UAC1016
#else
        [SerializeField, SerializedDictionary("Level", "Value")] private SerializedDictionary<SceneReference, T> dictionary;
#endif

        private bool _init;

        private Dictionary<string, T> _sceneNameDictionary;
        public Dictionary<string, T> SceneNameDictionary
        {
            get
            {
                if (!_init)
                    _sceneNameDictionary = new Dictionary<string, T>(
                        dictionary
#if ZLINQ
                            .AsValueEnumerable()
#endif
                            .ToDictionary(x => x.Key.Name, x => x.Value));
                _init = true;
                return _sceneNameDictionary;
            }
        }

        public T this[string sceneName] => SceneNameDictionary[sceneName];
        public bool TryGetValue(string sceneName, out T value) => SceneNameDictionary.TryGetValue(sceneName, out value);
    }
}

#endif
