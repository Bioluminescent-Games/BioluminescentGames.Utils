using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;

namespace BioluminescentGames.Utils
{
    public static class GenericUtils
    {
        public static bool PlayerPrefsGetBool(string key, bool fallback = false)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetInt(key) != 0;
            else
                return fallback;
        }

        public static void PlayerPrefsSetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        private static void DisposeAll(IEnumerable<IDisposable> disposables)
        {
            foreach (IDisposable disposable in disposables)
                disposable.Dispose();
        }

        public static void Dispose(this IEnumerable<IDisposable> disposables) => DisposeAll(disposables);

        public static ushort Get16BitHash(string s)
        {
            using MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
            return BitConverter.ToUInt16(data, 0);
        }
    }

    // ReSharper disable InconsistentNaming
    
    [Serializable]
    public struct Pair<T1, T2>
    {
        public T1 A;
        public T2 B;

        public Pair(T1 A, T2 B)
        {
            this.A = A;
            this.B = B;
        }
    }

    [Serializable]
    [GeneratePropertyBag]
    public struct Tuple<T1, T2, T3>
    {
        public T1 A;
        public T2 B;
        public T3 C;

        public Tuple(T1 A, T2 B, T3 C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
        }
    }
    
    // ReSharper enable InconsistentNaming
}
