using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities
{
    public static class GeneralUtils
    {
        public static bool PlayerPrefsGetBool(string key, bool fallback = false)
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetInt(key) != 0;
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
    [GeneratePropertyBag]
    public struct Pair<T1, T2> : IEquatable<Pair<T1, T2>>
    {
        public T1 A;
        public T2 B;

        public Pair(T1 A, T2 B)
        {
            this.A = A;
            this.B = B;
        }

        public bool Equals(Pair<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(A, other.A) 
                   && EqualityComparer<T2>.Default.Equals(B, other.B);
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<T1, T2> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A, B);
        }

        public static bool operator ==(Pair<T1, T2> left, Pair<T1, T2> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Pair<T1, T2> left, Pair<T1, T2> right)
        {
            return !left.Equals(right);
        }
    }

    [Serializable]
    [GeneratePropertyBag]
    public struct Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
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

        public bool Equals(Tuple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(A, other.A) 
                   && EqualityComparer<T2>.Default.Equals(B, other.B) 
                   && EqualityComparer<T3>.Default.Equals(C, other.C);
        }

        public override bool Equals(object obj)
        {
            return obj is Tuple<T1, T2, T3> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A, B, C);
        }

        public static bool operator ==(Tuple<T1, T2, T3> left, Tuple<T1, T2, T3> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tuple<T1, T2, T3> left, Tuple<T1, T2, T3> right)
        {
            return !left.Equals(right);
        }
    }

#if UNITY_NGO
    [Serializable]
    [GeneratePropertyBag]
    public struct NetworkSerializablePair<T1, T2> : IEquatable<NetworkSerializablePair<T1, T2>>, IEquatable<Pair<T1, T2>>
        , INetworkSerializeByMemcpy 
        where T1 : unmanaged
        where T2 : unmanaged
    {
        public T1 A;
        public T2 B;

        public NetworkSerializablePair(T1 A, T2 B)
        {
            this.A = A;
            this.B = B;
        }

        public bool Equals(NetworkSerializablePair<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(A, other.A) 
                   && EqualityComparer<T2>.Default.Equals(B, other.B);
        }

        public bool Equals(Pair<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(A, other.A) 
                   && EqualityComparer<T2>.Default.Equals(B, other.B);
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<T1, T2> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A, B);
        }

        public static bool operator ==(NetworkSerializablePair<T1, T2> left, NetworkSerializablePair<T1, T2> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NetworkSerializablePair<T1, T2> left, NetworkSerializablePair<T1, T2> right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Pair<T1, T2>(NetworkSerializablePair<T1, T2> value)
        {
            return new Pair<T1, T2>(value.A, value.B);
        }

        public static implicit operator NetworkSerializablePair<T1, T2>(Pair<T1, T2> value)
        {
            return new NetworkSerializablePair<T1, T2>(value.A, value.B);
        }
    }

    [Serializable]
    [GeneratePropertyBag]
    public struct NetworkSerializableTuple<T1, T2, T3> : IEquatable<NetworkSerializableTuple<T1, T2, T3>>, IEquatable<Tuple<T1, T2, T3>>
        , INetworkSerializeByMemcpy 
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        public T1 A;
        public T2 B;
        public T3 C;

        public NetworkSerializableTuple(T1 A, T2 B, T3 C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
        }

        public bool Equals(NetworkSerializableTuple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(A, other.A) 
                   && EqualityComparer<T2>.Default.Equals(B, other.B) 
                   && EqualityComparer<T3>.Default.Equals(C, other.C);
        }

        public bool Equals(Tuple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(A, other.A) 
                   && EqualityComparer<T2>.Default.Equals(B, other.B) 
                   && EqualityComparer<T3>.Default.Equals(C, other.C);
        }

        public override bool Equals(object obj)
        {
            return obj is NetworkSerializableTuple<T1, T2, T3> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A, B, C);
        }

        public static bool operator ==(NetworkSerializableTuple<T1, T2, T3> left, NetworkSerializableTuple<T1, T2, T3> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NetworkSerializableTuple<T1, T2, T3> left, NetworkSerializableTuple<T1, T2, T3> right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Tuple<T1, T2, T3>(NetworkSerializableTuple<T1, T2, T3> value)
        {
            return new Tuple<T1, T2, T3>(value.A, value.B, value.C);
        }

        public static implicit operator NetworkSerializableTuple<T1, T2, T3>(Tuple<T1, T2, T3> value)
        {
            return new NetworkSerializableTuple<T1, T2, T3>(value.A, value.B, value.C);
        }
    }
#endif

    // ReSharper enable InconsistentNaming
}
