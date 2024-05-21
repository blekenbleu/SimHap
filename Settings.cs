// Decompiled with JetBrains decompiler
// Type: SimHaptics.Settings
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using System.Collections.Generic;
using System.ComponentModel;

namespace sierses.SimHap
{
	public class Settings : INotifyPropertyChanged
	{
		public Spec Vehicle = new();
		public Dictionary<string, double> EngineMult;
		public Dictionary<string, double> RumbleMult;
		public Dictionary<string, double> SuspensionMult;
		public Dictionary<string, double> SuspensionGamma;
		public Dictionary<string, double> SlipXMult;
		public Dictionary<string, double> SlipYMult;
		public Dictionary<string, double> SlipXGamma;
		public Dictionary<string, double> SlipYGamma;
		public Dictionary<string, double> Motion;
		private bool unlocked = true;
		private int absPulseLength;
		private int downshiftDurationMs;
		private int upshiftDurationMs;

		public bool Unlocked
		{
			get => this.unlocked;
			set { SetProp(ref this.unlocked, value, nameof(Unlocked)); }
		}

		public int ABSPulseLength
		{
			get => this.absPulseLength;
			set { SetProp(ref this.absPulseLength, value, nameof(ABSPulseLength)); }
		}

		public int DownshiftDurationMs
		{
			get => this.downshiftDurationMs;
			set { SetProp(ref this.downshiftDurationMs, value, nameof(DownshiftDurationMs)); }
		}

		public int UpshiftDurationMs
		{
			get => this.upshiftDurationMs;
			set { SetProp(ref this.upshiftDurationMs, value, nameof(UpshiftDurationMs)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetProp<T>(ref T property, T value, string propertyName)
		{
			if (EqualityComparer<T>.Default.Equals(property, value))
				return false;

			property = value;
			PropertyChangedEventHandler handle = this.PropertyChanged;
			if (handle == null)
				return false;

			handle((object) this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
