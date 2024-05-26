using GameReaderCommon;
using MahApps.Metro.Controls;
using SimHub;
using System;
using System.Collections.Generic;

namespace sierses.Sim
{

	public partial class SimData
	{
		private long FrameTimeTicks;
		private long FrameCountTicks;
		internal Haptics SHP;
		private ushort idleRPM;
		Spec V;

		public SimData()
		{
			GameAltText = "";
			LoadText = "Not Loaded";
			Gear = 0;
			GearPrevious = 0;
			Downshift = false;
			Upshift = false;
			CarInitCount = 0;
			ShiftTicks = FrameTimeTicks = DateTime.Now.Ticks;
			FrameCountTicks = 0;
			IdleSampleCount = 0;
			RumbleFromPlugin = false;
			SuspensionDistFLP = 0.0;
			SuspensionDistFRP = 0.0;
			SuspensionDistRLP = 0.0;
			SuspensionDistRRP = 0.0;
			AccSamples = 16;
			Acc1 = 0;
			TireDiameterSampleMax = 100;
			SlipXGammaBaseMult = 1.0;
			SlipYGammaBaseMult = 1.0;
			idleRPM = 2500;
			V = new();
		}

		Settings MySet;
		double GetSetting(string name, double trouble)	// Init() helper
		{
			return MySet.Motion.TryGetValue(name, out double num) ? num : trouble;
		}

		int Index;
		internal void Init(Settings Settings, Haptics sh)
		{
			Index = -2;
			MySet = Settings;
			SHP = sh;
			string GDBtext = Haptics.GameDBText;
			EngineMult = Settings.EngineMult.TryGetValue(GDBtext, out double num) ? num : 1.0;
			EngineMultAll = Settings.EngineMult.TryGetValue("AllGames", out num) ? num : 1.0;
			RumbleMult = Settings.RumbleMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			RumbleMultAll = Settings.RumbleMult.TryGetValue("AllGames", out num) ? num : 5.0;
			SuspensionMult = Settings.SuspensionMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SuspensionMultAll = Settings.SuspensionMult.TryGetValue("AllGames", out num) ? num : 1.5;
			SuspensionGamma = Settings.SuspensionGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SuspensionGammaAll = Settings.SuspensionGamma.TryGetValue("AllGames", out num) ? num : 1.75;
			SlipXMult = Settings.SlipXMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipXMultAll = Settings.SlipXMult.TryGetValue("AllGames", out num) ? num : 1.6;
			SlipYMult = Settings.SlipYMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipYMultAll = Settings.SlipYMult.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipXGamma = Settings.SlipXGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipXGammaAll = Settings.SlipXGamma.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipYGamma = Settings.SlipYGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipYGammaAll = Settings.SlipYGamma.TryGetValue("AllGames", out num) ? num : 1.0;

			LockedText = Settings.Unlocked ? "Lock" : "Unlock";
			MotionPitchOffset = GetSetting("MotionPitchOffset", 0.0);
			MotionPitchMult = GetSetting("MotionPitchMult", 1.6);
			MotionPitchGamma = GetSetting("MotionPitchGamma", 1.5);
			MotionRollOffset = GetSetting("MotionRollOffset", 0.0);
			MotionRollMult = GetSetting("MotionRollMult", 1.2);
			MotionRollGamma = GetSetting("MotionRollGamma", 1.5);
			MotionYawOffset = GetSetting("MotionYawOffset", 0.0);
			MotionYawMult = GetSetting("MotionYawMult", 1.0);
			MotionYawGamma = GetSetting("MotionYawGamma", 1.0);
			MotionHeaveOffset = GetSetting("MotionHeaveOffset", 0.0);
			MotionHeaveMult = GetSetting("MotionHeaveMult", 1.0);
			MotionHeaveGamma = GetSetting("MotionHeaveGamma", 1.0);
			MotionSurgeOffset = GetSetting("MotionSurgeOffset", 0.0);
			MotionSurgeMult = GetSetting("MotionSurgeMult", 1.0);
			MotionSurgeGamma = GetSetting("MotionSurgeGamma", 1.0);
			MotionSwayOffset = GetSetting("MotionSwayOffset", 0.0);
			MotionSwayMult = GetSetting("MotionSwayMult", 1.0);
			MotionSwayGamma = GetSetting("MotionSwayGamma", 1.0);
		}

		// called from DataUpdate()
		internal void SetVehicle(Haptics shp)
		{
			StatusDataBase db;

			Logging.Current.Info($"Haptics.SetVehicle({shp.Gdat.NewData.CarId}): "
							   + (Haptics.Loaded ? " Loaded " : "") + (Haptics.Waiting ? " Waiting" : ""));
			SHP = shp;
			db = SHP.Gdat.NewData;
			string cid = db.CarId;

			if (Index < -1)
				Index = SHP.S.Lcars.FindIndex(x => x.id == cid);
			if (0 <= Index)
			{
				Haptics.Waiting = false;
				Haptics.dljc = new();	// lock out FetchCarData()
				SHP.S.SelectCar(Index);
				Haptics.Loaded = false;
			}	

			db = SHP.Gdat.NewData;
			switch (Haptics.CurrentGame)
			{
				case GameId.AC:
				case GameId.ACC:
				case GameId.PC2:
				case GameId.RBR:
				case GameId.GTR2:
					Haptics.FetchCarData(db.CarId, null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					break;
				case GameId.AMS1:
				case GameId.LMU:
				case GameId.RF2:
					Haptics.FetchCarData(db.CarId, db.CarClass, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					break;
				case GameId.AMS2:
					Haptics.FetchCarData(db.CarId, null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					SHP.S.CarName = V.CarName = db.CarModel;
					SHP.S.Category = V.Category = db.CarClass;
					break;
				case GameId.D4:
				case GameId.DR2:
					Haptics.FetchCarData(db.CarId, null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					if (0 == SHP.S.IdleRPM)
						SHP.S.IdleRPM = Convert.ToUInt16(10 * (int)SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.IdleRpm"));
					if (0 == V.IdleRPM)
						V.IdleRPM = Convert.ToUInt16(10 * (int)SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.IdleRpm"));
					break;
				case GameId.WRC23:
					Haptics.FetchCarData(db.CarId, null, V, Math.Floor(db.CarSettings_CurrentGearRedLineRPM), db.MaxRpm);
					if (0 == SHP.S.IdleRPM)
						SHP.S.IdleRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_engine_rpm_idle"));
					if (0 == V.IdleRPM)
						V.IdleRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.SessionUpdate.vehicle_engine_rpm_idle"));
					break;
				case GameId.F12022:
				case GameId.F12023:
					Haptics.FetchCarData(db.CarId, null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					if (0 == SHP.S.IdleRPM)
						SHP.S.IdleRPM = Convert.ToUInt16(10 * (int)SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.PlayerCarStatusData.m_idleRPM"));
					if (0 == V.IdleRPM)
						V.IdleRPM = Convert.ToUInt16(10 * (int)SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.PlayerCarStatusData.m_idleRPM"));
					break;
				case GameId.Forza:
					Haptics.FetchCarData(db.CarId.Substring(4), null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					if (0 == SHP.S.IdleRPM)
						SHP.S.IdleRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.EngineIdleRpm"));
					if (0 == V.IdleRPM)
						V.IdleRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.EngineIdleRpm"));
					break;
				case GameId.IRacing:
					Haptics.FetchCarData(db.CarId, null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					GameAltText = SHP.PM.GameName + (string)SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.WeekendInfo.Category");
					if (0 == V.IdleRPM || 0 == SHP.S.IdleRPM)
					{
						var rpm = SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.DriverInfo.DriverCarIdleRPM");
						if (null != rpm)
						{
							if (0 == SHP.S.IdleRPM)
								SHP.S.IdleRPM = Convert.ToUInt16(rpm);
							if (0 == V.IdleRPM)
								V.IdleRPM = Convert.ToUInt16(rpm);
						}
					}
					break;
				case GameId.RRRE:
						Haptics.FetchCarData(db.CarModel, null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					break;
				case GameId.BeamNG:
					Haptics.FetchCarData(db.CarId, null, V,
							SHP.S.Redline = Convert.ToUInt16(0.5 + db.MaxRpm),
							SHP.S.MaxRPM = Convert.ToUInt16((Math.Ceiling(db.MaxRpm * 0.001) - db.MaxRpm * 0.001) > 0.55
								 ? Math.Ceiling(db.MaxRpm * 0.001) * 1000.0
								 : Math.Ceiling((db.MaxRpm + 1000.0) * 0.001) * 1000.0)
						);
					if (0 == SHP.S.IdleRPM)
						SHP.S.IdleRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.idle_rpm"));
					if (0 == V.IdleRPM)
						V.IdleRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.idle_rpm"));
					break;
				case GameId.GPBikes:
				case GameId.MXBikes:
					if (SHP.S.Id != db.CarId)
					{
						SHP.S.Id = db.CarId;
						SHP.S.MaxRPM = Convert.ToUInt16(0.5 + db.MaxRpm);
						SHP.S.Redline = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.m_sEvent.m_iShiftRPM"));
					}
					Haptics.Loaded = false;		// Bikes are not saved
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					Haptics.FetchCarData(db.CarId, null, V, db.CarSettings_CurrentGearRedLineRPM, db.MaxRpm);
					SHP.S.Redline = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.MinAlertRPM"));
					SHP.S.MaxRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.MaxAlertRPM"));
					break;
				default:
					SHP.S.Redline = Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM);
					SHP.S.MaxRPM = Convert.ToUInt16(db.MaxRpm);
					break;
			}

			if (Haptics.Waiting)
			{
				return;
			}

			if (0 <= Index)
			{
				Logging.Current.Info("Haptics.SetVehicle():  " + (LoadText = $"{SHP.S.Game} {SHP.S.CarName} JSON Load Success"));
				Haptics.dljc = null;
				Haptics.dls = "";
			}
			else if (Haptics.Loaded)
			{
				SHP.S = V;  // deferred setting SHP.S, to simplify 
				Logging.Current.Info("Haptics.SetVehicle():  " + (LoadText = $"{SHP.S.Game} {SHP.S.CarName} DB Load Success"));
				V = new();
				Haptics.dljc = null;
				Haptics.dls = "";
				Haptics.Loaded = false;
			}
			Index = -2;	// for next time

			// finalize vehicle
			Gears = db.CarSettings_MaxGears > 0 ? db.CarSettings_MaxGears : 1;
			GearInterval = 1 / Gears;
			CarInitCount = 0;
			IdleSampleCount = 0;
			idleRPM = 2500;
			Gear = 0;
			SuspensionFL = 0.0;
			SuspensionFR = 0.0;
			SuspensionRL = 0.0;
			SuspensionRR = 0.0;
			SuspensionDistFLP = 0.0;
			SuspensionDistFRP = 0.0;
			SuspensionDistRLP = 0.0;
			SuspensionDistRRP = 0.0;
			SuspensionVelFLP = 0.0;
			SuspensionVelFRP = 0.0;
			SuspensionVelRLP = 0.0;
			SuspensionVelRRP = 0.0;
			SuspensionAccFLP = 0.0;
			SuspensionAccFRP = 0.0;
			SuspensionAccRLP = 0.0;
			SuspensionAccRRP = 0.0;
			Array.Clear(AccHeave, 0, AccHeave.Length);
			Array.Clear(AccSurge, 0, AccSurge.Length);
			Array.Clear(AccSway, 0, AccSway.Length);
			Acc1 = 0;
			TireDiameterSampleCount = TireDiameterSampleCount == -1 ? -1 : 0;
			TireDiameterFL = 0.0;
			TireDiameterFR = 0.0;
			TireDiameterRL = 0.0;
			TireDiameterRR = 0.0;
			RumbleLeftAvg = 0.0;
			RumbleRightAvg = 0.0;
			SetRPMIntervals();
			SetRPMMix();
		}
	}
}
