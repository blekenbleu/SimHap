// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimHapticsPlugin
// Assembly: SimHaptics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
// Assembly location: C:\Users\demas\Downloads\dnSpy-net-win64\SimHaptics.dll

using GameReaderCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimHaptics.Properties;
using SimHub;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace SimHaptics
{
  [PluginDescription("Properties for haptic feedback and more")]
  [PluginAuthor("sierses")]
  [PluginName("SimHaptics")]
  public class SimHapticsPlugin : IPlugin, IDataPlugin, IWPFSettingsV2, IWPFSettings
  {
    public static string PluginVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
    public static string SimHubVersion;
    public static int LoadFailCount;
    public static bool LoadFinish;
    public static DataStatus LoadStatus;
    public static APIStatus FetchStatus;
    public static long FrameTimeTicks = 0;
    public static long FrameCountTicks = 0;
    public static GameId CurrentGame = GameId.Other;
    public static string GameDBText;
    public static string FailedId = "";
    public static string FailedCategory = "";
    private static readonly HttpClient client = new HttpClient();

    public Spec S { get; set; }

    public SimData D { get; set; }

    public Settings Settings { get; set; }

    public PluginManager PluginManager { get; set; }

    public ImageSource PictureIcon
    {
      get
      {
        return (ImageSource) IWpfSettingsExtensions.ToIcon((IWPFSettings) this, Resources.SimHapticsShakerStyleIcon_alt012);
      }
    }

    public ImageSource RPMIcon
    {
      get
      {
        return (ImageSource) IWpfSettingsExtensions.ToIcon((IWPFSettings) this, Resources._100x100_RPM_White);
      }
    }

    public ImageSource ImpactsIcon
    {
      get
      {
        return (ImageSource) IWpfSettingsExtensions.ToIcon((IWPFSettings) this, Resources._100x100_Impacts_White);
      }
    }

    public ImageSource SuspensionIcon
    {
      get
      {
        return (ImageSource) IWpfSettingsExtensions.ToIcon((IWPFSettings) this, Resources._100x100_Suspension_White);
      }
    }

    public ImageSource TractionIcon
    {
      get
      {
        return (ImageSource) IWpfSettingsExtensions.ToIcon((IWPFSettings) this, Resources._100x100_Traction_White);
      }
    }

    public string LeftMenuTitle => "SimHaptics";

    public void DataUpdate(PluginManager pluginManager, ref GameData data)
    {
      SimHapticsPlugin.FrameCountTicks = SimHapticsPlugin.FrameCountTicks + DateTime.Now.Ticks - SimHapticsPlugin.FrameTimeTicks;
      SimHapticsPlugin.FrameTimeTicks = DateTime.Now.Ticks;
      if (SimHapticsPlugin.FrameCountTicks > 864000000000L)
        SimHapticsPlugin.FrameCountTicks = 0L;
      if (SimHapticsPlugin.FrameCountTicks % 2500000L <= 150000L && (data.GameRunning || data.GamePaused || data.GameReplay || data.GameInMenu) && this.Settings.Unlocked && data.NewData != null)
        this.SetVehiclePerGame(pluginManager, ref data.NewData);
      if (!data.GameRunning || data.OldData == null || data.NewData == null)
        return;
      this.D.FPS = (double) pluginManager.GetPropertyValue("DataCorePlugin.DataUpdateFps");
      this.D.RPMPercent = data.NewData.Rpms * this.D.InvMaxRPM;
      this.D.SpeedMs = data.NewData.SpeedKmh * 0.277778;
      this.D.InvSpeedMs = this.D.SpeedMs != 0.0 ? 1.0 / this.D.SpeedMs : 0.0;
      this.D.Accelerator = data.NewData.Throttle;
      this.D.Brake = data.NewData.Brake;
      this.D.Clutch = data.NewData.Clutch;
      this.D.Handbrake = data.NewData.Handbrake;
      this.D.BrakeBias = data.NewData.BrakeBias;
      this.D.BrakeF = this.D.Brake * (2.0 * this.D.BrakeBias) * 0.01;
      this.D.BrakeR = this.D.Brake * (200.0 - 2.0 * this.D.BrakeBias) * 0.01;
      this.D.BrakeVelP = this.D.BrakeVel;
      this.D.BrakeVel = (this.D.Brake - data.OldData.Brake) * this.D.FPS;
      this.D.BrakeAcc = (this.D.BrakeVel - this.D.BrakeVelP) * this.D.FPS;
      if (this.D.CarInitCount < 2)
      {
        this.D.SuspensionDistFLP = this.D.SuspensionDistFL;
        this.D.SuspensionDistFRP = this.D.SuspensionDistFR;
        this.D.SuspensionDistRLP = this.D.SuspensionDistRL;
        this.D.SuspensionDistRRP = this.D.SuspensionDistRR;
        this.D.YawPrev = data.NewData.OrientationYaw;
        this.D.Yaw = data.NewData.OrientationYaw;
        this.D.RumbleLeftAvg = 0.0;
        this.D.RumbleRightAvg = 0.0;
      }
      this.D.YawPrev = this.D.Yaw;
      this.D.Yaw = -data.NewData.OrientationYaw;
      if (this.D.Yaw > 100.0 && this.D.YawPrev < -100.0)
        this.D.YawPrev += 360.0;
      else if (this.D.Yaw < -100.0 && this.D.YawPrev > 100.0)
        this.D.YawPrev -= 360.0;
      this.D.YawRate = (this.D.Yaw - this.D.YawPrev) * this.D.FPS;
      if (this.D.YawRateAvg != 0.0)
      {
        if (Math.Abs(this.D.YawRate) < Math.Abs(this.D.YawRateAvg * 1.25))
          this.D.YawRateAvg = (this.D.YawRateAvg + this.D.YawRate) * 0.5;
        else
          this.D.YawRateAvg *= 1.25;
      }
      else
        this.D.YawRateAvg = this.D.YawRate;
      ++this.D.Acc0;
      this.D.Acc1 = this.D.Acc0 - 1;
      if (this.D.Acc0 >= this.D.AccSamples)
      {
        this.D.Acc0 = 0;
        this.D.Acc1 = this.D.AccSamples - 1;
      }
      this.D.AccHeave[this.D.Acc0] = data.NewData.AccelerationHeave.GetValueOrDefault();
      this.D.AccSurge[this.D.Acc0] = data.NewData.AccelerationSurge.GetValueOrDefault();
      this.D.AccSway[this.D.Acc0] = data.NewData.AccelerationSway.GetValueOrDefault();
      if (!data.NewData.AccelerationHeave.HasValue)
      {
        this.D.AccHeave[this.D.Acc0] = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WorldSpeedY");
        this.D.AccHeave[this.D.Acc0] = (this.D.AccHeave[this.D.Acc0] - this.D.WorldSpeedY) * this.D.FPS;
        this.D.WorldSpeedY = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WorldSpeedY");
      }
      this.D.AccHeave2S = (this.D.AccHeave[this.D.Acc0] + this.D.AccHeave[this.D.Acc1]) * 0.5;
      this.D.AccSurge2S = (this.D.AccSurge[this.D.Acc0] + this.D.AccSurge[this.D.Acc1]) * 0.5;
      this.D.AccSway2S = (this.D.AccSway[this.D.Acc0] + this.D.AccSway[this.D.Acc1]) * 0.5;
      this.D.JerkZ = (this.D.AccHeave[this.D.Acc0] - this.D.AccHeave[this.D.Acc1]) * this.D.FPS;
      this.D.JerkY = (this.D.AccSurge[this.D.Acc0] - this.D.AccSurge[this.D.Acc1]) * this.D.FPS;
      this.D.JerkX = (this.D.AccSway[this.D.Acc0] - this.D.AccSway[this.D.Acc1]) * this.D.FPS;
      double num1 = 1.0 / 16.0;
      double accSurgeAvg = this.D.AccSurgeAvg;
      this.D.AccHeaveAvg = 0.0;
      this.D.AccSurgeAvg = 0.0;
      this.D.AccSwayAvg = 0.0;
      for (int index = 0; index < this.D.AccSamples; ++index)
      {
        this.D.AccHeaveAvg += this.D.AccHeave[index];
        this.D.AccSurgeAvg += this.D.AccSurge[index];
        this.D.AccSwayAvg += this.D.AccSway[index];
      }
      this.D.AccHeaveAvg *= num1;
      this.D.AccSurgeAvg *= num1;
      this.D.AccSwayAvg *= num1;
      this.D.JerkYAvg = (this.D.AccSurgeAvg - accSurgeAvg) * this.D.FPS;
      this.D.AccHeaveAbs = Math.Abs(this.D.AccHeave[this.D.Acc0]);
      this.D.InvAccSurgeAvg = this.D.AccSurgeAvg != 0.0 ? 1.0 / this.D.AccSurgeAvg : 0.0;
      this.D.MotionPitch = this.D.MotionPitchOffset + 100.0 * Math.Pow(Math.Abs(this.D.MotionPitchMult * data.NewData.OrientationPitch) * 0.01, 1.0 / this.D.MotionPitchGamma);
      this.D.MotionRoll = this.D.MotionRollOffset + 100.0 * Math.Pow(Math.Abs(this.D.MotionRollMult * data.NewData.OrientationRoll) * 0.01, 1.0 / this.D.MotionRollGamma);
      this.D.MotionYaw = this.D.MotionYawOffset + 100.0 * Math.Pow(Math.Abs(this.D.MotionYawMult * this.D.YawRateAvg) * 0.01, 1.0 / this.D.MotionYawGamma);
      this.D.MotionHeave = this.D.MotionHeaveOffset + 100.0 * Math.Pow(Math.Abs(this.D.MotionHeaveMult * this.D.AccHeave[this.D.Acc0]) * 0.01, 1.0 / this.D.MotionHeaveGamma);
      if (data.NewData.OrientationPitch < 0.0)
        this.D.MotionPitch = -this.D.MotionPitch;
      if (data.NewData.OrientationRoll < 0.0)
        this.D.MotionRoll = -this.D.MotionRoll;
      if (this.D.YawRateAvg < 0.0)
        this.D.MotionYaw = -this.D.MotionYaw;
      if (this.D.AccHeave[this.D.Acc0] < 0.0)
        this.D.MotionHeave = -this.D.MotionHeave;
      this.D.WheelLoadFL = ((100.0 + this.D.AccSurge[this.D.Acc0]) * (100.0 - this.D.AccSway[this.D.Acc0]) * 0.01 - 50.0) * 0.01;
      this.D.WheelLoadFR = ((100.0 + this.D.AccSurge[this.D.Acc0]) * (100.0 + this.D.AccSway[this.D.Acc0]) * 0.01 - 50.0) * 0.01;
      this.D.WheelLoadRL = ((100.0 - this.D.AccSurge[this.D.Acc0]) * (100.0 - this.D.AccSway[this.D.Acc0]) * 0.01 - 50.0) * 0.01;
      this.D.WheelLoadRR = ((100.0 - this.D.AccSurge[this.D.Acc0]) * (100.0 + this.D.AccSway[this.D.Acc0]) * 0.01 - 50.0) * 0.01;
      this.UpdateVehiclePerGame(pluginManager, ref data);
      this.D.Airborne = this.D.AccHeave2S < -2.0 || Math.Abs(data.NewData.OrientationRoll) > 60.0;
      if (this.D.Airborne && this.D.SuspensionFL < 0.1)
      {
        this.D.SlipXFL = 0.0;
        this.D.SlipYFL = 0.0;
      }
      else
      {
        this.D.SlipXFL = this.D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(this.D.SlipXFL, 0.0), 1.0 / (this.D.SlipXGammaBaseMult * this.D.SlipXGamma * this.D.SlipXGammaAll));
        this.D.SlipYFL = this.D.SlipYFL < 0.0 ? this.D.SlipYMultAll * -100.0 * Math.Pow(-this.D.SlipYFL, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll)) : this.D.SlipYMultAll * 100.0 * Math.Pow(this.D.SlipYFL, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll));
      }
      if (this.D.Airborne && this.D.SuspensionFR < 0.1)
      {
        this.D.SlipXFR = 0.0;
        this.D.SlipYFR = 0.0;
      }
      else
      {
        this.D.SlipXFR = this.D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(this.D.SlipXFR, 0.0), 1.0 / (this.D.SlipXGammaBaseMult * this.D.SlipXGamma * this.D.SlipXGammaAll));
        this.D.SlipYFR = this.D.SlipYFR < 0.0 ? this.D.SlipYMultAll * -100.0 * Math.Pow(-this.D.SlipYFR, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll)) : this.D.SlipYMultAll * 100.0 * Math.Pow(this.D.SlipYFR, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll));
      }
      if (this.D.Airborne && this.D.SuspensionRL < 0.1)
      {
        this.D.SlipXRL = 0.0;
        this.D.SlipYRL = 0.0;
      }
      else
      {
        this.D.SlipXRL = this.D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(this.D.SlipXRL, 0.0), 1.0 / (this.D.SlipXGammaBaseMult * this.D.SlipXGamma * this.D.SlipXGammaAll));
        this.D.SlipYRL = this.D.SlipYRL < 0.0 ? this.D.SlipYMultAll * -100.0 * Math.Pow(-this.D.SlipYRL, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll)) : this.D.SlipYMultAll * 100.0 * Math.Pow(this.D.SlipYRL, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll));
      }
      if (this.D.Airborne && this.D.SuspensionRR < 0.1)
      {
        this.D.SlipXRR = 0.0;
        this.D.SlipYRR = 0.0;
      }
      else
      {
        this.D.SlipXRR = this.D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(this.D.SlipXRR, 0.0), 1.0 / (this.D.SlipXGammaBaseMult * this.D.SlipXGamma * this.D.SlipXGammaAll));
        this.D.SlipYRR = this.D.SlipYRR < 0.0 ? this.D.SlipYMultAll * -100.0 * Math.Pow(-this.D.SlipYRR, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll)) : this.D.SlipYMultAll * 100.0 * Math.Pow(this.D.SlipYRR, 1.0 / (this.D.SlipYGammaBaseMult * this.D.SlipYGamma * this.D.SlipYGammaAll));
      }
      this.D.Airborne = this.D.Airborne && this.D.SuspensionAll < 0.1;
      this.D.SlipXAll = (this.D.SlipXFL + this.D.SlipXFR + this.D.SlipXRL + this.D.SlipXRR) * 0.5;
      this.D.SlipYAll = (this.D.SlipYFL + this.D.SlipYFR + this.D.SlipYRL + this.D.SlipYRR) * 0.5;
      this.D.WheelSpinAll = !(this.S.PoweredWheels == "F") ? (!(this.S.PoweredWheels == "R") ? (Math.Max(-this.D.SlipYFL, 0.0) + Math.Max(-this.D.SlipYFR, 0.0) + Math.Max(-this.D.SlipYRL, 0.0) + Math.Max(-this.D.SlipYRR, 0.0)) * 0.25 : (Math.Max(-this.D.SlipYRL, 0.0) + Math.Max(-this.D.SlipYRR, 0.0)) * 0.5) : (Math.Max(-this.D.SlipYFL, 0.0) + Math.Max(-this.D.SlipYFR, 0.0)) * 0.5;
      this.D.WheelLockAll = 0.0;
      if (this.D.SlipYFL > 50.0)
        this.D.WheelLockAll += this.D.SlipYFL - 50.0;
      if (this.D.SlipYFR > 50.0)
        this.D.WheelLockAll += this.D.SlipYFR - 50.0;
      if (this.D.SlipYRL > 50.0)
        this.D.WheelLockAll += this.D.SlipYRL - 50.0;
      if (this.D.SlipYRR > 50.0)
        this.D.WheelLockAll += this.D.SlipYRR - 50.0;
      if (DateTime.Now.Ticks - this.D.ShiftTicks > (long) (this.Settings.DownshiftDurationMs * 10000))
        this.D.Downshift = false;
      if (DateTime.Now.Ticks - this.D.ShiftTicks > (long) (this.Settings.UpshiftDurationMs * 10000))
        this.D.Upshift = false;
      DateTime now;
      if (data.OldData.Gear != data.NewData.Gear)
      {
        if (this.D.Gear != 0)
          this.D.GearPrevious = this.D.Gear;
        this.D.Gear = !(data.NewData.Gear == "N") ? (!(data.NewData.Gear == "R") ? Convert.ToInt32(data.NewData.Gear) : -1) : 0;
        if (this.D.Gear != 0)
        {
          if (this.D.Gear < this.D.GearPrevious)
          {
            this.D.Downshift = true;
            SimData d = this.D;
            now = DateTime.Now;
            long ticks = now.Ticks;
            d.ShiftTicks = ticks;
          }
          else if (this.D.Gear > this.D.GearPrevious)
          {
            this.D.Upshift = true;
            SimData d = this.D;
            now = DateTime.Now;
            long ticks = now.Ticks;
            d.ShiftTicks = ticks;
          }
        }
      }
      this.D.ABSPauseInterval = this.D.SlipYAll <= 0.0 ? (long) (1166667.0 - 666667.0 * ((data.NewData.SpeedKmh - 20.0) * 0.003333333).Clamp<double>(0.0, 1.0)) : (long) (1250000.0 - 666667.0 * this.D.SlipYAll.Clamp<double>(0.0, 1.0));
      this.D.ABSPulseInterval = 166666L * (long) this.Settings.ABSPulseLength;
      if (this.D.ABSActive)
      {
        if (this.D.ABSTicks <= 0L)
        {
          SimData d = this.D;
          now = DateTime.Now;
          long ticks = now.Ticks;
          d.ABSTicks = ticks;
        }
        now = DateTime.Now;
        if (now.Ticks - this.D.ABSTicks < this.D.ABSPulseInterval)
        {
          this.D.ABSPulse = 100.0;
        }
        else
        {
          now = DateTime.Now;
          if (now.Ticks - this.D.ABSTicks < this.D.ABSPauseInterval)
          {
            this.D.ABSPulse = 0.0;
          }
          else
          {
            this.D.ABSPulse = 100.0;
            SimData d = this.D;
            now = DateTime.Now;
            long ticks = now.Ticks;
            d.ABSTicks = ticks;
          }
        }
      }
      else
      {
        this.D.ABSPulse = 0.0;
        this.D.ABSTicks = -1L;
      }
      this.D.SuspensionAccFLP = this.D.SuspensionAccFL;
      this.D.SuspensionAccFRP = this.D.SuspensionAccFR;
      this.D.SuspensionAccRLP = this.D.SuspensionAccRL;
      this.D.SuspensionAccRRP = this.D.SuspensionAccRR;
      this.D.SuspensionAccFL = (this.D.SuspensionVelFL - this.D.SuspensionVelFLP) * this.D.FPS;
      this.D.SuspensionAccFR = (this.D.SuspensionVelFR - this.D.SuspensionVelFRP) * this.D.FPS;
      this.D.SuspensionAccRL = (this.D.SuspensionVelRL - this.D.SuspensionVelRLP) * this.D.FPS;
      this.D.SuspensionAccRR = (this.D.SuspensionVelRR - this.D.SuspensionVelRRP) * this.D.FPS;
      this.D.SuspensionFL = 10.0 * this.D.SuspensionMult * this.D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(this.D.SuspensionVelFL, 0.0) * 0.01, 1.0 / (this.D.SuspensionGamma * this.D.SuspensionGammaAll));
      this.D.SuspensionFR = 10.0 * this.D.SuspensionMult * this.D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(this.D.SuspensionVelFR, 0.0) * 0.01, 1.0 / (this.D.SuspensionGamma * this.D.SuspensionGammaAll));
      this.D.SuspensionRL = 10.0 * this.D.SuspensionMult * this.D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(this.D.SuspensionVelRL, 0.0) * 0.01, 1.0 / (this.D.SuspensionGamma * this.D.SuspensionGammaAll));
      this.D.SuspensionRR = 10.0 * this.D.SuspensionMult * this.D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(this.D.SuspensionVelRR, 0.0) * 0.01, 1.0 / (this.D.SuspensionGamma * this.D.SuspensionGammaAll));
      this.D.SuspensionFront = this.D.SuspensionFL + this.D.SuspensionFR;
      this.D.SuspensionRear = this.D.SuspensionRL + this.D.SuspensionRR;
      this.D.SuspensionLeft = this.D.SuspensionFL + this.D.SuspensionRL;
      this.D.SuspensionRight = this.D.SuspensionFR + this.D.SuspensionRR;
      this.D.SuspensionAll = (this.D.SuspensionFL + this.D.SuspensionFR + this.D.SuspensionRL + this.D.SuspensionRR) * 0.5;
      this.D.SuspensionAccAll = (this.D.SuspensionAccFL + this.D.SuspensionAccFR + this.D.SuspensionAccRL + this.D.SuspensionAccRR) * 0.5;
      if (this.D.CarInitCount < 10 && SimHapticsPlugin.FrameCountTicks % 2000000L <= 150000L)
      {
        this.D.SuspensionFL *= 0.1 * (double) this.D.CarInitCount;
        this.D.SuspensionFR *= 0.1 * (double) this.D.CarInitCount;
        this.D.SuspensionRL *= 0.1 * (double) this.D.CarInitCount;
        this.D.SuspensionRR *= 0.1 * (double) this.D.CarInitCount;
        ++this.D.CarInitCount;
      }
      this.D.SuspensionFreq = data.NewData.SpeedKmh * (3.0 / 16.0);
      double num2 = 94.0 + 0.4 * this.D.SpeedMs;
      double num3 = 76.0 + 0.45 * this.D.SpeedMs;
      double num4 = 60.0 + 0.5 * this.D.SpeedMs;
      double num5 = 46.0 + 0.55 * this.D.SpeedMs;
      double num6 = 34.0 + 0.6 * this.D.SpeedMs;
      double num7 = 24.0 + 0.65 * this.D.SpeedMs;
      double num8 = 16.0 + 0.7 * this.D.SpeedMs;
      double num9 = 10.0 + 0.75 * this.D.SpeedMs;
      double num10 = 0.55 + 1.8 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num2) / (num2 * num2);
      double num11 = 0.5 + 2.0 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num3) / (num3 * num3);
      double num12 = 0.45 + 2.2 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num4) / (num4 * num4);
      double num13 = 0.4 + 2.4 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num5) / (num5 * num5);
      double num14 = 0.5 + 2.0 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num6) / (num6 * num6);
      double num15 = 0.6 + 1.6 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num7) / (num7 * num7);
      double num16 = 0.7 + 1.2 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num8) / (num8 * num8);
      double num17 = 0.8 + 0.8 * this.D.AccHeaveAbs * (this.D.AccHeaveAbs + num9) / (num9 * num9);
      double num18 = this.D.RumbleMult * this.D.RumbleMultAll * (0.6 + this.D.SpeedMs * (90.0 - this.D.SpeedMs) * 0.0002);
      if (this.D.SuspensionFreq < 30.0)
      {
        if (this.D.SuspensionFreq < 20.0)
        {
          if (this.D.SuspensionFreq < 15.0)
          {
            if (this.D.SuspensionFreq < 10.0)
            {
              if (this.D.SuspensionFreq < 7.5)
              {
                if (this.D.SuspensionFreq < 3.75)
                {
                  this.D.SuspensionFreq *= 4.0;
                  this.D.SuspensionFreqRa = this.D.SuspensionFreq * 0.715;
                  this.D.SuspensionFreqRb = this.D.SuspensionFreq * 1.0;
                  this.D.SuspensionFreqRc = this.D.SuspensionFreq * 1.43;
                  this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 2.0;
                  this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 2.86;
                  this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 4.0;
                  this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 5.72;
                  this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 8.0;
                  this.D.SuspensionMultRa = num10 * 0.5;
                  this.D.SuspensionMultRb = num11 * 1.0;
                  this.D.SuspensionMultRc = num12 * 0.5;
                  this.D.SuspensionMultR1 = num13 * 0.8;
                  this.D.SuspensionMultR2 = num14 * 0.25;
                  this.D.SuspensionMultR3 = num15 * 0.6;
                  this.D.SuspensionMultR4 = num16 * 0.125;
                  this.D.SuspensionMultR5 = num17 * 0.4;
                  this.D.SuspensionRumbleMultRa = num18 * 0.0;
                  this.D.SuspensionRumbleMultRb = num18 * 2.0;
                  this.D.SuspensionRumbleMultRc = num18 * 0.0;
                  this.D.SuspensionRumbleMultR1 = num18 * 1.5;
                  this.D.SuspensionRumbleMultR2 = num18 * 0.0;
                  this.D.SuspensionRumbleMultR3 = num18 * 1.0;
                  this.D.SuspensionRumbleMultR4 = num18 * 0.0;
                  this.D.SuspensionRumbleMultR5 = num18 * 0.5;
                }
                else
                {
                  this.D.SuspensionFreq *= 2.0;
                  this.D.SuspensionFreqRa = this.D.SuspensionFreq * 0.715;
                  this.D.SuspensionFreqRb = this.D.SuspensionFreq * 1.0;
                  this.D.SuspensionFreqRc = this.D.SuspensionFreq * 1.43;
                  this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 2.0;
                  this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 2.86;
                  this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 4.0;
                  this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 5.72;
                  this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 8.0;
                  this.D.SuspensionMultRa = num10 * 0.5;
                  this.D.SuspensionMultRb = num11 * 1.0;
                  this.D.SuspensionMultRc = num12 * 0.5;
                  this.D.SuspensionMultR1 = num13 * 0.8;
                  this.D.SuspensionMultR2 = num14 * 0.25;
                  this.D.SuspensionMultR3 = num15 * 0.6;
                  this.D.SuspensionMultR4 = num16 * 0.125;
                  this.D.SuspensionMultR5 = num17 * 0.4;
                  this.D.SuspensionRumbleMultRa = num18 * 0.0;
                  this.D.SuspensionRumbleMultRb = num18 * 2.0;
                  this.D.SuspensionRumbleMultRc = num18 * 0.0;
                  this.D.SuspensionRumbleMultR1 = num18 * 1.5;
                  this.D.SuspensionRumbleMultR2 = num18 * 0.0;
                  this.D.SuspensionRumbleMultR3 = num18 * 1.0;
                  this.D.SuspensionRumbleMultR4 = num18 * 0.0;
                  this.D.SuspensionRumbleMultR5 = num18 * 0.5;
                }
              }
              else
              {
                this.D.SuspensionFreqRa = this.D.SuspensionFreq * 1.0;
                this.D.SuspensionFreqRb = this.D.SuspensionFreq * 1.43;
                this.D.SuspensionFreqRc = this.D.SuspensionFreq * 2.0;
                this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 2.86;
                this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 4.0;
                this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 5.72;
                this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 8.0;
                this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 11.44;
                this.D.SuspensionMultRa = num10 * 1.0;
                this.D.SuspensionMultRb = num11 * 0.5;
                this.D.SuspensionMultRc = num12 * 0.8;
                this.D.SuspensionMultR1 = num13 * 0.25;
                this.D.SuspensionMultR2 = num14 * 0.6;
                this.D.SuspensionMultR3 = num15 * 0.125;
                this.D.SuspensionMultR4 = num16 * 0.4;
                this.D.SuspensionMultR5 = num17 * (1.0 / 16.0);
                this.D.SuspensionRumbleMultRa = num18 * 2.0;
                this.D.SuspensionRumbleMultRb = num18 * 0.0;
                this.D.SuspensionRumbleMultRc = num18 * 1.5;
                this.D.SuspensionRumbleMultR1 = num18 * 0.0;
                this.D.SuspensionRumbleMultR2 = num18 * 1.0;
                this.D.SuspensionRumbleMultR3 = num18 * 0.0;
                this.D.SuspensionRumbleMultR4 = num18 * 0.5;
                this.D.SuspensionRumbleMultR5 = num18 * 0.0;
              }
            }
            else
            {
              this.D.SuspensionFreqRa = this.D.SuspensionFreq * 0.715;
              this.D.SuspensionFreqRb = this.D.SuspensionFreq * 1.0;
              this.D.SuspensionFreqRc = this.D.SuspensionFreq * 1.43;
              this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 2.0;
              this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 2.86;
              this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 4.0;
              this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 5.72;
              this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 8.0;
              this.D.SuspensionMultRa = num10 * 0.5;
              this.D.SuspensionMultRb = num11 * 1.0;
              this.D.SuspensionMultRc = num12 * 0.5;
              this.D.SuspensionMultR1 = num13 * 0.8;
              this.D.SuspensionMultR2 = num14 * 0.25;
              this.D.SuspensionMultR3 = num15 * 0.6;
              this.D.SuspensionMultR4 = num16 * 0.125;
              this.D.SuspensionMultR5 = num17 * 0.4;
              this.D.SuspensionRumbleMultRa = num18 * 0.0;
              this.D.SuspensionRumbleMultRb = num18 * 2.0;
              this.D.SuspensionRumbleMultRc = num18 * 0.0;
              this.D.SuspensionRumbleMultR1 = num18 * 1.5;
              this.D.SuspensionRumbleMultR2 = num18 * 0.0;
              this.D.SuspensionRumbleMultR3 = num18 * 1.0;
              this.D.SuspensionRumbleMultR4 = num18 * 0.0;
              this.D.SuspensionRumbleMultR5 = num18 * 0.5;
            }
          }
          else
          {
            this.D.SuspensionFreqRa = this.D.SuspensionFreq * 0.5;
            this.D.SuspensionFreqRb = this.D.SuspensionFreq * 0.715;
            this.D.SuspensionFreqRc = this.D.SuspensionFreq * 1.0;
            this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 1.43;
            this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 2.0;
            this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 2.86;
            this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 4.0;
            this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 5.72;
            this.D.SuspensionMultRa = num10 * 0.8;
            this.D.SuspensionMultRb = num11 * 0.5;
            this.D.SuspensionMultRc = num12 * 1.0;
            this.D.SuspensionMultR1 = num13 * 0.5;
            this.D.SuspensionMultR2 = num14 * 0.8;
            this.D.SuspensionMultR3 = num15 * 0.25;
            this.D.SuspensionMultR4 = num16 * 0.6;
            this.D.SuspensionMultR5 = num17 * 0.125;
            this.D.SuspensionRumbleMultRa = num18 * 1.5;
            this.D.SuspensionRumbleMultRb = num18 * 0.0;
            this.D.SuspensionRumbleMultRc = num18 * 2.0;
            this.D.SuspensionRumbleMultR1 = num18 * 0.0;
            this.D.SuspensionRumbleMultR2 = num18 * 1.5;
            this.D.SuspensionRumbleMultR3 = num18 * 0.0;
            this.D.SuspensionRumbleMultR4 = num18 * 1.0;
            this.D.SuspensionRumbleMultR5 = num18 * 0.0;
          }
        }
        else
        {
          this.D.SuspensionFreqRa = this.D.SuspensionFreq * (143.0 / 400.0);
          this.D.SuspensionFreqRb = this.D.SuspensionFreq * 0.5;
          this.D.SuspensionFreqRc = this.D.SuspensionFreq * 0.715;
          this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 1.0;
          this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 1.43;
          this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 2.0;
          this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 2.86;
          this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 4.0;
          this.D.SuspensionMultRa = num10 * 0.25;
          this.D.SuspensionMultRb = num11 * 0.8;
          this.D.SuspensionMultRc = num12 * 0.5;
          this.D.SuspensionMultR1 = num13 * 1.0;
          this.D.SuspensionMultR2 = num14 * 0.5;
          this.D.SuspensionMultR3 = num15 * 0.8;
          this.D.SuspensionMultR4 = num16 * 0.25;
          this.D.SuspensionMultR5 = num17 * 0.6;
          this.D.SuspensionRumbleMultRa = num18 * 0.0;
          this.D.SuspensionRumbleMultRb = num18 * 1.5;
          this.D.SuspensionRumbleMultRc = num18 * 0.0;
          this.D.SuspensionRumbleMultR1 = num18 * 2.0;
          this.D.SuspensionRumbleMultR2 = num18 * 0.0;
          this.D.SuspensionRumbleMultR3 = num18 * 1.5;
          this.D.SuspensionRumbleMultR4 = num18 * 0.0;
          this.D.SuspensionRumbleMultR5 = num18 * 1.0;
        }
      }
      else if (this.D.SuspensionFreq > 40.0)
      {
        if (this.D.SuspensionFreq > 60.0)
        {
          if (this.D.SuspensionFreq > 80.0)
          {
            if (this.D.SuspensionFreq > 120.0)
            {
              this.D.SuspensionFreqRa = this.D.SuspensionFreq * (1.0 / 16.0);
              this.D.SuspensionFreqRb = this.D.SuspensionFreq * 0.089375;
              this.D.SuspensionFreqRc = this.D.SuspensionFreq * 0.125;
              this.D.SuspensionFreqR1 = this.D.SuspensionFreq * (143.0 / 800.0);
              this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 0.25;
              this.D.SuspensionFreqR3 = this.D.SuspensionFreq * (143.0 / 400.0);
              this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 0.5;
              this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 0.715;
              this.D.SuspensionMultRa = num10 * 0.2;
              this.D.SuspensionMultRb = num11 * (1.0 / 16.0);
              this.D.SuspensionMultRc = num12 * 0.4;
              this.D.SuspensionMultR1 = num13 * 0.125;
              this.D.SuspensionMultR2 = num14 * 0.6;
              this.D.SuspensionMultR3 = num15 * 0.25;
              this.D.SuspensionMultR4 = num16 * 0.8;
              this.D.SuspensionMultR5 = num17 * 0.5;
              this.D.SuspensionRumbleMultRa = num18 * 0.3;
              this.D.SuspensionRumbleMultRb = num18 * 0.0;
              this.D.SuspensionRumbleMultRc = num18 * 0.5;
              this.D.SuspensionRumbleMultR1 = num18 * 0.0;
              this.D.SuspensionRumbleMultR2 = num18 * 1.0;
              this.D.SuspensionRumbleMultR3 = num18 * 0.0;
              this.D.SuspensionRumbleMultR4 = num18 * 1.5;
              this.D.SuspensionRumbleMultR5 = num18 * 0.0;
            }
            else
            {
              this.D.SuspensionFreqRa = this.D.SuspensionFreq * 0.089375;
              this.D.SuspensionFreqRb = this.D.SuspensionFreq * 0.125;
              this.D.SuspensionFreqRc = this.D.SuspensionFreq * (143.0 / 800.0);
              this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 0.25;
              this.D.SuspensionFreqR2 = this.D.SuspensionFreq * (143.0 / 400.0);
              this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 0.5;
              this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 0.715;
              this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 1.0;
              this.D.SuspensionMultRa = num10 * (1.0 / 16.0);
              this.D.SuspensionMultRb = num11 * 0.4;
              this.D.SuspensionMultRc = num12 * 0.125;
              this.D.SuspensionMultR1 = num13 * 0.6;
              this.D.SuspensionMultR2 = num14 * 0.25;
              this.D.SuspensionMultR3 = num15 * 0.8;
              this.D.SuspensionMultR4 = num16 * 0.5;
              this.D.SuspensionMultR5 = num17 * 1.0;
              this.D.SuspensionRumbleMultRa = num18 * 0.0;
              this.D.SuspensionRumbleMultRb = num18 * 0.5;
              this.D.SuspensionRumbleMultRc = num18 * 0.0;
              this.D.SuspensionRumbleMultR1 = num18 * 1.0;
              this.D.SuspensionRumbleMultR2 = num18 * 0.0;
              this.D.SuspensionRumbleMultR3 = num18 * 1.5;
              this.D.SuspensionRumbleMultR4 = num18 * 0.0;
              this.D.SuspensionRumbleMultR5 = num18 * 2.0;
            }
          }
          else
          {
            this.D.SuspensionFreqRa = this.D.SuspensionFreq * 0.125;
            this.D.SuspensionFreqRb = this.D.SuspensionFreq * (143.0 / 800.0);
            this.D.SuspensionFreqRc = this.D.SuspensionFreq * 0.25;
            this.D.SuspensionFreqR1 = this.D.SuspensionFreq * (143.0 / 400.0);
            this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 0.5;
            this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 0.715;
            this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 1.0;
            this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 1.43;
            this.D.SuspensionMultRa = num10 * 0.4;
            this.D.SuspensionMultRb = num11 * 0.125;
            this.D.SuspensionMultRc = num12 * 0.6;
            this.D.SuspensionMultR1 = num13 * 0.25;
            this.D.SuspensionMultR2 = num14 * 0.8;
            this.D.SuspensionMultR3 = num15 * 0.5;
            this.D.SuspensionMultR4 = num16 * 1.0;
            this.D.SuspensionMultR5 = num17 * 0.5;
            this.D.SuspensionRumbleMultRa = num18 * 0.5;
            this.D.SuspensionRumbleMultRb = num18 * 0.0;
            this.D.SuspensionRumbleMultRc = num18 * 1.0;
            this.D.SuspensionRumbleMultR1 = num18 * 0.0;
            this.D.SuspensionRumbleMultR2 = num18 * 1.5;
            this.D.SuspensionRumbleMultR3 = num18 * 0.0;
            this.D.SuspensionRumbleMultR4 = num18 * 2.0;
            this.D.SuspensionRumbleMultR5 = num18 * 0.0;
          }
        }
        else
        {
          this.D.SuspensionFreqRa = this.D.SuspensionFreq * (143.0 / 800.0);
          this.D.SuspensionFreqRb = this.D.SuspensionFreq * 0.25;
          this.D.SuspensionFreqRc = this.D.SuspensionFreq * (143.0 / 400.0);
          this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 0.5;
          this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 0.715;
          this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 1.0;
          this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 1.43;
          this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 2.0;
          this.D.SuspensionMultRa = num10 * 0.125;
          this.D.SuspensionMultRb = num11 * 0.6;
          this.D.SuspensionMultRc = num12 * 0.25;
          this.D.SuspensionMultR1 = num13 * 0.8;
          this.D.SuspensionMultR2 = num14 * 0.5;
          this.D.SuspensionMultR3 = num15 * 1.0;
          this.D.SuspensionMultR4 = num16 * 0.5;
          this.D.SuspensionMultR5 = num17 * 0.8;
          this.D.SuspensionRumbleMultRa = num18 * 0.0;
          this.D.SuspensionRumbleMultRb = num18 * 1.0;
          this.D.SuspensionRumbleMultRc = num18 * 0.0;
          this.D.SuspensionRumbleMultR1 = num18 * 1.5;
          this.D.SuspensionRumbleMultR2 = num18 * 0.0;
          this.D.SuspensionRumbleMultR3 = num18 * 2.0;
          this.D.SuspensionRumbleMultR4 = num18 * 0.0;
          this.D.SuspensionRumbleMultR5 = num18 * 1.5;
        }
      }
      else
      {
        this.D.SuspensionFreqRa = this.D.SuspensionFreq * 0.25;
        this.D.SuspensionFreqRb = this.D.SuspensionFreq * (143.0 / 400.0);
        this.D.SuspensionFreqRc = this.D.SuspensionFreq * 0.5;
        this.D.SuspensionFreqR1 = this.D.SuspensionFreq * 0.715;
        this.D.SuspensionFreqR2 = this.D.SuspensionFreq * 1.0;
        this.D.SuspensionFreqR3 = this.D.SuspensionFreq * 1.43;
        this.D.SuspensionFreqR4 = this.D.SuspensionFreq * 2.0;
        this.D.SuspensionFreqR5 = this.D.SuspensionFreq * 2.86;
        this.D.SuspensionMultRa = num10 * 0.6;
        this.D.SuspensionMultRb = num11 * 0.25;
        this.D.SuspensionMultRc = num12 * 0.8;
        this.D.SuspensionMultR1 = num13 * 0.5;
        this.D.SuspensionMultR2 = num14 * 1.0;
        this.D.SuspensionMultR3 = num15 * 0.5;
        this.D.SuspensionMultR4 = num16 * 0.8;
        this.D.SuspensionMultR5 = num17 * 0.25;
        this.D.SuspensionRumbleMultRa = num18 * 1.0;
        this.D.SuspensionRumbleMultRb = num18 * 0.0;
        this.D.SuspensionRumbleMultRc = num18 * 1.5;
        this.D.SuspensionRumbleMultR1 = num18 * 0.0;
        this.D.SuspensionRumbleMultR2 = num18 * 2.0;
        this.D.SuspensionRumbleMultR3 = num18 * 0.0;
        this.D.SuspensionRumbleMultR4 = num18 * 1.5;
        this.D.SuspensionRumbleMultR5 = num18 * 0.0;
      }
      this.D.EngineLoad = data.NewData.CarSettings_CurrentDisplayedRPMPercent * 0.5;
      this.D.EngineLoad += data.NewData.SpeedKmh * data.NewData.SpeedKmh * 0.0003;
      this.D.EngineLoad += data.NewData.SpeedKmh * 0.02;
      if (Math.Abs(this.D.SuspensionAccAll) > 0.5)
        this.D.EngineLoad += 200.0 * Math.Sin(data.NewData.OrientationPitch * 0.0174533);
      this.D.EngineLoad -= this.D.EngineLoad * (1.0 - this.D.MixPower) * 0.5;
      this.D.EngineLoad *= data.NewData.Throttle * 0.01 * 0.01;
      if (this.D.IdleSampleCount < 20 && SimHapticsPlugin.FrameCountTicks % 2500000L <= 150000L)
      {
        double num19 = Math.Abs(data.OldData.Rpms - data.NewData.Rpms) * this.D.FPS;
        if (data.NewData.Rpms > data.NewData.MaxRpm * 0.1 && data.NewData.Rpms <= this.D.IdleRPM + 20.0 && num19 < 40.0)
        {
          this.D.IdleRPM = (this.D.IdleRPM + data.NewData.Rpms) * 0.5;
          ++this.D.IdleSampleCount;
          double num20 = this.D.IdleRPM * 0.008333333;
          this.D.FrequencyMultiplier = num20 >= 5.0 ? (num20 >= 10.0 ? (num20 <= 20.0 ? (num20 <= 40.0 ? 1.0 : 0.25) : 0.5) : 2.0) : 4.0;
        }
      }
      if (SimHapticsPlugin.FrameCountTicks % 5000000L <= 150000L)
      {
        this.SetRPMIntervals();
        this.SetRPMMix();
      }
      this.D.FreqHarmonic = data.NewData.Rpms * 0.008333333;
      this.D.FreqOctave = this.D.FreqHarmonic * (1.0 + this.D.IntervalOctave * 0.08333333);
      this.D.FreqLFEAdaptive = this.D.FreqHarmonic * this.D.FrequencyMultiplier;
      this.D.FreqIntervalA1 = this.D.FreqHarmonic * (1.0 + this.D.IntervalA * 0.08333333);
      this.D.FreqIntervalA2 = this.D.FreqHarmonic * 0.5 * (1.0 + this.D.IntervalA * 0.08333333);
      this.D.FreqPeakA1 = this.D.FreqHarmonic * (1.0 + this.D.IntervalPeakA * 0.08333333);
      this.D.FreqPeakB1 = this.D.FreqHarmonic * (1.0 + this.D.IntervalPeakB * 0.08333333);
      this.D.FreqPeakA2 = this.D.FreqHarmonic * 0.5 * (1.0 + this.D.IntervalPeakA * 0.08333333);
      this.D.FreqPeakB2 = this.D.FreqHarmonic * 0.5 * (1.0 + this.D.IntervalPeakB * 0.08333333);
      double num21 = 1.0;
      double num22 = 1.0;
      if (this.D.Gear > 0)
      {
        num21 -= this.D.AccSurge2S.Clamp<double>(0.0, 15.0) * 0.01;
        if (this.D.Accelerator < 20.0 && this.D.AccSurgeAvg < 0.0)
          num22 += Math.Max(Math.Max(this.D.RPMPercent - this.D.IdlePercent * (1.0 + (double) this.D.Gear * 0.2), 0.0) * (0.2 + 0.6 * this.D.MixDisplacement) - this.D.Accelerator * 0.05 * (0.2 + 0.6 * this.D.MixDisplacement), 0.0);
      }
      this.D.Gain1H = this.D.FreqHarmonic >= 25.0 ? (this.D.FreqHarmonic >= 40.0 ? (this.D.FreqHarmonic >= 65.0 ? (this.D.FreqHarmonic >= 95.0 ? (this.D.FreqHarmonic >= 125.0 ? 75.0 - (this.D.FreqHarmonic - 125.0) : 95.0 - (this.D.FreqHarmonic - 95.0) * 0.667) : 65.0 + (this.D.FreqHarmonic - 65.0) * 1.0) : 52.5 + (this.D.FreqHarmonic - 40.0) * 0.5) : 40.0 + (this.D.FreqHarmonic - 25.0) * 0.834) : 30.0 + (this.D.FreqHarmonic - 15.0) * 1.0;
      this.D.Gain1H = Math.Max(this.D.Gain1H, 0.0) * num21 * num22 * (0.8 + 0.2 * this.D.MixPower + 0.2 * this.D.MixCylinder);
      this.D.Gain1H = Math.Floor(this.D.Gain1H.Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.Gain1H2 = this.D.FreqHarmonic >= 25.0 ? (this.D.FreqHarmonic >= 40.0 ? (this.D.FreqHarmonic >= 65.0 ? (this.D.FreqHarmonic >= 95.0 ? (this.D.FreqHarmonic >= 125.0 ? 75.0 - (this.D.FreqHarmonic - 125.0) : 95.0 - (this.D.FreqHarmonic - 95.0) * 0.667) : 65.0 + (this.D.FreqHarmonic - 65.0) * 1.0) : 52.5 + (this.D.FreqHarmonic - 40.0) * 0.5) : 40.0 + (this.D.FreqHarmonic - 25.0) * 0.834) : 30.0 + (this.D.FreqHarmonic - 15.0) * 1.0;
      this.D.Gain1H2 = Math.Max(this.D.Gain1H2, 0.0) * num21 * num22 * (0.8 + 0.1 * this.D.MixDisplacement + 0.3 * this.D.MixCylinder);
      this.D.Gain1H2 = Math.Floor(this.D.Gain1H2.Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.Gain2H = this.D.FreqHarmonic >= 25.0 ? (this.D.FreqHarmonic >= 40.0 ? (this.D.FreqHarmonic >= 65.0 ? (this.D.FreqHarmonic >= 95.0 ? (this.D.FreqHarmonic >= 125.0 ? 75.0 - (this.D.FreqHarmonic - 125.0) : 95.0 - (this.D.FreqHarmonic - 95.0) * 0.667) : 65.0 + (this.D.FreqHarmonic - 65.0) * 1.0) : 52.5 + (this.D.FreqHarmonic - 40.0) * 0.5) : 40.0 + (this.D.FreqHarmonic - 25.0) * 0.834) : 30.0 + (this.D.FreqHarmonic - 15.0) * 1.0;
      this.D.Gain2H = Math.Max(this.D.Gain2H, 0.0) * num21 * num22 * (0.8 + 0.3 * this.D.MixPower + 0.1 * this.D.MixCylinder);
      this.D.Gain2H = Math.Floor(this.D.Gain2H.Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.Gain4H = this.D.FreqHarmonic >= 25.0 ? (this.D.FreqHarmonic >= 40.0 ? (this.D.FreqHarmonic >= 65.0 ? (this.D.FreqHarmonic >= 95.0 ? (this.D.FreqHarmonic >= 125.0 ? 75.0 - (this.D.FreqHarmonic - 125.0) : 95.0 - (this.D.FreqHarmonic - 95.0) * 0.667) : 66.0 + (this.D.FreqHarmonic - 65.0) * 1.0) : 52.5 + (this.D.FreqHarmonic - 40.0) * 0.5) : 40.0 + (this.D.FreqHarmonic - 25.0) * 0.834) : 30.0 + (this.D.FreqHarmonic - 15.0) * 1.0;
      this.D.Gain4H = Math.Max(this.D.Gain4H, 0.0) * num21 * num22 * (0.8 + 0.2 * this.D.MixPower + 0.2 * this.D.MixDisplacement);
      this.D.Gain4H = Math.Floor(this.D.Gain4H.Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainOctave = this.D.FreqOctave >= 55.0 ? (this.D.FreqOctave >= 80.0 ? 75.0 - (this.D.FreqOctave - 80.0) * 0.75 : 30.0 + (this.D.FreqOctave - 55.0) * 1.8) : (this.D.FreqOctave - 30.0) * 1.2;
      this.D.GainOctave = Math.Max(this.D.GainOctave, 0.0) * num21 * (0.3 * this.D.MixPower + 0.3 * this.D.MixCylinder + 0.6 * this.D.EngineLoad);
      this.D.GainOctave = Math.Floor(this.D.GainOctave.Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainIntervalA1 = this.D.FreqIntervalA1 >= 70.0 ? (this.D.FreqIntervalA1 >= 85.0 ? 75.0 - (this.D.FreqIntervalA1 - 85.0) * 0.85 : 45.0 + (this.D.FreqIntervalA1 - 70.0) * 2.0) : (this.D.FreqIntervalA1 - 40.0) * 1.5;
      this.D.GainIntervalA1 = Math.Max(this.D.GainIntervalA1, 0.0) * num21 * (0.2 * this.D.MixPower + 1.0 * this.D.EngineLoad);
      this.D.GainIntervalA1 = Math.Floor(this.D.GainIntervalA1.Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainIntervalA2 = this.D.FreqIntervalA2 >= 70.0 ? (this.D.FreqIntervalA2 >= 85.0 ? 75.0 - (this.D.FreqIntervalA2 - 85.0) * 0.85 : 45.0 + (this.D.FreqIntervalA2 - 70.0) * 2.0) : (this.D.FreqIntervalA2 - 40.0) * 1.5;
      this.D.GainIntervalA2 = Math.Max(this.D.GainIntervalA2, 0.0) * num21 * (0.1 * this.D.MixPower + 0.3 * this.D.MixCylinder + 0.8 * this.D.EngineLoad);
      this.D.GainIntervalA2 = Math.Floor(this.D.GainIntervalA2.Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.PeakA1Start = this.D.RedlinePercent * (0.96 + this.D.GearInterval * (double) this.D.Gear * 0.04);
      this.D.PeakB1Start = this.D.RedlinePercent * (0.92 + this.D.GearInterval * (double) this.D.Gear * 0.04);
      this.D.PeakA2Start = this.D.RedlinePercent * (0.9 + this.D.MixPower * this.D.GearInterval * (double) this.D.Gear * 0.06);
      this.D.PeakB2Start = this.D.RedlinePercent * (0.98 - this.D.MixTorque * 0.08);
      this.D.PeakA1Modifier = ((this.D.RPMPercent - this.D.PeakA1Start) / (this.D.RedlinePercent - this.D.PeakA1Start + (1.0 - this.D.RedlinePercent) * (0.75 + this.D.MixCylinder * 0.75))).Clamp<double>(0.0, 1.0);
      this.D.PeakB1Modifier = ((this.D.RPMPercent - this.D.PeakB1Start) / (this.D.RedlinePercent - this.D.PeakB1Start + (1.0 - this.D.RedlinePercent) * (0.0 + this.D.MixCylinder))).Clamp<double>(0.0, 1.0);
      this.D.PeakA2Modifier = ((this.D.RPMPercent - this.D.PeakA2Start) / (this.D.RedlinePercent - this.D.PeakA2Start)).Clamp<double>(0.0, 1.0);
      this.D.PeakB2Modifier = ((this.D.RPMPercent - this.D.PeakB2Start) / (this.D.RedlinePercent - this.D.PeakB2Start + (1.0 - this.D.RedlinePercent) * (1.0 - this.D.MixDisplacement))).Clamp<double>(0.0, 1.0);
      this.D.GainPeakA1 = this.D.FreqPeakA1 >= 55.0 ? (this.D.FreqPeakA1 >= 75.0 ? (this.D.FreqPeakA1 >= 105.0 ? 90.0 - (this.D.FreqPeakA1 - 105.0) * 0.75 : 60.0 + (this.D.FreqPeakA1 - 75.0) * 1.0) : 30.0 + (this.D.FreqPeakA1 - 55.0) * 1.5) : (this.D.FreqPeakA1 - 45.0) * 3.0;
      this.D.GainPeakA1 = Math.Max(this.D.GainPeakA1, 0.0) * (0.9 + 0.1 * this.D.MixPower + 0.1 * this.D.MixCylinder + 0.1 * this.D.MixTorque);
      this.D.GainPeakA1Front = Math.Floor((this.D.PeakA1Modifier * this.D.GainPeakA1 * (0.9 + 0.3 * this.D.MixFront)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakA1Rear = Math.Floor((this.D.PeakA1Modifier * this.D.GainPeakA1 * (0.9 + 0.3 * this.D.MixRear)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakA1 = Math.Floor((this.D.PeakA1Modifier * this.D.GainPeakA1 * (0.9 + 0.3 * this.D.MixMiddle)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakB1 = this.D.FreqPeakB1 >= 55.0 ? (this.D.FreqPeakB1 >= 75.0 ? (this.D.FreqPeakB1 >= 105.0 ? 90.0 - (this.D.FreqPeakB1 - 105.0) * 0.75 : 60.0 + (this.D.FreqPeakB1 - 75.0) * 1.0) : 30.0 + (this.D.FreqPeakB1 - 55.0) * 1.5) : (this.D.FreqPeakB1 - 45.0) * 3.0;
      this.D.GainPeakB1 = Math.Max(this.D.GainPeakB1, 0.0) * (0.9 + 0.1 * this.D.MixPower + 0.1 * this.D.MixCylinder + 0.1 * this.D.MixTorque);
      this.D.GainPeakB1Front = Math.Floor((this.D.PeakB1Modifier * this.D.GainPeakB1 * (0.9 + 0.3 * this.D.MixFront)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakB1Rear = Math.Floor((this.D.PeakB1Modifier * this.D.GainPeakB1 * (0.9 + 0.3 * this.D.MixRear)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakB1 = Math.Floor((this.D.PeakB1Modifier * this.D.GainPeakB1 * (0.9 + 0.3 * this.D.MixMiddle)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakA2 = this.D.FreqPeakA2 >= 55.0 ? (this.D.FreqPeakA2 >= 75.0 ? (this.D.FreqPeakA2 >= 105.0 ? 90.0 - (this.D.FreqPeakA2 - 105.0) * 0.75 : 60.0 + (this.D.FreqPeakA2 - 75.0) * 1.0) : 30.0 + (this.D.FreqPeakA2 - 55.0) * 1.5) : (this.D.FreqPeakA2 - 45.0) * 3.0;
      this.D.GainPeakA2 = Math.Max(this.D.GainPeakA2, 0.0) * (0.9 + 0.1 * this.D.MixPower + 0.1 * this.D.MixCylinder + 0.1 * this.D.MixTorque);
      this.D.GainPeakA2Front = Math.Floor((this.D.PeakA2Modifier * this.D.GainPeakA2 * (0.9 + 0.3 * this.D.MixFront)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakA2Rear = Math.Floor((this.D.PeakA2Modifier * this.D.GainPeakA2 * (0.9 + 0.3 * this.D.MixRear)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakA2 = Math.Floor((this.D.PeakA2Modifier * this.D.GainPeakA2 * (0.9 + 0.3 * this.D.MixMiddle)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakB2 = this.D.FreqPeakB2 >= 60.0 ? (this.D.FreqPeakB2 >= 100.0 ? 100.0 - (this.D.FreqPeakB2 - 100.0) * 0.85 : 30.0 + (this.D.FreqPeakB2 - 60.0) * 1.75) : (this.D.FreqPeakB2 - 30.0) * 1.0;
      this.D.GainPeakB2 = Math.Max(this.D.GainPeakB2, 0.0) * (0.9 + 0.1 * this.D.MixPower + 0.1 * this.D.MixCylinder + 0.1 * this.D.MixTorque);
      this.D.GainPeakB2Front = Math.Floor((this.D.PeakB2Modifier * this.D.GainPeakB2 * (0.9 + 0.3 * this.D.MixFront)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakB2Rear = Math.Floor((this.D.PeakB2Modifier * this.D.GainPeakB2 * (0.9 + 0.3 * this.D.MixRear)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      this.D.GainPeakB2 = Math.Floor((this.D.PeakB2Modifier * this.D.GainPeakB2 * (0.9 + 0.3 * this.D.MixMiddle)).Clamp<double>(0.0, (double) sbyte.MaxValue));
      if (this.S.EngineCylinders < 1.0)
      {
        this.D.GainLFEAdaptive = 0.0;
        this.D.Gain1H = Math.Floor(this.D.Gain1H * 0.7);
        this.D.Gain1H2 = 0.0;
        this.D.Gain2H = 0.0;
        this.D.Gain4H = 0.0;
        this.D.GainOctave = 0.0;
        this.D.GainIntervalA1 = 0.0;
        this.D.GainIntervalA2 = 0.0;
        this.D.GainPeakA1Front = 0.0;
        this.D.GainPeakA1Rear = 0.0;
        this.D.GainPeakA1 = 0.0;
        this.D.GainPeakA2Front = 0.0;
        this.D.GainPeakA2Rear = 0.0;
        this.D.GainPeakA2 = 0.0;
        this.D.GainPeakB1Front = 0.0;
        this.D.GainPeakB1Rear = 0.0;
        this.D.GainPeakB1 = 0.0;
        this.D.GainPeakB2Front = 0.0;
        this.D.GainPeakB2Rear = 0.0;
        this.D.GainPeakB2 = 0.0;
      }
      else if (this.S.EngineCylinders < 2.0)
        this.D.Gain4H = 0.0;
      if (this.D.EngineMult == 1.0)
        return;
      this.D.GainLFEAdaptive *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.Gain1H *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.Gain1H2 *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.Gain2H *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.Gain4H *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainOctave *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainIntervalA1 *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainIntervalA2 *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakA1Front *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakA1Rear *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakA1 *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakA2Front *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakA2Rear *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakA2 *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakB1Front *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakB1Rear *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakB1 *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakB2Front *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakB2Rear *= this.D.EngineMult * this.D.EngineMultAll;
      this.D.GainPeakB2 *= this.D.EngineMult * this.D.EngineMultAll;
    }

    private void UpdateVehiclePerGame(PluginManager pluginManager, ref GameData data)
    {
      this.D.SuspensionDistFLP = this.D.SuspensionDistFL;
      this.D.SuspensionDistFRP = this.D.SuspensionDistFR;
      this.D.SuspensionDistRLP = this.D.SuspensionDistRL;
      this.D.SuspensionDistRRP = this.D.SuspensionDistRR;
      this.D.SuspensionDistFL = 0.0;
      this.D.SuspensionDistFR = 0.0;
      this.D.SuspensionDistRL = 0.0;
      this.D.SuspensionDistRR = 0.0;
      this.D.SuspensionVelFLP = this.D.SuspensionVelFL;
      this.D.SuspensionVelFRP = this.D.SuspensionVelFR;
      this.D.SuspensionVelRLP = this.D.SuspensionVelRL;
      this.D.SuspensionVelRRP = this.D.SuspensionVelRR;
      this.D.SuspensionVelFL = 0.0;
      this.D.SuspensionVelFR = 0.0;
      this.D.SuspensionVelRL = 0.0;
      this.D.SuspensionVelRR = 0.0;
      this.D.SlipXFL = 0.0;
      this.D.SlipXFR = 0.0;
      this.D.SlipXRL = 0.0;
      this.D.SlipXRR = 0.0;
      this.D.SlipYFL = 0.0;
      this.D.SlipYFR = 0.0;
      this.D.SlipYRL = 0.0;
      this.D.SlipYRR = 0.0;
      this.D.ABSActive = data.NewData.ABSActive == 1;
      bool flag = true;
      switch (SimHapticsPlugin.CurrentGame)
      {
        case GameId.AC:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel02");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel03");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel04");
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed01"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed02"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed03"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed04"));
          this.SlipFromRPS();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip01") - Math.Abs(this.D.SlipYFL) * 1.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip02") - Math.Abs(this.D.SlipYFR) * 1.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip03") - Math.Abs(this.D.SlipYRL) * 1.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip04") - Math.Abs(this.D.SlipYRR) * 1.0, 0.0);
          if (this.D.TireDiameterFL == 0.0)
          {
            this.D.SlipXFL *= 0.5;
            this.D.SlipXFR *= 0.5;
            this.D.SlipXRL *= 0.5;
            this.D.SlipXRR *= 0.5;
          }
          this.D.TiresLeft = 1.0 + (double) Math.Max((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading01.Y"), (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading03.Y"));
          this.D.TiresRight = 1.0 + (double) Math.Max((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading02.Y"), (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading04.Y"));
          if (this.D.RumbleLeftAvg == 0.0)
            this.D.RumbleLeftAvg = this.D.TiresLeft;
          if (this.D.RumbleRightAvg == 0.0)
            this.D.RumbleRightAvg = this.D.TiresRight;
          this.D.RumbleLeftAvg = (this.D.RumbleLeftAvg + this.D.TiresLeft) * 0.5;
          this.D.RumbleRightAvg = (this.D.RumbleRightAvg + this.D.TiresRight) * 0.5;
          this.D.RumbleLeft = Math.Abs(this.D.TiresLeft / this.D.RumbleLeftAvg - 1.0) * 2000.0;
          this.D.RumbleRight = Math.Abs(this.D.TiresRight / this.D.RumbleRightAvg - 1.0) * 2000.0;
          break;
        case GameId.ACC:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel02");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel03");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel04");
          this.D.WiperStatus = (int) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Graphics.WiperLV");
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed01"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed02"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed03"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed04"));
          this.SlipFromRPS();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip01") - Math.Abs(this.D.SlipYFL) * 2.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip02") - Math.Abs(this.D.SlipYFR) * 2.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip03") - Math.Abs(this.D.SlipYRL) * 2.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip04") - Math.Abs(this.D.SlipYRR) * 2.0, 0.0);
          if (this.D.TireDiameterFL == 0.0)
          {
            this.D.SlipXFL *= 0.5;
            this.D.SlipXFR *= 0.5;
            this.D.SlipXRL *= 0.5;
            this.D.SlipXRR *= 0.5;
            break;
          }
          break;
        case GameId.AMS1:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.suspensionDeflection");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.suspensionDeflection");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.suspensionDeflection");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.suspensionDeflection");
          this.D.SpeedMs = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayer.speed");
          this.D.InvSpeedMs = this.D.SpeedMs != 0.0 ? 1.0 / this.D.SpeedMs : 0.0;
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.rotation"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.rotation"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.rotation"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.rotation"));
          this.SlipFromRPS();
          this.D.SlipXFL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.gripFract") - Math.Abs(this.D.SlipYFL) * 1.0, 0.0);
          this.D.SlipXFR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.gripFract") - Math.Abs(this.D.SlipYFR) * 1.0, 0.0);
          this.D.SlipXRL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.gripFract") - Math.Abs(this.D.SlipYRL) * 1.0, 0.0);
          this.D.SlipXRR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.gripFract") - Math.Abs(this.D.SlipYRR) * 1.0, 0.0);
          if (this.D.TireDiameterFL == 0.0)
          {
            this.D.SlipXFL *= 0.5;
            this.D.SlipXFR *= 0.5;
            this.D.SlipXRL *= 0.5;
            this.D.SlipXRR *= 0.5;
            break;
          }
          break;
        case GameId.AMS2:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel02");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel03");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel04");
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS01"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS02"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS03"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS04"));
          this.SlipFromRPS();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed01") * 0.1 - Math.Abs(this.D.SlipYFL) * 1.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed02") * 0.1 - Math.Abs(this.D.SlipYFR) * 1.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed03") * 0.1 - Math.Abs(this.D.SlipYRL) * 1.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed04") * 0.1 - Math.Abs(this.D.SlipYRR) * 1.0, 0.0);
          if (this.D.TireDiameterFL == 0.0)
          {
            this.D.SlipXFL *= 0.5;
            this.D.SlipXFR *= 0.5;
            this.D.SlipXRL *= 0.5;
            this.D.SlipXRR *= 0.5;
            break;
          }
          break;
        case GameId.D4:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight") * 0.001;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft") * 0.001;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight") * 0.001;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
          this.SlipFromWheelSpeed();
          break;
        case GameId.DR2:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight") * 0.001;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft") * 0.001;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight") * 0.001;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
          this.SlipFromWheelSpeed();
          this.D.VelocityX = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WorldSpeedX") * Math.Sin((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.XR"));
          this.D.YawRate = data.NewData.OrientationYawAcceleration;
          if (this.D.VelocityX < 0.0)
          {
            if (this.D.YawRate < 0.0)
            {
              this.D.SlipXFL = -this.D.VelocityX * this.D.WheelLoadFL * this.D.YawRate * 0.5;
              this.D.SlipXFR = -this.D.VelocityX * this.D.WheelLoadFR * this.D.YawRate * 0.5;
              this.D.SlipXRL = -this.D.VelocityX * this.D.WheelLoadRL * this.D.YawRate * 1.0;
              this.D.SlipXRR = -this.D.VelocityX * this.D.WheelLoadRR * this.D.YawRate * 1.0;
            }
            else
            {
              this.D.SlipXFL = -this.D.VelocityX * this.D.WheelLoadFL * this.D.YawRate * 1.0;
              this.D.SlipXFR = -this.D.VelocityX * this.D.WheelLoadFR * this.D.YawRate * 1.0;
              this.D.SlipXRL = -this.D.VelocityX * this.D.WheelLoadRL * this.D.YawRate * 0.5;
              this.D.SlipXRR = -this.D.VelocityX * this.D.WheelLoadRR * this.D.YawRate * 0.5;
            }
          }
          else if (this.D.YawRate < 0.0)
          {
            this.D.SlipXFL = this.D.VelocityX * this.D.WheelLoadFL * -this.D.YawRate * 1.0;
            this.D.SlipXFR = this.D.VelocityX * this.D.WheelLoadFR * -this.D.YawRate * 1.0;
            this.D.SlipXRL = this.D.VelocityX * this.D.WheelLoadRL * -this.D.YawRate * 0.5;
            this.D.SlipXRR = this.D.VelocityX * this.D.WheelLoadRR * -this.D.YawRate * 0.5;
          }
          else
          {
            this.D.SlipXFL = this.D.VelocityX * this.D.WheelLoadFL * -this.D.YawRate * 0.5;
            this.D.SlipXFR = this.D.VelocityX * this.D.WheelLoadFR * -this.D.YawRate * 0.5;
            this.D.SlipXRL = this.D.VelocityX * this.D.WheelLoadRL * -this.D.YawRate * 1.0;
            this.D.SlipXRR = this.D.VelocityX * this.D.WheelLoadRR * -this.D.YawRate * 1.0;
          }
          this.D.SlipXFL = Math.Max(this.D.SlipXFL, 0.0);
          this.D.SlipXFR = Math.Max(this.D.SlipXFL, 0.0);
          this.D.SlipXRL = Math.Max(this.D.SlipXFL, 0.0);
          this.D.SlipXRR = Math.Max(this.D.SlipXFL, 0.0);
          break;
        case GameId.WRC23:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_fl");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_fr");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_bl");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_br");
          this.D.SpeedMs = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_speed");
          this.D.InvSpeedMs = this.D.SpeedMs != 0.0 ? 1.0 / this.D.SpeedMs : 0.0;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_fl"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_fr"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_bl"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_br"));
          this.SlipFromWheelSpeed();
          this.D.VelocityX = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdateLocalVelocity.X");
          this.D.YawRate = data.NewData.OrientationYawAcceleration;
          if (this.D.VelocityX < 0.0)
          {
            if (this.D.YawRate < 0.0)
            {
              this.D.SlipXFL = -this.D.VelocityX * this.D.WheelLoadFL * this.D.YawRate * 0.5;
              this.D.SlipXFR = -this.D.VelocityX * this.D.WheelLoadFR * this.D.YawRate * 0.5;
              this.D.SlipXRL = -this.D.VelocityX * this.D.WheelLoadRL * this.D.YawRate * 1.0;
              this.D.SlipXRR = -this.D.VelocityX * this.D.WheelLoadRR * this.D.YawRate * 1.0;
            }
            else
            {
              this.D.SlipXFL = -this.D.VelocityX * this.D.WheelLoadFL * this.D.YawRate * 1.0;
              this.D.SlipXFR = -this.D.VelocityX * this.D.WheelLoadFR * this.D.YawRate * 1.0;
              this.D.SlipXRL = -this.D.VelocityX * this.D.WheelLoadRL * this.D.YawRate * 0.5;
              this.D.SlipXRR = -this.D.VelocityX * this.D.WheelLoadRR * this.D.YawRate * 0.5;
            }
          }
          else if (this.D.YawRate < 0.0)
          {
            this.D.SlipXFL = this.D.VelocityX * this.D.WheelLoadFL * -this.D.YawRate * 1.0;
            this.D.SlipXFR = this.D.VelocityX * this.D.WheelLoadFR * -this.D.YawRate * 1.0;
            this.D.SlipXRL = this.D.VelocityX * this.D.WheelLoadRL * -this.D.YawRate * 0.5;
            this.D.SlipXRR = this.D.VelocityX * this.D.WheelLoadRR * -this.D.YawRate * 0.5;
          }
          else
          {
            this.D.SlipXFL = this.D.VelocityX * this.D.WheelLoadFL * -this.D.YawRate * 0.5;
            this.D.SlipXFR = this.D.VelocityX * this.D.WheelLoadFR * -this.D.YawRate * 0.5;
            this.D.SlipXRL = this.D.VelocityX * this.D.WheelLoadRL * -this.D.YawRate * 1.0;
            this.D.SlipXRR = this.D.VelocityX * this.D.WheelLoadRR * -this.D.YawRate * 1.0;
          }
          this.D.SlipXFL = Math.Max(this.D.SlipXFL, 0.0);
          this.D.SlipXFR = Math.Max(this.D.SlipXFL, 0.0);
          this.D.SlipXRL = Math.Max(this.D.SlipXFL, 0.0);
          this.D.SlipXRR = Math.Max(this.D.SlipXFL, 0.0);
          break;
        case GameId.F12022:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition01") * 0.001;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition02") * 0.001;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition03") * 0.001;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition04") * 0.001;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed03"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed04"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed01"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed02"));
          this.SlipFromWheelSpeed();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip03") - Math.Abs(this.D.SlipYFL) * 2.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip04") - Math.Abs(this.D.SlipYFR) * 2.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip01") - Math.Abs(this.D.SlipYRL) * 2.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip02") - Math.Abs(this.D.SlipYRR) * 2.0, 0.0);
          break;
        case GameId.F12023:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition01") * 0.001;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition02") * 0.001;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition03") * 0.001;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition04") * 0.001;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed03"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed04"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed01"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed02"));
          this.SlipFromWheelSpeed();
          this.D.SlipXFL = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio01")) * 5.0 - Math.Abs(this.D.SlipYFL) * 1.0, 0.0);
          this.D.SlipXFR = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio02")) * 5.0 - Math.Abs(this.D.SlipYFR) * 1.0, 0.0);
          this.D.SlipXRL = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio03")) * 5.0 - Math.Abs(this.D.SlipYRL) * 1.0, 0.0);
          this.D.SlipXRR = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio04")) * 5.0 - Math.Abs(this.D.SlipYRR) * 1.0, 0.0);
          break;
        case GameId.Forza:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersFrontLeft");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersFrontRight");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersRearLeft");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersRearRight");
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedFrontLeft"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedFrontRight"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedRearLeft"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedRearRight"));
          this.SlipFromRPS();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipFrontLeft") - Math.Abs(this.D.SlipYFL) * 2.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipFrontRight") - Math.Abs(this.D.SlipYFR) * 2.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipRearLeft") - Math.Abs(this.D.SlipYRL) * 2.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipRearRight") - Math.Abs(this.D.SlipYRR) * 2.0, 0.0);
          if (this.D.TireDiameterFL == 0.0)
          {
            this.D.SlipXFL *= 0.5;
            this.D.SlipXFR *= 0.5;
            this.D.SlipXRL *= 0.5;
            this.D.SlipXRR *= 0.5;
            break;
          }
          break;
        case GameId.GTR2:
        case GameId.GSCE:
        case GameId.RF1:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.suspensionDeflection");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.suspensionDeflection");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.suspensionDeflection");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.suspensionDeflection");
          this.D.SpeedMs = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayer.speed");
          this.D.InvSpeedMs = this.D.SpeedMs != 0.0 ? 1.0 / this.D.SpeedMs : 0.0;
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.rotation"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.rotation"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.rotation"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.rotation"));
          this.SlipFromRPS();
          this.D.SlipXFL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.gripFract") - Math.Abs(this.D.SlipYFL) * 1.0, 0.0);
          this.D.SlipXFR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.gripFract") - Math.Abs(this.D.SlipYFR) * 1.0, 0.0);
          this.D.SlipXRL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.gripFract") - Math.Abs(this.D.SlipYRL) * 1.0, 0.0);
          this.D.SlipXRR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.gripFract") - Math.Abs(this.D.SlipYRR) * 1.0, 0.0);
          if (this.D.TireDiameterFL == 0.0)
          {
            this.D.SlipXFL *= 0.5;
            this.D.SlipXFR *= 0.5;
            this.D.SlipXRL *= 0.5;
            this.D.SlipXRR *= 0.5;
            break;
          }
          break;
        case GameId.IRacing:
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFshockDefl") != null)
          {
            this.D.SuspensionDistFL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFshockDefl"));
            this.D.SuspensionDistFR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RFshockDefl"));
          }
          else if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFSHshockDefl") != null)
          {
            this.D.SuspensionDistFL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFSHshockDefl"));
            this.D.SuspensionDistFR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RFSHshockDefl"));
          }
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRshockDefl") != null)
          {
            this.D.SuspensionDistRL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRshockDefl"));
            this.D.SuspensionDistRR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RRshockDefl"));
          }
          else if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRSHshockDefl") != null)
          {
            this.D.SuspensionDistRL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRSHshockDefl"));
            this.D.SuspensionDistRR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RRSHshockDefl"));
          }
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CFshockDefl") != null)
          {
            this.D.SuspensionDistFL = 0.5 * this.D.SuspensionDistFL + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CFshockDefl"));
            this.D.SuspensionDistFR = 0.5 * this.D.SuspensionDistFR + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CFshockDefl"));
          }
          else if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.HFshockDefl") != null)
          {
            this.D.SuspensionDistFL = 0.5 * this.D.SuspensionDistFL + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.HFshockDefl"));
            this.D.SuspensionDistFR = 0.5 * this.D.SuspensionDistFR + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.HFshockDefl"));
          }
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CRshockDefl") != null)
          {
            this.D.SuspensionDistRL = 0.5 * this.D.SuspensionDistRL + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CRshockDefl"));
            this.D.SuspensionDistRR = 0.5 * this.D.SuspensionDistRR + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CRshockDefl"));
          }
          this.D.VelocityX = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.VelocityY")) * 10.0;
          if (this.D.VelocityX < 0.0)
          {
            if (this.D.YawRate < 0.0)
            {
              this.D.SlipXFL = -this.D.VelocityX * this.D.WheelLoadFL * this.D.YawRate * 0.1;
              this.D.SlipXFR = -this.D.VelocityX * this.D.WheelLoadFR * this.D.YawRate * 0.1;
              this.D.SlipXRL = -this.D.VelocityX * this.D.WheelLoadRL * this.D.YawRate * 0.2;
              this.D.SlipXRR = -this.D.VelocityX * this.D.WheelLoadRR * this.D.YawRate * 0.2;
            }
            else
            {
              this.D.SlipXFL = -this.D.VelocityX * this.D.WheelLoadFL * this.D.YawRate * 0.2;
              this.D.SlipXFR = -this.D.VelocityX * this.D.WheelLoadFR * this.D.YawRate * 0.2;
              this.D.SlipXRL = -this.D.VelocityX * this.D.WheelLoadRL * this.D.YawRate * 0.1;
              this.D.SlipXRR = -this.D.VelocityX * this.D.WheelLoadRR * this.D.YawRate * 0.1;
            }
          }
          else if (this.D.YawRate < 0.0)
          {
            this.D.SlipXFL = this.D.VelocityX * this.D.WheelLoadFL * -this.D.YawRate * 0.2;
            this.D.SlipXFR = this.D.VelocityX * this.D.WheelLoadFR * -this.D.YawRate * 0.2;
            this.D.SlipXRL = this.D.VelocityX * this.D.WheelLoadRL * -this.D.YawRate * 0.1;
            this.D.SlipXRR = this.D.VelocityX * this.D.WheelLoadRR * -this.D.YawRate * 0.1;
          }
          else
          {
            this.D.SlipXFL = this.D.VelocityX * this.D.WheelLoadFL * -this.D.YawRate * 0.1;
            this.D.SlipXFR = this.D.VelocityX * this.D.WheelLoadFR * -this.D.YawRate * 0.1;
            this.D.SlipXRL = this.D.VelocityX * this.D.WheelLoadRL * -this.D.YawRate * 0.2;
            this.D.SlipXRR = this.D.VelocityX * this.D.WheelLoadRR * -this.D.YawRate * 0.2;
          }
          if (this.D.Brake > 0.0)
          {
            this.D.SlipYFL = this.D.BrakeF * this.D.SpeedMs * this.D.WheelLoadFL * this.D.InvAccSurgeAvg * 0.04;
            this.D.SlipYFR = this.D.BrakeF * this.D.SpeedMs * this.D.WheelLoadFR * this.D.InvAccSurgeAvg * 0.04;
            this.D.SlipYRL = (this.D.BrakeR + this.D.Handbrake) * this.D.SpeedMs * this.D.WheelLoadRL * this.D.InvAccSurgeAvg * 0.04;
            this.D.SlipYRR = (this.D.BrakeR + this.D.Handbrake) * this.D.SpeedMs * this.D.WheelLoadRR * this.D.InvAccSurgeAvg * 0.04;
          }
          else if (this.D.Accelerator > 10.0 && this.D.SpeedMs > 0.0 && this.D.AccSurgeAvg < 0.0)
          {
            if (this.S.PoweredWheels == "F")
            {
              this.D.SlipYFL = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.2;
              this.D.SlipYFR = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.2;
              this.D.SlipYRL = 0.0;
              this.D.SlipYRR = 0.0;
            }
            else if (this.S.PoweredWheels == "R")
            {
              this.D.SlipYFL = 0.0;
              this.D.SlipYFR = 0.0;
              this.D.SlipYRL = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.2;
              this.D.SlipYRR = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.2;
            }
            else
            {
              this.D.SlipYFL = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.15;
              this.D.SlipYFR = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.15;
              this.D.SlipYRL = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.15;
              this.D.SlipYRR = this.D.Accelerator * -this.D.InvAccSurgeAvg * this.D.InvSpeedMs * 0.15;
            }
          }
          this.D.SlipXFL = 0.0;
          this.D.SlipXFR = 0.0;
          this.D.SlipXRL = 0.0;
          this.D.SlipXRR = 0.0;
          this.D.SlipYFL = 0.0;
          this.D.SlipYFR = 0.0;
          this.D.SlipYRL = 0.0;
          this.D.SlipYRR = 0.0;
          break;
        case GameId.PC2:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel02");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel03");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel04");
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS01"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS02"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS03"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS04"));
          this.SlipFromRPS();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed01") * 0.1 - Math.Abs(this.D.SlipYFL) * 1.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed02") * 0.1 - Math.Abs(this.D.SlipYFR) * 1.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed03") * 0.1 - Math.Abs(this.D.SlipYRL) * 1.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed04") * 0.1 - Math.Abs(this.D.SlipYRR) * 1.0, 0.0);
          if (this.D.TireDiameterFL == 0.0)
          {
            this.D.SlipXFL *= 0.5;
            this.D.SlipXFR *= 0.5;
            this.D.SlipXRL *= 0.5;
            this.D.SlipXRR *= 0.5;
            break;
          }
          break;
        case GameId.RBR:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionLF.springDeflection");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionRF.springDeflection");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionLB.springDeflection");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionRB.springDeflection");
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFL"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFR"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRL"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRR"));
          this.SlipFromWheelSpeed();
          break;
        case GameId.RF2:
          this.D.SuspensionDistFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
          this.D.SuspensionDistFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
          this.D.SuspensionDistRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
          this.D.SuspensionDistRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
          this.D.WheelRotationFL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mRotation"));
          this.D.WheelRotationFR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mRotation"));
          this.D.WheelRotationRL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mRotation"));
          this.D.WheelRotationRR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mRotation"));
          this.SlipFromRPS();
          this.D.SlipXFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralGroundVel");
          this.D.SlipXFL = this.D.SlipXFL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralPatchVel") / this.D.SlipXFL;
          this.D.SlipXFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralGroundVel");
          this.D.SlipXFR = this.D.SlipXFR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralPatchVel") / this.D.SlipXFR;
          this.D.SlipXRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralGroundVel");
          this.D.SlipXRL = this.D.SlipXRL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralPatchVel") / this.D.SlipXRL;
          this.D.SlipXRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralGroundVel");
          this.D.SlipXRR = this.D.SlipXRR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralPatchVel") / this.D.SlipXRR;
          this.D.SlipXFL *= 0.5;
          this.D.SlipXFR *= 0.5;
          this.D.SlipXRL *= 0.5;
          this.D.SlipXRR *= 0.5;
          if (data.NewData.Brake > 90.0)
          {
            this.D.ABSActive = ((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mBrakePressure") + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < data.NewData.Brake - 1.0;
            break;
          }
          break;
        case GameId.RRRE:
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.FrontLeft") != null)
          {
            this.D.SuspensionDistFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.FrontLeft");
            this.D.SuspensionDistFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.FrontRight");
          }
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.RearLeft") != null)
          {
            this.D.SuspensionDistRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.RearLeft");
            this.D.SuspensionDistRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.RearRight");
          }
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionFront") != null)
          {
            this.D.SuspensionDistFL = 0.5 * this.D.SuspensionDistFL + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionFront");
            this.D.SuspensionDistFR = 0.5 * this.D.SuspensionDistFR + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionFront");
          }
          if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionRear") != null)
          {
            this.D.SuspensionDistRL = 0.5 * this.D.SuspensionDistRL + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionRear");
            this.D.SuspensionDistRR = 0.5 * this.D.SuspensionDistRR + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionRear");
          }
          this.D.WheelRotationFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.FrontLeft"));
          this.D.WheelRotationFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.FrontRight"));
          this.D.WheelRotationRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.RearLeft"));
          this.D.WheelRotationRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.RearRight"));
          this.SlipFromRPS();
          break;
        case GameId.GTL:
        case GameId.RACE07:
          double num1 = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.carCGLocY") * 0.2;
          this.D.SuspensionDistFL = num1 * this.D.WheelLoadFL;
          this.D.SuspensionDistFR = num1 * this.D.WheelLoadFR;
          this.D.SuspensionDistRL = num1 * this.D.WheelLoadRL;
          this.D.SuspensionDistRR = num1 * this.D.WheelLoadRR;
          break;
        case GameId.LFS:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels01.SuspDeflect");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels02.SuspDeflect");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels03.SuspDeflect");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels04.SuspDeflect");
          break;
        case GameId.WRC10:
        case GameId.WRCX:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position02");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position03");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position04");
          break;
        case GameId.WRCGen:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight");
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
          this.SlipFromWheelSpeed();
          break;
        case GameId.F12016:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
          this.SlipFromWheelSpeed();
          break;
        case GameId.F12017:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos01") * 0.001;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos02") * 0.001;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos03") * 0.001;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos04") * 0.001;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed03"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed04"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed01"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed02"));
          this.SlipFromWheelSpeed();
          break;
        case GameId.GLegends:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight") * 0.001;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft") * 0.001;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight") * 0.001;
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
          this.SlipFromWheelSpeed();
          break;
        case GameId.KK:
          flag = false;
          double propertyValue = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Motion.VelocityZ");
          if (this.D.VelocityZAvg == 0.0)
            this.D.VelocityZAvg = propertyValue;
          this.D.VelocityZAvg = (this.D.VelocityZAvg + propertyValue) * 0.5;
          double num2 = (propertyValue / this.D.VelocityZAvg - 1.0) * 0.5;
          this.D.SuspensionVelFL = num2 * this.D.WheelLoadFL;
          this.D.SuspensionVelFR = num2 * this.D.WheelLoadFR;
          this.D.SuspensionVelRL = num2 * this.D.WheelLoadRL;
          this.D.SuspensionVelRR = num2 * this.D.WheelLoadRR;
          break;
        case GameId.ATS:
        case GameId.ETS2:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection02");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection03");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection04");
          this.D.WiperStatus = (bool) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.DashboardValues.Wipers") ? 1 : 0;
          break;
        case GameId.BeamNG:
          flag = false;
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_fl") * 0.05;
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_fr") * 0.05;
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_rl") * 0.05;
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_rr") * 0.05;
          this.D.SuspensionVelFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_fl") * 0.05;
          this.D.SuspensionVelFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_fr") * 0.05;
          this.D.SuspensionVelRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_rl") * 0.05;
          this.D.SuspensionVelRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_rr") * 0.05;
          this.D.WheelSpeedFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_fl");
          this.D.WheelSpeedFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_fr");
          this.D.WheelSpeedRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_rl");
          this.D.WheelSpeedRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_rr");
          this.SlipFromWheelSpeed();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_fl") * 0.1 - Math.Abs(this.D.SlipYFL) * 2.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_fr") * 0.1 - Math.Abs(this.D.SlipYFR) * 2.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_rl") * 0.1 - Math.Abs(this.D.SlipYRL) * 2.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_rr") * 0.1 - Math.Abs(this.D.SlipYRR) * 2.0, 0.0);
          break;
        case GameId.GPBikes:
        case GameId.MXBikes:
          flag = false;
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength01");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength02");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength02");
          this.D.SuspensionVelFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity01");
          this.D.SuspensionVelFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity01");
          this.D.SuspensionVelRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity02");
          this.D.SuspensionVelRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity02");
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed01"));
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed01"));
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed02"));
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed02"));
          this.SlipFromWheelSpeed();
          if ((int) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_aiWheelMaterial01") == 7 || (int) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_aiWheelMaterial02") == 7)
          {
            this.D.RumbleLeft = 50.0;
            this.D.RumbleRight = 50.0;
            break;
          }
          this.D.RumbleLeft = 0.0;
          this.D.RumbleRight = 0.0;
          break;
        case GameId.LMU:
          this.D.SuspensionDistFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
          this.D.SuspensionDistFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
          this.D.SuspensionDistRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
          this.D.SuspensionDistRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
          this.D.WheelRotationFL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mRotation"));
          this.D.WheelRotationFR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mRotation"));
          this.D.WheelRotationRL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mRotation"));
          this.D.WheelRotationRR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mRotation"));
          this.SlipFromRPS();
          this.D.SlipXFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralGroundVel");
          this.D.SlipXFL = this.D.SlipXFL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralPatchVel") / this.D.SlipXFL;
          this.D.SlipXFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralGroundVel");
          this.D.SlipXFR = this.D.SlipXFR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralPatchVel") / this.D.SlipXFR;
          this.D.SlipXRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralGroundVel");
          this.D.SlipXRL = this.D.SlipXRL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralPatchVel") / this.D.SlipXRL;
          this.D.SlipXRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralGroundVel");
          this.D.SlipXRR = this.D.SlipXRR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralPatchVel") / this.D.SlipXRR;
          this.D.SlipXFL = Math.Abs(this.D.SlipXFL - 1.0);
          this.D.SlipXFR = Math.Abs(this.D.SlipXFR - 1.0);
          this.D.SlipXRL = Math.Abs(this.D.SlipXRL - 1.0);
          this.D.SlipXRR = Math.Abs(this.D.SlipXRR - 1.0);
          if (data.NewData.Brake > 80.0)
          {
            this.D.ABSActive = ((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mBrakePressure") + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < data.NewData.Brake - 1.0;
            break;
          }
          break;
        case GameId.GranTurismo7:
        case GameId.GranTurismoSport:
          this.D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight01");
          this.D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight02");
          this.D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight03");
          this.D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight04");
          this.D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed01")) * 0.277778;
          this.D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed02")) * 0.277778;
          this.D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed03")) * 0.277778;
          this.D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed04")) * 0.277778;
          this.SlipFromWheelSpeed();
          this.D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip01") - Math.Abs(this.D.SlipYFL) * 2.0, 0.0);
          this.D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip02") - Math.Abs(this.D.SlipYFR) * 2.0, 0.0);
          this.D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip03") - Math.Abs(this.D.SlipYRL) * 2.0, 0.0);
          this.D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip04") - Math.Abs(this.D.SlipYRR) * 2.0, 0.0);
          break;
      }
      if (!flag)
        return;
      this.D.SuspensionVelFL = (this.D.SuspensionDistFL - this.D.SuspensionDistFLP) * this.D.FPS;
      this.D.SuspensionVelFR = (this.D.SuspensionDistFR - this.D.SuspensionDistFRP) * this.D.FPS;
      this.D.SuspensionVelRL = (this.D.SuspensionDistRL - this.D.SuspensionDistRLP) * this.D.FPS;
      this.D.SuspensionVelRR = (this.D.SuspensionDistRR - this.D.SuspensionDistRRP) * this.D.FPS;
    }

    private void SlipFromRPS()
    {
      if (this.D.TireDiameterSampleCount < this.D.TireDiameterSampleMax && this.D.Accelerator < 60.0 && this.D.SpeedMs > 5.0 && Math.Abs(this.D.AccHeave2S) < 0.1 && Math.Abs(this.D.AccSurge2S) < 0.01 && Math.Abs(this.D.AccSway2S) < 0.08)
      {
        this.D.TireDiameterSampleFL = 2.0 * this.D.SpeedMs / this.D.WheelRotationFL;
        this.D.TireDiameterSampleFR = 2.0 * this.D.SpeedMs / this.D.WheelRotationFR;
        this.D.TireDiameterSampleRL = 2.0 * this.D.SpeedMs / this.D.WheelRotationRL;
        this.D.TireDiameterSampleRR = 2.0 * this.D.SpeedMs / this.D.WheelRotationRR;
        if (this.D.TireDiameterSampleFL > 1.0)
        {
          if (this.D.TireDiameterSampleFL > 3.0)
            this.D.TireDiameterSampleFL = 0.66;
          else if (this.D.TireDiameterSampleCount > 0 && Math.Abs(this.D.TireDiameterSampleFL - this.D.TireDiameterFL) > 0.2 * this.D.TireDiameterFL)
            this.D.TireDiameterSampleFL = this.D.TireDiameterFL * 0.9 + this.D.TireDiameterSampleFL * 0.1;
        }
        if (this.D.TireDiameterSampleFR > 1.0)
        {
          if (this.D.TireDiameterSampleFR > 3.0)
            this.D.TireDiameterSampleFR = 0.66;
          else if (this.D.TireDiameterSampleCount > 0 && Math.Abs(this.D.TireDiameterSampleFR - this.D.TireDiameterFR) > 0.2 * this.D.TireDiameterFR)
            this.D.TireDiameterSampleFR = this.D.TireDiameterFR * 0.9 + this.D.TireDiameterSampleFR * 0.1;
        }
        if (this.D.TireDiameterSampleRL > 1.0)
        {
          if (this.D.TireDiameterSampleRL > 3.0)
            this.D.TireDiameterSampleRL = 0.66;
          else if (this.D.TireDiameterSampleCount > 0 && Math.Abs(this.D.TireDiameterSampleRL - this.D.TireDiameterRL) > 0.2 * this.D.TireDiameterRL)
            this.D.TireDiameterSampleRL = this.D.TireDiameterRL * 0.9 + this.D.TireDiameterSampleRL * 0.1;
        }
        if (this.D.TireDiameterSampleRR > 1.0)
        {
          if (this.D.TireDiameterSampleRR > 3.0)
            this.D.TireDiameterSampleRR = 0.66;
          else if (this.D.TireDiameterSampleCount > 0 && Math.Abs(this.D.TireDiameterSampleRR - this.D.TireDiameterRR) > 0.2 * this.D.TireDiameterRR)
            this.D.TireDiameterSampleRR = this.D.TireDiameterRR * 0.9 + this.D.TireDiameterSampleRR * 0.1;
        }
        double num1 = Math.Min(Math.Abs(this.D.YawRate) * 0.1, 1.0);
        if (this.D.YawRate < 0.0)
          this.D.TireDiameterSampleFR = this.D.TireDiameterSampleFL * num1 + this.D.TireDiameterSampleFR * (1.0 - num1);
        else
          this.D.TireDiameterSampleFL = this.D.TireDiameterSampleFR * num1 + this.D.TireDiameterSampleFL * (1.0 - num1);
        if (this.D.TireDiameterSampleCount == 0)
        {
          this.D.TireDiameterFL = this.D.TireDiameterSampleFL;
          this.D.TireDiameterFR = this.D.TireDiameterSampleFR;
          this.D.TireDiameterRL = this.D.TireDiameterSampleRL;
          this.D.TireDiameterRR = this.D.TireDiameterSampleRR;
        }
        else
        {
          double num2 = (0.5 * (double) this.D.TireDiameterSampleMax + (double) this.D.TireDiameterSampleCount) / (double) (2 * this.D.TireDiameterSampleMax);
          this.D.TireDiameterFL = this.D.TireDiameterFL * num2 + this.D.TireDiameterSampleFL * (1.0 - num2);
          this.D.TireDiameterFR = this.D.TireDiameterFR * num2 + this.D.TireDiameterSampleFR * (1.0 - num2);
          this.D.TireDiameterRL = this.D.TireDiameterRL * num2 + this.D.TireDiameterSampleRL * (1.0 - num2);
          this.D.TireDiameterRR = this.D.TireDiameterRR * num2 + this.D.TireDiameterSampleRR * (1.0 - num2);
        }
        ++this.D.TireDiameterSampleCount;
        if (this.D.TireDiameterSampleCount == this.D.TireDiameterSampleMax)
        {
          if (Math.Abs(this.D.TireDiameterFL - this.D.TireDiameterFR) < 0.1)
          {
            this.D.TireDiameterFL = (this.D.TireDiameterFL + this.D.TireDiameterFR) * 0.5;
            this.D.TireDiameterFR = this.D.TireDiameterFL;
          }
          if (Math.Abs(this.D.TireDiameterRL - this.D.TireDiameterRR) < 0.1)
          {
            this.D.TireDiameterRL = (this.D.TireDiameterRL + this.D.TireDiameterRR) * 0.5;
            this.D.TireDiameterRR = this.D.TireDiameterRL;
          }
        }
      }
      if (this.D.TireDiameterFL <= 0.0)
        return;
      this.D.WheelSpeedFL = this.D.TireDiameterFL * this.D.WheelRotationFL * 0.5;
      this.D.WheelSpeedFR = this.D.TireDiameterFR * this.D.WheelRotationFR * 0.5;
      this.D.WheelSpeedRL = this.D.TireDiameterRL * this.D.WheelRotationRL * 0.5;
      this.D.WheelSpeedRR = this.D.TireDiameterRR * this.D.WheelRotationRR * 0.5;
      this.SlipFromWheelSpeed();
    }

    private void SlipFromWheelSpeed()
    {
      if (this.D.SpeedMs <= 0.05)
        return;
      this.D.SlipYFL = (this.D.SpeedMs - this.D.WheelSpeedFL) * this.D.InvSpeedMs;
      this.D.SlipYFR = (this.D.SpeedMs - this.D.WheelSpeedFR) * this.D.InvSpeedMs;
      this.D.SlipYRL = (this.D.SpeedMs - this.D.WheelSpeedRL) * this.D.InvSpeedMs;
      this.D.SlipYRR = (this.D.SpeedMs - this.D.WheelSpeedRR) * this.D.InvSpeedMs;
      if (this.D.SpeedMs >= 3.0)
        return;
      this.D.SlipYFL *= this.D.SpeedMs * 0.333;
      this.D.SlipYFR *= this.D.SpeedMs * 0.333;
      this.D.SlipYRL *= this.D.SpeedMs * 0.333;
      this.D.SlipYRR *= this.D.SpeedMs * 0.333;
    }

    private void SetVehiclePerGame(PluginManager pluginManager, ref StatusDataBase db)
    {
      switch (SimHapticsPlugin.CurrentGame)
      {
        case GameId.AC:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.ACC:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.AMS1:
          if (this.S.Id != db.CarId && this.S.Category != db.CarClass && SimHapticsPlugin.FailedId != db.CarId && SimHapticsPlugin.FailedCategory != db.CarClass)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, db.CarClass, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.AMS2:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.S.Name = db.CarModel;
            this.S.Category = db.CarClass;
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.D4:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = 10.0 * (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.IdleRpm");
            break;
          }
          break;
        case GameId.DR2:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = 10.0 * (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.IdleRpm");
            break;
          }
          break;
        case GameId.WRC23:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, Math.Floor(db.CarSettings_CurrentGearRedLineRPM), db.MaxRpm);
            this.D.IdleRPM = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_engine_rpm_idle");
            break;
          }
          break;
        case GameId.F12022:
        case GameId.F12023:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = 10.0 * (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerCarStatusData.m_idleRPM");
            break;
          }
          break;
        case GameId.Forza:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId.Substring(4), (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.EngineIdleRpm");
            break;
          }
          break;
        case GameId.GTR2:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.IRacing:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.DriverInfo.DriverCarIdleRPM");
            this.D.GameAltText = pluginManager.GameName + (string) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.WeekendInfo.Category");
            break;
          }
          break;
        case GameId.PC2:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.RBR:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.RF2:
          if (this.S.Id != db.CarId && this.S.Category != db.CarClass && SimHapticsPlugin.FailedId != db.CarId && SimHapticsPlugin.FailedCategory != db.CarClass)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, db.CarClass, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.RRRE:
          if (this.S.Id != db.CarModel && SimHapticsPlugin.FailedId != db.CarModel)
          {
            SimHapticsPlugin.FetchCarData(db.CarModel, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.BeamNG:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            this.S.Redline = db.MaxRpm;
            this.S.MaxRPM = Math.Ceiling(db.MaxRpm * 0.001) - db.MaxRpm * 0.001 > 0.55 ? Math.Ceiling(db.MaxRpm * 0.001) * 1000.0 : Math.Ceiling((db.MaxRpm + 1000.0) * 0.001) * 1000.0;
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, this.S.Redline, this.S.MaxRPM);
            this.D.IdleRPM = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.idle_rpm");
            break;
          }
          break;
        case GameId.GPBikes:
        case GameId.MXBikes:
          if (this.S.Id != db.CarId)
          {
            this.S.Id = db.CarId;
            this.D.IdleRPM = db.MaxRpm * 0.25;
            this.S.MaxRPM = db.MaxRpm;
            this.S.Redline = (double) (int) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sEvent.m_iShiftRPM");
            SimHapticsPlugin.LoadFinish = false;
            SimHapticsPlugin.FetchStatus = APIStatus.Fail;
            break;
          }
          break;
        case GameId.LMU:
          if (this.S.Id != db.CarId && this.S.Category != db.CarClass && SimHapticsPlugin.FailedId != db.CarId && SimHapticsPlugin.FailedCategory != db.CarClass)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, db.CarClass, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        case GameId.GranTurismo7:
        case GameId.GranTurismoSport:
          if (this.S.Id != db.CarId && SimHapticsPlugin.FailedId != db.CarId)
          {
            SimHapticsPlugin.FetchCarData(db.CarId, (string) null, this.S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
            this.S.Redline = (double) (short) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.MinAlertRPM");
            this.S.MaxRPM = (double) (short) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.MaxAlertRPM");
            this.D.IdleRPM = db.MaxRpm * 0.25;
            break;
          }
          break;
        default:
          this.D.IdleRPM = db.MaxRpm * 0.25;
          this.S.Redline = db.CarSettings_CurrentGearRedLineRPM;
          this.S.MaxRPM = db.MaxRpm;
          SimHapticsPlugin.FetchStatus = APIStatus.Fail;
          break;
      }
      if (!SimHapticsPlugin.LoadFinish && SimHapticsPlugin.FetchStatus == APIStatus.Success)
      {
        this.Settings.Vehicle = new Spec(this.S);
        SimHapticsPlugin.LoadStatus = DataStatus.SimHapticsAPI;
        this.D.LoadStatusText = "DB Load Success";
        this.FinalizeVehicleLoad();
      }
      if (!SimHapticsPlugin.LoadFinish && SimHapticsPlugin.FetchStatus == APIStatus.Fail)
      {
        this.SetDefaultVehicle(ref db);
        this.FinalizeVehicleLoad();
      }
      this.D.Gears = db.CarSettings_MaxGears > 0 ? db.CarSettings_MaxGears : 1;
      this.D.GearInterval = (double) (1 / this.D.Gears);
    }

    private void SetDefaultVehicle(ref StatusDataBase db)
    {
      if (this.Settings.Vehicle != null && (this.Settings.Vehicle.Id == db.CarId || this.Settings.Vehicle.Id == db.CarModel))
      {
        this.S = this.Settings.Vehicle;
        SimHapticsPlugin.LoadStatus = DataStatus.SettingsFile;
        this.D.LoadStatusText = "Load Fail: Loaded from Settings";
      }
      if (SimHapticsPlugin.LoadStatus == DataStatus.SettingsFile)
        return;
      this.S.Game = SimHapticsPlugin.GameDBText;
      this.S.Name = db.CarModel;
      this.S.Id = db.CarId;
      this.S.Category = db.CarClass;
      this.S.EngineConfiguration = "V";
      this.S.EngineCylinders = 6.0;
      this.S.EngineLocation = "RM";
      this.S.PoweredWheels = "A";
      this.S.Displacement = 3000.0;
      this.S.MaxPower = 300.0;
      this.S.ElectricMaxPower = 0.0;
      this.S.MaxTorque = 250.0;
      SimHapticsPlugin.LoadStatus = DataStatus.GameData;
      switch (SimHapticsPlugin.CurrentGame)
      {
        case GameId.AC:
        case GameId.ACC:
        case GameId.AMS1:
        case GameId.AMS2:
        case GameId.Forza:
        case GameId.GTR2:
        case GameId.IRacing:
        case GameId.PC2:
        case GameId.RBR:
        case GameId.RF2:
        case GameId.RRRE:
        case GameId.BeamNG:
          this.D.LoadStatusText = "Not in DB: using generic car";
          break;
        case GameId.D4:
        case GameId.DR2:
        case GameId.WRC23:
          this.D.LoadStatusText = "Not in DB: using generic Rally2";
          this.S.EngineConfiguration = "I";
          this.S.EngineCylinders = 4.0;
          this.S.EngineLocation = "F";
          this.S.PoweredWheels = "A";
          this.S.Displacement = 1600.0;
          this.S.MaxPower = 300.0;
          this.S.ElectricMaxPower = 0.0;
          this.S.MaxTorque = 400.0;
          break;
        case GameId.F12022:
        case GameId.F12023:
          this.D.LoadStatusText = "Not in DB: using generic F1";
          this.S.EngineConfiguration = "V";
          this.S.EngineCylinders = 6.0;
          this.S.EngineLocation = "RM";
          this.S.PoweredWheels = "R";
          this.S.Displacement = 1600.0;
          this.S.MaxPower = 1000.0;
          this.S.ElectricMaxPower = 0.0;
          this.S.MaxTorque = 650.0;
          break;
        case GameId.KK:
          this.D.LoadStatusText = "Not in DB: using generic Kart";
          this.S.EngineConfiguration = "I";
          this.S.EngineCylinders = 1.0;
          this.S.EngineLocation = "RM";
          this.S.PoweredWheels = "R";
          this.S.Displacement = 130.0;
          this.S.MaxPower = 34.0;
          this.S.ElectricMaxPower = 0.0;
          this.S.MaxTorque = 24.0;
          break;
        case GameId.GPBikes:
          this.D.LoadStatusText = "Not in DB: using generic Superbike";
          this.S.EngineConfiguration = "I";
          this.S.EngineCylinders = 4.0;
          this.S.EngineLocation = "M";
          this.S.PoweredWheels = "R";
          this.S.Displacement = 998.0;
          this.S.MaxPower = 200.0;
          this.S.ElectricMaxPower = 0.0;
          this.S.MaxTorque = 100.0;
          break;
        case GameId.MXBikes:
          this.D.LoadStatusText = "Not in DB: using generic MX Bike";
          this.S.EngineConfiguration = "I";
          this.S.EngineCylinders = 1.0;
          this.S.EngineLocation = "M";
          this.S.PoweredWheels = "R";
          this.S.Displacement = 450.0;
          this.S.MaxPower = 50.0;
          this.S.ElectricMaxPower = 0.0;
          this.S.MaxTorque = 45.0;
          break;
        case GameId.GranTurismo7:
        case GameId.GranTurismoSport:
          this.D.LoadStatusText = "Not in DB: redline loaded from game";
          this.S.EngineConfiguration = "V";
          this.S.EngineCylinders = 6.0;
          this.S.EngineLocation = "RM";
          this.S.PoweredWheels = "R";
          this.S.Displacement = 4000.0;
          this.S.MaxPower = 500.0;
          this.S.ElectricMaxPower = 0.0;
          this.S.MaxTorque = 400.0;
          break;
        default:
          this.D.LoadStatusText = "Load Fail: Specs not available for this game";
          break;
      }
      if (SimHapticsPlugin.CurrentGame != GameId.RRRE && SimHapticsPlugin.CurrentGame != GameId.D4 && SimHapticsPlugin.CurrentGame != GameId.DR2)
        return;
      this.S.Id = db.CarModel;
    }

    private void FinalizeVehicleLoad()
    {
      this.D.CarInitCount = 0;
      this.D.IdleSampleCount = 0;
      this.D.Gear = 0;
      this.D.SuspensionFL = 0.0;
      this.D.SuspensionFR = 0.0;
      this.D.SuspensionRL = 0.0;
      this.D.SuspensionRR = 0.0;
      this.D.SuspensionDistFLP = 0.0;
      this.D.SuspensionDistFRP = 0.0;
      this.D.SuspensionDistRLP = 0.0;
      this.D.SuspensionDistRRP = 0.0;
      this.D.SuspensionVelFLP = 0.0;
      this.D.SuspensionVelFRP = 0.0;
      this.D.SuspensionVelRLP = 0.0;
      this.D.SuspensionVelRRP = 0.0;
      this.D.SuspensionAccFLP = 0.0;
      this.D.SuspensionAccFRP = 0.0;
      this.D.SuspensionAccRLP = 0.0;
      this.D.SuspensionAccRRP = 0.0;
      Array.Clear((Array) this.D.AccHeave, 0, this.D.AccHeave.Length);
      Array.Clear((Array) this.D.AccSurge, 0, this.D.AccSurge.Length);
      Array.Clear((Array) this.D.AccSway, 0, this.D.AccSway.Length);
      this.D.Acc1 = 0;
      this.D.TireDiameterSampleCount = this.D.TireDiameterSampleCount == -1 ? -1 : 0;
      this.D.TireDiameterFL = 0.0;
      this.D.TireDiameterFR = 0.0;
      this.D.TireDiameterRL = 0.0;
      this.D.TireDiameterRR = 0.0;
      this.D.RumbleLeftAvg = 0.0;
      this.D.RumbleRightAvg = 0.0;
      this.SetRPMIntervals();
      this.SetRPMMix();
      SimHapticsPlugin.LoadFinish = true;
    }

    private void SetRPMIntervals()
    {
      if (this.S.EngineCylinders == 1.0)
      {
        this.D.IntervalOctave = 4.0;
        this.D.IntervalA = 0.0;
        this.D.IntervalB = 0.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 2.0)
      {
        this.D.IntervalOctave = 4.0;
        this.D.IntervalA = 6.0;
        this.D.IntervalB = 0.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 4.0)
      {
        this.D.IntervalOctave = 8.0;
        this.D.IntervalA = 4.0;
        this.D.IntervalB = 5.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 8.0)
      {
        this.D.IntervalOctave = 16.0;
        this.D.IntervalA = 6.0;
        this.D.IntervalB = 10.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 16.0)
      {
        this.D.IntervalOctave = 16.0;
        this.D.IntervalA = 12.0;
        this.D.IntervalB = 20.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 3.0)
      {
        this.D.IntervalOctave = 6.0;
        this.D.IntervalA = 4.0;
        this.D.IntervalB = 0.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 6.0)
      {
        this.D.IntervalOctave = 12.0;
        this.D.IntervalA = 8.0;
        this.D.IntervalB = 10.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 12.0)
      {
        this.D.IntervalOctave = 12.0;
        this.D.IntervalA = 16.0;
        this.D.IntervalB = 20.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 5.0)
      {
        this.D.IntervalOctave = 10.0;
        this.D.IntervalA = 6.0;
        this.D.IntervalB = 9.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineCylinders == 10.0)
      {
        this.D.IntervalOctave = 10.0;
        this.D.IntervalA = 12.0;
        this.D.IntervalB = 18.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else if (this.S.EngineConfiguration == "R")
      {
        this.D.IntervalOctave = 12.0;
        this.D.IntervalA = 9.0;
        this.D.IntervalB = 15.0;
        this.D.IntervalPeakA = 8.0;
        this.D.IntervalPeakB = 4.0;
      }
      else
      {
        this.D.IntervalOctave = 0.0;
        this.D.IntervalA = 0.0;
        this.D.IntervalB = 0.0;
        this.D.IntervalPeakA = 0.0;
        this.D.IntervalPeakB = 0.0;
      }
    }

    private void SetRPMMix()
    {
      this.D.InvMaxRPM = this.S.MaxRPM > 0.0 ? 1.0 / this.S.MaxRPM : 0.0001;
      this.D.IdlePercent = this.D.IdleRPM * this.D.InvMaxRPM;
      this.D.RedlinePercent = this.S.Redline * this.D.InvMaxRPM;
      if (this.S.Displacement > 0.0)
      {
        this.D.CylinderDisplacement = this.S.Displacement / this.S.EngineCylinders;
        this.D.MixCylinder = 1.0 - Math.Max(2000.0 - this.D.CylinderDisplacement, 0.0) * Math.Max(2000.0 - this.D.CylinderDisplacement, 0.0) * 2.5E-07;
        this.D.MixDisplacement = 1.0 - Math.Max(10000.0 - this.S.Displacement, 0.0) * Math.Max(10000.0 - this.S.Displacement, 0.0) * 1E-08;
      }
      else
      {
        this.D.MixCylinder = 0.0;
        this.D.MixDisplacement = 0.0;
      }
      this.D.MixPower = 1.0 - Math.Max(2000.0 - (this.S.MaxPower - this.S.ElectricMaxPower), 0.0) * Math.Max(2000.0 - (this.S.MaxPower - this.S.ElectricMaxPower), 0.0) * 2.5E-07;
      this.D.MixTorque = 1.0 - Math.Max(2000.0 - this.S.MaxTorque, 0.0) * Math.Max(2000.0 - this.S.MaxTorque, 0.0) * 2.5E-07;
      this.D.MixFront = !(this.S.EngineLocation == "F") ? (!(this.S.EngineLocation == "FM") ? (!(this.S.EngineLocation == "M") ? (!(this.S.EngineLocation == "RM") ? (!(this.S.PoweredWheels == "F") ? (!(this.S.PoweredWheels == "R") ? 0.2 : 0.1) : 0.3) : (!(this.S.PoweredWheels == "F") ? (!(this.S.PoweredWheels == "R") ? 0.3 : 0.2) : 0.4)) : (!(this.S.PoweredWheels == "F") ? (!(this.S.PoweredWheels == "R") ? 0.5 : 0.4) : 0.6)) : (!(this.S.PoweredWheels == "F") ? (!(this.S.PoweredWheels == "R") ? 0.7 : 0.6) : 0.8)) : (!(this.S.PoweredWheels == "F") ? (!(this.S.PoweredWheels == "R") ? 0.8 : 0.7) : 0.9);
      this.D.MixMiddle = Math.Abs(this.D.MixFront - 0.5) * 2.0;
      this.D.MixRear = 1.0 - this.D.MixFront;
    }

    public void SetGame(PluginManager pluginManager, SimData sd)
    {
      SimHapticsPlugin.GameDBText = pluginManager.GameName;
      sd.GameAltText = pluginManager.GameName;
      switch (SimHapticsPlugin.GameDBText)
      {
        case "AssettoCorsa":
          SimHapticsPlugin.CurrentGame = GameId.AC;
          SimHapticsPlugin.GameDBText = "AC";
          sd.RumbleFromPlugin = true;
          break;
        case "AssettoCorsaCompetizione":
          SimHapticsPlugin.CurrentGame = GameId.ACC;
          SimHapticsPlugin.GameDBText = "ACC";
          break;
        case "Automobilista":
          SimHapticsPlugin.CurrentGame = GameId.AMS1;
          SimHapticsPlugin.GameDBText = "AMS1";
          break;
        case "Automobilista2":
          SimHapticsPlugin.CurrentGame = GameId.AMS2;
          SimHapticsPlugin.GameDBText = "AMS2";
          break;
        case "FH4":
        case "FH5":
        case "FM7":
        case "FM8":
          SimHapticsPlugin.CurrentGame = GameId.Forza;
          SimHapticsPlugin.GameDBText = "Forza";
          break;
        case "IRacing":
          SimHapticsPlugin.CurrentGame = GameId.IRacing;
          sd.GameAltText += (string) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.WeekendInfo.Category");
          break;
        case "KartKraft":
          SimHapticsPlugin.CurrentGame = GameId.KK;
          SimHapticsPlugin.GameDBText = "KK";
          break;
        case "LFS":
          SimHapticsPlugin.CurrentGame = GameId.LFS;
          break;
        case "PCars1":
        case "PCars2":
        case "PCars3":
          SimHapticsPlugin.CurrentGame = GameId.PC2;
          SimHapticsPlugin.GameDBText = "PC2";
          break;
        case "RBR":
          SimHapticsPlugin.CurrentGame = GameId.RBR;
          sd.TireDiameterSampleCount = -1;
          break;
        case "RFactor1":
          SimHapticsPlugin.CurrentGame = GameId.RF1;
          SimHapticsPlugin.GameDBText = "RF1";
          break;
        case "RFactor2":
        case "RFactor2Spectator":
          SimHapticsPlugin.CurrentGame = GameId.RF2;
          SimHapticsPlugin.GameDBText = "RF2";
          break;
        case "LMU":
          SimHapticsPlugin.CurrentGame = GameId.LMU;
          SimHapticsPlugin.GameDBText = "LMU";
          break;
        case "RRRE":
          SimHapticsPlugin.CurrentGame = GameId.RRRE;
          break;
        case "SIMBINGTLEGENDS":
          SimHapticsPlugin.CurrentGame = GameId.GTL;
          SimHapticsPlugin.GameDBText = "GTL";
          break;
        case "SIMBINGTR2":
          SimHapticsPlugin.CurrentGame = GameId.GTR2;
          SimHapticsPlugin.GameDBText = "GTR2";
          break;
        case "SIMBINRACE07":
          SimHapticsPlugin.CurrentGame = GameId.RACE07;
          SimHapticsPlugin.GameDBText = "RACE07";
          break;
        case "StockCarExtreme":
          SimHapticsPlugin.CurrentGame = GameId.GSCE;
          SimHapticsPlugin.GameDBText = "GSCE";
          break;
        case "CodemastersDirtRally1":
        case "CodemastersDirtRally2":
          SimHapticsPlugin.CurrentGame = GameId.DR2;
          SimHapticsPlugin.GameDBText = "DR2";
          sd.TireDiameterSampleCount = -1;
          break;
        case "CodemastersDirt2":
        case "CodemastersDirt3":
        case "CodemastersDirtShowdown":
        case "CodemastersDirt4":
          SimHapticsPlugin.CurrentGame = GameId.D4;
          SimHapticsPlugin.GameDBText = "D4";
          sd.TireDiameterSampleCount = -1;
          break;
        case "EAWRC23":
          SimHapticsPlugin.CurrentGame = GameId.WRC23;
          SimHapticsPlugin.GameDBText = "WRC23";
          sd.AccSamples = 32;
          sd.TireDiameterSampleCount = -1;
          break;
        case "F12012":
        case "F12013":
        case "F12014":
        case "F12015":
        case "F12016":
          SimHapticsPlugin.CurrentGame = GameId.F12016;
          SimHapticsPlugin.GameDBText = "F12016";
          break;
        case "F12017":
          SimHapticsPlugin.CurrentGame = GameId.F12017;
          sd.TireDiameterSampleCount = -1;
          break;
        case "F12018":
        case "F12019":
        case "F12020":
        case "F12021":
        case "F12022":
          SimHapticsPlugin.CurrentGame = GameId.F12022;
          SimHapticsPlugin.GameDBText = "F12022";
          sd.TireDiameterSampleCount = -1;
          break;
        case "F12023":
        case "F12024":
        case "F12025":
        case "F12026":
          SimHapticsPlugin.CurrentGame = GameId.F12023;
          SimHapticsPlugin.GameDBText = "F12022";
          sd.TireDiameterSampleCount = -1;
          break;
        case "CodemastersGrid2":
        case "CodemastersGrid2019":
        case "CodemastersAutosport":
        case "CodemastersGridLegends":
          SimHapticsPlugin.CurrentGame = GameId.GLegends;
          SimHapticsPlugin.GameDBText = "Grid";
          sd.TireDiameterSampleCount = -1;
          break;
        case "BeamNgDrive":
          SimHapticsPlugin.CurrentGame = GameId.BeamNG;
          SimHapticsPlugin.GameDBText = "BeamNG";
          sd.TireDiameterSampleCount = -1;
          break;
        case "GPBikes":
          SimHapticsPlugin.CurrentGame = GameId.GPBikes;
          sd.RumbleFromPlugin = true;
          sd.TireDiameterSampleCount = -1;
          break;
        case "MXBikes":
          SimHapticsPlugin.CurrentGame = GameId.MXBikes;
          sd.TireDiameterSampleCount = -1;
          break;
        case "WRCGenerations":
          SimHapticsPlugin.CurrentGame = GameId.WRCGen;
          sd.TireDiameterSampleCount = -1;
          break;
        case "WRCX":
          SimHapticsPlugin.CurrentGame = GameId.WRCX;
          break;
        case "WRC10":
          SimHapticsPlugin.CurrentGame = GameId.WRC10;
          break;
        case "ATS":
          SimHapticsPlugin.CurrentGame = GameId.ATS;
          break;
        case "ETS2":
          SimHapticsPlugin.CurrentGame = GameId.ETS2;
          break;
        case "GranTurismo7":
        case "GranTurismoSport":
          SimHapticsPlugin.CurrentGame = GameId.GranTurismo7;
          SimHapticsPlugin.GameDBText = "GranTurismo7";
          sd.TireDiameterSampleCount = -1;
          break;
        default:
          SimHapticsPlugin.CurrentGame = GameId.Other;
          break;
      }
      this.D.AccHeave = new double[this.D.AccSamples];
      this.D.AccSurge = new double[this.D.AccSamples];
      this.D.AccSway = new double[this.D.AccSamples];
    }

    private static async void FetchCarData(
      string id,
      string category,
      Spec v,
      double redlineFromGame,
      double maxRPMFromGame)
    {
      try
      {
        if (SimHapticsPlugin.FetchStatus == APIStatus.Waiting)
          return;
        SimHapticsPlugin.FetchStatus = APIStatus.Waiting;
        SimHapticsPlugin.LoadFinish = false;
        Logging.Current.Info((object) ("SimHapticsPlugin: Loading " + category + " " + id));
        if (id == null)
          id = "0";
        if (category == null)
          category = "0";
        Uri requestUri = new Uri("https://api.simhaptics.com/data/" + SimHapticsPlugin.GameDBText + "/" + Uri.EscapeDataString(id) + "/" + Uri.EscapeDataString(category));
        HttpResponseMessage async = await SimHapticsPlugin.client.GetAsync(requestUri);
        async.EnsureSuccessStatusCode();
        JObject jobject = (JObject) JsonConvert.DeserializeObject(async.Content.ReadAsStringAsync().Result);
        Logging.Current.Info((object) jobject);
        if (jobject["data"].HasValues)
        {
          JToken jtoken = jobject["data"][(object) 0];
          v.Game = SimHapticsPlugin.GameDBText;
          v.Id = Convert.ToString((object) jtoken[(object) nameof (id)]);
          v.Category = Convert.ToString((object) jtoken[(object) nameof (category)]);
          v.Name = Convert.ToString((object) jtoken[(object) "name"]);
          v.EngineLocation = Convert.ToString((object) jtoken[(object) "loc"]);
          v.PoweredWheels = Convert.ToString((object) jtoken[(object) "drive"]);
          v.EngineConfiguration = Convert.ToString((object) jtoken[(object) "config"]);
          double result1;
          v.EngineCylinders = !double.TryParse(JToken.op_Explicit(jtoken[(object) "cyl"]), out result1) ? 0.0 : result1;
          double result2;
          v.Redline = !double.TryParse(JToken.op_Explicit(jtoken[(object) "redline"]), out result2) ? redlineFromGame : result2;
          double result3;
          v.MaxRPM = !double.TryParse(JToken.op_Explicit(jtoken[(object) "maxrpm"]), out result3) ? maxRPMFromGame : result3;
          double result4;
          v.MaxPower = !double.TryParse(JToken.op_Explicit(jtoken[(object) "hp"]), out result4) ? 333.0 : result4;
          double result5;
          v.ElectricMaxPower = !double.TryParse(JToken.op_Explicit(jtoken[(object) "ehp"]), out result5) ? 0.0 : result5;
          double result6;
          v.Displacement = !double.TryParse(JToken.op_Explicit(jtoken[(object) "cc"]), out result6) ? 3333.0 : result6;
          double result7;
          v.MaxTorque = !double.TryParse(JToken.op_Explicit(jtoken[(object) "nm"]), out result7) ? v.MaxPower : result7;
          if (SimHapticsPlugin.CurrentGame == GameId.Forza)
            v.Id = "Car_" + v.Id;
          Logging.Current.Info((object) ("SimHapticsPlugin: Successfully loaded " + v.Name));
          SimHapticsPlugin.LoadFailCount = 0;
          SimHapticsPlugin.FailedId = "";
          SimHapticsPlugin.FailedCategory = "";
          SimHapticsPlugin.FetchStatus = APIStatus.Success;
        }
        else
        {
          Logging.Current.Info((object) ("SimHapticsPlugin: Failed to load " + id + " : " + category));
          ++SimHapticsPlugin.LoadFailCount;
          if (SimHapticsPlugin.LoadFailCount > 3)
          {
            SimHapticsPlugin.FailedId = SimHapticsPlugin.CurrentGame != GameId.Forza ? id : "Car_" + id;
            SimHapticsPlugin.FailedCategory = category;
            SimHapticsPlugin.FetchStatus = APIStatus.Fail;
          }
          else
            SimHapticsPlugin.FetchStatus = APIStatus.Retry;
        }
      }
      catch (HttpRequestException ex)
      {
        Logging.Current.Error((object) ("SimHapticsPlugin: " + ex.Message));
        SimHapticsPlugin.LoadFailCount = 0;
        SimHapticsPlugin.FetchStatus = APIStatus.Retry;
      }
    }

    public void End(PluginManager pluginManager)
    {
      if (this.Settings.EngineMult.TryGetValue("AllGames", out double _))
        this.Settings.EngineMult.Remove("AllGames");
      if (this.Settings.EngineMult.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.EngineMult == 1.0)
          this.Settings.EngineMult.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.EngineMult[SimHapticsPlugin.GameDBText] = this.D.EngineMult;
      }
      else if (this.D.EngineMult != 1.0)
        this.Settings.EngineMult.Add(SimHapticsPlugin.GameDBText, this.D.EngineMult);
      if (this.Settings.RumbleMult.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.RumbleMult == 1.0)
          this.Settings.RumbleMult.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.RumbleMult[SimHapticsPlugin.GameDBText] = this.D.RumbleMult;
      }
      else if (this.D.RumbleMult != 1.0)
        this.Settings.RumbleMult.Add(SimHapticsPlugin.GameDBText, this.D.RumbleMult);
      if (this.Settings.SuspensionMult.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.SuspensionMult == 1.0)
          this.Settings.SuspensionMult.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.SuspensionMult[SimHapticsPlugin.GameDBText] = this.D.SuspensionMult;
      }
      else if (this.D.SuspensionMult != 1.0)
        this.Settings.SuspensionMult.Add(SimHapticsPlugin.GameDBText, this.D.SuspensionMult);
      if (this.Settings.SuspensionGamma.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.SuspensionGamma == 1.0)
          this.Settings.SuspensionGamma.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.SuspensionGamma[SimHapticsPlugin.GameDBText] = this.D.SuspensionGamma;
      }
      else if (this.D.SuspensionGamma != 1.0)
        this.Settings.SuspensionGamma.Add(SimHapticsPlugin.GameDBText, this.D.SuspensionGamma);
      if (this.Settings.SlipXMult.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.SlipXMult == 1.0)
          this.Settings.SlipXMult.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.SlipXMult[SimHapticsPlugin.GameDBText] = this.D.SlipXMult;
      }
      else if (this.D.SlipXMult != 1.0)
        this.Settings.SlipXMult.Add(SimHapticsPlugin.GameDBText, this.D.SlipXMult);
      if (this.Settings.SlipYMult.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.SlipYMult == 1.0)
          this.Settings.SlipYMult.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.SlipYMult[SimHapticsPlugin.GameDBText] = this.D.SlipYMult;
      }
      else if (this.D.SlipYMult != 1.0)
        this.Settings.SlipYMult.Add(SimHapticsPlugin.GameDBText, this.D.SlipYMult);
      if (this.Settings.SlipXGamma.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.SlipXGamma == 1.0)
          this.Settings.SlipXGamma.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.SlipXGamma[SimHapticsPlugin.GameDBText] = this.D.SlipXGamma;
      }
      else if (this.D.SlipXGamma != 1.0)
        this.Settings.SlipXGamma.Add(SimHapticsPlugin.GameDBText, this.D.SlipXGamma);
      if (this.Settings.SlipYGamma.TryGetValue(SimHapticsPlugin.GameDBText, out double _))
      {
        if (this.D.SlipYGamma == 1.0)
          this.Settings.SlipYGamma.Remove(SimHapticsPlugin.GameDBText);
        else
          this.Settings.SlipYGamma[SimHapticsPlugin.GameDBText] = this.D.SlipYGamma;
      }
      else if (this.D.SlipYGamma != 1.0)
        this.Settings.SlipYGamma.Add(SimHapticsPlugin.GameDBText, this.D.SlipYGamma);
      this.Settings.RumbleMult["AllGames"] = this.D.RumbleMultAll;
      this.Settings.SuspensionGamma["AllGames"] = this.D.SuspensionGammaAll;
      this.Settings.SuspensionMult["AllGames"] = this.D.SuspensionMultAll;
      this.Settings.SlipXMult["AllGames"] = this.D.SlipXMultAll;
      this.Settings.SlipYMult["AllGames"] = this.D.SlipYMultAll;
      this.Settings.Motion["MotionPitchOffset"] = this.D.MotionPitchOffset;
      this.Settings.Motion["MotionPitchMult"] = this.D.MotionPitchMult;
      this.Settings.Motion["MotionPitchGamma"] = this.D.MotionPitchGamma;
      this.Settings.Motion["MotionRollOffset"] = this.D.MotionRollOffset;
      this.Settings.Motion["MotionRollMult"] = this.D.MotionRollMult;
      this.Settings.Motion["MotionRollGamma"] = this.D.MotionRollGamma;
      this.Settings.Motion["MotionYawOffset"] = this.D.MotionYawOffset;
      this.Settings.Motion["MotionYawMult"] = this.D.MotionYawMult;
      this.Settings.Motion["MotionYawGamma"] = this.D.MotionYawGamma;
      this.Settings.Motion["MotionHeaveOffset"] = this.D.MotionHeaveOffset;
      this.Settings.Motion["MotionHeaveMult"] = this.D.MotionHeaveMult;
      this.Settings.Motion["MotionHeaveGamma"] = this.D.MotionHeaveGamma;
      this.Settings.Motion["MotionSurgeOffset"] = this.D.MotionSurgeOffset;
      this.Settings.Motion["MotionSurgeMult"] = this.D.MotionSurgeMult;
      this.Settings.Motion["MotionSurgeGamma"] = this.D.MotionSurgeGamma;
      this.Settings.Motion["MotionSwayOffset"] = this.D.MotionSwayOffset;
      this.Settings.Motion["MotionSwayMult"] = this.D.MotionSwayMult;
      this.Settings.Motion["MotionSwayGamma"] = this.D.MotionSwayGamma;
      IPluginExtensions.SaveCommonSettings<Settings>((IPlugin) this, "Settings", this.Settings);
    }

    public Control GetWPFSettingsControl(PluginManager pluginManager)
    {
      return (Control) new SettingsControl(this);
    }

    public void Init(PluginManager pluginManager)
    {
      SimHapticsPlugin.SimHubVersion = (string) pluginManager.GetPropertyValue("DataCorePlugin.SimHubVersion");
      SimHapticsPlugin.LoadFailCount = 0;
      SimHapticsPlugin.LoadFinish = false;
      SimHapticsPlugin.LoadStatus = DataStatus.None;
      SimHapticsPlugin.FetchStatus = APIStatus.None;
      this.S = new Spec();
      this.D = new SimData()
      {
        GameAltText = "",
        LoadStatusText = "Not Loaded",
        Gear = 0,
        GearPrevious = 0,
        Downshift = false,
        Upshift = false,
        CarInitCount = 0,
        IdleSampleCount = 0,
        RumbleFromPlugin = false,
        SuspensionDistFLP = 0.0,
        SuspensionDistFRP = 0.0,
        SuspensionDistRLP = 0.0,
        SuspensionDistRRP = 0.0,
        AccSamples = 16,
        Acc1 = 0,
        TireDiameterSampleMax = 100,
        SlipXGammaBaseMult = 1.0,
        SlipYGammaBaseMult = 1.0
      };
      this.SetGame(pluginManager, this.D);
      this.Settings = IPluginExtensions.ReadCommonSettings<Settings>((IPlugin) this, "Settings", (Func<Settings>) (() => new Settings()));
      this.D.LockedText = this.Settings.Unlocked ? "Lock" : "Unlock";
      this.Settings.ABSPulseLength = this.Settings.ABSPulseLength > 0 ? this.Settings.ABSPulseLength : 2;
      this.Settings.DownshiftDurationMs = this.Settings.DownshiftDurationMs > 0 ? this.Settings.DownshiftDurationMs : 400;
      this.Settings.UpshiftDurationMs = this.Settings.UpshiftDurationMs > 0 ? this.Settings.UpshiftDurationMs : 400;
      if (this.Settings.EngineMult == null)
        this.Settings.EngineMult = new Dictionary<string, double>();
      double num1;
      this.D.EngineMult = !this.Settings.EngineMult.TryGetValue(SimHapticsPlugin.GameDBText, out num1) ? 1.0 : num1;
      if (this.Settings.EngineMult.TryGetValue("AllGames", out double _))
      {
        this.D.EngineMultAll = 1.0;
      }
      else
      {
        this.D.EngineMultAll = 1.0;
        this.Settings.EngineMult.Add("AllGames", this.D.EngineMultAll);
      }
      if (this.Settings.RumbleMult == null)
        this.Settings.RumbleMult = new Dictionary<string, double>();
      double num2;
      this.D.RumbleMult = !this.Settings.RumbleMult.TryGetValue(SimHapticsPlugin.GameDBText, out num2) ? 1.0 : num2;
      double num3;
      if (this.Settings.RumbleMult.TryGetValue("AllGames", out num3))
      {
        this.D.RumbleMultAll = num3;
      }
      else
      {
        this.D.RumbleMultAll = 5.0;
        this.Settings.RumbleMult.Add("AllGames", this.D.RumbleMultAll);
      }
      if (this.Settings.SuspensionMult == null)
        this.Settings.SuspensionMult = new Dictionary<string, double>();
      double num4;
      this.D.SuspensionMult = !this.Settings.SuspensionMult.TryGetValue(SimHapticsPlugin.GameDBText, out num4) ? 1.0 : num4;
      double num5;
      if (this.Settings.SuspensionMult.TryGetValue("AllGames", out num5))
      {
        this.D.SuspensionMultAll = num5;
      }
      else
      {
        this.D.SuspensionMultAll = 1.5;
        this.Settings.SuspensionMult.Add("AllGames", this.D.SuspensionMultAll);
      }
      if (this.Settings.SuspensionGamma == null)
        this.Settings.SuspensionGamma = new Dictionary<string, double>();
      double num6;
      this.D.SuspensionGamma = !this.Settings.SuspensionGamma.TryGetValue(SimHapticsPlugin.GameDBText, out num6) ? 1.0 : num6;
      double num7;
      if (this.Settings.SuspensionGamma.TryGetValue("AllGames", out num7))
      {
        this.D.SuspensionGammaAll = num7;
      }
      else
      {
        this.D.SuspensionGammaAll = 1.75;
        this.Settings.SuspensionGamma.Add("AllGames", this.D.SuspensionGammaAll);
      }
      if (this.Settings.SlipXMult == null)
        this.Settings.SlipXMult = new Dictionary<string, double>();
      double num8;
      this.D.SlipXMult = !this.Settings.SlipXMult.TryGetValue(SimHapticsPlugin.GameDBText, out num8) ? 1.0 : num8;
      double num9;
      if (this.Settings.SlipXMult.TryGetValue("AllGames", out num9))
      {
        this.D.SlipXMultAll = num9;
      }
      else
      {
        this.D.SlipXMultAll = 1.0;
        this.Settings.SlipXMult.Add("AllGames", this.D.SlipXMultAll);
      }
      if (this.Settings.SlipYMult == null)
        this.Settings.SlipYMult = new Dictionary<string, double>();
      double num10;
      this.D.SlipYMult = !this.Settings.SlipYMult.TryGetValue(SimHapticsPlugin.GameDBText, out num10) ? 1.0 : num10;
      double num11;
      if (this.Settings.SlipYMult.TryGetValue("AllGames", out num11))
      {
        this.D.SlipYMultAll = num11;
      }
      else
      {
        this.D.SlipYMultAll = 1.0;
        this.Settings.SlipYMult.Add("AllGames", this.D.SlipYMultAll);
      }
      if (this.Settings.SlipXGamma == null)
        this.Settings.SlipXGamma = new Dictionary<string, double>();
      double num12;
      this.D.SlipXGamma = !this.Settings.SlipXGamma.TryGetValue(SimHapticsPlugin.GameDBText, out num12) ? 1.0 : num12;
      double num13;
      if (this.Settings.SlipXGamma.TryGetValue("AllGames", out num13))
      {
        this.D.SlipXGammaAll = num13;
      }
      else
      {
        this.D.SlipXGammaAll = 1.0;
        this.Settings.SlipXGamma.Add("AllGames", this.D.SlipXGammaAll);
      }
      if (this.Settings.SlipYGamma == null)
        this.Settings.SlipYGamma = new Dictionary<string, double>();
      double num14;
      this.D.SlipYGamma = !this.Settings.SlipYGamma.TryGetValue(SimHapticsPlugin.GameDBText, out num14) ? 1.0 : num14;
      double num15;
      if (this.Settings.SlipYGamma.TryGetValue("AllGames", out num15))
      {
        this.D.SlipYGammaAll = num15;
      }
      else
      {
        this.D.SlipYGammaAll = 1.0;
        this.Settings.SlipYGamma.Add("AllGames", this.D.SlipYGammaAll);
      }
      if (this.Settings.Motion == null)
        this.Settings.Motion = new Dictionary<string, double>();
      double num16;
      this.D.MotionPitchOffset = this.Settings.Motion.TryGetValue("MotionPitchOffset", out num16) ? num16 : 0.0;
      double num17;
      this.D.MotionPitchMult = this.Settings.Motion.TryGetValue("MotionPitchMult", out num17) ? num17 : 1.6;
      double num18;
      this.D.MotionPitchGamma = this.Settings.Motion.TryGetValue("MotionPitchGamma", out num18) ? num18 : 1.5;
      double num19;
      this.D.MotionRollOffset = this.Settings.Motion.TryGetValue("MotionRollOffset", out num19) ? num19 : 0.0;
      double num20;
      this.D.MotionRollMult = this.Settings.Motion.TryGetValue("MotionRollMult", out num20) ? num20 : 1.2;
      double num21;
      this.D.MotionRollGamma = this.Settings.Motion.TryGetValue("MotionRollGamma", out num21) ? num21 : 1.5;
      double num22;
      this.D.MotionYawOffset = this.Settings.Motion.TryGetValue("MotionYawOffset", out num22) ? num22 : 0.0;
      double num23;
      this.D.MotionYawMult = this.Settings.Motion.TryGetValue("MotionYawMult", out num23) ? num23 : 1.0;
      double num24;
      this.D.MotionYawGamma = this.Settings.Motion.TryGetValue("MotionYawGamma", out num24) ? num24 : 1.0;
      double num25;
      this.D.MotionHeaveOffset = this.Settings.Motion.TryGetValue("MotionHeaveOffset", out num25) ? num25 : 0.0;
      double num26;
      this.D.MotionHeaveMult = this.Settings.Motion.TryGetValue("MotionHeaveMult", out num26) ? num26 : 1.0;
      double num27;
      this.D.MotionHeaveGamma = this.Settings.Motion.TryGetValue("MotionHeaveGamma", out num27) ? num27 : 1.0;
      double num28;
      this.D.MotionSurgeOffset = this.Settings.Motion.TryGetValue("MotionSurgeOffset", out num28) ? num28 : 0.0;
      double num29;
      this.D.MotionSurgeMult = this.Settings.Motion.TryGetValue("MotionSurgeMult", out num29) ? num29 : 1.0;
      double num30;
      this.D.MotionSurgeGamma = this.Settings.Motion.TryGetValue("MotionSurgeGamma", out num30) ? num30 : 1.0;
      double num31;
      this.D.MotionSwayOffset = this.Settings.Motion.TryGetValue("MotionSwayOffset", out num31) ? num31 : 0.0;
      double num32;
      this.D.MotionSwayMult = this.Settings.Motion.TryGetValue("MotionSwayMult", out num32) ? num32 : 1.0;
      double num33;
      this.D.MotionSwayGamma = this.Settings.Motion.TryGetValue("MotionSwayGamma", out num33) ? num33 : 1.0;
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, string>(this, "CarName", (Func<string>) (() => this.S.Name));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, string>(this, "CarId", (Func<string>) (() => this.S.Id));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, string>(this, "Category", (Func<string>) (() => this.S.Category));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "RedlineRPM", (Func<double>) (() => this.S.Redline));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MaxRPM", (Func<double>) (() => this.S.MaxRPM));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, string>(this, "EngineConfig", (Func<string>) (() => this.S.EngineConfiguration));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "EngineCylinders", (Func<double>) (() => this.S.EngineCylinders));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, string>(this, "EngineLocation", (Func<string>) (() => this.S.EngineLocation));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, string>(this, "PoweredWheels", (Func<string>) (() => this.S.PoweredWheels));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "DisplacementCC", (Func<double>) (() => this.S.Displacement));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "PowerTotalHP", (Func<double>) (() => this.S.MaxPower));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "PowerEngineHP", (Func<double>) (() => this.S.MaxPower - this.S.ElectricMaxPower));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "PowerMotorHP", (Func<double>) (() => this.S.ElectricMaxPower));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MaxTorqueNm", (Func<double>) (() => this.S.MaxTorque));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, int>(this, "LoadStatus", (Func<int>) (() => (int) SimHapticsPlugin.LoadStatus));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "EngineLoad", (Func<double>) (() => this.D.EngineLoad));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "IdleRPM", (Func<double>) (() => this.D.IdleRPM));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqHarmonic", (Func<double>) (() => this.D.FreqHarmonic));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqOctave", (Func<double>) (() => this.D.FreqOctave));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqIntervalA1", (Func<double>) (() => this.D.FreqIntervalA1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqIntervalA2", (Func<double>) (() => this.D.FreqIntervalA2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqLFEAdaptive", (Func<double>) (() => this.D.FreqLFEAdaptive));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqPeakA1", (Func<double>) (() => this.D.FreqPeakA1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqPeakB1", (Func<double>) (() => this.D.FreqPeakB1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqPeakA2", (Func<double>) (() => this.D.FreqPeakA2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "FreqPeakB2", (Func<double>) (() => this.D.FreqPeakB2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "Gain1H", (Func<double>) (() => this.D.Gain1H));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "Gain1H2", (Func<double>) (() => this.D.Gain1H2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "Gain2H", (Func<double>) (() => this.D.Gain2H));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "Gain4H", (Func<double>) (() => this.D.Gain4H));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainOctave", (Func<double>) (() => this.D.GainOctave));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainIntervalA1", (Func<double>) (() => this.D.GainIntervalA1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainIntervalA2", (Func<double>) (() => this.D.GainIntervalA2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakA1Front", (Func<double>) (() => this.D.GainPeakA1Front));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakA1Middle", (Func<double>) (() => this.D.GainPeakA1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakA1Rear", (Func<double>) (() => this.D.GainPeakA1Rear));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakA2Front", (Func<double>) (() => this.D.GainPeakA2Front));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakA2Middle", (Func<double>) (() => this.D.GainPeakA2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakA2Rear", (Func<double>) (() => this.D.GainPeakA2Rear));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakB1Front", (Func<double>) (() => this.D.GainPeakB1Front));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakB1Middle", (Func<double>) (() => this.D.GainPeakB1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakB1Rear", (Func<double>) (() => this.D.GainPeakB1Rear));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakB2Front", (Func<double>) (() => this.D.GainPeakB2Front));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakB2Middle", (Func<double>) (() => this.D.GainPeakB2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "GainPeakB2Rear", (Func<double>) (() => this.D.GainPeakB2Rear));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipXFL", (Func<double>) (() => this.D.SlipXFL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipXFR", (Func<double>) (() => this.D.SlipXFR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipXRL", (Func<double>) (() => this.D.SlipXRL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipXRR", (Func<double>) (() => this.D.SlipXRR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipXAll", (Func<double>) (() => this.D.SlipXAll));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipYFL", (Func<double>) (() => this.D.SlipYFL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipYFR", (Func<double>) (() => this.D.SlipYFR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipYRL", (Func<double>) (() => this.D.SlipYRL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipYRR", (Func<double>) (() => this.D.SlipYRR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SlipYAll", (Func<double>) (() => this.D.SlipYAll));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "WheelLockAll", (Func<double>) (() => this.D.WheelLockAll));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "WheelSpinAll", (Func<double>) (() => this.D.WheelSpinAll));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireDiameterFL", (Func<double>) (() => this.D.TireDiameterFL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireDiameterFR", (Func<double>) (() => this.D.TireDiameterFR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireDiameterRL", (Func<double>) (() => this.D.TireDiameterRL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireDiameterRR", (Func<double>) (() => this.D.TireDiameterRR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireSpeedFL", (Func<double>) (() => this.D.WheelSpeedFL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireSpeedFR", (Func<double>) (() => this.D.WheelSpeedFR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireSpeedRL", (Func<double>) (() => this.D.WheelSpeedRL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireSpeedRR", (Func<double>) (() => this.D.WheelSpeedRR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SpeedMs", (Func<double>) (() => this.D.SpeedMs));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireLoadFL", (Func<double>) (() => this.D.WheelLoadFL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireLoadFR", (Func<double>) (() => this.D.WheelLoadFR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireLoadRL", (Func<double>) (() => this.D.WheelLoadRL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "TireLoadRR", (Func<double>) (() => this.D.WheelLoadRR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "YawRate", (Func<double>) (() => this.D.YawRate));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "YawRateAvg", (Func<double>) (() => this.D.YawRateAvg));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreq", (Func<double>) (() => this.D.SuspensionFreq));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR0a", (Func<double>) (() => this.D.SuspensionFreqRa));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR0b", (Func<double>) (() => this.D.SuspensionFreqRb));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR0c", (Func<double>) (() => this.D.SuspensionFreqRc));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR1", (Func<double>) (() => this.D.SuspensionFreqR1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR2", (Func<double>) (() => this.D.SuspensionFreqR2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR3", (Func<double>) (() => this.D.SuspensionFreqR3));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR4", (Func<double>) (() => this.D.SuspensionFreqR4));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFreqR5", (Func<double>) (() => this.D.SuspensionFreqR5));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR0a", (Func<double>) (() => this.D.SuspensionMultRa));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR0b", (Func<double>) (() => this.D.SuspensionMultRb));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR0c", (Func<double>) (() => this.D.SuspensionMultRc));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR1", (Func<double>) (() => this.D.SuspensionMultR1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR2", (Func<double>) (() => this.D.SuspensionMultR2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR3", (Func<double>) (() => this.D.SuspensionMultR3));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR4", (Func<double>) (() => this.D.SuspensionMultR4));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionMultR5", (Func<double>) (() => this.D.SuspensionMultR5));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRumbleMultR0b", (Func<double>) (() => this.D.SuspensionRumbleMultRb));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRumbleMultR0c", (Func<double>) (() => this.D.SuspensionRumbleMultRc));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRumbleMultR1", (Func<double>) (() => this.D.SuspensionRumbleMultR1));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRumbleMultR2", (Func<double>) (() => this.D.SuspensionRumbleMultR2));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRumbleMultR3", (Func<double>) (() => this.D.SuspensionRumbleMultR3));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRumbleMultR4", (Func<double>) (() => this.D.SuspensionRumbleMultR4));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRumbleMultR5", (Func<double>) (() => this.D.SuspensionRumbleMultR5));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFL", (Func<double>) (() => this.D.SuspensionFL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFR", (Func<double>) (() => this.D.SuspensionFR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRL", (Func<double>) (() => this.D.SuspensionRL));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRR", (Func<double>) (() => this.D.SuspensionRR));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionFront", (Func<double>) (() => this.D.SuspensionFront));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRear", (Func<double>) (() => this.D.SuspensionRear));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionLeft", (Func<double>) (() => this.D.SuspensionLeft));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionRight", (Func<double>) (() => this.D.SuspensionRight));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionAll", (Func<double>) (() => this.D.SuspensionAll));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "SuspensionAccAll", (Func<double>) (() => this.D.SuspensionAccAll));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, bool>(this, "RumbleFromPlugin", (Func<bool>) (() => this.D.RumbleFromPlugin));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "RumbleMult", (Func<double>) (() => this.D.RumbleMult));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "RumbleLeft", (Func<double>) (() => this.D.RumbleLeft));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "RumbleRight", (Func<double>) (() => this.D.RumbleRight));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "ABSPulse", (Func<double>) (() => this.D.ABSPulse));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, bool>(this, "Airborne", (Func<bool>) (() => this.D.Airborne));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, int>(this, "Gear", (Func<int>) (() => this.D.Gear));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, int>(this, "Gears", (Func<int>) (() => this.D.Gears));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, bool>(this, "ShiftDown", (Func<bool>) (() => this.D.Downshift));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, bool>(this, "ShiftUp", (Func<bool>) (() => this.D.Upshift));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, int>(this, "WiperStatus", (Func<int>) (() => this.D.WiperStatus));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccHeave", (Func<double>) (() => this.D.AccHeave[this.D.Acc0]));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccSurge", (Func<double>) (() => this.D.AccSurge[this.D.Acc0]));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccSway", (Func<double>) (() => this.D.AccSway[this.D.Acc0]));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccHeave2", (Func<double>) (() => this.D.AccHeave2S));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccSurge2", (Func<double>) (() => this.D.AccSurge2S));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccSway2", (Func<double>) (() => this.D.AccSway2S));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccHeaveAvg", (Func<double>) (() => this.D.AccHeaveAvg));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccSurgeAvg", (Func<double>) (() => this.D.AccSurgeAvg));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "AccSwayAvg", (Func<double>) (() => this.D.AccSwayAvg));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "JerkZ", (Func<double>) (() => this.D.JerkZ));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "JerkY", (Func<double>) (() => this.D.JerkY));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "JerkX", (Func<double>) (() => this.D.JerkX));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "JerkYAvg", (Func<double>) (() => this.D.JerkYAvg));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MPitch", (Func<double>) (() => this.D.MotionPitch));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MRoll", (Func<double>) (() => this.D.MotionRoll));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MYaw", (Func<double>) (() => this.D.MotionYaw));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MHeave", (Func<double>) (() => this.D.MotionHeave));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MSurge", (Func<double>) (() => this.D.MotionSurge));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "MSway", (Func<double>) (() => this.D.MotionSway));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, int>(this, "TireSamples", (Func<int>) (() => this.D.TireDiameterSampleCount));
      IPluginExtensions.AttachDelegate<SimHapticsPlugin, double>(this, "VelocityX", (Func<double>) (() => this.D.VelocityX));
      SimHapticsPlugin.FrameTimeTicks = DateTime.Now.Ticks;
    }
  }
}
