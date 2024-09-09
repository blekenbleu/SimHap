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
		public double FreqHarmonic;
		public double FreqOctave;
		public double FreqLFEAdaptive;
#if slim
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
#endif
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
#if !slim
		public double EngineLoad;
		public double FrequencyMultiplier;
		public double FreqPeakA1;
		public double GainLFEAdaptive;
		public double MotionPitch;
		public double MotionRoll;
		public double MotionYaw;
		public double MotionHeave;
		public double MotionSurge;
		public double MotionSway;
		public bool RumbleFromPlugin;
		public double RumbleLeftAvg;
		public double RumbleRightAvg;
		public double RumbleLeft;
		public double RumbleRight;
		public double SuspensionRumbleMultR1;
		public double SuspensionRumbleMultR2;
		public double SuspensionRumbleMultR3;
#endif
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
#if slim
		private long FrameTimeTicks;
        private long FrameCountTicks;
        internal Haptics H;
        internal int Index;
        internal string raw = "DataCorePlugin.GameRawData.";
        private ushort idleRPM;
#else
		public bool ABSActive;
		public double ABSPulse;
		public long ABSPauseInterval;
		public long ABSPulseInterval;
		public long ABSTicks;
		public double IntervalOctave;
		public double IntervalA;
		public double IntervalB;
		public double IntervalPeakA;
		public double IntervalPeakB;
		public double FreqIntervalA1;
		public double FreqIntervalA2;
		public double Gain1H2;
		public double Gain2H;
		public double Gain4H;
		public double GainOctave;
		public double GainIntervalA1;
		public double GainIntervalA2;
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
		public double SuspensionFreqRa;
		public double SuspensionFreqRb;
		public double SuspensionFreqRc;
		public double SuspensionFreqR4;
		public double SuspensionFreqR5;
		public double SuspensionMultRa;
		public double SuspensionMultRb;
		public double SuspensionMultRc;
		public double SuspensionMultR4;
		public double SuspensionMultR5;
		public double SuspensionRumbleMultRa;
		public double SuspensionRumbleMultRb;
		public double SuspensionRumbleMultRc;
		public double SuspensionRumbleMultR4;
		public double SuspensionRumbleMultR5;

		internal Haptics H;
		internal int Index;
		internal string raw;
		private long FrameTimeTicks;
		private long FrameCountTicks;
		private ushort idleRPM;						 // for sniffing in Refresh()
#endif

		public SimData()
		{
#if slim
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
#else
			AccSamples = 16;
			Acc1 = 0;
			CarInitCount = 0;
			Downshift = false;
			FrameCountTicks = 0;
			GameAltText = "";
			Gear = 0;
			GearPrevious = 0;
			IdleSampleCount = 0;
			LoadText = "Not Loaded";
			raw = "DataCorePlugin.GameRawData.";
			RumbleFromPlugin = false;
			ShiftTicks = FrameTimeTicks = DateTime.Now.Ticks;
			SlipXGammaBaseMult = 1.0;
			SlipYGammaBaseMult = 1.0;
			SuspensionDistFLP = 0.0;
			SuspensionDistFRP = 0.0;
			SuspensionDistRLP = 0.0;
			SuspensionDistRRP = 0.0;
			TireDiameterSampleMax = 100;
			Upshift = false;
            RumbleFromPlugin = false;
            idleRPM = 2500;                         // default value; seems high IMO
#endif
		}
#if !slim
		private void SetRPMIntervals()
		{
			if (H.S.EngineCylinders == 1.0)
			{
				IntervalOctave = 4.0;
				IntervalA = 0.0;
				IntervalB = 0.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 2.0)
			{
				IntervalOctave = 4.0;
				IntervalA = 6.0;
				IntervalB = 0.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 4.0)
			{
				IntervalOctave = 8.0;
				IntervalA = 4.0;
				IntervalB = 5.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 8.0)
			{
				IntervalOctave = 16.0;
				IntervalA = 6.0;
				IntervalB = 10.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 16.0)
			{
				IntervalOctave = 16.0;
				IntervalA = 12.0;
				IntervalB = 20.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 3.0)
			{
				IntervalOctave = 6.0;
				IntervalA = 4.0;
				IntervalB = 0.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 6.0)
			{
				IntervalOctave = 12.0;
				IntervalA = 8.0;
				IntervalB = 10.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 12.0)
			{
				IntervalOctave = 12.0;
				IntervalA = 16.0;
				IntervalB = 20.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 5.0)
			{
				IntervalOctave = 10.0;
				IntervalA = 6.0;
				IntervalB = 9.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineCylinders == 10.0)
			{
				IntervalOctave = 10.0;
				IntervalA = 12.0;
				IntervalB = 18.0;
				IntervalPeakA = 8.0;
				IntervalPeakB = 4.0;
			}
			else if (H.S.EngineConfiguration == "R")
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

		private float Data(string prop)
		{
			var foo = PM.GetPropertyValue(raw+"Data."+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}
#endif

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
#if !slim
			SlipXFL = 0.0;
			SlipXFR = 0.0;
			SlipXRL = 0.0;
			SlipXRR = 0.0;
			SlipYFL = 0.0;
			SlipYFR = 0.0;
			SlipYRL = 0.0;
			SlipYRR = 0.0;
			ABSActive = H.N.ABSActive == 1;
#endif
			bool flag = true;
			switch (Haptics.CurrentGame)
			{
				case GameId.AC:
					SuspensionDistFL = Physics("SuspensionTravel01");
					SuspensionDistFR = Physics("SuspensionTravel02");
					SuspensionDistRL = Physics("SuspensionTravel03");
					SuspensionDistRR = Physics("SuspensionTravel04");
#if !slim
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
#endif
					TiresLeft = 1.0 + (double) Math.Max(Physics("TyreContactHeading01.Y"), Physics("TyreContactHeading03.Y"));
					TiresRight = 1.0 + (double) Math.Max(Physics("TyreContactHeading02.Y"), Physics("TyreContactHeading04.Y"));
#if !slim
					if (RumbleLeftAvg == 0.0)
						RumbleLeftAvg = TiresLeft;
					if (RumbleRightAvg == 0.0)
						RumbleRightAvg = TiresRight;
					RumbleLeftAvg = (RumbleLeftAvg + TiresLeft) * 0.5;
					RumbleRightAvg = (RumbleRightAvg + TiresRight) * 0.5;
					RumbleLeft = Math.Abs(TiresLeft / RumbleLeftAvg - 1.0) * 2000.0;
					RumbleRight = Math.Abs(TiresRight / RumbleRightAvg - 1.0) * 2000.0;
#endif
					break;
				case GameId.ACC:
					SuspensionDistFL = Physics("SuspensionTravel01");
					SuspensionDistFR = Physics("SuspensionTravel02");
					SuspensionDistRL = Physics("SuspensionTravel03");
					SuspensionDistRR = Physics("SuspensionTravel04");
					WiperStatus = (int) PM.GetPropertyValue(raw+"Graphics.WiperLV");
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
#if !slim
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
#endif
					break;
				case GameId.DR2:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
#if !slim
					WheelSpeedFL = Math.Abs(Raw("WheelSpeedFrontLeft"));
					WheelSpeedFR = Math.Abs(Raw("WheelSpeedFrontRight"));
					WheelSpeedRL = Math.Abs(Raw("WheelSpeedRearLeft"));
					WheelSpeedRR = Math.Abs(Raw("WheelSpeedRearRight"));
					SlipFromWheelSpeed();
#endif
					VelocityX = Raw("WorldSpeedX") * Math.Sin(Raw("XR"));
					YawRate = H.N.OrientationYawAcceleration;
#if !slim
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
#endif
					break;
				case GameId.WRC23:
					SuspensionDistFL = Raw("SessionUpdate.vehicle_hub_position_fl");
					SuspensionDistFR = Raw("SessionUpdate.vehicle_hub_position_fr");
					SuspensionDistRL = Raw("SessionUpdate.vehicle_hub_position_bl");
					SuspensionDistRR = Raw("SessionUpdate.vehicle_hub_position_br");
					SpeedMs = Raw("SessionUpdate.vehicle_speed");
					InvSpeedMs = SpeedMs != 0.0 ? 1.0 / SpeedMs : 0.0;
#if !slim
					WheelSpeedFL = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_fl"));
					WheelSpeedFR = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_fr"));
					WheelSpeedRL = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_bl"));
					WheelSpeedRR = Math.Abs(Raw("SessionUpdate.vehicle_cp_forward_speed_br"));
					SlipFromWheelSpeed();
#endif
					VelocityX = Raw("SessionUpdateLocalVelocity.X");
					YawRate = H.N.OrientationYawAcceleration;
#if !slim
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
#endif
					break;
				case GameId.Forza:
					SuspensionDistFL = Raw("SuspensionTravelMetersFrontLeft");
					SuspensionDistFR = Raw("SuspensionTravelMetersFrontRight");
					SuspensionDistRL = Raw("SuspensionTravelMetersRearLeft");
					SuspensionDistRR = Raw("SuspensionTravelMetersRearRight");
#if !slim
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
#endif
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
#if !slim
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
						if (H.S.PoweredWheels == "F")
						{
							SlipYFL = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.2;
							SlipYFR = Accelerator * -InvAccSurgeAvg * InvSpeedMs * 0.2;
							SlipYRL = 0.0;
							SlipYRR = 0.0;
						}
						else if (H.S.PoweredWheels == "R")
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
					if (H.N.Brake > 90.0)
					{
						ABSActive = (Raw("CurrentPlayerTelemetry.mWheels01.mBrakePressure")
								   + Raw("CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < H.N.Brake - 1.0;
						break;
					}
					break;
				case GameId.RRRE:
					if (PM.GetPropertyValue(raw+"Player.SuspensionDeflection.FrontLeft") != null)
					{
						SuspensionDistFL = Raw("Player.SuspensionDeflection.FrontLeft");
						SuspensionDistFR = Raw("Player.SuspensionDeflection.FrontRight");
					}
					if (PM.GetPropertyValue(raw+"Player.SuspensionDeflection.RearLeft") != null)
					{
						SuspensionDistRL = Raw("Player.SuspensionDeflection.RearLeft");
						SuspensionDistRR = Raw("Player.SuspensionDeflection.RearRight");
					}
					if (PM.GetPropertyValue(raw+"Player.ThirdSpringSuspensionDeflectionFront") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Player.ThirdSpringSuspensionDeflectionFront");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Player.ThirdSpringSuspensionDeflectionFront");
					}
					if (PM.GetPropertyValue(raw+"Player.ThirdSpringSuspensionDeflectionRear") != null)
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
					WiperStatus = (bool) PM.GetPropertyValue(raw+"TruckValues.CurrentValues.DashboardValues.Wipers") ? 1 : 0;
#endif
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
#if !slim
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
					if ((int) PM.GetPropertyValue(raw+"m_sData.m_aiWheelMaterial01") == 7
					 || (int) PM.GetPropertyValue(raw+"m_sData.m_aiWheelMaterial02") == 7)
					{
						RumbleLeft = 50.0;
						RumbleRight = 50.0;
						break;
					}
					RumbleLeft = 0.0;
					RumbleRight = 0.0;
#endif
					break;
				case GameId.LMU:
					SuspensionDistFL = Raw("CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					SuspensionDistFR = Raw("CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					SuspensionDistRL = Raw("CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					SuspensionDistRR = Raw("CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
#if !slim
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
					if (H.N.Brake > 80.0)
					{
						ABSActive = (Raw("CurrentPlayerTelemetry.mWheels01.mBrakePressure")
								   + Raw("CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < H.N.Brake - 1.0;
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
#endif
					break;
			}
			if (!flag)
				return;
			SuspensionVelFL = (SuspensionDistFL - SuspensionDistFLP) * FPS;
			SuspensionVelFR = (SuspensionDistFR - SuspensionDistFRP) * FPS;
			SuspensionVelRL = (SuspensionDistRL - SuspensionDistRLP) * FPS;
			SuspensionVelRR = (SuspensionDistRR - SuspensionDistRRP) * FPS;
		}

#if !slim
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
#endif
		internal ushort Rpms;

		// called from DataUpdate()
		internal void Refresh(Haptics shp, PluginManager pluginManager)
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
#if !slim
				RumbleLeftAvg = 0.0;
				RumbleRightAvg = 0.0;
#endif
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
#if !slim
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
#endif
			WheelLoadFL = ((100.0 + AccSurge[Acc0]) * (100.0 - AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadFR = ((100.0 + AccSurge[Acc0]) * (100.0 + AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadRL = ((100.0 - AccSurge[Acc0]) * (100.0 - AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			WheelLoadRR = ((100.0 - AccSurge[Acc0]) * (100.0 + AccSway[Acc0]) * 0.01 - 50.0) * 0.01;
			UpdateVehicle();
			Airborne = AccHeave2S < -2.0 || Math.Abs(H.N.OrientationRoll) > 60.0;
#if !slim
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
			WheelSpinAll = !(H.S.PoweredWheels == "F")
				? (!(H.S.PoweredWheels == "R")
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
#endif

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
			
#if !slim
			ABSPauseInterval = SlipYAll <= 0.0
								? (long) (1166667.0 - 666667.0 * ((H.N.SpeedKmh - 20.0) * 0.003333333).Clamp(0.0, 1.0))
								: (long) (1250000.0 - 666667.0 * SlipYAll.Clamp(0.0, 1.0));
			ABSPulseInterval = 166666L * H.Settings.ABSPulseLength;
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
#endif
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
#if !slim
			double num2 = 94.0 + 0.4 * SpeedMs;
			double num3 = 76.0 + 0.45 * SpeedMs;
			double num4 = 60.0 + 0.5 * SpeedMs;
			double num8 = 16.0 + 0.7 * SpeedMs;
			double num9 = 10.0 + 0.75 * SpeedMs;
			double num10 = 0.55 + 1.8 * AccHeaveAbs * (AccHeaveAbs + num2) / (num2 * num2);
			double num11 = 0.5 + 2.0 * AccHeaveAbs * (AccHeaveAbs + num3) / (num3 * num3);
			double num12 = 0.45 + 2.2 * AccHeaveAbs * (AccHeaveAbs + num4) / (num4 * num4);
			double num16 = 0.7 + 1.2 * AccHeaveAbs * (AccHeaveAbs + num8) / (num8 * num8);
			double num17 = 0.8 + 0.8 * AccHeaveAbs * (AccHeaveAbs + num9) / (num9 * num9);
#endif
			double num13 = 0.4 + 2.4 * AccHeaveAbs * (AccHeaveAbs + num5) / (num5 * num5);
			double num14 = 0.5 + 2.0 * AccHeaveAbs * (AccHeaveAbs + num6) / (num6 * num6);
			double num15 = 0.6 + 1.6 * AccHeaveAbs * (AccHeaveAbs + num7) / (num7 * num7);
#if !slim
			double num18 = RumbleMult * RumbleMultAll * (0.6 + SpeedMs * (90.0 - SpeedMs) * 0.0002);
#endif
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
#if !slim
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
									SuspensionFreqRa = SuspensionFreq * 0.715;
									SuspensionFreqRb = SuspensionFreq * 1.0;
									SuspensionFreqRc = SuspensionFreq * 1.43;
									SuspensionFreqR4 = SuspensionFreq * 5.72;
									SuspensionFreqR5 = SuspensionFreq * 8.0;
									SuspensionMultRa = num10 * 0.5;
									SuspensionMultRb = num11 * 1.0;
									SuspensionMultRc = num12 * 0.5;
									SuspensionMultR4 = num16 * 0.125;
									SuspensionMultR5 = num17 * 0.4;
									SuspensionRumbleMultRa = num18 * 0.0;
									SuspensionRumbleMultRb = num18 * 2.0;
									SuspensionRumbleMultRc = num18 * 0.0;
									SuspensionRumbleMultR4 = num18 * 0.0;
									SuspensionRumbleMultR5 = num18 * 0.5;
#endif
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
#if !slim
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
									SuspensionFreqRa = SuspensionFreq * 0.715;
									SuspensionFreqRb = SuspensionFreq * 1.0;
									SuspensionFreqRc = SuspensionFreq * 1.43;
									SuspensionFreqR4 = SuspensionFreq * 5.72;
									SuspensionFreqR5 = SuspensionFreq * 8.0;
									SuspensionMultRa = num10 * 0.5;
									SuspensionMultRb = num11 * 1.0;
									SuspensionMultRc = num12 * 0.5;
									SuspensionMultR4 = num16 * 0.125;
									SuspensionMultR5 = num17 * 0.4;
									SuspensionRumbleMultRa = num18 * 0.0;
									SuspensionRumbleMultRb = num18 * 2.0;
									SuspensionRumbleMultRc = num18 * 0.0;
									SuspensionRumbleMultR4 = num18 * 0.0;
									SuspensionRumbleMultR5 = num18 * 0.5;
#endif
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
#if !slim
								SuspensionRumbleMultR1 = num18 * 0.0;
								SuspensionRumbleMultR2 = num18 * 1.0;
								SuspensionRumbleMultR3 = num18 * 0.0;
								SuspensionFreqRa = SuspensionFreq * 1.0;
								SuspensionFreqRb = SuspensionFreq * 1.43;
								SuspensionFreqRc = SuspensionFreq * 2.0;
								SuspensionFreqR4 = SuspensionFreq * 8.0;
								SuspensionFreqR5 = SuspensionFreq * 11.44;
								SuspensionMultRa = num10 * 1.0;
								SuspensionMultRb = num11 * 0.5;
								SuspensionMultRc = num12 * 0.8;
								SuspensionMultR4 = num16 * 0.4;
								SuspensionMultR5 = num17 * (1.0 / 16.0);
								SuspensionRumbleMultRa = num18 * 2.0;
								SuspensionRumbleMultRb = num18 * 0.0;
								SuspensionRumbleMultRc = num18 * 1.5;
								SuspensionRumbleMultR4 = num18 * 0.5;
								SuspensionRumbleMultR5 = num18 * 0.0;
#endif
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
#if !slim
							SuspensionRumbleMultR1 = num18 * 1.5;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.0;
							SuspensionFreqRa = SuspensionFreq * 0.715;
							SuspensionFreqRb = SuspensionFreq * 1.0;
							SuspensionFreqRc = SuspensionFreq * 1.43;
							SuspensionFreqR4 = SuspensionFreq * 5.72;
							SuspensionFreqR5 = SuspensionFreq * 8.0;
							SuspensionMultRa = num10 * 0.5;
							SuspensionMultRb = num11 * 1.0;
							SuspensionMultRc = num12 * 0.5;
							SuspensionMultR4 = num16 * 0.125;
							SuspensionMultR5 = num17 * 0.4;
							SuspensionRumbleMultRa = num18 * 0.0;
							SuspensionRumbleMultRb = num18 * 2.0;
							SuspensionRumbleMultRc = num18 * 0.0;
							SuspensionRumbleMultR4 = num18 * 0.0;
							SuspensionRumbleMultR5 = num18 * 0.5;
#endif
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
#if !slim
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
						SuspensionFreqRa = SuspensionFreq * 0.5;
						SuspensionFreqRb = SuspensionFreq * 0.715;
						SuspensionFreqRc = SuspensionFreq * 1.0;
						SuspensionFreqR4 = SuspensionFreq * 4.0;
						SuspensionFreqR5 = SuspensionFreq * 5.72;
						SuspensionMultRa = num10 * 0.8;
						SuspensionMultRb = num11 * 0.5;
						SuspensionMultRc = num12 * 1.0;
						SuspensionMultR4 = num16 * 0.6;
						SuspensionMultR5 = num17 * 0.125;
						SuspensionRumbleMultRa = num18 * 1.5;
						SuspensionRumbleMultRb = num18 * 0.0;
						SuspensionRumbleMultRc = num18 * 2.0;
						SuspensionRumbleMultR4 = num18 * 1.0;
						SuspensionRumbleMultR5 = num18 * 0.0;
#endif
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
#if !slim
					SuspensionRumbleMultR1 = num18 * 2.0;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 1.5;
					SuspensionFreqRa = SuspensionFreq * (143.0 / 400.0);
					SuspensionFreqRb = SuspensionFreq * 0.5;
					SuspensionFreqRc = SuspensionFreq * 0.715;
					SuspensionFreqR4 = SuspensionFreq * 2.86;
					SuspensionFreqR5 = SuspensionFreq * 4.0;
					SuspensionMultRa = num10 * 0.25;
					SuspensionMultRb = num11 * 0.8;
					SuspensionMultRc = num12 * 0.5;
					SuspensionMultR4 = num16 * 0.25;
					SuspensionMultR5 = num17 * 0.6;
					SuspensionRumbleMultRa = num18 * 0.0;
					SuspensionRumbleMultRb = num18 * 1.5;
					SuspensionRumbleMultRc = num18 * 0.0;
					SuspensionRumbleMultR4 = num18 * 0.0;
					SuspensionRumbleMultR5 = num18 * 1.0;
#endif
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
#if !slim
							SuspensionRumbleMultR1 = num18 * 0.0;
							SuspensionRumbleMultR2 = num18 * 1.0;
							SuspensionRumbleMultR3 = num18 * 0.0;
							SuspensionFreqRa = SuspensionFreq * (1.0 / 16.0);
							SuspensionFreqRb = SuspensionFreq * 0.089375;
							SuspensionFreqRc = SuspensionFreq * 0.125;
							SuspensionFreqR4 = SuspensionFreq * 0.5;
							SuspensionFreqR5 = SuspensionFreq * 0.715;
							SuspensionMultRa = num10 * 0.2;
							SuspensionMultRb = num11 * (1.0 / 16.0);
							SuspensionMultRc = num12 * 0.4;
							SuspensionMultR4 = num16 * 0.8;
							SuspensionMultR5 = num17 * 0.5;
							SuspensionRumbleMultRa = num18 * 0.3;
							SuspensionRumbleMultRb = num18 * 0.0;
							SuspensionRumbleMultRc = num18 * 0.5;
							SuspensionRumbleMultR4 = num18 * 1.5;
							SuspensionRumbleMultR5 = num18 * 0.0;
#endif
						}
						else
						{
							SuspensionFreqR1 = SuspensionFreq * 0.25;
							SuspensionFreqR2 = SuspensionFreq * (143.0 / 400.0);
							SuspensionFreqR3 = SuspensionFreq * 0.5;
							SuspensionMultR1 = num13 * 0.6;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.8;
#if !slim
							SuspensionRumbleMultR1 = num18 * 1.0;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.5;
							SuspensionFreqRa = SuspensionFreq * 0.089375;
							SuspensionFreqRb = SuspensionFreq * 0.125;
							SuspensionFreqRc = SuspensionFreq * (143.0 / 800.0);
							SuspensionFreqR4 = SuspensionFreq * 0.715;
							SuspensionFreqR5 = SuspensionFreq * 1.0;
							SuspensionMultRa = num10 * (1.0 / 16.0);
							SuspensionMultRb = num11 * 0.4;
							SuspensionMultRc = num12 * 0.125;
							SuspensionMultR4 = num16 * 0.5;
							SuspensionMultR5 = num17 * 1.0;
							SuspensionRumbleMultRa = num18 * 0.0;
							SuspensionRumbleMultRb = num18 * 0.5;
							SuspensionRumbleMultRc = num18 * 0.0;
							SuspensionRumbleMultR4 = num18 * 0.0;
							SuspensionRumbleMultR5 = num18 * 2.0;
#endif
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
#if !slim
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
						SuspensionFreqRa = SuspensionFreq * 0.125;
						SuspensionFreqRb = SuspensionFreq * (143.0 / 800.0);
						SuspensionFreqRc = SuspensionFreq * 0.25;
						SuspensionFreqR4 = SuspensionFreq * 1.0;
						SuspensionFreqR5 = SuspensionFreq * 1.43;
						SuspensionMultRa = num10 * 0.4;
						SuspensionMultRb = num11 * 0.125;
						SuspensionMultRc = num12 * 0.6;
						SuspensionMultR4 = num16 * 1.0;
						SuspensionMultR5 = num17 * 0.5;
						SuspensionRumbleMultRa = num18 * 0.5;
						SuspensionRumbleMultRb = num18 * 0.0;
						SuspensionRumbleMultRc = num18 * 1.0;
						SuspensionRumbleMultR4 = num18 * 2.0;
						SuspensionRumbleMultR5 = num18 * 0.0;
#endif
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
#if !slim
					SuspensionRumbleMultR1 = num18 * 1.5;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 2.0;
					SuspensionFreqRa = SuspensionFreq * (143.0 / 800.0);
					SuspensionFreqRb = SuspensionFreq * 0.25;
					SuspensionFreqRc = SuspensionFreq * (143.0 / 400.0);
					SuspensionFreqR4 = SuspensionFreq * 1.43;
					SuspensionFreqR5 = SuspensionFreq * 2.0;
					SuspensionMultRa = num10 * 0.125;
					SuspensionMultRb = num11 * 0.6;
					SuspensionMultRc = num12 * 0.25;
					SuspensionMultR4 = num16 * 0.5;
					SuspensionMultR5 = num17 * 0.8;
					SuspensionRumbleMultRa = num18 * 0.0;
					SuspensionRumbleMultRb = num18 * 1.0;
					SuspensionRumbleMultRc = num18 * 0.0;
					SuspensionRumbleMultR4 = num18 * 0.0;
					SuspensionRumbleMultR5 = num18 * 1.5;
#endif
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
#if !slim
				SuspensionRumbleMultR1 = num18 * 0.0;
				SuspensionRumbleMultR2 = num18 * 2.0;
				SuspensionRumbleMultR3 = num18 * 0.0;
				SuspensionFreqRa = SuspensionFreq * 0.25;
				SuspensionFreqRb = SuspensionFreq * (143.0 / 400.0);
				SuspensionFreqRc = SuspensionFreq * 0.5;
				SuspensionFreqR4 = SuspensionFreq * 2.0;
				SuspensionFreqR5 = SuspensionFreq * 2.86;
				SuspensionMultRa = num10 * 0.6;
				SuspensionMultRb = num11 * 0.25;
				SuspensionMultRc = num12 * 0.8;
				SuspensionMultR4 = num16 * 0.8;
				SuspensionMultR5 = num17 * 0.25;
				SuspensionRumbleMultRa = num18 * 1.0;
				SuspensionRumbleMultRb = num18 * 0.0;
				SuspensionRumbleMultRc = num18 * 1.5;
				SuspensionRumbleMultR4 = num18 * 1.5;
				SuspensionRumbleMultR5 = num18 * 0.0;
#endif
			}
#if !slim
			EngineLoad = H.N.CarSettings_CurrentDisplayedRPMPercent * 0.5;
			EngineLoad += H.N.SpeedKmh * H.N.SpeedKmh * 0.0003;
			EngineLoad += H.N.SpeedKmh * 0.02;
			if (Math.Abs(SuspensionAccAll) > 0.5)
				EngineLoad += 200.0 * Math.Sin(H.N.OrientationPitch * 0.0174533);
			EngineLoad -= EngineLoad * (1.0 - MixPower) * 0.5;
			EngineLoad *= H.N.Throttle * 0.01 * 0.01;
			if (IdleSampleCount < 20) /*&& FrameCountTicks % 2500000L <= 150000L*/	// Refresh() sniff: ignore FrameCountTicks .. for now
#else
			if (IdleSampleCount < 0) /*&& FrameCountTicks % 2500000L <= 150000L*/	// Refresh()sniff: ignore FrameCountTicks .. for now
#endif
				if (H.N.Rpms > 300 && H.N.Rpms <= idleRPM * 1.1) // Refresh(): supposes that idleRPM is somewhat valid..??
			{
				double num19 = Math.Abs(H.Gdat.OldData.Rpms - H.N.Rpms) * FPS;

				if (num19 < 40.0)
				{
					idleRPM = Convert.ToUInt16((1 + idleRPM + (int)H.N.Rpms) >> 1); // Refresh(): averaging with previous average
					++IdleSampleCount;								// Refresh(): increment if difference < 40
#if !slim
					double num20 = idleRPM * 0.008333333;		// Refresh(): some FrequencyMultiplier magic
					FrequencyMultiplier = num20 >= 5.0 ? (num20 >= 10.0 ? (num20 <= 20.0 ? (num20 <= 40.0 ? 1.0 : 0.25) : 0.5) : 2.0) : 4.0;
#endif
				}
				if (20 == IdleSampleCount && 0 == H.S.IdleRPM)	// Refresh(): change H.S.IdleRPM?
					H.S.Idle(idleRPM);							// Refresh() sniff: only if it was 0
			}

			if (FrameCountTicks % 5000000L <= 150000L)
			{
#if !slim
				SetRPMIntervals();
#endif
				SetRPMMix();
			}

			FreqHarmonic = H.N.Rpms * 0.008333333;
			FreqLFEAdaptive = FreqHarmonic * FrequencyMultiplier;
#if slim
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
#else
			FreqPeakA1 = FreqHarmonic * 1.5 * (1.0 + 8 * 0.08333333);
			FreqPeakB1 = FreqHarmonic * 0.75 * (1.0 + 8 * 0.08333333);
			FreqPeakA2 = FreqHarmonic * 0.5 * (1.0 + 8 * 0.08333333);
			FreqPeakB2 = FreqHarmonic * 1.25 * (1.0 + 8 * 0.08333333);
#endif
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
#if slim
									 ? (FreqHarmonic >= 150.0
									 ? (1 - (FreqHarmonic - 150) / (300 - 150)) * 90   // 150-300hz:  90-0
									: (1 - (FreqHarmonic - 125) / (125 - 1225)) * 88)  // 125-150hz:  88-90
									: (1 - (FreqHarmonic - 95) / (95 - 945)) * 85)	 //  95-125hz:  85-88
								: (1 - (FreqHarmonic - 65) / (65 - 137)) * 60)	 //   65-95hz:  60-85
							: (1 - (FreqHarmonic - 40) / (40 - 165)) * 50)	 //  40-65hz:   50-60
						: (1 - (FreqHarmonic - 25) / (25 - 160)) * 45)		//  25-40hz:  45-50
					: (1 - (FreqHarmonic - 0) / (0 - 200)) * 40;		   // 00-25hz:  40-45
#else
									? 75.0 - (FreqHarmonic - 125.0)
									: 95.0 - (FreqHarmonic - 95.0) * 0.667)
								: 65.0 + (FreqHarmonic - 65.0) * 1.0)
							: 52.5 + (FreqHarmonic - 40.0) * 0.5)
						: 40.0 + (FreqHarmonic - 25.0) * 0.834)
					: 30.0 + (FreqHarmonic - 15.0) * 1.0;
#endif
			Gain1H = Math.Max(Gain1H, 0.0) * num21 * num22 * (0.8 + 0.2 * MixPower + 0.2 * MixCylinder);
#if slim
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

			GainPeakA1 = FreqPeakA1 >= 30.0 ? (FreqPeakA1 >= 150.0 ? (FreqPeakA1 >= 250.0 ? (1 - (FreqPeakA1 - 250) / (450 - 250)) * 30 : (1 - (FreqPeakA1 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakA1 - 110) / (30 - 110)) * 60) : 0;
#else
			GainPeakA1 = FreqPeakA1 >= 30.0 ? (FreqPeakA1 >= 150.0 ? (FreqPeakA1 >= 250.0 ? (1 - (FreqPeakA1 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakA1 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakA1 - 110) / (30 - 110)) * 60) : 0;
#endif
			GainPeakA1 = Math.Max(GainPeakA1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
#if slim
			GainPeakA1Front = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1Rear = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1 = ((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
#else
			GainPeakA1Front = Math.Floor((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1Rear = Math.Floor((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA1 = Math.Floor((PeakA1Modifier * GainPeakA1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
#endif

#if slim
			GainPeakB1 = FreqPeakB1 >= 30.0 ? (FreqPeakB1 >= 150.0 ? (FreqPeakB1 >= 250.0 ? (1 - (FreqPeakB1 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakB1 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakB1 - 110) / (30 - 110)) * 60) : 0;
#else
			GainPeakB1 = FreqPeakB1 >= 55.0 ? (FreqPeakB1 >= 75.0 ? (FreqPeakB1 >= 105.0 ? 90.0 - (FreqPeakB1 - 105.0) * 0.75 : 60.0 + (FreqPeakB1 - 75.0) * 1.0) : 30.0 + (FreqPeakB1 - 55.0) * 1.5) : (FreqPeakB1 - 45.0) * 3.0;
#endif
			GainPeakB1 = Math.Max(GainPeakB1, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
#if slim
			GainPeakB1Front = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1Rear = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1 = ((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
#else
			GainPeakB1Front = Math.Floor((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1Rear = Math.Floor((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB1 = Math.Floor((PeakB1Modifier * GainPeakB1 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
#endif

#if slim
			GainPeakA2 = FreqPeakA2 >= 30.0 ? (FreqPeakA2 >= 150.0 ? (FreqPeakA2 >= 250.0 ? (1 - (FreqPeakA2 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakA2 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakA2 - 110) / (30 - 110)) * 60) : 0;
#else
			GainPeakA2 = FreqPeakA2 >= 55.0 ? (FreqPeakA2 >= 75.0 ? (FreqPeakA2 >= 105.0 ? 90.0 - (FreqPeakA2 - 105.0) * 0.75 : 60.0 + (FreqPeakA2 - 75.0) * 1.0) : 30.0 + (FreqPeakA2 - 55.0) * 1.5) : (FreqPeakA2 - 45.0) * 3.0;
#endif
			GainPeakA2 = Math.Max(GainPeakA2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
#if slim
			GainPeakA2Front = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2Rear = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakA2 = ((PeakA2Modifier * GainPeakA2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = FreqPeakB2 >= 30.0 ? (FreqPeakB2 >= 150.0 ? (FreqPeakB2 >= 250.0 ? (1 - (FreqPeakB2 - 250) / (450 - 250)) * 30
					   : (1 - (FreqPeakB2 - 150) / (300 - 150)) * 90) : (1 - (FreqPeakB2 - 110) / (30 - 110)) * 60) : 0;
			GainPeakB2 = Math.Max(GainPeakB2, 0.0) * (0.9 + 0.1 * MixPower + 0.1 * MixCylinder + 0.1 * MixTorque);
			GainPeakB2Front = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2Rear = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = ((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));
#endif
#if !slim
			GainPeakB2Front = Math.Floor((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixFront)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2Rear = Math.Floor((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixRear)).Clamp(0.0, sbyte.MaxValue));
			GainPeakB2 = Math.Floor((PeakB2Modifier * GainPeakB2 * (0.9 + 0.3 * MixMiddle)).Clamp(0.0, sbyte.MaxValue));

			if (H.S.EngineCylinders < 1.0)
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
			else if (H.S.EngineCylinders < 2.0)
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
#else
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
#endif
		}	// Refresh()
	}
}
