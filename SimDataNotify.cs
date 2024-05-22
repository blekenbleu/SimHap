
namespace sierses.SimHap
{
	// see Spec.cs for public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	public partial class SimData : NotifyPropertyChanged	// The whole class inherits NotifyPropertyChanged
	{
		private string gameAltText;
		private string loadStatusText;
		private string lockedText;
		private double engineMult;
		private double engineMultAll;
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

		public string GameAltText
		{
			get => this.gameAltText;
			set { SetQuiet(ref this.gameAltText, value, nameof(GameAltText)); }
		}

		public string LoadText
		{
			get => this.loadStatusText;
			set { SetQuiet(ref this.loadStatusText, value, nameof(LoadText)); }
		}

		public string LockedText
		{
			get => this.lockedText;
			set { SetQuiet(ref this.lockedText, value, nameof(LockedText)); }
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
