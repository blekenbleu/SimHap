// Decompiled with JetBrains decompiler
// Type: SimHaptics.Spec
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;
using Newtonsoft.Json;
using SimHub;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace sierses.Sim
{
	/// <summary>
	/// Abstract base class to implement INotifyPropertyChanged interface
	/// https://gist.github.com/itajaja/7439345
	/// </summary>
	public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string Propertyname)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Propertyname));
		}

		protected void SetField<T>(ref T field, T value, string propertyname)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return;
			field = value;
			OnPropertyChanged(propertyname);
		}
	}

	public class CarSpec
	{
		public string game;
		public string name;
		public string id;
		public string config;
		public ushort cyl;
		public string loc;
		public string drive;
		public ushort hp;
		public ushort ehp;
		public ushort cc;
		public ushort nm;
		public ushort redline;
		public ushort maxrpm;
		public ushort idlerpm;
		public string category;
		public string notes;
	}

	// format for downloading from website; must be public
	public class Download
	{
		public List<CarSpec> data;
	}

	public class ListDictionary : NotifyPropertyChanged
	{
		private Dictionary<string, List<CarSpec>> inDict;
		private readonly Spec PS;
		internal ListDictionary(Spec s)
		{
			PS = s;
//			inDict = new();
		}

		internal bool Add()
		{
			PS.Add();
			List<CarSpec> s = PS.Lcars;
			if (0 == s.Count)
				return false;

			int Index;

			string k = s[0].game;

            if (inDict.ContainsKey(k))
				for (int i = 0; i < s.Count; i++)
				{
					Index = inDict[k].FindIndex(x => x.id == s[i].id);
					if (0 <= Index)
						inDict[k][Index] = s[i];
					else inDict[k].Add(s[i]);
				}
			else inDict.Add(k, s);
			return true;
		}
 
		// create inDict
		public bool Load(Dictionary<string, List<CarSpec>> json)
		{
			return (null != json || 0 < (inDict = new()).Count) && 0 < (inDict = json).Count;
		}

		internal ushort Count
		{
			get { return (ushort)inDict.Count; }
		}

		public bool Extract(string game)
		{
			return inDict.ContainsKey(game) && 0 < (PS.Lcars = inDict[game]).Count;
		}

		internal string Jstring()	// ignore null (string) values; indent JSON
		{
			return JsonConvert.SerializeObject(PS.LD.inDict, new JsonSerializerSettings
			{ Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });
		}
	}	// class ListDictionary

	public class Spec : NotifyPropertyChanged
	{
		private CarSpec Private_Car { get; set; }
		internal CarSpec Car { get => Private_Car; }
		internal List<CarSpec> Lcars;
		public ListDictionary LD { get; set; }	// needs to be public for JsonConvert
		//private Haptics SHP;

		public void Init()
		{
			Lcars = new();						// required 24 May 2024
			LD = new(this); 
		}

		public Spec()
		{
			Private_Car = new();				// required 24 May 2024
		}

		public Spec(Spec s)
		{
			Private_Car = s.Private_Car;
		}

		private void Changed(int i, CarSpec s)
		{
			if (Lcars[i].id != s.id)
			{
				Haptics.Save = true;
				Lcars[i].id = s.id;
			}
			if (Lcars[i].game != s.game)
			{
				Haptics.Save = true;
				Lcars[i].game = s.game;
			}
			if (Lcars[i].name != s.name)
			{
				Haptics.Save = true;
				Lcars[i].name = s.name;
			}
			if (Lcars[i].config != s.config)
			{
				Haptics.Save = true;
				Lcars[i].config = s.config;
			}
			if (Lcars[i].cyl != s.cyl)
			{
				Haptics.Save = true;
				Lcars[i].cyl = s.cyl;
			}
			if (Lcars[i].loc != s.loc)
			{
				Haptics.Save = true;
				Lcars[i].loc = s.loc;
			}
			if (Lcars[i].drive != s.drive)
			{
				Haptics.Save = true;
				Lcars[i].drive = s.drive;
			}
			if (Lcars[i].hp != s.hp)
			{
				Haptics.Save = true;
				Lcars[i].hp = s.hp;
			}
			if (Lcars[i].ehp != s.ehp)
			{
				Haptics.Save = true;
				Lcars[i].ehp = s.ehp;
			}
			if (Lcars[i].cc != s.cc)
			{
				Haptics.Save = true;
				Lcars[i].cc = s.cc;
			}
			if (Lcars[i].nm != s.nm)
			{
				Haptics.Save = true;
				Lcars[i].nm = s.nm;
			}
			if (Lcars[i].redline != s.redline)
			{
				Haptics.Save = true;
				Lcars[i].redline = s.redline;
			}
			if (Lcars[i].maxrpm != s.maxrpm)
			{
				Haptics.Save = true;
				Lcars[i].maxrpm = s.maxrpm;
			}
			if (Lcars[i].idlerpm != s.idlerpm)
			{
				Haptics.Save = true;
				Lcars[i].idlerpm = s.idlerpm;
			}
			if (Lcars[i].category != s.category)
			{
				Haptics.Save = true;
				Lcars[i].category = s.category;
			}
			if (Lcars[i].notes != s.notes)
			{
				Haptics.Save = true;
				Lcars[i].notes = s.notes;
			}
		}

		internal void Add()			// add or update Private_Car in Lcars
		{
			if ((null == Private_Car) || (null == Private_Car.id) || (null == Private_Car.game)
			 || (null == Private_Car.name))
				return;

			string cid = Private_Car.id;

			int Index = Lcars.FindIndex(x => x.id == cid);
			if (0 > Index)
			{
				Lcars.Add(Private_Car);
				Haptics.Save = true;
			}
			else Changed(Index, Private_Car);
		}

		// called by Haptics.FetchCarData()
		internal bool Set(Download dl, ushort gameRedline, ushort gameMaxRPM)
		{
			if (null == dl || null == dl.data || 0 == dl.data.Count)
				return false;

			CarSpec data = dl.data[0];

			if (null == data.name || null == data.id)
				return false;

			Game = Haptics.GameDBText;
			Id = Haptics.CurrentGame == GameId.Forza ? "Car_" + data.id : data.id;
			Redline  =	 0 == data.redline ? gameRedline 	: data.redline;
			MaxRPM   =	 0 == data.maxrpm  ? gameMaxRPM		: data.maxrpm;
			MaxPower =	 0 == data.hp 	   ? Convert.ToUInt16(333) : data.hp;
			Category = 			data.category;
			CarName = 				data.name;
			EngineLocation = 	data.loc;
			PoweredWheels = 	data.drive;
			EngineConfiguration = data.config;
			EngineCylinders = 	data.cyl;
			ElectricMaxPower = 	data.ehp;
			Displacement = 		data.cc;
			MaxTorque = 		data.nm;

			return true;
		}	// Set()

		// makes sense only as Spec instance from  JSON
		internal void SelectCar(int i)
		{
			Private_Car = Lcars[i];	// no need to Save
		}

		internal string Defaults(string game, StatusDataBase db, GameId CurrentGame)	
		{
			string StatusText;

			if (null == game)
				return "Haptics.Spec.Defaults():  null game";

			Logging.Current.Info($"Haptics.Defaults({db.CarId}):"
								+ "  {Haptics.FetchStatus} {Haptics.LoadStatus}");
			Game = game;
			CarName = db.CarModel;
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
				case GameId.RRRE:
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
					StatusText = "Not in DB: assume 500HP 4 Liter V6";
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
			if (0 == Redline)
				Redline = 6000;
			if (0 == MaxRPM)
				MaxRPM = 6500;
			if (string.IsNullOrEmpty(Category))
				Category = "street";
			if (GameId.RRRE == CurrentGame || GameId.D4 == CurrentGame || GameId.DR2 == CurrentGame)
				Id = db.CarModel;
			Haptics.FetchStatus = APIStatus.Loaded;	// for adding to JSON
			return StatusText;
		}

		public string Game
		{
			get => Private_Car.game;
			set { SetField(ref Private_Car.game, value, nameof(Game)); }
		}

		public string CarName
		{
			get => Private_Car.name;
			set { SetField(ref Private_Car.name, value, nameof(CarName)); }
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
