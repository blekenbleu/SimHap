// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimData
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;		// for GameData
using SimHub.Plugins;		// PluginManager
using System;				// for Math

namespace sierses.Sim
{
	public partial class SimData
	{
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
		public double EngineLoad;
		public int WiperStatus;
		public int CarInitCount;
		public int IdleSampleCount;			// used in Refresh()
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
		public double FrequencyMultiplier;
		public double FreqHarmonic;
		public double FreqOctave;
		public double FreqLFEAdaptive;
		public double FreqPeakA1;
		public double FreqPeakA2;
		public double FreqPeakB1;
		public double FreqPeakB2;
		public double Gain1H;
		public double GainLFEAdaptive;
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
		public double MotionPitch;
		public double MotionRoll;
		public double MotionYaw;
		public double MotionHeave;
		public double MotionSurge;
		public double MotionSway;
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
		public bool RumbleFromPlugin;
		public double TiresLeft;
		public double TiresRight;
		public double RumbleLeftAvg;
		public double RumbleRightAvg;
		public double RumbleLeft;
		public double RumbleRight;
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
		public double SuspensionRumbleMultR1;
		public double SuspensionRumbleMultR2;
		public double SuspensionRumbleMultR3;


		private void SetRPMMix()
		{
			InvMaxRPM = H.S.MaxRPM > 0.0 ? 1.0 / H.S.MaxRPM : 0.0001;
			IdlePercent = H.S.IdleRPM * InvMaxRPM;
			RedlinePercent = H.S.Redline * InvMaxRPM;
			if (H.S.Displacement > 0.0)
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

		static PluginManager PM;

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
					if (RumbleLeftAvg == 0.0)
						RumbleLeftAvg = TiresLeft;
					if (RumbleRightAvg == 0.0)
						RumbleRightAvg = TiresRight;
					RumbleLeftAvg = (RumbleLeftAvg + TiresLeft) * 0.5;
					RumbleRightAvg = (RumbleRightAvg + TiresRight) * 0.5;
					RumbleLeft = Math.Abs(TiresLeft / RumbleLeftAvg - 1.0) * 2000.0;
					RumbleRight = Math.Abs(TiresRight / RumbleRightAvg - 1.0) * 2000.0;
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
		internal void Refresh(Haptics shp, PluginManager pluginManager)
		{
			H = shp;
			PM = pluginManager;
			FPS = (double) PM.GetPropertyValue("DataCorePlugin.DataUpdateFps");
			Rpms = Convert.ToUInt16(0.5 + H.N.Rpms);
			RPMPercent = H.N.Rpms * InvMaxRPM;
			SpeedMs = H.N.SpeedKmh * 0.277778;
			InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
			Accelerator = H.N.Throttle;
			Brake = H.N.Brake;
			Clutch = H.N.Clutch;
			Handbrake = H.N.Handbrake;
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
				RumbleLeftAvg = 0.0;
				RumbleRightAvg = 0.0;
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
			MotionPitch = MotionPitchOffset + 100.0 * Math.Pow(Math.Abs(MotionPitchMult * H.N.OrientationPitch) * 0.01, 1.0 / MotionPitchGamma);
			MotionRoll = MotionRollOffset + 100.0 * Math.Pow(Math.Abs(MotionRollMult * H.N.OrientationRoll) * 0.01, 1.0 / MotionRollGamma);
			MotionYaw = MotionYawOffset + 100.0 * Math.Pow(Math.Abs(MotionYawMult * YawRateAvg) * 0.01, 1.0 / MotionYawGamma);
			MotionHeave = MotionHeaveOffset + 100.0 * Math.Pow(Math.Abs(MotionHeaveMult * AccHeave[Acc0]) * 0.01, 1.0 / MotionHeaveGamma);
			if (H.N.OrientationPitch < 0.0)
				MotionPitch = -MotionPitch;
			if (H.N.OrientationRoll < 0.0)
				MotionRoll = -MotionRoll;
			if (YawRateAvg < 0.0)
				MotionYaw = -MotionYaw;
			if (AccHeave[Acc0] < 0.0)
				MotionHeave = -MotionHeave;
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
			double num18 = RumbleMult * RumbleMultAll * (0.6 + SpeedMs * (90.0 - SpeedMs) * 0.0002);
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
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
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
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
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
								SuspensionRumbleMultR1 = num18 * 0.0;
								SuspensionRumbleMultR2 = num18 * 1.0;
								SuspensionRumbleMultR3 = num18 * 0.0;
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
							SuspensionRumbleMultR1 = num18 * 1.5;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.0;
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
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
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
					SuspensionRumbleMultR1 = num18 * 2.0;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 1.5;
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
							SuspensionRumbleMultR1 = num18 * 0.0;
							SuspensionRumbleMultR2 = num18 * 1.0;
							SuspensionRumbleMultR3 = num18 * 0.0;
						}
						else
						{
							SuspensionFreqR1 = SuspensionFreq * 0.25;
							SuspensionFreqR2 = SuspensionFreq * (143.0 / 400.0);
							SuspensionFreqR3 = SuspensionFreq * 0.5;
							SuspensionMultR1 = num13 * 0.6;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.8;
							SuspensionRumbleMultR1 = num18 * 1.0;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.5;
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
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
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
					SuspensionRumbleMultR1 = num18 * 1.5;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 2.0;
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
				SuspensionRumbleMultR1 = num18 * 0.0;
				SuspensionRumbleMultR2 = num18 * 2.0;
				SuspensionRumbleMultR3 = num18 * 0.0;
			}
			EngineLoad = H.N.CarSettings_CurrentDisplayedRPMPercent * 0.5;
			EngineLoad += H.N.SpeedKmh * H.N.SpeedKmh * 0.0003;
			EngineLoad += H.N.SpeedKmh * 0.02;
			if (Math.Abs(SuspensionAccAll) > 0.5)
				EngineLoad += 200.0 * Math.Sin(H.N.OrientationPitch * 0.0174533);
			EngineLoad -= EngineLoad * (1.0 - MixPower) * 0.5;
			EngineLoad *= H.N.Throttle * 0.01 * 0.01;

			if (IdleSampleCount < 20) /*&& FrameCountTicks % 2500000L <= 150000L*/	// Refresh()sniff: ignore FrameCountTicks .. for now
				if (H.N.Rpms > 300 && H.N.Rpms <= idleRPM * 1.1) // Refresh(): supposes that idleRPM is somewhat valid..??
			{
				double num19 = Math.Abs(H.Gdat.OldData.Rpms - H.N.Rpms) * FPS;

				if (num19 < 40.0)
				{
					idleRPM = Convert.ToUInt16((1 + idleRPM + (int)H.N.Rpms) >> 1); // Refresh(): averaging with previous average
					++IdleSampleCount;								// Refresh(): increment if difference < 40
					double num20 = idleRPM * 0.008333333;		// Refresh(): some FrequencyMultiplier magic
					FrequencyMultiplier = num20 >= 5.0 ? (num20 >= 10.0 ? (num20 <= 20.0 ? (num20 <= 40.0 ? 1.0 : 0.25) : 0.5) : 2.0) : 4.0;
				}
				if (20 == IdleSampleCount && 0 == H.S.IdleRPM)	// Refresh(): change H.S.IdleRPM?
					H.S.Idle(idleRPM);			// Refresh() sniff: only if it was 0
			}

			if (FrameCountTicks % 5000000L <= 150000L)
			{
				SetRPMMix();
			}
			FreqHarmonic = H.N.Rpms * 0.008333333;
			FreqLFEAdaptive = FreqHarmonic * FrequencyMultiplier;
			FreqPeakA1 = FreqHarmonic * 1.5 * (1.0 + 8 * 0.08333333);
			FreqPeakB1 = FreqHarmonic * 0.75 * (1.0 + 8 * 0.08333333);
			FreqPeakA2 = FreqHarmonic * 0.5 * (1.0 + 8 * 0.08333333);
			FreqPeakB2 = FreqHarmonic * 1.25 * (1.0 + 8 * 0.08333333);
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
                                    ? (1 - (FreqHarmonic - 95) / (200 - 95)) * 95.55
                                    : (1 - (FreqHarmonic - 95) / (200 - 95)) * 95.55)
                                : (3.5 - (FreqHarmonic - 95) / (65 - 95)) * 27.3)
                            : (5.25 - (FreqHarmonic - 65) / (40 - 65)) * 13)
                        : (1.6 - (FreqHarmonic - 25) / (1 - 25)) * 25)
                    : (1.6 - (FreqHarmonic - 25) / (1 - 25)) * 25;
            Gain1H = Math.Max(Gain1H, 0.0) * num21 * num22 * (0.8 + 0.2 * MixPower + 0.2 * MixCylinder);
			Gain1H = (Gain1H.Clamp(0.0, sbyte.MaxValue));
			PeakA1Start = RedlinePercent * (0.76 + GearInterval * Gear * 0.06);
			PeakB1Start = RedlinePercent * (0.72 + GearInterval * Gear * 0.06);
			PeakA2Start = RedlinePercent * (0.70 + MixPower * GearInterval * Gear * 0.9);
			PeakB2Start = RedlinePercent * (0.90 - MixTorque * 0.08);
			PeakA1Modifier = ((RPMPercent - PeakA1Start) / (RedlinePercent - PeakA1Start + (1.0 - RedlinePercent) * (0.75 + MixCylinder * 0.75))).Clamp(0.0, 1.0);
			PeakB1Modifier = ((RPMPercent - PeakB1Start) / (RedlinePercent - PeakB1Start + (1.0 - RedlinePercent) * (0.0 + MixCylinder))).Clamp(0.0, 1.0);
			PeakA2Modifier = ((RPMPercent - PeakA2Start) / (RedlinePercent - PeakA2Start)).Clamp(0.0, 1.0);
			PeakB2Modifier = ((RPMPercent - PeakB2Start) / (RedlinePercent - PeakB2Start + (1.0 - RedlinePercent) * (1.0 - MixDisplacement))).Clamp(0.0, 1.0);

			GainPeakA1 = FreqPeakA1 >= 45.0 ? (FreqPeakA1 >= 105.0 ? (1 - (FreqPeakA1 - 105) / (225 - 105)) * 90 : (1 - (FreqPeakA1 - 105) / (45 - 105)) * 90) : 0;

            GainPeakA1 = Math.Max(GainPeakA1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakA1Front = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1Rear = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1 = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));

			GainPeakB1 = FreqPeakB1 >= 45.0 ? (FreqPeakB1 >= 105.0 ? (1 - (FreqPeakB1 - 105) / (225 - 105)) * 90 : (1 - (FreqPeakB1 - 105) / (45 - 105)) * 90) : 0;

            GainPeakB1 = Math.Max(GainPeakB1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakB1Front = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1Rear = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1 = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));

            GainPeakA2 = FreqPeakA2 >= 45.0 ? (FreqPeakA2 >= 105.0 ? (1 - (FreqPeakA2 - 105) / (225 - 105)) * 90 : (1 - (FreqPeakA2 - 105) / (45 - 105)) * 90) : 0;

            GainPeakA2 = Math.Max(GainPeakA2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakA2Front = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2Rear = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2 = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));

			GainPeakB2 = FreqPeakB2 >= 45.0 ? (FreqPeakB2 >= 105.0 ? (1 - (FreqPeakB2 - 105) / (225 - 105)) * 90 : (1 - (FreqPeakB2 - 105) / (45 - 105)) * 90) : 0;
            GainPeakB2 = Math.Max(GainPeakB2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakB2Front = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2Rear = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));

			if (H.S.EngineCylinders < 1.0)
			{
				GainLFEAdaptive = 0.0;
				Gain1H = Math.Floor(Gain1H * 0.7);
				GainPeakA1Front = 0.0;
				GainPeakA1Rear = 0.0;
				GainPeakA1 = 0.0;
				GainPeakA2Front = 0.0;
				GainPeakA2Rear = 0.0;
				GainPeakA2 = 0.0;
				GainPeakB1Front = 0.0;
				GainPeakB1Rear = 0.0;
				GainPeakB1 = 0.0;
				GainPeakB2Front = 0.0;
				GainPeakB2Rear = 0.0;
				GainPeakB2 = 0.0;
			}
			
			if (EngineMult == 1.0)
				return;
			GainLFEAdaptive *= EngineMult * EngineMultAll;
			Gain1H *= EngineMult * EngineMultAll;
			GainPeakA1Front *= EngineMult * EngineMultAll;
			GainPeakA1Rear *= EngineMult * EngineMultAll;
			GainPeakA1 *= EngineMult * EngineMultAll;
			GainPeakA2Front *= EngineMult * EngineMultAll;
			GainPeakA2Rear *= EngineMult * EngineMultAll;
			GainPeakA2 *= EngineMult * EngineMultAll;
			GainPeakB1Front *= EngineMult * EngineMultAll;
			GainPeakB1Rear *= EngineMult * EngineMultAll;
			GainPeakB1 *= EngineMult * EngineMultAll;
			GainPeakB2Front *= EngineMult * EngineMultAll;
			GainPeakB2Rear *= EngineMult * EngineMultAll;
			GainPeakB2 *= EngineMult * EngineMultAll;
		}

	}
}
