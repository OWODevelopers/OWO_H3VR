using System;

namespace OWOGame
{
    internal static class Math
    {
        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;

            return val;
        }

        public static float Round(float value, int decimals = 1) => (float)System.Math.Round(value, decimals);
    }
}