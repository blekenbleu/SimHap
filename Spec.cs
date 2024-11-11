using GameReaderCommon;
using System.Collections.Generic;
using System.ComponentModel;
using Atlas;

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

	// format for downloading from website; must be public
	public class Download
	{
		public List<CarSpec> data;
	}

	public partial class Spec : NotifyPropertyChanged
	{
		public ListDictionary LD { get; set; }  // needs to be public for JsonConvert
		private static ushort redlineFromGame, maxRPMFromGame, ushortIdleRPM;
		private readonly List<CarSpec> Lcache;
		private CarSpec Private_Car, DfltCar;
		internal CarSpec Car { get => Private_Car; }
		internal List<CarSpec> Cars { get => Lcache; }
		internal int CacheIndex = -1;
		internal string Src;
		int AtlasId = -1;
		BlekHapt H;

		public Spec()
		{
			Private_Car = new() { };			// required 24 May 2024
			Lcache = new();						// required 24 May 2024
			LD = new() { };
		}

		internal void Init(BlekHapt h)
		{
			H = h;
		}

		private CarSpec NewCar(CarSpec c)
		{
			return new CarSpec()
			{
				id = c.id,
				name = c.name,
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
				idlerpm = c.idlerpm,							// CarSpec element
				order = c.order,
				category = c.category,
				notes = c.notes,
				defaults = c.defaults,
				properties = c.properties
			};
		}

		internal void CarId()									// store CarID until Set()
		{
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
			Private_Car.idlerpm = Lcache[CacheIndex].idlerpm = rpm;
		}

		internal void Set()				// S.Set, called in SimDataMethods.SetCar()
		{
			CacheIndex = Lcache.FindIndex(x => x.id == Car.id);
			CarSpec c = Lcache[CacheIndex];
			Private_Car = new();
	  		CarName = c.name;
	  		Id = c.id;
	  		EngineConfiguration = c.config;
			if (null != c.cyl)
				EngineCylinders = (ushort)c.cyl;
	  		EngineLocation = c.loc;
	  		PoweredWheels = c.drive;
			if (null != c.hp)
	  			MaxPower = (ushort)c.hp;
	  		ElectricMaxPower = c.ehp ?? 0;
			if (null != c.cc)
	  			Displacement = (ushort)c.cc;
			if (null != c.nm)
	  			MaxTorque = (ushort)c.nm;
			if (null != c.redline)
				Redline = (ushort)c.redline;
			if (null != c.maxrpm)
				MaxRPM = (ushort)c.maxrpm;
			if (null != c.idlerpm)
				IdleRPM = (ushort)c.idlerpm;
			FiringOrder = c.order;
	  		Category = c.category;
			Notes = c.notes;
			Default = c.defaults;
			Property = c.properties;
			H.Set = true;			// subsequent value changes set Changed = true
			H.Changed = false;
		}

		// apply default RPMs and add to Lcache
		void Cache(CarSpec c)
		{
			// wtf FindIndex returns 0 for 0 == Lcache.count ??!!
			if (0 == Lcache.Count || 0 > (CacheIndex = Lcache.FindIndex(x => x.id == c.id)))
			{
				if (0 == c.redline)
					c.redline = redlineFromGame;
				if (0 == c.maxrpm)
					c.maxrpm = maxRPMFromGame;
				if (0 == c.idlerpm)
					c.idlerpm = ushortIdleRPM;
				Lcache.Add(NewCar(c));
				CacheIndex = Lcache.FindIndex(x => x.id == c.id);
			}
		}

		internal bool UnCache()
		{
			if (0 > CacheIndex)
				return false;

			Lcache.RemoveAt(CacheIndex);
			CacheIndex = -1;
			int Idx;

			if (0 <= AtlasId && LD.inDict.ContainsKey(H.GameDBText)
			  && 0 <= (Idx = LD.inDict[H.GameDBText].FindIndex(x => x.id == Car.id)))
					LD.inDict[H.GameDBText].RemoveAt(Idx);	// remove JSON entry if also in Atlas
			return true;
		}

		internal int SelectCar(List<CarSpec> Atlas, ushort r, ushort m, ushort I)
		{
			redlineFromGame = r;  maxRPMFromGame = m;  ushortIdleRPM = I;
			CacheIndex = Lcache.FindIndex(x => x.id == Car.id);				// changed Car.id
			AtlasId = (1 > Atlas.Count) ? -1 : Atlas.FindIndex(x => x.id == Car.id);

			if (0 <= CacheIndex)
			{
				Src = "Cache match";
				return CacheIndex;
			}

			int id;

			if (LD.inDict.ContainsKey(H.GameDBText) && 0 <= (id = LD.inDict[H.GameDBText].FindIndex(x => x.id == Car.id)))
			{
				Src = "JSON match";
				Cache(LD.inDict[H.GameDBText][id]);
				return 0;
			}

			if (1 > Atlas.Count)
				return -1;

			if (0 <= (id = AtlasId))
			{
				Src = "Atlas match";
				Default = "Atlas";
				Cache(Atlas[id]);						// SelectCar()
				return id;
			}

			if (0 <= (id = Atlas.FindIndex(x => x.name == Car.name)))
			{
				Src = "Atlas CarName match";					// RRRE
				Default = "Atlas";
				DfltCar = Atlas[id];
				DfltCar.id = Car.id;
				DfltCar.name = Car.name;
				Cache(DfltCar);									// SelectCar()
			}
			return id;
		}

		internal bool SaveCar()					// S.SaveCar():  add or update Car in Cars
		{
			if ("Defaults" == Private_Car.defaults && null != DfltCar && DfltCar.name == Private_Car.name
				&& DfltCar.category == Private_Car.category && DfltCar.config == Private_Car.config
				&& DfltCar.cyl == Private_Car.cyl && DfltCar.loc == Private_Car.loc
				&& DfltCar.drive == Private_Car.drive && DfltCar.cc == Private_Car.cc
				&& DfltCar.hp == Private_Car.hp && DfltCar.ehp == Private_Car.ehp && DfltCar.nm == Private_Car.nm)
					return false;	// do not save Car with all default values

			CacheIndex = Lcache.FindIndex(x => x.id == Car.id);
			if (0 > CacheIndex)
			{
				Lcache.Add(Private_Car);		// generic List<CarSpec>.Add()
				return false;
			}

			bool tf = false;

			if (Lcache[CacheIndex].name != Private_Car.name)
			{
				tf = true;
				Lcache[CacheIndex].name = Private_Car.name;
			}
			if (Lcache[CacheIndex].config != Private_Car.config)
			{
				tf = true;
				Lcache[CacheIndex].config = Private_Car.config;
			}
			if (Lcache[CacheIndex].cyl != Private_Car.cyl)
			{
				tf = true;
				Lcache[CacheIndex].cyl = Private_Car.cyl;
			}
			if (Lcache[CacheIndex].loc != Private_Car.loc)
			{
				tf = true;
				Lcache[CacheIndex].loc = Private_Car.loc;
			}
			if (Lcache[CacheIndex].drive != Private_Car.drive)
			{
				tf = true;
				Lcache[CacheIndex].drive = Private_Car.drive;
			}
			if (Lcache[CacheIndex].hp != Private_Car.hp)
			{
				tf = true;
				Lcache[CacheIndex].hp = Private_Car.hp;
			}
			if (Lcache[CacheIndex].ehp != Private_Car.ehp)
			{
				tf = true;
				Lcache[CacheIndex].ehp = Private_Car.ehp;
			}
			if (Lcache[CacheIndex].cc != Private_Car.cc)
			{
				tf = true;
				Lcache[CacheIndex].cc = Private_Car.cc;
			}
			if (Lcache[CacheIndex].nm != Private_Car.nm)
			{
				tf = true;
				Lcache[CacheIndex].nm = Private_Car.nm;
			}
			if (Lcache[CacheIndex].redline != Private_Car.redline)
			{
				tf = true;
				Lcache[CacheIndex].redline = Private_Car.redline;
			}
			if (Lcache[CacheIndex].maxrpm != Private_Car.maxrpm)
			{
				tf = true;
				Lcache[CacheIndex].maxrpm = Private_Car.maxrpm;
			}
			if (Lcache[CacheIndex].idlerpm != Private_Car.idlerpm)		// SaveCar(): changing value in Cars?
			{
				tf = true;
				Lcache[CacheIndex].idlerpm = Private_Car.idlerpm;		// SaveCar(): Yes, value has changed
			}
			if (Lcache[CacheIndex].order != Private_Car.order)
			{
				tf = true;
				Lcache[CacheIndex].order = Private_Car.order;
			}
			if (Lcache[CacheIndex].defaults != Private_Car.defaults)
			{
				tf = true;
				Lcache[CacheIndex].defaults = Private_Car.defaults;
			}
			if (Lcache[CacheIndex].category != Private_Car.category)
			{
				tf = true;
				Lcache[CacheIndex].category = Private_Car.category;
			}
			if (Lcache[CacheIndex].notes != Private_Car.notes)
			{
				tf = true;
				Lcache[CacheIndex].notes = Private_Car.notes;
			}
			H.Save |= tf;
			return tf;
		}	// S.SaveCar()

		internal void Defaults(StatusDataBase db)
		{
			string StatusText = "Defaults:  ";

			if (null == DfltCar)
				DfltCar = new()
				{
					notes = StatusText = "Specs unknown",
					ehp = 0,
					cyl = 0,
					cc = 0,
					hp = 0,
					nm = 0
				};
			else if(!string.IsNullOrEmpty(DfltCar.notes))
				StatusText = DfltCar.notes;
            Src = StatusText;
			DfltCar.id = Car.id;						// CarId()
			DfltCar.name = db.CarModel;
			if (!string.IsNullOrEmpty(db.CarClass))
				DfltCar.category = db.CarClass;
			DfltCar.redline = redlineFromGame;
			DfltCar.maxrpm  = maxRPMFromGame;
			DfltCar.idlerpm = ushortIdleRPM;
			Cache(DfltCar);
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
			get => (ushort)(Private_Car.redline ?? 0);
			set { SetSpec(ref Private_Car.redline, value, nameof(Redline));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort MaxRPM
		{
			get => (ushort)(Private_Car.maxrpm ?? 0);
			set { SetSpec(ref Private_Car.maxrpm, value, nameof(MaxRPM));
					H.Changed = H.Set;				// SetSpec()
			}
		}

		public ushort IdleRPM								// public for Private_Car.idlerpm
		{
			get => (ushort)(Private_Car.idlerpm ?? 0);						// IdleRPM
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
			get => (ushort)(Private_Car.cyl ?? 0);
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
			get => (ushort)(Private_Car.hp ?? 0);
			set { SetSpec(ref Private_Car.hp, value, nameof(MaxPower));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort ElectricMaxPower
		{
			get => (ushort)(Private_Car.ehp ?? 0);
			set { SetSpec(ref Private_Car.ehp, value, nameof(ElectricMaxPower));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort Displacement
		{
			get => (ushort)(Private_Car.cc ?? 0);
			set { SetSpec(ref Private_Car.cc, value, nameof(Displacement));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	
		public ushort MaxTorque
		{
			get => (ushort)(Private_Car.nm ?? 0);
			set { SetSpec(ref Private_Car.nm, value, nameof(MaxTorque));
					H.Changed = H.Set;				// SetSpec()
			}
		}
	}	// class Spec
}
