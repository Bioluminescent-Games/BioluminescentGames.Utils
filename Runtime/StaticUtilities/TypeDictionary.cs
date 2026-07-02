using System;
using System.Collections.Generic;
using UnityEngine;

namespace BioluminescentGames.Utils.StaticUtilities
{
    public static class TypeDictionary
    {
        private static readonly Dictionary<Type, string> NameDictionary = new();

        public static string GetName(this Type type)
        {
            if (NameDictionary.TryGetValue(type, out string typeName)) return typeName;

            // failure to get typename from the cache.
            typeName = type.Name;
            NameDictionary[type] = typeName;

            return typeName;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            NameDictionary.Clear();
        }
    }
}
