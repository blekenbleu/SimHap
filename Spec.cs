using GameReaderCommon;
using Newtonsoft.Json;
using SimHub;
using System.Collections.Generic;
using System.ComponentModel;

namespace blekenbleu.Haptic
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

		protected void SetSpec<T>(ref T field, T value, string propertyname)
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
		public ushort idlerpm;							// CarSpec element
		public string order;							// firing order added 19 Jun 2024
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
		internal Dictionary<string, List<CarSpec>> inDict;
		internal ListDictionary() { inDict = new(); }
		BlekHapt H;

		// create inDict, return List<CarSpec>
		internal string SetGame(BlekHapt h, Dictionary<string, List<CarSpec>> json)
		{
			H = h;
			inDict = json;
			int ct = inDict.ContainsKey(H.GameDBText) ? inDict[H.GameDBText].Count : 0;

			return $"{inDict.Count} games, {ct} {H.GameDBText} cars in ";
		}

		internal void AddCar(CarSpec car)			// ListDictionary: S.LD.AddCar; update Save
		{
			string g = H.GameDBText;

			if (inDict.ContainsKey(g))
			{
				int idx = inDict[g].FindIndex(x => x.id == car.id);

				if (0 <= idx)
					inDict[g][idx] = car;				// ListDictionary:  replace car in dictionary
				else inDict[g].Add(car);				// ListDictionary:  add car to current dictionary
			}
			else inDict.Add(g, new() { car } );			// ListDictionary:  add new dictionary with car
			H.Save = true;
			return;
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
		public ListDictionary LD { get; set; }  // needs to be public for JsonConvert
		private static ushort redlineFromGame;
		private static ushort maxRPMFromGame;
		private static ushort ushortIdleRPM;
		private readonly List<CarSpec> Lcache;
		private CarSpec Private_Car, DfltCar;
		internal CarSpec Car { get => Private_Car; }
		internal List<CarSpec> Cars { get => Lcache; }
		internal string Src;
		BlekHapt H;

		public Spec()
		{
			Private_Car = new() { };				// required 24 May 2024
			Lcache = new() { };						// required 24 May 2024
			LD = new() { };
		}

		private CarSpec NewCar(CarSpec c)
		{
			return new CarSpec()
			{
				game = c.game,
				name = c.name,
				id = c.id,
				config = c.config,
				cyl = c.cyl,
				loc = c.loc,
				drive = c.drive,
				hp = c.hp,
				ehp = c.ehp,
				cc = c.cc,
				nm = c.nm,
				redline = c.redline,
				maxrpm = c.maxrpm,
				idlerpm = c.idlerpm,						  // CarSpec element
				order = c.order,
				category = c.category,
				notes = c.notes,
				defaults = c.defaults,
				properties = c.properties
			};
		}

		internal void CarId(BlekHapt h)						// store CarID until Set()
		{
			H = h;
			Private_Car.id = H.N.CarId;
		}

		internal void CarId(string along)						// store CarID until Set()
		{
			Private_Car.id = along;
		}

		internal void CarModel(string along)					// store CarName until Set()
		{
			Private_Car.name = along;
		}

		internal void Idle(ushort rpm)
		{
			int i = Lcache.FindIndex(x => x.id == Car.id);
			IdleRPM = Lcache[i].idlerpm = rpm;
			H.Changed = false;
		}

		internal void Set()				// S.Set
		{
			int i = Lcache.FindIndex(x => x.id == Car.id);
			CarSpec c = Lcache[i];
			Private_Car = new();
			Game = c.game;
	  		CarName = c.name;
	  		Id = c.id;
	  		EngineConfiguration = c.config;
	  		EngineCylinders = c.cyl;
	  		EngineLocation = c.loc;
	  		PoweredWheels = c.drive;
	  		MaxPower = c.hp;
	  		ElectricMaxPower = c.ehp;
	  		Displacement = c.cc;
	  		MaxTorque = c.nm;
			Redline = c.redline;
			MaxRPM = c.maxrpm;
			IdleRPM = c.idlerpm;
			FiringOrder = c.order;
	  		Category = c.category;
			Notes = c.notes;
			Default = c.defaults;
			Property = c.properties;
			H.Set = true;			// subsequent value changes set Changed = true
			H.Changed = false;
		}

		// apply game defaults and add to LCars
		internal int Cache(CarSpec c)
		{
			int Idx;

			// wtf FindIndex returns 0 for 0 == Lcache.count ??!!
			if (0 == Lcache.Count || 0 > (Idx  = Lcache.FindIndex(x => x.id == c.id)))
			{
				c.game = H.GameDBText;
				c.redline =	0 < c.redline ? c.redline : redlineFromGame;
				c.maxrpm  =	0 < c.maxrpm ? c.maxrpm : maxRPMFromGame;
				c.idlerpm =	0 < c.idlerpm ? c.idlerpm : ushortIdleRPM;
				c.nm =	  0 < c.nm ? c.nm : MaxPower;
#if !slim
				c.cc =	  0 < c.cc ? c.cc : (ushort)3333;
				c.hp =	  0 < c.hp ? c.hp : (ushort)333;
#endif
				c.category = string.IsNullOrEmpty(c.category) ? "street" : c.category;
				Lcache.Add(NewCar(c));
				Idx = Lcache.FindIndex(x => x.id == c.id);
			}
			Logging.Current.Info($"H.S.Cache({c.name}): Car {Idx} of {Lcache.Count}");
			return Idx;
		}

		internal int SelectCar(BlekHapt h, List<CarSpec> Atlas, ushort r, ushort m, ushort I)
		{
			H = h;
			Game = H.GameDBText;
			redlineFromGame = r;  maxRPMFromGame = m;  ushortIdleRPM = I;
			int i = Lcache.FindIndex(x => x.id == Car.id);

			if (0 <= i)
			{
				Src = "Cache match";
				return i;
			}

			if (LD.inDict.ContainsKey(Game) && 0 <= (i = LD.inDict[Game].FindIndex(x => x.id == Car.id)))
			{
				Src = "JSON match";
				Cache(LD.inDict[Game][i]);
				return 0;
			}

			if (1 > Atlas.Count)
				return -1;

			if (0 <= (i = Atlas.FindIndex(x => x.id == Car.id)))
			{
				Src = "Atlas match";
				Default = "Atlas";
				Cache(Atlas[i]);						// SelectCar()
				return i;
			}

			if (0 <= (i = Atlas.FindIndex(x => x.name == Car.name)))
			{
				Src = "Atlas CarName match";					// RRRE
				Default = "Atlas";
				DfltCar = Atlas[i];
				DfltCar.id = Car.id;
				DfltCar.name = Car.name;
				Cache(DfltCar);									// SelectCar()
			}
			return i;
		}

		internal bool SaveCar(string pname)				// S.SaveCar():  add or update Car in Cars
		{
			if ("Defaults" == Private_Car.defaults && null != DfltCar && DfltCar.name == Private_Car.name
				&& DfltCar.category == Private_Car.category && DfltCar.config == Private_Car.config
				&& DfltCar.cyl == Private_Car.cyl && DfltCar.loc == Private_Car.loc
				&& DfltCar.drive == Private_Car.drive && DfltCar.cc == Private_Car.cc
				&& DfltCar.hp == Private_Car.hp && DfltCar.ehp == Private_Car.ehp && DfltCar.nm == Private_Car.nm)
					return false;	// do not save Car with all default values

			int Index = Cars.FindIndex(x => x.id == Car.id);
			if (0 > Index)
			{
				Lcache.Add(Private_Car);		// generic List<CarSpec>.Add()
				Logging.Current.Info($"\t{pname}.S.SaveCar():  {Car.id} makes {Cars.Count} {Car.game} cars");
				LD.AddCar(Car);
				return false;
			}

			Logging.Current.Info($"\t{pname}.S.SaveCar(): {Car.id} Index = {Index}/{Cars.Count}");
			bool tf = false;
			if (Lcache[Index].game != Private_Car.game)
			{
				tf = true;
				Lcache[Index].game = Private_Car.game;
			}
			if (Lcache[Index].name != Private_Car.name)
			{
				tf = true;
				Lcache[Index].name = Private_Car.name;
			}
			if (Lcache[Index].config != Private_Car.config)
			{
				tf = true;
				Lcache[Index].config = Private_Car.config;
			}
			if (Lcache[Index].cyl != Private_Car.cyl)
			{
				tf = true;
				Lcache[Index].cyl = Private_Car.cyl;
			}
			if (Lcache[Index].loc != Private_Car.loc)
			{
				tf = true;
				Lcache[Index].loc = Private_Car.loc;
			}
			if (Lcache[Index].drive != Private_Car.drive)
			{
				tf = true;
				Lcache[Index].drive = Private_Car.drive;
			}
			if (Lcache[Index].hp != Private_Car.hp)
			{
				tf = true;
				Lcache[Index].hp = Private_Car.hp;
			}
			if (Lcache[Index].ehp != Private_Car.ehp)
			{
				tf = true;
				Lcache[Index].ehp = Private_Car.ehp;
			}
			if (Lcache[Index].cc != Private_Car.cc)
			{
				tf = true;
				Lcache[Index].cc = Private_Car.cc;
			}
			if (Lcache[Index].nm != Private_Car.nm)
			{
				tf = true;
				Lcache[Index].nm = Private_Car.nm;
			}
			if (Lcache[Index].redline != Private_Car.redline)
			{
				tf = true;
				Lcache[Index].redline = Private_Car.redline;
			}
			if (Lcache[Index].maxrpm != Private_Car.maxrpm)
			{
				tf = true;
				Lcache[Index].maxrpm = Private_Car.maxrpm;
			}
			if (Lcache[Index].idlerpm != Private_Car.idlerpm)		// SaveCar(): changing value in Cars?
			{
				tf = true;
				Lcache[Index].idlerpm = Private_Car.idlerpm;			// SaveCar(): Yes, value has changed
			}
			if (Lcache[Index].order != Private_Car.order)
			{
				tf = true;
				Lcache[Index].order = Private_Car.order;
			}
			if (Lcache[Index].defaults != Private_Car.defaults)
			{
				tf = true;
				Lcache[Index].defaults = Private_Car.defaults;
			}
			if (Lcache[Index].category != Private_Car.category)
			{
				tf = true;
				Lcache[Index].category = Private_Car.category;
			}
			if (Lcache[Index].notes != Private_Car.notes)
			{
				tf = true;
				Lcache[Index].notes = Private_Car.notes;
			}
			if (tf)
				LD.AddCar(Car);
			return false;
		}	// S.SaveCar()

		internal void Defaults(StatusDataBase db)
		{
			string StatusText = "Defaults:  ";

			if (null == DfltCar)
			{
				DfltCar = new()
#if slim
				{
					config = "?",
					loc = "?",
					drive = "?",
					ehp = 0,
					cyl = 0,
					cc = 0,
					hp = 0,
					nm = 0
				};
				StatusText = "Specs unknown";
#else
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

				switch (BlekHapt.CurrentGame)
				{
					case GameId.AC:
					case GameId.ACC:
					case GameId.AMS2:
					case GameId.BeamNG:
					case GameId.Forza:
					case GameId.IRacing:
					case GameId.RRRE:
					case GameId.AMS1:
					case GameId.PC2:
					case GameId.GTR2:
					case GameId.RBR:
					case GameId.RF2:
						DfltCar.cc = 3000;
						DfltCar.drive = "A";
						StatusText += "unavailable: using generic car";
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
					case GameId.GranTurismo7:
					case GameId.GranTurismoSport:
						StatusText += "unavailable: assume 500HP 4 Liter V6";
						DfltCar.cc = 4000;
						DfltCar.hp = 500;
						DfltCar.nm = 400;
						break;
					case GameId.D4:
					case GameId.DR2:
					case GameId.WRC23:
						StatusText += "unavailable: using generic Rally2";
						DfltCar.config = "I";
						DfltCar.loc = "F";
						DfltCar.drive = "A";
						DfltCar.cyl = 4;
						DfltCar.nm = 400;
						break;
					default:
						StatusText += $"specs unavailable for {BlekHapt.CurrentGame}";
						break;
				}
#endif
				DfltCar.notes = StatusText;
			}
			else StatusText = DfltCar.notes;
			DfltCar.name = db.CarModel;
			DfltCar.category = string.IsNullOrEmpty(db.CarClass) ? "street" : db.CarClass;
			DfltCar.id = Car.id;						// CarId()
			Cache(DfltCar);
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
			set { SetSpec(ref Private_Car.notes, value, nameof(Notes));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public string Id
		{
			get => Private_Car.id;
			set { SetField(ref Private_Car.id, value, nameof(Id));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public string Default
		{
			get => Private_Car.defaults;
			set { SetSpec(ref Private_Car.defaults, value, nameof(Default));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public string Property
		{
			get => Private_Car.properties;
			set { SetSpec(ref Private_Car.properties, value, nameof(Property));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public string Category
		{
			get => Private_Car.category;
			set { SetSpec(ref Private_Car.category, value, nameof(Category));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public ushort Redline
		{
			get => Private_Car.redline;
			set { SetSpec(ref Private_Car.redline, value, nameof(Redline));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort MaxRPM
		{
			get => Private_Car.maxrpm;
			set { SetSpec(ref Private_Car.maxrpm, value, nameof(MaxRPM));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public ushort IdleRPM								// public for Private_Car.idlerpm
		{
			get => Private_Car.idlerpm;						// IdleRPM
			set { SetSpec(ref Private_Car.idlerpm, value, nameof(IdleRPM));
					H.Changed = H.Set;				// SetSpec()
			}	// IdleRPM
		}

		public string FiringOrder								// public for Private_Car.order
		{
			get => Private_Car.order;						// FiringOrder
			set { SetSpec(ref Private_Car.order, value, nameof(FiringOrder));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public string EngineConfiguration
		{
			get => Private_Car.config;
			set { SetSpec(ref Private_Car.config, value, nameof(EngineConfiguration));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort EngineCylinders
		{
			get => Private_Car.cyl;
			set { SetSpec(ref Private_Car.cyl, value, nameof(EngineCylinders));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public string EngineLocation
		{
			get => Private_Car.loc;
			set { SetSpec(ref Private_Car.loc, value, nameof(EngineLocation));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public string PoweredWheels
		{
			get => Private_Car.drive;
			set { SetSpec(ref Private_Car.drive, value, nameof(PoweredWheels));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public ushort MaxPower
		{
			get => Private_Car.hp;
			set { SetSpec(ref Private_Car.hp, value, nameof(MaxPower));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort ElectricMaxPower
		{
			get => Private_Car.ehp;
			set { SetSpec(ref Private_Car.ehp, value, nameof(ElectricMaxPower));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort Displacement
		{
			get => Private_Car.cc;
			set { SetSpec(ref Private_Car.cc, value, nameof(Displacement));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort MaxTorque
		{
			get => Private_Car.nm;
			set { SetSpec(ref Private_Car.nm, value, nameof(MaxTorque));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	}	// class Spec
}
