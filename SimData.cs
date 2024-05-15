// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimData
// Assembly: SimHaptics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
// Assembly location: C:\Users\demas\Downloads\dnSpy-net-win64\SimHaptics.dll

using System.ComponentModel;

namespace sierses.SimHap
{
  public class SimData : INotifyPropertyChanged
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

    public string GameAltText
    {
      get => this.gameAltText;
      set
      {
        this.gameAltText = value;
        this.OnPropertyChanged("gameAltText");
      }
    }

    public string LoadStatusText
    {
      get => this.loadStatusText;
      set
      {
        this.loadStatusText = value;
        this.OnPropertyChanged("loadStatusText");
      }
    }

    public string LockedText
    {
      get => this.lockedText;
      set
      {
        this.lockedText = value;
        this.OnPropertyChanged("lockedText");
      }
    }

    public double EngineMult
    {
      get => this.engineMult;
      set
      {
        this.engineMult = value;
        this.OnPropertyChanged("engineMult");
      }
    }

    public double EngineMultAll
    {
      get => this.engineMultAll;
      set
      {
        this.engineMultAll = value;
        this.OnPropertyChanged("engineMultAll");
      }
    }

    public double RumbleMult
    {
      get => this.rumbleMult;
      set
      {
        this.rumbleMult = value;
        this.OnPropertyChanged("rumbleMult");
      }
    }

    public double RumbleMultAll
    {
      get => this.rumbleMultAll;
      set
      {
        this.rumbleMultAll = value;
        this.OnPropertyChanged("rumbleMultAll");
      }
    }

    public double SuspensionGamma
    {
      get => this.suspensionGamma;
      set
      {
        this.suspensionGamma = value;
        this.OnPropertyChanged("suspensionGamma");
      }
    }

    public double SuspensionGammaAll
    {
      get => this.suspensionGammaAll;
      set
      {
        this.suspensionGammaAll = value;
        this.OnPropertyChanged("suspensionGammaAll");
      }
    }

    public double SuspensionMult
    {
      get => this.suspensionMult;
      set
      {
        this.suspensionMult = value;
        this.OnPropertyChanged("suspensionMult");
      }
    }

    public double SuspensionMultAll
    {
      get => this.suspensionMultAll;
      set
      {
        this.suspensionMultAll = value;
        this.OnPropertyChanged("suspensionMultAll");
      }
    }

    public double SlipXMult
    {
      get => this.slipXMult;
      set
      {
        this.slipXMult = value;
        this.OnPropertyChanged("slipXMult");
      }
    }

    public double SlipXMultAll
    {
      get => this.slipXMultAll;
      set
      {
        this.slipXMultAll = value;
        this.OnPropertyChanged("slipXMultAll");
      }
    }

    public double SlipXGamma
    {
      get => this.slipXGamma;
      set
      {
        this.slipXGamma = value;
        this.OnPropertyChanged("slipXGamma");
      }
    }

    public double SlipXGammaAll
    {
      get => this.slipXGammaAll;
      set
      {
        this.slipXGammaAll = value;
        this.OnPropertyChanged("slipXGammaAll");
      }
    }

    public double SlipYMult
    {
      get => this.slipYMult;
      set
      {
        this.slipYMult = value;
        this.OnPropertyChanged("slipYMult");
      }
    }

    public double SlipYMultAll
    {
      get => this.slipYMultAll;
      set
      {
        this.slipYMultAll = value;
        this.OnPropertyChanged("slipYMultAll");
      }
    }

    public double SlipYGamma
    {
      get => this.slipYGamma;
      set
      {
        this.slipYGamma = value;
        this.OnPropertyChanged("slipYGamma");
      }
    }

    public double SlipYGammaAll
    {
      get => this.slipYGammaAll;
      set
      {
        this.slipYGammaAll = value;
        this.OnPropertyChanged("slipYGammaAll");
      }
    }

    public double MotionPitchOffset
    {
      get => this.motionPitchOffset;
      set
      {
        this.motionPitchOffset = value;
        this.OnPropertyChanged("motionPitchOffset");
      }
    }

    public double MotionPitchMult
    {
      get => this.motionPitchMult;
      set
      {
        this.motionPitchMult = value;
        this.OnPropertyChanged("motionPitchMult");
      }
    }

    public double MotionPitchGamma
    {
      get => this.motionPitchGamma;
      set
      {
        this.motionPitchGamma = value;
        this.OnPropertyChanged("motionPitchGamma");
      }
    }

    public double MotionRollOffset
    {
      get => this.motionRollOffset;
      set
      {
        this.motionRollOffset = value;
        this.OnPropertyChanged("motionRollOffset");
      }
    }

    public double MotionRollMult
    {
      get => this.motionRollMult;
      set
      {
        this.motionRollMult = value;
        this.OnPropertyChanged("motionRollMult");
      }
    }

    public double MotionRollGamma
    {
      get => this.motionRollGamma;
      set
      {
        this.motionRollGamma = value;
        this.OnPropertyChanged("motionRollGamma");
      }
    }

    public double MotionYawOffset
    {
      get => this.motionYawOffset;
      set
      {
        this.motionYawOffset = value;
        this.OnPropertyChanged("motionYawOffset");
      }
    }

    public double MotionYawMult
    {
      get => this.motionYawMult;
      set
      {
        this.motionYawMult = value;
        this.OnPropertyChanged("motionYawMult");
      }
    }

    public double MotionYawGamma
    {
      get => this.motionYawGamma;
      set
      {
        this.motionYawGamma = value;
        this.OnPropertyChanged("motionYawGamma");
      }
    }

    public double MotionHeaveOffset
    {
      get => this.motionHeaveOffset;
      set
      {
        this.motionHeaveOffset = value;
        this.OnPropertyChanged("motionHeaveOffset");
      }
    }

    public double MotionHeaveMult
    {
      get => this.motionHeaveMult;
      set
      {
        this.motionHeaveMult = value;
        this.OnPropertyChanged("motionHeaveMult");
      }
    }

    public double MotionHeaveGamma
    {
      get => this.motionHeaveGamma;
      set
      {
        this.motionHeaveGamma = value;
        this.OnPropertyChanged("motionHeaveGamma");
      }
    }

    public double MotionSurgeOffset
    {
      get => this.motionSurgeOffset;
      set
      {
        this.motionSurgeOffset = value;
        this.OnPropertyChanged("motionSurgeOffset");
      }
    }

    public double MotionSurgeMult
    {
      get => this.motionSurgeMult;
      set
      {
        this.motionSurgeMult = value;
        this.OnPropertyChanged("motionSurgeMult");
      }
    }

    public double MotionSurgeGamma
    {
      get => this.motionSurgeGamma;
      set
      {
        this.motionSurgeGamma = value;
        this.OnPropertyChanged("motionSurgeGamma");
      }
    }

    public double MotionSwayOffset
    {
      get => this.motionSwayOffset;
      set
      {
        this.motionSwayOffset = value;
        this.OnPropertyChanged("motionSwayOffset");
      }
    }

    public double MotionSwayMult
    {
      get => this.motionSwayMult;
      set
      {
        this.motionSwayMult = value;
        this.OnPropertyChanged("motionSwayMult");
      }
    }

    public double MotionSwayGamma
    {
      get => this.motionSwayGamma;
      set
      {
        this.motionSwayGamma = value;
        this.OnPropertyChanged("motionSwayGamma");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
