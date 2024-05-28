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
		public string defaults;
		public string properties;
	}	// class CarSpec

	// format for downloading from website; must be public
	public class Download
	{
		public List<CarSpec> data;
	}

	public class ListDictionary : NotifyPropertyChanged
	{
		private Dictionary<string, List<CarSpec>> inDict;
		private readonly Spec Sp;
		internal ListDictionary(Spec s) { Sp = s; inDict = new(); }

		internal bool Add()				// ListDictionary: S.LD.Add; update Save
		{
			Sp.Add(Sp.Car.id);					// ListDictionary:  S.Add() Private_Car to Lcars
			List<CarSpec> s = Sp.Cars;
			if (0 == s.Count)
				return false;

			string k = s[0].game;

			if (inDict.ContainsKey(k))
				for (int i = 0; i < s.Count; i++)
				{
					int idx = inDict[k].FindIndex(x => x.id == s[i].id);
					if (0 <= idx)
						inDict[k][idx] = s[i];
					else {
						inDict[k].Add(s[i]);		// ListDictionary:  add Sp.Cars[i] to current dictionary
						Haptics.Save = true;
					}
				}
			else {
				inDict.Add(k, s);					// ListDictionary:  add Lcars to dictionary
				Haptics.Save = true;
			}
			return true;
		}
 
		// create inDict
		public bool Load(Dictionary<string, List<CarSpec>> json) { return (null != json || 0 < (inDict = new()).Count) && 0 < (inDict = json).Count; }

		public bool Extract(string game) { return 0 < inDict.Count && inDict.ContainsKey(game) && 0 < Sp.Set(inDict[game]); }

		internal ushort Count { get { return (ushort)inDict.Count; } }

		internal string Jstring()	// ignore null (string) values; indent JSON
		{
			return JsonConvert.SerializeObject(Sp.LD.inDict, new JsonSerializerSettings
			{ Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });
		}
	}   // class ListDictionary

	public class Spec : NotifyPropertyChanged
	{
		private CarSpec Private_Car { get; set; }
		internal CarSpec Car { get => Private_Car; }
		private List<CarSpec> Lcars;
		internal List<CarSpec> Cars { get => Lcars; }
		public ListDictionary LD { get; set; }  // needs to be public for JsonConvert

		public Spec()
		{
			Private_Car = new() { };				// required 24 May 2024
			Lcars = new() { };					  // required 24 May 2024
			LD = new(this) { };
		}

		// called after FetchCarData() retrievals
		internal void SetId(string id) { Id = Haptics.CurrentGame == GameId.Forza ? "Car_" + id : id; }

		internal bool Set(CarSpec data, ushort gameRedline, ushort gameMaxRPM) // called >>only<< by Wait()
		{
			// if (Haptics.CurrentGame == GameId.Forza)
			//	 data.id = "Car_" + data.id;		// Spec.Set() for FetchCarData()
			data.id = "";
			data.game = Haptics.GameDBText;
			if (0 == data.redline)
				data.redline = gameRedline;
			if (0 == data.maxrpm)
				data.maxrpm	= gameMaxRPM;
			if (0 ==data.hp)
	  			data.hp = Convert.ToUInt16(333);
			return Set(data);
		}	// Set(CarSpec, ushort, ushort)

		internal bool Set(CarSpec data)				// Spec.Set
		{
			if (null == data)
			{
				Logging.Current.Info("Haptics.Spec.Set(Spec s):  null Car");
				return false;
			}
			Id = data.id;				// Spec.Set()
			Game = data.game;
			Redline  =	 		data.redline;
			MaxRPM   =	 		data.maxrpm;
			MaxPower =	 		data.hp;
			Category = 			data.category;
			CarName = 			data.name;
			EngineLocation = 	data.loc;
			PoweredWheels = 	data.drive;
			EngineConfiguration = data.config;
			EngineCylinders = 	data.cyl;
			ElectricMaxPower = 	data.ehp;
			Displacement = 		data.cc;
			MaxTorque = 		data.nm;
			Property =			data.properties;
			Notes =				data.notes;
			return true;
		}

		internal int SelectCar(string along)
		{
			int i = Lcars.FindIndex(x => x.id == along);
			if (0 <= i)
				Set(Lcars[i]);
			return i;
		}

		internal int Set(List<CarSpec> list)		// Spec.Set
		{
			if (null == list || 1 > list.Count)
			{
				Logging.Current.Info("Haptics.Spec.Set(List<CarSpec>):  empty List");
				return -1;
			}

			return (Lcars = list).Count;
		}

		internal void Add(string cId)				// S.Add():  add or update Car in Cars
		{
			Haptics.Loaded = false;		// done with this car;  update Save
			if ((null == Car.game) || (null == Car.name))
			{
				Logging.Current.Info($"Haptics.Spec.Add({cId}) : missing essential elements");
				return;
			}

			int Index = Cars.FindIndex(x => x.id == cId);
			if (0 > Index)
			{
				Lcars.Add(Private_Car);		// generic List<CarSpec>.Add()
				Logging.Current.Info($"\tHaptics.Spec.Add({cId}) : {Cars.Count} {Car.game} cars");
				Haptics.Save = true;
				Private_Car = new() {};
				return;
			}

			Logging.Current.Info($"\tHaptics.Spec.Add({cId}) : {Car.id} Index = {Index}/{Cars.Count}");
		
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
			if (Lcars[Index].defaults != Private_Car.defaults)
			{
				Haptics.Save = true;
				Lcars[Index].defaults = Private_Car.defaults;
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
		}	// S.Add()

		internal string Defaults(StatusDataBase db)
		{
			if (null == Haptics.GameDBText)
				return $"Haptics.Defaults({db.CarId}):  null GameDBText";

			Haptics.Loaded = true;	// Defaults(): add to JSON
			string StatusText = "Haptics.Defaults:  ";

			Default = "Defaults";
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
					StatusText += "unavailable: using generic car";
					break;
				case GameId.D4:
				case GameId.DR2:
				case GameId.WRC23:
					StatusText += "unavailable: using generic Rally2";
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
					StatusText += "unavailable: using generic F1";
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
					StatusText += "unavailable: using generic Kart";
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
					StatusText += "unavailable: using generic Superbike";
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
					StatusText += "unavailable: using generic MX Bike"; EngineConfiguration = "I";
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
					StatusText += "unavailable: assume 500HP 4 Liter V6";
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
					StatusText += $"specs unavailable for {Haptics.CurrentGame}";
					break;
			}
			if (0 == Redline)
				Redline = 6000;
			if (0 == MaxRPM)
				MaxRPM = 6500;
			if (string.IsNullOrEmpty(Category))
				Category = "street";
			Id =					// Defaults()
				(GameId.RRRE == Haptics.CurrentGame || GameId.D4 == Haptics.CurrentGame || GameId.DR2 == Haptics.CurrentGame) ?
					db.CarModel : db.CarId;
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

		public string Notes
		{
			get => Private_Car.notes;
			set { SetField(ref Private_Car.notes, value, nameof(Notes)); }
		}

		public string Id
		{
			get => Private_Car.id;
			set { SetField(ref Private_Car.id, value, nameof(Id)); }
		}

		public string Default
		{
			get => Private_Car.defaults;
			set { SetField(ref Private_Car.defaults, value, nameof(Default)); }
		}

		public string Property
		{
			get => Private_Car.properties;
			set { SetField(ref Private_Car.properties, value, nameof(Property)); }
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
	}	// class Spec
}
