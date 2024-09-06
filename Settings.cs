using System.Collections.Generic;
using System.ComponentModel;

namespace sierses.Sim
{
	public class Settings : INotifyPropertyChanged
	{
//		public Engine Engine;
		public string Theme;
		public Spec Vehicle;
		public Dictionary<string, double> SuspensionMult;
		public Dictionary<string, double> SuspensionGamma;
		private int downshiftDurationMs;
		private int upshiftDurationMs;


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
