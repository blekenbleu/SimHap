// Decompiled with JetBrains decompiler
// Type: SimHaptics.SettingsControl
// Assembly: SimHaptics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
// Assembly location: C:\Users\demas\Downloads\dnSpy-net-win64\SimHaptics.dll

using SimHub.Plugins.Styles;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

#nullable disable
namespace SimHaptics
{
  public partial class SettingsControl : UserControl //, IComponentConnector
  {
/*
    internal TextBox CarName;
    internal TextBox CarId;
    internal TextBox CarClass;
    internal TextBox Redline;
    internal TextBox MaxRPM;
    internal TextBox DownshiftDurationMs;
    internal TextBox UpshiftDurationMs;
    internal TextBox EngineMult;
    internal TextBox EngineConfig;
    internal TextBox CylinderCount;
    internal TextBox EngineLocation;
    internal TextBox PoweredWheels;
    internal TextBox MaxPowerHP;
    internal TextBox EMaxPowerHP;
    internal TextBox DisplacementCC;
    internal TextBox MaxTorqueNm;
    internal TextBlock Version;
    internal SHButtonSecondary Lock;
    internal SHButtonSecondary Refresh;
    internal TextBox MotionPitchOffset;
    internal TextBox MotionPitchMult;
    internal TextBox MotionPitchGamma;
    internal TextBox MotionRollOffset;
    internal TextBox MotionRollMult;
    internal TextBox MotionRollGamma;
    internal TextBox MotionYawOffset;
    internal TextBox MotionYawMult;
    internal TextBox MotionYawGamma;
    internal TextBox MotionHeaveOffset;
    internal TextBox MotionHeaveMult;
    internal TextBox MotionHeaveGamma;
    internal TextBox MotionSurgeOffset;
    internal TextBox MotionSurgeMult;
    internal TextBox MotionSurgeGamma;
    internal TextBox MotionSwayOffset;
    internal TextBox MotionSwayMult;
    internal TextBox MotionSwayGamma;
    internal TextBox RumbleMultAll;
    internal TextBox RumbleMult;
    internal TextBox SuspensionMultAll;
    internal TextBox SuspensionMult;
    internal TextBox SuspensionGammaAll;
    internal TextBox SuspensionGamma;
    internal TextBox SlipXMultAll;
    internal TextBox SlipXMult;
    internal TextBox SlipXGammaAll;
    internal TextBox SlipXGamma;
    internal TextBox ABSPulseLength;
    internal TextBox SlipYMultAll;
    internal TextBox SlipYMult;
    internal TextBox SlipYGammaAll;
    internal TextBox SlipYGamma;
    private bool _contentLoaded;
*/
    public SimHapticsPlugin Plugin { get; }
    public SettingsControl() => this.InitializeComponent();

    public SettingsControl(SimHapticsPlugin plugin) : this()
    {
      this.Plugin = plugin;
      this.DataContext = (object) this.Plugin;
      this.Version.Text = SimHapticsPlugin.PluginVersion;
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
      this.Plugin.S.Id = "";
      SimHapticsPlugin.LoadFailCount = 0;
      SimHapticsPlugin.FetchStatus = APIStatus.Retry;
      SimHapticsPlugin.LoadStatus = DataStatus.None;
      SimHapticsPlugin.LoadFinish = !SimHapticsPlugin.LoadFinish;
    }

    private void Lock_Click(object sender, RoutedEventArgs e)
    {
      this.Plugin.Settings.Unlocked = !this.Plugin.Settings.Unlocked;
      this.Plugin.D.LockedText = this.Plugin.Settings.Unlocked ? "Lock" : "Unlock";
    }
/*
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/SimHaptics;component/settingscontrol.xaml", UriKind.Relative));
    }
*/
/*
    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.CarName = (TextBox) target;
          break;
        case 2:
          this.CarId = (TextBox) target;
          break;
        case 3:
          this.CarClass = (TextBox) target;
          break;
        case 4:
          this.Redline = (TextBox) target;
          break;
        case 5:
          this.MaxRPM = (TextBox) target;
          break;
        case 6:
          this.DownshiftDurationMs = (TextBox) target;
          break;
        case 7:
          this.UpshiftDurationMs = (TextBox) target;
          break;
        case 8:
          this.EngineMult = (TextBox) target;
          break;
        case 9:
          this.EngineConfig = (TextBox) target;
          break;
        case 10:
          this.CylinderCount = (TextBox) target;
          break;
        case 11:
          this.EngineLocation = (TextBox) target;
          break;
        case 12:
          this.PoweredWheels = (TextBox) target;
          break;
        case 13:
          this.MaxPowerHP = (TextBox) target;
          break;
        case 14:
          this.EMaxPowerHP = (TextBox) target;
          break;
        case 15:
          this.DisplacementCC = (TextBox) target;
          break;
        case 16:
          this.MaxTorqueNm = (TextBox) target;
          break;
        case 17:
          this.Version = (TextBlock) target;
          break;
        case 18:
          this.Lock = (SHButtonSecondary) target;
          ((ButtonBase) this.Lock).Click += new RoutedEventHandler(this.Lock_Click);
          break;
        case 19:
          this.Refresh = (SHButtonSecondary) target;
          ((ButtonBase) this.Refresh).Click += new RoutedEventHandler(this.Refresh_Click);
          break;
        case 20:
          this.MotionPitchOffset = (TextBox) target;
          break;
        case 21:
          this.MotionPitchMult = (TextBox) target;
          break;
        case 22:
          this.MotionPitchGamma = (TextBox) target;
          break;
        case 23:
          this.MotionRollOffset = (TextBox) target;
          break;
        case 24:
          this.MotionRollMult = (TextBox) target;
          break;
        case 25:
          this.MotionRollGamma = (TextBox) target;
          break;
        case 26:
          this.MotionYawOffset = (TextBox) target;
          break;
        case 27:
          this.MotionYawMult = (TextBox) target;
          break;
        case 28:
          this.MotionYawGamma = (TextBox) target;
          break;
        case 29:
          this.MotionHeaveOffset = (TextBox) target;
          break;
        case 30:
          this.MotionHeaveMult = (TextBox) target;
          break;
        case 31:
          this.MotionHeaveGamma = (TextBox) target;
          break;
        case 32:
          this.MotionSurgeOffset = (TextBox) target;
          break;
        case 33:
          this.MotionSurgeMult = (TextBox) target;
          break;
        case 34:
          this.MotionSurgeGamma = (TextBox) target;
          break;
        case 35:
          this.MotionSwayOffset = (TextBox) target;
          break;
        case 36:
          this.MotionSwayMult = (TextBox) target;
          break;
        case 37:
          this.MotionSwayGamma = (TextBox) target;
          break;
        case 38:
          this.RumbleMultAll = (TextBox) target;
          break;
        case 39:
          this.RumbleMult = (TextBox) target;
          break;
        case 40:
          this.SuspensionMultAll = (TextBox) target;
          break;
        case 41:
          this.SuspensionMult = (TextBox) target;
          break;
        case 42:
          this.SuspensionGammaAll = (TextBox) target;
          break;
        case 43:
          this.SuspensionGamma = (TextBox) target;
          break;
        case 44:
          this.SlipXMultAll = (TextBox) target;
          break;
        case 45:
          this.SlipXMult = (TextBox) target;
          break;
        case 46:
          this.SlipXGammaAll = (TextBox) target;
          break;
        case 47:
          this.SlipXGamma = (TextBox) target;
          break;
        case 48:
          this.ABSPulseLength = (TextBox) target;
          break;
        case 49:
          this.SlipYMultAll = (TextBox) target;
          break;
        case 50:
          this.SlipYMult = (TextBox) target;
          break;
        case 51:
          this.SlipYGammaAll = (TextBox) target;
          break;
        case 52:
          this.SlipYGamma = (TextBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
*/
  }
}
