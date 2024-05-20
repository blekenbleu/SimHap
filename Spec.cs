﻿// Decompiled with JetBrains decompiler
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

		private string game;
		private string name;
		private string id;
		private string category;
		private ushort redline;
		private ushort maxRPM;
		private ushort idleRPM;
		private string engineConfiguration;
		private ushort engineCylinders;
		private string engineLocation;
		private string poweredWheels;
		private ushort maxPower;
		private ushort electricMaxPower;
		private ushort displacement;
		private ushort maxTorque;

		public Spec(Spec s) : this()
		{
			Game = s.game;
			Name = s.name;
			Id = s.id;
			Category = s.category;
			Redline = s.redline;
			MaxRPM = s.maxRPM;
			IdleRPM = s.idleRPM;
			EngineConfiguration = s.engineConfiguration;
			EngineCylinders = s.engineCylinders;
			EngineLocation = s.engineLocation;
			PoweredWheels = s.poweredWheels;
			MaxPower = s.maxPower;
			ElectricMaxPower = s.electricMaxPower;
			Displacement = s.displacement;
			MaxTorque = s.maxTorque;
		}

		internal Spec Import(Download d)
		{
			return new()
			{
				Game = d.game,
				Name = d.name,
				Id = d.id,
				Category = d.category,
				Redline = d.redline,
				MaxRPM = d.maxrpm,
				IdleRPM = d.idlerpm,
				EngineConfiguration = d.config,
				EngineCylinders = d.cyl,
				EngineLocation = d.loc,
				PoweredWheels = d.drive,
				MaxPower = d.hp,
				ElectricMaxPower = d.ehp,
				Displacement = d.cc,
				MaxTorque = d.nm,
			};
		}

		internal string Default(string game, StatusDataBase db, GameId CurrentGame)	
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

		internal Download Car => new()
		{
			cc = this.displacement,
			nm = this.maxTorque,
			ehp = this.electricMaxPower,
			hp = this.maxPower,
			drive = this.poweredWheels,
			config = this.engineConfiguration,
			cyl = this.engineCylinders,
			loc = this.engineLocation,
			maxrpm = this.maxRPM,
			idlerpm = this.idleRPM,
			redline = this.redline,
			category = this.category,
			name = this.name,
			id = this.id,
			game = this.game
		};

		internal Download Emit() => Car;

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

		public ushort Redline
		{
			get => this.redline;
			set { SetField(ref this.redline, value, nameof(Redline)); }
		}
	
		public ushort MaxRPM
		{
			get => this.maxRPM;
			set { SetField(ref this.maxRPM, value, nameof(MaxRPM)); }
		}
		public ushort IdleRPM
		{
			get => this.idleRPM;
			set { SetField(ref this.idleRPM, value, nameof(IdleRPM)); }
		}

		public string EngineConfiguration
		{
			get => this.engineConfiguration;
			set { SetField(ref this.engineConfiguration, value, nameof(EngineConfiguration)); }
		}
	
		public ushort EngineCylinders
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

		public ushort MaxPower
		{
			get => this.maxPower;
			set { SetField(ref this.maxPower, value, nameof(MaxPower)); }
		}
	
		public ushort ElectricMaxPower
		{
			get => this.electricMaxPower;
			set { SetField(ref this.electricMaxPower, value, nameof(ElectricMaxPower)); }
		}
	
		public ushort Displacement
		{
			get => this.displacement;
			set { SetField(ref this.displacement, value, nameof(Displacement)); }
		}
	
		public ushort MaxTorque
		{
			get => this.maxTorque;
			set { SetField(ref this.maxTorque, value, nameof(MaxTorque)); }
		}
	}
}
