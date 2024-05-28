// Decompiled with JetBrains decompiler
// Type: SimHaptics.Settings
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

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
		private int absPulseLength;
		private int downshiftDurationMs;
		private int upshiftDurationMs;

		public int ABSPulseLength
		{
			get => absPulseLength;
			set { SetProp(ref absPulseLength, value, nameof(ABSPulseLength)); }
		}

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
