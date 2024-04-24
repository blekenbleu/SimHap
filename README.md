# SimHap  
- decompiled by @**dMASS** from @**sierses**' `.dll`   
- decompiled `SimHap.csproj` hacked for SimHub plugin compatibility by @**blekelbleu**  
- [initial build errors](Doc/error1.txt)  
- [JToken.explicit build errors](Doc/error2.txt) after others addressed
- `namespace` renamed from `SimHaptics` to `sierses.SimHap`  
	- **Note**:  Visual Studio builds .cs files in `obj/` from `.xaml` using e.g.  
      `<UserControl x:Class="sierses.SimHap.SettingsControl"`
		- which must match (renamed) namespace...
