using System;
using UnityEngine;

namespace BioluminescentGames.Utils.Utilities {

    public static class NumberUtils
    {
        public static float Map(float value, float from1, float to1, float from2, float to2)
        {
            if (Mathf.Approximately(from1, to1) || Mathf.Approximately(from2, to2))
                throw new ArgumentException("Cannot remap range with same from and to values. (got " + from1 + " to " + to1 + ")");

            if (Mathf.Approximately(value, float.MaxValue) || Mathf.Approximately(from1, float.MaxValue) ||
                Mathf.Approximately(to1, float.MaxValue) || Mathf.Approximately(from2, float.MaxValue) ||
                Mathf.Approximately(to2, float.MaxValue))
                throw new ArgumentException("Cannot remap range with MaxValue values.");

            return from2 + (value - from1) * (to2 - from2) / (to1 - from1);
        }

        public static int Map(int value, int from1, int to1, int from2, int to2)
        {
            if (from1 == to1 || from2 == to2)
                throw new ArgumentException("Cannot remap range with same from and to values. (got " + from1 + " to " + to1 + ")");

            if (value == int.MaxValue || from1 == int.MaxValue || to1 == int.MaxValue || from2 == int.MaxValue || to2 == int.MaxValue)
                throw new ArgumentException("Cannot remap range with MaxValue values.");

            return from2 + (value - from1) * (to2 - from2) / (to1 - from1);
        }

        public static double Map(double value, double from1, double to1, double from2, double to2)
        {
            if (Approximately(from1, to1) || Approximately(from2, to2))
                throw new ArgumentException("Cannot remap range with same from and to values. (got " + from1 + " to " + to1 + ")");

            if (Approximately(value, double.MaxValue) || Approximately(from1, double.MaxValue) ||
                Approximately(to1, double.MaxValue) || Approximately(from2, double.MaxValue) ||
                Approximately(to2, double.MaxValue))
                throw new ArgumentException("Cannot remap range with MaxValue values.");

            return from2 + (value - from1) * (to2 - from2) / (to1 - from1);
        }

        public static bool Approximately(double a, double b)
        {
            return Math.Abs(b - a) < Math.Max(1E-06f * Math.Max(Math.Abs(a), Math.Abs(b)), Mathf.Epsilon * 8f); // Yoink from Mathf.Approximately.
        }

        public static float RoundToDecimals(float value, uint decimals)
        {
            float powerOfTen = Mathf.Pow(10, decimals);
            return Mathf.Round(value * powerOfTen) / powerOfTen;
        }

        public static double RoundToDecimals(double value, uint decimals)
        {
            double powerOfTen = Math.Pow(10, decimals);
            return Math.Round(value * powerOfTen) / powerOfTen;
        }
    }

}
