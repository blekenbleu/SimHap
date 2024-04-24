// Decompiled with JetBrains decompiler
// Type: SimHaptics.Properties.Resources
// Assembly: SimHaptics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
// Assembly location: C:\Users\demas\Downloads\dnSpy-net-win64\SimHaptics.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace SimHaptics.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (SimHaptics.Properties.Resources.resourceMan == null)
          SimHaptics.Properties.Resources.resourceMan = new ResourceManager("SimHaptics.Properties.Resources", typeof (SimHaptics.Properties.Resources).Assembly);
        return SimHaptics.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => SimHaptics.Properties.Resources.resourceCulture;
      set => SimHaptics.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap _100x100_Impacts_White
    {
      get
      {
        return (Bitmap) SimHaptics.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Impacts_White), SimHaptics.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_RPM_White
    {
      get
      {
        return (Bitmap) SimHaptics.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_RPM_White), SimHaptics.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_Suspension_White
    {
      get
      {
        return (Bitmap) SimHaptics.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Suspension_White), SimHaptics.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_Traction_White
    {
      get
      {
        return (Bitmap) SimHaptics.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Traction_White), SimHaptics.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap SimHapticsShakerStyleIcon_alt012
    {
      get
      {
        return (Bitmap) SimHaptics.Properties.Resources.ResourceManager.GetObject(nameof (SimHapticsShakerStyleIcon_alt012), SimHaptics.Properties.Resources.resourceCulture);
      }
    }
  }
}
