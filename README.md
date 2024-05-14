# SimHap  
- decompiled by @**dMASS** from @**sierses**' `.dll`   
- decompiled `SimHap.csproj` hacked for SimHub plugin compatibility by @**blekelbleu**  
- [initial build errors](Doc/error1.txt)  
- [JToken.explicit build errors](Doc/error2.txt) after others addressed
- `namespace` renamed from `SimHaptics` to `sierses.SimHap`  
	- **Note**:  Visual Studio builds .cs files in `obj/` from `.xaml` using e.g.  
      `<UserControl x:Class="sierses.SimHap.SettingsControl"`
		- which must match (renamed) namespace...
### To Do (or at least consider)
- getting blank engine data:  
	![](Doc/blank.jpg)  
	*should be*:  
	![](Doc/engine.jpg)  
	- presumably based on wrong fix for [build errors](Doc/message.txt)  
	- *14 May 2024*:&nbsp;  seemingly eliminated `SettingsControl.xaml.cs` stripping for missing engine data
- lookup in .json for Internet fails
	- For unknown id, plugin presents users with list of known names not already mapped.  
	When users select a name, the new carID gets added to JSON for that name with old carID.
	![](Doc/spreadsheet.jpg)  
	*carID is columnC*
