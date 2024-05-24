// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimData
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;		// for GameData
using SimHub.Plugins;		// PluginManager
using System;				// for Math

namespace sierses.SimHap
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
		public bool ABSActive;
		public double ABSPulse;
		public long ABSPauseInterval;
		public long ABSPulseInterval;
		public long ABSTicks;
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
		public int IdleSampleCount;
		public double IdlePercent;
		public double RedlinePercent;
		public double RPMPercent;
		public double IntervalOctave;
		public double IntervalA;
		public double IntervalB;
		public double IntervalPeakA;
		public double IntervalPeakB;
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
		public double FreqIntervalA1;
		public double FreqIntervalA2;
		public double FreqPeakA1;
		public double FreqPeakA2;
		public double FreqPeakB1;
		public double FreqPeakB2;
		public double Gain1H;
		public double Gain1H2;
		public double Gain2H;
		public double Gain4H;
		public double GainOctave;
		public double GainLFEAdaptive;
		public double GainIntervalA1;
		public double GainIntervalA2;
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
		public double SlipXFL;
		public double SlipXFR;
		public double SlipXRL;
		public double SlipXRR;
		public double SlipXAll;
		public double SlipYFL;
		public double SlipYFR;
		public double SlipYRL;
		public double SlipYRR;
		public double SlipYAll;
		private double SlipXGammaBaseMult;
		public double SlipYGammaBaseMult;
		public double TireDiameterFL;
		public double TireDiameterFR;
		public double TireDiameterRL;
		public double TireDiameterRR;
		public double TireDiameterSampleFL;
		public double TireDiameterSampleFR;
		public double TireDiameterSampleRL;
		public double TireDiameterSampleRR;
		public int TireDiameterSampleCount;
		public int TireDiameterSampleMax;
		public double VelocityX;
		public double WheelLoadFL;
		public double WheelLoadFR;
		public double WheelLoadRL;
		public double WheelLoadRR;
		public double WheelSpeedFL;
		public double WheelSpeedFR;
		public double WheelSpeedRL;
		public double WheelSpeedRR;
		public double WheelRotationFL;
		public double WheelRotationFR;
		public double WheelRotationRL;
		public double WheelRotationRR;
		public double WheelSpinAll;
		public double WheelLockAll;
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
		public double SuspensionFreqRa;
		public double SuspensionFreqRb;
		public double SuspensionFreqRc;
		public double SuspensionFreqR1;
		public double SuspensionFreqR2;
		public double SuspensionFreqR3;
		public double SuspensionFreqR4;
		public double SuspensionFreqR5;
		public double SuspensionMultRa;
		public double SuspensionMultRb;
		public double SuspensionMultRc;
		public double SuspensionMultR1;
		public double SuspensionMultR2;
		public double SuspensionMultR3;
		public double SuspensionMultR4;
		public double SuspensionMultR5;
		public double SuspensionRumbleMultRa;
		public double SuspensionRumbleMultRb;
		public double SuspensionRumbleMultRc;
		public double SuspensionRumbleMultR1;
		public double SuspensionRumbleMultR2;
		public double SuspensionRumbleMultR3;
		public double SuspensionRumbleMultR4;
		public double SuspensionRumbleMultR5;

		private void SetRPMIntervals()
		{
			if (SHP.S.EngineCylinders == 1.0)
			{
				IntervalOctave = 4.0;
				IntervalA = 0.0;
				IntervalB = 0.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 2.0)
			{
				IntervalOctave = 4.0;
				IntervalA = 6.0;
				IntervalB = 0.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 4.0)
			{
				IntervalOctave = 8.0;
				IntervalA = 4.0;
				IntervalB = 5.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 8.0)
			{
				IntervalOctave = 16.0;
				IntervalA = 6.0;
				IntervalB = 10.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 16.0)
			{
				IntervalOctave = 16.0;
				IntervalA = 12.0;
				IntervalB = 20.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 3.0)
			{
				IntervalOctave = 6.0;
				IntervalA = 4.0;
				IntervalB = 0.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 6.0)
			{
				IntervalOctave = 12.0;
				IntervalA = 8.0;
				IntervalB = 10.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 12.0)
			{
				IntervalOctave = 12.0;
				IntervalA = 16.0;
				IntervalB = 20.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 5.0)
			{
				IntervalOctave = 10.0;
				IntervalA = 6.0;
				IntervalB = 9.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineCylinders == 10.0)
			{
				IntervalOctave = 10.0;
				IntervalA = 12.0;
				IntervalB = 18.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (SHP.S.EngineConfiguration == "R")
			{
				IntervalOctave = 12.0;
				IntervalA = 9.0;
				IntervalB = 15.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else
			{
				IntervalOctave = 0.0;
				IntervalA = 0.0;
				IntervalB = 0.0;
				IntervalPeakA = 0.0;
				IntervalPeakB = 0.0;
			}
		}

		private void SetRPMMix()
		{
			InvMaxRPM = SHP.S.MaxRPM > 0.0 ? 1.0 / SHP.S.MaxRPM : 0.0001;
			IdlePercent = SHP.S.IdleRPM * InvMaxRPM;
			RedlinePercent = SHP.S.Redline * InvMaxRPM;
			if (SHP.S.Displacement > 0.0)
			{
				CylinderDisplacement = SHP.S.Displacement / SHP.S.EngineCylinders;
				MixCylinder = 1.0 - Math.Max(2000.0 - CylinderDisplacement, 0.0)
									 * Math.Max(2000.0 - CylinderDisplacement, 0.0) * 2.5E-07;
				MixDisplacement = 1.0 - Math.Max(10000.0 - SHP.S.Displacement, 0.0)
										 * Math.Max(10000.0 - SHP.S.Displacement, 0.0) * 1E-08;
			}
			else
			{
				MixCylinder = 0.0;
				MixDisplacement = 0.0;
			}
			MixPower = 1.0 - Math.Max(2000.0 - (SHP.S.MaxPower - SHP.S.ElectricMaxPower), 0.0)
							 * Math.Max(2000.0 - (SHP.S.MaxPower - SHP.S.ElectricMaxPower), 0.0) * 2.5E-07;
			MixTorque = 1.0 - Math.Max(2000.0 - SHP.S.MaxTorque, 0.0) * Math.Max(2000.0 - SHP.S.MaxTorque, 0.0) * 2.5E-07;
			MixFront = !(SHP.S.EngineLocation == "F")
						 ? (!(SHP.S.EngineLocation == "FM")
							 ? (!(SHP.S.EngineLocation == "M")
								 ? (!(SHP.S.EngineLocation == "RM")
									 ? (!(SHP.S.PoweredWheels == "F")
										 ? (!(SHP.S.PoweredWheels == "R")
											 ? 0.2
											 : 0.1)
										 : 0.3)
									 : (!(SHP.S.PoweredWheels == "F")
									 ? (!(SHP.S.PoweredWheels == "R")
										 ? 0.3
										 : 0.2)
									 : 0.4)
								   )
								 : (!(SHP.S.PoweredWheels == "F")
								 	? (!(SHP.S.PoweredWheels == "R")
									 	? 0.5
									 	: 0.4
									  )
									 : 0.6
								   )
							   )
						 : (!(SHP.S.PoweredWheels == "F")
							 ? (!(SHP.S.PoweredWheels == "R")
								 ? 0.7
								 : 0.6
							   )
							 : 0.8
						   )
						)
					 : (!(SHP.S.PoweredWheels == "F")
					 	? (!(SHP.S.PoweredWheels == "R")
							 ? 0.8
					 	 	: 0.7
							)
					 	: 0.9
					   );
			MixMiddle = Math.Abs(MixFront - 0.5) * 2.0;
			MixRear = 1.0 - MixFront;
		}

		static PluginManager PM;
		static GameData data;
		private float Data(string prop)
		{
			var foo = PM.GetPropertyValue("DataCorePlugin.GameRawData.Data."+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private float Physics(string prop)
		{
			var foo = PM.GetPropertyValue("DataCorePlugin.GameRawData.Physics."+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private float Raw(string prop)
		{
			var foo = PM.GetPropertyValue("DataCorePlugin.GameRawData."+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private void UpdateVehicle(ref GameData Gdat)
		{
			PM = SHP.PM;
			data = Gdat;
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
			SlipXFL = 0.0;
			SlipXFR = 0.0;
			SlipXRL = 0.0;
			SlipXRR = 0.0;
			SlipYFL = 0.0;
			SlipYFR = 0.0;
			SlipYRL = 0.0;
			SlipYRR = 0.0;
			ABSActive = data.NewData.ABSActive == 1;
			bool flag = true;
			switch (SimHap.CurrentGame)
			{
				case GameId.AC:
					SuspensionDistFL = Physics("SuspensionTravel01");
					SuspensionDistFR = Physics("SuspensionTravel02");
					SuspensionDistRL = Physics("SuspensionTravel03");
					SuspensionDistRR = Physics("SuspensionTravel04");
					WheelRotationFL = Math.Abs(Physics("WheelAngularSpeed01"));
					WheelRotationFR = Math.Abs(Physics("WheelAngularSpeed02"));
					WheelRotationRL = Math.Abs(Physics("WheelAngularSpeed03"));
					WheelRotationRR = Math.Abs(Physics("WheelAngularSpeed04"));
					SlipFromRPS();
					SlipXFL = Math.Max(Physics("WheelSlip01") - Math.Abs(SlipYFL) * 1.0, 0.0);
					SlipXFR = Math.Max(Physics("WheelSlip02") - Math.Abs(SlipYFR) * 1.0, 0.0);
					SlipXRL = Math.Max(Physics("WheelSlip03") - Math.Abs(SlipYRL) * 1.0, 0.0);
					SlipXRR = Math.Max(Physics("WheelSlip04") - Math.Abs(SlipYRR) * 1.0, 0.0);
					if (TireDiameterFL == 0.0)
					{
						SlipXFL *= 0.5;
						SlipXFR *= 0.5;
						SlipXRL *= 0.5;
						SlipXRR *= 0.5;
					}
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
					WiperStatus = (int) SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Graphics.WiperLV");
					WheelRotationFL = Math.Abs(Physics("WheelAngularSpeed01"));
					WheelRotationFR = Math.Abs(Physics("WheelAngularSpeed02"));
					WheelRotationRL = Math.Abs(Physics("WheelAngularSpeed03"));
					WheelRotationRR = Math.Abs(Physics("WheelAngularSpeed04"));
					SlipFromRPS();
					SlipXFL = Math.Max(Physics("WheelSlip01") - Math.Abs(SlipYFL) * 2.0, 0.0);
					SlipXFR = Math.Max(Physics("WheelSlip02") - Math.Abs(SlipYFR) * 2.0, 0.0);
					SlipXRL = Math.Max(Physics("WheelSlip03") - Math.Abs(SlipYRL) * 2.0, 0.0);
					SlipXRR = Math.Max(Physics("WheelSlip04") - Math.Abs(SlipYRR) * 2.0, 0.0);
					if (TireDiameterFL == 0.0)
					{
						SlipXFL *= 0.5;
						SlipXFR *= 0.5;
						SlipXRL *= 0.5;
						SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.AMS1:
					SuspensionDistFL = Data("wheel01.suspensionDeflection");
					SuspensionDistFR = Data("wheel02.suspensionDeflection");
					SuspensionDistRL = Data("wheel03.suspensionDeflection");
					SuspensionDistRR = Data("wheel04.suspensionDeflection");
					SpeedMs = Raw("CurrentPlayer.speed");
					InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
					WheelRotationFL = Math.Abs(Data("wheel01.rotation"));
					WheelRotationFR = Math.Abs(Data("wheel02.rotation"));
					WheelRotationRL = Math.Abs(Data("wheel03.rotation"));
					WheelRotationRR = Math.Abs(Data("wheel04.rotation"));
					SlipFromRPS();
					SlipXFL = Math.Max(1.0 - (double) Data("wheel01.gripFract") - Math.Abs(SlipYFL) * 1.0, 0.0);
					SlipXFR = Math.Max(1.0 - (double) Data("wheel02.gripFract") - Math.Abs(SlipYFR) * 1.0, 0.0);
					SlipXRL = Math.Max(1.0 - (double) Data("wheel03.gripFract") - Math.Abs(SlipYRL) * 1.0, 0.0);
					SlipXRR = Math.Max(1.0 - (double) Data("wheel04.gripFract") - Math.Abs(SlipYRR) * 1.0, 0.0);
					if (TireDiameterFL == 0.0)
					{
						SlipXFL *= 0.5;
						SlipXFR *= 0.5;
						SlipXRL *= 0.5;
						SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.AMS2:
					SuspensionDistFL = Raw("mSuspensionTravel01");
					SuspensionDistFR = Raw("mSuspensionTravel02");
					SuspensionDistRL = Raw("mSuspensionTravel03");
					SuspensionDistRR = Raw("mSuspensionTravel04");
					WheelRotationFL = Math.Abs(Raw("mTyreRPS01"));
					WheelRotationFR = Math.Abs(Raw("mTyreRPS02"));
					WheelRotationRL = Math.Abs(Raw("mTyreRPS03"));
					WheelRotationRR = Math.Abs(Raw("mTyreRPS04"));
					SlipFromRPS();
					SlipXFL = Math.Max(Raw("mTyreSlipSpeed01") * 0.1 - Math.Abs(SlipYFL) * 1.0, 0.0);
					SlipXFR = Math.Max(Raw("mTyreSlipSpeed02") * 0.1 - Math.Abs(SlipYFR) * 1.0, 0.0);
					SlipXRL = Math.Max(Raw("mTyreSlipSpeed03") * 0.1 - Math.Abs(SlipYRL) * 1.0, 0.0);
					SlipXRR = Math.Max(Raw("mTyreSlipSpeed04") * 0.1 - Math.Abs(SlipYRR) * 1.0, 0.0);
					if (TireDiameterFL == 0.0)
					{
						SlipXFL *= 0.5;
						SlipXFR *= 0.5;
						SlipXRL *= 0.5;
						SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.D4:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
					WheelSpeedFL = Math.Abs(Raw("WheelSpeedFrontLeft"));
					WheelSpeedFR = Math.Abs(Raw("WheelSpeedFrontRight"));
					WheelSpeedRL = Math.Abs(Raw("WheelSpeedRearLeft"));
					WheelSpeedRR = Math.Abs(Raw("WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.DR2:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
					WheelSpeedFL = Math.Abs(Raw("WheelSpeedFrontLeft"));
					WheelSpeedFR = Math.Abs(Raw("WheelSpeedFrontRight"));
					WheelSpeedRL = Math.Abs(Raw("WheelSpeedRearLeft"));
					WheelSpeedRR = Math.Abs(Raw("WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					VelocityX = Raw("WorldSpeedX") * Math.Sin(Raw("XR"));
					YawRate = data.NewData.OrientationYawAcceleration;
					if (VelocityX < 0.0)
					{
						if (YawRate < 0.0)
						{
							SlipXFL = -VelocityX * WheelLoadFL * YawRate * 0.5;
							SlipXFR = -VelocityX * WheelLoadFR * YawRate * 0.5;
							SlipXRL = -VelocityX * WheelLoadRL * YawRate * 1.0;
							SlipXRR = -VelocityX * WheelLoadRR * YawRate * 1.0;
						}
						else
						{
							SlipXFL = -VelocityX * WheelLoadFL * YawRate * 1.0;
							SlipXFR = -VelocityX * WheelLoadFR * YawRate * 1.0;
							SlipXRL = -VelocityX * WheelLoadRL * YawRate * 0.5;
							SlipXRR = -VelocityX * WheelLoadRR * YawRate * 0.5;
						}
					}
					else if (YawRate < 0.0)
					{
						SlipXFL = VelocityX * WheelLoadFL * -YawRate * 1.0;
						SlipXFR = VelocityX * WheelLoadFR * -YawRate * 1.0;
						SlipXRL = VelocityX * WheelLoadRL * -YawRate * 0.5;
						SlipXRR = VelocityX * WheelLoadRR * -YawRate * 0.5;
					}
					else
					{
						SlipXFL = VelocityX * WheelLoadFL * -YawRate * 0.5;
						SlipXFR = VelocityX * WheelLoadFR * -YawRate * 0.5;
						SlipXRL = VelocityX * WheelLoadRL * -YawRate * 1.0;
						SlipXRR = VelocityX * WheelLoadRR * -YawRate * 1.0;
					}
					SlipXFL = Math.Max(SlipXFL, 0.0);
					SlipXFR = Math.Max(SlipXFL, 0.0);
					SlipXRL = Math.Max(SlipXFL, 0.0);
					SlipXRR = Math.Max(SlipXFL, 0.0);
					break;
				case GameId.WRC23:
					SuspensionDistFL = Raw("SessionUpdate.vehicle_hub_position_fl");
					SuspensionDistFR = Raw("SessionUpdate.vehicle_hub_position_fr");
					SuspensionDistRL = Raw("SessionUpdate.vehicle_hub_position_bl");
					SuspensionDistRR = Raw("SessionUpdate.vehicle_hub_position_br");
					SpeedMs = Raw("SessionUpdate.vehicle_speed");
					InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
					WheelSpeedFL = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_fl"));
					WheelSpeedFR = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_fr"));
					WheelSpeedRL = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_bl"));
					WheelSpeedRR = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_br"));
					SlipFromWheelSpeed();
					VelocityX = Raw("SessionUpdateLocalVelocity.X");
					YawRate = data.NewData.OrientationYawAcceleration;
					if (VelocityX < 0.0)
					{
						if (YawRate < 0.0)
						{
							SlipXFL = -VelocityX * WheelLoadFL * YawRate * 0.5;
							SlipXFR = -VelocityX * WheelLoadFR * YawRate * 0.5;
							SlipXRL = -VelocityX * WheelLoadRL * YawRate * 1.0;
							SlipXRR = -VelocityX * WheelLoadRR * YawRate * 1.0;
						}
						else
						{
							SlipXFL = -VelocityX * WheelLoadFL * YawRate * 1.0;
							SlipXFR = -VelocityX * WheelLoadFR * YawRate * 1.0;
							SlipXRL = -VelocityX * WheelLoadRL * YawRate * 0.5;
							SlipXRR = -VelocityX * WheelLoadRR * YawRate * 0.5;
						}
					}
					else if (YawRate < 0.0)
					{
						SlipXFL = VelocityX * WheelLoadFL * -YawRate * 1.0;
						SlipXFR = VelocityX * WheelLoadFR * -YawRate * 1.0;
						SlipXRL = VelocityX * WheelLoadRL * -YawRate * 0.5;
						SlipXRR = VelocityX * WheelLoadRR * -YawRate * 0.5;
					}
					else
					{
						SlipXFL = VelocityX * WheelLoadFL * -YawRate * 0.5;
						SlipXFR = VelocityX * WheelLoadFR * -YawRate * 0.5;
						SlipXRL = VelocityX * WheelLoadRL * -YawRate * 1.0;
						SlipXRR = VelocityX * WheelLoadRR * -YawRate * 1.0;
					}
					SlipXFL = Math.Max(SlipXFL, 0.0);
					SlipXFR = Math.Max(SlipXFL, 0.0);
					SlipXRL = Math.Max(SlipXFL, 0.0);
					SlipXRR = Math.Max(SlipXFL, 0.0);
					break;
				case GameId.F12022:
					SuspensionDistFL = Raw("PlayerMotionData.m_suspensionPosition01") * 0.001;
					SuspensionDistFR = Raw("PlayerMotionData.m_suspensionPosition02") * 0.001;
					SuspensionDistRL = Raw("PlayerMotionData.m_suspensionPosition03") * 0.001;
					SuspensionDistRR = Raw("PlayerMotionData.m_suspensionPosition04") * 0.001;
					WheelSpeedFL = Math.Abs(Raw("PlayerMotionData.m_wheelSpeed03"));
					WheelSpeedFR = Math.Abs(Raw("PlayerMotionData.m_wheelSpeed04"));
					WheelSpeedRL = Math.Abs(Raw("PlayerMotionData.m_wheelSpeed01"));
					WheelSpeedRR = Math.Abs(Raw("PlayerMotionData.m_wheelSpeed02"));
					SlipFromWheelSpeed();
					SlipXFL = Math.Max(Raw("PlayerMotionData.m_wheelSlip03") - Math.Abs(SlipYFL) * 2.0, 0.0);
					SlipXFR = Math.Max(Raw("PlayerMotionData.m_wheelSlip04") - Math.Abs(SlipYFR) * 2.0, 0.0);
					SlipXRL = Math.Max(Raw("PlayerMotionData.m_wheelSlip01") - Math.Abs(SlipYRL) * 2.0, 0.0);
					SlipXRR = Math.Max(Raw("PlayerMotionData.m_wheelSlip02") - Math.Abs(SlipYRR) * 2.0, 0.0);
					break;
				case GameId.F12023:
					SuspensionDistFL = Raw("PacketMotionExData.m_suspensionPosition01") * 0.001;
					SuspensionDistFR = Raw("PacketMotionExData.m_suspensionPosition02") * 0.001;
					SuspensionDistRL = Raw("PacketMotionExData.m_suspensionPosition03") * 0.001;
					SuspensionDistRR = Raw("PacketMotionExData.m_suspensionPosition04") * 0.001;
					WheelSpeedFL = Math.Abs(Raw("PacketMotionExData.m_wheelSpeed03"));
					WheelSpeedFR = Math.Abs(Raw("PacketMotionExData.m_wheelSpeed04"));
					WheelSpeedRL = Math.Abs(Raw("PacketMotionExData.m_wheelSpeed01"));
					WheelSpeedRR = Math.Abs(Raw("PacketMotionExData.m_wheelSpeed02"));
					SlipFromWheelSpeed();
					SlipXFL = Math.Max((double) Math.Abs(Raw("PacketMotionExData.m_wheelSlipRatio01")) * 5.0 - Math.Abs(SlipYFL) * 1.0, 0.0);
					SlipXFR = Math.Max((double) Math.Abs(Raw("PacketMotionExData.m_wheelSlipRatio02")) * 5.0 - Math.Abs(SlipYFR) * 1.0, 0.0);
					SlipXRL = Math.Max((double) Math.Abs(Raw("PacketMotionExData.m_wheelSlipRatio03")) * 5.0 - Math.Abs(SlipYRL) * 1.0, 0.0);
					SlipXRR = Math.Max((double) Math.Abs(Raw("PacketMotionExData.m_wheelSlipRatio04")) * 5.0 - Math.Abs(SlipYRR) * 1.0, 0.0);
					break;
				case GameId.Forza:
					SuspensionDistFL = Raw("SuspensionTravelMetersFrontLeft");
					SuspensionDistFR = Raw("SuspensionTravelMetersFrontRight");
					SuspensionDistRL = Raw("SuspensionTravelMetersRearLeft");
					SuspensionDistRR = Raw("SuspensionTravelMetersRearRight");
					WheelRotationFL = Math.Abs(Raw("WheelRotationSpeedFrontLeft"));
					WheelRotationFR = Math.Abs(Raw("WheelRotationSpeedFrontRight"));
					WheelRotationRL = Math.Abs(Raw("WheelRotationSpeedRearLeft"));
					WheelRotationRR = Math.Abs(Raw("WheelRotationSpeedRearRight"));
					SlipFromRPS();
					SlipXFL = Math.Max(Raw("TireCombinedSlipFrontLeft") - Math.Abs(SlipYFL) * 2.0, 0.0);
					SlipXFR = Math.Max(Raw("TireCombinedSlipFrontRight") - Math.Abs(SlipYFR) * 2.0, 0.0);
					SlipXRL = Math.Max(Raw("TireCombinedSlipRearLeft") - Math.Abs(SlipYRL) * 2.0, 0.0);
					SlipXRR = Math.Max(Raw("TireCombinedSlipRearRight") - Math.Abs(SlipYRR) * 2.0, 0.0);
					if (TireDiameterFL == 0.0)
					{
						SlipXFL *= 0.5;
						SlipXFR *= 0.5;
						SlipXRL *= 0.5;
						SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.GTR2:
				case GameId.GSCE:
				case GameId.RF1:
					SuspensionDistFL = Data("wheel01.suspensionDeflection");
					SuspensionDistFR = Data("wheel02.suspensionDeflection");
					SuspensionDistRL = Data("wheel03.suspensionDeflection");
					SuspensionDistRR = Data("wheel04.suspensionDeflection");
					SpeedMs = Raw("CurrentPlayer.speed");
					InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
					WheelRotationFL = Math.Abs(Data("wheel01.rotation"));
					WheelRotationFR = Math.Abs(Data("wheel02.rotation"));
					WheelRotationRL = Math.Abs(Data("wheel03.rotation"));
					WheelRotationRR = Math.Abs(Data("wheel04.rotation"));
					SlipFromRPS();
					SlipXFL = Math.Max(1.0 - Data("wheel01.gripFract") - Math.Abs(SlipYFL) * 1.0, 0.0);
					SlipXFR = Math.Max(1.0 - Data("wheel02.gripFract") - Math.Abs(SlipYFR) * 1.0, 0.0);
					SlipXRL = Math.Max(1.0 - Data("wheel03.gripFract") - Math.Abs(SlipYRL) * 1.0, 0.0);
					SlipXRR = Math.Max(1.0 - Data("wheel04.gripFract") - Math.Abs(SlipYRR) * 1.0, 0.0);
					if (TireDiameterFL == 0.0)
					{
						SlipXFL *= 0.5;
						SlipXFR *= 0.5;
						SlipXRL *= 0.5;
						SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.IRacing:
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFshockDefl") != null)
					{
						SuspensionDistFL = Raw("Telemetry.LFshockDefl");
						SuspensionDistFR = Raw("Telemetry.RFshockDefl");
					}
					else if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LFSHshockDefl") != null)
					{
						SuspensionDistFL = Raw("Telemetry.LFSHshockDefl");
						SuspensionDistFR = Raw("Telemetry.RFSHshockDefl");
					}
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRshockDefl") != null)
					{
						SuspensionDistRL = Raw("Telemetry.LRshockDefl");
						SuspensionDistRR = Raw("Telemetry.RRshockDefl");
					}
					else if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.LRSHshockDefl") != null)
					{
						SuspensionDistRL = Raw("Telemetry.LRSHshockDefl");
						SuspensionDistRR = Raw("Telemetry.RRSHshockDefl");
					}
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CFshockDefl") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Telemetry.CFshockDefl");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Telemetry.CFshockDefl");
					}
					else if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.HFshockDefl") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Telemetry.HFshockDefl");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Telemetry.HFshockDefl");
					}
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Telemetry.CRshockDefl") != null)
					{
						SuspensionDistRL = 0.5 * SuspensionDistRL + Raw("Telemetry.CRshockDefl");
						SuspensionDistRR = 0.5 * SuspensionDistRR + Raw("Telemetry.CRshockDefl");
					}
					VelocityX = Raw("Telemetry.VelocityY") * 10.0;
					if (VelocityX < 0.0)
					{
						if (YawRate < 0.0)
						{
							SlipXFL = -VelocityX * WheelLoadFL * YawRate * 0.1;
							SlipXFR = -VelocityX * WheelLoadFR * YawRate * 0.1;
							SlipXRL = -VelocityX * WheelLoadRL * YawRate * 0.2;
							SlipXRR = -VelocityX * WheelLoadRR * YawRate * 0.2;
						}
						else
						{
							SlipXFL = -VelocityX * WheelLoadFL * YawRate * 0.2;
							SlipXFR = -VelocityX * WheelLoadFR * YawRate * 0.2;
							SlipXRL = -VelocityX * WheelLoadRL * YawRate * 0.1;
							SlipXRR = -VelocityX * WheelLoadRR * YawRate * 0.1;
						}
					}
					else if (YawRate < 0.0)
					{
						SlipXFL = VelocityX * WheelLoadFL * -YawRate * 0.2;
						SlipXFR = VelocityX * WheelLoadFR * -YawRate * 0.2;
						SlipXRL = VelocityX * WheelLoadRL * -YawRate * 0.1;
						SlipXRR = VelocityX * WheelLoadRR * -YawRate * 0.1;
					}
					else
					{
						SlipXFL = VelocityX * WheelLoadFL * -YawRate * 0.1;
						SlipXFR = VelocityX * WheelLoadFR * -YawRate * 0.1;
						SlipXRL = VelocityX * WheelLoadRL * -YawRate * 0.2;
						SlipXRR = VelocityX * WheelLoadRR * -YawRate * 0.2;
					}
					if (Brake > 0.0)
					{
						SlipYFL = BrakeF * SpeedMs * WheelLoadFL * InvAccSurgeAvg * 0.04;
						SlipYFR = BrakeF * SpeedMs * WheelLoadFR * InvAccSurgeAvg * 0.04;
						SlipYRL = (BrakeR + Handbrake) * SpeedMs * WheelLoadRL * InvAccSurgeAvg * 0.04;
						SlipYRR = (BrakeR + Handbrake) * SpeedMs * WheelLoadRR * InvAccSurgeAvg * 0.04;
					}
					else if (Accelerator > 10.0 && SpeedMs > 0.0 && AccSurgeAvg < 0.0)
					{
						if (SHP.S.PoweredWheels == "F")
						{
							SlipYFL = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.2;
							SlipYFR = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.2;
							SlipYRL = 0.0;
							SlipYRR = 0.0;
						}
						else if (SHP.S.PoweredWheels == "R")
						{
							SlipYFL = 0.0;
							SlipYFR = 0.0;
							SlipYRL = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.2;
							SlipYRR = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.2;
						}
						else
						{
							SlipYFL = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.15;
							SlipYFR = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.15;
							SlipYRL = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.15;
							SlipYRR = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.15;
						}
					}
					SlipXFL = 0.0;
					SlipXFR = 0.0;
					SlipXRL = 0.0;
					SlipXRR = 0.0;
					SlipYFL = 0.0;
					SlipYFR = 0.0;
					SlipYRL = 0.0;
					SlipYRR = 0.0;
					break;
				case GameId.PC2:
					SuspensionDistFL = Raw("mSuspensionTravel01");
					SuspensionDistFR = Raw("mSuspensionTravel02");
					SuspensionDistRL = Raw("mSuspensionTravel03");
					SuspensionDistRR = Raw("mSuspensionTravel04");
					WheelRotationFL = Math.Abs(Raw("mTyreRPS01"));
					WheelRotationFR = Math.Abs(Raw("mTyreRPS02"));
					WheelRotationRL = Math.Abs(Raw("mTyreRPS03"));
					WheelRotationRR = Math.Abs(Raw("mTyreRPS04"));
					SlipFromRPS();
					SlipXFL = Math.Max(Raw("mTyreSlipSpeed01") * 0.1 - Math.Abs(SlipYFL) * 1.0, 0.0);
					SlipXFR = Math.Max(Raw("mTyreSlipSpeed02") * 0.1 - Math.Abs(SlipYFR) * 1.0, 0.0);
					SlipXRL = Math.Max(Raw("mTyreSlipSpeed03") * 0.1 - Math.Abs(SlipYRL) * 1.0, 0.0);
					SlipXRR = Math.Max(Raw("mTyreSlipSpeed04") * 0.1 - Math.Abs(SlipYRR) * 1.0, 0.0);
					if (TireDiameterFL == 0.0)
					{
						SlipXFL *= 0.5;
						SlipXFR *= 0.5;
						SlipXRL *= 0.5;
						SlipXRR *= 0.5;
						break;
					}
					break;
				case GameId.RBR:
					SuspensionDistFL = Raw("NGPTelemetry.car.suspensionLF.springDeflection");
					SuspensionDistFR = Raw("NGPTelemetry.car.suspensionRF.springDeflection");
					SuspensionDistRL = Raw("NGPTelemetry.car.suspensionLB.springDeflection");
					SuspensionDistRR = Raw("NGPTelemetry.car.suspensionRB.springDeflection");
					WheelSpeedFL = Math.Abs(Raw("WheelSpeedFL"));
					WheelSpeedFR = Math.Abs(Raw("WheelSpeedFR"));
					WheelSpeedRL = Math.Abs(Raw("WheelSpeedRL"));
					WheelSpeedRR = Math.Abs(Raw("WheelSpeedRR"));
					SlipFromWheelSpeed();
					break;
				case GameId.RF2:
					SuspensionDistFL = Raw("CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					SuspensionDistFR = Raw("CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					SuspensionDistRL = Raw("CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					SuspensionDistRR = Raw("CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
					WheelRotationFL = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels01.mRotation"));
					WheelRotationFR = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels02.mRotation"));
					WheelRotationRL = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels03.mRotation"));
					WheelRotationRR = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels04.mRotation"));
					SlipFromRPS();
					SlipXFL = Raw("CurrentPlayerTelemetry.mWheels01.mLateralGroundVel");
					SlipXFL = SlipXFL == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels01.mLateralPatchVel") / SlipXFL;
					SlipXFR = Raw("CurrentPlayerTelemetry.mWheels02.mLateralGroundVel");
					SlipXFR = SlipXFR == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels02.mLateralPatchVel") / SlipXFR;
					SlipXRL = Raw("CurrentPlayerTelemetry.mWheels03.mLateralGroundVel");
					SlipXRL = SlipXRL == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels03.mLateralPatchVel") / SlipXRL;
					SlipXRR = Raw("CurrentPlayerTelemetry.mWheels04.mLateralGroundVel");
					SlipXRR = SlipXRR == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels04.mLateralPatchVel") / SlipXRR;
					SlipXFL *= 0.5;
					SlipXFR *= 0.5;
					SlipXRL *= 0.5;
					SlipXRR *= 0.5;
					if (data.NewData.Brake > 90.0)
					{
						ABSActive = (Raw("CurrentPlayerTelemetry.mWheels01.mBrakePressure")
								   + Raw("CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < data.NewData.Brake - 1.0;
						break;
					}
					break;
				case GameId.RRRE:
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.FrontLeft") != null)
					{
						SuspensionDistFL = Raw("Player.SuspensionDeflection.FrontLeft");
						SuspensionDistFR = Raw("Player.SuspensionDeflection.FrontRight");
					}
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Player.SuspensionDeflection.RearLeft") != null)
					{
						SuspensionDistRL = Raw("Player.SuspensionDeflection.RearLeft");
						SuspensionDistRR = Raw("Player.SuspensionDeflection.RearRight");
					}
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionFront") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Player.ThirdSpringSuspensionDeflectionFront");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Player.ThirdSpringSuspensionDeflectionFront");
					}
					if (SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.Player.ThirdSpringSuspensionDeflectionRear") != null)
					{
						SuspensionDistRL = 0.5 * SuspensionDistRL + Raw("Player.ThirdSpringSuspensionDeflectionRear");
						SuspensionDistRR = 0.5 * SuspensionDistRR + Raw("Player.ThirdSpringSuspensionDeflectionRear");
					}
					WheelRotationFL = Math.Abs(Raw("TireRps.FrontLeft"));
					WheelRotationFR = Math.Abs(Raw("TireRps.FrontRight"));
					WheelRotationRL = Math.Abs(Raw("TireRps.RearLeft"));
					WheelRotationRR = Math.Abs(Raw("TireRps.RearRight"));
					SlipFromRPS();
					break;
				case GameId.GTL:
				case GameId.RACE07:
					double num1 = Raw("carCGLocY") * 0.2;
					SuspensionDistFL = num1 * WheelLoadFL;
					SuspensionDistFR = num1 * WheelLoadFR;
					SuspensionDistRL = num1 * WheelLoadRL;
					SuspensionDistRR = num1 * WheelLoadRR;
					break;
				case GameId.LFS:
					SuspensionDistFL = Raw("OutSim2.OSWheels01.SuspDeflect");
					SuspensionDistFR = Raw("OutSim2.OSWheels02.SuspDeflect");
					SuspensionDistRL = Raw("OutSim2.OSWheels03.SuspDeflect");
					SuspensionDistRR = Raw("OutSim2.OSWheels04.SuspDeflect");
					break;
				case GameId.WRC10:
				case GameId.WRCX:
					SuspensionDistFL = Raw("suspension_position01");
					SuspensionDistFR = Raw("suspension_position02");
					SuspensionDistRL = Raw("suspension_position03");
					SuspensionDistRR = Raw("suspension_position04");
					break;
				case GameId.WRCGen:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft");
					SuspensionDistFR = Raw("SuspensionPositionFrontRight");
					SuspensionDistRL = Raw("SuspensionPositionRearLeft");
					SuspensionDistRR = Raw("SuspensionPositionRearRight");
					WheelSpeedFL = Math.Abs(Raw("WheelSpeedFrontLeft"));
					WheelSpeedFR = Math.Abs(Raw("WheelSpeedFrontRight"));
					WheelSpeedRL = Math.Abs(Raw("WheelSpeedRearLeft"));
					WheelSpeedRR = Math.Abs(Raw("WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.F12016:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionFrontLeft") * 0.001;
					WheelSpeedFL = Math.Abs(Raw("WheelSpeedFrontLeft"));
					WheelSpeedFR = Math.Abs(Raw("WheelSpeedFrontRight"));
					WheelSpeedRL = Math.Abs(Raw("WheelSpeedRearLeft"));
					WheelSpeedRR = Math.Abs(Raw("WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.F12017:
					SuspensionDistFL = Raw("m_susp_pos01") * 0.001;
					SuspensionDistFR = Raw("m_susp_pos02") * 0.001;
					SuspensionDistRL = Raw("m_susp_pos03") * 0.001;
					SuspensionDistRR = Raw("m_susp_pos04") * 0.001;
					WheelSpeedFL = Math.Abs(Raw("m_wheelSpeed03"));
					WheelSpeedFR = Math.Abs(Raw("m_wheelSpeed04"));
					WheelSpeedRL = Math.Abs(Raw("m_wheelSpeed01"));
					WheelSpeedRR = Math.Abs(Raw("m_wheelSpeed02"));
					SlipFromWheelSpeed();
					break;
				case GameId.GLegends:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
					WheelSpeedFL = Math.Abs(Raw("WheelSpeedFrontLeft"));
					WheelSpeedFR = Math.Abs(Raw("WheelSpeedFrontRight"));
					WheelSpeedRL = Math.Abs(Raw("WheelSpeedRearLeft"));
					WheelSpeedRR = Math.Abs(Raw("WheelSpeedRearRight"));
					SlipFromWheelSpeed();
					break;
				case GameId.KK:
					flag = false;
					double propertyValue = Raw("Motion.VelocityZ");
					if (VelocityZAvg == 0.0)
						VelocityZAvg = propertyValue;
					VelocityZAvg = (VelocityZAvg + propertyValue) * 0.5;
					double num2 = (propertyValue / VelocityZAvg - 1.0) * 0.5;
					SuspensionVelFL = num2 * WheelLoadFL;
					SuspensionVelFR = num2 * WheelLoadFR;
					SuspensionVelRL = num2 * WheelLoadRL;
					SuspensionVelRR = num2 * WheelLoadRR;
					break;
				case GameId.ATS:
				case GameId.ETS2:
					SuspensionDistFL = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection01");
					SuspensionDistFR = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection02");
					SuspensionDistRL = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection03");
					SuspensionDistRR = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection04");
					WiperStatus = (bool) SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.TruckValues.CurrentValues.DashboardValues.Wipers") ? 1 : 0;
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
					WheelSpeedFL = Raw("wheel_speed_fl");
					WheelSpeedFR = Raw("wheel_speed_fr");
					WheelSpeedRL = Raw("wheel_speed_rl");
					WheelSpeedRR = Raw("wheel_speed_rr");
					SlipFromWheelSpeed();
					SlipXFL = Math.Max(Raw("wheel_slip_fl") * 0.1 - Math.Abs(SlipYFL) * 2.0, 0.0);
					SlipXFR = Math.Max(Raw("wheel_slip_fr") * 0.1 - Math.Abs(SlipYFR) * 2.0, 0.0);
					SlipXRL = Math.Max(Raw("wheel_slip_rl") * 0.1 - Math.Abs(SlipYRL) * 2.0, 0.0);
					SlipXRR = Math.Max(Raw("wheel_slip_rr") * 0.1 - Math.Abs(SlipYRR) * 2.0, 0.0);
					break;
				case GameId.GPBikes:
				case GameId.MXBikes:
					flag = false;
					SuspensionDistFL = Raw("m_sData.m_afSuspLength01");
					SuspensionDistFR = Raw("m_sData.m_afSuspLength01");
					SuspensionDistRL = Raw("m_sData.m_afSuspLength02");
					SuspensionDistRR = Raw("m_sData.m_afSuspLength02");
					SuspensionVelFL = Raw("m_sData.m_afSuspVelocity01");
					SuspensionVelFR = Raw("m_sData.m_afSuspVelocity01");
					SuspensionVelRL = Raw("m_sData.m_afSuspVelocity02");
					SuspensionVelRR = Raw("m_sData.m_afSuspVelocity02");
					WheelSpeedFL = Math.Abs(Raw("m_sData.m_afWheelSpeed01"));
					WheelSpeedFR = Math.Abs(Raw("m_sData.m_afWheelSpeed01"));
					WheelSpeedRL = Math.Abs(Raw("m_sData.m_afWheelSpeed02"));
					WheelSpeedRR = Math.Abs(Raw("m_sData.m_afWheelSpeed02"));
					SlipFromWheelSpeed();
					if ((int) SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_aiWheelMaterial01") == 7
					 || (int) SHP.PM.GetPropertyValue("DataCorePlugin.GameRawData.m_sData.m_aiWheelMaterial02") == 7)
					{
						RumbleLeft = 50.0;
						RumbleRight = 50.0;
						break;
					}
					RumbleLeft = 0.0;
					RumbleRight = 0.0;
					break;
				case GameId.LMU:
					SuspensionDistFL = Raw("CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					SuspensionDistFR = Raw("CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					SuspensionDistRL = Raw("CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					SuspensionDistRR = Raw("CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
					WheelRotationFL = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels01.mRotation"));
					WheelRotationFR = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels02.mRotation"));
					WheelRotationRL = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels03.mRotation"));
					WheelRotationRR = Math.Abs(Raw("CurrentPlayerTelemetry.mWheels04.mRotation"));
					SlipFromRPS();
					SlipXFL = Raw("CurrentPlayerTelemetry.mWheels01.mLateralGroundVel");
					SlipXFL = SlipXFL == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels01.mLateralPatchVel") / SlipXFL;
					SlipXFR = Raw("CurrentPlayerTelemetry.mWheels02.mLateralGroundVel");
					SlipXFR = SlipXFR == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels02.mLateralPatchVel") / SlipXFR;
					SlipXRL = Raw("CurrentPlayerTelemetry.mWheels03.mLateralGroundVel");
					SlipXRL = SlipXRL == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels03.mLateralPatchVel") / SlipXRL;
					SlipXRR = Raw("CurrentPlayerTelemetry.mWheels04.mLateralGroundVel");
					SlipXRR = SlipXRR == 0.0 ? 0.0 : Raw("CurrentPlayerTelemetry.mWheels04.mLateralPatchVel") / SlipXRR;
					SlipXFL = Math.Abs(SlipXFL - 1.0);
					SlipXFR = Math.Abs(SlipXFR - 1.0);
					SlipXRL = Math.Abs(SlipXRL - 1.0);
					SlipXRR = Math.Abs(SlipXRR - 1.0);
					if (data.NewData.Brake > 80.0)
					{
						ABSActive = (Raw("CurrentPlayerTelemetry.mWheels01.mBrakePressure")
								   + Raw("CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < data.NewData.Brake - 1.0;
						break;
					}
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					SuspensionDistFL = Raw("Tire_SusHeight01");
					SuspensionDistFR = Raw("Tire_SusHeight02");
					SuspensionDistRL = Raw("Tire_SusHeight03");
					SuspensionDistRR = Raw("Tire_SusHeight04");
					WheelSpeedFL = (double) Math.Abs(Raw("Wheel_Speed01")) * 0.277778;
					WheelSpeedFR = (double) Math.Abs(Raw("Wheel_Speed02")) * 0.277778;
					WheelSpeedRL = (double) Math.Abs(Raw("Wheel_Speed03")) * 0.277778;
					WheelSpeedRR = (double) Math.Abs(Raw("Wheel_Speed04")) * 0.277778;
					SlipFromWheelSpeed();
					SlipXFL = Math.Max(Raw("Wheel_Slip01") - Math.Abs(SlipYFL) * 2.0, 0.0);
					SlipXFR = Math.Max(Raw("Wheel_Slip02") - Math.Abs(SlipYFR) * 2.0, 0.0);
					SlipXRL = Math.Max(Raw("Wheel_Slip03") - Math.Abs(SlipYRL) * 2.0, 0.0);
					SlipXRR = Math.Max(Raw("Wheel_Slip04") - Math.Abs(SlipYRR) * 2.0, 0.0);
					break;
			}
			if (!flag)
				return;
			SuspensionVelFL = (SuspensionDistFL - SuspensionDistFLP) * FPS;
			SuspensionVelFR = (SuspensionDistFR - SuspensionDistFRP) * FPS;
			SuspensionVelRL = (SuspensionDistRL - SuspensionDistRLP) * FPS;
			SuspensionVelRR = (SuspensionDistRR - SuspensionDistRRP) * FPS;
		}

		private void SlipFromRPS()
		{
			if (TireDiameterSampleCount < TireDiameterSampleMax
			 && Accelerator < 60.0 && SpeedMs > 5.0 && Math.Abs(AccHeave2S) < 0.1
			 && Math.Abs(AccSurge2S) < 0.01 && Math.Abs(AccSway2S) < 0.08)
			{
				TireDiameterSampleFL = 2.0 * SpeedMs / WheelRotationFL;
				TireDiameterSampleFR = 2.0 * SpeedMs / WheelRotationFR;
				TireDiameterSampleRL = 2.0 * SpeedMs / WheelRotationRL;
				TireDiameterSampleRR = 2.0 * SpeedMs / WheelRotationRR;
				if (TireDiameterSampleFL > 1.0)
				{
					if (TireDiameterSampleFL > 3.0)
						TireDiameterSampleFL = 0.66;
					else if (TireDiameterSampleCount > 0 && Math.Abs(TireDiameterSampleFL - TireDiameterFL) > 0.2 * TireDiameterFL)
						TireDiameterSampleFL = TireDiameterFL * 0.9 + TireDiameterSampleFL * 0.1;
				}
				if (TireDiameterSampleFR > 1.0)
				{
					if (TireDiameterSampleFR > 3.0)
						TireDiameterSampleFR = 0.66;
					else if (TireDiameterSampleCount > 0 && Math.Abs(TireDiameterSampleFR - TireDiameterFR) > 0.2 * TireDiameterFR)
						TireDiameterSampleFR = TireDiameterFR * 0.9 + TireDiameterSampleFR * 0.1;
				}
				if (TireDiameterSampleRL > 1.0)
				{
					if (TireDiameterSampleRL > 3.0)
						TireDiameterSampleRL = 0.66;
					else if (TireDiameterSampleCount > 0 && Math.Abs(TireDiameterSampleRL - TireDiameterRL) > 0.2 * TireDiameterRL)
						TireDiameterSampleRL = TireDiameterRL * 0.9 + TireDiameterSampleRL * 0.1;
				}
				if (TireDiameterSampleRR > 1.0)
				{
					if (TireDiameterSampleRR > 3.0)
						TireDiameterSampleRR = 0.66;
					else if (TireDiameterSampleCount > 0 && Math.Abs(TireDiameterSampleRR - TireDiameterRR) > 0.2 * TireDiameterRR)
						TireDiameterSampleRR = TireDiameterRR * 0.9 + TireDiameterSampleRR * 0.1;
				}
				double num1 = Math.Min(Math.Abs(YawRate) * 0.1, 1.0);
				if (YawRate < 0.0)
					TireDiameterSampleFR = TireDiameterSampleFL * num1 + TireDiameterSampleFR * (1.0 - num1);
				else
					TireDiameterSampleFL = TireDiameterSampleFR * num1 + TireDiameterSampleFL * (1.0 - num1);
				if (TireDiameterSampleCount == 0)
				{
					TireDiameterFL = TireDiameterSampleFL;
					TireDiameterFR = TireDiameterSampleFR;
					TireDiameterRL = TireDiameterSampleRL;
					TireDiameterRR = TireDiameterSampleRR;
				}
				else
				{
					double num2 = (0.5 * TireDiameterSampleMax + TireDiameterSampleCount) / (2 * TireDiameterSampleMax);
					TireDiameterFL = TireDiameterFL * num2 + TireDiameterSampleFL * (1.0 - num2);
					TireDiameterFR = TireDiameterFR * num2 + TireDiameterSampleFR * (1.0 - num2);
					TireDiameterRL = TireDiameterRL * num2 + TireDiameterSampleRL * (1.0 - num2);
					TireDiameterRR = TireDiameterRR * num2 + TireDiameterSampleRR * (1.0 - num2);
				}
				++TireDiameterSampleCount;
				if (TireDiameterSampleCount == TireDiameterSampleMax)
				{
					if (Math.Abs(TireDiameterFL - TireDiameterFR) < 0.1)
					{
						TireDiameterFL = (TireDiameterFL + TireDiameterFR) * 0.5;
						TireDiameterFR = TireDiameterFL;
					}
					if (Math.Abs(TireDiameterRL - TireDiameterRR) < 0.1)
					{
						TireDiameterRL = (TireDiameterRL + TireDiameterRR) * 0.5;
						TireDiameterRR = TireDiameterRL;
					}
				}
			}
			if (TireDiameterFL <= 0.0)
				return;
			WheelSpeedFL = TireDiameterFL * WheelRotationFL * 0.5;
			WheelSpeedFR = TireDiameterFR * WheelRotationFR * 0.5;
			WheelSpeedRL = TireDiameterRL * WheelRotationRL * 0.5;
			WheelSpeedRR = TireDiameterRR * WheelRotationRR * 0.5;
			SlipFromWheelSpeed();
		}

		private void SlipFromWheelSpeed()
		{
			if (SpeedMs <= 0.05)
				return;
			SlipYFL = (SpeedMs - WheelSpeedFL) * InvSpeedMs;
			SlipYFR = (SpeedMs - WheelSpeedFR) * InvSpeedMs;
			SlipYRL = (SpeedMs - WheelSpeedRL) * InvSpeedMs;
			SlipYRR = (SpeedMs - WheelSpeedRR) * InvSpeedMs;
			if (SpeedMs >= 3.0)
				return;
			SlipYFL *= SpeedMs * 0.333;
			SlipYFR *= SpeedMs * 0.333;
			SlipYRL *= SpeedMs * 0.333;
			SlipYRR *= SpeedMs * 0.333;
		}

		// called from DataUpdate()
		internal void Refresh(ref GameData Gdat, SimHap shp)
		{
			SHP = shp;
			data = Gdat;
			FPS = (double) SHP.PM.GetPropertyValue("DataCorePlugin.DataUpdateFps");
			RPMPercent = data.NewData.Rpms * InvMaxRPM;
			SpeedMs = data.NewData.SpeedKmh * 0.277778;
			InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
			Accelerator = data.NewData.Throttle;
			Brake = data.NewData.Brake;
			Clutch = data.NewData.Clutch;
			Handbrake = data.NewData.Handbrake;
			BrakeBias = data.NewData.BrakeBias;
			BrakeF = Brake * (2.0 * BrakeBias) * 0.01;
			BrakeR = Brake * (200.0 - 2.0 * BrakeBias) * 0.01;
			BrakeVelP = BrakeVel;
			BrakeVel = (Brake - data.OldData.Brake) * FPS;
			BrakeAcc = (BrakeVel - BrakeVelP) * FPS;
			if (CarInitCount < 2)
			{
				SuspensionDistFLP = SuspensionDistFL;
				SuspensionDistFRP = SuspensionDistFR;
				SuspensionDistRLP = SuspensionDistRL;
				SuspensionDistRRP = SuspensionDistRR;
				YawPrev = data.NewData.OrientationYaw;
				Yaw = data.NewData.OrientationYaw;
				RumbleLeftAvg = 0.0;
				RumbleRightAvg = 0.0;
			}
			YawPrev = Yaw;
			Yaw = -data.NewData.OrientationYaw;
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
			AccHeave[Acc0] = data.NewData.AccelerationHeave.GetValueOrDefault();
			AccSurge[Acc0] = data.NewData.AccelerationSurge.GetValueOrDefault();
			AccSway[Acc0] = data.NewData.AccelerationSway.GetValueOrDefault();
			if (!data.NewData.AccelerationHeave.HasValue)
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
			MotionPitch = MotionPitchOffset + 100.0 * Math.Pow(Math.Abs(MotionPitchMult * data.NewData.OrientationPitch) * 0.01, 1.0 / MotionPitchGamma);
			MotionRoll = MotionRollOffset + 100.0 * Math.Pow(Math.Abs(MotionRollMult * data.NewData.OrientationRoll) * 0.01, 1.0 / MotionRollGamma);
			MotionYaw = MotionYawOffset + 100.0 * Math.Pow(Math.Abs(MotionYawMult * YawRateAvg) * 0.01, 1.0 / MotionYawGamma);
			MotionHeave = MotionHeaveOffset + 100.0 * Math.Pow(Math.Abs(MotionHeaveMult * AccHeave[Acc0]) * 0.01, 1.0 / MotionHeaveGamma);
			if (data.NewData.OrientationPitch < 0.0)
				MotionPitch = -MotionPitch;
			if (data.NewData.OrientationRoll < 0.0)
				MotionRoll = -MotionRoll;
			if (YawRateAvg < 0.0)
				MotionYaw = -MotionYaw;
			if (AccHeave[Acc0] < 0.0)
				MotionHeave = -MotionHeave;
			WheelLoadFL = ((100.0 + AccSurge[Acc0]) * (100.0 - AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadFR = ((100.0 + AccSurge[Acc0]) * (100.0 + AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadRL = ((100.0 - AccSurge[Acc0]) * (100.0 - AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadRR = ((100.0 - AccSurge[Acc0]) * (100.0 + AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			UpdateVehicle(ref data);
			Airborne = AccHeave2S < -2.0 || Math.Abs(data.NewData.OrientationRoll) > 60.0;
			if (Airborne && SuspensionFL < 0.1)
				SlipXFL = SlipYFL = 0.0;
			else
			{
				SlipXFL = SlipXMultAll * 100.0 * Math.Pow(Math.Max(SlipXFL, 0.0), 1.0 / (SlipXGammaBaseMult * SlipXGamma * SlipXGammaAll));
				SlipYFL = SlipYFL < 0.0
							 ? SlipYMultAll * -100.0 * Math.Pow(-SlipYFL, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll))
							 : SlipYMultAll * 100.0 * Math.Pow(SlipYFL, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll));
			}
			if (Airborne && SuspensionFR < 0.1)
				SlipXFR = SlipYFR = 0.0;
			else
			{
				SlipXFR = SlipXMultAll * 100.0 * Math.Pow(Math.Max(SlipXFR, 0.0), 1.0 / (SlipXGammaBaseMult * SlipXGamma * SlipXGammaAll));
				SlipYFR = SlipYFR < 0.0
							 ? SlipYMultAll * -100.0 * Math.Pow(-SlipYFR, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll))
							 : SlipYMultAll * 100.0 * Math.Pow(SlipYFR, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll));
			}
			if (Airborne && SuspensionRL < 0.1)
				SlipXRL = SlipYRL = 0.0;
			else
			{
				SlipXRL = SlipXMultAll * 100.0 * Math.Pow(Math.Max(SlipXRL, 0.0), 1.0 / (SlipXGammaBaseMult * SlipXGamma * SlipXGammaAll));
				SlipYRL = SlipYRL < 0.0
							 ? SlipYMultAll * -100.0 * Math.Pow(-SlipYRL, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll))
							 : SlipYMultAll * 100.0 * Math.Pow(SlipYRL, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll));
			}
			if (Airborne && SuspensionRR < 0.1)
				SlipXRR = SlipYRR = 0.0;
			else
			{
				SlipXRR = SlipXMultAll * 100.0 * Math.Pow(Math.Max(SlipXRR, 0.0), 1.0 / (SlipXGammaBaseMult * SlipXGamma * SlipXGammaAll));
				SlipYRR = SlipYRR < 0.0
							 ? SlipYMultAll * -100.0 * Math.Pow(-SlipYRR, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll))
							 : SlipYMultAll * 100.0 * Math.Pow(SlipYRR, 1.0 / (SlipYGammaBaseMult * SlipYGamma * SlipYGammaAll));
			}
			Airborne = Airborne && SuspensionAll < 0.1;
			SlipXAll = (SlipXFL + SlipXFR + SlipXRL + SlipXRR) * 0.5;
			SlipYAll = (SlipYFL + SlipYFR + SlipYRL + SlipYRR) * 0.5;
			WheelSpinAll = !(SHP.S.PoweredWheels == "F")
				? (!(SHP.S.PoweredWheels == "R")
					? (Math.Max(-SlipYFL, 0.0) + Math.Max(-SlipYFR, 0.0) + Math.Max(-SlipYRL, 0.0) + Math.Max(-SlipYRR, 0.0)) * 0.25
					: (Math.Max(-SlipYRL, 0.0) + Math.Max(-SlipYRR, 0.0)) * 0.5)
				: (Math.Max(-SlipYFL, 0.0) + Math.Max(-SlipYFR, 0.0)) * 0.5;
			WheelLockAll = 0.0;
			if (SlipYFL > 50.0)
				WheelLockAll += SlipYFL - 50.0;
			if (SlipYFR > 50.0)
				WheelLockAll += SlipYFR - 50.0;
			if (SlipYRL > 50.0)
				WheelLockAll += SlipYRL - 50.0;
			if (SlipYRR > 50.0)
				WheelLockAll += SlipYRR - 50.0;

			if (DateTime.Now.Ticks < FrameTimeTicks)	// long rollover?
				FrameCountTicks += (long.MaxValue - FrameTimeTicks) + DateTime.Now.Ticks;	// rollover
			else FrameCountTicks += DateTime.Now.Ticks - FrameTimeTicks;
			FrameTimeTicks = DateTime.Now.Ticks;
			if (FrameCountTicks > 864000000000L)
				FrameCountTicks = 0L;

			if (DateTime.Now.Ticks < ShiftTicks)	// long rollover?
				ShiftTicks = - (DateTime.Now.Ticks + (long.MaxValue - ShiftTicks));
			if (DateTime.Now.Ticks - ShiftTicks > SHP.Settings.DownshiftDurationMs * 10000)
				Downshift = false;
			if (DateTime.Now.Ticks - ShiftTicks > SHP.Settings.UpshiftDurationMs * 10000)
				Upshift = false;
			DateTime now;
			if (data.OldData.Gear != data.NewData.Gear)
			{
				if (Gear != 0)
					GearPrevious = Gear;
				Gear = !(data.NewData.Gear == "N")
						? (!(data.NewData.Gear == "R")
							? Convert.ToInt32(data.NewData.Gear)
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
			ABSPauseInterval = SlipYAll <= 0.0
								? (long) (1166667.0 - 666667.0 * ((data.NewData.SpeedKmh - 20.0) * 0.003333333).Clamp(0.0, 1.0))
								: (long) (1250000.0 - 666667.0 * SlipYAll.Clamp(0.0, 1.0));
			ABSPulseInterval = 166666L * SHP.Settings.ABSPulseLength;
			if (ABSActive)
			{
				now = DateTime.Now;
				if (ABSTicks <= 0L)
					ABSTicks = now.Ticks;
				if (now.Ticks - ABSTicks < ABSPulseInterval)
					ABSPulse = 100.0;
				else if (now.Ticks - ABSTicks < ABSPauseInterval)
					ABSPulse = 0.0;
				else
				{
					ABSPulse = 100.0;
					ABSTicks = now.Ticks;
				}
			}
			else
			{
				ABSPulse = 0.0;
				ABSTicks = -1L;
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
			SuspensionFreq = data.NewData.SpeedKmh * (3.0 / 16.0);
			double num2 = 94.0 + 0.4 * SpeedMs;
			double num3 = 76.0 + 0.45 * SpeedMs;
			double num4 = 60.0 + 0.5 * SpeedMs;
			double num5 = 46.0 + 0.55 * SpeedMs;
			double num6 = 34.0 + 0.6 * SpeedMs;
			double num7 = 24.0 + 0.65 * SpeedMs;
			double num8 = 16.0 + 0.7 * SpeedMs;
			double num9 = 10.0 + 0.75 * SpeedMs;
			double num10 = 0.55 + 1.8 * AccHeaveAbs * (AccHeaveAbs + num2) / (num2 * num2);
			double num11 = 0.5 + 2.0 * AccHeaveAbs * (AccHeaveAbs + num3) / (num3 * num3);
			double num12 = 0.45 + 2.2 * AccHeaveAbs * (AccHeaveAbs + num4) / (num4 * num4);
			double num13 = 0.4 + 2.4 * AccHeaveAbs * (AccHeaveAbs + num5) / (num5 * num5);
			double num14 = 0.5 + 2.0 * AccHeaveAbs * (AccHeaveAbs + num6) / (num6 * num6);
			double num15 = 0.6 + 1.6 * AccHeaveAbs * (AccHeaveAbs + num7) / (num7 * num7);
			double num16 = 0.7 + 1.2 * AccHeaveAbs * (AccHeaveAbs + num8) / (num8 * num8);
			double num17 = 0.8 + 0.8 * AccHeaveAbs * (AccHeaveAbs + num9) / (num9 * num9);
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
									SuspensionFreqRa = SuspensionFreq * 0.715;
									SuspensionFreqRb = SuspensionFreq * 1.0;
									SuspensionFreqRc = SuspensionFreq * 1.43;
									SuspensionFreqR1 = SuspensionFreq * 2.0;
									SuspensionFreqR2 = SuspensionFreq * 2.86;
									SuspensionFreqR3 = SuspensionFreq * 4.0;
									SuspensionFreqR4 = SuspensionFreq * 5.72;
									SuspensionFreqR5 = SuspensionFreq * 8.0;
									SuspensionMultRa = num10 * 0.5;
									SuspensionMultRb = num11 * 1.0;
									SuspensionMultRc = num12 * 0.5;
									SuspensionMultR1 = num13 * 0.8;
									SuspensionMultR2 = num14 * 0.25;
									SuspensionMultR3 = num15 * 0.6;
									SuspensionMultR4 = num16 * 0.125;
									SuspensionMultR5 = num17 * 0.4;
									SuspensionRumbleMultRa = num18 * 0.0;
									SuspensionRumbleMultRb = num18 * 2.0;
									SuspensionRumbleMultRc = num18 * 0.0;
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
									SuspensionRumbleMultR4 = num18 * 0.0;
									SuspensionRumbleMultR5 = num18 * 0.5;
								}
								else
								{
									SuspensionFreq *= 2.0;
									SuspensionFreqRa = SuspensionFreq * 0.715;
									SuspensionFreqRb = SuspensionFreq * 1.0;
									SuspensionFreqRc = SuspensionFreq * 1.43;
									SuspensionFreqR1 = SuspensionFreq * 2.0;
									SuspensionFreqR2 = SuspensionFreq * 2.86;
									SuspensionFreqR3 = SuspensionFreq * 4.0;
									SuspensionFreqR4 = SuspensionFreq * 5.72;
									SuspensionFreqR5 = SuspensionFreq * 8.0;
									SuspensionMultRa = num10 * 0.5;
									SuspensionMultRb = num11 * 1.0;
									SuspensionMultRc = num12 * 0.5;
									SuspensionMultR1 = num13 * 0.8;
									SuspensionMultR2 = num14 * 0.25;
									SuspensionMultR3 = num15 * 0.6;
									SuspensionMultR4 = num16 * 0.125;
									SuspensionMultR5 = num17 * 0.4;
									SuspensionRumbleMultRa = num18 * 0.0;
									SuspensionRumbleMultRb = num18 * 2.0;
									SuspensionRumbleMultRc = num18 * 0.0;
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
									SuspensionRumbleMultR4 = num18 * 0.0;
									SuspensionRumbleMultR5 = num18 * 0.5;
								}
							}
							else
							{
								SuspensionFreqRa = SuspensionFreq * 1.0;
								SuspensionFreqRb = SuspensionFreq * 1.43;
								SuspensionFreqRc = SuspensionFreq * 2.0;
								SuspensionFreqR1 = SuspensionFreq * 2.86;
								SuspensionFreqR2 = SuspensionFreq * 4.0;
								SuspensionFreqR3 = SuspensionFreq * 5.72;
								SuspensionFreqR4 = SuspensionFreq * 8.0;
								SuspensionFreqR5 = SuspensionFreq * 11.44;
								SuspensionMultRa = num10 * 1.0;
								SuspensionMultRb = num11 * 0.5;
								SuspensionMultRc = num12 * 0.8;
								SuspensionMultR1 = num13 * 0.25;
								SuspensionMultR2 = num14 * 0.6;
								SuspensionMultR3 = num15 * 0.125;
								SuspensionMultR4 = num16 * 0.4;
								SuspensionMultR5 = num17 * (1.0 / 16.0);
								SuspensionRumbleMultRa = num18 * 2.0;
								SuspensionRumbleMultRb = num18 * 0.0;
								SuspensionRumbleMultRc = num18 * 1.5;
								SuspensionRumbleMultR1 = num18 * 0.0;
								SuspensionRumbleMultR2 = num18 * 1.0;
								SuspensionRumbleMultR3 = num18 * 0.0;
								SuspensionRumbleMultR4 = num18 * 0.5;
								SuspensionRumbleMultR5 = num18 * 0.0;
							}
						}
						else
						{
							SuspensionFreqRa = SuspensionFreq * 0.715;
							SuspensionFreqRb = SuspensionFreq * 1.0;
							SuspensionFreqRc = SuspensionFreq * 1.43;
							SuspensionFreqR1 = SuspensionFreq * 2.0;
							SuspensionFreqR2 = SuspensionFreq * 2.86;
							SuspensionFreqR3 = SuspensionFreq * 4.0;
							SuspensionFreqR4 = SuspensionFreq * 5.72;
							SuspensionFreqR5 = SuspensionFreq * 8.0;
							SuspensionMultRa = num10 * 0.5;
							SuspensionMultRb = num11 * 1.0;
							SuspensionMultRc = num12 * 0.5;
							SuspensionMultR1 = num13 * 0.8;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.6;
							SuspensionMultR4 = num16 * 0.125;
							SuspensionMultR5 = num17 * 0.4;
							SuspensionRumbleMultRa = num18 * 0.0;
							SuspensionRumbleMultRb = num18 * 2.0;
							SuspensionRumbleMultRc = num18 * 0.0;
							SuspensionRumbleMultR1 = num18 * 1.5;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.0;
							SuspensionRumbleMultR4 = num18 * 0.0;
							SuspensionRumbleMultR5 = num18 * 0.5;
						}
					}
					else
					{
						SuspensionFreqRa = SuspensionFreq * 0.5;
						SuspensionFreqRb = SuspensionFreq * 0.715;
						SuspensionFreqRc = SuspensionFreq * 1.0;
						SuspensionFreqR1 = SuspensionFreq * 1.43;
						SuspensionFreqR2 = SuspensionFreq * 2.0;
						SuspensionFreqR3 = SuspensionFreq * 2.86;
						SuspensionFreqR4 = SuspensionFreq * 4.0;
						SuspensionFreqR5 = SuspensionFreq * 5.72;
						SuspensionMultRa = num10 * 0.8;
						SuspensionMultRb = num11 * 0.5;
						SuspensionMultRc = num12 * 1.0;
						SuspensionMultR1 = num13 * 0.5;
						SuspensionMultR2 = num14 * 0.8;
						SuspensionMultR3 = num15 * 0.25;
						SuspensionMultR4 = num16 * 0.6;
						SuspensionMultR5 = num17 * 0.125;
						SuspensionRumbleMultRa = num18 * 1.5;
						SuspensionRumbleMultRb = num18 * 0.0;
						SuspensionRumbleMultRc = num18 * 2.0;
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
						SuspensionRumbleMultR4 = num18 * 1.0;
						SuspensionRumbleMultR5 = num18 * 0.0;
					}
				}
				else
				{
					SuspensionFreqRa = SuspensionFreq * (143.0 / 400.0);
					SuspensionFreqRb = SuspensionFreq * 0.5;
					SuspensionFreqRc = SuspensionFreq * 0.715;
					SuspensionFreqR1 = SuspensionFreq * 1.0;
					SuspensionFreqR2 = SuspensionFreq * 1.43;
					SuspensionFreqR3 = SuspensionFreq * 2.0;
					SuspensionFreqR4 = SuspensionFreq * 2.86;
					SuspensionFreqR5 = SuspensionFreq * 4.0;
					SuspensionMultRa = num10 * 0.25;
					SuspensionMultRb = num11 * 0.8;
					SuspensionMultRc = num12 * 0.5;
					SuspensionMultR1 = num13 * 1.0;
					SuspensionMultR2 = num14 * 0.5;
					SuspensionMultR3 = num15 * 0.8;
					SuspensionMultR4 = num16 * 0.25;
					SuspensionMultR5 = num17 * 0.6;
					SuspensionRumbleMultRa = num18 * 0.0;
					SuspensionRumbleMultRb = num18 * 1.5;
					SuspensionRumbleMultRc = num18 * 0.0;
					SuspensionRumbleMultR1 = num18 * 2.0;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 1.5;
					SuspensionRumbleMultR4 = num18 * 0.0;
					SuspensionRumbleMultR5 = num18 * 1.0;
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
							SuspensionFreqRa = SuspensionFreq * (1.0 / 16.0);
							SuspensionFreqRb = SuspensionFreq * 0.089375;
							SuspensionFreqRc = SuspensionFreq * 0.125;
							SuspensionFreqR1 = SuspensionFreq * (143.0 / 800.0);
							SuspensionFreqR2 = SuspensionFreq * 0.25;
							SuspensionFreqR3 = SuspensionFreq * (143.0 / 400.0);
							SuspensionFreqR4 = SuspensionFreq * 0.5;
							SuspensionFreqR5 = SuspensionFreq * 0.715;
							SuspensionMultRa = num10 * 0.2;
							SuspensionMultRb = num11 * (1.0 / 16.0);
							SuspensionMultRc = num12 * 0.4;
							SuspensionMultR1 = num13 * 0.125;
							SuspensionMultR2 = num14 * 0.6;
							SuspensionMultR3 = num15 * 0.25;
							SuspensionMultR4 = num16 * 0.8;
							SuspensionMultR5 = num17 * 0.5;
							SuspensionRumbleMultRa = num18 * 0.3;
							SuspensionRumbleMultRb = num18 * 0.0;
							SuspensionRumbleMultRc = num18 * 0.5;
							SuspensionRumbleMultR1 = num18 * 0.0;
							SuspensionRumbleMultR2 = num18 * 1.0;
							SuspensionRumbleMultR3 = num18 * 0.0;
							SuspensionRumbleMultR4 = num18 * 1.5;
							SuspensionRumbleMultR5 = num18 * 0.0;
						}
						else
						{
							SuspensionFreqRa = SuspensionFreq * 0.089375;
							SuspensionFreqRb = SuspensionFreq * 0.125;
							SuspensionFreqRc = SuspensionFreq * (143.0 / 800.0);
							SuspensionFreqR1 = SuspensionFreq * 0.25;
							SuspensionFreqR2 = SuspensionFreq * (143.0 / 400.0);
							SuspensionFreqR3 = SuspensionFreq * 0.5;
							SuspensionFreqR4 = SuspensionFreq * 0.715;
							SuspensionFreqR5 = SuspensionFreq * 1.0;
							SuspensionMultRa = num10 * (1.0 / 16.0);
							SuspensionMultRb = num11 * 0.4;
							SuspensionMultRc = num12 * 0.125;
							SuspensionMultR1 = num13 * 0.6;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.8;
							SuspensionMultR4 = num16 * 0.5;
							SuspensionMultR5 = num17 * 1.0;
							SuspensionRumbleMultRa = num18 * 0.0;
							SuspensionRumbleMultRb = num18 * 0.5;
							SuspensionRumbleMultRc = num18 * 0.0;
							SuspensionRumbleMultR1 = num18 * 1.0;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.5;
							SuspensionRumbleMultR4 = num18 * 0.0;
							SuspensionRumbleMultR5 = num18 * 2.0;
						}
					}
					else
					{
						SuspensionFreqRa = SuspensionFreq * 0.125;
						SuspensionFreqRb = SuspensionFreq * (143.0 / 800.0);
						SuspensionFreqRc = SuspensionFreq * 0.25;
						SuspensionFreqR1 = SuspensionFreq * (143.0 / 400.0);
						SuspensionFreqR2 = SuspensionFreq * 0.5;
						SuspensionFreqR3 = SuspensionFreq * 0.715;
						SuspensionFreqR4 = SuspensionFreq * 1.0;
						SuspensionFreqR5 = SuspensionFreq * 1.43;
						SuspensionMultRa = num10 * 0.4;
						SuspensionMultRb = num11 * 0.125;
						SuspensionMultRc = num12 * 0.6;
						SuspensionMultR1 = num13 * 0.25;
						SuspensionMultR2 = num14 * 0.8;
						SuspensionMultR3 = num15 * 0.5;
						SuspensionMultR4 = num16 * 1.0;
						SuspensionMultR5 = num17 * 0.5;
						SuspensionRumbleMultRa = num18 * 0.5;
						SuspensionRumbleMultRb = num18 * 0.0;
						SuspensionRumbleMultRc = num18 * 1.0;
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
						SuspensionRumbleMultR4 = num18 * 2.0;
						SuspensionRumbleMultR5 = num18 * 0.0;
					}
				}
				else
				{
					SuspensionFreqRa = SuspensionFreq * (143.0 / 800.0);
					SuspensionFreqRb = SuspensionFreq * 0.25;
					SuspensionFreqRc = SuspensionFreq * (143.0 / 400.0);
					SuspensionFreqR1 = SuspensionFreq * 0.5;
					SuspensionFreqR2 = SuspensionFreq * 0.715;
					SuspensionFreqR3 = SuspensionFreq * 1.0;
					SuspensionFreqR4 = SuspensionFreq * 1.43;
					SuspensionFreqR5 = SuspensionFreq * 2.0;
					SuspensionMultRa = num10 * 0.125;
					SuspensionMultRb = num11 * 0.6;
					SuspensionMultRc = num12 * 0.25;
					SuspensionMultR1 = num13 * 0.8;
					SuspensionMultR2 = num14 * 0.5;
					SuspensionMultR3 = num15 * 1.0;
					SuspensionMultR4 = num16 * 0.5;
					SuspensionMultR5 = num17 * 0.8;
					SuspensionRumbleMultRa = num18 * 0.0;
					SuspensionRumbleMultRb = num18 * 1.0;
					SuspensionRumbleMultRc = num18 * 0.0;
					SuspensionRumbleMultR1 = num18 * 1.5;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 2.0;
					SuspensionRumbleMultR4 = num18 * 0.0;
					SuspensionRumbleMultR5 = num18 * 1.5;
				}
			}
			else
			{
				SuspensionFreqRa = SuspensionFreq * 0.25;
				SuspensionFreqRb = SuspensionFreq * (143.0 / 400.0);
				SuspensionFreqRc = SuspensionFreq * 0.5;
				SuspensionFreqR1 = SuspensionFreq * 0.715;
				SuspensionFreqR2 = SuspensionFreq * 1.0;
				SuspensionFreqR3 = SuspensionFreq * 1.43;
				SuspensionFreqR4 = SuspensionFreq * 2.0;
				SuspensionFreqR5 = SuspensionFreq * 2.86;
				SuspensionMultRa = num10 * 0.6;
				SuspensionMultRb = num11 * 0.25;
				SuspensionMultRc = num12 * 0.8;
				SuspensionMultR1 = num13 * 0.5;
				SuspensionMultR2 = num14 * 1.0;
				SuspensionMultR3 = num15 * 0.5;
				SuspensionMultR4 = num16 * 0.8;
				SuspensionMultR5 = num17 * 0.25;
				SuspensionRumbleMultRa = num18 * 1.0;
				SuspensionRumbleMultRb = num18 * 0.0;
				SuspensionRumbleMultRc = num18 * 1.5;
				SuspensionRumbleMultR1 = num18 * 0.0;
				SuspensionRumbleMultR2 = num18 * 2.0;
				SuspensionRumbleMultR3 = num18 * 0.0;
				SuspensionRumbleMultR4 = num18 * 1.5;
				SuspensionRumbleMultR5 = num18 * 0.0;
			}
			EngineLoad = data.NewData.CarSettings_CurrentDisplayedRPMPercent * 0.5;
			EngineLoad += data.NewData.SpeedKmh * data.NewData.SpeedKmh * 0.0003;
			EngineLoad += data.NewData.SpeedKmh * 0.02;
			if (Math.Abs(SuspensionAccAll) > 0.5)
				EngineLoad += 200.0 * Math.Sin(data.NewData.OrientationPitch * 0.0174533);
			EngineLoad -= EngineLoad * (1.0 - MixPower) * 0.5;
			EngineLoad *= data.NewData.Throttle * 0.01 * 0.01;

			if (IdleSampleCount < 20) /*&& FrameCountTicks % 2500000L <= 150000L*/
				if (data.NewData.Rpms > 300)
					if (data.NewData.Rpms <= idleRPM * 1.1)
			{
				double num19 = Math.Abs(data.OldData.Rpms - data.NewData.Rpms) * FPS;

				if (num19 < 40.0)
				{
					idleRPM = Convert.ToUInt16((1 + idleRPM + (int)data.NewData.Rpms) >> 1);
					++IdleSampleCount;
					double num20 = idleRPM * 0.008333333;
					FrequencyMultiplier = num20 >= 5.0 ? (num20 >= 10.0 ? (num20 <= 20.0 ? (num20 <= 40.0 ? 1.0 : 0.25) : 0.5) : 2.0) : 4.0;
				}
				if (20 == IdleSampleCount)
					if (0 == SHP.S.IdleRPM)
						SHP.S.IdleRPM = idleRPM;
			}

			if (FrameCountTicks % 5000000L <= 150000L)
			{
				SetRPMIntervals();
				SetRPMMix();
			}
			FreqHarmonic = data.NewData.Rpms * 0.008333333;
			FreqOctave = FreqHarmonic * (1.0 + IntervalOctave * 0.08333333);
			FreqLFEAdaptive = FreqHarmonic * FrequencyMultiplier;
			FreqIntervalA1 = FreqHarmonic * (1.0 + IntervalA * 0.08333333);
			FreqIntervalA2 = FreqHarmonic * 0.5 * (1.0 + IntervalA * 0.08333333);
			FreqPeakA1 = FreqHarmonic * (1.0 + IntervalPeakA * 0.08333333);
			FreqPeakB1 = FreqHarmonic * (1.0 + IntervalPeakB * 0.08333333);
			FreqPeakA2 = FreqHarmonic * 0.5 * (1.0 + IntervalPeakA * 0.08333333);
			FreqPeakB2 = FreqHarmonic * 0.5 * (1.0 + IntervalPeakB * 0.08333333);
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
									? 75.0 - (FreqHarmonic - 125.0)
									: 95.0 - (FreqHarmonic - 95.0) * 0.667)
								: 65.0 + (FreqHarmonic - 65.0) * 1.0)
							: 52.5 + (FreqHarmonic - 40.0) * 0.5)
						: 40.0 + (FreqHarmonic - 25.0) * 0.834)
					: 30.0 + (FreqHarmonic - 15.0) * 1.0;
			Gain1H = Math.Max(Gain1H, 0.0) * num21 * num22 * (0.8 + 0.2 * MixPower + 0.2 * MixCylinder);
			Gain1H = Math.Floor(Gain1H.Clamp(0.0, sbyte.MaxValue));
			Gain1H2 = FreqHarmonic >= 25.0 ? (FreqHarmonic >= 40.0
												 ? (FreqHarmonic >= 65.0
													 ? (FreqHarmonic >= 95.0
														 ? (FreqHarmonic >= 125.0
															 ? 75.0 - (FreqHarmonic - 125.0)
															 : 95.0 - (FreqHarmonic - 95.0) * 0.667)
														 : 65.0 + (FreqHarmonic - 65.0) * 1.0)
													 : 52.5 + (FreqHarmonic - 40.0) * 0.5)
												 : 40.0 + (FreqHarmonic - 25.0) * 0.834)
											 : 30.0 + (FreqHarmonic - 15.0) * 1.0;
			Gain1H2 = Math.Max(Gain1H2, 0.0) * num21 * num22 * (0.8 + 0.1 * MixDisplacement + 0.3 * MixCylinder);
			Gain1H2 = Math.Floor(Gain1H2.Clamp(0.0, sbyte.MaxValue));
			Gain2H = FreqHarmonic >= 25.0 ? (FreqHarmonic >= 40.0
												 ? (FreqHarmonic >= 65.0
													 ? (FreqHarmonic >= 95.0
														 ? (FreqHarmonic >= 125.0
															 ? 75.0 - (FreqHarmonic - 125.0)
															 : 95.0 - (FreqHarmonic - 95.0) * 0.667)
														 : 65.0 + (FreqHarmonic - 65.0) * 1.0)
													 : 52.5 + (FreqHarmonic - 40.0) * 0.5)
												 : 40.0 + (FreqHarmonic - 25.0) * 0.834)
											 : 30.0 + (FreqHarmonic - 15.0) * 1.0;
			Gain2H = Math.Max(Gain2H, 0.0) * num21 * num22 * (0.8 + 0.3 * MixPower + 0.1 * MixCylinder);
			Gain2H = Math.Floor(Gain2H.Clamp(0.0, sbyte.MaxValue));
			Gain4H = FreqHarmonic >= 25.0 ? (FreqHarmonic >= 40.0
												 ? (FreqHarmonic >= 65.0
													 ? (FreqHarmonic >= 95.0
														 ? (FreqHarmonic >= 125.0
															 ? 75.0 - (FreqHarmonic - 125.0)
															 : 95.0 - (FreqHarmonic - 95.0) * 0.667)
														 : 66.0 + (FreqHarmonic - 65.0) * 1.0)
													 : 52.5 + (FreqHarmonic - 40.0) * 0.5)
												 : 40.0 + (FreqHarmonic - 25.0) * 0.834)
											 : 30.0 + (FreqHarmonic - 15.0) * 1.0;
			Gain4H = Math.Max(Gain4H, 0.0) * num21 * num22 * (0.8 + 0.2 * MixPower + 0.2 * MixDisplacement);
			Gain4H = Math.Floor(Gain4H.Clamp(0.0, sbyte.MaxValue));
			GainOctave = FreqOctave >= 55.0 ? (FreqOctave >= 80.0 ? 75.0 - (FreqOctave - 80.0) * 0.75 : 30.0 + (FreqOctave - 55.0) * 1.8) : (FreqOctave - 30.0) * 1.2;
			GainOctave = Math.Max(GainOctave, 0.0) * num21 * (0.3 * MixPower + 0.3 * MixCylinder + 0.6 * EngineLoad);
			GainOctave = Math.Floor(GainOctave.Clamp(0.0, sbyte.MaxValue));
			GainIntervalA1 = FreqIntervalA1 >= 70.0 ? (FreqIntervalA1 >= 85.0 ? 75.0 - (FreqIntervalA1 - 85.0) * 0.85 : 45.0 + (FreqIntervalA1 - 70.0) * 2.0) : (FreqIntervalA1 - 40.0) * 1.5;
			GainIntervalA1 = Math.Max(GainIntervalA1, 0.0) * num21 * (0.2 * MixPower + 1.0 * EngineLoad);
			GainIntervalA1 = Math.Floor(GainIntervalA1.Clamp(0.0, sbyte.MaxValue));
			GainIntervalA2 = FreqIntervalA2 >= 70.0 ? (FreqIntervalA2 >= 85.0 ? 75.0 - (FreqIntervalA2 - 85.0) * 0.85 : 45.0 + (FreqIntervalA2 - 70.0) * 2.0) : (FreqIntervalA2 - 40.0) * 1.5;
			GainIntervalA2 = Math.Max(GainIntervalA2, 0.0) * num21 * (0.1 * MixPower + 0.3 * MixCylinder + 0.8 * EngineLoad);
			GainIntervalA2 = Math.Floor(GainIntervalA2.Clamp(0.0, sbyte.MaxValue));
			PeakA1Start = RedlinePercent * (0.96 + GearInterval * Gear * 0.04);
			PeakB1Start = RedlinePercent * (0.92 + GearInterval * Gear * 0.04);
			PeakA2Start = RedlinePercent * (0.9 + MixPower * GearInterval * Gear * 0.06);
			PeakB2Start = RedlinePercent * (0.98 - MixTorque * 0.08);
			PeakA1Modifier = ((RPMPercent - PeakA1Start) / (RedlinePercent - PeakA1Start + (1.0 - RedlinePercent) * (0.75 + MixCylinder * 0.75))).Clamp(0.0, 1.0);
			PeakB1Modifier = ((RPMPercent - PeakB1Start) / (RedlinePercent - PeakB1Start + (1.0 - RedlinePercent) * (0.0 + MixCylinder))).Clamp(0.0, 1.0);
			PeakA2Modifier = ((RPMPercent - PeakA2Start) / (RedlinePercent - PeakA2Start)).Clamp(0.0, 1.0);
			PeakB2Modifier = ((RPMPercent - PeakB2Start) / (RedlinePercent - PeakB2Start + (1.0 - RedlinePercent) * (1.0 - MixDisplacement))).Clamp(0.0, 1.0);
			GainPeakA1 = FreqPeakA1 >= 55.0 ? (FreqPeakA1 >= 75.0 ? (FreqPeakA1 >= 105.0 ? 90.0 - (FreqPeakA1 - 105.0) * 0.75 : 60.0 + (FreqPeakA1 - 75.0) * 1.0) : 30.0 + (FreqPeakA1 - 55.0) * 1.5) : (FreqPeakA1 - 45.0) * 3.0;
			GainPeakA1 = Math.Max(GainPeakA1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakA1Front = Math.Floor((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1Rear = Math.Floor((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1 = Math.Floor((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1 = FreqPeakB1 >= 55.0 ? (FreqPeakB1 >= 75.0 ? (FreqPeakB1 >= 105.0 ? 90.0 - (FreqPeakB1 - 105.0) * 0.75 : 60.0 + (FreqPeakB1 - 75.0) * 1.0) : 30.0 + (FreqPeakB1 - 55.0) * 1.5) : (FreqPeakB1 - 45.0) * 3.0;
			GainPeakB1 = Math.Max(GainPeakB1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakB1Front = Math.Floor((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1Rear = Math.Floor((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1 = Math.Floor((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2 = FreqPeakA2 >= 55.0 ? (FreqPeakA2 >= 75.0 ? (FreqPeakA2 >= 105.0 ? 90.0 - (FreqPeakA2 - 105.0) * 0.75 : 60.0 + (FreqPeakA2 - 75.0) * 1.0) : 30.0 + (FreqPeakA2 - 55.0) * 1.5) : (FreqPeakA2 - 45.0) * 3.0;
			GainPeakA2 = Math.Max(GainPeakA2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakA2Front = Math.Floor((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2Rear = Math.Floor((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2 = Math.Floor((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = FreqPeakB2 >= 60.0 ? (FreqPeakB2 >= 100.0 ? 100.0 - (FreqPeakB2 - 100.0) * 0.85 : 30.0 + (FreqPeakB2 - 60.0) * 1.75) : (FreqPeakB2 - 30.0) * 1.0;
			GainPeakB2 = Math.Max(GainPeakB2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakB2Front = Math.Floor((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2Rear = Math.Floor((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = Math.Floor((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			if (SHP.S.EngineCylinders < 1.0)
			{
				GainLFEAdaptive = 0.0;
				Gain1H = Math.Floor(Gain1H * 0.7);
				Gain1H2 = 0.0;
				Gain2H = 0.0;
				Gain4H = 0.0;
				GainOctave = 0.0;
				GainIntervalA1 = 0.0;
				GainIntervalA2 = 0.0;
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
			else if (SHP.S.EngineCylinders < 2.0)
				Gain4H = 0.0;
			if (EngineMult == 1.0)
				return;
			GainLFEAdaptive *= EngineMult * EngineMultAll;
			Gain1H *= EngineMult * EngineMultAll;
			Gain1H2 *= EngineMult * EngineMultAll;
			Gain2H *= EngineMult * EngineMultAll;
			Gain4H *= EngineMult * EngineMultAll;
			GainOctave *= EngineMult * EngineMultAll;
			GainIntervalA1 *= EngineMult * EngineMultAll;
			GainIntervalA2 *= EngineMult * EngineMultAll;
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
