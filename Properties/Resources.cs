// Decompiled with JetBrains decompiler
// Type: sierses.Haptics.Properties.Resources
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace sierses.Sim.Properties
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
        if (sierses.Sim.Properties.Resources.resourceMan == null)
          sierses.Sim.Properties.Resources.resourceMan = new ResourceManager("sierses.Sim.Properties.Resources", typeof (sierses.Sim.Properties.Resources).Assembly);
        return sierses.Sim.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => sierses.Sim.Properties.Resources.resourceCulture;
      set => sierses.Sim.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap _100x100_Impacts_White
    {
      get
      {
        return (Bitmap) sierses.Sim.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Impacts_White), sierses.Sim.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_RPM_White
    {
      get
      {
        return (Bitmap) sierses.Sim.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_RPM_White), sierses.Sim.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_Suspension_White
    {
      get
      {
        return (Bitmap) sierses.Sim.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Suspension_White), sierses.Sim.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_Traction_White
    {
      get
      {
        return (Bitmap) sierses.Sim.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Traction_White), sierses.Sim.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap SimHapticsShakerStyleIcon_alt012
    {
      get
      {
        return (Bitmap) sierses.Sim.Properties.Resources.ResourceManager.GetObject(nameof (SimHapticsShakerStyleIcon_alt012), sierses.Sim.Properties.Resources.resourceCulture);
      }
    }
  }
}
