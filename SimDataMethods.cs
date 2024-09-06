using GameReaderCommon;
using SimHub;
using System;

namespace sierses.Sim
{

	public partial class SimData
	{
		private long FrameTimeTicks;
		private long FrameCountTicks;
		internal Haptics SHP;
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
			SuspensionDistFLP = 0.0;
			SuspensionDistFRP = 0.0;
			SuspensionDistRLP = 0.0;
			SuspensionDistRRP = 0.0;
			AccSamples = 16;
			Acc1 = 0;
			idleRPM = 0;							// default value; seems high IMO
		}



		internal int Index;
		internal void Init(Settings Settings, Haptics sh)
		{
			Index = -2;
			SHP = sh;
			string GDBtext = Haptics.GameDBText;
			SuspensionMult = Settings.SuspensionMult.TryGetValue(GDBtext, out double num) ? num : 1.0;
			SuspensionMultAll = Settings.SuspensionMult.TryGetValue("AllGames", out num) ? num : 1.5;
			SuspensionGamma = Settings.SuspensionGamma.TryGetValue(GDBtext, out num) ? num : 1.0;
			SuspensionGammaAll = Settings.SuspensionGamma.TryGetValue("AllGames", out num) ? num : 1.75;

			LockedText = Unlocked ? "Lock" : "Unlock";
		}
			
		// called from DataUpdate()
		internal void SetVehicle(Haptics shp)
		{
			SHP = shp;
			StatusDataBase db = SHP.Gdat.NewData;
/*
			Logging.Current.Info($"Haptics.SetVehicle({shp.Gdat.NewData.CarId}): " +
								(Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "") + (Haptics.Waiting ? " Waiting" : "")
								+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? "Changed " : "") + $" Index = {Index}");
 */
			if (-2 == Index || -1 == Index) switch (Haptics.CurrentGame)
			{
				case GameId.AC:
				case GameId.ACC:
					Haptics.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm), 0);
					break;
				case GameId.LMU:
					Haptics.FetchCarData(db.CarId, db.CarClass, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm), 0);
					break;
				case GameId.AMS2:
					Haptics.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm), 0);
					SHP.S.CarName = db.CarModel;
					SHP.S.Category = db.CarClass;
					break;
				case GameId.DR2:
					Haptics.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm),
										 Convert.ToUInt16(10 * Convert.ToInt32(SHP.PM.GetPropertyValue(raw+"IdleRpm"))));	// SetVehicle(DR2)
					break;
				case GameId.WRC23:
					Haptics.FetchCarData(db.CarId, null, Convert.ToUInt16(Math.Floor(db.CarSettings_CurrentGearRedLineRPM)), Convert.ToUInt16(db.MaxRpm),
										 Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"SessionUpdate.vehicle_engine_rpm_idle")));
					break;
				case GameId.Forza:
					Haptics.FetchCarData(db.CarId.Substring(4), null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM), Convert.ToUInt16(db.MaxRpm),
										 Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"EngineIdleRpm")));		// SetVehicle(Forza)
					break;
				case GameId.IRacing:
					var rpm = SHP.PM.GetPropertyValue(raw+"SessionData.DriverInfo.DriverCarIdleRPM");	// SetVehicle(IRacing)
					Haptics.FetchCarData(db.CarId, null, Convert.ToUInt16(db.CarSettings_CurrentGearRedLineRPM),
										 Convert.ToUInt16(db.MaxRpm), Convert.ToUInt16(rpm ?? 0));
					GameAltText = SHP.PM.GameName + (string)SHP.PM.GetPropertyValue(raw+"SessionData.WeekendInfo.Category");
					break;
				case GameId.BeamNG:
					Haptics.FetchCarData(db.CarId, null, Convert.ToUInt16(0.5 + db.MaxRpm),
							Convert.ToUInt16((Math.Ceiling(db.MaxRpm * 0.001) - db.MaxRpm * 0.001) > 0.55
								 ? Math.Ceiling(db.MaxRpm * 0.001) * 1000.0
								 : Math.Ceiling((db.MaxRpm + 1000.0) * 0.001) * 1000.0),
							Convert.ToUInt16(SHP.PM.GetPropertyValue(raw+"idle_rpm"))
						);
					break;
			}

			if (Haptics.Waiting)	// still hoping for online match?
			{
				Logging.Current.Info($"Haptics.SetVehicle({db.CarId}) Waiting return: "
									+ (Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "")
									+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? " Changed" : "") + $" Index = {Index}");
				return;				// FetchCarData() DB accesses run SetVehicle() at least twice.
			}

			if (Haptics.Loaded = Index == -4)					// Neither JSON nor Defaults() ?
				SHP.S.Src = "DB Load Success";

			Logging.Current.Info($"Haptics.SetVehicle({db.CarId}/{SHP.S.Id}): "
								+ (Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "")
								+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? "Changed " : "")
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
			IdleSampleCount = 0;
			idleRPM = 0;							// SetVehicle(): reset to default value for each car
			SetRPMMix();
			SHP.S.Set(db.CarId);						// set from Cache() AKA Lcars
			CarInitCount = 0;
			Index = -2;	// for next time
			Logging.Current.Info($"Haptics.SetVehicle({db.CarId}) ending: "
									+ (Haptics.Save ? " Save" : "") + (Haptics.Loaded ? " Loaded" : "")
									+ (Haptics.Set ? " Set": "") + (Haptics.Changed ? "Changed " : "") + $" Index = {Index}");

		}	// SetVehicle()
	}
}
