// Decompiled with JetBrains decompiler
// Type: SimHaptics.Spec
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;
using Newtonsoft.Json;
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

		protected bool SetQuiet<T>(ref T field, T value, string propertyName)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected bool SetField<T>(ref T field, T value, string propertyName)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;
			OnPropertyChanged(propertyName);
			return SimHapticsPlugin.Changed = true;
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

	public class ListDictionary : NotifyPropertyChanged
	{
		private Dictionary<string, List<Download>> internalDictionary;

		internal bool Add(List<Download> s)
		{
			int Index;

			if (null == s || 0 == s.Count)
				return false;

			if (null == internalDictionary)
				internalDictionary = new();
			if (internalDictionary.ContainsKey(s[0].game))
				for (int i = 0; i < s.Count; i++)
				{
					Index = internalDictionary[s[0].game].FindIndex(x => x.id == s[i].id);
					if (0 <= Index)
						internalDictionary[s[0].game][Index] = s[i];
					else internalDictionary[s[0].game].Add(s[i]);
				}
			else
			{
				List<Download> l = new();
				for (int i = 0; i < s.Count; i++)
					l.Add(s[i]);
				internalDictionary.Add(s[0].game, l);
			}
			return true;
		}
/*
		internal bool Add(Download[] s)
		{
			int Index;

			if (null == s || 0 == s.Length)
				return false;

			if (null == internalDictionary)
				internalDictionary = new();
			if (internalDictionary.ContainsKey(s[0].game))
				for (int i = 0; i < s.Length; i++)
				{
					Index = internalDictionary[s[0].game].FindIndex(x => x.id == s[i].id);
					if (0 <= Index)
						internalDictionary[s[0].game][Index] = s[i];
					else internalDictionary[s[0].game].Add(s[i]);
				}
			else
			{
				List<Download> l = new();
				for (int i = 0; i < s.Length; i++)
					l.Add(s[i]);
				internalDictionary.Add(s[0].game, l);
			}
			return true;
		}

		internal bool Add(Spec s)
		{
			int Index;

			if (null == s)
				return false;

			Download car = s.Car;
			if (null == car || null == car.game || null == car.id || 0 == car.game.Length || 0 == car.id.Length)
				return false;

			if (null == internalDictionary)
				internalDictionary = new();
			if (internalDictionary.ContainsKey(car.game))
			{
				Index = internalDictionary[car.game].FindIndex(x => x.id == car.id);
				if (0 <= Index)
					internalDictionary[car.game][Index] = car;

				else
					internalDictionary[car.game].Add(car);
			}
			else internalDictionary.Add(car.game, new List<Download> { car });
			return true;
		}
 */
		public bool Load(Dictionary<string, List<Download>> json)
		{
			if (null == json || 0 == json.Count)
				return false;
			internalDictionary = json;
			return true;
		}

		internal string Jstring()
		{
			return JsonConvert.SerializeObject(internalDictionary, Formatting.Indented);
		}

		internal ushort Count
		{
			get { return (ushort)internalDictionary.Count; }
		}

		public List<Download> Extract(string game)
		{
			if (!internalDictionary.ContainsKey(game))
				return null;
			return internalDictionary[game];
		}
	}	// class ListDictionary

	public class Spec : NotifyPropertyChanged
	{
		public Spec()
		{
			Private_Car = new Download();
		}

		private Download Private_Car { get; set; }

		internal Download Car { get => Private_Car; }

		public Spec(Spec s)
		{
			Private_Car = s.Private_Car;
		}

		// makes sense only as Spec instance.Import(download)
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

			if (null != game)
				Game = game;
			else return "SimHap.Spec.Defaults():  null game";
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

		public string Game
		{
			get => Private_Car.game;
			set { SetField(ref Private_Car.game, value, nameof(Game)); }
		}

		public string Name
		{
			get => Private_Car.name;
			set { SetField(ref Private_Car.name, value, nameof(Name)); }
		}

		public string Id
		{
			get => Private_Car.id;
			set { SetField(ref Private_Car.id, value, nameof(Id)); }
		}

		public string Category
		{
			get => Private_Car.category;
			set { SetField(ref Private_Car.category, value, nameof(Category)); }
		}

		public ushort Redline
		{
			get => Private_Car.redline;
			set { SetField(ref Private_Car.redline, value, nameof(Redline)); }
		}
	
		public ushort MaxRPM
		{
			get => Private_Car.maxrpm;
			set { SetField(ref Private_Car.maxrpm, value, nameof(MaxRPM)); }
		}
		public ushort IdleRPM
		{
			get => Private_Car.idlerpm;
			set { SetField(ref Private_Car.idlerpm, value, nameof(IdleRPM)); }
		}

		public string EngineConfiguration
		{
			get => Private_Car.config;
			set { SetField(ref Private_Car.config, value, nameof(EngineConfiguration)); }
		}
	
		public ushort EngineCylinders
		{
			get => Private_Car.cyl;
			set { SetField(ref Private_Car.cyl, value, nameof(EngineCylinders)); }
		}

		public string EngineLocation
		{
			get => Private_Car.loc;
			set { SetField(ref Private_Car.loc, value, nameof(EngineLocation)); }
		}

		public string PoweredWheels
		{
			get => Private_Car.drive;
			set { SetField(ref Private_Car.drive, value, nameof(PoweredWheels)); }
		}

		public ushort MaxPower
		{
			get => Private_Car.hp;
			set { SetField(ref Private_Car.hp, value, nameof(MaxPower)); }
		}
	
		public ushort ElectricMaxPower
		{
			get => Private_Car.ehp;
			set { SetField(ref Private_Car.ehp, value, nameof(ElectricMaxPower)); }
		}
	
		public ushort Displacement
		{
			get => Private_Car.cc;
			set { SetField(ref Private_Car.cc, value, nameof(Displacement)); }
		}
	
		public ushort MaxTorque
		{
			get => Private_Car.nm;
			set { SetField(ref Private_Car.nm, value, nameof(MaxTorque)); }
		}
	}
}
