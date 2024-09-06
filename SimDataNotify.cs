
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
		private bool unlocked = true;
		public bool Unlocked
		{
			get => unlocked;
			set { SetField(ref unlocked, value, nameof(Unlocked)); }
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
	}
}
