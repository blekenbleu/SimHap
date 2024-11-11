using SimHub;
using SimHub.Plugins;
using System;

namespace blekenbleu.Haptic
{
	public partial class SimData
	{
		internal void Init(BlekHapt sh)
		{
			H = sh;
			string GDBtext = H.GameDBText;
#pragma warning disable IDE0018 // Inline variable declaration
            double num;
#pragma warning restore IDE0018 // Inline variable declaration

			Index = -2;
			LockedText = Locked ? "Unlock" : "Lock";
			SuspensionMult = H.Settings.SuspensionMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SuspensionMultAll = H.Settings.SuspensionMult.TryGetValue("AllGames", out num) ? num : 1.5;
			SuspensionGamma = H.Settings.SuspensionGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SuspensionGammaAll = H.Settings.SuspensionGamma.TryGetValue("AllGames", out num) ? num : 1.75;
#if slim
			slowerR1Sway = 15;
            fasterR1Sway = 1;
            sluggishR1Sway = 0;
			impactsAccSway2S = 0;
			more = filtered = 0;
		}

		public double ThrottleIIR()
		{
			if (null == PM)	// required when called by this.AttachDelegate()
				return 0;

			double throttle = (double)PM.GetPropertyValue("DataCorePlugin.GameData.Throttle");
			filtered += (throttle - filtered) / 14;
			more += (throttle > 0.5 + filtered) ? (throttle - filtered) / 6
												: ((95 < throttle ? 1 : 0) - more) * 0.2;
			return more;
#else
			double GetSetting(string name, double trouble)  // Init() helper
	   		{
				return H.Settings.Motion.TryGetValue(name, out num) ? num : trouble;
			}

			EngineMult = H.Settings.EngineMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			EngineMultAll = H.Settings.EngineMult.TryGetValue("AllGames", out num) ? num : 1.0;
			RumbleMult = H.Settings.RumbleMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			RumbleMultAll = H.Settings.RumbleMult.TryGetValue("AllGames", out num) ? num : 5.0;
			SlipXMult = H.Settings.SlipXMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipXMultAll = H.Settings.SlipXMult.TryGetValue("AllGames", out num) ? num : 1.6;
			SlipYMult = H.Settings.SlipYMult.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipYMultAll = H.Settings.SlipYMult.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipXGamma = H.Settings.SlipXGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipXGammaAll = H.Settings.SlipXGamma.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipYGamma = H.Settings.SlipYGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SlipYGammaAll = H.Settings.SlipYGamma.TryGetValue("AllGames", out num) ? num : 1.0;
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
#endif
		}
#if slim
		public double BipolarIIR(double input, double root, double faster, double slower, double scale)
        {
            double factor = (input > root) ? faster : slower;
            root += (input - root) / factor;
            return scale * root;
        }

		public double ImpactsR1Sway()
        {
            double sway = AccSway2S;
            double factor = (sway > sluggishR1Sway) ? fasterR1Sway : slowerR1Sway;
            sluggishR1Sway += (sway - sluggishR1Sway) / factor;
            return sluggishR1Sway / 10;
        }
#endif
		// called from DataUpdate(), initially with -2 == Index
		internal void SetCar(PluginManager pluginManager)
		{
			PM = pluginManager;

			if (-2 == Index || -1 == Index)
			{
				H.S.CarId();										// SetCar() only exceptions: Forza, RRRE
				switch (BlekHapt.CurrentGame)
				{
					case GameId.AC:
					case GameId.ACC:
#if !slim
					case GameId.PC2:
					case GameId.RBR:
					case GameId.GTR2:
#endif
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						break;
#if !slim
					case GameId.AMS1:
					case GameId.RF2:
#endif
					case GameId.LMU:
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						break;
					case GameId.AMS2:
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						H.S.CarName = H.N.CarModel;
						H.S.Category = H.N.CarClass;
						break;
					case GameId.BeamNG:
						H.FetchCarData(Convert.ToUInt16(0.5 + H.N.MaxRpm),
								Convert.ToUInt16((Math.Ceiling(H.N.MaxRpm * 0.001) - H.N.MaxRpm * 0.001) > 0.55
								 	? Math.Ceiling(H.N.MaxRpm * 0.001) * 1000.0
								 	: Math.Ceiling((H.N.MaxRpm + 1000.0) * 0.001) * 1000.0),
								Convert.ToUInt16(PM.GetPropertyValue(raw+"idle_rpm"))
							);
						break;
#if !slim
					case GameId.D4:
#endif
					case GameId.DR2:
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm),
											Convert.ToUInt16(10 * Convert.ToInt32(PM.GetPropertyValue(raw+"IdleRpm"))));	// SetCar(DR2)
						break;
#if !slim
					case GameId.F12022:
					case GameId.F12023:
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm),
										 	Convert.ToUInt16(10 * Convert.ToInt32(PM.GetPropertyValue(raw+"PlayerCarStatusData.m_idleRPM"))));	// SetCar(F12023): to FetchCarData()
						break;
#endif
					case GameId.Forza:
						H.S.CarId(H.N.CarId.Substring(4));						// remove "Car_" prefix
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm),
										 	Convert.ToUInt16(PM.GetPropertyValue(raw+"EngineIdleRpm")));		// SetCar(Forza)
						break;
					case GameId.IRacing:
						var rpm = PM.GetPropertyValue(raw+"SessionData.DriverInfo.DriverCarIdleRPM");	// SetCar(IRacing)
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM),
											Convert.ToUInt16(H.N.MaxRpm), Convert.ToUInt16(rpm ?? 0));
						GameAltText = PM.GameName + (string)PM.GetPropertyValue(raw+"SessionData.WeekendInfo.Category");
						break;
#if !slim
					case GameId.GranTurismo7:
					case GameId.GranTurismoSport:
						H.FetchCarData(Convert.ToUInt16(PM.GetPropertyValue(raw+"MinAlertRPM")),
										 	Convert.ToUInt16(PM.GetPropertyValue(raw+"MaxAlertRPM")), 0);
						break;
					case GameId.RRRE:
						H.S.CarId(H.N.CarId.Split(',')[0]);		// number before comma
						H.S.CarModel(H.N.CarModel);				// try for Atlas match on CarName
						H.FetchCarData(Convert.ToUInt16(H.N.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(H.N.MaxRpm), 0);
						break;
#endif
					case GameId.WRC23:
						H.FetchCarData(Convert.ToUInt16(Math.Floor(H.N.CarSettings_CurrentGearRedLineRPM)),
											Convert.ToUInt16(H.N.MaxRpm),
										 	Convert.ToUInt16(PM.GetPropertyValue(raw+"SessionUpdate.vehicle_engine_rpm_idle")));
						break;
					default:
						H.FetchCarData(0, 0, 0);
						break;
				}
			}

			if (0 > Index)
				H.S.Defaults(H.N);	// SetCar()

			LoadText = $" {H.S.Game} " + H.S.Src;
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
			IdleSampleCount = 0;
			idleRPM = 0;							// SetCar(): reset to default value for each car
#if !slim
			SetRPMIntervals();
			TireDiameterSampleCount = TireDiameterSampleCount == -1 ? -1 : 0;
			TireDiameterFL = 0.0;
			TireDiameterFR = 0.0;
			TireDiameterRL = 0.0;
			TireDiameterRR = 0.0;
			RumbleLeftAvg = 0.0;
			RumbleRightAvg = 0.0;
#endif
			SetRPMMix();
			H.S.Set();								// NotifyPropertyChanged
			H.CarId = H.N.CarId;					// SetCar(): car change is complete
			CarInitCount = 0;
			Index = -2;	// for next time
#if BS
			switch (H.S.EngineCylinders)	// BS
			{
				case 2:
				case 4:
				case 8:
				case 16:
					BSratio = 2;
					break;
				case 3:
				case 6:
				case 12:
					BSratio = 3;
					BSratio /= 2;
					break;
				case 5:
				case 10:
					BSratio = 5;
					BSratio /= 4;
					break;
				default:
					break;
			}
			H.SC.Ratio = H.S.EngineCylinders;
#endif
		}	// SetCar()
#if BS
		/*  if you could make me a version where you change ratios so it' s
			2/4/8/16 cyl to 2:1
			3/6/12 to 3:2
			5/10 to 5:4
			and ('BlekHapt.E.Q0.[1-8]') also change for that I would appreciate it very much.
			I have an idea to stack main harmonic instead with slight freq shift and delay on each one
			to make chorus effect for more cylinders rather than doubling  or tripling freq like we currently do
			Hopefully you don't need to change code in a million places
			22 Jun 2024 BS
		 */
		internal double BSratio = 1.0; 
#endif
	}		// SimData
}
