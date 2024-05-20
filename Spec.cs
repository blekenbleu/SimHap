// Decompiled with JetBrains decompiler
// Type: SimHaptics.Spec
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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


	public class ListDictionary : NotifyPropertyChanged
	{
		private Dictionary<string, List<Download>> internalDictionary = new();

		public bool Add(Download value)
		{
			if (null == value || null == value.game || null == value.id || 0 == value.game.Length || 0 == value.id.Length)
				return false;
			if (this.internalDictionary.ContainsKey(value.game))
			{
				int Index = this.internalDictionary[value.game].FindIndex(x => x.id == value.id);
				if (0 <= Index)
					this.internalDictionary[value.game][Index] = value;
				else this.internalDictionary[value.game].Add(value);
			}
			else this.internalDictionary.Add(value.game, new() { value });
			return true;
		}

		public Dictionary<string, List<Download>> InternalDictionary
		{
			get => this.internalDictionary;
			set { SetField(ref this.internalDictionary, value, nameof(InternalDictionary)); }
		}

		public List<Download> Extract(string game)
		{
			if (!internalDictionary.ContainsKey(game))
				internalDictionary.Add(game, new() {});
			return internalDictionary[game];
		}
	}

	public class Download
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
		public ushort idlerpm;
		public ushort redline;
		public string category;
		public string name;
		public string id;
		public string game;
	}

	// format for downloading from website
	public class Download_array
	{
		public Download[] data;
	}

	public class Spec : NotifyPropertyChanged
	{
		public Spec()
		{
		}

		private Download car { get; set; }

		public Spec(Spec s)
		{
			Import(s.car);
		}

		internal void Import(Download d)
		{
			Game = d.game;
			Name = d.name;
			Id = d.id;
			Category = d.category;
			Redline = d.redline;
			MaxRPM = d.maxrpm;
			IdleRPM = d.idlerpm;
			EngineConfiguration = d.config;
			EngineCylinders = d.cyl;
			EngineLocation = d.loc;
			PoweredWheels = d.drive;
			MaxPower = d.hp;
			ElectricMaxPower = d.ehp;
			Displacement = d.cc;
			MaxTorque = d.nm;
		}

		internal string Defaults(string game, StatusDataBase db, GameId CurrentGame)	
		{
			string StatusText;

			Game = game;
			Name = db.CarModel;
			Id = db.CarId;
			Category = db.CarClass;
			EngineConfiguration = "V";
			EngineCylinders = 6;
			EngineLocation = "RM";
			PoweredWheels = "A";
			Displacement = 3000;
			MaxPower = 300;
			ElectricMaxPower = 0;
			MaxTorque = 250;
			IdleRPM = 0;

			switch (CurrentGame)
			{
				case GameId.AC:
				case GameId.ACC:
				case GameId.AMS1:
				case GameId.AMS2:
				case GameId.Forza:
				case GameId.GTR2:
				case GameId.IRacing:
				case GameId.PC2:
				case GameId.RBR:
				case GameId.RF2:
				case GameId.RRRE:
				case GameId.BeamNG:
					StatusText = "Not in DB: using generic car";
					break;
				case GameId.D4:
				case GameId.DR2:
				case GameId.WRC23:
					StatusText = "Not in DB: using generic Rally2";
					EngineConfiguration = "I";
					EngineCylinders = 4;
					EngineLocation = "F";
					PoweredWheels = "A";
					Displacement = 1600;
					MaxPower = 300;
					ElectricMaxPower = 0;
					MaxTorque = 400;
					break;
				case GameId.F12022:
				case GameId.F12023:
					StatusText = "Not in DB: using generic F1";
					EngineConfiguration = "V";
					EngineCylinders = 6;
					EngineLocation = "RM";
					PoweredWheels = "R";
					Displacement = 1600;
					MaxPower = 1000;
					ElectricMaxPower = 0;
					MaxTorque = 650;
					break;
				case GameId.KK:
					StatusText = "Not in DB: using generic Kart";
					EngineConfiguration = "I";
					EngineCylinders = 1;
					EngineLocation = "RM";
					PoweredWheels = "R";
					Displacement = 130;
					MaxPower = 34;
					ElectricMaxPower = 0;
					MaxTorque = 24;
					break;
				case GameId.GPBikes:
					StatusText = "Not in DB: using generic Superbike";
					EngineConfiguration = "I";
					EngineCylinders = 4;
					EngineLocation = "M";
					PoweredWheels = "R";
					Displacement = 998;
					MaxPower = 200;
					ElectricMaxPower = 0;
					MaxTorque = 100;
					break;
				case GameId.MXBikes:
					StatusText = "Not in DB: using generic MX Bike";
					EngineConfiguration = "I";
					EngineCylinders = 1;
					EngineLocation = "M";
					PoweredWheels = "R";
					Displacement = 450;
					MaxPower = 50;
					ElectricMaxPower = 0;
					MaxTorque = 45;
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					StatusText = "Not in DB: redline loaded from game";
					EngineConfiguration = "V";
					EngineCylinders = 6;
					EngineLocation = "RM";
					PoweredWheels = "R";
					Displacement = 4000;
					MaxPower = 500;
					ElectricMaxPower = 0;
					MaxTorque = 400;
					break;
				default:
					StatusText = $"Specs unavailable for {CurrentGame}";
					break;
			}
			return StatusText;
		}

		internal Download Car { get => this.car; }

		public string Game
		{
			get => this.car.game;
			set { SetField(ref this.car.game, value, nameof(Game)); }
		}

		public string Name
		{
			get => this.car.name;
			set { SetField(ref this.car.name, value, nameof(Name)); }
		}

		public string Id
		{
			get => this.car.id;
			set { SetField(ref this.car.id, value, nameof(Id)); }
		}

		public string Category
		{
			get => this.car.category;
			set { SetField(ref this.car.category, value, nameof(Category)); }
		}

		public ushort Redline
		{
			get => this.car.redline;
			set { SetField(ref this.car.redline, value, nameof(Redline)); }
		}
	
		public ushort MaxRPM
		{
			get => this.car.maxrpm;
			set { SetField(ref this.car.maxrpm, value, nameof(MaxRPM)); }
		}
		public ushort IdleRPM
		{
			get => this.car.idlerpm;
			set { SetField(ref this.car.idlerpm, value, nameof(IdleRPM)); }
		}

		public string EngineConfiguration
		{
			get => this.car.config;
			set { SetField(ref this.car.config, value, nameof(EngineConfiguration)); }
		}
	
		public ushort EngineCylinders
		{
			get => this.car.cyl;
			set { SetField(ref this.car.cyl, value, nameof(EngineCylinders)); }
		}

		public string EngineLocation
		{
			get => this.car.loc;
			set { SetField(ref this.car.loc, value, nameof(EngineLocation)); }
		}

		public string PoweredWheels
		{
			get => this.car.drive;
			set { SetField(ref this.car.drive, value, nameof(PoweredWheels)); }
		}

		public ushort MaxPower
		{
			get => this.car.hp;
			set { SetField(ref this.car.hp, value, nameof(MaxPower)); }
		}
	
		public ushort ElectricMaxPower
		{
			get => this.car.ehp;
			set { SetField(ref this.car.ehp, value, nameof(ElectricMaxPower)); }
		}
	
		public ushort Displacement
		{
			get => this.car.cc;
			set { SetField(ref this.car.cc, value, nameof(Displacement)); }
		}
	
		public ushort MaxTorque
		{
			get => this.car.nm;
			set { SetField(ref this.car.nm, value, nameof(MaxTorque)); }
		}
	}
}
