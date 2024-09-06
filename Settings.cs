using System.Collections.Generic;
using System.ComponentModel;

namespace sierses.Sim
{
	public class Settings : INotifyPropertyChanged
	{
		public Spec Vehicle;
		public Dictionary<string, double> EngineMult;
		public Dictionary<string, double> RumbleMult;
		public Dictionary<string, double> SuspensionMult;
		public Dictionary<string, double> SuspensionGamma;
		public Dictionary<string, double> SlipXMult;
		public Dictionary<string, double> SlipYMult;
		public Dictionary<string, double> SlipXGamma;
		public Dictionary<string, double> SlipYGamma;
		public Dictionary<string, double> Motion;
		private int downshiftDurationMs;
		private int upshiftDurationMs;
#if slim
		public string Theme;
#else
		public Engine Engine;
		private int absPulseLength;

		public int ABSPulseLength
		{
			get => absPulseLength;
			set { SetProp(ref absPulseLength, value, nameof(ABSPulseLength)); }
		}
#endif
		public int DownshiftDurationMs
		{
			get => downshiftDurationMs;
			set { SetProp(ref downshiftDurationMs, value, nameof(DownshiftDurationMs)); }
		}

		public int UpshiftDurationMs
		{
			get => upshiftDurationMs;
			set { SetProp(ref upshiftDurationMs, value, nameof(UpshiftDurationMs)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetProp<T>(ref T property, T value, string propertyname)
		{
			if (EqualityComparer<T>.Default.Equals(property, value))
				return false;

			property = value;
			PropertyChangedEventHandler handle = PropertyChanged;
			if (handle == null)
				return false;

			handle((object) this, new PropertyChangedEventArgs(propertyname));
			return true;
		}
	}
}
