#if AYELLOWPAPER_SERIALIZED_COLLECTIONS_INSTALLED && EFLATUN_SCENE_REFERENCE_INSTALLED

#region

using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Eflatun.SceneReference;
using UnityEngine;

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
                    _sceneNameDictionary = new Dictionary<string, T>(dictionary.Select(kv => new KeyValuePair<string, T>(kv.Key.Name, kv.Value)));
                _init = true;
                return _sceneNameDictionary;
            }
        }

        public T this[string sceneName] => SceneNameDictionary[sceneName];
        public bool TryGetValue(string sceneName, out T value) => SceneNameDictionary.TryGetValue(sceneName, out value);
    }
}

#endif
