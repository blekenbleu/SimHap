// Decompiled with JetBrains decompiler
// Type: SimHaptics.SpecStatus
// Assembly: SimHaptics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
// Assembly location: C:\Users\demas\Downloads\dnSpy-net-win64\SimHaptics.dll

namespace sierses.SimHap
{
  public struct SpecStatus
  {
    public DataStatus Name;
    public DataStatus Id;
    public DataStatus Category;
    public DataStatus Redline;
    public DataStatus MaxRPM;
    public DataStatus EngineConfig;
    public DataStatus EngineCylinders;
    public DataStatus EngineLocation;
    public DataStatus PoweredWheels;
    public DataStatus Displacement;
    public DataStatus MaxPower;
    public DataStatus ElectricMaxPower;
    public DataStatus MaxTorque;

    public void SetAll(DataStatus status)
    {
      this.Name = status;
      this.Id = status;
      this.Category = status;
      this.Redline = status;
      this.MaxRPM = status;
      this.EngineConfig = status;
      this.EngineCylinders = status;
      this.EngineLocation = status;
      this.PoweredWheels = status;
      this.Displacement = status;
      this.MaxPower = status;
      this.ElectricMaxPower = status;
      this.MaxTorque = status;
    }
  }
}
