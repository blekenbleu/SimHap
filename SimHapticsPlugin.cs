// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimHapticsPlugin
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sierses.SimHap.Properties;
using SimHub;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace sierses.SimHap
{
	[PluginDescription("Properties for haptic feedback and more")]
	[PluginAuthor("sierses")]
	[PluginName("SimHap")]
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
		private static readonly HttpClient client = new();

		public Spec S { get; set; }

		public SimData D { get; set; }

		public Settings Settings { get; set; }

		public PluginManager PluginManager { get; set; }

		public ImageSource PictureIcon
		{
			get { return this.ToIcon(Resources.SimHapticsShakerStyleIcon_alt012); }
		}

		public ImageSource RPMIcon
		{
			get { return this.ToIcon(Resources._100x100_RPM_White); }
		}

		public ImageSource ImpactsIcon
		{
			get { return this.ToIcon(Resources._100x100_Impacts_White); }
		}

		public ImageSource SuspensionIcon
		{
			get { return this.ToIcon(Resources._100x100_Suspension_White); }
		}

		public ImageSource TractionIcon
		{
			get { return this.ToIcon(Resources._100x100_Traction_White); }
		}

		private List<string[]> Sling(DownloadData data)
		{
			return new List<string[]>
            {
                new string[] { "notes", data.notes },
                new string[] { "cc", data.cc.ToString() },
                new string[] { "nm", data.nm.ToString() },
                new string[] { "ehp", data.ehp.ToString() },
                new string[] { "hp", data.hp.ToString() },
                new string[] { "drive", data.drive },
                new string[] { "config", data.config },
                new string[] { "cyl", data.cyl.ToString() },
                new string[] { "loc", data.loc },
                new string[] { "maxrpm", data.maxrpm.ToString() },
                new string[] { "redline", data.redline.ToString() },
                new string[] { "category", data.category },
                new string[] { "name", data.name },
                new string[] { "id", data.id },
                new string[] { "game", data.game }
            };
		}

		public string LeftMenuTitle => "SimHaptics";

		/// <summary>
        /// Called one time per game data update, contains all normalized game data,
        /// raw data are intentionnally "hidden" under a generic object type (plugins SHOULD NOT USE)
        /// This method is on the critical path, must execute as fast as possible and avoid throwing any error
        /// </summary>
        /// <param name="pluginManager"></param>
        /// <param name="data">Current game data, including present and previous data frames.</param> 
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			FrameCountTicks = FrameCountTicks + DateTime.Now.Ticks - FrameTimeTicks;
			FrameTimeTicks = DateTime.Now.Ticks;
			if (FrameCountTicks > 864000000000L)
				FrameCountTicks = 0L;
			if (null == data.NewData)
				return;

			if (Settings.Unlocked && FrameCountTicks % 2500000L <= 150000L
			 && (data.GameRunning || data.GamePaused || data.GameReplay || data.GameInMenu))
				SetVehiclePerGame(pluginManager, ref data.NewData);
			if (!data.GameRunning || data.OldData == null)
				return;

			D.FPS = (double) pluginManager.GetPropertyValue("DataCorePlugin.DataUpdateFps");
			D.RPMPercent = data.NewData.Rpms * D.InvMaxRPM;
			D.SpeedMs = data.NewData.SpeedKmh * 0.277778;
			D.InvSpeedMs = D.SpeedMs != 0.0 ? 1.0 / D.SpeedMs : 0.0;
			D.Accelerator = data.NewData.Throttle;
			D.Brake = data.NewData.Brake;
			D.Clutch = data.NewData.Clutch;
			D.Handbrake = data.NewData.Handbrake;
			D.BrakeBias = data.NewData.BrakeBias;
			D.BrakeF = D.Brake * (2.0 * D.BrakeBias) * 0.01;
			D.BrakeR = D.Brake * (200.0 - 2.0 * D.BrakeBias) * 0.01;
			D.BrakeVelP = D.BrakeVel;
			D.BrakeVel = (D.Brake - data.OldData.Brake) * D.FPS;
			D.BrakeAcc = (D.BrakeVel - D.BrakeVelP) * D.FPS;
			if (D.CarInitCount < 2)
			{
				D.SuspensionDistFLP = D.SuspensionDistFL;
				D.SuspensionDistFRP = D.SuspensionDistFR;
				D.SuspensionDistRLP = D.SuspensionDistRL;
				D.SuspensionDistRRP = D.SuspensionDistRR;
				D.YawPrev = data.NewData.OrientationYaw;
				D.Yaw = data.NewData.OrientationYaw;
				D.RumbleLeftAvg = 0.0;
				D.RumbleRightAvg = 0.0;
			}
			D.YawPrev = D.Yaw;
			D.Yaw = -data.NewData.OrientationYaw;
			if (D.Yaw > 100.0 && D.YawPrev < -100.0)
				D.YawPrev += 360.0;
			else if (D.Yaw < -100.0 && D.YawPrev > 100.0)
				D.YawPrev -= 360.0;
			D.YawRate = (D.Yaw - D.YawPrev) * D.FPS;
			if (D.YawRateAvg != 0.0)
			{
				if (Math.Abs(D.YawRate) < Math.Abs(D.YawRateAvg * 1.25))
					D.YawRateAvg = (D.YawRateAvg + D.YawRate) * 0.5;
				else
					D.YawRateAvg *= 1.25;
			}
			else D.YawRateAvg = D.YawRate;
			++D.Acc0;
			D.Acc1 = D.Acc0 - 1;
			if (D.Acc0 >= D.AccSamples)
			{
				D.Acc0 = 0;
				D.Acc1 = D.AccSamples - 1;
			}
			D.AccHeave[D.Acc0] = data.NewData.AccelerationHeave.GetValueOrDefault();
			D.AccSurge[D.Acc0] = data.NewData.AccelerationSurge.GetValueOrDefault();
			D.AccSway[D.Acc0] = data.NewData.AccelerationSway.GetValueOrDefault();
			if (!data.NewData.AccelerationHeave.HasValue)
			{
				D.AccHeave[D.Acc0] = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WorldSpeedY");
				D.AccHeave[D.Acc0] = (D.AccHeave[D.Acc0] - D.WorldSpeedY) * D.FPS;
				D.WorldSpeedY = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WorldSpeedY");
			}
			D.AccHeave2S = (D.AccHeave[D.Acc0] + D.AccHeave[D.Acc1]) * 0.5;
			D.AccSurge2S = (D.AccSurge[D.Acc0] + D.AccSurge[D.Acc1]) * 0.5;
			D.AccSway2S = (D.AccSway[D.Acc0] + D.AccSway[D.Acc1]) * 0.5;
			D.JerkZ = (D.AccHeave[D.Acc0] - D.AccHeave[D.Acc1]) * D.FPS;
			D.JerkY = (D.AccSurge[D.Acc0] - D.AccSurge[D.Acc1]) * D.FPS;
			D.JerkX = (D.AccSway[D.Acc0] - D.AccSway[D.Acc1]) * D.FPS;
			double num1 = 1.0 / 16.0;
			double accSurgeAvg = D.AccSurgeAvg;
			D.AccHeaveAvg = 0.0;
			D.AccSurgeAvg = 0.0;
			D.AccSwayAvg = 0.0;
			for (int index = 0; index < D.AccSamples; ++index)
			{
				D.AccHeaveAvg += D.AccHeave[index];
				D.AccSurgeAvg += D.AccSurge[index];
				D.AccSwayAvg += D.AccSway[index];
			}
			D.AccHeaveAvg *= num1;
			D.AccSurgeAvg *= num1;
			D.AccSwayAvg *= num1;
			D.JerkYAvg = (D.AccSurgeAvg - accSurgeAvg) * D.FPS;
			D.AccHeaveAbs = Math.Abs(D.AccHeave[D.Acc0]);
			D.InvAccSurgeAvg = D.AccSurgeAvg != 0.0 ? 1.0 / D.AccSurgeAvg : 0.0;
			D.MotionPitch = D.MotionPitchOffset + 100.0 * Math.Pow(Math.Abs(D.MotionPitchMult * data.NewData.OrientationPitch) * 0.01, 1.0 / D.MotionPitchGamma);
			D.MotionRoll = D.MotionRollOffset + 100.0 * Math.Pow(Math.Abs(D.MotionRollMult * data.NewData.OrientationRoll) * 0.01, 1.0 / D.MotionRollGamma);
			D.MotionYaw = D.MotionYawOffset + 100.0 * Math.Pow(Math.Abs(D.MotionYawMult * D.YawRateAvg) * 0.01, 1.0 / D.MotionYawGamma);
			D.MotionHeave = D.MotionHeaveOffset + 100.0 * Math.Pow(Math.Abs(D.MotionHeaveMult * D.AccHeave[D.Acc0]) * 0.01, 1.0 / D.MotionHeaveGamma);
			if (data.NewData.OrientationPitch < 0.0)
				D.MotionPitch = -D.MotionPitch;
			if (data.NewData.OrientationRoll < 0.0)
				D.MotionRoll = -D.MotionRoll;
			if (D.YawRateAvg < 0.0)
				D.MotionYaw = -D.MotionYaw;
			if (D.AccHeave[D.Acc0] < 0.0)
				D.MotionHeave = -D.MotionHeave;
			D.WheelLoadFL = ((100.0 + D.AccSurge[D.Acc0]) * (100.0 - D.AccSway[D.Acc0]) * 0.01 - 50.0) * 0.01;
			D.WheelLoadFR = ((100.0 + D.AccSurge[D.Acc0]) * (100.0 + D.AccSway[D.Acc0]) * 0.01 - 50.0) * 0.01;
			D.WheelLoadRL = ((100.0 - D.AccSurge[D.Acc0]) * (100.0 - D.AccSway[D.Acc0]) * 0.01 - 50.0) * 0.01;
			D.WheelLoadRR = ((100.0 - D.AccSurge[D.Acc0]) * (100.0 + D.AccSway[D.Acc0]) * 0.01 - 50.0) * 0.01;
			UpdateVehiclePerGame(pluginManager, ref data);
			D.Airborne = D.AccHeave2S < -2.0 || Math.Abs(data.NewData.OrientationRoll) > 60.0;
			if (D.Airborne && D.SuspensionFL < 0.1)
				D.SlipXFL = D.SlipYFL = 0.0;
			else
			{
				D.SlipXFL = D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(D.SlipXFL, 0.0), 1.0 / (D.SlipXGammaBaseMult * D.SlipXGamma * D.SlipXGammaAll));
				D.SlipYFL = D.SlipYFL < 0.0
							 ? D.SlipYMultAll * -100.0 * Math.Pow(-D.SlipYFL, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll))
							 : D.SlipYMultAll * 100.0 * Math.Pow(D.SlipYFL, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll));
			}
			if (D.Airborne && D.SuspensionFR < 0.1)
				D.SlipXFR = D.SlipYFR = 0.0;
			else
			{
				D.SlipXFR = D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(D.SlipXFR, 0.0), 1.0 / (D.SlipXGammaBaseMult * D.SlipXGamma * D.SlipXGammaAll));
				D.SlipYFR = D.SlipYFR < 0.0
							 ? D.SlipYMultAll * -100.0 * Math.Pow(-D.SlipYFR, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll))
							 : D.SlipYMultAll * 100.0 * Math.Pow(D.SlipYFR, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll));
			}
			if (D.Airborne && D.SuspensionRL < 0.1)
				D.SlipXRL = D.SlipYRL = 0.0;
			else
			{
				D.SlipXRL = D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(D.SlipXRL, 0.0), 1.0 / (D.SlipXGammaBaseMult * D.SlipXGamma * D.SlipXGammaAll));
				D.SlipYRL = D.SlipYRL < 0.0
							 ? D.SlipYMultAll * -100.0 * Math.Pow(-D.SlipYRL, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll))
							 : D.SlipYMultAll * 100.0 * Math.Pow(D.SlipYRL, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll));
			}
			if (D.Airborne && D.SuspensionRR < 0.1)
				D.SlipXRR = D.SlipYRR = 0.0;
			else
			{
				D.SlipXRR = D.SlipXMultAll * 100.0 * Math.Pow(Math.Max(D.SlipXRR, 0.0), 1.0 / (D.SlipXGammaBaseMult * D.SlipXGamma * D.SlipXGammaAll));
				D.SlipYRR = D.SlipYRR < 0.0
							 ? D.SlipYMultAll * -100.0 * Math.Pow(-D.SlipYRR, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll))
							 : D.SlipYMultAll * 100.0 * Math.Pow(D.SlipYRR, 1.0 / (D.SlipYGammaBaseMult * D.SlipYGamma * D.SlipYGammaAll));
			}
			D.Airborne = D.Airborne && D.SuspensionAll < 0.1;
			D.SlipXAll = (D.SlipXFL + D.SlipXFR + D.SlipXRL + D.SlipXRR) * 0.5;
			D.SlipYAll = (D.SlipYFL + D.SlipYFR + D.SlipYRL + D.SlipYRR) * 0.5;
			D.WheelSpinAll = !(S.PoweredWheels == "F")
				? (!(S.PoweredWheels == "R")
					? (Math.Max(-D.SlipYFL, 0.0) + Math.Max(-D.SlipYFR, 0.0) + Math.Max(-D.SlipYRL, 0.0) + Math.Max(-D.SlipYRR, 0.0)) * 0.25
					: (Math.Max(-D.SlipYRL, 0.0) + Math.Max(-D.SlipYRR, 0.0)) * 0.5)
				: (Math.Max(-D.SlipYFL, 0.0) + Math.Max(-D.SlipYFR, 0.0)) * 0.5;
			D.WheelLockAll = 0.0;
			if (D.SlipYFL > 50.0)
				D.WheelLockAll += D.SlipYFL - 50.0;
			if (D.SlipYFR > 50.0)
				D.WheelLockAll += D.SlipYFR - 50.0;
			if (D.SlipYRL > 50.0)
				D.WheelLockAll += D.SlipYRL - 50.0;
			if (D.SlipYRR > 50.0)
				D.WheelLockAll += D.SlipYRR - 50.0;
			if (DateTime.Now.Ticks - D.ShiftTicks > Settings.DownshiftDurationMs * 10000)
				D.Downshift = false;
			if (DateTime.Now.Ticks - D.ShiftTicks > Settings.UpshiftDurationMs * 10000)
				D.Upshift = false;
			DateTime now;
			if (data.OldData.Gear != data.NewData.Gear)
			{
				if (D.Gear != 0)
					D.GearPrevious = D.Gear;
				D.Gear = !(data.NewData.Gear == "N")
						? (!(data.NewData.Gear == "R")
							? Convert.ToInt32(data.NewData.Gear)
							: -1)
						: 0;
				if (D.Gear != 0)
				{
					if (D.Gear < D.GearPrevious)
					{
						D.Downshift = true;
						SimData d = D;
						now = DateTime.Now;
						long ticks = now.Ticks;
						d.ShiftTicks = ticks;
					}
					else if (D.Gear > D.GearPrevious)
					{
						D.Upshift = true;
						SimData d = D;
						now = DateTime.Now;
						long ticks = now.Ticks;
						d.ShiftTicks = ticks;
					}
				}
			}
			D.ABSPauseInterval = D.SlipYAll <= 0.0
								? (long) (1166667.0 - 666667.0 * ((data.NewData.SpeedKmh - 20.0) * 0.003333333).Clamp(0.0, 1.0))
								: (long) (1250000.0 - 666667.0 * D.SlipYAll.Clamp(0.0, 1.0));
			D.ABSPulseInterval = 166666L * Settings.ABSPulseLength;
			if (D.ABSActive)
			{
				if (D.ABSTicks <= 0L)
				{
					SimData d = D;
					now = DateTime.Now;
					long ticks = now.Ticks;
					d.ABSTicks = ticks;
				}
				now = DateTime.Now;
				if (now.Ticks - D.ABSTicks < D.ABSPulseInterval)
					D.ABSPulse = 100.0;
				else
				{
					now = DateTime.Now;
					if (now.Ticks - D.ABSTicks < D.ABSPauseInterval)
						D.ABSPulse = 0.0;
					else
					{
						D.ABSPulse = 100.0;
						SimData d = D;
						now = DateTime.Now;
						long ticks = now.Ticks;
						d.ABSTicks = ticks;
					}
				}
			}
			else
			{
				D.ABSPulse = 0.0;
				D.ABSTicks = -1L;
			}
			D.SuspensionAccFLP = D.SuspensionAccFL;
			D.SuspensionAccFRP = D.SuspensionAccFR;
			D.SuspensionAccRLP = D.SuspensionAccRL;
			D.SuspensionAccRRP = D.SuspensionAccRR;
			D.SuspensionAccFL = (D.SuspensionVelFL - D.SuspensionVelFLP) * D.FPS;
			D.SuspensionAccFR = (D.SuspensionVelFR - D.SuspensionVelFRP) * D.FPS;
			D.SuspensionAccRL = (D.SuspensionVelRL - D.SuspensionVelRLP) * D.FPS;
			D.SuspensionAccRR = (D.SuspensionVelRR - D.SuspensionVelRRP) * D.FPS;
			D.SuspensionFL = 10.0 * D.SuspensionMult * D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(D.SuspensionVelFL, 0.0) * 0.01, 1.0 / (D.SuspensionGamma * D.SuspensionGammaAll));
			D.SuspensionFR = 10.0 * D.SuspensionMult * D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(D.SuspensionVelFR, 0.0) * 0.01, 1.0 / (D.SuspensionGamma * D.SuspensionGammaAll));
			D.SuspensionRL = 10.0 * D.SuspensionMult * D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(D.SuspensionVelRL, 0.0) * 0.01, 1.0 / (D.SuspensionGamma * D.SuspensionGammaAll));
			D.SuspensionRR = 10.0 * D.SuspensionMult * D.SuspensionMultAll * 100.0 * Math.Pow(Math.Max(D.SuspensionVelRR, 0.0) * 0.01, 1.0 / (D.SuspensionGamma * D.SuspensionGammaAll));
			D.SuspensionFront = D.SuspensionFL + D.SuspensionFR;
			D.SuspensionRear = D.SuspensionRL + D.SuspensionRR;
			D.SuspensionLeft = D.SuspensionFL + D.SuspensionRL;
			D.SuspensionRight = D.SuspensionFR + D.SuspensionRR;
			D.SuspensionAll = (D.SuspensionFL + D.SuspensionFR + D.SuspensionRL + D.SuspensionRR) * 0.5;
			D.SuspensionAccAll = (D.SuspensionAccFL + D.SuspensionAccFR + D.SuspensionAccRL + D.SuspensionAccRR) * 0.5;
			if (D.CarInitCount < 10 && FrameCountTicks % 2000000L <= 150000L)
			{
				D.SuspensionFL *= 0.1 * D.CarInitCount;
				D.SuspensionFR *= 0.1 * D.CarInitCount;
				D.SuspensionRL *= 0.1 * D.CarInitCount;
				D.SuspensionRR *= 0.1 * D.CarInitCount;
				++D.CarInitCount;
			}
			D.SuspensionFreq = data.NewData.SpeedKmh * (3.0 / 16.0);
			double num2 = 94.0 + 0.4 * D.SpeedMs;
			double num3 = 76.0 + 0.45 * D.SpeedMs;
			double num4 = 60.0 + 0.5 * D.SpeedMs;
			double num5 = 46.0 + 0.55 * D.SpeedMs;
			double num6 = 34.0 + 0.6 * D.SpeedMs;
			double num7 = 24.0 + 0.65 * D.SpeedMs;
			double num8 = 16.0 + 0.7 * D.SpeedMs;
			double num9 = 10.0 + 0.75 * D.SpeedMs;
			double num10 = 0.55 + 1.8 * D.AccHeaveAbs * (D.AccHeaveAbs + num2) / (num2 * num2);
			double num11 = 0.5 + 2.0 * D.AccHeaveAbs * (D.AccHeaveAbs + num3) / (num3 * num3);
			double num12 = 0.45 + 2.2 * D.AccHeaveAbs * (D.AccHeaveAbs + num4) / (num4 * num4);
			double num13 = 0.4 + 2.4 * D.AccHeaveAbs * (D.AccHeaveAbs + num5) / (num5 * num5);
			double num14 = 0.5 + 2.0 * D.AccHeaveAbs * (D.AccHeaveAbs + num6) / (num6 * num6);
			double num15 = 0.6 + 1.6 * D.AccHeaveAbs * (D.AccHeaveAbs + num7) / (num7 * num7);
			double num16 = 0.7 + 1.2 * D.AccHeaveAbs * (D.AccHeaveAbs + num8) / (num8 * num8);
			double num17 = 0.8 + 0.8 * D.AccHeaveAbs * (D.AccHeaveAbs + num9) / (num9 * num9);
			double num18 = D.RumbleMult * D.RumbleMultAll * (0.6 + D.SpeedMs * (90.0 - D.SpeedMs) * 0.0002);
			if (D.SuspensionFreq < 30.0)
			{
				if (D.SuspensionFreq < 20.0)
				{
					if (D.SuspensionFreq < 15.0)
					{
						if (D.SuspensionFreq < 10.0)
						{
							if (D.SuspensionFreq < 7.5)
							{
								if (D.SuspensionFreq < 3.75)
								{
									D.SuspensionFreq *= 4.0;
									D.SuspensionFreqRa = D.SuspensionFreq * 0.715;
									D.SuspensionFreqRb = D.SuspensionFreq * 1.0;
									D.SuspensionFreqRc = D.SuspensionFreq * 1.43;
									D.SuspensionFreqR1 = D.SuspensionFreq * 2.0;
									D.SuspensionFreqR2 = D.SuspensionFreq * 2.86;
									D.SuspensionFreqR3 = D.SuspensionFreq * 4.0;
									D.SuspensionFreqR4 = D.SuspensionFreq * 5.72;
									D.SuspensionFreqR5 = D.SuspensionFreq * 8.0;
									D.SuspensionMultRa = num10 * 0.5;
									D.SuspensionMultRb = num11 * 1.0;
									D.SuspensionMultRc = num12 * 0.5;
									D.SuspensionMultR1 = num13 * 0.8;
									D.SuspensionMultR2 = num14 * 0.25;
									D.SuspensionMultR3 = num15 * 0.6;
									D.SuspensionMultR4 = num16 * 0.125;
									D.SuspensionMultR5 = num17 * 0.4;
									D.SuspensionRumbleMultRa = num18 * 0.0;
									D.SuspensionRumbleMultRb = num18 * 2.0;
									D.SuspensionRumbleMultRc = num18 * 0.0;
									D.SuspensionRumbleMultR1 = num18 * 1.5;
									D.SuspensionRumbleMultR2 = num18 * 0.0;
									D.SuspensionRumbleMultR3 = num18 * 1.0;
									D.SuspensionRumbleMultR4 = num18 * 0.0;
									D.SuspensionRumbleMultR5 = num18 * 0.5;
								}
								else
								{
									D.SuspensionFreq *= 2.0;
									D.SuspensionFreqRa = D.SuspensionFreq * 0.715;
									D.SuspensionFreqRb = D.SuspensionFreq * 1.0;
									D.SuspensionFreqRc = D.SuspensionFreq * 1.43;
									D.SuspensionFreqR1 = D.SuspensionFreq * 2.0;
									D.SuspensionFreqR2 = D.SuspensionFreq * 2.86;
									D.SuspensionFreqR3 = D.SuspensionFreq * 4.0;
									D.SuspensionFreqR4 = D.SuspensionFreq * 5.72;
									D.SuspensionFreqR5 = D.SuspensionFreq * 8.0;
									D.SuspensionMultRa = num10 * 0.5;
									D.SuspensionMultRb = num11 * 1.0;
									D.SuspensionMultRc = num12 * 0.5;
									D.SuspensionMultR1 = num13 * 0.8;
									D.SuspensionMultR2 = num14 * 0.25;
									D.SuspensionMultR3 = num15 * 0.6;
									D.SuspensionMultR4 = num16 * 0.125;
									D.SuspensionMultR5 = num17 * 0.4;
									D.SuspensionRumbleMultRa = num18 * 0.0;
									D.SuspensionRumbleMultRb = num18 * 2.0;
									D.SuspensionRumbleMultRc = num18 * 0.0;
									D.SuspensionRumbleMultR1 = num18 * 1.5;
									D.SuspensionRumbleMultR2 = num18 * 0.0;
									D.SuspensionRumbleMultR3 = num18 * 1.0;
									D.SuspensionRumbleMultR4 = num18 * 0.0;
									D.SuspensionRumbleMultR5 = num18 * 0.5;
								}
							}
							else
							{
								D.SuspensionFreqRa = D.SuspensionFreq * 1.0;
								D.SuspensionFreqRb = D.SuspensionFreq * 1.43;
								D.SuspensionFreqRc = D.SuspensionFreq * 2.0;
								D.SuspensionFreqR1 = D.SuspensionFreq * 2.86;
								D.SuspensionFreqR2 = D.SuspensionFreq * 4.0;
								D.SuspensionFreqR3 = D.SuspensionFreq * 5.72;
								D.SuspensionFreqR4 = D.SuspensionFreq * 8.0;
								D.SuspensionFreqR5 = D.SuspensionFreq * 11.44;
								D.SuspensionMultRa = num10 * 1.0;
								D.SuspensionMultRb = num11 * 0.5;
								D.SuspensionMultRc = num12 * 0.8;
								D.SuspensionMultR1 = num13 * 0.25;
								D.SuspensionMultR2 = num14 * 0.6;
								D.SuspensionMultR3 = num15 * 0.125;
								D.SuspensionMultR4 = num16 * 0.4;
								D.SuspensionMultR5 = num17 * (1.0 / 16.0);
								D.SuspensionRumbleMultRa = num18 * 2.0;
								D.SuspensionRumbleMultRb = num18 * 0.0;
								D.SuspensionRumbleMultRc = num18 * 1.5;
								D.SuspensionRumbleMultR1 = num18 * 0.0;
								D.SuspensionRumbleMultR2 = num18 * 1.0;
								D.SuspensionRumbleMultR3 = num18 * 0.0;
								D.SuspensionRumbleMultR4 = num18 * 0.5;
								D.SuspensionRumbleMultR5 = num18 * 0.0;
							}
						}
						else
						{
							D.SuspensionFreqRa = D.SuspensionFreq * 0.715;
							D.SuspensionFreqRb = D.SuspensionFreq * 1.0;
							D.SuspensionFreqRc = D.SuspensionFreq * 1.43;
							D.SuspensionFreqR1 = D.SuspensionFreq * 2.0;
							D.SuspensionFreqR2 = D.SuspensionFreq * 2.86;
							D.SuspensionFreqR3 = D.SuspensionFreq * 4.0;
							D.SuspensionFreqR4 = D.SuspensionFreq * 5.72;
							D.SuspensionFreqR5 = D.SuspensionFreq * 8.0;
							D.SuspensionMultRa = num10 * 0.5;
							D.SuspensionMultRb = num11 * 1.0;
							D.SuspensionMultRc = num12 * 0.5;
							D.SuspensionMultR1 = num13 * 0.8;
							D.SuspensionMultR2 = num14 * 0.25;
							D.SuspensionMultR3 = num15 * 0.6;
							D.SuspensionMultR4 = num16 * 0.125;
							D.SuspensionMultR5 = num17 * 0.4;
							D.SuspensionRumbleMultRa = num18 * 0.0;
							D.SuspensionRumbleMultRb = num18 * 2.0;
							D.SuspensionRumbleMultRc = num18 * 0.0;
							D.SuspensionRumbleMultR1 = num18 * 1.5;
							D.SuspensionRumbleMultR2 = num18 * 0.0;
							D.SuspensionRumbleMultR3 = num18 * 1.0;
							D.SuspensionRumbleMultR4 = num18 * 0.0;
							D.SuspensionRumbleMultR5 = num18 * 0.5;
						}
					}
					else
					{
						D.SuspensionFreqRa = D.SuspensionFreq * 0.5;
						D.SuspensionFreqRb = D.SuspensionFreq * 0.715;
						D.SuspensionFreqRc = D.SuspensionFreq * 1.0;
						D.SuspensionFreqR1 = D.SuspensionFreq * 1.43;
						D.SuspensionFreqR2 = D.SuspensionFreq * 2.0;
						D.SuspensionFreqR3 = D.SuspensionFreq * 2.86;
						D.SuspensionFreqR4 = D.SuspensionFreq * 4.0;
						D.SuspensionFreqR5 = D.SuspensionFreq * 5.72;
						D.SuspensionMultRa = num10 * 0.8;
						D.SuspensionMultRb = num11 * 0.5;
						D.SuspensionMultRc = num12 * 1.0;
						D.SuspensionMultR1 = num13 * 0.5;
						D.SuspensionMultR2 = num14 * 0.8;
						D.SuspensionMultR3 = num15 * 0.25;
						D.SuspensionMultR4 = num16 * 0.6;
						D.SuspensionMultR5 = num17 * 0.125;
						D.SuspensionRumbleMultRa = num18 * 1.5;
						D.SuspensionRumbleMultRb = num18 * 0.0;
						D.SuspensionRumbleMultRc = num18 * 2.0;
						D.SuspensionRumbleMultR1 = num18 * 0.0;
						D.SuspensionRumbleMultR2 = num18 * 1.5;
						D.SuspensionRumbleMultR3 = num18 * 0.0;
						D.SuspensionRumbleMultR4 = num18 * 1.0;
						D.SuspensionRumbleMultR5 = num18 * 0.0;
					}
				}
				else
				{
					D.SuspensionFreqRa = D.SuspensionFreq * (143.0 / 400.0);
					D.SuspensionFreqRb = D.SuspensionFreq * 0.5;
					D.SuspensionFreqRc = D.SuspensionFreq * 0.715;
					D.SuspensionFreqR1 = D.SuspensionFreq * 1.0;
					D.SuspensionFreqR2 = D.SuspensionFreq * 1.43;
					D.SuspensionFreqR3 = D.SuspensionFreq * 2.0;
					D.SuspensionFreqR4 = D.SuspensionFreq * 2.86;
					D.SuspensionFreqR5 = D.SuspensionFreq * 4.0;
					D.SuspensionMultRa = num10 * 0.25;
					D.SuspensionMultRb = num11 * 0.8;
					D.SuspensionMultRc = num12 * 0.5;
					D.SuspensionMultR1 = num13 * 1.0;
					D.SuspensionMultR2 = num14 * 0.5;
					D.SuspensionMultR3 = num15 * 0.8;
					D.SuspensionMultR4 = num16 * 0.25;
					D.SuspensionMultR5 = num17 * 0.6;
					D.SuspensionRumbleMultRa = num18 * 0.0;
					D.SuspensionRumbleMultRb = num18 * 1.5;
					D.SuspensionRumbleMultRc = num18 * 0.0;
					D.SuspensionRumbleMultR1 = num18 * 2.0;
					D.SuspensionRumbleMultR2 = num18 * 0.0;
					D.SuspensionRumbleMultR3 = num18 * 1.5;
					D.SuspensionRumbleMultR4 = num18 * 0.0;
					D.SuspensionRumbleMultR5 = num18 * 1.0;
				}
			}
			else if (D.SuspensionFreq > 40.0)
			{
				if (D.SuspensionFreq > 60.0)
				{
					if (D.SuspensionFreq > 80.0)
					{
						if (D.SuspensionFreq > 120.0)
						{
							D.SuspensionFreqRa = D.SuspensionFreq * (1.0 / 16.0);
							D.SuspensionFreqRb = D.SuspensionFreq * 0.089375;
							D.SuspensionFreqRc = D.SuspensionFreq * 0.125;
							D.SuspensionFreqR1 = D.SuspensionFreq * (143.0 / 800.0);
							D.SuspensionFreqR2 = D.SuspensionFreq * 0.25;
							D.SuspensionFreqR3 = D.SuspensionFreq * (143.0 / 400.0);
							D.SuspensionFreqR4 = D.SuspensionFreq * 0.5;
							D.SuspensionFreqR5 = D.SuspensionFreq * 0.715;
							D.SuspensionMultRa = num10 * 0.2;
							D.SuspensionMultRb = num11 * (1.0 / 16.0);
							D.SuspensionMultRc = num12 * 0.4;
							D.SuspensionMultR1 = num13 * 0.125;
							D.SuspensionMultR2 = num14 * 0.6;
							D.SuspensionMultR3 = num15 * 0.25;
							D.SuspensionMultR4 = num16 * 0.8;
							D.SuspensionMultR5 = num17 * 0.5;
							D.SuspensionRumbleMultRa = num18 * 0.3;
							D.SuspensionRumbleMultRb = num18 * 0.0;
							D.SuspensionRumbleMultRc = num18 * 0.5;
							D.SuspensionRumbleMultR1 = num18 * 0.0;
							D.SuspensionRumbleMultR2 = num18 * 1.0;
							D.SuspensionRumbleMultR3 = num18 * 0.0;
							D.SuspensionRumbleMultR4 = num18 * 1.5;
							D.SuspensionRumbleMultR5 = num18 * 0.0;
						}
						else
						{
							D.SuspensionFreqRa = D.SuspensionFreq * 0.089375;
							D.SuspensionFreqRb = D.SuspensionFreq * 0.125;
							D.SuspensionFreqRc = D.SuspensionFreq * (143.0 / 800.0);
							D.SuspensionFreqR1 = D.SuspensionFreq * 0.25;
							D.SuspensionFreqR2 = D.SuspensionFreq * (143.0 / 400.0);
							D.SuspensionFreqR3 = D.SuspensionFreq * 0.5;
							D.SuspensionFreqR4 = D.SuspensionFreq * 0.715;
							D.SuspensionFreqR5 = D.SuspensionFreq * 1.0;
							D.SuspensionMultRa = num10 * (1.0 / 16.0);
							D.SuspensionMultRb = num11 * 0.4;
							D.SuspensionMultRc = num12 * 0.125;
							D.SuspensionMultR1 = num13 * 0.6;
							D.SuspensionMultR2 = num14 * 0.25;
							D.SuspensionMultR3 = num15 * 0.8;
							D.SuspensionMultR4 = num16 * 0.5;
							D.SuspensionMultR5 = num17 * 1.0;
							D.SuspensionRumbleMultRa = num18 * 0.0;
							D.SuspensionRumbleMultRb = num18 * 0.5;
							D.SuspensionRumbleMultRc = num18 * 0.0;
							D.SuspensionRumbleMultR1 = num18 * 1.0;
							D.SuspensionRumbleMultR2 = num18 * 0.0;
							D.SuspensionRumbleMultR3 = num18 * 1.5;
							D.SuspensionRumbleMultR4 = num18 * 0.0;
							D.SuspensionRumbleMultR5 = num18 * 2.0;
						}
					}
					else
					{
						D.SuspensionFreqRa = D.SuspensionFreq * 0.125;
						D.SuspensionFreqRb = D.SuspensionFreq * (143.0 / 800.0);
						D.SuspensionFreqRc = D.SuspensionFreq * 0.25;
						D.SuspensionFreqR1 = D.SuspensionFreq * (143.0 / 400.0);
						D.SuspensionFreqR2 = D.SuspensionFreq * 0.5;
						D.SuspensionFreqR3 = D.SuspensionFreq * 0.715;
						D.SuspensionFreqR4 = D.SuspensionFreq * 1.0;
						D.SuspensionFreqR5 = D.SuspensionFreq * 1.43;
						D.SuspensionMultRa = num10 * 0.4;
						D.SuspensionMultRb = num11 * 0.125;
						D.SuspensionMultRc = num12 * 0.6;
						D.SuspensionMultR1 = num13 * 0.25;
						D.SuspensionMultR2 = num14 * 0.8;
						D.SuspensionMultR3 = num15 * 0.5;
						D.SuspensionMultR4 = num16 * 1.0;
						D.SuspensionMultR5 = num17 * 0.5;
						D.SuspensionRumbleMultRa = num18 * 0.5;
						D.SuspensionRumbleMultRb = num18 * 0.0;
						D.SuspensionRumbleMultRc = num18 * 1.0;
						D.SuspensionRumbleMultR1 = num18 * 0.0;
						D.SuspensionRumbleMultR2 = num18 * 1.5;
						D.SuspensionRumbleMultR3 = num18 * 0.0;
						D.SuspensionRumbleMultR4 = num18 * 2.0;
						D.SuspensionRumbleMultR5 = num18 * 0.0;
					}
				}
				else
				{
					D.SuspensionFreqRa = D.SuspensionFreq * (143.0 / 800.0);
					D.SuspensionFreqRb = D.SuspensionFreq * 0.25;
					D.SuspensionFreqRc = D.SuspensionFreq * (143.0 / 400.0);
					D.SuspensionFreqR1 = D.SuspensionFreq * 0.5;
					D.SuspensionFreqR2 = D.SuspensionFreq * 0.715;
					D.SuspensionFreqR3 = D.SuspensionFreq * 1.0;
					D.SuspensionFreqR4 = D.SuspensionFreq * 1.43;
					D.SuspensionFreqR5 = D.SuspensionFreq * 2.0;
					D.SuspensionMultRa = num10 * 0.125;
					D.SuspensionMultRb = num11 * 0.6;
					D.SuspensionMultRc = num12 * 0.25;
					D.SuspensionMultR1 = num13 * 0.8;
					D.SuspensionMultR2 = num14 * 0.5;
					D.SuspensionMultR3 = num15 * 1.0;
					D.SuspensionMultR4 = num16 * 0.5;
					D.SuspensionMultR5 = num17 * 0.8;
					D.SuspensionRumbleMultRa = num18 * 0.0;
					D.SuspensionRumbleMultRb = num18 * 1.0;
					D.SuspensionRumbleMultRc = num18 * 0.0;
					D.SuspensionRumbleMultR1 = num18 * 1.5;
					D.SuspensionRumbleMultR2 = num18 * 0.0;
					D.SuspensionRumbleMultR3 = num18 * 2.0;
					D.SuspensionRumbleMultR4 = num18 * 0.0;
					D.SuspensionRumbleMultR5 = num18 * 1.5;
				}
			}
			else
			{
				D.SuspensionFreqRa = D.SuspensionFreq * 0.25;
				D.SuspensionFreqRb = D.SuspensionFreq * (143.0 / 400.0);
				D.SuspensionFreqRc = D.SuspensionFreq * 0.5;
				D.SuspensionFreqR1 = D.SuspensionFreq * 0.715;
				D.SuspensionFreqR2 = D.SuspensionFreq * 1.0;
				D.SuspensionFreqR3 = D.SuspensionFreq * 1.43;
				D.SuspensionFreqR4 = D.SuspensionFreq * 2.0;
				D.SuspensionFreqR5 = D.SuspensionFreq * 2.86;
				D.SuspensionMultRa = num10 * 0.6;
				D.SuspensionMultRb = num11 * 0.25;
				D.SuspensionMultRc = num12 * 0.8;
				D.SuspensionMultR1 = num13 * 0.5;
				D.SuspensionMultR2 = num14 * 1.0;
				D.SuspensionMultR3 = num15 * 0.5;
				D.SuspensionMultR4 = num16 * 0.8;
				D.SuspensionMultR5 = num17 * 0.25;
				D.SuspensionRumbleMultRa = num18 * 1.0;
				D.SuspensionRumbleMultRb = num18 * 0.0;
				D.SuspensionRumbleMultRc = num18 * 1.5;
				D.SuspensionRumbleMultR1 = num18 * 0.0;
				D.SuspensionRumbleMultR2 = num18 * 2.0;
				D.SuspensionRumbleMultR3 = num18 * 0.0;
				D.SuspensionRumbleMultR4 = num18 * 1.5;
				D.SuspensionRumbleMultR5 = num18 * 0.0;
			}
			D.EngineLoad = data.NewData.CarSettings_CurrentDisplayedRPMPercent * 0.5;
			D.EngineLoad += data.NewData.SpeedKmh * data.NewData.SpeedKmh * 0.0003;
			D.EngineLoad += data.NewData.SpeedKmh * 0.02;
			if (Math.Abs(D.SuspensionAccAll) > 0.5)
				D.EngineLoad += 200.0 * Math.Sin(data.NewData.OrientationPitch * 0.0174533);
			D.EngineLoad -= D.EngineLoad * (1.0 - D.MixPower) * 0.5;
			D.EngineLoad *= data.NewData.Throttle * 0.01 * 0.01;
			if (D.IdleSampleCount < 20 && FrameCountTicks % 2500000L <= 150000L)
			{
				double num19 = Math.Abs(data.OldData.Rpms - data.NewData.Rpms) * D.FPS;
				if (data.NewData.Rpms > data.NewData.MaxRpm * 0.1 && data.NewData.Rpms <= D.IdleRPM + 20.0 && num19 < 40.0)
				{
					D.IdleRPM = (D.IdleRPM + data.NewData.Rpms) * 0.5;
					++D.IdleSampleCount;
					double num20 = D.IdleRPM * 0.008333333;
					D.FrequencyMultiplier = num20 >= 5.0 ? (num20 >= 10.0 ? (num20 <= 20.0 ? (num20 <= 40.0 ? 1.0 : 0.25) : 0.5) : 2.0) : 4.0;
				}
			}
			if (FrameCountTicks % 5000000L <= 150000L)
			{
				SetRPMIntervals();
				SetRPMMix();
			}
			D.FreqHarmonic = data.NewData.Rpms * 0.008333333;
			D.FreqOctave = D.FreqHarmonic * (1.0 + D.IntervalOctave * 0.08333333);
			D.FreqLFEAdaptive = D.FreqHarmonic * D.FrequencyMultiplier;
			D.FreqIntervalA1 = D.FreqHarmonic * (1.0 + D.IntervalA * 0.08333333);
			D.FreqIntervalA2 = D.FreqHarmonic * 0.5 * (1.0 + D.IntervalA * 0.08333333);
			D.FreqPeakA1 = D.FreqHarmonic * (1.0 + D.IntervalPeakA * 0.08333333);
			D.FreqPeakB1 = D.FreqHarmonic * (1.0 + D.IntervalPeakB * 0.08333333);
			D.FreqPeakA2 = D.FreqHarmonic * 0.5 * (1.0 + D.IntervalPeakA * 0.08333333);
			D.FreqPeakB2 = D.FreqHarmonic * 0.5 * (1.0 + D.IntervalPeakB * 0.08333333);
			double num21 = 1.0;
			double num22 = 1.0;
			if (D.Gear > 0)
			{
				num21 -= D.AccSurge2S.Clamp(0.0, 15.0) * 0.01;
				if (D.Accelerator < 20.0 && D.AccSurgeAvg < 0.0)
					num22 += Math.Max(0.0, Math.Max(0.0, D.RPMPercent - D.IdlePercent * (1.0 + D.Gear * 0.2))
											* (0.2 + 0.6 * D.MixDisplacement) - D.Accelerator * 0.05 * (0.2 + 0.6 * D.MixDisplacement));
			}
			D.Gain1H = D.FreqHarmonic >= 25.0
					? (D.FreqHarmonic >= 40.0
						? (D.FreqHarmonic >= 65.0
							? (D.FreqHarmonic >= 95.0
								? (D.FreqHarmonic >= 125.0
									? 75.0 - (D.FreqHarmonic - 125.0)
									: 95.0 - (D.FreqHarmonic - 95.0) * 0.667)
								: 65.0 + (D.FreqHarmonic - 65.0) * 1.0)
							: 52.5 + (D.FreqHarmonic - 40.0) * 0.5)
						: 40.0 + (D.FreqHarmonic - 25.0) * 0.834)
					: 30.0 + (D.FreqHarmonic - 15.0) * 1.0;
			D.Gain1H = Math.Max(D.Gain1H, 0.0) * num21 * num22 * (0.8 + 0.2 * D.MixPower + 0.2 * D.MixCylinder);
			D.Gain1H = Math.Floor(D.Gain1H.Clamp(0.0, sbyte.MaxValue));
			D.Gain1H2 = D.FreqHarmonic >= 25.0 ? (D.FreqHarmonic >= 40.0
												 ? (D.FreqHarmonic >= 65.0
													 ? (D.FreqHarmonic >= 95.0
														 ? (D.FreqHarmonic >= 125.0
															 ? 75.0 - (D.FreqHarmonic - 125.0)
															 : 95.0 - (D.FreqHarmonic - 95.0) * 0.667)
														 : 65.0 + (D.FreqHarmonic - 65.0) * 1.0)
													 : 52.5 + (D.FreqHarmonic - 40.0) * 0.5)
												 : 40.0 + (D.FreqHarmonic - 25.0) * 0.834)
											 : 30.0 + (D.FreqHarmonic - 15.0) * 1.0;
			D.Gain1H2 = Math.Max(D.Gain1H2, 0.0) * num21 * num22 * (0.8 + 0.1 * D.MixDisplacement + 0.3 * D.MixCylinder);
			D.Gain1H2 = Math.Floor(D.Gain1H2.Clamp(0.0, sbyte.MaxValue));
			D.Gain2H = D.FreqHarmonic >= 25.0 ? (D.FreqHarmonic >= 40.0
												 ? (D.FreqHarmonic >= 65.0
													 ? (D.FreqHarmonic >= 95.0
														 ? (D.FreqHarmonic >= 125.0
															 ? 75.0 - (D.FreqHarmonic - 125.0)
															 : 95.0 - (D.FreqHarmonic - 95.0) * 0.667)
														 : 65.0 + (D.FreqHarmonic - 65.0) * 1.0)
													 : 52.5 + (D.FreqHarmonic - 40.0) * 0.5)
												 : 40.0 + (D.FreqHarmonic - 25.0) * 0.834)
											 : 30.0 + (D.FreqHarmonic - 15.0) * 1.0;
			D.Gain2H = Math.Max(D.Gain2H, 0.0) * num21 * num22 * (0.8 + 0.3 * D.MixPower + 0.1 * D.MixCylinder);
			D.Gain2H = Math.Floor(D.Gain2H.Clamp(0.0, sbyte.MaxValue));
			D.Gain4H = D.FreqHarmonic >= 25.0 ? (D.FreqHarmonic >= 40.0
												 ? (D.FreqHarmonic >= 65.0
													 ? (D.FreqHarmonic >= 95.0
														 ? (D.FreqHarmonic >= 125.0
															 ? 75.0 - (D.FreqHarmonic - 125.0)
															 : 95.0 - (D.FreqHarmonic - 95.0) * 0.667)
														 : 66.0 + (D.FreqHarmonic - 65.0) * 1.0)
													 : 52.5 + (D.FreqHarmonic - 40.0) * 0.5)
												 : 40.0 + (D.FreqHarmonic - 25.0) * 0.834)
											 : 30.0 + (D.FreqHarmonic - 15.0) * 1.0;
			D.Gain4H = Math.Max(D.Gain4H, 0.0) * num21 * num22 * (0.8 + 0.2 * D.MixPower + 0.2 * D.MixDisplacement);
			D.Gain4H = Math.Floor(D.Gain4H.Clamp(0.0, sbyte.MaxValue));
			D.GainOctave = D.FreqOctave >= 55.0 ? (D.FreqOctave >= 80.0 ? 75.0 - (D.FreqOctave - 80.0) * 0.75 : 30.0 + (D.FreqOctave - 55.0) * 1.8) : (D.FreqOctave - 30.0) * 1.2;
			D.GainOctave = Math.Max(D.GainOctave, 0.0) * num21 * (0.3 * D.MixPower + 0.3 * D.MixCylinder + 0.6 * D.EngineLoad);
			D.GainOctave = Math.Floor(D.GainOctave.Clamp(0.0, sbyte.MaxValue));
			D.GainIntervalA1 = D.FreqIntervalA1 >= 70.0 ? (D.FreqIntervalA1 >= 85.0 ? 75.0 - (D.FreqIntervalA1 - 85.0) * 0.85 : 45.0 + (D.FreqIntervalA1 - 70.0) * 2.0) : (D.FreqIntervalA1 - 40.0) * 1.5;
			D.GainIntervalA1 = Math.Max(D.GainIntervalA1, 0.0) * num21 * (0.2 * D.MixPower + 1.0 * D.EngineLoad);
			D.GainIntervalA1 = Math.Floor(D.GainIntervalA1.Clamp(0.0, sbyte.MaxValue));
			D.GainIntervalA2 = D.FreqIntervalA2 >= 70.0 ? (D.FreqIntervalA2 >= 85.0 ? 75.0 - (D.FreqIntervalA2 - 85.0) * 0.85 : 45.0 + (D.FreqIntervalA2 - 70.0) * 2.0) : (D.FreqIntervalA2 - 40.0) * 1.5;
			D.GainIntervalA2 = Math.Max(D.GainIntervalA2, 0.0) * num21 * (0.1 * D.MixPower + 0.3 * D.MixCylinder + 0.8 * D.EngineLoad);
			D.GainIntervalA2 = Math.Floor(D.GainIntervalA2.Clamp(0.0, sbyte.MaxValue));
			D.PeakA1Start = D.RedlinePercent * (0.96 + D.GearInterval * D.Gear * 0.04);
			D.PeakB1Start = D.RedlinePercent * (0.92 + D.GearInterval * D.Gear * 0.04);
			D.PeakA2Start = D.RedlinePercent * (0.9 + D.MixPower * D.GearInterval * D.Gear * 0.06);
			D.PeakB2Start = D.RedlinePercent * (0.98 - D.MixTorque * 0.08);
			D.PeakA1Modifier = ((D.RPMPercent - D.PeakA1Start) / (D.RedlinePercent - D.PeakA1Start + (1.0 - D.RedlinePercent) * (0.75 + D.MixCylinder * 0.75))).Clamp(0.0, 1.0);
			D.PeakB1Modifier = ((D.RPMPercent - D.PeakB1Start) / (D.RedlinePercent - D.PeakB1Start + (1.0 - D.RedlinePercent) * (0.0 + D.MixCylinder))).Clamp(0.0, 1.0);
			D.PeakA2Modifier = ((D.RPMPercent - D.PeakA2Start) / (D.RedlinePercent - D.PeakA2Start)).Clamp(0.0, 1.0);
			D.PeakB2Modifier = ((D.RPMPercent - D.PeakB2Start) / (D.RedlinePercent - D.PeakB2Start + (1.0 - D.RedlinePercent) * (1.0 - D.MixDisplacement))).Clamp(0.0, 1.0);
			D.GainPeakA1 = D.FreqPeakA1 >= 55.0 ? (D.FreqPeakA1 >= 75.0 ? (D.FreqPeakA1 >= 105.0 ? 90.0 - (D.FreqPeakA1 - 105.0) * 0.75 : 60.0 + (D.FreqPeakA1 - 75.0) * 1.0) : 30.0 + (D.FreqPeakA1 - 55.0) * 1.5) : (D.FreqPeakA1 - 45.0) * 3.0;
			D.GainPeakA1 = Math.Max(D.GainPeakA1, 0.0) * (0.9 + 0.1 * D.MixPower + 0.1 * D.MixCylinder + 0.1 * D.MixTorque);
			D.GainPeakA1Front = Math.Floor((D.PeakA1Modifier * D.GainPeakA1 * (0.9 + 0.3 * D.MixFront)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakA1Rear = Math.Floor((D.PeakA1Modifier * D.GainPeakA1 * (0.9 + 0.3 * D.MixRear)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakA1 = Math.Floor((D.PeakA1Modifier * D.GainPeakA1 * (0.9 + 0.3 * D.MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakB1 = D.FreqPeakB1 >= 55.0 ? (D.FreqPeakB1 >= 75.0 ? (D.FreqPeakB1 >= 105.0 ? 90.0 - (D.FreqPeakB1 - 105.0) * 0.75 : 60.0 + (D.FreqPeakB1 - 75.0) * 1.0) : 30.0 + (D.FreqPeakB1 - 55.0) * 1.5) : (D.FreqPeakB1 - 45.0) * 3.0;
			D.GainPeakB1 = Math.Max(D.GainPeakB1, 0.0) * (0.9 + 0.1 * D.MixPower + 0.1 * D.MixCylinder + 0.1 * D.MixTorque);
			D.GainPeakB1Front = Math.Floor((D.PeakB1Modifier * D.GainPeakB1 * (0.9 + 0.3 * D.MixFront)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakB1Rear = Math.Floor((D.PeakB1Modifier * D.GainPeakB1 * (0.9 + 0.3 * D.MixRear)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakB1 = Math.Floor((D.PeakB1Modifier * D.GainPeakB1 * (0.9 + 0.3 * D.MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakA2 = D.FreqPeakA2 >= 55.0 ? (D.FreqPeakA2 >= 75.0 ? (D.FreqPeakA2 >= 105.0 ? 90.0 - (D.FreqPeakA2 - 105.0) * 0.75 : 60.0 + (D.FreqPeakA2 - 75.0) * 1.0) : 30.0 + (D.FreqPeakA2 - 55.0) * 1.5) : (D.FreqPeakA2 - 45.0) * 3.0;
			D.GainPeakA2 = Math.Max(D.GainPeakA2, 0.0) * (0.9 + 0.1 * D.MixPower + 0.1 * D.MixCylinder + 0.1 * D.MixTorque);
			D.GainPeakA2Front = Math.Floor((D.PeakA2Modifier * D.GainPeakA2 * (0.9 + 0.3 * D.MixFront)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakA2Rear = Math.Floor((D.PeakA2Modifier * D.GainPeakA2 * (0.9 + 0.3 * D.MixRear)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakA2 = Math.Floor((D.PeakA2Modifier * D.GainPeakA2 * (0.9 + 0.3 * D.MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakB2 = D.FreqPeakB2 >= 60.0 ? (D.FreqPeakB2 >= 100.0 ? 100.0 - (D.FreqPeakB2 - 100.0) * 0.85 : 30.0 + (D.FreqPeakB2 - 60.0) * 1.75) : (D.FreqPeakB2 - 30.0) * 1.0;
			D.GainPeakB2 = Math.Max(D.GainPeakB2, 0.0) * (0.9 + 0.1 * D.MixPower + 0.1 * D.MixCylinder + 0.1 * D.MixTorque);
			D.GainPeakB2Front = Math.Floor((D.PeakB2Modifier * D.GainPeakB2 * (0.9 + 0.3 * D.MixFront)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakB2Rear = Math.Floor((D.PeakB2Modifier * D.GainPeakB2 * (0.9 + 0.3 * D.MixRear)).Clamp(0.0, sbyte.MaxValue));
			D.GainPeakB2 = Math.Floor((D.PeakB2Modifier * D.GainPeakB2 * (0.9 + 0.3 * D.MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			if (S.EngineCylinders < 1.0)
			{
				D.GainLFEAdaptive = 0.0;
				D.Gain1H = Math.Floor(D.Gain1H * 0.7);
				D.Gain1H2 = 0.0;
				D.Gain2H = 0.0;
				D.Gain4H = 0.0;
				D.GainOctave = 0.0;
				D.GainIntervalA1 = 0.0;
				D.GainIntervalA2 = 0.0;
				D.GainPeakA1Front = 0.0;
				D.GainPeakA1Rear = 0.0;
				D.GainPeakA1 = 0.0;
				D.GainPeakA2Front = 0.0;
				D.GainPeakA2Rear = 0.0;
				D.GainPeakA2 = 0.0;
				D.GainPeakB1Front = 0.0;
				D.GainPeakB1Rear = 0.0;
				D.GainPeakB1 = 0.0;
				D.GainPeakB2Front = 0.0;
				D.GainPeakB2Rear = 0.0;
				D.GainPeakB2 = 0.0;
			}
			else if (S.EngineCylinders < 2.0)
				D.Gain4H = 0.0;
			if (D.EngineMult == 1.0)
				return;
			D.GainLFEAdaptive *= D.EngineMult * D.EngineMultAll;
			D.Gain1H *= D.EngineMult * D.EngineMultAll;
			D.Gain1H2 *= D.EngineMult * D.EngineMultAll;
			D.Gain2H *= D.EngineMult * D.EngineMultAll;
			D.Gain4H *= D.EngineMult * D.EngineMultAll;
			D.GainOctave *= D.EngineMult * D.EngineMultAll;
			D.GainIntervalA1 *= D.EngineMult * D.EngineMultAll;
			D.GainIntervalA2 *= D.EngineMult * D.EngineMultAll;
			D.GainPeakA1Front *= D.EngineMult * D.EngineMultAll;
			D.GainPeakA1Rear *= D.EngineMult * D.EngineMultAll;
			D.GainPeakA1 *= D.EngineMult * D.EngineMultAll;
			D.GainPeakA2Front *= D.EngineMult * D.EngineMultAll;
			D.GainPeakA2Rear *= D.EngineMult * D.EngineMultAll;
			D.GainPeakA2 *= D.EngineMult * D.EngineMultAll;
			D.GainPeakB1Front *= D.EngineMult * D.EngineMultAll;
			D.GainPeakB1Rear *= D.EngineMult * D.EngineMultAll;
			D.GainPeakB1 *= D.EngineMult * D.EngineMultAll;
			D.GainPeakB2Front *= D.EngineMult * D.EngineMultAll;
			D.GainPeakB2Rear *= D.EngineMult * D.EngineMultAll;
			D.GainPeakB2 *= D.EngineMult * D.EngineMultAll;
		}

		private void UpdateVehiclePerGame(PluginManager pluginManager, ref GameData data)
		{
			D.SuspensionDistFLP = D.SuspensionDistFL;
			D.SuspensionDistFRP = D.SuspensionDistFR;
			D.SuspensionDistRLP = D.SuspensionDistRL;
			D.SuspensionDistRRP = D.SuspensionDistRR;
			D.SuspensionDistFL = 0.0;
			D.SuspensionDistFR = 0.0;
			D.SuspensionDistRL = 0.0;
			D.SuspensionDistRR = 0.0;
			D.SuspensionVelFLP = D.SuspensionVelFL;
			D.SuspensionVelFRP = D.SuspensionVelFR;
			D.SuspensionVelRLP = D.SuspensionVelRL;
			D.SuspensionVelRRP = D.SuspensionVelRR;
			D.SuspensionVelFL = 0.0;
			D.SuspensionVelFR = 0.0;
			D.SuspensionVelRL = 0.0;
			D.SuspensionVelRR = 0.0;
			D.SlipXFL = 0.0;
			D.SlipXFR = 0.0;
			D.SlipXRL = 0.0;
			D.SlipXRR = 0.0;
			D.SlipYFL = 0.0;
			D.SlipYFR = 0.0;
			D.SlipYRL = 0.0;
			D.SlipYRR = 0.0;
			D.ABSActive = data.NewData.ABSActive == 1;
			bool flag = true;
			switch (CurrentGame)
			{
				case GameId.AC:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel02");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel03");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel04");
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed01"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed02"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed03"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed04"));
					SlipFromRPS();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip01") - Math.Abs(D.SlipYFL) * 1.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip02") - Math.Abs(D.SlipYFR) * 1.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip03") - Math.Abs(D.SlipYRL) * 1.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip04") - Math.Abs(D.SlipYRR) * 1.0, 0.0);
					if (D.TireDiameterFL == 0.0)
					{
						D.SlipXFL *= 0.5;
						D.SlipXFR *= 0.5;
						D.SlipXRL *= 0.5;
						D.SlipXRR *= 0.5;
					}
					D.TiresLeft = 1.0 + (double) Math.Max((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading01.Y"), (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading03.Y"));
					D.TiresRight = 1.0 + (double) Math.Max((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading02.Y"), (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.TyreContactHeading04.Y"));
					if (D.RumbleLeftAvg == 0.0)
						D.RumbleLeftAvg = D.TiresLeft;
					if (D.RumbleRightAvg == 0.0)
						D.RumbleRightAvg = D.TiresRight;
					D.RumbleLeftAvg = (D.RumbleLeftAvg + D.TiresLeft) * 0.5;
					D.RumbleRightAvg = (D.RumbleRightAvg + D.TiresRight) * 0.5;
					D.RumbleLeft = Math.Abs(D.TiresLeft / D.RumbleLeftAvg - 1.0) * 2000.0;
					D.RumbleRight = Math.Abs(D.TiresRight / D.RumbleRightAvg - 1.0) * 2000.0;
					break;
				case GameId.ACC:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel02");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel03");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.SuspensionTravel04");
					D.WiperStatus = (int) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Graphics.WiperLV");
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed01"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed02"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed03"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelAngularSpeed04"));
					SlipFromRPS();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip01") - Math.Abs(D.SlipYFL) * 2.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip02") - Math.Abs(D.SlipYFR) * 2.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip03") - Math.Abs(D.SlipYRL) * 2.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Physics.WheelSlip04") - Math.Abs(D.SlipYRR) * 2.0, 0.0);
					if (D.TireDiameterFL == 0.0)
					{
						D.SlipXFL *= 0.5;
						D.SlipXFR *= 0.5;
						D.SlipXRL *= 0.5;
						D.SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.AMS1:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.suspensionDeflection");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.suspensionDeflection");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.suspensionDeflection");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.suspensionDeflection");
					D.SpeedMs = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayer.speed");
					D.InvSpeedMs = D.SpeedMs != 0.0 ? 1.0 / D.SpeedMs : 0.0;
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.rotation"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.rotation"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.rotation"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.rotation"));
					SlipFromRPS();
					D.SlipXFL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.gripFract") - Math.Abs(D.SlipYFL) * 1.0, 0.0);
					D.SlipXFR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.gripFract") - Math.Abs(D.SlipYFR) * 1.0, 0.0);
					D.SlipXRL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.gripFract") - Math.Abs(D.SlipYRL) * 1.0, 0.0);
					D.SlipXRR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.gripFract") - Math.Abs(D.SlipYRR) * 1.0, 0.0);
					if (D.TireDiameterFL == 0.0)
					{
						D.SlipXFL *= 0.5;
						D.SlipXFR *= 0.5;
						D.SlipXRL *= 0.5;
						D.SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.AMS2:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel02");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel03");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel04");
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS01"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS02"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS03"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS04"));
					SlipFromRPS();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed01") * 0.1 - Math.Abs(D.SlipYFL) * 1.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed02") * 0.1 - Math.Abs(D.SlipYFR) * 1.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed03") * 0.1 - Math.Abs(D.SlipYRL) * 1.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed04") * 0.1 - Math.Abs(D.SlipYRR) * 1.0, 0.0);
					if (D.TireDiameterFL == 0.0)
					{
						D.SlipXFL *= 0.5;
						D.SlipXFR *= 0.5;
						D.SlipXRL *= 0.5;
						D.SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.D4:
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight") * 0.001;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft") * 0.001;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight") * 0.001;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.DR2:
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight") * 0.001;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft") * 0.001;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight") * 0.001;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					D.VelocityX = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WorldSpeedX") * Math.Sin((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.XR"));
					D.YawRate = data.NewData.OrientationYawAcceleration;
					if (D.VelocityX < 0.0)
					{
						if (D.YawRate < 0.0)
						{
							D.SlipXFL = -D.VelocityX * D.WheelLoadFL * D.YawRate * 0.5;
							D.SlipXFR = -D.VelocityX * D.WheelLoadFR * D.YawRate * 0.5;
							D.SlipXRL = -D.VelocityX * D.WheelLoadRL * D.YawRate * 1.0;
							D.SlipXRR = -D.VelocityX * D.WheelLoadRR * D.YawRate * 1.0;
						}
						else
						{
							D.SlipXFL = -D.VelocityX * D.WheelLoadFL * D.YawRate * 1.0;
							D.SlipXFR = -D.VelocityX * D.WheelLoadFR * D.YawRate * 1.0;
							D.SlipXRL = -D.VelocityX * D.WheelLoadRL * D.YawRate * 0.5;
							D.SlipXRR = -D.VelocityX * D.WheelLoadRR * D.YawRate * 0.5;
						}
					}
					else if (D.YawRate < 0.0)
					{
						D.SlipXFL = D.VelocityX * D.WheelLoadFL * -D.YawRate * 1.0;
						D.SlipXFR = D.VelocityX * D.WheelLoadFR * -D.YawRate * 1.0;
						D.SlipXRL = D.VelocityX * D.WheelLoadRL * -D.YawRate * 0.5;
						D.SlipXRR = D.VelocityX * D.WheelLoadRR * -D.YawRate * 0.5;
					}
					else
					{
						D.SlipXFL = D.VelocityX * D.WheelLoadFL * -D.YawRate * 0.5;
						D.SlipXFR = D.VelocityX * D.WheelLoadFR * -D.YawRate * 0.5;
						D.SlipXRL = D.VelocityX * D.WheelLoadRL * -D.YawRate * 1.0;
						D.SlipXRR = D.VelocityX * D.WheelLoadRR * -D.YawRate * 1.0;
					}
					D.SlipXFL = Math.Max(D.SlipXFL, 0.0);
					D.SlipXFR = Math.Max(D.SlipXFL, 0.0);
					D.SlipXRL = Math.Max(D.SlipXFL, 0.0);
					D.SlipXRR = Math.Max(D.SlipXFL, 0.0);
					break;
				case GameId.WRC23:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_fl");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_fr");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_bl");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_hub_position_br");
					D.SpeedMs = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_speed");
					D.InvSpeedMs = D.SpeedMs != 0.0 ? 1.0 / D.SpeedMs : 0.0;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_fl"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_fr"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_bl"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_cp_forward_speed_br"));
					SlipFromWheelSpeed();
					D.VelocityX = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdateLocalVelocity.X");
					D.YawRate = data.NewData.OrientationYawAcceleration;
					if (D.VelocityX < 0.0)
					{
						if (D.YawRate < 0.0)
						{
							D.SlipXFL = -D.VelocityX * D.WheelLoadFL * D.YawRate * 0.5;
							D.SlipXFR = -D.VelocityX * D.WheelLoadFR * D.YawRate * 0.5;
							D.SlipXRL = -D.VelocityX * D.WheelLoadRL * D.YawRate * 1.0;
							D.SlipXRR = -D.VelocityX * D.WheelLoadRR * D.YawRate * 1.0;
						}
						else
						{
							D.SlipXFL = -D.VelocityX * D.WheelLoadFL * D.YawRate * 1.0;
							D.SlipXFR = -D.VelocityX * D.WheelLoadFR * D.YawRate * 1.0;
							D.SlipXRL = -D.VelocityX * D.WheelLoadRL * D.YawRate * 0.5;
							D.SlipXRR = -D.VelocityX * D.WheelLoadRR * D.YawRate * 0.5;
						}
					}
					else if (D.YawRate < 0.0)
					{
						D.SlipXFL = D.VelocityX * D.WheelLoadFL * -D.YawRate * 1.0;
						D.SlipXFR = D.VelocityX * D.WheelLoadFR * -D.YawRate * 1.0;
						D.SlipXRL = D.VelocityX * D.WheelLoadRL * -D.YawRate * 0.5;
						D.SlipXRR = D.VelocityX * D.WheelLoadRR * -D.YawRate * 0.5;
					}
					else
					{
						D.SlipXFL = D.VelocityX * D.WheelLoadFL * -D.YawRate * 0.5;
						D.SlipXFR = D.VelocityX * D.WheelLoadFR * -D.YawRate * 0.5;
						D.SlipXRL = D.VelocityX * D.WheelLoadRL * -D.YawRate * 1.0;
						D.SlipXRR = D.VelocityX * D.WheelLoadRR * -D.YawRate * 1.0;
					}
					D.SlipXFL = Math.Max(D.SlipXFL, 0.0);
					D.SlipXFR = Math.Max(D.SlipXFL, 0.0);
					D.SlipXRL = Math.Max(D.SlipXFL, 0.0);
					D.SlipXRR = Math.Max(D.SlipXFL, 0.0);
					break;
				case GameId.F12022:
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition01") * 0.001;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition02") * 0.001;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition03") * 0.001;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_suspensionPosition04") * 0.001;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed03"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed04"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed01"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSpeed02"));
					SlipFromWheelSpeed();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip03") - Math.Abs(D.SlipYFL) * 2.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip04") - Math.Abs(D.SlipYFR) * 2.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip01") - Math.Abs(D.SlipYRL) * 2.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerMotionData.m_wheelSlip02") - Math.Abs(D.SlipYRR) * 2.0, 0.0);
					break;
				case GameId.F12023:
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition01") * 0.001;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition02") * 0.001;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition03") * 0.001;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_suspensionPosition04") * 0.001;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed03"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed04"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed01"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSpeed02"));
					SlipFromWheelSpeed();
					D.SlipXFL = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio01")) * 5.0 - Math.Abs(D.SlipYFL) * 1.0, 0.0);
					D.SlipXFR = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio02")) * 5.0 - Math.Abs(D.SlipYFR) * 1.0, 0.0);
					D.SlipXRL = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio03")) * 5.0 - Math.Abs(D.SlipYRL) * 1.0, 0.0);
					D.SlipXRR = Math.Max((double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PacketMotionExData.m_wheelSlipRatio04")) * 5.0 - Math.Abs(D.SlipYRR) * 1.0, 0.0);
					break;
				case GameId.Forza:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersFrontLeft");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersFrontRight");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersRearLeft");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionTravelMetersRearRight");
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedFrontLeft"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedFrontRight"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedRearLeft"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelRotationSpeedRearRight"));
					SlipFromRPS();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipFrontLeft") - Math.Abs(D.SlipYFL) * 2.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipFrontRight") - Math.Abs(D.SlipYFR) * 2.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipRearLeft") - Math.Abs(D.SlipYRL) * 2.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireCombinedSlipRearRight") - Math.Abs(D.SlipYRR) * 2.0, 0.0);
					if (D.TireDiameterFL == 0.0)
					{
						D.SlipXFL *= 0.5;
						D.SlipXFR *= 0.5;
						D.SlipXRL *= 0.5;
						D.SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.GTR2:
				case GameId.GSCE:
				case GameId.RF1:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.suspensionDeflection");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.suspensionDeflection");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.suspensionDeflection");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.suspensionDeflection");
					D.SpeedMs = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayer.speed");
					D.InvSpeedMs = D.SpeedMs != 0.0 ? 1.0 / D.SpeedMs : 0.0;
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.rotation"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.rotation"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.rotation"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.rotation"));
					SlipFromRPS();
					D.SlipXFL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel01.gripFract") - Math.Abs(D.SlipYFL) * 1.0, 0.0);
					D.SlipXFR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel02.gripFract") - Math.Abs(D.SlipYFR) * 1.0, 0.0);
					D.SlipXRL = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel03.gripFract") - Math.Abs(D.SlipYRL) * 1.0, 0.0);
					D.SlipXRR = Math.Max(1.0 - (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Data.wheel04.gripFract") - Math.Abs(D.SlipYRR) * 1.0, 0.0);
					if (D.TireDiameterFL == 0.0)
					{
						D.SlipXFL *= 0.5;
						D.SlipXFR *= 0.5;
						D.SlipXRL *= 0.5;
						D.SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.IRacing:
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFshockDefl") != null)
					{
						D.SuspensionDistFL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFshockDefl"));
						D.SuspensionDistFR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RFshockDefl"));
					}
					else if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFSHshockDefl") != null)
					{
						D.SuspensionDistFL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFSHshockDefl"));
						D.SuspensionDistFR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RFSHshockDefl"));
					}
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRshockDefl") != null)
					{
						D.SuspensionDistRL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRshockDefl"));
						D.SuspensionDistRR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RRshockDefl"));
					}
					else if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRSHshockDefl") != null)
					{
						D.SuspensionDistRL = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRSHshockDefl"));
						D.SuspensionDistRR = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.RRSHshockDefl"));
					}
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CFshockDefl") != null)
					{
						D.SuspensionDistFL = 0.5 * D.SuspensionDistFL + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CFshockDefl"));
						D.SuspensionDistFR = 0.5 * D.SuspensionDistFR + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CFshockDefl"));
					}
					else if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.HFshockDefl") != null)
					{
						D.SuspensionDistFL = 0.5 * D.SuspensionDistFL + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.HFshockDefl"));
						D.SuspensionDistFR = 0.5 * D.SuspensionDistFR + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.HFshockDefl"));
					}
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CRshockDefl") != null)
					{
						D.SuspensionDistRL = 0.5 * D.SuspensionDistRL + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CRshockDefl"));
						D.SuspensionDistRR = 0.5 * D.SuspensionDistRR + Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CRshockDefl"));
					}
					D.VelocityX = Convert.ToDouble(pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.VelocityY")) * 10.0;
					if (D.VelocityX < 0.0)
					{
						if (D.YawRate < 0.0)
						{
							D.SlipXFL = -D.VelocityX * D.WheelLoadFL * D.YawRate * 0.1;
							D.SlipXFR = -D.VelocityX * D.WheelLoadFR * D.YawRate * 0.1;
							D.SlipXRL = -D.VelocityX * D.WheelLoadRL * D.YawRate * 0.2;
							D.SlipXRR = -D.VelocityX * D.WheelLoadRR * D.YawRate * 0.2;
						}
						else
						{
							D.SlipXFL = -D.VelocityX * D.WheelLoadFL * D.YawRate * 0.2;
							D.SlipXFR = -D.VelocityX * D.WheelLoadFR * D.YawRate * 0.2;
							D.SlipXRL = -D.VelocityX * D.WheelLoadRL * D.YawRate * 0.1;
							D.SlipXRR = -D.VelocityX * D.WheelLoadRR * D.YawRate * 0.1;
						}
					}
					else if (D.YawRate < 0.0)
					{
						D.SlipXFL = D.VelocityX * D.WheelLoadFL * -D.YawRate * 0.2;
						D.SlipXFR = D.VelocityX * D.WheelLoadFR * -D.YawRate * 0.2;
						D.SlipXRL = D.VelocityX * D.WheelLoadRL * -D.YawRate * 0.1;
						D.SlipXRR = D.VelocityX * D.WheelLoadRR * -D.YawRate * 0.1;
					}
					else
					{
						D.SlipXFL = D.VelocityX * D.WheelLoadFL * -D.YawRate * 0.1;
						D.SlipXFR = D.VelocityX * D.WheelLoadFR * -D.YawRate * 0.1;
						D.SlipXRL = D.VelocityX * D.WheelLoadRL * -D.YawRate * 0.2;
						D.SlipXRR = D.VelocityX * D.WheelLoadRR * -D.YawRate * 0.2;
					}
					if (D.Brake > 0.0)
					{
						D.SlipYFL = D.BrakeF * D.SpeedMs * D.WheelLoadFL * D.InvAccSurgeAvg * 0.04;
						D.SlipYFR = D.BrakeF * D.SpeedMs * D.WheelLoadFR * D.InvAccSurgeAvg * 0.04;
						D.SlipYRL = (D.BrakeR + D.Handbrake) * D.SpeedMs * D.WheelLoadRL * D.InvAccSurgeAvg * 0.04;
						D.SlipYRR = (D.BrakeR + D.Handbrake) * D.SpeedMs * D.WheelLoadRR * D.InvAccSurgeAvg * 0.04;
					}
					else if (D.Accelerator > 10.0 && D.SpeedMs > 0.0 && D.AccSurgeAvg < 0.0)
					{
						if (S.PoweredWheels == "F")
						{
							D.SlipYFL = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.2;
							D.SlipYFR = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.2;
							D.SlipYRL = 0.0;
							D.SlipYRR = 0.0;
						}
						else if (S.PoweredWheels == "R")
						{
							D.SlipYFL = 0.0;
							D.SlipYFR = 0.0;
							D.SlipYRL = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.2;
							D.SlipYRR = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.2;
						}
						else
						{
							D.SlipYFL = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.15;
							D.SlipYFR = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.15;
							D.SlipYRL = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.15;
							D.SlipYRR = D.Accelerator * -D.InvAccSurgeAvg * D.InvSpeedMs * 0.15;
						}
					}
					D.SlipXFL = 0.0;
					D.SlipXFR = 0.0;
					D.SlipXRL = 0.0;
					D.SlipXRR = 0.0;
					D.SlipYFL = 0.0;
					D.SlipYFR = 0.0;
					D.SlipYRL = 0.0;
					D.SlipYRR = 0.0;
					break;
				case GameId.PC2:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel02");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel03");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mSuspensionTravel04");
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS01"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS02"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS03"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreRPS04"));
					SlipFromRPS();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed01") * 0.1 - Math.Abs(D.SlipYFL) * 1.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed02") * 0.1 - Math.Abs(D.SlipYFR) * 1.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed03") * 0.1 - Math.Abs(D.SlipYRL) * 1.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.mTyreSlipSpeed04") * 0.1 - Math.Abs(D.SlipYRR) * 1.0, 0.0);
					if (D.TireDiameterFL == 0.0)
					{
						D.SlipXFL *= 0.5;
						D.SlipXFR *= 0.5;
						D.SlipXRL *= 0.5;
						D.SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.RBR:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionLF.springDeflection");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionRF.springDeflection");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionLB.springDeflection");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.NGPTelemetry.car.suspensionRB.springDeflection");
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFL"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFR"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRL"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRR"));
					SlipFromWheelSpeed();
					break;
				case GameId.RF2:
					D.SuspensionDistFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					D.SuspensionDistFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					D.SuspensionDistRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					D.SuspensionDistRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
					D.WheelRotationFL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mRotation"));
					D.WheelRotationFR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mRotation"));
					D.WheelRotationRL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mRotation"));
					D.WheelRotationRR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mRotation"));
					SlipFromRPS();
					D.SlipXFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralGroundVel");
					D.SlipXFL = D.SlipXFL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralPatchVel") / D.SlipXFL;
					D.SlipXFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralGroundVel");
					D.SlipXFR = D.SlipXFR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralPatchVel") / D.SlipXFR;
					D.SlipXRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralGroundVel");
					D.SlipXRL = D.SlipXRL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralPatchVel") / D.SlipXRL;
					D.SlipXRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralGroundVel");
					D.SlipXRR = D.SlipXRR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralPatchVel") / D.SlipXRR;
					D.SlipXFL *= 0.5;
					D.SlipXFR *= 0.5;
					D.SlipXRL *= 0.5;
					D.SlipXRR *= 0.5;
					if (data.NewData.Brake > 90.0)
					{
						D.ABSActive = ((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mBrakePressure")
									 + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < data.NewData.Brake - 1.0;
						break;
					}
					break;
				case GameId.RRRE:
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.FrontLeft") != null)
					{
						D.SuspensionDistFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.FrontLeft");
						D.SuspensionDistFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.FrontRight");
					}
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.RearLeft") != null)
					{
						D.SuspensionDistRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.RearLeft");
						D.SuspensionDistRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.RearRight");
					}
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionFront") != null)
					{
						D.SuspensionDistFL = 0.5 * D.SuspensionDistFL + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionFront");
						D.SuspensionDistFR = 0.5 * D.SuspensionDistFR + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionFront");
					}
					if (pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionRear") != null)
					{
						D.SuspensionDistRL = 0.5 * D.SuspensionDistRL + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionRear");
						D.SuspensionDistRR = 0.5 * D.SuspensionDistRR + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionRear");
					}
					D.WheelRotationFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.FrontLeft"));
					D.WheelRotationFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.FrontRight"));
					D.WheelRotationRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.RearLeft"));
					D.WheelRotationRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TireRps.RearRight"));
					SlipFromRPS();
					break;
				case GameId.GTL:
				case GameId.RACE07:
					double num1 = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.carCGLocY") * 0.2;
					D.SuspensionDistFL = num1 * D.WheelLoadFL;
					D.SuspensionDistFR = num1 * D.WheelLoadFR;
					D.SuspensionDistRL = num1 * D.WheelLoadRL;
					D.SuspensionDistRR = num1 * D.WheelLoadRR;
					break;
				case GameId.LFS:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels01.SuspDeflect");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels02.SuspDeflect");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels03.SuspDeflect");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.OutSim2.OSWheels04.SuspDeflect");
					break;
				case GameId.WRC10:
				case GameId.WRCX:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position02");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position03");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position04");
					break;
				case GameId.WRCGen:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight");
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.F12016:
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.F12017:
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos01") * 0.001;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos02") * 0.001;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos03") * 0.001;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_susp_pos04") * 0.001;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed03"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed04"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed01"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_wheelSpeed02"));
					SlipFromWheelSpeed();
					break;
				case GameId.GLegends:
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontLeft") * 0.001;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionFrontRight") * 0.001;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearLeft") * 0.001;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SuspensionPositionRearRight") * 0.001;
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontLeft"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedFrontRight"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearLeft"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.KK:
					flag = false;
					double propertyValue = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Motion.VelocityZ");
					if (D.VelocityZAvg == 0.0)
						D.VelocityZAvg = propertyValue;
					D.VelocityZAvg = (D.VelocityZAvg + propertyValue) * 0.5;
					double num2 = (propertyValue / D.VelocityZAvg - 1.0) * 0.5;
					D.SuspensionVelFL = num2 * D.WheelLoadFL;
					D.SuspensionVelFR = num2 * D.WheelLoadFR;
					D.SuspensionVelRL = num2 * D.WheelLoadRL;
					D.SuspensionVelRR = num2 * D.WheelLoadRR;
					break;
				case GameId.ATS:
				case GameId.ETS2:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection02");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection03");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.WheelsValues.SuspDeflection04");
					D.WiperStatus = (bool) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.DashboardValues.Wipers") ? 1 : 0;
					break;
				case GameId.BeamNG:
					flag = false;
					D.SuspensionDistFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_fl") * 0.05;
					D.SuspensionDistFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_fr") * 0.05;
					D.SuspensionDistRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_rl") * 0.05;
					D.SuspensionDistRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_position_rr") * 0.05;
					D.SuspensionVelFL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_fl") * 0.05;
					D.SuspensionVelFR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_fr") * 0.05;
					D.SuspensionVelRL = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_rl") * 0.05;
					D.SuspensionVelRR = (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.suspension_velocity_rr") * 0.05;
					D.WheelSpeedFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_fl");
					D.WheelSpeedFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_fr");
					D.WheelSpeedRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_rl");
					D.WheelSpeedRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_speed_rr");
					SlipFromWheelSpeed();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_fl") * 0.1 - Math.Abs(D.SlipYFL) * 2.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_fr") * 0.1 - Math.Abs(D.SlipYFR) * 2.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_rl") * 0.1 - Math.Abs(D.SlipYRL) * 2.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.wheel_slip_rr") * 0.1 - Math.Abs(D.SlipYRR) * 2.0, 0.0);
					break;
				case GameId.GPBikes:
				case GameId.MXBikes:
					flag = false;
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength01");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength02");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspLength02");
					D.SuspensionVelFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity01");
					D.SuspensionVelFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity01");
					D.SuspensionVelRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity02");
					D.SuspensionVelRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afSuspVelocity02");
					D.WheelSpeedFL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed01"));
					D.WheelSpeedFR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed01"));
					D.WheelSpeedRL = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed02"));
					D.WheelSpeedRR = Math.Abs((float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_afWheelSpeed02"));
					SlipFromWheelSpeed();
					if ((int) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_aiWheelMaterial01") == 7
					 || (int) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_aiWheelMaterial02") == 7)
					{
						D.RumbleLeft = 50.0;
						D.RumbleRight = 50.0;
						break;
					}
					D.RumbleLeft = 0.0;
					D.RumbleRight = 0.0;
					break;
				case GameId.LMU:
					D.SuspensionDistFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					D.SuspensionDistFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					D.SuspensionDistRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					D.SuspensionDistRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
					D.WheelRotationFL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mRotation"));
					D.WheelRotationFR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mRotation"));
					D.WheelRotationRL = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mRotation"));
					D.WheelRotationRR = Math.Abs((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mRotation"));
					SlipFromRPS();
					D.SlipXFL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralGroundVel");
					D.SlipXFL = D.SlipXFL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mLateralPatchVel") / D.SlipXFL;
					D.SlipXFR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralGroundVel");
					D.SlipXFR = D.SlipXFR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mLateralPatchVel") / D.SlipXFR;
					D.SlipXRL = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralGroundVel");
					D.SlipXRL = D.SlipXRL == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels03.mLateralPatchVel") / D.SlipXRL;
					D.SlipXRR = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralGroundVel");
					D.SlipXRR = D.SlipXRR == 0.0 ? 0.0 : (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels04.mLateralPatchVel") / D.SlipXRR;
					D.SlipXFL = Math.Abs(D.SlipXFL - 1.0);
					D.SlipXFR = Math.Abs(D.SlipXFR - 1.0);
					D.SlipXRL = Math.Abs(D.SlipXRL - 1.0);
					D.SlipXRR = Math.Abs(D.SlipXRR - 1.0);
					if (data.NewData.Brake > 80.0)
					{
						D.ABSActive = ((double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels01.mBrakePressure")
									 + (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < data.NewData.Brake - 1.0;
						break;
					}
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					D.SuspensionDistFL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight01");
					D.SuspensionDistFR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight02");
					D.SuspensionDistRL = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight03");
					D.SuspensionDistRR = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Tire_SusHeight04");
					D.WheelSpeedFL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed01")) * 0.277778;
					D.WheelSpeedFR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed02")) * 0.277778;
					D.WheelSpeedRL = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed03")) * 0.277778;
					D.WheelSpeedRR = (double) Math.Abs((float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Speed04")) * 0.277778;
					SlipFromWheelSpeed();
					D.SlipXFL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip01") - Math.Abs(D.SlipYFL) * 2.0, 0.0);
					D.SlipXFR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip02") - Math.Abs(D.SlipYFR) * 2.0, 0.0);
					D.SlipXRL = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip03") - Math.Abs(D.SlipYRL) * 2.0, 0.0);
					D.SlipXRR = Math.Max((double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.Wheel_Slip04") - Math.Abs(D.SlipYRR) * 2.0, 0.0);
					break;
			}
			if (!flag)
				return;
			D.SuspensionVelFL = (D.SuspensionDistFL - D.SuspensionDistFLP) * D.FPS;
			D.SuspensionVelFR = (D.SuspensionDistFR - D.SuspensionDistFRP) * D.FPS;
			D.SuspensionVelRL = (D.SuspensionDistRL - D.SuspensionDistRLP) * D.FPS;
			D.SuspensionVelRR = (D.SuspensionDistRR - D.SuspensionDistRRP) * D.FPS;
		}

		private void SlipFromRPS()
		{
			if (D.TireDiameterSampleCount < D.TireDiameterSampleMax
			 && D.Accelerator < 60.0 && D.SpeedMs > 5.0 && Math.Abs(D.AccHeave2S) < 0.1
			 && Math.Abs(D.AccSurge2S) < 0.01 && Math.Abs(D.AccSway2S) < 0.08)
			{
				D.TireDiameterSampleFL = 2.0 * D.SpeedMs / D.WheelRotationFL;
				D.TireDiameterSampleFR = 2.0 * D.SpeedMs / D.WheelRotationFR;
				D.TireDiameterSampleRL = 2.0 * D.SpeedMs / D.WheelRotationRL;
				D.TireDiameterSampleRR = 2.0 * D.SpeedMs / D.WheelRotationRR;
				if (D.TireDiameterSampleFL > 1.0)
				{
					if (D.TireDiameterSampleFL > 3.0)
						D.TireDiameterSampleFL = 0.66;
					else if (D.TireDiameterSampleCount > 0 && Math.Abs(D.TireDiameterSampleFL - D.TireDiameterFL) > 0.2 * D.TireDiameterFL)
						D.TireDiameterSampleFL = D.TireDiameterFL * 0.9 + D.TireDiameterSampleFL * 0.1;
				}
				if (D.TireDiameterSampleFR > 1.0)
				{
					if (D.TireDiameterSampleFR > 3.0)
						D.TireDiameterSampleFR = 0.66;
					else if (D.TireDiameterSampleCount > 0 && Math.Abs(D.TireDiameterSampleFR - D.TireDiameterFR) > 0.2 * D.TireDiameterFR)
						D.TireDiameterSampleFR = D.TireDiameterFR * 0.9 + D.TireDiameterSampleFR * 0.1;
				}
				if (D.TireDiameterSampleRL > 1.0)
				{
					if (D.TireDiameterSampleRL > 3.0)
						D.TireDiameterSampleRL = 0.66;
					else if (D.TireDiameterSampleCount > 0 && Math.Abs(D.TireDiameterSampleRL - D.TireDiameterRL) > 0.2 * D.TireDiameterRL)
						D.TireDiameterSampleRL = D.TireDiameterRL * 0.9 + D.TireDiameterSampleRL * 0.1;
				}
				if (D.TireDiameterSampleRR > 1.0)
				{
					if (D.TireDiameterSampleRR > 3.0)
						D.TireDiameterSampleRR = 0.66;
					else if (D.TireDiameterSampleCount > 0 && Math.Abs(D.TireDiameterSampleRR - D.TireDiameterRR) > 0.2 * D.TireDiameterRR)
						D.TireDiameterSampleRR = D.TireDiameterRR * 0.9 + D.TireDiameterSampleRR * 0.1;
				}
				double num1 = Math.Min(Math.Abs(D.YawRate) * 0.1, 1.0);
				if (D.YawRate < 0.0)
					D.TireDiameterSampleFR = D.TireDiameterSampleFL * num1 + D.TireDiameterSampleFR * (1.0 - num1);
				else
					D.TireDiameterSampleFL = D.TireDiameterSampleFR * num1 + D.TireDiameterSampleFL * (1.0 - num1);
				if (D.TireDiameterSampleCount == 0)
				{
					D.TireDiameterFL = D.TireDiameterSampleFL;
					D.TireDiameterFR = D.TireDiameterSampleFR;
					D.TireDiameterRL = D.TireDiameterSampleRL;
					D.TireDiameterRR = D.TireDiameterSampleRR;
				}
				else
				{
					double num2 = (0.5 * D.TireDiameterSampleMax + D.TireDiameterSampleCount) / (2 * D.TireDiameterSampleMax);
					D.TireDiameterFL = D.TireDiameterFL * num2 + D.TireDiameterSampleFL * (1.0 - num2);
					D.TireDiameterFR = D.TireDiameterFR * num2 + D.TireDiameterSampleFR * (1.0 - num2);
					D.TireDiameterRL = D.TireDiameterRL * num2 + D.TireDiameterSampleRL * (1.0 - num2);
					D.TireDiameterRR = D.TireDiameterRR * num2 + D.TireDiameterSampleRR * (1.0 - num2);
				}
				++D.TireDiameterSampleCount;
				if (D.TireDiameterSampleCount == D.TireDiameterSampleMax)
				{
					if (Math.Abs(D.TireDiameterFL - D.TireDiameterFR) < 0.1)
					{
						D.TireDiameterFL = (D.TireDiameterFL + D.TireDiameterFR) * 0.5;
						D.TireDiameterFR = D.TireDiameterFL;
					}
					if (Math.Abs(D.TireDiameterRL - D.TireDiameterRR) < 0.1)
					{
						D.TireDiameterRL = (D.TireDiameterRL + D.TireDiameterRR) * 0.5;
						D.TireDiameterRR = D.TireDiameterRL;
					}
				}
			}
			if (D.TireDiameterFL <= 0.0)
				return;
			D.WheelSpeedFL = D.TireDiameterFL * D.WheelRotationFL * 0.5;
			D.WheelSpeedFR = D.TireDiameterFR * D.WheelRotationFR * 0.5;
			D.WheelSpeedRL = D.TireDiameterRL * D.WheelRotationRL * 0.5;
			D.WheelSpeedRR = D.TireDiameterRR * D.WheelRotationRR * 0.5;
			SlipFromWheelSpeed();
		}

		private void SlipFromWheelSpeed()
		{
			if (D.SpeedMs <= 0.05)
				return;
			D.SlipYFL = (D.SpeedMs - D.WheelSpeedFL) * D.InvSpeedMs;
			D.SlipYFR = (D.SpeedMs - D.WheelSpeedFR) * D.InvSpeedMs;
			D.SlipYRL = (D.SpeedMs - D.WheelSpeedRL) * D.InvSpeedMs;
			D.SlipYRR = (D.SpeedMs - D.WheelSpeedRR) * D.InvSpeedMs;
			if (D.SpeedMs >= 3.0)
				return;
			D.SlipYFL *= D.SpeedMs * 0.333;
			D.SlipYFR *= D.SpeedMs * 0.333;
			D.SlipYRL *= D.SpeedMs * 0.333;
			D.SlipYRR *= D.SpeedMs * 0.333;
		}

		private void SetVehiclePerGame(PluginManager pluginManager, ref StatusDataBase db)
		{
			switch (CurrentGame)
			{
				case GameId.AC:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.ACC:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.AMS1:
					if (S.Id != db.CarId && S.Category != db.CarClass && FailedId != db.CarId && FailedCategory != db.CarClass)
					{
						FetchCarData(db.CarId, db.CarClass, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.AMS2:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						S.Name = db.CarModel;
						S.Category = db.CarClass;
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.D4:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = 10.0 * (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.IdleRpm");
						break;
					}
					break;
				case GameId.DR2:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = 10.0 * (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.IdleRpm");
						break;
					}
					break;
				case GameId.WRC23:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, Math.Floor(db.CarSettings_CurrentGearRedLineRPM), db.MaxRpm);
						D.IdleRPM = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_engine_rpm_idle");
						break;
					}
					break;
				case GameId.F12022:
				case GameId.F12023:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = 10.0 * (double) (float) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.PlayerCarStatusData.m_idleRPM");
						break;
					}
					break;
				case GameId.Forza:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId.Substring(4), null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.EngineIdleRpm");
						break;
					}
					break;
				case GameId.GTR2:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.IRacing:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = (double) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.DriverInfo.DriverCarIdleRPM");
						D.GameAltText = pluginManager.GameName + (string) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.WeekendInfo.Category");
						break;
					}
					break;
				case GameId.PC2:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.RBR:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.RF2:
					if (S.Id != db.CarId && S.Category != db.CarClass && FailedId != db.CarId && FailedCategory != db.CarClass)
					{
						FetchCarData(db.CarId, db.CarClass, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.RRRE:
					if (S.Id != db.CarModel && FailedId != db.CarModel)
					{
						FetchCarData(db.CarModel, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.BeamNG:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						S.Redline = db.MaxRpm;
						S.MaxRPM = Math.Ceiling(db.MaxRpm * 0.001) - db.MaxRpm * 0.001 > 0.55
								 ? Math.Ceiling(db.MaxRpm * 0.001) * 1000.0
								 : Math.Ceiling((db.MaxRpm + 1000.0) * 0.001) * 1000.0;
						FetchCarData(db.CarId, null, S, S.Redline, S.MaxRPM);
						D.IdleRPM = (float)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.idle_rpm");
						break;
					}
					break;
				case GameId.GPBikes:
				case GameId.MXBikes:
					if (S.Id != db.CarId)
					{
						S.Id = db.CarId;
						D.IdleRPM = db.MaxRpm * 0.25;
						S.MaxRPM = db.MaxRpm;
						S.Redline = (int)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.m_sEvent.m_iShiftRPM");
						LoadFinish = false;
						FetchStatus = APIStatus.Fail;
						break;
					}
					break;
				case GameId.LMU:
					if (S.Id != db.CarId && S.Category != db.CarClass && FailedId != db.CarId && FailedCategory != db.CarClass)
					{
						FetchCarData(db.CarId, db.CarClass, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					if (S.Id != db.CarId && FailedId != db.CarId)
					{
						FetchCarData(db.CarId, null, S, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
						S.Redline = (short)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.MinAlertRPM");
						S.MaxRPM = (short)pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.MaxAlertRPM");
						D.IdleRPM = db.MaxRpm * 0.25;
						break;
					}
					break;
				default:
					D.IdleRPM = db.MaxRpm * 0.25;
					S.Redline = db.CarSettings_CurrentGearRedLineRPM;
					S.MaxRPM = db.MaxRpm;
					FetchStatus = APIStatus.Fail;
					break;
			}
			if (!LoadFinish && FetchStatus == APIStatus.Success)
			{
				Settings.Vehicle = new Spec(S);
				LoadStatus = DataStatus.SimHapticsAPI;
				D.LoadStatusText = "DB Load Success";
				FinalizeVehicleLoad();
			}
			if (!LoadFinish && FetchStatus == APIStatus.Fail)
			{
				SetDefaultVehicle(ref db);
				FinalizeVehicleLoad();
			}
			D.Gears = db.CarSettings_MaxGears > 0 ? db.CarSettings_MaxGears : 1;
			D.GearInterval = 1 / D.Gears;
		}

		private void SetDefaultVehicle(ref StatusDataBase db)
		{
			if (Settings.Vehicle != null && (Settings.Vehicle.Id == db.CarId || Settings.Vehicle.Id == db.CarModel))
			{
				S = Settings.Vehicle;
				LoadStatus = DataStatus.SettingsFile;
				D.LoadStatusText = "Load Fail: Loaded from Settings";
				return;
			}
			S.Game = GameDBText;
			S.Name = db.CarModel;
			S.Id = db.CarId;
			S.Category = db.CarClass;
			S.EngineConfiguration = "V";
			S.EngineCylinders = 6.0;
			S.EngineLocation = "RM";
			S.PoweredWheels = "A";
			S.Displacement = 3000.0;
			S.MaxPower = 300.0;
			S.ElectricMaxPower = 0.0;
			S.MaxTorque = 250.0;
			LoadStatus = DataStatus.GameData;
			switch (CurrentGame)
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
					D.LoadStatusText = "Not in DB: using generic car";
					break;
				case GameId.D4:
				case GameId.DR2:
				case GameId.WRC23:
					D.LoadStatusText = "Not in DB: using generic Rally2";
					S.EngineConfiguration = "I";
					S.EngineCylinders = 4.0;
					S.EngineLocation = "F";
					S.PoweredWheels = "A";
					S.Displacement = 1600.0;
					S.MaxPower = 300.0;
					S.ElectricMaxPower = 0.0;
					S.MaxTorque = 400.0;
					break;
				case GameId.F12022:
				case GameId.F12023:
					D.LoadStatusText = "Not in DB: using generic F1";
					S.EngineConfiguration = "V";
					S.EngineCylinders = 6.0;
					S.EngineLocation = "RM";
					S.PoweredWheels = "R";
					S.Displacement = 1600.0;
					S.MaxPower = 1000.0;
					S.ElectricMaxPower = 0.0;
					S.MaxTorque = 650.0;
					break;
				case GameId.KK:
					D.LoadStatusText = "Not in DB: using generic Kart";
					S.EngineConfiguration = "I";
					S.EngineCylinders = 1.0;
					S.EngineLocation = "RM";
					S.PoweredWheels = "R";
					S.Displacement = 130.0;
					S.MaxPower = 34.0;
					S.ElectricMaxPower = 0.0;
					S.MaxTorque = 24.0;
					break;
				case GameId.GPBikes:
					D.LoadStatusText = "Not in DB: using generic Superbike";
					S.EngineConfiguration = "I";
					S.EngineCylinders = 4.0;
					S.EngineLocation = "M";
					S.PoweredWheels = "R";
					S.Displacement = 998.0;
					S.MaxPower = 200.0;
					S.ElectricMaxPower = 0.0;
					S.MaxTorque = 100.0;
					break;
				case GameId.MXBikes:
					D.LoadStatusText = "Not in DB: using generic MX Bike";
					S.EngineConfiguration = "I";
					S.EngineCylinders = 1.0;
					S.EngineLocation = "M";
					S.PoweredWheels = "R";
					S.Displacement = 450.0;
					S.MaxPower = 50.0;
					S.ElectricMaxPower = 0.0;
					S.MaxTorque = 45.0;
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					D.LoadStatusText = "Not in DB: redline loaded from game";
					S.EngineConfiguration = "V";
					S.EngineCylinders = 6.0;
					S.EngineLocation = "RM";
					S.PoweredWheels = "R";
					S.Displacement = 4000.0;
					S.MaxPower = 500.0;
					S.ElectricMaxPower = 0.0;
					S.MaxTorque = 400.0;
					break;
				default:
					D.LoadStatusText = "Load Fail: Specs not available for this game";
					break;
			}
			if (CurrentGame != GameId.RRRE && CurrentGame != GameId.D4 && CurrentGame != GameId.DR2)
				return;
			S.Id = db.CarModel;
		}

		private void FinalizeVehicleLoad()
		{
			D.CarInitCount = 0;
			D.IdleSampleCount = 0;
			D.Gear = 0;
			D.SuspensionFL = 0.0;
			D.SuspensionFR = 0.0;
			D.SuspensionRL = 0.0;
			D.SuspensionRR = 0.0;
			D.SuspensionDistFLP = 0.0;
			D.SuspensionDistFRP = 0.0;
			D.SuspensionDistRLP = 0.0;
			D.SuspensionDistRRP = 0.0;
			D.SuspensionVelFLP = 0.0;
			D.SuspensionVelFRP = 0.0;
			D.SuspensionVelRLP = 0.0;
			D.SuspensionVelRRP = 0.0;
			D.SuspensionAccFLP = 0.0;
			D.SuspensionAccFRP = 0.0;
			D.SuspensionAccRLP = 0.0;
			D.SuspensionAccRRP = 0.0;
			Array.Clear(D.AccHeave, 0, D.AccHeave.Length);
			Array.Clear(D.AccSurge, 0, D.AccSurge.Length);
			Array.Clear(D.AccSway, 0, D.AccSway.Length);
			D.Acc1 = 0;
			D.TireDiameterSampleCount = D.TireDiameterSampleCount == -1 ? -1 : 0;
			D.TireDiameterFL = 0.0;
			D.TireDiameterFR = 0.0;
			D.TireDiameterRL = 0.0;
			D.TireDiameterRR = 0.0;
			D.RumbleLeftAvg = 0.0;
			D.RumbleRightAvg = 0.0;
			SetRPMIntervals();
			SetRPMMix();
			LoadFinish = true;
		}

		private void SetRPMIntervals()
		{
			if (S.EngineCylinders == 1.0)
			{
				D.IntervalOctave = 4.0;
				D.IntervalA = 0.0;
				D.IntervalB = 0.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 2.0)
			{
				D.IntervalOctave = 4.0;
				D.IntervalA = 6.0;
				D.IntervalB = 0.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 4.0)
			{
				D.IntervalOctave = 8.0;
				D.IntervalA = 4.0;
				D.IntervalB = 5.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 8.0)
			{
				D.IntervalOctave = 16.0;
				D.IntervalA = 6.0;
				D.IntervalB = 10.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 16.0)
			{
				D.IntervalOctave = 16.0;
				D.IntervalA = 12.0;
				D.IntervalB = 20.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 3.0)
			{
				D.IntervalOctave = 6.0;
				D.IntervalA = 4.0;
				D.IntervalB = 0.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 6.0)
			{
				D.IntervalOctave = 12.0;
				D.IntervalA = 8.0;
				D.IntervalB = 10.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 12.0)
			{
				D.IntervalOctave = 12.0;
				D.IntervalA = 16.0;
				D.IntervalB = 20.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 5.0)
			{
				D.IntervalOctave = 10.0;
				D.IntervalA = 6.0;
				D.IntervalB = 9.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineCylinders == 10.0)
			{
				D.IntervalOctave = 10.0;
				D.IntervalA = 12.0;
				D.IntervalB = 18.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else if (S.EngineConfiguration == "R")
			{
				D.IntervalOctave = 12.0;
				D.IntervalA = 9.0;
				D.IntervalB = 15.0;
				D.IntervalPeakA = 8.0;
				D.IntervalPeakB = 4.0;
			}
			else
			{
				D.IntervalOctave = 0.0;
				D.IntervalA = 0.0;
				D.IntervalB = 0.0;
				D.IntervalPeakA = 0.0;
				D.IntervalPeakB = 0.0;
			}
		}

		private void SetRPMMix()
		{
			D.InvMaxRPM = S.MaxRPM > 0.0 ? 1.0 / S.MaxRPM : 0.0001;
			D.IdlePercent = D.IdleRPM * D.InvMaxRPM;
			D.RedlinePercent = S.Redline * D.InvMaxRPM;
			if (S.Displacement > 0.0)
			{
				D.CylinderDisplacement = S.Displacement / S.EngineCylinders;
				D.MixCylinder = 1.0 - Math.Max(2000.0 - D.CylinderDisplacement, 0.0) * Math.Max(2000.0 - D.CylinderDisplacement, 0.0) * 2.5E-07;
				D.MixDisplacement = 1.0 - Math.Max(10000.0 - S.Displacement, 0.0) * Math.Max(10000.0 - S.Displacement, 0.0) * 1E-08;
			}
			else
			{
				D.MixCylinder = 0.0;
				D.MixDisplacement = 0.0;
			}
			D.MixPower = 1.0 - Math.Max(2000.0 - (S.MaxPower - S.ElectricMaxPower), 0.0) * Math.Max(2000.0 - (S.MaxPower - S.ElectricMaxPower), 0.0) * 2.5E-07;
			D.MixTorque = 1.0 - Math.Max(2000.0 - S.MaxTorque, 0.0) * Math.Max(2000.0 - S.MaxTorque, 0.0) * 2.5E-07;
			D.MixFront = !(S.EngineLocation == "F")
						 ? (!(S.EngineLocation == "FM")
							 ? (!(S.EngineLocation == "M")
								 ? (!(S.EngineLocation == "RM")
									 ? (!(S.PoweredWheels == "F")
										 ? (!(S.PoweredWheels == "R")
											 ? 0.2
											 : 0.1)
										 : 0.3)
									 : (!(S.PoweredWheels == "F")
									 ? (!(S.PoweredWheels == "R")
										 ? 0.3
										 : 0.2)
									 : 0.4)
								   )
								 : (!(S.PoweredWheels == "F")
								 	? (!(S.PoweredWheels == "R")
									 	? 0.5
									 	: 0.4
									  )
									 : 0.6
								   )
							   )
						 : (!(S.PoweredWheels == "F")
							 ? (!(S.PoweredWheels == "R")
								 ? 0.7
								 : 0.6
							   )
							 : 0.8
						   )
						)
					 : (!(S.PoweredWheels == "F")
					 	? (!(S.PoweredWheels == "R")
							 ? 0.8
					 	 	: 0.7
							)
					 	: 0.9
					   );
			D.MixMiddle = Math.Abs(D.MixFront - 0.5) * 2.0;
			D.MixRear = 1.0 - D.MixFront;
		}

		public void SetGame(PluginManager pluginManager, SimData sd)
		{
			GameDBText = pluginManager.GameName;
			sd.GameAltText = pluginManager.GameName;
			switch (GameDBText)
			{
				case "AssettoCorsa":
					CurrentGame = GameId.AC;
					GameDBText = "AC";
					sd.RumbleFromPlugin = true;
					break;
				case "AssettoCorsaCompetizione":
					CurrentGame = GameId.ACC;
					GameDBText = "ACC";
					break;
				case "Automobilista":
					CurrentGame = GameId.AMS1;
					GameDBText = "AMS1";
					break;
				case "Automobilista2":
					CurrentGame = GameId.AMS2;
					GameDBText = "AMS2";
					break;
				case "FH4":
				case "FH5":
				case "FM7":
				case "FM8":
					CurrentGame = GameId.Forza;
					GameDBText = "Forza";
					break;
				case "IRacing":
					CurrentGame = GameId.IRacing;
					sd.GameAltText += (string) pluginManager.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.WeekendInfo.Category");
					break;
				case "KartKraft":
					CurrentGame = GameId.KK;
					GameDBText = "KK";
					break;
				case "LFS":
					CurrentGame = GameId.LFS;
					break;
				case "PCars1":
				case "PCars2":
				case "PCars3":
					CurrentGame = GameId.PC2;
					GameDBText = "PC2";
					break;
				case "RBR":
					CurrentGame = GameId.RBR;
					sd.TireDiameterSampleCount = -1;
					break;
				case "RFactor1":
					CurrentGame = GameId.RF1;
					GameDBText = "RF1";
					break;
				case "RFactor2":
				case "RFactor2Spectator":
					CurrentGame = GameId.RF2;
					GameDBText = "RF2";
					break;
				case "LMU":
					CurrentGame = GameId.LMU;
					GameDBText = "LMU";
					break;
				case "RRRE":
					CurrentGame = GameId.RRRE;
					break;
				case "SIMBINGTLEGENDS":
					CurrentGame = GameId.GTL;
					GameDBText = "GTL";
					break;
				case "SIMBINGTR2":
					CurrentGame = GameId.GTR2;
					GameDBText = "GTR2";
					break;
				case "SIMBINRACE07":
					CurrentGame = GameId.RACE07;
					GameDBText = "RACE07";
					break;
				case "StockCarExtreme":
					CurrentGame = GameId.GSCE;
					GameDBText = "GSCE";
					break;
				case "CodemastersDirtRally1":
				case "CodemastersDirtRally2":
					CurrentGame = GameId.DR2;
					GameDBText = "DR2";
					sd.TireDiameterSampleCount = -1;
					break;
				case "CodemastersDirt2":
				case "CodemastersDirt3":
				case "CodemastersDirtShowdown":
				case "CodemastersDirt4":
					CurrentGame = GameId.D4;
					GameDBText = "D4";
					sd.TireDiameterSampleCount = -1;
					break;
				case "EAWRC23":
					CurrentGame = GameId.WRC23;
					GameDBText = "WRC23";
					sd.AccSamples = 32;
					sd.TireDiameterSampleCount = -1;
					break;
				case "F12012":
				case "F12013":
				case "F12014":
				case "F12015":
				case "F12016":
					CurrentGame = GameId.F12016;
					GameDBText = "F12016";
					break;
				case "F12017":
					CurrentGame = GameId.F12017;
					sd.TireDiameterSampleCount = -1;
					break;
				case "F12018":
				case "F12019":
				case "F12020":
				case "F12021":
				case "F12022":
					CurrentGame = GameId.F12022;
					GameDBText = "F12022";
					sd.TireDiameterSampleCount = -1;
					break;
				case "F12023":
				case "F12024":
				case "F12025":
				case "F12026":
					CurrentGame = GameId.F12023;
					GameDBText = "F12022";
					sd.TireDiameterSampleCount = -1;
					break;
				case "CodemastersGrid2":
				case "CodemastersGrid2019":
				case "CodemastersAutosport":
				case "CodemastersGridLegends":
					CurrentGame = GameId.GLegends;
					GameDBText = "Grid";
					sd.TireDiameterSampleCount = -1;
					break;
				case "BeamNgDrive":
					CurrentGame = GameId.BeamNG;
					GameDBText = "BeamNG";
					sd.TireDiameterSampleCount = -1;
					break;
				case "GPBikes":
					CurrentGame = GameId.GPBikes;
					sd.RumbleFromPlugin = true;
					sd.TireDiameterSampleCount = -1;
					break;
				case "MXBikes":
					CurrentGame = GameId.MXBikes;
					sd.TireDiameterSampleCount = -1;
					break;
				case "WRCGenerations":
					CurrentGame = GameId.WRCGen;
					sd.TireDiameterSampleCount = -1;
					break;
				case "WRCX":
					CurrentGame = GameId.WRCX;
					break;
				case "WRC10":
					CurrentGame = GameId.WRC10;
					break;
				case "ATS":
					CurrentGame = GameId.ATS;
					break;
				case "ETS2":
					CurrentGame = GameId.ETS2;
					break;
				case "GranTurismo7":
				case "GranTurismoSport":
					CurrentGame = GameId.GranTurismo7;
					GameDBText = "GranTurismo7";
					sd.TireDiameterSampleCount = -1;
					break;
				default:
					CurrentGame = GameId.Other;
					break;
			}
			D.AccHeave = new double[D.AccSamples];
			D.AccSurge = new double[D.AccSamples];
			D.AccSway = new double[D.AccSamples];
		}

		private static JToken jtoken;									// shared between FetchCarData() and Untoken()
		private static double Untoken(string token, double trouble)		// helper for FetchCarData()
		{
            return double.TryParse((string)jtoken[token], out double result) ? result : trouble;
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
				if (FetchStatus == APIStatus.Waiting)
					return;
				FetchStatus = APIStatus.Waiting;
				LoadFinish = false;
				Logging.Current.Info("SimHapticsPlugin: Loading " + category + " " + id);
				id ??= "0";
				category ??= "0";
				Uri requestUri = new("https://api.simhaptics.com/data/" + GameDBText
								 + "/" + Uri.EscapeDataString(id) + "/" + Uri.EscapeDataString(category));
				HttpResponseMessage async = await client.GetAsync(requestUri);
				async.EnsureSuccessStatusCode();
				string dls;
				JObject jobject = (JObject) JsonConvert.DeserializeObject(dls = async.Content.ReadAsStringAsync().Result);
				Logging.Current.Info(jobject);
				if (jobject["data"].HasValues)
				{
					jtoken = jobject["data"][0];
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                 
                    Download dljc = JsonConvert.DeserializeObject<Download>(dls, settings);
					v.Game = GameDBText;
					v.Id = Convert.ToString(jtoken[nameof(id)]);
					v.Category = Convert.ToString(jtoken[nameof(category)]);
					v.Name = Convert.ToString(jtoken["name"]);
					v.EngineLocation = Convert.ToString(jtoken["loc"]);
					v.PoweredWheels = Convert.ToString(jtoken["drive"]);
					v.EngineConfiguration = Convert.ToString(jtoken["config"]);
					v.EngineCylinders = Untoken("cyl", 0.0);
					v.Redline = Untoken("redline", redlineFromGame);
					v.MaxRPM = Untoken("maxrpm", maxRPMFromGame);
					v.MaxPower = Untoken("hp", 333.0);
					v.ElectricMaxPower = Untoken("ehp", 0.0);
					v.Displacement = Untoken("cc", 3333.0);
					v.MaxTorque = Untoken("nm", v.MaxPower);
					if (CurrentGame == GameId.Forza)
						v.Id = "Car_" + v.Id;
					Logging.Current.Info("SimHapticsPlugin: Successfully loaded " + v.Name);
					LoadFailCount = 0;
					FailedId = "";
					FailedCategory = "";
					FetchStatus = APIStatus.Success;
                    File.WriteAllText("PluginsData/" + v.Name + "." + v.Game + ".Converted.json",
                                      			JsonConvert.SerializeObject(dljc, Formatting.Indented));
                    File.WriteAllText("PluginsData/"+v.Name+"."+v.Game+".jobject.json",
												JsonConvert.SerializeObject(jobject, Formatting.Indented));
				}
				else
				{
					Logging.Current.Info("SimHapticsPlugin: Failed to load " + id + " : " + category);
					++LoadFailCount;
					if (LoadFailCount > 3)
					{
						FailedId = CurrentGame != GameId.Forza ? id : "Car_" + id;
						FailedCategory = category;
						FetchStatus = APIStatus.Fail;
					}
					else
						FetchStatus = APIStatus.Retry;
				}
			}
			catch (HttpRequestException ex)
			{
				Logging.Current.Error("SimHapticsPlugin: " + ex.Message);
				LoadFailCount = 0;
				FetchStatus = APIStatus.Retry;
			}
		}

		public void End(PluginManager pluginManager)
		{
/*
			string sjs = JsonConvert.SerializeObject(D, Formatting.Indented);
			if (0 == sjs.Length || "{}" == sjs)
                Logging.Current.Info("SimHapticsPlugin.End():  SimData Json Serializer failure");
			else File.WriteAllText("PluginsData/"+S.Name+"."+S.Game+".SimData.json", sjs);

			sjs = JsonConvert.SerializeObject(S, Formatting.Indented);
			if (0 == sjs.Length || "{}" == sjs)
                Logging.Current.Info("SimHapticsPlugin.End():  Spec Json Serializer failure");
			else File.WriteAllText("PluginsData/"+S.Name+"."+S.Game+".Spec.json", sjs);
*/
			if (Settings.EngineMult.TryGetValue("AllGames", out double _))
				Settings.EngineMult.Remove("AllGames");
			if (Settings.EngineMult.TryGetValue(GameDBText, out double _))
			{
				if (D.EngineMult == 1.0)
					Settings.EngineMult.Remove(GameDBText);
				else
					Settings.EngineMult[GameDBText] = D.EngineMult;
			}
			else if (D.EngineMult != 1.0)
				Settings.EngineMult.Add(GameDBText, D.EngineMult);
			if (Settings.RumbleMult.TryGetValue(GameDBText, out double _))
			{
				if (D.RumbleMult == 1.0)
					Settings.RumbleMult.Remove(GameDBText);
				else
					Settings.RumbleMult[GameDBText] = D.RumbleMult;
			}
			else if (D.RumbleMult != 1.0)
				Settings.RumbleMult.Add(GameDBText, D.RumbleMult);
			if (Settings.SuspensionMult.TryGetValue(GameDBText, out double _))
			{
				if (D.SuspensionMult == 1.0)
					Settings.SuspensionMult.Remove(GameDBText);
				else
					Settings.SuspensionMult[GameDBText] = D.SuspensionMult;
			}
			else if (D.SuspensionMult != 1.0)
				Settings.SuspensionMult.Add(GameDBText, D.SuspensionMult);
			if (Settings.SuspensionGamma.TryGetValue(GameDBText, out double _))
			{
				if (D.SuspensionGamma == 1.0)
					Settings.SuspensionGamma.Remove(GameDBText);
				else
					Settings.SuspensionGamma[GameDBText] = D.SuspensionGamma;
			}
			else if (D.SuspensionGamma != 1.0)
				Settings.SuspensionGamma.Add(GameDBText, D.SuspensionGamma);
			if (Settings.SlipXMult.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipXMult == 1.0)
					Settings.SlipXMult.Remove(GameDBText);
				else
					Settings.SlipXMult[GameDBText] = D.SlipXMult;
			}
			else if (D.SlipXMult != 1.0)
				Settings.SlipXMult.Add(GameDBText, D.SlipXMult);
			if (Settings.SlipYMult.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipYMult == 1.0)
					Settings.SlipYMult.Remove(GameDBText);
				else
					Settings.SlipYMult[GameDBText] = D.SlipYMult;
			}
			else if (D.SlipYMult != 1.0)
				Settings.SlipYMult.Add(GameDBText, D.SlipYMult);
			if (Settings.SlipXGamma.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipXGamma == 1.0)
					Settings.SlipXGamma.Remove(GameDBText);
				else
					Settings.SlipXGamma[GameDBText] = D.SlipXGamma;
			}
			else if (D.SlipXGamma != 1.0)
				Settings.SlipXGamma.Add(GameDBText, D.SlipXGamma);
			if (Settings.SlipYGamma.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipYGamma == 1.0)
					Settings.SlipYGamma.Remove(GameDBText);
				else
					Settings.SlipYGamma[GameDBText] = D.SlipYGamma;
			}
			else if (D.SlipYGamma != 1.0)
				Settings.SlipYGamma.Add(GameDBText, D.SlipYGamma);
			Settings.RumbleMult["AllGames"] = D.RumbleMultAll;
			Settings.SuspensionGamma["AllGames"] = D.SuspensionGammaAll;
			Settings.SuspensionMult["AllGames"] = D.SuspensionMultAll;
			Settings.SlipXMult["AllGames"] = D.SlipXMultAll;
			Settings.SlipYMult["AllGames"] = D.SlipYMultAll;
			Settings.Motion["MotionPitchOffset"] = D.MotionPitchOffset;
			Settings.Motion["MotionPitchMult"] = D.MotionPitchMult;
			Settings.Motion["MotionPitchGamma"] = D.MotionPitchGamma;
			Settings.Motion["MotionRollOffset"] = D.MotionRollOffset;
			Settings.Motion["MotionRollMult"] = D.MotionRollMult;
			Settings.Motion["MotionRollGamma"] = D.MotionRollGamma;
			Settings.Motion["MotionYawOffset"] = D.MotionYawOffset;
			Settings.Motion["MotionYawMult"] = D.MotionYawMult;
			Settings.Motion["MotionYawGamma"] = D.MotionYawGamma;
			Settings.Motion["MotionHeaveOffset"] = D.MotionHeaveOffset;
			Settings.Motion["MotionHeaveMult"] = D.MotionHeaveMult;
			Settings.Motion["MotionHeaveGamma"] = D.MotionHeaveGamma;
			Settings.Motion["MotionSurgeOffset"] = D.MotionSurgeOffset;
			Settings.Motion["MotionSurgeMult"] = D.MotionSurgeMult;
			Settings.Motion["MotionSurgeGamma"] = D.MotionSurgeGamma;
			Settings.Motion["MotionSwayOffset"] = D.MotionSwayOffset;
			Settings.Motion["MotionSwayMult"] = D.MotionSwayMult;
			Settings.Motion["MotionSwayGamma"] = D.MotionSwayGamma;
			IPluginExtensions.SaveCommonSettings(this, "Settings", Settings);
		}

		public Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return new SettingsControl(this);
		}

		double GetSetting(string name, double trouble)	// Init() helper
		{
            return Settings.Motion.TryGetValue(name, out double num) ? num : trouble;
        }

		public void Init(PluginManager pluginManager)
		{
			SimHubVersion = (string) pluginManager.GetPropertyValue("DataCorePlugin.SimHubVersion");
			LoadFailCount = 0;
			LoadFinish = false;
			LoadStatus = DataStatus.None;
			FetchStatus = APIStatus.None;
			S = new Spec();
			D = new SimData()
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
			SetGame(pluginManager, D);
			Settings = IPluginExtensions.ReadCommonSettings(this, "Settings", () => new Settings());
			D.LockedText = Settings.Unlocked ? "Lock" : "Unlock";
			Settings.ABSPulseLength = Settings.ABSPulseLength > 0 ? Settings.ABSPulseLength : 2;
			Settings.DownshiftDurationMs = Settings.DownshiftDurationMs > 0 ? Settings.DownshiftDurationMs : 400;
			Settings.UpshiftDurationMs = Settings.UpshiftDurationMs > 0 ? Settings.UpshiftDurationMs : 400;
			if (Settings.EngineMult == null)
				Settings.EngineMult = new Dictionary<string, double>();
            double num;
            D.EngineMult = !Settings.EngineMult.TryGetValue(GameDBText, out num) ? 1.0 : num;
            if (Settings.EngineMult.TryGetValue("AllGames", out double _))
			{
				D.EngineMultAll = 1.0;
			}
			else
			{
				D.EngineMultAll = 1.0;
				Settings.EngineMult.Add("AllGames", D.EngineMultAll);
			}
			if (Settings.RumbleMult == null)
				Settings.RumbleMult = new Dictionary<string, double>();
			
			D.RumbleMult = !Settings.RumbleMult.TryGetValue(GameDBText, out num) ? 1.0 : num;
			
			if (Settings.RumbleMult.TryGetValue("AllGames", out num))
				D.RumbleMultAll = num;
			else
			{
				D.RumbleMultAll = 5.0;
				Settings.RumbleMult.Add("AllGames", D.RumbleMultAll);
			}
			if (Settings.SuspensionMult == null)
				Settings.SuspensionMult = new Dictionary<string, double>();
			D.SuspensionMult = !Settings.SuspensionMult.TryGetValue(GameDBText, out num) ? 1.0 : num;
			if (Settings.SuspensionMult.TryGetValue("AllGames", out num))
				D.SuspensionMultAll = num;
			else
			{
				D.SuspensionMultAll = 1.5;
				Settings.SuspensionMult.Add("AllGames", D.SuspensionMultAll);
			}
			if (Settings.SuspensionGamma == null)
				Settings.SuspensionGamma = new Dictionary<string, double>();
			D.SuspensionGamma = !Settings.SuspensionGamma.TryGetValue(GameDBText, out num) ? 1.0 : num;
			if (Settings.SuspensionGamma.TryGetValue("AllGames", out num))
				D.SuspensionGammaAll = num;
			else
			{
				D.SuspensionGammaAll = 1.75;
				Settings.SuspensionGamma.Add("AllGames", D.SuspensionGammaAll);
			}
			if (Settings.SlipXMult == null)
				Settings.SlipXMult = new Dictionary<string, double>();
			D.SlipXMult = !Settings.SlipXMult.TryGetValue(GameDBText, out num) ? 1.0 : num;
			if (Settings.SlipXMult.TryGetValue("AllGames", out num))
			{
				D.SlipXMultAll = num;
			}
			else
			{
				D.SlipXMultAll = 1.0;
				Settings.SlipXMult.Add("AllGames", D.SlipXMultAll);
			}
			if (Settings.SlipYMult == null)
				Settings.SlipYMult = new Dictionary<string, double>();
			D.SlipYMult = !Settings.SlipYMult.TryGetValue(GameDBText, out num) ? 1.0 : num;
			if (Settings.SlipYMult.TryGetValue("AllGames", out num))
			{
				D.SlipYMultAll = num;
			}
			else
			{
				D.SlipYMultAll = 1.0;
				Settings.SlipYMult.Add("AllGames", D.SlipYMultAll);
			}
			if (Settings.SlipXGamma == null)
				Settings.SlipXGamma = new Dictionary<string, double>();
			D.SlipXGamma = !Settings.SlipXGamma.TryGetValue(GameDBText, out num) ? 1.0 : num;
			if (Settings.SlipXGamma.TryGetValue("AllGames", out num))
			{
				D.SlipXGammaAll = num;
			}
			else
			{
				D.SlipXGammaAll = 1.0;
				Settings.SlipXGamma.Add("AllGames", D.SlipXGammaAll);
			}
			if (Settings.SlipYGamma == null)
				Settings.SlipYGamma = new Dictionary<string, double>();
			D.SlipYGamma = !Settings.SlipYGamma.TryGetValue(GameDBText, out num) ? 1.0 : num;
			if (Settings.SlipYGamma.TryGetValue("AllGames", out num))
			{
				D.SlipYGammaAll = num;
			}
			else
			{
				D.SlipYGammaAll = 1.0;
				Settings.SlipYGamma.Add("AllGames", D.SlipYGammaAll);
			}
			if (Settings.Motion == null)
				Settings.Motion = new Dictionary<string, double>();
			D.MotionPitchOffset = GetSetting("MotionPitchOffset", 0.0);
			D.MotionPitchMult = GetSetting("MotionPitchMult", 1.6);
			D.MotionPitchGamma = GetSetting("MotionPitchGamma", 1.5);
			D.MotionRollOffset = GetSetting("MotionRollOffset", 0.0);
			D.MotionRollMult = GetSetting("MotionRollMult", 1.2);
			D.MotionRollGamma = GetSetting("MotionRollGamma", 1.5);
			D.MotionYawOffset = GetSetting("MotionYawOffset", 0.0);
			D.MotionYawMult = GetSetting("MotionYawMult", 1.0);
			D.MotionYawGamma = GetSetting("MotionYawGamma", 1.0);
			D.MotionHeaveOffset = GetSetting("MotionHeaveOffset", 0.0);
			D.MotionHeaveMult = GetSetting("MotionHeaveMult", 1.0);
			D.MotionHeaveGamma = GetSetting("MotionHeaveGamma", 1.0);
			D.MotionSurgeOffset = GetSetting("MotionSurgeOffset", 0.0);
			D.MotionSurgeMult = GetSetting("MotionSurgeMult", 1.0);
			D.MotionSurgeGamma = GetSetting("MotionSurgeGamma", 1.0);
			D.MotionSwayOffset = GetSetting("MotionSwayOffset", 0.0);
			D.MotionSwayMult = GetSetting("MotionSwayMult", 1.0);
			D.MotionSwayGamma = GetSetting("MotionSwayGamma", 1.0);
			IPluginExtensions.AttachDelegate(this, "CarName", () => S.Name);
			IPluginExtensions.AttachDelegate(this, "CarId", () => S.Id);
			IPluginExtensions.AttachDelegate(this, "Category", () => S.Category);
			IPluginExtensions.AttachDelegate(this, "RedlineRPM", () => S.Redline);
			IPluginExtensions.AttachDelegate(this, "MaxRPM", () => S.MaxRPM);
			IPluginExtensions.AttachDelegate(this, "EngineConfig", () => S.EngineConfiguration);
			IPluginExtensions.AttachDelegate(this, "EngineCylinders", () => S.EngineCylinders);
			IPluginExtensions.AttachDelegate(this, "EngineLocation", () => S.EngineLocation);
			IPluginExtensions.AttachDelegate(this, "PoweredWheels", () => S.PoweredWheels);
			IPluginExtensions.AttachDelegate(this, "DisplacementCC", () => S.Displacement);
			IPluginExtensions.AttachDelegate(this, "PowerTotalHP", () => S.MaxPower);
			IPluginExtensions.AttachDelegate(this, "PowerEngineHP", () => S.MaxPower - S.ElectricMaxPower);
			IPluginExtensions.AttachDelegate(this, "PowerMotorHP", () => S.ElectricMaxPower);
			IPluginExtensions.AttachDelegate(this, "MaxTorqueNm", () => S.MaxTorque);
			IPluginExtensions.AttachDelegate(this, "LoadStatus", () => (int)LoadStatus);
			IPluginExtensions.AttachDelegate(this, "EngineLoad", () => D.EngineLoad);
			IPluginExtensions.AttachDelegate(this, "IdleRPM", () => D.IdleRPM);
			IPluginExtensions.AttachDelegate(this, "FreqHarmonic", () => D.FreqHarmonic);
			IPluginExtensions.AttachDelegate(this, "FreqOctave", () => D.FreqOctave);
			IPluginExtensions.AttachDelegate(this, "FreqIntervalA1", () => D.FreqIntervalA1);
			IPluginExtensions.AttachDelegate(this, "FreqIntervalA2", () => D.FreqIntervalA2);
			IPluginExtensions.AttachDelegate(this, "FreqLFEAdaptive", () => D.FreqLFEAdaptive);
			IPluginExtensions.AttachDelegate(this, "FreqPeakA1", () => D.FreqPeakA1);
			IPluginExtensions.AttachDelegate(this, "FreqPeakB1", () => D.FreqPeakB1);
			IPluginExtensions.AttachDelegate(this, "FreqPeakA2", () => D.FreqPeakA2);
			IPluginExtensions.AttachDelegate(this, "FreqPeakB2", () => D.FreqPeakB2);
			IPluginExtensions.AttachDelegate(this, "Gain1H", () => D.Gain1H);
			IPluginExtensions.AttachDelegate(this, "Gain1H2", () => D.Gain1H2);
			IPluginExtensions.AttachDelegate(this, "Gain2H", () => D.Gain2H);
			IPluginExtensions.AttachDelegate(this, "Gain4H", () => D.Gain4H);
			IPluginExtensions.AttachDelegate(this, "GainOctave", () => D.GainOctave);
			IPluginExtensions.AttachDelegate(this, "GainIntervalA1", () => D.GainIntervalA1);
			IPluginExtensions.AttachDelegate(this, "GainIntervalA2", () => D.GainIntervalA2);
			IPluginExtensions.AttachDelegate(this, "GainPeakA1Front", () => D.GainPeakA1Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakA1Middle", () => D.GainPeakA1);
			IPluginExtensions.AttachDelegate(this, "GainPeakA1Rear", () => D.GainPeakA1Rear);
			IPluginExtensions.AttachDelegate(this, "GainPeakA2Front", () => D.GainPeakA2Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakA2Middle", () => D.GainPeakA2);
			IPluginExtensions.AttachDelegate(this, "GainPeakA2Rear", () => D.GainPeakA2Rear);
			IPluginExtensions.AttachDelegate(this, "GainPeakB1Front", () => D.GainPeakB1Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakB1Middle", () => D.GainPeakB1);
			IPluginExtensions.AttachDelegate(this, "GainPeakB1Rear", () => D.GainPeakB1Rear);
			IPluginExtensions.AttachDelegate(this, "GainPeakB2Front", () => D.GainPeakB2Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakB2Middle", () => D.GainPeakB2);
			IPluginExtensions.AttachDelegate(this, "GainPeakB2Rear", () => D.GainPeakB2Rear);
			IPluginExtensions.AttachDelegate(this, "SlipXFL", () => D.SlipXFL);
			IPluginExtensions.AttachDelegate(this, "SlipXFR", () => D.SlipXFR);
			IPluginExtensions.AttachDelegate(this, "SlipXRL", () => D.SlipXRL);
			IPluginExtensions.AttachDelegate(this, "SlipXRR", () => D.SlipXRR);
			IPluginExtensions.AttachDelegate(this, "SlipXAll", () => D.SlipXAll);
			IPluginExtensions.AttachDelegate(this, "SlipYFL", () => D.SlipYFL);
			IPluginExtensions.AttachDelegate(this, "SlipYFR", () => D.SlipYFR);
			IPluginExtensions.AttachDelegate(this, "SlipYRL", () => D.SlipYRL);
			IPluginExtensions.AttachDelegate(this, "SlipYRR", () => D.SlipYRR);
			IPluginExtensions.AttachDelegate(this, "SlipYAll", () => D.SlipYAll);
			IPluginExtensions.AttachDelegate(this, "WheelLockAll", () => D.WheelLockAll);
			IPluginExtensions.AttachDelegate(this, "WheelSpinAll", () => D.WheelSpinAll);
			IPluginExtensions.AttachDelegate(this, "TireDiameterFL", () => D.TireDiameterFL);
			IPluginExtensions.AttachDelegate(this, "TireDiameterFR", () => D.TireDiameterFR);
			IPluginExtensions.AttachDelegate(this, "TireDiameterRL", () => D.TireDiameterRL);
			IPluginExtensions.AttachDelegate(this, "TireDiameterRR", () => D.TireDiameterRR);
			IPluginExtensions.AttachDelegate(this, "TireSpeedFL", () => D.WheelSpeedFL);
			IPluginExtensions.AttachDelegate(this, "TireSpeedFR", () => D.WheelSpeedFR);
			IPluginExtensions.AttachDelegate(this, "TireSpeedRL", () => D.WheelSpeedRL);
			IPluginExtensions.AttachDelegate(this, "TireSpeedRR", () => D.WheelSpeedRR);
			IPluginExtensions.AttachDelegate(this, "SpeedMs", () => D.SpeedMs);
			IPluginExtensions.AttachDelegate(this, "TireLoadFL", () => D.WheelLoadFL);
			IPluginExtensions.AttachDelegate(this, "TireLoadFR", () => D.WheelLoadFR);
			IPluginExtensions.AttachDelegate(this, "TireLoadRL", () => D.WheelLoadRL);
			IPluginExtensions.AttachDelegate(this, "TireLoadRR", () => D.WheelLoadRR);
			IPluginExtensions.AttachDelegate(this, "YawRate", () => D.YawRate);
			IPluginExtensions.AttachDelegate(this, "YawRateAvg", () => D.YawRateAvg);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreq", () => D.SuspensionFreq);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR0a", () => D.SuspensionFreqRa);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR0b", () => D.SuspensionFreqRb);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR0c", () => D.SuspensionFreqRc);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR1", () => D.SuspensionFreqR1);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR2", () => D.SuspensionFreqR2);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR3", () => D.SuspensionFreqR3);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR4", () => D.SuspensionFreqR4);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR5", () => D.SuspensionFreqR5);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR0a", () => D.SuspensionMultRa);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR0b", () => D.SuspensionMultRb);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR0c", () => D.SuspensionMultRc);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR1", () => D.SuspensionMultR1);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR2", () => D.SuspensionMultR2);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR3", () => D.SuspensionMultR3);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR4", () => D.SuspensionMultR4);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR5", () => D.SuspensionMultR5);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR0b", () => D.SuspensionRumbleMultRb);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR0c", () => D.SuspensionRumbleMultRc);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR1", () => D.SuspensionRumbleMultR1);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR2", () => D.SuspensionRumbleMultR2);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR3", () => D.SuspensionRumbleMultR3);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR4", () => D.SuspensionRumbleMultR4);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR5", () => D.SuspensionRumbleMultR5);
			IPluginExtensions.AttachDelegate(this, "SuspensionFL", () => D.SuspensionFL);
			IPluginExtensions.AttachDelegate(this, "SuspensionFR", () => D.SuspensionFR);
			IPluginExtensions.AttachDelegate(this, "SuspensionRL", () => D.SuspensionRL);
			IPluginExtensions.AttachDelegate(this, "SuspensionRR", () => D.SuspensionRR);
			IPluginExtensions.AttachDelegate(this, "SuspensionFront", () => D.SuspensionFront);
			IPluginExtensions.AttachDelegate(this, "SuspensionRear", () => D.SuspensionRear);
			IPluginExtensions.AttachDelegate(this, "SuspensionLeft", () => D.SuspensionLeft);
			IPluginExtensions.AttachDelegate(this, "SuspensionRight", () => D.SuspensionRight);
			IPluginExtensions.AttachDelegate(this, "SuspensionAll", () => D.SuspensionAll);
			IPluginExtensions.AttachDelegate(this, "SuspensionAccAll", () => D.SuspensionAccAll);
			IPluginExtensions.AttachDelegate(this, "RumbleFromPlugin", () => D.RumbleFromPlugin);
			IPluginExtensions.AttachDelegate(this, "RumbleMult", () => D.RumbleMult);
			IPluginExtensions.AttachDelegate(this, "RumbleLeft", () => D.RumbleLeft);
			IPluginExtensions.AttachDelegate(this, "RumbleRight", () => D.RumbleRight);
			IPluginExtensions.AttachDelegate(this, "ABSPulse", () => D.ABSPulse);
			IPluginExtensions.AttachDelegate(this, "Airborne", () => D.Airborne);
			IPluginExtensions.AttachDelegate(this, "Gear", () => D.Gear);
			IPluginExtensions.AttachDelegate(this, "Gears", () => D.Gears);
			IPluginExtensions.AttachDelegate(this, "ShiftDown", () => D.Downshift);
			IPluginExtensions.AttachDelegate(this, "ShiftUp", () => D.Upshift);
			IPluginExtensions.AttachDelegate(this, "WiperStatus", () => D.WiperStatus);
			IPluginExtensions.AttachDelegate(this, "AccHeave", () => D.AccHeave[D.Acc0]);
			IPluginExtensions.AttachDelegate(this, "AccSurge", () => D.AccSurge[D.Acc0]);
			IPluginExtensions.AttachDelegate(this, "AccSway", () => D.AccSway[D.Acc0]);
			IPluginExtensions.AttachDelegate(this, "AccHeave2", () => D.AccHeave2S);
			IPluginExtensions.AttachDelegate(this, "AccSurge2", () => D.AccSurge2S);
			IPluginExtensions.AttachDelegate(this, "AccSway2", () => D.AccSway2S);
			IPluginExtensions.AttachDelegate(this, "AccHeaveAvg", () => D.AccHeaveAvg);
			IPluginExtensions.AttachDelegate(this, "AccSurgeAvg", () => D.AccSurgeAvg);
			IPluginExtensions.AttachDelegate(this, "AccSwayAvg", () => D.AccSwayAvg);
			IPluginExtensions.AttachDelegate(this, "JerkZ", () => D.JerkZ);
			IPluginExtensions.AttachDelegate(this, "JerkY", () => D.JerkY);
			IPluginExtensions.AttachDelegate(this, "JerkX", () => D.JerkX);
			IPluginExtensions.AttachDelegate(this, "JerkYAvg", () => D.JerkYAvg);
			IPluginExtensions.AttachDelegate(this, "MPitch", () => D.MotionPitch);
			IPluginExtensions.AttachDelegate(this, "MRoll", () => D.MotionRoll);
			IPluginExtensions.AttachDelegate(this, "MYaw", () => D.MotionYaw);
			IPluginExtensions.AttachDelegate(this, "MHeave", () => D.MotionHeave);
			IPluginExtensions.AttachDelegate(this, "MSurge", () => D.MotionSurge);
			IPluginExtensions.AttachDelegate(this, "MSway", () => D.MotionSway);
			IPluginExtensions.AttachDelegate(this, "TireSamples", () => D.TireDiameterSampleCount);
			IPluginExtensions.AttachDelegate(this, "VelocityX", () => D.VelocityX);
			FrameTimeTicks = DateTime.Now.Ticks;
		}
	}
}
