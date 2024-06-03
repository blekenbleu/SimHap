// Decompiled with JetBrains decompiler
// Type: SimHaptics.Spec
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;
using Newtonsoft.Json;
using SimHub;
using System;
using System.Collections;
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

		protected void SetCarSpec<T>(ref T field, T value, string propertyname)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return;
			field = value;
			Haptics.Changed = Haptics.Set;				// SetCarSpec()
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
		public ushort idlerpm;							  // CarSpec element
		public string category;
		public string notes;
		public string defaults;
		public string properties;
	}   // class CarSpec

	// format for downloading from website; must be public
	public class Download
	{
		public List<CarSpec> data;
	}

	public class ListDictionary : NotifyPropertyChanged
	{
		private Dictionary<string, List<CarSpec>> inDict;
		internal ListDictionary() { inDict = new(); }

		// create inDict, return List<CarSpec>
		internal void Set(Dictionary<string, List<CarSpec>> json)
		{
			inDict = json;
		}

		internal void Add(CarSpec car)			// ListDictionary: S.LD.Add; update Save
		{
			if (null == car || null == car.id)
				return;

			string g = Haptics.GameDBText;

            if (inDict.ContainsKey(g))
			{
				int idx = inDict[g].FindIndex(x => x.id == car.id);

                if (0 <= idx)
					inDict[g][idx] = car;				// ListDictionary:  replace car in dictionary
				else inDict[g].Add(car);				// ListDictionary:  add car to current dictionary
			}
			else inDict.Add(g, new() { car } );			// ListDictionary:  add new dictionary with car
			Haptics.Save = true;
			return;
		}

		internal CarSpec FindCar(string cn)
		{
			string g = Haptics.GameDBText;
			int i;

			if (inDict.ContainsKey(g) && 0 < (i = inDict[g].FindIndex(x => x.id == cn)))
				return inDict[g][i];
			else return null;
		}

		internal int CarCount(string g) { return inDict.ContainsKey(g) ? inDict[g].Count : 0; }

		internal ushort Count { get { return (ushort)inDict.Count; } }

		internal string Jstring()	// ignore null (string) values; indent JSON
		{
			return JsonConvert.SerializeObject(inDict, new JsonSerializerSettings
			{ Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });
		}
	}   // class ListDictionary

	public class Spec : NotifyPropertyChanged
	{
		private CarSpec Private_Car;
		internal CarSpec Car { get => Private_Car; }
		private List<CarSpec> Lcars;
		internal List<CarSpec> Cars { get => Lcars; }
		public ListDictionary LD { get; set; }  // needs to be public for JsonConvert
		internal string Src;

		public Spec()
		{
			Private_Car = new() { };				// required 24 May 2024
			Lcars = new() { };					  // required 24 May 2024
			LD = new() { };
		}

		private static ushort redlineFromGame;
		private static ushort maxRPMFromGame;
		private static ushort ushortIdleRPM;

		internal bool Set(CarSpec c)				// S.Set
		{
			if (null == c)
			{
				Logging.Current.Info("Haptics.Spec.Set(Spec s):  null Car");
				return false;
			}
			Private_Car = new();
			Id = c.id;				// S.Set()
			Game = Haptics.GameDBText;
			Redline  =	 		0 < c.redline ? c.redline : redlineFromGame;
			MaxRPM   =	 		0 < c.maxrpm ? c.maxrpm : maxRPMFromGame;
			IdleRPM =			0 < c.idlerpm ? c.idlerpm : ushortIdleRPM;
			Displacement = 		0 < c.cc ? c.cc : (ushort)3333;
			MaxTorque = 		0 < c.nm ? c.nm : MaxPower;
			MaxPower =	 		0 < c.hp ? c.hp : (ushort)333;
			ElectricMaxPower = 	c.ehp;
			Category = 			string.IsNullOrEmpty(c.category) ? "street" : c.category;
			CarName = 			c.name;
			EngineLocation = 	c.loc;
			PoweredWheels = 	c.drive;
			EngineConfiguration = c.config;
			EngineCylinders = 	c.cyl;
			Property =			c.properties;
			Notes =				c.notes;
			Haptics.Set = true;			// subsequent value changes set Changed = true
			Haptics.Changed = false;
			return true;
		}

		internal int SelectCar(string along, ushort r, ushort m, ushort I)
		{
			redlineFromGame = r;  maxRPMFromGame = m;  ushortIdleRPM = I;
			int i = Lcars.FindIndex(x => x.id == along);
			if (0 <= i)
			{
				Src = "Cache match";	
				Set(Lcars[i]);						// SelectCar()
				return i;
			}

			CarSpec car = LD.FindCar(along);
            if (null != car)
			{
				Src = "JSON match";
				Set(car);
				return 0;
			}

			if (0 <= (i = 0 < Haptics.AtlasCt ? Haptics.Atlas.FindIndex(x => x.id == along) : -1))
			{
				Src = "Atlas match";
				Default = "Atlas";
				Set(Haptics.Atlas[i]);						// SelectCar()
			}
			return i;
		}

		// create inDict or Haptics.Atlas
		public int Extract(Dictionary<string, List<CarSpec>> json, string game)
		{
			return Haptics.AtlasCt = (null != json && json.ContainsKey(game)) ? (Haptics.Atlas = json[game]).Count : 0;
		}

		internal bool Add(string cId)				// S.Add():  add or update Car in Cars
		{
			if ("Defaults" == Private_Car.defaults && null != DfltCar && DfltCar.name == Private_Car.name
				&& DfltCar.category == Private_Car.category && DfltCar.config == Private_Car.config
				&& DfltCar.cyl == Private_Car.cyl && DfltCar.loc == Private_Car.loc
				&& DfltCar.drive == Private_Car.drive && DfltCar.cc == Private_Car.cc
				&& DfltCar.hp == Private_Car.hp && DfltCar.ehp == Private_Car.ehp && DfltCar.nm == Private_Car.nm)
					return false;	// do not save Car with all default values

			int Index = Cars.FindIndex(x => x.id == cId);
			if (0 > Index)
			{
				Lcars.Add(Private_Car);		// generic List<CarSpec>.Add()
				Logging.Current.Info($"\tHaptics.S.Add({cId}) : {Cars.Count} {Car.game} cars");
				return true;
			}

			Logging.Current.Info($"\tHaptics.S.Add({cId}) : {Car.id} Index = {Index}/{Cars.Count}");
			bool tf = false;
			if (Lcars[Index].id != Private_Car.id)
			{
				tf = true;
				Lcars[Index].id = Private_Car.id;
			}
			if (Lcars[Index].game != Private_Car.game)
			{
				tf = true;
				Lcars[Index].game = Private_Car.game;
			}
			if (Lcars[Index].name != Private_Car.name)
			{
				tf = true;
				Lcars[Index].name = Private_Car.name;
			}
			if (Lcars[Index].config != Private_Car.config)
			{
				tf = true;
				Lcars[Index].config = Private_Car.config;
			}
			if (Lcars[Index].cyl != Private_Car.cyl)
			{
				tf = true;
				Lcars[Index].cyl = Private_Car.cyl;
			}
			if (Lcars[Index].loc != Private_Car.loc)
			{
				tf = true;
				Lcars[Index].loc = Private_Car.loc;
			}
			if (Lcars[Index].drive != Private_Car.drive)
			{
				tf = true;
				Lcars[Index].drive = Private_Car.drive;
			}
			if (Lcars[Index].hp != Private_Car.hp)
			{
				tf = true;
				Lcars[Index].hp = Private_Car.hp;
			}
			if (Lcars[Index].ehp != Private_Car.ehp)
			{
				tf = true;
				Lcars[Index].ehp = Private_Car.ehp;
			}
			if (Lcars[Index].cc != Private_Car.cc)
			{
				tf = true;
				Lcars[Index].cc = Private_Car.cc;
			}
			if (Lcars[Index].nm != Private_Car.nm)
			{
				tf = true;
				Lcars[Index].nm = Private_Car.nm;
			}
			if (Lcars[Index].redline != Private_Car.redline)
			{
				tf = true;
				Lcars[Index].redline = Private_Car.redline;
			}
			if (Lcars[Index].maxrpm != Private_Car.maxrpm)
			{
				tf = true;
				Lcars[Index].maxrpm = Private_Car.maxrpm;
			}
			if (Lcars[Index].idlerpm != Private_Car.idlerpm)			// Add(): changing value in Cars?
			{
				tf = true;
				Lcars[Index].idlerpm = Private_Car.idlerpm;			// Add(): Yes, value has changed
			}
			if (Lcars[Index].defaults != Private_Car.defaults)
			{
				tf = true;
				Lcars[Index].defaults = Private_Car.defaults;
			}
			if (Lcars[Index].category != Private_Car.category)
			{
				tf = true;
				Lcars[Index].category = Private_Car.category;
			}
			if (Lcars[Index].notes != Private_Car.notes)
			{
				tf = true;
				Lcars[Index].notes = Private_Car.notes;
			}
			return tf;
		}	// S.Add()

		private CarSpec DfltCar;
		internal void Defaults(StatusDataBase db)
		{
			if (null == Haptics.GameDBText)
			{
				Src = $"Haptics.Defaults({db.CarId}):  null GameDBText";
				return;
			}

			string StatusText = "Haptics.Defaults:  ";

			if (null == DfltCar)
			{
				DfltCar = new()
				{
					config = "V",
					cyl = 6,
					loc = "RM",
					drive = "R",
					cc = 1600,
					hp = 300,
					ehp = 0,
					nm = 250
				};

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
						DfltCar.cc = 3000;
						DfltCar.drive = "A";
						StatusText += "unavailable: using generic car";
						break;
					case GameId.D4:
					case GameId.DR2:
					case GameId.WRC23:
						StatusText += "unavailable: using generic Rally2";
						DfltCar.config = "I";
						DfltCar.cyl = 4;
						DfltCar.loc = "F";
						DfltCar.drive = "A";
						DfltCar.nm = 400;
						break;
					case GameId.F12022:
					case GameId.F12023:
						StatusText += "unavailable: using generic F1";
						DfltCar.hp = 1000;
						DfltCar.nm = 650;
						break;
					case GameId.KK:
						StatusText += "unavailable: using generic Kart";
						DfltCar.config = "I";
						DfltCar.cyl = 1;
						DfltCar.cc = 130;
						DfltCar.hp = 34;
						DfltCar.nm = 24;
						break;
					case GameId.GPBikes:
						StatusText += "unavailable: using generic Superbike";
						DfltCar.config = "I";
						DfltCar.cyl = 4;
						DfltCar.loc = "M";
						DfltCar.cc = 998;
						DfltCar.hp = 200;
						DfltCar.nm = 100;
						break;
					case GameId.MXBikes:
						StatusText += "unavailable: using generic MX Bike"; EngineConfiguration = "I";
						DfltCar.cyl = 1;
						DfltCar.loc = "M";
						DfltCar.cc = 450;
						DfltCar.hp = 50;
						DfltCar.nm = 45;
						break;
					case GameId.GranTurismo7:
					case GameId.GranTurismoSport:
						StatusText += "unavailable: assume 500HP 4 Liter V6";
						DfltCar.cc = 4000;
						DfltCar.hp = 500;
						DfltCar.nm = 400;
						break;
					default:
						StatusText += $"specs unavailable for {Haptics.CurrentGame}";
						break;
				}
				DfltCar.notes = StatusText;
			}
			else StatusText = DfltCar.notes;
			DfltCar.name = db.CarModel;
			DfltCar.category = string.IsNullOrEmpty(db.CarClass) ? "street" : db.CarClass;
			DfltCar.id = ( GameId.RRRE == Haptics.CurrentGame			// Defaults()
						|| GameId.D4   == Haptics.CurrentGame
						|| GameId.DR2  == Haptics.CurrentGame )
						 ? db.CarModel : db.CarId;
			Set(DfltCar);
			Src = StatusText;
		}												// Defaults()

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
			set { SetCarSpec(ref Private_Car.notes, value, nameof(Notes)); }
		}

		public string Id
		{
			get => Private_Car.id;
			set { SetField(ref Private_Car.id, value, nameof(Id)); }
		}

		public string Default
		{
			get => Private_Car.defaults;
			set { SetCarSpec(ref Private_Car.defaults, value, nameof(Default)); }
		}

		public string Property
		{
			get => Private_Car.properties;
			set { SetCarSpec(ref Private_Car.properties, value, nameof(Property)); }
		}

		public string Category
		{
			get => Private_Car.category;
			set { SetCarSpec(ref Private_Car.category, value, nameof(Category)); }
		}

		public ushort Redline
		{
			get => Private_Car.redline;
			set { SetCarSpec(ref Private_Car.redline, value, nameof(Redline)); }
		}
	
		public ushort MaxRPM
		{
			get => Private_Car.maxrpm;
			set { SetCarSpec(ref Private_Car.maxrpm, value, nameof(MaxRPM)); }
		}
		public ushort IdleRPM								// public for Private_Car.idlerpm
		{
			get => Private_Car.idlerpm;						// IdleRPM
			set { SetCarSpec(ref Private_Car.idlerpm, value, nameof(IdleRPM)); }	// IdleRPM
		}

		public string EngineConfiguration
		{
			get => Private_Car.config;
			set { SetCarSpec(ref Private_Car.config, value, nameof(EngineConfiguration)); }
		}
	
		public ushort EngineCylinders
		{
			get => Private_Car.cyl;
			set { SetCarSpec(ref Private_Car.cyl, value, nameof(EngineCylinders)); }
		}

		public string EngineLocation
		{
			get => Private_Car.loc;
			set { SetCarSpec(ref Private_Car.loc, value, nameof(EngineLocation)); }
		}

		public string PoweredWheels
		{
			get => Private_Car.drive;
			set { SetCarSpec(ref Private_Car.drive, value, nameof(PoweredWheels)); }
		}

		public ushort MaxPower
		{
			get => Private_Car.hp;
			set { SetCarSpec(ref Private_Car.hp, value, nameof(MaxPower)); }
		}
	
		public ushort ElectricMaxPower
		{
			get => Private_Car.ehp;
			set { SetCarSpec(ref Private_Car.ehp, value, nameof(ElectricMaxPower)); }
		}
	
		public ushort Displacement
		{
			get => Private_Car.cc;
			set { SetCarSpec(ref Private_Car.cc, value, nameof(Displacement)); }
		}
	
		public ushort MaxTorque
		{
			get => Private_Car.nm;
			set { SetCarSpec(ref Private_Car.nm, value, nameof(MaxTorque)); }
		}
	}	// class Spec
}
