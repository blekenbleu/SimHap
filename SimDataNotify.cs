namespace sierses.Sim
{
	// see Spec.cs for public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	public partial class SimData : NotifyPropertyChanged	// The whole class inherits NotifyPropertyChanged
	{
		private string gameAltText;
		private string loadStatusText;
		private string lockedText;
		private double suspensionMult;
		private double suspensionMultAll;
		private double suspensionGamma;
		private double suspensionGammaAll;
#if !slim
		private double engineMult;
		private double engineMultAll;
		private double rumbleMult;
		private double rumbleMultAll;
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
#endif
		private bool locked = false;
		public bool Locked
		{
			get => locked;
			set { SetField(ref locked, value, nameof(Locked)); }
		}

		public string GameAltText
		{
			get => gameAltText;
			set { SetField(ref gameAltText, value, nameof(GameAltText)); }
		}

		public string LoadText
		{
			get => loadStatusText;
			set { SetField(ref loadStatusText, value, nameof(LoadText)); }
		}

		public string LockedText
		{
			get => lockedText;
			set { SetField(ref lockedText, value, nameof(LockedText)); }
		}

		public double SuspensionGamma
		{
			get => suspensionGamma;
			set { SetField(ref suspensionGamma, value, nameof(SuspensionGamma)); }
		}

		public double SuspensionGammaAll
		{
			get => suspensionGammaAll;
			set { SetField(ref suspensionGammaAll, value, nameof(SuspensionGammaAll)); }
		}

		public double SuspensionMult
		{
			get => suspensionMult;
			set { SetField(ref suspensionMult, value, nameof(SuspensionMult)); }
		}

		public double SuspensionMultAll
		{
			get => suspensionMultAll;
			set { SetField(ref suspensionMultAll, value, nameof(SuspensionMultAll)); }
		}
#if !slim
		public double EngineMult
		{
			get => engineMult;
			set { SetField(ref engineMult, value, nameof(EngineMult)); }
		}

		public double EngineMultAll
		{
			get => engineMultAll;
			set { SetField(ref engineMultAll, value, nameof(EngineMultAll)); }
		}

		public double RumbleMult
		{
			get => rumbleMult;
			set { SetField(ref rumbleMult, value, nameof(RumbleMult)); }
		}

		public double RumbleMultAll
		{
			get => rumbleMultAll;
			set { SetField(ref rumbleMultAll, value, nameof(RumbleMultAll)); }
		}

		public double SlipXMult
		{
			get => slipXMult;
			set { SetField(ref slipXMult, value, nameof(SlipXMult)); }
		}

		public double SlipXMultAll
		{
			get => slipXMultAll;
			set { SetField(ref slipXMultAll, value, nameof(SlipXMultAll)); }
		}

		public double SlipXGamma
		{
			get => slipXGamma;
			set { SetField(ref slipXGamma, value, nameof(SlipXGamma)); }
		}

		public double SlipXGammaAll
		{
			get => slipXGammaAll;
			set { SetField(ref slipXGammaAll, value, nameof(SlipXGammaAll)); }
		}

		public double SlipYMult
		{
			get => slipYMult;
			set { SetField(ref slipYMult, value, nameof(SlipYMult)); }
		}

		public double SlipYMultAll
		{
			get => slipYMultAll;
			set { SetField(ref slipYMultAll, value, nameof(SlipYMultAll)); }
		}

		public double SlipYGamma
		{
			get => slipYGamma;
			set { SetField(ref slipYGamma, value, nameof(SlipYGamma)); }
		}

		public double SlipYGammaAll
		{
			get => slipYGammaAll;
			set { SetField(ref slipYGammaAll, value, nameof(SlipYGammaAll)); }
		}

		public double MotionPitchOffset
		{
			get => motionPitchOffset;
			set { SetField(ref motionPitchOffset, value, nameof(MotionPitchOffset)); }
		}

		public double MotionPitchMult
		{
			get => motionPitchMult;
			set { SetField(ref motionPitchMult, value, nameof(MotionPitchMult)); }
		}

		public double MotionPitchGamma
		{
			get => motionPitchGamma;
			set { SetField(ref motionPitchGamma, value, nameof(MotionPitchGamma)); }
		}

		public double MotionRollOffset
		{
			get => motionRollOffset;
			set { SetField(ref motionRollOffset, value, nameof(MotionRollOffset)); }
		}

		public double MotionRollMult
		{
			get => motionRollMult;
			set { SetField(ref motionRollMult, value, nameof(MotionRollMult)); }
		}

		public double MotionRollGamma
		{
			get => motionRollGamma;
			set { SetField(ref motionRollGamma, value, nameof(MotionRollGamma)); }
		}

		public double MotionYawOffset
		{
			get => motionYawOffset;
			set { SetField(ref motionYawOffset, value, nameof(MotionYawOffset)); }
		}

		public double MotionYawMult
		{
			get => motionYawMult;
			set { SetField(ref motionYawMult, value, nameof(MotionYawMult)); }
		}

		public double MotionYawGamma
		{
			get => motionYawGamma;
			set { SetField(ref motionYawGamma, value, nameof(MotionYawGamma)); }
		}

		public double MotionHeaveOffset
		{
			get => motionHeaveOffset;
			set { SetField(ref motionHeaveOffset, value, nameof(MotionHeaveOffset)); }
		}

		public double MotionHeaveMult
		{
			get => motionHeaveMult;
			set { SetField(ref motionHeaveMult, value, nameof(MotionHeaveMult)); }
		}

		public double MotionHeaveGamma
		{
			get => motionHeaveGamma;
			set { SetField(ref motionHeaveGamma, value, nameof(MotionHeaveGamma)); }
		}

		public double MotionSurgeOffset
		{
			get => motionSurgeOffset;
			set { SetField(ref motionSurgeOffset, value, nameof(MotionSurgeOffset)); }
		}

		public double MotionSurgeMult
		{
			get => motionSurgeMult;
			set { SetField(ref motionSurgeMult, value, nameof(MotionSurgeMult)); }
		}

		public double MotionSurgeGamma
		{
			get => motionSurgeGamma;
			set { SetField(ref motionSurgeGamma, value, nameof(MotionSurgeGamma)); }
		}

		public double MotionSwayOffset
		{
			get => motionSwayOffset;
			set { SetField(ref motionSwayOffset, value, nameof(MotionSwayOffset)); }
		}

		public double MotionSwayMult
		{
			get => motionSwayMult;
			set { SetField(ref motionSwayMult, value, nameof(MotionSwayMult)); }
		}

		public double MotionSwayGamma
		{
			get => motionSwayGamma;
			set { SetField(ref motionSwayGamma, value, nameof(MotionSwayGamma)); }
		}
#endif
	}
}
