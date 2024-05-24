// Decompiled with JetBrains decompiler
// Type: SimHaptics.Helpers
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using System;

namespace sierses.Sim
{
  internal static class Helpers
  {
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
    {
      if (value.CompareTo(min) < 0)
        return min;
      return value.CompareTo(max) <= 0 ? value : max;
    }
  }
}
