# Haptics  
- decompiled by @**dMASS** from @**sierses**' `.dll`   
- decompiled `Haptics.csproj` hacked for SimHub plugin compatibility by @**blekelbleu**  
- [initial build errors](Doc/error1.txt)  
- [JToken.explicit build errors](Doc/error2.txt) after others addressed
- `namespace` renamed from `SimHaptics` to `sierses.Sim`  
	- **Note**:  Visual Studio builds .cs files in `obj/` from `.xaml` using e.g.  
      `<UserControl x:Class="sierses.Sim.SettingsControl"`
		- which must match (renamed) namespace...
### Done
-  blank engine data;&nbsp; *was*:  
	![](Doc/blank.jpg)  
	*now*:  
	![](Doc/engine.jpg)  
	- caused by wrong fix for [build errors](Doc/message.txt)  
	- *14 May 2024*:&nbsp;  eliminated `SettingsControl.xaml.cs` stripping for missing engine data
		- `Untoken()` replaced disallowed `JToken.op_Explicit(jtoken[(object) "name"])` 
### To Do (or at least consider)
- lookup in .json for Internet fails
	- For unknown id, plugin presents users with list of known names not already mapped.  
	When users select a name, the new carID gets added to JSON for that name with old carID.
### New to me
- [async](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios)
- [Dictionary](https://stackoverflow.com/questions/4245064/method-to-add-new-or-update-existing-item-in-c-sharp-dictionary)
### How it works
- with game running: `DataUpdate()`
	- `SetVehicle()` based on time
		- better done by event...
		- calls `FetchCarData()`
			- `async await` caused `FetchCarData()`  
				to *not return* during the same invocation,   
				so modified it to *recall its invoking method*  
				for completion in that invocation.   
		- may call SetDefaultVehicle(), which calls Spec Init()
	- update physics:
		- Yaw, YawRate, YawRateAvg
		- AccHeave, AccSurge, AccSway, Jerk[XYZ]
		- MotionPitch, MotionRoll, MotionYaw, MotionHeave
		- WheelLoads, Slips, Gear, ABS
		- Suspension, EngineLoad
- End()
	- remove some defaults from Settings Dictionaries
	- update Settings.Motion from D.Motion

- Init()
	- new Spec(), SimData()
	- SetGame()
	- Load and hack Settings
	- initialize SimData
	- AttachDelegate Spec, SimData

- UI
	- Motion, Suspension and Traction settings are saved in Settings Dictionaries
		- Only one set of Motion properties, all in a single Dictionary
		- Suspension and Traction have per-game dictionaries
	- Engine specs are downloaded

### changes
- consolidate `SimData` methods in that source file
- likewise for `Spec`
- created a `ListDictionary` class for download server compatibility
- began writing (and eventually reading) local json to preserve changed values
- reworked `IdleRPM` handling

### to do
- write json when values change - *20 May 2024*: coded  
- improve performance and simplify code - *22 May 2024*: in progress  
- share `CarSpec` class between `Spec` and `SimData` - *21 May 2024*: done  
	 to save storage and eliminate copying
- have UI entries affect properties
- check XAML data bindings
- add Log messages to sort issues
	- `async await` sequence
- free RAM in `Init` by discarding all but the current game dictionary after initial loading,
  then reloading in `End()` *only* to save changes.
- debug loading a default car after a server car or JSON car or vice-versa
- add a reference catalog lookup, for `PluginsData/Catalog.Haptics.json`,
	for cars *not* in personal JSON..
	That catalog would NOT get overwritten, preventing new cars from contaminating it.
- add another Spec entry, indicating cars from `Defaults()`, preserving that heritage.
- for loads, just reset `Changed` after updating `Save`
- `FetchCarData()` or Defaults(), these `Car`s will not exist in `Lcars`,
  so will set `Changed` automatically in `Add()`.
	
### refactoring
- when SimHub invokes `DataUpdate()` (at 60 Hz),
	- avoid invoking either `Refresh()` or `SetVehicle()` if `FetchStatus == APIStatus.Waiting`
- when `static async void FetchCarData()` eventually gets valid `Download dljc`,
	- set new `FetchStatus = APIStatus.Loaded` to preclude looping

### asynchronous event states *23 May 2024*
- sorting xaml Bindings got boring
- adding lots of log messages helps understand car Spec load events
- events are identified by: &nbsp;  enums `FetchStatus` and `LoadStatus`,  
	 booleans `Changed` and `LoadFinish`, and integer `Index`  
- in theory, a *very large* event state space, then...  
	- incrementally, *conditionally disable* log messages for *expected events*

- to prevent being recalled while waiting for server response,   
	`async FetchCarData()` immediately returns if called    
	when `DataStatus.NotAPI == LoadStatus || APIStatus.Fail == FetchStatus`  
- consequently, server car Specs are associated with  
	`DataStatus.NotAPI == LoadStatus && APIStatus.Loaded == FetchStatus`  
- but code *first* tries getting car Spec from JSON, identified by `Index >= 0`  
- if neither of those succeed,  `Defaults()` attempts to generate game-specific car `Spec`...  
	 but since `FetchCarData()` is indeed async, code is more *event driven* than *procedural*.  
- For example,  looking for new car `Spec`s depends on noting that `current_car != requested_car`  
- however, SimHub's 60Hz `DataUpdate()` can (and does) make calls  
  when `FetchCarData()` has matched `requested_car`,  
	`(DataStatus.NotAPI == LoadStatus && APIStatus.Loaded == FetchStatus)`  
     ...but car `Spec` code has not completed dotting I's and crossing T's....  
- when all I's are dotted and Ts are crossed,  `APIStatus.Success == FetchStatus`  
- if `Changed`, then `Add()` current car `Spec` to current game list
	- *just before* looking for *another* car
	- during `Exit()`
- while rarely logged, `APIStatus.Fail`, `APIStatus.Waiting`,  and `APIStatus.Retry` are *essential*  
- After further refactoring, `LoadFinish` may eventually turn out to be redundant to `APIStatus.Loaded`.  
- while 6 `LoadStatus` enums are defined, code currently tests for only 2..  
