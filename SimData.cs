using SimHub.Plugins;
using System;				// for Math

namespace sierses.Sim
{
	public partial class SimData
	{
		static PluginManager PM;

		public double FPS;
		public double InvMaxRPM;
		public double InvSpeedKmh;
		public double InvMaxSpeedKmh;
		public double SpeedMs;
		public double InvSpeedMs;
		public double Accelerator;
		public double Brake;
		public double BrakeF;
		public double BrakeR;
		public double Clutch;
		public double Handbrake;
		public double BrakeBias;
		public double BrakeVel;
		public double BrakeVelP;
		public double BrakeAcc;
		public int Gear;
		public int Gears;
		public bool Downshift;
		public bool Upshift;
		public int GearPrevious;
		public double GearInterval;
		public long ShiftTicks;
		public double CylinderDisplacement;
		public int WiperStatus;
		public int CarInitCount;
		public int IdleSampleCount;			// used in Runtime()
		public double IdlePercent;
		public double RedlinePercent;
		public double RPMPercent;
		public double MixCylinder;
		public double MixDisplacement;
		public double MixPower;
		public double MixTorque;
		public double MixFront;
		public double MixMiddle;
		public double MixRear;
		public double PeakA1Start;
		public double PeakA2Start;
		public double PeakB1Start;
		public double PeakB2Start;
		public double PeakA1Modifier;
		public double PeakA2Modifier;
		public double PeakB1Modifier;
		public double PeakB2Modifier;
		public double FreqHarmonic;
		public double FreqOctave;
		public double FreqLFEAdaptive;
		public double PeakA1CylMod;
		public double PeakA2CylMod;
		public double PeakB1CylMod;
		public double PeakB2CylMod;
		public double FrequencyMultiplier;
		public double LFEeq;
		public double LFEhpScale;
		public double peakEQeng;
		public double peakGearMulti;
		public double peakEQ;
		public double rpmMainEQ;
		public double rpmMainSum;
		public double rpmMain;
		public double rpmPeakA2RearSum;
		public double rpmPeakA2Rear;
		public double rpmPeakB1RearSum;
		public double rpmPeakB1Rear;
		public double rpmPeakA1RearSum;
		public double rpmPeakA1Rear;
		public double rpmPeakB2RearSum;
		public double rpmPeakB2Rear;

		public double rpmPeakA2FrontSum;
		public double rpmPeakA2Front;
		public double rpmPeakB1FrontSum;
		public double rpmPeakB1Front;
		public double rpmPeakA1FrontSum;
		public double rpmPeakA1Front;
		public double rpmPeakB2FrontSum;
		public double rpmPeakB2Front;

		public double FreqPeakA1;
		public double FreqPeakA2;
		public double FreqPeakB1;
		public double FreqPeakB2;
		public double Gain1H;
		public double GainPeakA1;
		public double GainPeakA1Front;
		public double GainPeakA1Rear;
		public double GainPeakA2;
		public double GainPeakA2Front;
		public double GainPeakA2Rear;
		public double GainPeakB1;
		public double GainPeakB1Front;
		public double GainPeakB1Rear;
		public double GainPeakB2;
		public double GainPeakB2Front;
		public double GainPeakB2Rear;
		public int AccSamples;
		public int Acc0;
		public int Acc1;
		public double[] AccHeave;
		public double[] AccSurge;
		public double[] AccSway;
		public double AccHeave2S;
		public double AccSurge2S;
		public double AccSway2S;
		public double WorldSpeedY;
		public double JerkZ;
		public double JerkY;
		public double JerkX;
		public double AccHeaveAvg;
		public double AccSurgeAvg;
		public double AccSwayAvg;
		public double JerkYAvg;
		public double AccHeaveAbs;
		public double InvAccSurgeAvg;
		public bool Airborne;
		public double VelocityX;
		public double WheelLoadFL;
		public double WheelLoadFR;
		public double WheelLoadRL;
		public double WheelLoadRR;
		public double Yaw;
		public double YawPrev;
		public double YawRate;
		public double YawRateAvg;
		public double SuspensionDistFL;
		public double SuspensionDistFR;
		public double SuspensionDistRL;
		public double SuspensionDistRR;
		public double SuspensionDistFLP;
		public double SuspensionDistFRP;
		public double SuspensionDistRLP;
		public double SuspensionDistRRP;
		public double SuspensionVelFL;
		public double SuspensionVelFR;
		public double SuspensionVelRL;
		public double SuspensionVelRR;
		public double SuspensionVelFLP;
		public double SuspensionVelFRP;
		public double SuspensionVelRLP;
		public double SuspensionVelRRP;
		public double SuspensionAccFL;
		public double SuspensionAccFR;
		public double SuspensionAccRL;
		public double SuspensionAccRR;
		public double SuspensionAccFLP;
		public double SuspensionAccFRP;
		public double SuspensionAccRLP;
		public double SuspensionAccRRP;
		public double SuspensionJerkFL;
		public double SuspensionJerkFR;
		public double SuspensionJerkRL;
		public double SuspensionJerkRR;
		public double SuspensionFL;
		public double SuspensionFR;
		public double SuspensionRL;
		public double SuspensionRR;
		public double SuspensionFront;
		public double SuspensionRear;
		public double SuspensionLeft;
		public double SuspensionRight;
		public double SuspensionAll;
		public double SuspensionAccAll;
		public double TiresLeft;
		public double TiresRight;
		public int SurfaceMaterial;
		public double VelocityZAvg;
		public double GainSuspensionFront;
		public double GainSuspensionRear;
		public double GainSuspensionLeft;
		public double GainSuspensionRight;
		public double GainSuspensionAll;
		public int SuspensionMainOutput;
		public double SuspensionFreq;
		public double SuspensionFreqR1;
		public double SuspensionFreqR2;
		public double SuspensionFreqR3;
		public double SuspensionMultR1;
		public double SuspensionMultR2;
		public double SuspensionMultR3;
		private long FrameTimeTicks;
		private long FrameCountTicks;
		internal Haptics H;
		internal int Index;
		internal string raw = "DataCorePlugin.GameRawData.";
		private ushort idleRPM;
		private double slowerR1Sway;
		private double fasterR1Sway;
		private double sluggishR1Sway;

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
			idleRPM = 0;
		}

		private void SetRPMMix()
		{
			InvMaxRPM = H.S.MaxRPM > 0.0 ? 1.0 / H.S.MaxRPM : 0.0001;
			IdlePercent = H.S.IdleRPM * InvMaxRPM;
			RedlinePercent = H.S.Redline * InvMaxRPM;
			if (H.S.Displacement > 0.0 && 0 < H.S.EngineCylinders)
			{
				CylinderDisplacement = H.S.Displacement / H.S.EngineCylinders;
				MixCylinder = 1.0 - Math.Max(2000.0 - CylinderDisplacement, 0.0)
									 * Math.Max(2000.0 - CylinderDisplacement, 0.0) * 2.5E-07;
				MixDisplacement = 1.0 - Math.Max(10000.0 - H.S.Displacement, 0.0)
										 * Math.Max(10000.0 - H.S.Displacement, 0.0) * 1E-08;
			}
			else
			{
				MixCylinder = 0.0;
				MixDisplacement = 0.0;
			}
			MixPower = 1.0 - Math.Max(2000.0 - (H.S.MaxPower - H.S.ElectricMaxPower), 0.0)
							 * Math.Max(2000.0 - (H.S.MaxPower - H.S.ElectricMaxPower), 0.0) * 2.5E-07;
			MixTorque = 1.0 - Math.Max(2000.0 - H.S.MaxTorque, 0.0) * Math.Max(2000.0 - H.S.MaxTorque, 0.0) * 2.5E-07;
			MixFront = !(H.S.EngineLocation == "F")
						 ? (!(H.S.EngineLocation == "FM")
							 ? (!(H.S.EngineLocation == "M")
								 ? (!(H.S.EngineLocation == "RM")
									 ? (!(H.S.PoweredWheels == "F")
										 ? (!(H.S.PoweredWheels == "R")
											 ? 0.2
											 : 0.1)
										 : 0.3)
									 : (!(H.S.PoweredWheels == "F")
									 ? (!(H.S.PoweredWheels == "R")
										 ? 0.3
										 : 0.2)
									 : 0.4)
								   )
								 : (!(H.S.PoweredWheels == "F")
								 	? (!(H.S.PoweredWheels == "R")
									 	? 0.5
									 	: 0.4
									  )
									 : 0.6
								   )
							   )
						 : (!(H.S.PoweredWheels == "F")
							 ? (!(H.S.PoweredWheels == "R")
								 ? 0.7
								 : 0.6
							   )
							 : 0.8
						   )
						)
					 : (!(H.S.PoweredWheels == "F")
					 	? (!(H.S.PoweredWheels == "R")
							 ? 0.8
					 	 	: 0.7
							)
					 	: 0.9
					   );
			MixMiddle = Math.Abs(MixFront - 0.5) * 2.0;
			MixRear = 1.0 - MixFront;
		}

		private float Physics(string prop)
		{
			var foo = PM.GetPropertyValue(raw+"Physics."+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private float Raw(string prop)
		{
			var foo = PM.GetPropertyValue(raw+""+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private void UpdateVehicle()
		{
			SuspensionDistFLP = SuspensionDistFL;
			SuspensionDistFRP = SuspensionDistFR;
			SuspensionDistRLP = SuspensionDistRL;
			SuspensionDistRRP = SuspensionDistRR;
			SuspensionDistFL = 0.0;
			SuspensionDistFR = 0.0;
			SuspensionDistRL = 0.0;
			SuspensionDistRR = 0.0;
			SuspensionVelFLP = SuspensionVelFL;
			SuspensionVelFRP = SuspensionVelFR;
			SuspensionVelRLP = SuspensionVelRL;
			SuspensionVelRRP = SuspensionVelRR;
			SuspensionVelFL = 0.0;
			SuspensionVelFR = 0.0;
			SuspensionVelRL = 0.0;
			SuspensionVelRR = 0.0;
			bool flag = true;
			switch (Haptics.CurrentGame)
			{
				case GameId.AC:
					SuspensionDistFL = Physics("SuspensionTravel01");
					SuspensionDistFR = Physics("SuspensionTravel02");
					SuspensionDistRL = Physics("SuspensionTravel03");
					SuspensionDistRR = Physics("SuspensionTravel04");
					TiresLeft = 1.0 + (double) Math.Max(Physics("TyreContactHeading01.Y"), Physics("TyreContactHeading03.Y"));
					TiresRight = 1.0 + (double) Math.Max(Physics("TyreContactHeading02.Y"), Physics("TyreContactHeading04.Y"));
					break;
				case GameId.ACC:
					SuspensionDistFL = Physics("SuspensionTravel01");
					SuspensionDistFR = Physics("SuspensionTravel02");
					SuspensionDistRL = Physics("SuspensionTravel03");
					SuspensionDistRR = Physics("SuspensionTravel04");
					WiperStatus = (int) PM.GetPropertyValue(raw+"Graphics.WiperLV");
					break;
				case GameId.AMS2:
					SuspensionDistFL = Raw("mSuspensionTravel01");
					SuspensionDistFR = Raw("mSuspensionTravel02");
					SuspensionDistRL = Raw("mSuspensionTravel03");
					SuspensionDistRR = Raw("mSuspensionTravel04");
					break;
				case GameId.DR2:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
					VelocityX = Raw("WorldSpeedX") * Math.Sin(Raw("XR"));
					YawRate = H.N.OrientationYawAcceleration;
					break;
				case GameId.WRC23:
					SuspensionDistFL = Raw("SessionUpdate.vehicle_hub_position_fl");
					SuspensionDistFR = Raw("SessionUpdate.vehicle_hub_position_fr");
					SuspensionDistRL = Raw("SessionUpdate.vehicle_hub_position_bl");
					SuspensionDistRR = Raw("SessionUpdate.vehicle_hub_position_br");
					SpeedMs = Raw("SessionUpdate.vehicle_speed");
					InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
					VelocityX = Raw("SessionUpdateLocalVelocity.X");
					YawRate = H.N.OrientationYawAcceleration;
					break;
				case GameId.Forza:
					SuspensionDistFL = Raw("SuspensionTravelMetersFrontLeft");
					SuspensionDistFR = Raw("SuspensionTravelMetersFrontRight");
					SuspensionDistRL = Raw("SuspensionTravelMetersRearLeft");
					SuspensionDistRR = Raw("SuspensionTravelMetersRearRight");
					break;
				case GameId.IRacing:
					if (PM.GetPropertyValue(raw+"Telemetry.LFshockDefl") != null)
					{
						SuspensionDistFL = Raw("Telemetry.LFshockDefl");
						SuspensionDistFR = Raw("Telemetry.RFshockDefl");
					}
					else if (PM.GetPropertyValue(raw+"Telemetry.LFSHshockDefl") != null)
					{
						SuspensionDistFL = Raw("Telemetry.LFSHshockDefl");
						SuspensionDistFR = Raw("Telemetry.RFSHshockDefl");
					}
					if (PM.GetPropertyValue(raw+"Telemetry.LRshockDefl") != null)
					{
						SuspensionDistRL = Raw("Telemetry.LRshockDefl");
						SuspensionDistRR = Raw("Telemetry.RRshockDefl");
					}
					else if (PM.GetPropertyValue(raw+"Telemetry.LRSHshockDefl") != null)
					{
						SuspensionDistRL = Raw("Telemetry.LRSHshockDefl");
						SuspensionDistRR = Raw("Telemetry.RRSHshockDefl");
					}
					if (PM.GetPropertyValue(raw+"Telemetry.CFshockDefl") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Telemetry.CFshockDefl");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Telemetry.CFshockDefl");
					}
					else if (PM.GetPropertyValue(raw+"Telemetry.HFshockDefl") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Telemetry.HFshockDefl");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Telemetry.HFshockDefl");
					}
					if (PM.GetPropertyValue(raw+"Telemetry.CRshockDefl") != null)
					{
						SuspensionDistRL = 0.5 * SuspensionDistRL + Raw("Telemetry.CRshockDefl");
						SuspensionDistRR = 0.5 * SuspensionDistRR + Raw("Telemetry.CRshockDefl");
					}
					break;
				case GameId.BeamNG:
					flag = false;
					SuspensionDistFL = Raw("suspension_position_fl") * 0.05;
					SuspensionDistFR = Raw("suspension_position_fr") * 0.05;
					SuspensionDistRL = Raw("suspension_position_rl") * 0.05;
					SuspensionDistRR = Raw("suspension_position_rr") * 0.05;
					SuspensionVelFL = Raw("suspension_velocity_fl") * 0.05;
					SuspensionVelFR = Raw("suspension_velocity_fr") * 0.05;
					SuspensionVelRL = Raw("suspension_velocity_rl") * 0.05;
					SuspensionVelRR = Raw("suspension_velocity_rr") * 0.05;
					break;
				case GameId.LMU:
					SuspensionDistFL = Raw("CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					SuspensionDistFR = Raw("CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					SuspensionDistRL = Raw("CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					SuspensionDistRR = Raw("CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
					break;
			}
			if (!flag)
				return;
			SuspensionVelFL = (SuspensionDistFL - SuspensionDistFLP) * FPS;
			SuspensionVelFR = (SuspensionDistFR - SuspensionDistFRP) * FPS;
			SuspensionVelRL = (SuspensionDistRL - SuspensionDistRLP) * FPS;
			SuspensionVelRR = (SuspensionDistRR - SuspensionDistRRP) * FPS;
		}


		internal ushort Rpms;

		// called from DataUpdate()
		internal void Runtime(Haptics shp, PluginManager pluginManager)
		{
			H = shp;
			PM = pluginManager;
			FPS = (double) PM.GetPropertyValue("DataCorePlugin.DataUpdateFps");
			Rpms = Convert.ToUInt16(0.5 + H.N.Rpms);
			SpeedMs = H.N.SpeedKmh * 0.277778;
			Accelerator = H.N.Throttle;
			Clutch = H.N.Clutch;
			Handbrake = H.N.Handbrake;
			RPMPercent = H.N.Rpms * InvMaxRPM;
			InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
			Brake = H.N.Brake;
			BrakeBias = H.N.BrakeBias;
			BrakeF = Brake * (2.0 * BrakeBias) * 0.01;
			BrakeR = Brake * (200.0 - 2.0 * BrakeBias) * 0.01;
			BrakeVelP = BrakeVel;
			BrakeVel = (Brake - H.Gdat.OldData.Brake) * FPS;
			BrakeAcc = (BrakeVel - BrakeVelP) * FPS;
			if (CarInitCount < 2)
			{
				SuspensionDistFLP = SuspensionDistFL;
				SuspensionDistFRP = SuspensionDistFR;
				SuspensionDistRLP = SuspensionDistRL;
				SuspensionDistRRP = SuspensionDistRR;
				YawPrev = H.N.OrientationYaw;
				Yaw = H.N.OrientationYaw;
			}
			YawPrev = Yaw;
			Yaw = -H.N.OrientationYaw;
			if (Yaw > 100.0 && YawPrev < -100.0)
				YawPrev += 360.0;
			else if (Yaw < -100.0 && YawPrev > 100.0)
				YawPrev -= 360.0;
			YawRate = (Yaw - YawPrev) * FPS;
			if (YawRateAvg != 0.0)
			{
				if (Math.Abs(YawRate) < Math.Abs(YawRateAvg * 1.25))
					YawRateAvg = (YawRateAvg + YawRate) * 0.5;
				else
					YawRateAvg *= 1.25;
			}
			else YawRateAvg = YawRate;
			++Acc0;
			Acc1 = Acc0 - 1;
			if (Acc0 >= AccSamples)
			{
				Acc0 = 0;
				Acc1 = AccSamples - 1;
			}
			AccHeave[Acc0] = H.N.AccelerationHeave.GetValueOrDefault();
			AccSurge[Acc0] = H.N.AccelerationSurge.GetValueOrDefault();
			AccSway[Acc0] = H.N.AccelerationSway.GetValueOrDefault();
			if (!H.N.AccelerationHeave.HasValue)
			{
				AccHeave[Acc0] = Raw("WorldSpeedY");
				AccHeave[Acc0] = (AccHeave[Acc0] - WorldSpeedY) * FPS;
				WorldSpeedY = Raw("WorldSpeedY");
			}
			AccHeave2S = (AccHeave[Acc0] + AccHeave[Acc1]) * 0.5;
			AccSurge2S = (AccSurge[Acc0] + AccSurge[Acc1]) * 0.5;
			AccSway2S = (AccSway[Acc0] + AccSway[Acc1]) * 0.5;
			JerkZ = (AccHeave[Acc0] - AccHeave[Acc1]) * FPS;
			JerkY = (AccSurge[Acc0] - AccSurge[Acc1]) * FPS;
			JerkX = (AccSway[Acc0] - AccSway[Acc1]) * FPS;
			double num1 = 1.0 / 16.0;
			double accSurgeAvg = AccSurgeAvg;
			AccHeaveAvg = 0.0;
			AccSurgeAvg = 0.0;
			AccSwayAvg = 0.0;
			for (int index = 0; index < AccSamples; ++index)
			{
				AccHeaveAvg += AccHeave[index];
				AccSurgeAvg += AccSurge[index];
				AccSwayAvg += AccSway[index];
			}
			AccHeaveAvg *= num1;
			AccSurgeAvg *= num1;
			AccSwayAvg *= num1;
			JerkYAvg = (AccSurgeAvg - accSurgeAvg) * FPS;
			AccHeaveAbs = Math.Abs(AccHeave[Acc0]);
			InvAccSurgeAvg = AccSurgeAvg != 0.0 ? 1.0 / AccSurgeAvg : 0.0;
			WheelLoadFL = ((100.0 + AccSurge[Acc0]) * (100.0 - AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadFR = ((100.0 + AccSurge[Acc0]) * (100.0 + AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadRL = ((100.0 - AccSurge[Acc0]) * (100.0 - AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadRR = ((100.0 - AccSurge[Acc0]) * (100.0 + AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			UpdateVehicle();
			Airborne = AccHeave2S < -2.0 || Math.Abs(H.N.OrientationRoll) > 60.0;

			if (DateTime.Now.Ticks < FrameTimeTicks)	// long rollover?
				FrameCountTicks += (long.MaxValue - FrameTimeTicks) + DateTime.Now.Ticks;	// rollover
			else FrameCountTicks += DateTime.Now.Ticks - FrameTimeTicks;
			FrameTimeTicks = DateTime.Now.Ticks;
			if (FrameCountTicks > 864000000000L)
				FrameCountTicks = 0L;

			if (DateTime.Now.Ticks < ShiftTicks)	// long rollover?
				ShiftTicks = - (DateTime.Now.Ticks + (long.MaxValue - ShiftTicks));
			if (DateTime.Now.Ticks - ShiftTicks > H.Settings.DownshiftDurationMs * 10000)
				Downshift = false;
			if (DateTime.Now.Ticks - ShiftTicks > H.Settings.UpshiftDurationMs * 10000)
				Upshift = false;
			DateTime now;
			if (H.Gdat.OldData.Gear != H.N.Gear)
			{
				if (Gear != 0)
					GearPrevious = Gear;
				Gear = !(H.N.Gear == "N")
						? (!(H.N.Gear == "R")
							? Convert.ToInt32(H.N.Gear)
							: -1)
						: 0;
				if (Gear != 0 && Gear != GearPrevious)
				{
					now = DateTime.Now;
					ShiftTicks = now.Ticks;
					if (Gear < GearPrevious)
						Downshift = true;
					else Upshift = true;
				}
			}
			
			SuspensionAccFLP = SuspensionAccFL;
			SuspensionAccFRP = SuspensionAccFR;
			SuspensionAccRLP = SuspensionAccRL;
			SuspensionAccRRP = SuspensionAccRR;
			SuspensionAccFL = (SuspensionVelFL - SuspensionVelFLP) * FPS;
			SuspensionAccFR = (SuspensionVelFR - SuspensionVelFRP) * FPS;
			SuspensionAccRL = (SuspensionVelRL - SuspensionVelRLP) * FPS;
			SuspensionAccRR = (SuspensionVelRR - SuspensionVelRRP) * FPS;
			SuspensionFL = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelFL, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionFR = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelFR, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionRL = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelRL, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionRR = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelRR, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionFront = SuspensionFL + SuspensionFR;
			SuspensionRear = SuspensionRL + SuspensionRR;
			SuspensionLeft = SuspensionFL + SuspensionRL;
			SuspensionRight = SuspensionFR + SuspensionRR;
			SuspensionAll = (SuspensionFL + SuspensionFR + SuspensionRL + SuspensionRR) * 0.5;
			SuspensionAccAll = (SuspensionAccFL + SuspensionAccFR + SuspensionAccRL + SuspensionAccRR) * 0.5;
			if (CarInitCount < 10 && FrameCountTicks % 2000000L <= 150000L)
			{
				SuspensionFL *= 0.1 * CarInitCount;
				SuspensionFR *= 0.1 * CarInitCount;
				SuspensionRL *= 0.1 * CarInitCount;
				SuspensionRR *= 0.1 * CarInitCount;
				++CarInitCount;
			}
			SuspensionFreq = H.N.SpeedKmh * (3.0 / 16.0);
			double num5 = 46.0 + 0.55 * SpeedMs;
			double num6 = 34.0 + 0.6 * SpeedMs;
			double num7 = 24.0 + 0.65 * SpeedMs;
			double num13 = 0.4 + 2.4 * AccHeaveAbs * (AccHeaveAbs + num5) / (num5 * num5);
			double num14 = 0.5 + 2.0 * AccHeaveAbs * (AccHeaveAbs + num6) / (num6 * num6);
			double num15 = 0.6 + 1.6 * AccHeaveAbs * (AccHeaveAbs + num7) / (num7 * num7);
			if (SuspensionFreq < 30.0)
			{
				if (SuspensionFreq < 20.0)
				{
					if (SuspensionFreq < 15.0)
					{
						if (SuspensionFreq < 10.0)
						{
							if (SuspensionFreq < 7.5)
							{
								if (SuspensionFreq < 3.75)
								{
									SuspensionFreq *= 4.0;
									SuspensionFreqR1 = SuspensionFreq * 2.0;
									SuspensionFreqR2 = SuspensionFreq * 2.86;
									SuspensionFreqR3 = SuspensionFreq * 4.0;
									SuspensionMultR1 = num13 * 0.8;
									SuspensionMultR2 = num14 * 0.25;
									SuspensionMultR3 = num15 * 0.6;
								}
								else
								{
									SuspensionFreq *= 2.0;
									SuspensionFreqR1 = SuspensionFreq * 2.0;
									SuspensionFreqR2 = SuspensionFreq * 2.86;
									SuspensionFreqR3 = SuspensionFreq * 4.0;
									SuspensionMultR1 = num13 * 0.8;
									SuspensionMultR2 = num14 * 0.25;
									SuspensionMultR3 = num15 * 0.6;
								}
							}
							else
							{
								SuspensionFreqR1 = SuspensionFreq * 2.86;
								SuspensionFreqR2 = SuspensionFreq * 4.0;
								SuspensionFreqR3 = SuspensionFreq * 5.72;
								SuspensionMultR1 = num13 * 0.25;
								SuspensionMultR2 = num14 * 0.6;
								SuspensionMultR3 = num15 * 0.125;
							}
						}
						else
						{
							SuspensionFreqR1 = SuspensionFreq * 2.0;
							SuspensionFreqR2 = SuspensionFreq * 2.86;
							SuspensionFreqR3 = SuspensionFreq * 4.0;
							SuspensionMultR1 = num13 * 0.8;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.6;
						}
					}
					else
					{
						SuspensionFreqR1 = SuspensionFreq * 1.43;
						SuspensionFreqR2 = SuspensionFreq * 2.0;
						SuspensionFreqR3 = SuspensionFreq * 2.86;
						SuspensionMultR1 = num13 * 0.5;
						SuspensionMultR2 = num14 * 0.8;
						SuspensionMultR3 = num15 * 0.25;
					}
				}
				else
				{
					SuspensionFreqR1 = SuspensionFreq * 1.0;
					SuspensionFreqR2 = SuspensionFreq * 1.43;
					SuspensionFreqR3 = SuspensionFreq * 2.0;
					SuspensionMultR1 = num13 * 1.0;
					SuspensionMultR2 = num14 * 0.5;
					SuspensionMultR3 = num15 * 0.8;
				}
			}
			else if (SuspensionFreq > 40.0)
			{
				if (SuspensionFreq > 60.0)
				{
					if (SuspensionFreq > 80.0)
					{
						if (SuspensionFreq > 120.0)
						{
							SuspensionFreqR1 = SuspensionFreq * (143.0 / 800.0);
							SuspensionFreqR2 = SuspensionFreq * 0.25;
							SuspensionFreqR3 = SuspensionFreq * (143.0 / 400.0);
							SuspensionMultR1 = num13 * 0.125;
							SuspensionMultR2 = num14 * 0.6;
							SuspensionMultR3 = num15 * 0.25;
						}
						else
						{
							SuspensionFreqR1 = SuspensionFreq * 0.25;
							SuspensionFreqR2 = SuspensionFreq * (143.0 / 400.0);
							SuspensionFreqR3 = SuspensionFreq * 0.5;
							SuspensionMultR1 = num13 * 0.6;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.8;
						}
					}
					else
					{
						SuspensionFreqR1 = SuspensionFreq * (143.0 / 400.0);
						SuspensionFreqR2 = SuspensionFreq * 0.5;
						SuspensionFreqR3 = SuspensionFreq * 0.715;
						SuspensionMultR1 = num13 * 0.25;
						SuspensionMultR2 = num14 * 0.8;
						SuspensionMultR3 = num15 * 0.5;
					}
				}
				else
				{
					SuspensionFreqR1 = SuspensionFreq * 0.5;
					SuspensionFreqR2 = SuspensionFreq * 0.715;
					SuspensionFreqR3 = SuspensionFreq * 1.0;
					SuspensionMultR1 = num13 * 0.8;
					SuspensionMultR2 = num14 * 0.5;
					SuspensionMultR3 = num15 * 1.0;
				}
			}
			else
			{
				SuspensionFreqR1 = SuspensionFreq * 0.715;
				SuspensionFreqR2 = SuspensionFreq * 1.0;
				SuspensionFreqR3 = SuspensionFreq * 1.43;
				SuspensionMultR1 = num13 * 0.5;
				SuspensionMultR2 = num14 * 1.0;
				SuspensionMultR3 = num15 * 0.5;
			}
			if (IdleSampleCount < 0) /*&& FrameCountTicks % 2500000L <= 150000L*/	// Runtime() sniff: ignore FrameCountTicks .. for now
				if (H.N.Rpms > 300 && H.N.Rpms <= idleRPM * 1.1) // Runtime(): supposes that idleRPM is somewhat valid..??
			{
				double num19 = Math.Abs(H.Gdat.OldData.Rpms - H.N.Rpms) * FPS;

				if (num19 < 40.0)
				{
					idleRPM = Convert.ToUInt16((1 + idleRPM + (int)H.N.Rpms) >> 1); // Runtime(): averaging with previous average
					++IdleSampleCount;								// Runtime(): increment if difference < 40
				}
				if (20 == IdleSampleCount && 0 == H.S.IdleRPM)	// Runtime(): change H.S.IdleRPM?
					H.S.Idle(idleRPM);							// Runtime() sniff: only if it was 0
			}

			if (FrameCountTicks % 5000000L <= 150000L)
			{
				SetRPMMix();
			}

			FreqHarmonic = H.N.Rpms * 0.008333333;
			FreqLFEAdaptive = FreqHarmonic * FrequencyMultiplier;
			//peak interval based off cylinder count
			if (H.S.EngineCylinders == 1 || H.S.EngineCylinders == 2 || H.S.EngineCylinders == 4 || H.S.EngineCylinders == 8 || H.S.EngineCylinders == 16)
			{
				PeakA2CylMod = 0.8333333333333333;
				PeakB1CylMod = 1.666666666666667;
				PeakA1CylMod = 2.5;
				PeakB2CylMod = 2.083333333333333;
			}

			if (H.S.EngineCylinders == 3 || H.S.EngineCylinders == 6 || H.S.EngineCylinders == 12)
			{
				PeakA2CylMod = 1.0;
				PeakB1CylMod = 1.5;
				PeakA1CylMod = 2.5;
				PeakB2CylMod = 2.0;
			}

			if (H.S.EngineCylinders == 5 || H.S.EngineCylinders == 10)
			{
				PeakA2CylMod = 1.0;
				PeakB1CylMod = 1.5;
				PeakA1CylMod = 2.5;
				PeakB2CylMod = 2.0;
			}

			//lfe rpm freq multi 
			double num20 = H.S.IdleRPM * 0.008333333;
			if (num20 < 10)
				FrequencyMultiplier = 1 + (1 - (num20 / 10));
			else
				FrequencyMultiplier = 1;

			//LFE hp scaler
			if (H.S.MaxPower > 0 && H.S.MaxPower < 200)
				LFEhpScale = 0.85;
			if (H.S.MaxPower >= 200 && H.S.MaxPower < 300)
				LFEhpScale = 0.88;
			if (H.S.MaxPower >= 300 && H.S.MaxPower < 400)
				LFEhpScale = 0.91;
			if (H.S.MaxPower >= 400 && H.S.MaxPower < 500)
				LFEhpScale = 0.94;
			if (H.S.MaxPower >= 500 && H.S.MaxPower < 600)
				LFEhpScale = 0.97;
			if (H.S.MaxPower >= 600)
				LFEhpScale = 1;


			//LFEeq
			if (FreqLFEAdaptive <= 0 || FreqLFEAdaptive >= 300) 
				LFEeq = 0;
			if (FreqLFEAdaptive < 300 && FreqLFEAdaptive > 250)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 250) / (300 - 250)) * 38.5); // 250Hz:38.5 - 300Hz:0;
			if (FreqLFEAdaptive <= 250 && FreqLFEAdaptive > 70)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 70) / (70 - 142)) * 11); // 70Hz:11 - 250Hz:38.5
			if (FreqLFEAdaptive <= 70 && FreqLFEAdaptive > 65)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 65) / (65 - 96.666666666666)) * 9.5); //65Hz:9.5 - 70Hz:11
			if (FreqLFEAdaptive <= 65 && FreqLFEAdaptive > 60)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 60) / (60 - 102.5)) * 8.5); //   60Hz:8.5 - 65Hz:9.5
			if (FreqLFEAdaptive <= 60 && FreqLFEAdaptive > 45)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 45) / (45 - 115)) * 7); //   45Hz:7 - 60Hz:8.5
			if (FreqLFEAdaptive <= 45 && FreqLFEAdaptive > 32)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 32) / (32 - 201)) * 6.5); //   32Hz:6.5 - 45Hz:7
			if (FreqLFEAdaptive <= 32 && FreqLFEAdaptive > 29)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 29) / (29 - 39)) * 5); //   29Hz:5 - 32Hz:6.5
			if (FreqLFEAdaptive <= 29 && FreqLFEAdaptive > 21)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 21) / (21 - 53)) * 4); //   21Hz:4 - 29Hz:5
			if (FreqLFEAdaptive <= 21 && FreqLFEAdaptive > 17)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 17) / (29 - 17)) * 6); // 17Hz:6 - 21Hz:4
			if (FreqLFEAdaptive <= 17 && FreqLFEAdaptive > 14)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 14) / (53 - 14)) * 6.5); // 14Hz:6.5 - 17Hz:6
			if (FreqLFEAdaptive <= 14 && FreqLFEAdaptive > 11)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 11) / (53 - 11)) * 7); // 11Hz:7 - 14Hz:6.5
			if (FreqLFEAdaptive <= 11 && FreqLFEAdaptive > 10)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 10) / (18 - 10)) * 8); // 10Hz:8 - 11Hz:7
			if (FreqLFEAdaptive <= 10 && FreqLFEAdaptive > 9)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 9) / (18 - 9)) * 9); // 9Hz: 9 - 10Hz:8
			if (FreqLFEAdaptive <= 9 && FreqLFEAdaptive > 8)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 8) / (18 - 8)) * 10); // 8Hz:10 -  9Hz:9
			if (FreqLFEAdaptive <= 8 && FreqLFEAdaptive > 6)
				LFEeq = LFEhpScale * ((1 - (FreqLFEAdaptive - 8) / (10 - 8)) * 10); // 8Hz:10 - 6Hz:20
			if (FreqLFEAdaptive <= 6 && FreqLFEAdaptive > 0)
				LFEeq = LFEhpScale * ((FreqLFEAdaptive / 6) * 20);

			//peakEQ cylinder modifier
			if (H.S.EngineCylinders == 3 || H.S.EngineCylinders == 6 || H.S.EngineCylinders == 12)
				peakEQeng = 0.8;
			if (H.S.EngineCylinders == 5 || H.S.EngineCylinders == 10)
				peakEQeng = 0.9;
			if (H.S.EngineCylinders == 1 || H.S.EngineCylinders == 2 || H.S.EngineCylinders == 4 || H.S.EngineCylinders == 8 || H.S.EngineCylinders == 16)
				peakEQeng = 1;

			//peakEQ
			if (H.S.Redline > 0 && H.S.Redline < 3000)
				peakEQ = 5 * peakEQeng;
			if (H.S.Redline >= 3000 && H.S.Redline < 4000)
				peakEQ = 4 * peakEQeng;
			if (H.S.Redline >= 4000 && H.S.Redline < 5000)
				peakEQ = 3 * peakEQeng;
			if (H.S.Redline >= 5000 && H.S.Redline < 6000)
				peakEQ = 2 * peakEQeng;
			if (H.S.Redline >= 6000 && H.S.Redline < 7000)
				peakEQ = 1.85 * peakEQeng;
			if (H.S.Redline >= 7000 && H.S.Redline < 8000)
				peakEQ = 1.65 * peakEQeng;
			if (H.S.Redline >= 8000 && H.S.Redline < 9000)
				peakEQ = 1.5 * peakEQeng;
			if (H.S.Redline >= 9000 && H.S.Redline < 10000)
				peakEQ = 1.35 * peakEQeng;
			if (H.S.Redline >= 10000 && H.S.Redline < 11000)
				peakEQ = 1.35 * peakEQeng;
			if (H.S.Redline >= 11000 && H.S.Redline < 12000)
				peakEQ = 1.4 * peakEQeng;
			if (H.S.Redline >= 12000 && H.S.Redline < 13000)
				peakEQ = 1.45 * peakEQeng;
			if (H.S.Redline >= 13000 && H.S.Redline < 14000)
				peakEQ = 1.55 * peakEQeng;
			if (H.S.Redline >= 14000 && H.S.Redline < 15000)
				peakEQ = 1.8 * peakEQeng;
			if (H.S.Redline >= 15000 && H.S.Redline < 17000)
				peakEQ = 1.4 * peakEQeng;
			if (H.S.Redline >= 17000 && H.S.Redline < 18000)
				peakEQ = 1.7 * peakEQeng;
			if (H.S.Redline >= 18000)
				peakEQ = 1.45 * peakEQeng;

			//peakGear multi
			if (Gear <= 1)
				peakGearMulti = 1;
			if (Gear == 2)
				peakGearMulti = 1.03;
			if (Gear == 3)
				peakGearMulti = 1.06;
			if (Gear == 4)
				peakGearMulti = 1.12;
			if (Gear >= 5)
				peakGearMulti = 1.20;

			//rpmMain EQ
			if (FreqHarmonic > 65 && FreqHarmonic <= 95)
				rpmMainEQ = (1 - (FreqHarmonic - 65) / (210.375 - 65)) * 1.26; //1.26 at 30hz, 1 @ 95hz
			if (FreqHarmonic > 30 && FreqHarmonic <= 65)
				rpmMainEQ = (1 - (FreqHarmonic - 30) / (380 - 30)) * 1.4; //1.4 at 30hz, 1.26 @ 65hz
			if (FreqHarmonic > 20 && FreqHarmonic <= 30)
				rpmMainEQ = (1 - (FreqHarmonic - 20) / (70 - 20)) * 1.75; //1.75 @ 20hz, 1.4 @ 30hz
			if (FreqHarmonic > 0 && FreqHarmonic <= 20)
				rpmMainEQ = (1 - (FreqHarmonic - 10) / (27.77777777777777 - 10)) * 4; //4 at 10hz, 1.75 @ 20hz
			if (FreqHarmonic == 0 || FreqHarmonic > 95)
				rpmMainEQ = 1;



			FreqPeakA1 = FreqHarmonic * PeakA1CylMod;
			FreqPeakB1 = FreqHarmonic * PeakB1CylMod;
			FreqPeakA2 = FreqHarmonic * PeakA2CylMod;
			FreqPeakB2 = FreqHarmonic * PeakB2CylMod;
			double num21 = 1.0;
			double num22 = 1.0;
			if (Gear > 0)
			{
				num21 -= AccSurge2S.Clamp(0.0, 15.0) * 0.01;
				if (Accelerator < 20.0 && AccSurgeAvg < 0.0)
					num22 += Math.Max(0.0, Math.Max(0.0, RPMPercent - IdlePercent * (1.0 + Gear * 0.2))
											* (0.2 + 0.6 * MixDisplacement) - Accelerator * 0.05 * (0.2 + 0.6 * MixDisplacement));
			}
			Gain1H = FreqHarmonic >= 25.0
					? (FreqHarmonic >= 40.0
						? (FreqHarmonic >= 65.0
							? (FreqHarmonic >= 95.0
								? (FreqHarmonic >= 125.0
									 ? (FreqHarmonic >= 150.0

									 ? (1 - (FreqHarmonic - 150) / (300 - 150)) * 90   // 150-300hz:  90-0

									: (1 - (FreqHarmonic - 125) / (125 - 1225)) * 88)  // 125-150hz:  88-90

									: (1 - (FreqHarmonic - 95) / (95 - 945)) * 85)	 //  95-125hz:  85-88

								: (1 - (FreqHarmonic - 65) / (65 - 137)) * 60)	 //   65-95hz:  60-85

							: (1 - (FreqHarmonic - 40) / (40 - 165)) * 50)	 //  40-65hz:   50-60

						: (1 - (FreqHarmonic - 25) / (25 - 160)) * 45)		//  25-40hz:  45-50

					: (1 - (FreqHarmonic - 0) / (0 - 200)) * 40;		   // 00-25hz:  40-45

			Gain1H = Math.Max(Gain1H, 0.0) * num21 * num22 * (0.8 + 0.2 * MixPower + 0.2 * MixCylinder);
			Gain1H = (Gain1H.Clamp(0.0, sbyte.MaxValue));
			PeakA1Start = RedlinePercent * (0.68 + GearInterval * Gear * 0.12);
			PeakB1Start = RedlinePercent * (0.64 + GearInterval * Gear * 0.12);
			PeakA2Start = RedlinePercent * (0.62 + MixPower * GearInterval * Gear * 0.14);
			PeakB2Start = RedlinePercent * (0.82 - MixTorque * 0.16);
			PeakA1Modifier = ((RPMPercent - PeakA1Start) / (RedlinePercent - PeakA1Start + (1.0 - RedlinePercent) * (0.75 + MixCylinder * 0.75))).Clamp(0.0, 1.0);
			PeakB1Modifier = ((RPMPercent - PeakB1Start) / (RedlinePercent - PeakB1Start + (1.0 - RedlinePercent) * (0.0 + MixCylinder))).Clamp(0.0, 1.0);
			PeakA2Modifier = ((RPMPercent - PeakA2Start) / (RedlinePercent - PeakA2Start)).Clamp(0.0, 1.0);
			PeakB2Modifier = ((RPMPercent - PeakB2Start) / (RedlinePercent - PeakB2Start + (1.0 - RedlinePercent) * (1.0 - MixDisplacement))).Clamp(0.0, 1.0);
			//rpmMain sum
			rpmMainSum = (0.13333333333 * (FreqHarmonic + 64.375) * 0.7) * (Gain1H / 100);

			//rpmMain
			if (FreqHarmonic <= 0 && FreqHarmonic >= 900)
				rpmMain = 0;
			if (FreqHarmonic >= 750 && FreqHarmonic < 900)
				rpmMain = (1 - (FreqHarmonic - 750) / (900 - 750)) * rpmMainSum;
			if (FreqHarmonic > 0 && FreqHarmonic < 750)
				rpmMain = rpmMainSum;

			GainPeakA1 = FreqPeakA1 >= 30.0 ? (FreqPeakA1 >= 150.0 ? (FreqPeakA1 >= 250.0 ? (1 - (FreqPeakA1 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakA1 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakA1 - 110) / (30 - 110)) * 60) : 0;
			GainPeakA1 = Math.Max(GainPeakA1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakA1Front = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1Rear = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1 = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));

			GainPeakB1 = FreqPeakB1 >= 30.0 ? (FreqPeakB1 >= 150.0 ? (FreqPeakB1 >= 250.0 ? (1 - (FreqPeakB1 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakB1 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakB1 - 110) / (30 - 110)) * 60) : 0;
			GainPeakB1 = Math.Max(GainPeakB1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakB1Front = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1Rear = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1 = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));

			GainPeakA2 = FreqPeakA2 >= 30.0 ? (FreqPeakA2 >= 150.0 ? (FreqPeakA2 >= 250.0 ? (1 - (FreqPeakA2 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakA2 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakA2 - 110) / (30 - 110)) * 60) : 0;
			GainPeakA2 = Math.Max(GainPeakA2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakA2Front = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2Rear = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2 = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = FreqPeakB2 >= 30.0 ? (FreqPeakB2 >= 150.0 ? (FreqPeakB2 >= 250.0 ? (1 - (FreqPeakB2 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakB2 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakB2 - 110) / (30 - 110)) * 60) : 0;
			GainPeakB2 = Math.Max(GainPeakB2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakB2Front = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2Rear = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			//rpmPeakA2Rear sum
			rpmPeakA2RearSum = (0.35087719298 * (FreqPeakA2 + 25.6) * 0.7) * (GainPeakA2Rear / 127);

			//rpmPeakA2Rear
			if (FreqPeakA2 <= 15)
				rpmPeakA2Rear = ((FreqPeakA2/15) * rpmPeakA2RearSum) * peakGearMulti * peakEQ;

			if (FreqPeakA2 >= 325 && FreqPeakA2 <= 600)
				rpmPeakA2Rear = ((1 - (FreqPeakA2 - 325) / (600 - 325)) * rpmPeakA2RearSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakA2 > 600)
				rpmPeakA2Rear = 0;

			if (FreqPeakA2 > 15 && FreqPeakA2 < 325)
				rpmPeakA2Rear = rpmPeakA2RearSum * peakGearMulti * peakEQ;


			//rpmPeakB1Rear sum
			rpmPeakB1RearSum = (0.35087719298 * (FreqPeakB1 + 25.6) * 0.7) * (GainPeakB1Rear / 127);
			
			//rpmPeakB1Rear
			if (FreqPeakB1 <= 15)
				rpmPeakB1Rear = ((FreqPeakB1 / 15) * rpmPeakB1RearSum) * peakGearMulti * peakEQ;

			if (FreqPeakB1 >= 325 && FreqPeakB1 <= 600)
				rpmPeakB1Rear = ((1 - (FreqPeakB1 - 325) / (600 - 325)) * rpmPeakB1RearSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakB1 > 600)
				rpmPeakB1Rear = 0;

			if (FreqPeakB1 > 15 && FreqPeakB1 < 325)
				rpmPeakB1Rear = rpmPeakB1RearSum * peakGearMulti * peakEQ;

			//rpmPeakA1Rear sum
			rpmPeakA1RearSum = (0.35087719298 * (FreqPeakA1 + 25.6) * 0.7) * (GainPeakA1Rear / 127);

			//rpmPeakA1Rear
			if (FreqPeakA1 <= 15)
				rpmPeakA1Rear = ((FreqPeakA1 / 15) * rpmPeakA1RearSum) * peakGearMulti * peakEQ;

			if (FreqPeakA1 >= 325 && FreqPeakA1 <= 600)
				rpmPeakA1Rear = ((1 - (FreqPeakA1 - 325) / (600 - 325)) * rpmPeakA1RearSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakA1 > 600)
				rpmPeakA1Rear = 0;

			if (FreqPeakA1 > 15 && FreqPeakA1 < 325)
				rpmPeakA1Rear = rpmPeakA1RearSum * peakGearMulti * peakEQ;

			//rpmPeakB2Rear sum
			rpmPeakB2RearSum = (0.35087719298 * (FreqPeakB2 + 25.6) * 0.7) * (GainPeakB2Rear / 127);

			//rpmPeakB2Rear
			if (FreqPeakB2 <= 15)
				rpmPeakB2Rear = ((FreqPeakB2 / 15) * rpmPeakB2RearSum) * peakGearMulti * peakEQ;

			if (FreqPeakB2 >= 325 && FreqPeakB2 <= 600)
				rpmPeakB2Rear = ((1 - (FreqPeakB2 - 325) / (600 - 325)) * rpmPeakB2RearSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakB2 > 600)
				rpmPeakB2Rear = 0;

			if (FreqPeakB2 > 15 && FreqPeakB2 < 325)
				rpmPeakB2Rear = rpmPeakB2RearSum * peakGearMulti * peakEQ;

			//rpmPeakA2Front sum
			rpmPeakA2FrontSum = (0.35087719298 * (FreqPeakA2 + 25.6) * 0.7) * (GainPeakA2Front / 127);

			//rpmPeakA2Front
			if (FreqPeakA2 <= 15)
				rpmPeakA2Front = ((FreqPeakA2 / 15) * rpmPeakA2FrontSum) * peakGearMulti * peakEQ;

			if (FreqPeakA2 >= 325 && FreqPeakA2 <= 600)
				rpmPeakA2Front = ((1 - (FreqPeakA2 - 325) / (600 - 325)) * rpmPeakA2FrontSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakA2 > 600)
				rpmPeakA2Front = 0;

			if (FreqPeakA2 > 15 && FreqPeakA2 < 325)
				rpmPeakA2Front = rpmPeakA2FrontSum * peakGearMulti * peakEQ;

			//rpmPeakB1Front sum
			rpmPeakB1FrontSum = (0.35087719298 * (FreqPeakB1 + 25.6) * 0.7) * (GainPeakB1Front / 127);

			//rpmPeakB1Front
			if (FreqPeakB1 <= 15)
				rpmPeakB1Front = ((FreqPeakB1 / 15) * rpmPeakB1FrontSum) * peakGearMulti * peakEQ;

			if (FreqPeakB1 >= 325 && FreqPeakB1 <= 600)
				rpmPeakB1Front = ((1 - (FreqPeakB1 - 325) / (600 - 325)) * rpmPeakB1FrontSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakB1 > 600)
				rpmPeakB1Front = 0;

			if (FreqPeakB1 > 15 && FreqPeakB1 < 325)
				rpmPeakB1Front = rpmPeakB1FrontSum * peakGearMulti * peakEQ;

			//rpmPeakA1Front sum
			rpmPeakA1FrontSum = (0.35087719298 * (FreqPeakA1 + 25.6) * 0.7) * (GainPeakA1Front / 127);

			//rpmPeakA1Front
			if (FreqPeakA1 <= 15)
				rpmPeakA1Front = ((FreqPeakA1 / 15) * rpmPeakA1FrontSum) * peakGearMulti * peakEQ;

			if (FreqPeakA1 >= 325 && FreqPeakA1 <= 600)
				rpmPeakA1Front = ((1 - (FreqPeakA1 - 325) / (600 - 325)) * rpmPeakA1FrontSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakA1 > 600)
				rpmPeakA1Front = 0;

			if (FreqPeakA1 > 15 && FreqPeakA1 < 325)
				rpmPeakA1Front = rpmPeakA1FrontSum * peakGearMulti * peakEQ;

			//rpmPeakB2Front sum
			rpmPeakB2FrontSum = (0.35087719298 * (FreqPeakB2 + 25.6) * 0.7) * (GainPeakB2Front / 127);

			//rpmPeakB2Front
			if (FreqPeakB2 <= 15)
				rpmPeakB2Front = ((FreqPeakB2 / 15) * rpmPeakB2FrontSum) * peakGearMulti * peakEQ;

			if (FreqPeakB2 >= 325 && FreqPeakB2 <= 600)
				rpmPeakB2Front = ((1 - (FreqPeakB2 - 325) / (600 - 325)) * rpmPeakB2FrontSum) * peakGearMulti * peakEQ; // 1 at 325Hz, 0 at 600Hz;

			if (FreqPeakB2 > 600)
				rpmPeakB2Front = 0;

			if (FreqPeakB2 > 15 && FreqPeakB2 < 325)
				rpmPeakB2Front = rpmPeakB2FrontSum * peakGearMulti * peakEQ;
		}	// Runtime()
	}
}
