#if SERIALIZED_COLLECTIONS && SCENE_REFERENCE

#region

using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Eflatun.SceneReference;
using UnityEngine;
using ZLinq;

#endregion

namespace BioluminescentGames.Utils.Utilities
{
    [Serializable]
    public class SceneDictionary<T>
    {
        [SerializeField, SerializedDictionary("Level", "Value")] private SerializedDictionary<SceneReference, T> dictionary;

        private bool _init;

        private Dictionary<string, T> _sceneNameDictionary;
        public Dictionary<string, T> SceneNameDictionary
        {
            get
            {
                if (!_init)
                    _sceneNameDictionary = new Dictionary<string, T>(
                        dictionary
                            .AsValueEnumerable()
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
