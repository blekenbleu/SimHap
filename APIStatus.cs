// Decompiled with JetBrains decompiler
// Type: SimHaptics.APIStatus
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

namespace sierses.SimHap
{
  public enum APIStatus
  {
    None,
    Waiting,	// disables attemting another FetchCarData()
    Retry,
    Fail,
    Success,
	Loaded
  }
}
