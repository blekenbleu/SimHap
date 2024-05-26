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
		private readonly Spec S;
		internal ListDictionary(Spec s)
		{
			S = s;
//			inDict = new();
		}

		internal bool Add()				// S.LD.Add
		{
			// handle Loaded and Save
			S.Add();				// S.Lcars.Add for S.LD.Add()
			List<CarSpec> s = S.Lcars;
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
			return inDict.ContainsKey(game) && 0 < (S.Lcars = inDict[game]).Count;
		}

		internal string Jstring()	// ignore null (string) values; indent JSON
		{
			return JsonConvert.SerializeObject(S.LD.inDict, new JsonSerializerSettings
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

		internal void Add()				// S.Add():  add or update Private_Car in Lcars
		{
			Haptics.Loaded = false;		// done with this car;  update Save
			if ((null == Private_Car.id) || (null == Private_Car.game) || (null == Private_Car.name))
				return;

			string cid = Private_Car.id;

			int Index = Lcars.FindIndex(x => x.id == cid);
			if (0 > Index)
			{
				Lcars.Add(Private_Car);		// generic List<CarSpec>.Add()
				Haptics.Save = true;
				return;
			}
		
			if (Lcars[Index].id != Private_Car.id)
			{
				Haptics.Save = true;
				Lcars[Index].id = Private_Car.id;
			}
			if (Lcars[Index].game != Private_Car.game)
			{
				Haptics.Save = true;
				Lcars[Index].game = Private_Car.game;
			}
			if (Lcars[Index].name != Private_Car.name)
			{
				Haptics.Save = true;
				Lcars[Index].name = Private_Car.name;
			}
			if (Lcars[Index].config != Private_Car.config)
			{
				Haptics.Save = true;
				Lcars[Index].config = Private_Car.config;
			}
			if (Lcars[Index].cyl != Private_Car.cyl)
			{
				Haptics.Save = true;
				Lcars[Index].cyl = Private_Car.cyl;
			}
			if (Lcars[Index].loc != Private_Car.loc)
			{
				Haptics.Save = true;
				Lcars[Index].loc = Private_Car.loc;
			}
			if (Lcars[Index].drive != Private_Car.drive)
			{
				Haptics.Save = true;
				Lcars[Index].drive = Private_Car.drive;
			}
			if (Lcars[Index].hp != Private_Car.hp)
			{
				Haptics.Save = true;
				Lcars[Index].hp = Private_Car.hp;
			}
			if (Lcars[Index].ehp != Private_Car.ehp)
			{
				Haptics.Save = true;
				Lcars[Index].ehp = Private_Car.ehp;
			}
			if (Lcars[Index].cc != Private_Car.cc)
			{
				Haptics.Save = true;
				Lcars[Index].cc = Private_Car.cc;
			}
			if (Lcars[Index].nm != Private_Car.nm)
			{
				Haptics.Save = true;
				Lcars[Index].nm = Private_Car.nm;
			}
			if (Lcars[Index].redline != Private_Car.redline)
			{
				Haptics.Save = true;
				Lcars[Index].redline = Private_Car.redline;
			}
			if (Lcars[Index].maxrpm != Private_Car.maxrpm)
			{
				Haptics.Save = true;
				Lcars[Index].maxrpm = Private_Car.maxrpm;
			}
			if (Lcars[Index].idlerpm != Private_Car.idlerpm)
			{
				Haptics.Save = true;
				Lcars[Index].idlerpm = Private_Car.idlerpm;
			}
			if (Lcars[Index].category != Private_Car.category)
			{
				Haptics.Save = true;
				Lcars[Index].category = Private_Car.category;
			}
			if (Lcars[Index].notes != Private_Car.notes)
			{
				Haptics.Save = true;
				Lcars[Index].notes = Private_Car.notes;
			}
		}

		internal bool Set(Download dl, ushort gameRedline, ushort gameMaxRPM) // called by FetchCarData()
		{
			if (null == dl || null == dl.data || 0 == dl.data.Count)
				return false;

			CarSpec data = dl.data[0];

			if (null == data.name || null == data.id)
				return false;

			Game = Haptics.GameDBText;
			Id = Haptics.CurrentGame == GameId.Forza ? "Car_" + data.id : data.id;	// Set()
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
			Private_Car = Lcars[i];
		}

		internal string Defaults(StatusDataBase db)	
		{
			string StatusText;

			if (null == Haptics.GameDBText)
				return "Haptics.Spec.Defaults():  null GameDBText";

			Logging.Current.Info($"Haptics.Defaults({db.CarId}): "
								+(Haptics.Loaded ? " Loaded " : "") + (Haptics.Waiting ? " Waiting" : ""));
			Game = Haptics.GameDBText;
			CarName = db.CarModel;				// Defaults()
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

			switch (Haptics.CurrentGame)
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
					StatusText = "Unavailable: using generic car";
					break;
				case GameId.D4:
				case GameId.DR2:
				case GameId.WRC23:
					StatusText = "Unavailable: using generic Rally2";
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
					StatusText = "Unavailable: using generic F1";
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
					StatusText = "Unavailable: using generic Kart";
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
					StatusText = "Unavailable: using generic Superbike";
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
					StatusText = "Unavailable: using generic MX Bike";
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
					StatusText = "Unavailable: assume 500HP 4 Liter V6";
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
					StatusText = $"Specs unavailable for {Haptics.CurrentGame}";
					break;
			}
			if (0 == Redline)
				Redline = 6000;
			if (0 == MaxRPM)
				MaxRPM = 6500;
			if (string.IsNullOrEmpty(Category))
				Category = "street";
			Id =									// Defaults()
				(GameId.RRRE == Haptics.CurrentGame || GameId.D4 == Haptics.CurrentGame || GameId.DR2 == Haptics.CurrentGame) ?
					db.CarModel : db.CarId;
			if (0 < StatusText.Length)
				Logging.Current.Info($"Haptics.Defaults({Haptics.CurrentGame}, {db.CarModel}): "
								   + (Haptics.Loaded ? " Loaded" : "") + (Haptics.Waiting ? " Waiting" : "") + ":  " + StatusText);
			return StatusText;
		}	// Defaults()

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
