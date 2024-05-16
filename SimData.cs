// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimData
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using System.ComponentModel;

namespace sierses.SimHap
{
	// see Spec.cs for public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	public class SimData : NotifyPropertyChanged
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
		public double IdleRPM;
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
		public double SlipXGammaBaseMult;
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
		private string gameAltText;
		private string loadStatusText;
		private string lockedText;
		private double engineMult;
		private double engineMultAll;
		private double motionPitchOffset;
		private double motionPitchMult;
		private double motionPitchGamma;
		private double motionRollOffset;
		private double motionRollMult;
		private double motionRollGamma;
		private double motionYawOffset;
		private double motionYawMult;
		private double motionYawGamma;
		private double motionHeaveOffset;
		private double motionHeaveMult;
		private double motionHeaveGamma;
		private double motionSurgeOffset;
		private double motionSurgeMult;
		private double motionSurgeGamma;
		private double motionSwayOffset;
		private double motionSwayMult;
		private double motionSwayGamma;
		private double rumbleMult;
		private double rumbleMultAll;
		private double suspensionMult;
		private double suspensionMultAll;
		private double suspensionGamma;
		private double suspensionGammaAll;
		private double slipXMult;
		private double slipXMultAll;
		private double slipXGamma;
		private double slipXGammaAll;
		private double slipYMult;
		private double slipYMultAll;
		private double slipYGamma;
		private double slipYGammaAll;

		Settings MySet;
		double GetSetting(string name, double trouble)	// Init() helper
		{
			return MySet.Motion.TryGetValue(name, out double num) ? num : trouble;
		}

		internal void Init (Settings Settings, string GameDBText)
		{
			MySet = Settings;
			EngineMult = Settings.EngineMult.TryGetValue(GameDBText, out double num) ? num : 1.0;
			EngineMultAll = Settings.EngineMult.TryGetValue("AllGames", out num) ? num : 1.0;
			RumbleMult = Settings.RumbleMult.TryGetValue(GameDBText, out num) ? num : 1.0;
			RumbleMultAll = Settings.RumbleMult.TryGetValue("AllGames", out num) ? num : 5.0;
			SuspensionMult = Settings.SuspensionMult.TryGetValue(GameDBText, out num) ? num : 1.0;
			SuspensionMultAll = Settings.SuspensionMult.TryGetValue("AllGames", out num) ? num : 1.5;
			SuspensionGamma = Settings.SuspensionGamma.TryGetValue(GameDBText, out num) ? num : 1.0;
			SuspensionGammaAll = Settings.SuspensionGamma.TryGetValue("AllGames", out num) ? num : 1.75;
			SlipXMult = Settings.SlipXMult.TryGetValue(GameDBText, out num) ? num : 1.0;
			SlipXMultAll = Settings.SlipXMult.TryGetValue("AllGames", out num) ? num : 1.6;
			SlipYMult = Settings.SlipYMult.TryGetValue(GameDBText, out num) ? num : 1.0;
			SlipYMultAll = Settings.SlipYMult.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipXGamma = Settings.SlipXGamma.TryGetValue(GameDBText, out num) ? num : 1.0;
			SlipXGammaAll = Settings.SlipXGamma.TryGetValue("AllGames", out num) ? num : 1.0;
			SlipYGamma = Settings.SlipYGamma.TryGetValue(GameDBText, out num) ? num : 1.0;
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

		public string GameAltText
		{
			get => this.gameAltText;
			set { SetField(ref this.gameAltText, value, nameof(GameAltText)); }
		}

		public string LoadStatusText
		{
			get => this.loadStatusText;
			set { SetField(ref this.loadStatusText, value, nameof(LoadStatusText)); }
		}

		public string LockedText
		{
			get => this.lockedText;
			set { SetField(ref this.lockedText, value, nameof(LockedText)); }
		}

		public double EngineMult
		{
			get => this.engineMult;
			set { SetField(ref this.engineMult, value, nameof(EngineMult)); }
		}

		public double EngineMultAll
		{
			get => this.engineMultAll;
			set { SetField(ref this.engineMultAll, value, nameof(EngineMultAll)); }
		}

		public double RumbleMult
		{
			get => this.rumbleMult;
			set { SetField(ref this.rumbleMult, value, nameof(RumbleMult)); }
		}

		public double RumbleMultAll
		{
			get => this.rumbleMultAll;
			set { SetField(ref this.rumbleMultAll, value, nameof(RumbleMultAll)); }
		}

		public double SuspensionGamma
		{
			get => this.suspensionGamma;
			set { SetField(ref this.suspensionGamma, value, nameof(SuspensionGamma)); }
		}

		public double SuspensionGammaAll
		{
			get => this.suspensionGammaAll;
			set { SetField(ref this.suspensionGammaAll, value, nameof(SuspensionGammaAll)); }
		}

		public double SuspensionMult
		{
			get => this.suspensionMult;
			set { SetField(ref this.suspensionMult, value, nameof(SuspensionMult)); }
		}

		public double SuspensionMultAll
		{
			get => this.suspensionMultAll;
			set { SetField(ref this.suspensionMultAll, value, nameof(SuspensionMultAll)); }
		}

		public double SlipXMult
		{
			get => this.slipXMult;
			set { SetField(ref this.slipXMult, value, nameof(SlipXMult)); }
		}

		public double SlipXMultAll
		{
			get => this.slipXMultAll;
			set { SetField(ref this.slipXMultAll, value, nameof(SlipXMultAll)); }
		}

		public double SlipXGamma
		{
			get => this.slipXGamma;
			set { SetField(ref this.slipXGamma, value, nameof(SlipXGamma)); }
		}

		public double SlipXGammaAll
		{
			get => this.slipXGammaAll;
			set { SetField(ref this.slipXGammaAll, value, nameof(SlipXGammaAll)); }
		}

		public double SlipYMult
		{
			get => this.slipYMult;
			set { SetField(ref this.slipYMult, value, nameof(SlipYMult)); }
		}

		public double SlipYMultAll
		{
			get => this.slipYMultAll;
			set { SetField(ref this.slipYMultAll, value, nameof(SlipYMultAll)); }
		}

		public double SlipYGamma
		{
			get => this.slipYGamma;
			set { SetField(ref this.slipYGamma, value, nameof(SlipYGamma)); }
		}

		public double SlipYGammaAll
		{
			get => this.slipYGammaAll;
			set { SetField(ref this.slipYGammaAll, value, nameof(SlipYGammaAll)); }
		}

		public double MotionPitchOffset
		{
			get => this.motionPitchOffset;
			set { SetField(ref this.motionPitchOffset, value, nameof(MotionPitchOffset)); }
		}

		public double MotionPitchMult
		{
			get => this.motionPitchMult;
			set { SetField(ref this.motionPitchMult, value, nameof(MotionPitchMult)); }
		}

		public double MotionPitchGamma
		{
			get => this.motionPitchGamma;
			set { SetField(ref this.motionPitchGamma, value, nameof(MotionPitchGamma)); }
		}

		public double MotionRollOffset
		{
			get => this.motionRollOffset;
			set { SetField(ref this.motionRollOffset, value, nameof(MotionRollOffset)); }
		}

		public double MotionRollMult
		{
			get => this.motionRollMult;
			set { SetField(ref this.motionRollMult, value, nameof(MotionRollMult)); }
		}

		public double MotionRollGamma
		{
			get => this.motionRollGamma;
			set { SetField(ref this.motionRollGamma, value, nameof(MotionRollGamma)); }
		}

		public double MotionYawOffset
		{
			get => this.motionYawOffset;
			set { SetField(ref this.motionYawOffset, value, nameof(MotionYawOffset)); }
		}

		public double MotionYawMult
		{
			get => this.motionYawMult;
			set { SetField(ref this.motionYawMult, value, nameof(MotionYawMult)); }
		}

		public double MotionYawGamma
		{
			get => this.motionYawGamma;
			set { SetField(ref this.motionYawGamma, value, nameof(MotionYawGamma)); }
		}

		public double MotionHeaveOffset
		{
			get => this.motionHeaveOffset;
			set { SetField(ref this.motionHeaveOffset, value, nameof(MotionHeaveOffset)); }
		}

		public double MotionHeaveMult
		{
			get => this.motionHeaveMult;
			set { SetField(ref this.motionHeaveMult, value, nameof(MotionHeaveMult)); }
		}

		public double MotionHeaveGamma
		{
			get => this.motionHeaveGamma;
			set { SetField(ref this.motionHeaveGamma, value, nameof(MotionHeaveGamma)); }
		}

		public double MotionSurgeOffset
		{
			get => this.motionSurgeOffset;
			set { SetField(ref this.motionSurgeOffset, value, nameof(MotionSurgeOffset)); }
		}

		public double MotionSurgeMult
		{
			get => this.motionSurgeMult;
			set { SetField(ref this.motionSurgeMult, value, nameof(MotionSurgeMult)); }
		}

		public double MotionSurgeGamma
		{
			get => this.motionSurgeGamma;
			set { SetField(ref this.motionSurgeGamma, value, nameof(MotionSurgeGamma)); }
		}

		public double MotionSwayOffset
		{
			get => this.motionSwayOffset;
			set { SetField(ref this.motionSwayOffset, value, nameof(MotionSwayOffset)); }
		}

		public double MotionSwayMult
		{
			get => this.motionSwayMult;
			set { SetField(ref this.motionSwayMult, value, nameof(MotionSwayMult)); }
		}

		public double MotionSwayGamma
		{
			get => this.motionSwayGamma;
			set { SetField(ref this.motionSwayGamma, value, nameof(MotionSwayGamma)); }
		}
	}
}
