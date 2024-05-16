// Decompiled with JetBrains decompiler
// Type: SimHaptics.Spec
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using System.Collections.Generic;
using System.ComponentModel;

namespace sierses.SimHap
{
	/// <summary>
	/// Abstract base class to implement INotifyPropertyChanged interface
	/// https://gist.github.com/itajaja/7439345
	/// </summary>
	public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		protected bool SetField<T>(ref T field, T value, string propertyName)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;
			SimHapticsPlugin.Changed = true;
			OnPropertyChanged(propertyName);
			return true;
		}
	}


	public class ListDictionary
	{
		private Dictionary<string, List<Download>> internalDictionary = new();
		public void Add(string key, Download value)
		{
			if (this.internalDictionary.ContainsKey(key))
				this.internalDictionary[key].Add(value);
			else this.internalDictionary.Add(key, new() { value });
		}
	}

	public class DownloadData
	{
		public string notes;
		public ushort cc;
		public ushort nm;
		public ushort ehp;
		public ushort hp;
		public string drive;
		public string config;
		public ushort cyl;
		public string loc;
		public ushort maxrpm;
		public ushort redline;
		public string category;
		public string name;
		public string id;
		public string game;
	}

	public class Download
	{
		public DownloadData[] data;
	}

	public class Spec : NotifyPropertyChanged
	{
		private string game;
		private string name;
		private string id;
		private string category;
		private double redline;
		private double maxRPM;
		private string engineConfiguration;
		private double engineCylinders;
		private string engineLocation;
		private string poweredWheels;
		private double maxPower;
		private double electricMaxPower;
		private double displacement;
		private double maxTorque;

		public Spec()
		{
		}

		public Spec(Spec s) : this()
		{
			this.game = s.game;
			this.name = s.name;
			this.id = s.id;
			this.category = s.category;
			this.redline = s.redline;
			this.maxRPM = s.maxRPM;
			this.engineConfiguration = s.engineConfiguration;
			this.engineCylinders = s.engineCylinders;
			this.engineLocation = s.engineLocation;
			this.poweredWheels = s.poweredWheels;
			this.maxPower = s.maxPower;
			this.electricMaxPower = s.electricMaxPower;
			this.displacement = s.displacement;
			this.maxTorque = s.maxTorque;
		}

		public string Game
		{
			get => this.game;
			set { SetField(ref this.game, value, nameof(Game)); }
		}

		public string Name
		{
			get => this.name;
			set { SetField(ref this.name, value, nameof(Name)); }
		}

		public string Id
		{
			get => this.id;
			set { SetField(ref this.id, value, nameof(Id)); }
		}

		public string Category
		{
			get => this.category;
			set { SetField(ref this.category, value, nameof(Category)); }
		}

		public double Redline
		{
			get => this.redline;
			set { SetField(ref this.redline, value, nameof(Redline)); }
		}
	
		public double MaxRPM
		{
			get => this.maxRPM;
			set { SetField(ref this.maxRPM, value, nameof(MaxRPM)); }
		}

		public string EngineConfiguration
		{
			get => this.engineConfiguration;
			set { SetField(ref this.engineConfiguration, value, nameof(EngineConfiguration)); }
		}
	
		public double EngineCylinders
		{
			get => this.engineCylinders;
			set { SetField(ref this.engineCylinders, value, nameof(EngineCylinders)); }
		}

		public string EngineLocation
		{
			get => this.engineLocation;
			set { SetField(ref this.engineLocation, value, nameof(EngineLocation)); }
		}

		public string PoweredWheels
		{
			get => this.poweredWheels;
			set { SetField(ref this.poweredWheels, value, nameof(PoweredWheels)); }
		}

		public double MaxPower
		{
			get => this.maxPower;
			set { SetField(ref this.maxPower, value, nameof(MaxPower)); }
		}
	
		public double ElectricMaxPower
		{
			get => this.electricMaxPower;
			set { SetField(ref this.electricMaxPower, value, nameof(ElectricMaxPower)); }
		}
	
		public double Displacement
		{
			get => this.displacement;
			set { SetField(ref this.displacement, value, nameof(Displacement)); }
		}
	
		public double MaxTorque
		{
			get => this.maxTorque;
			set { SetField(ref this.maxTorque, value, nameof(MaxTorque)); }
		}
	}
}
