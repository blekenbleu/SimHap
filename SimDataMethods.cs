using GameReaderCommon;
using SimHub;
using System;

namespace blekenbleu.Haptic
{

	public partial class SimData
	{
		private long FrameTimeTicks;
		private long FrameCountTicks;
		internal BlekHapt SHP;
		private ushort idleRPM;						 // for sniffing in Refresh()
		internal string raw = "DataCorePlugin.GameRawData.";

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
			idleRPM = 2500;							// default value; seems high IMO
		}

		Settings MySet;
		double GetSetting(string name, double trouble)	// Init() helper
		{
			return MySet.Motion.TryGetValue(name, out double num) ? num : trouble;
		}

		internal int Index;
		internal void Init(Settings Settings, BlekHapt sh)
		{
			Index = -2;
			MySet = Settings;
			SHP = sh;
			string GDBtext = BlekHapt.GameDBText;
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

			LockedText = Unlocked ? "Lock" : "Unlock";
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
		internal void SetVehicle(BlekHapt shp)
		{
			SHP = shp;
			StatusDataBase db = SHP.Gdat.NewData;
/*
			Logging.Current.Info($"blekHapt.SetVehicle({shp.Gdat.NewData.CarId}): " +
								(BlekHapt.Save ? " Save" : "") + (BlekHapt.Loaded ? " Loaded" : "") + (BlekHapt.Waiting ? " Waiting" : "")
								+ (BlekHapt.Set ? " Set": "") + (BlekHapt.Changed ? "Changed " : "") + $" Index = {Index}");
 */
			if (-2 == Index || -1 == Index) switch (BlekHapt.CurrentGame)
			{
				case GameId.AC:
				case GameId.ACC:
				case GameId.PC2:
				case GameId.RBR:
				case GameId.GTR2:
					BlekHapt.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm), 0);
					break;
				case GameId.AMS1:
				case GameId.LMU:
				case GameId.RF2:
					BlekHapt.FetchCarData(db.CarId, db.CarClass, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm), 0);
					break;
				case GameId.AMS2:
					BlekHapt.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm), 0);
					SHP.S.CarName = db.CarModel;
					SHP.S.Category = db.CarClass;
					break;
				case GameId.D4:
				case GameId.DR2:
					BlekHapt.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm),
										 Convert.ToUInt16(10 * Convert.ToInt32(SHP.PM.GetPropertyValue(raw+"IdleRpm"))));	// SetVehicle(DR2)
					break;
				case GameId.WRC23:
					BlekHapt.FetchCarData(db.CarId, null, Convert.ToUInt16(Math.Floor(db.CarSettings_CurrentGearRedLineRPM)), Convert.ToUInt16(db.MaxRpm),
										 Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"SessionUpdate.vehicle_engine_rpm_idle")));
					break;
				case GameId.F12022:
				case GameId.F12023:
					BlekHapt.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm),
										 Convert.ToUInt16(10 * Convert.ToInt32(SHP.PM.GetPropertyValue(raw+"PlayerCarStatusData.m_idleRPM"))));	// SetVehicle(F12023): to FetchCarData()
					break;
				case GameId.Forza:
					BlekHapt.FetchCarData(db.CarId.Substring(4), null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm),
										 Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"EngineIdleRpm")));		// SetVehicle(Forza)
					break;
				case GameId.IRacing:
					var rpm = SHP.PM.GetPropertyValue(raw+"SessionData.DriverInfo.DriverCarIdleRPM");	// SetVehicle(IRacing)
					BlekHapt.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM),
										 Convert.ToUInt16(db.MaxRpm), Convert.ToUInt16(rpm ?? 0));
					GameAltText = SHP.PM.GameName + (string)SHP.PM.GetPropertyValue(raw+"SessionData.WeekendInfo.Category");
					break;
				case GameId.RRRE:
						BlekHapt.FetchCarData(db.CarModel, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm), 0);
					break;
				case GameId.BeamNG:
					BlekHapt.FetchCarData(db.CarId, null, Convert.ToUInt16(0.5 + db.MaxRpm),
							Convert.ToUInt16((Math.Ceiling(db.MaxRpm * 0.001) - db.MaxRpm * 0.001) > 0.55
								 ? Math.Ceiling(db.MaxRpm * 0.001) * 1000.0
								 : Math.Ceiling((db.MaxRpm + 1000.0) * 0.001) * 1000.0),
							Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"idle_rpm"))
						);
					break;
				case GameId.GPBikes:
				case GameId.MXBikes:
					if (SHP.S.Id != db.CarId)	// SetVehicle() Switch case: Bikes
					{
						SHP.S.Id = db.CarId;	// SetVehicle() Switch: Bikes not in database
						SHP.S.MaxRPM = Convert.ToUInt16(0.5 + db.MaxRpm);
						SHP.S.Redline = Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"m_sEvent.m_iShiftRPM"));
					}
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					BlekHapt.FetchCarData(db.CarId, null,
										 Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"MinAlertRPM")),
										 Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"MaxAlertRPM")), 0);
					break;
				default:
					SHP.S.Redline = Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM);
					SHP.S.MaxRPM  = Convert.ToUInt16(db.MaxRpm);
					SHP.S.IdleRPM = Convert.ToUInt16(SHP.PM.GetPropertyValue("DataCorePlugin.IdleRPM"));		// SetVehicle(default game)
					break;
			}

			if (BlekHapt.Waiting)	// still hoping for online match?
			{
				Logging.Current.Info($"blekHapt.SetVehicle({db.CarId}) Waiting return: "
									+ (BlekHapt.Save ? " Save" : "") + (BlekHapt.Loaded ? " Loaded" : "")
									+ (BlekHapt.Set ? " Set": "") + (BlekHapt.Changed ? " Changed" : "") + $" Index = {Index}");
				return;				// FetchCarData() DB accesses run SetVehicle() at least twice.
			}

			if (BlekHapt.Loaded = Index == -4)					// Neither JSON nor Defaults() ?
				SHP.S.Src = "DB Load Success";
			else if(0 > Index)
				SHP.S.Defaults(db);	// SetVehicle()

			Logging.Current.Info($"blekHapt.SetVehicle({db.CarId}/{SHP.S.Id}): "
								+ (BlekHapt.Save ? " Save" : "") + (BlekHapt.Loaded ? " Loaded" : "")
								+ (BlekHapt.Set ? " Set": "") + (BlekHapt.Changed ? "Changed " : "")
								+ $" {db.CarModel}; "
								+ (LoadText = $" {SHP.S.Game} " + SHP.S.Src));

			// finalize vehicle
			Gears = db.CarSettings_MaxGears > 0 ? db.CarSettings_MaxGears : 1;
			GearInterval = 1 / Gears;
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
			RumbleLeftAvg = 0.0;
			RumbleRightAvg = 0.0;
			IdleSampleCount = 0;
			idleRPM = 2500;							// SetVehicle(): reset to default value for each car
			SHP.S.Set(db.CarId);						// set from Cache() AKA Lcars
			CarInitCount = 0;
			Index = -2;	// for next time
			Logging.Current.Info($"blekHapt.SetVehicle({db.CarId}) ending: "
									+ (BlekHapt.Save ? " Save" : "") + (BlekHapt.Loaded ? " Loaded" : "")
									+ (BlekHapt.Set ? " Set": "") + (BlekHapt.Changed ? "Changed " : "") + $" Index = {Index}");
		}	// SetVehicle()
	}
}
