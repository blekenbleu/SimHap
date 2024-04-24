// Decompiled with JetBrains decompiler
// Type: SimHaptics.Helpers
// Assembly: SimHaptics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
// Assembly location: C:\Users\demas\Downloads\dnSpy-net-win64\SimHaptics.dll

using System;

#nullable disable
namespace SimHaptics
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
