using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace blekenbleu.Haptic.Properties
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
        if (blekenbleu.Haptic.Properties.Resources.resourceMan == null)
          blekenbleu.Haptic.Properties.Resources.resourceMan = new ResourceManager("blekenbleu.Haptic.Properties.Resources", typeof (blekenbleu.Haptic.Properties.Resources).Assembly);
        return blekenbleu.Haptic.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => blekenbleu.Haptic.Properties.Resources.resourceCulture;
      set => blekenbleu.Haptic.Properties.Resources.resourceCulture = value;
    }

    internal static Bitmap _100x100_Impacts_White
    {
      get
      {
        return (Bitmap) blekenbleu.Haptic.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Impacts_White), blekenbleu.Haptic.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_RPM_White
    {
      get
      {
        return (Bitmap) blekenbleu.Haptic.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_RPM_White), blekenbleu.Haptic.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_Suspension_White
    {
      get
      {
        return (Bitmap) blekenbleu.Haptic.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Suspension_White), blekenbleu.Haptic.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap _100x100_Traction_White
    {
      get
      {
        return (Bitmap) blekenbleu.Haptic.Properties.Resources.ResourceManager.GetObject(nameof (_100x100_Traction_White), blekenbleu.Haptic.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap SimHapticsShakerStyleIcon_alt012
    {
      get
      {
        return (Bitmap) blekenbleu.Haptic.Properties.Resources.ResourceManager.GetObject(nameof (SimHapticsShakerStyleIcon_alt012), blekenbleu.Haptic.Properties.Resources.resourceCulture);
      }
    }
  }
}
