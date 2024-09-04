using GameReaderCommon;
using SimHub;
using SimHub.Plugins;       // PluginManager
using System;

namespace sierses.Sim
{

	public partial class SimData
	{
		private long FrameTimeTicks;
		private long FrameCountTicks;
		internal Haptics H;
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
			TireDiameterSampleMax = 100;
			SlipXGammaBaseMult = 1.0;
			SlipYGammaBaseMult = 1.0;
			idleRPM = 2500;							// default value; seems high IMO
		}

		double GetSetting(string name, double trouble)	// Init() helper
		{
			return H.Settings.Motion.TryGetValue(name, out double num) ? num : trouble;
		}

		internal int Index;
		internal void Init(Haptics sh)
		{
			H = sh;
			Index = -2;
			string GDBtext = Haptics.GameDBText;
			EngineMult = H.Settings.EngineMult.TryGetValue(GDBtext, out double num) ? num : 1.0;
			EngineMultAll = H.Settings.EngineMult.TryGetValue("AllGames", out num) ? num : 1.0;
			RumbleMult = H.Settings.RumbleMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			RumbleMultAll = H.Settings.RumbleMult.TryGetValue("AllGames", out num) ? num : 5.0;
			SuspensionMult = H.Settings.SuspensionMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SuspensionMultAll = H.Settings.SuspensionMult.TryGetValue("AllGames", out num) ? num : 1.5;
			SuspensionGamma = H.Settings.SuspensionGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SuspensionGammaAll = H.Settings.SuspensionGamma.TryGetValue("AllGames", out num) ? num : 1.75;
			SlipXMult = H.Settings.SlipXMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipXMultAll = H.Settings.SlipXMult.TryGetValue("AllGames", out num) ? num : 1.6;
			SlipYMult = H.Settings.SlipYMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipYMultAll = H.Settings.SlipYMult.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipXGamma = H.Settings.SlipXGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipXGammaAll = H.Settings.SlipXGamma.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipYGamma = H.Settings.SlipYGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipYGammaAll = H.Settings.SlipYGamma.TryGetValue("AllGames", out num) ? num : 1.0;

			LockedText = Locked ? "Unlock" : "Lock";
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

		/*	if you could make me a version where you change ratios so it' s 
			2/4/8/16 cyl to 2:1
			3/6/12 to 3:2
			5/10 to 5:4
			and ('Haptics.E.Q0.[1-8]') also change for that I would appreciate it very much.
  			I have an idea to stack main harmonic instead with slight freq shift and delay on each one
 			to make chorus effect for more cylinders rather than doubling  or tripling freq like we currently do 
			Hopefully you don't need to change code in a million places
			22 Jun 2024 BS
		 */
		internal double BS = 1.0;

		// called from DataUpdate(), initially with -2 == Index
		internal void SetCar(Haptics shp, PluginManager pluginManager)
		{
			H = shp;
			PM = pluginManager;
/*
			Logging.Current.Info($"Haptics.SetCar({shp.Gdat.NewData.CarId}): " +
								(Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "") + (Haptics.Waiting ? " Waiting" : "")
								+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? "Changed " : "") + $" Index = {Index}");
 */
			if (-2 == Index || -1 == Index)
			{
				H.S.CarId(H.N.CarId);										// only 2 exceptions: RRRE and Forza
				switch (Haptics.CurrentGame)
				{
					case GameId.AC:
					case GameId.ACC:
					case GameId.PC2:
					case GameId.RBR:
					case GameId.GTR2:
						Haptics.FetchCarData(null, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						break;
					case GameId.AMS1:
					case GameId.LMU:
					case GameId.RF2:
						Haptics.FetchCarData(H.N.CarClass, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						break;
					case GameId.AMS2:
						Haptics.FetchCarData(null, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						H.S.CarName = H.N.CarModel;
						H.S.Category = H.N.CarClass;
						break;
					case GameId.D4:
					case GameId.DR2:
						Haptics.FetchCarData(null, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm),
										 	Convert.ToUInt16(10 * Convert.ToInt32(PM.GetPropertyValue(raw+"IdleRpm"))));	// SetCar(DR2)
						break;
					case GameId.WRC23:
						Haptics.FetchCarData(null, Convert.ToUInt16(Math.Floor(H.N.CarSettings_CurrentGearRedLineRPM)), Convert.ToUInt16(H.N.MaxRpm),
										 	Convert.ToUInt16(PM.GetPropertyValue(raw+"SessionUpdate.vehicle_engine_rpm_idle")));
						break;
					case GameId.F12022:
					case GameId.F12023:
						Haptics.FetchCarData(null, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm),
										 	Convert.ToUInt16(10 * Convert.ToInt32(PM.GetPropertyValue(raw+"PlayerCarStatusData.m_idleRPM"))));	// SetCar(F12023): to FetchCarData()
						break;
					case GameId.Forza:
						H.S.CarId(H.N.CarId.Substring(4));						// remove "Car_" prefix
						Haptics.FetchCarData(null, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm),
										 	Convert.ToUInt16(PM.GetPropertyValue(raw+"EngineIdleRpm")));		// SetCar(Forza)
						break;
					case GameId.IRacing:
						var rpm = PM.GetPropertyValue(raw+"SessionData.DriverInfo.DriverCarIdleRPM");	// SetCar(IRacing)
						Haptics.FetchCarData(null, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM),
										 	Convert.ToUInt16(H.N.MaxRpm), Convert.ToUInt16(rpm ?? 0));
						GameAltText = PM.GameName + (string)PM.GetPropertyValue(raw+"SessionData.WeekendInfo.Category");
						break;
					case GameId.RRRE:
						H.S.CarId(H.N.CarId.Split(',')[0]);		// number before comma
						H.S.CarModel(H.N.CarModel);				// try for Atlas match on CarName
						Haptics.FetchCarData(null, Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						break;
					case GameId.BeamNG:
						Haptics.FetchCarData(null, Convert.ToUInt16(0.5 + H.N.MaxRpm),
								Convert.ToUInt16((Math.Ceiling(H.N.MaxRpm * 0.001) - H.N.MaxRpm * 0.001) > 0.55
								 	? Math.Ceiling(H.N.MaxRpm * 0.001) * 1000.0
								 	: Math.Ceiling((H.N.MaxRpm + 1000.0) * 0.001) * 1000.0),
								Convert.ToUInt16(PM.GetPropertyValue(raw+"idle_rpm"))
							);
						break;
					case GameId.GranTurismo7:
					case GameId.GranTurismoSport:
						Haptics.FetchCarData(null,
										 	Convert.ToUInt16(PM.GetPropertyValue(raw+"MinAlertRPM")),
										 	Convert.ToUInt16(PM.GetPropertyValue(raw+"MaxAlertRPM")), 0);
						break;
					default:
						H.S.Redline = Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM);
						H.S.MaxRPM  = Convert.ToUInt16(H.N.MaxRpm);
						H.S.IdleRPM = Convert.ToUInt16(PM.GetPropertyValue("DataCorePlugin.IdleRPM"));		// SetCar(default game)
						break;
				}
			}

			if (Haptics.Waiting)	// still hoping for online match?
			{
				Logging.Current.Info($"Haptics.SetCar({H.N.CarId}) Waiting return: "
									+ (Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "")
									+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? " Changed" : "") + $" Index = {Index}");
				return;				// FetchCarData() DB accesses run SetCar() at least twice.
			}

			if (Haptics.Loaded = Index == -4)					// Neither JSON nor Defaults() ?
				H.S.Src = "DB Load Success";
			else if (0 > Index)
				H.S.Defaults(H.N);	// SetCar()

			Logging.Current.Info($"Haptics.SetCar({H.N.CarId}/{H.S.Id}): "
								+ (Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "")
								+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? "Changed " : "")
								+ $" {H.N.CarModel}; "
								+ (LoadText = $" {H.S.Game} " + H.S.Src));

			// finalize car
			Gears = H.N.CarSettings_MaxGears > 0 ? H.N.CarSettings_MaxGears : 1;
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
			TireDiameterSampleCount = TireDiameterSampleCount == -1 ? -1 : 0;
			TireDiameterFL = 0.0;
			TireDiameterFR = 0.0;
			TireDiameterRL = 0.0;
			TireDiameterRR = 0.0;
			RumbleLeftAvg = 0.0;
			RumbleRightAvg = 0.0;
			IdleSampleCount = 0;
			idleRPM = 2500;							// SetCar(): reset to default value for each car
			SetRPMIntervals();
			SetRPMMix();
			H.S.Set();								// NotifyPropertyChanged
			H.CarId = H.N.CarId;					// SetCar(): car change is complete
			CarInitCount = 0;
			Index = -2;	// for next time
			Logging.Current.Info($"Haptics.SetCar({H.N.CarId}) ending: "
									+ (Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "")
									+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? "Changed " : "") + $" Index = {Index}");

			switch (H.S.EngineCylinders)	// BS
			{
				case 2:
				case 4:
				case 8:
				case 16:
					BS = 2;
					break;
				case 3:
				case 6:
				case 12:
					BS = 3;
					BS /= 2;
					break;
				case 5:
				case 10:
					BS = 5;
					BS /= 4;
					break;
				default:
					break;
			}
			H.SC.Ratio = H.S.EngineCylinders;
		}	// SetCar()
	}
}
