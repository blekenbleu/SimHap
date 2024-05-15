// Decompiled with JetBrains decompiler
// Type: SimHaptics.Spec
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using System.ComponentModel;

namespace sierses.SimHap
{
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

  public class Spec : INotifyPropertyChanged
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

    public Spec(Spec s)
      : this()
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
      set
      {
        this.game = value;
        this.OnPropertyChanged("game");
      }
    }

    public string Name
    {
      get => this.name;
      set
      {
        this.name = value;
        this.OnPropertyChanged("name");
      }
    }

    public string Id
    {
      get => this.id;
      set
      {
        this.id = value;
        this.OnPropertyChanged("id");
      }
    }

    public string Category
    {
      get => this.category;
      set
      {
        this.category = value;
        this.OnPropertyChanged("category");
      }
    }

    public double Redline
    {
      get => this.redline;
      set
      {
        this.redline = value;
        this.OnPropertyChanged("redline");
      }
    }

    public double MaxRPM
    {
      get => this.maxRPM;
      set
      {
        this.maxRPM = value;
        this.OnPropertyChanged("maxRPM");
      }
    }

    public string EngineConfiguration
    {
      get => this.engineConfiguration;
      set
      {
        this.engineConfiguration = value;
        this.OnPropertyChanged("engineConfiguration");
      }
    }

    public double EngineCylinders
    {
      get => this.engineCylinders;
      set
      {
        this.engineCylinders = value;
        this.OnPropertyChanged("engineCylinders");
      }
    }

    public string EngineLocation
    {
      get => this.engineLocation;
      set
      {
        this.engineLocation = value;
        this.OnPropertyChanged("engineLocation");
      }
    }

    public string PoweredWheels
    {
      get => this.poweredWheels;
      set
      {
        this.poweredWheels = value;
        this.OnPropertyChanged("poweredWheels");
      }
    }

    public double MaxPower
    {
      get => this.maxPower;
      set
      {
        this.maxPower = value;
        this.OnPropertyChanged("maxPower");
      }
    }

    public double ElectricMaxPower
    {
      get => this.electricMaxPower;
      set
      {
        this.electricMaxPower = value;
        this.OnPropertyChanged("electricMaxPower");
      }
    }

    public double Displacement
    {
      get => this.displacement;
      set
      {
        this.displacement = value;
        this.OnPropertyChanged("displacement");
      }
    }

    public double MaxTorque
    {
      get => this.maxTorque;
      set
      {
        this.maxTorque = value;
        this.OnPropertyChanged("maxTorque");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
