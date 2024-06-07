// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimHapticsPlugin
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;
using Newtonsoft.Json;
using sierses.Sim.Properties;
using SimHub;
using SimHub.Plugins;
using SimHub.Plugins.DataPlugins.ShakeItV3.Outputs.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace sierses.Sim
{
	[PluginDescription("Properties for haptic feedback and more")]
	[PluginAuthor("sierses")]
	[PluginName("Haptics")]
	public class Haptics : IPlugin, IDataPlugin, IWPFSettingsV2 //, IWPFSettings
	{
		public string PluginVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
		public static int LoadFailCount;
		public static long FrameTimeTicks = 0;
		public static long FrameCountTicks = 0;
		public static GameId CurrentGame = GameId.Other;
		public static string GameDBText;
		internal static bool Loaded, Waiting, Save, Set, Changed;
		private static readonly HttpClient client = new();
		private readonly string myfile = $"PluginsData/{nameof(Haptics)}.{Environment.UserName}.json";
		private readonly string Atlasfile = $"PluginsData/{nameof(Haptics)}.Atlas.json";
		internal static List<CarSpec> Atlas;
		internal static int AtlasCt;		// use this to force Atlas
		public Spec S { get; } = new() { };

		public SimData D { get; set; }

		public Geq E { get; set; } 

		public ImageSource PictureIcon
		{
			get { return this.ToIcon(Resources.SimHapticsShakerStyleIcon_alt012); }
		}

		public ImageSource RPMIcon
		{
			get { return this.ToIcon(Resources._100x100_RPM_White); }
		}

		public ImageSource ImpactsIcon
		{
			get { return this.ToIcon(Resources._100x100_Impacts_White); }
		}

		public ImageSource SuspensionIcon
		{
			get { return this.ToIcon(Resources._100x100_Suspension_White); }
		}

		public ImageSource TractionIcon
		{
			get { return this.ToIcon(Resources._100x100_Traction_White); }
		}
/*
		private List<string[]> Sling(DownloadData data)
		{
			return new List<string[]>
			{
				new string[] { "notes", data.notes },
				new string[] { "cc", data.cc.ToString() },
				new string[] { "nm", data.nm.ToString() },
				new string[] { "ehp", data.ehp.ToString() },
				new string[] { "hp", data.hp.ToString() },
				new string[] { "drive", data.drive },
				new string[] { "config", data.config },
				new string[] { "cyl", data.cyl.ToString() },
				new string[] { "loc", data.loc },
				new string[] { "maxrpm", data.maxrpm.ToString() },
				new string[] { "redline", data.redline.ToString() },
				new string[] { "category", data.category },
				new string[] { "defaults", data.defaults },
				new string[] { "name", data.name },
				new string[] { "id", data.id },
				new string[] { "game", data.game }
			};
		}
 */

		// boilerplate SimHub -------------------------------------------
		public string LeftMenuTitle => "Haptics";

		public Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return new SettingsControl(this);
		}

		public Settings Settings { get; set; }

		public PluginManager PluginManager { get; set; }
		// ----------------------------------------------------------------

		// must be void and static;  invoked by D.SetVehicle()
		private static Haptics This;

		internal static async void FetchCarData(	// called from SetVehicle() switch
			string id,
			string category,
			ushort redlineFromGame,
			ushort maxRPMFromGame,
			ushort ushortIdleRPM)							// FetchCarData() argument
		{
			Logging.Current.Info($"Haptics.FetchCarData({id}/{category}):  Index = {This.D.Index}," +
							   (Save ? " Save " : "") + (Loaded ? " Loaded " : "") + (Waiting ? " Waiting" : "")
								+ (Set ? " Set": "") + (Changed ? "Changed " : ""));

			if (-2 == This.D.Index)	// first time for this CarId change?  Also called for retries with Index = -1
			{
				This.S.Notes = "";
				Set = false;
				StatusDataBase db = This.Gdat.NewData;
				string sid = (GameId.Forza == CurrentGame && "Car_" == db.CarId.Substring(0,4)) ? db.CarId.Substring(4) : db.CarId;
				This.D.Index = This.S.SelectCar(sid, // set game RPM defaults
													redlineFromGame, maxRPMFromGame, ushortIdleRPM);
				Logging.Current.Info($"Haptics.SelectCar({sid}): "
									+ (Save ? " Save " : "") + (Loaded ? " Loaded " : "") + (Waiting ? " Waiting" : "")
							   		+ (Set ? " Set": "") + (Changed ? "Changed " : "")
									+ $" Index = {This.D.Index}");
				if (0 <= This.D.Index)
					return;
			}
			// Index should be -1; non-negative values returned
			try
			{
				Waiting = true;
				string dls = null;
				id ??= "0";
				category ??= "0";
				Uri requestUri = new("https://api.simhaptics.com/data/" + GameDBText
								 + "/" + Uri.EscapeDataString(id) + "/" + Uri.EscapeDataString(category));
				HttpResponseMessage async = await client.GetAsync(requestUri);
				async.EnsureSuccessStatusCode();
				dls = async.Content.ReadAsStringAsync().Result;
				if (null != dls && 11 < dls.Length)
				{
					Waiting = false;	// ReadAsStringAsync() success
					Download dljc =	JsonConvert.DeserializeObject<Download>(dls,
										new JsonSerializerSettings {
											NullValueHandling = NullValueHandling.Ignore,
											MissingMemberHandling = MissingMemberHandling.Ignore
										}
									);
					if (null != dljc && null != dljc.data && 0 < dljc.data.Count
					 && null != dljc.data[0].id && null != dljc.data[0].game && null != dljc.data[0].name)
					{
						CarSpec car = dljc.data[0];
						car.defaults = "DB";
						This.S.Cache(car);								// FetchCarData(): Set(id) at the end of SetVehicle()
						LoadFailCount = 1;
						Logging.Current.Info($"Haptics.FetchCarData({car.name}): Successfully loaded; CarInitCount = {This.D.CarInitCount}");
						This.D.CarInitCount = 0;
						This.D.Index = -4;
						return;
					}
				}
				else if (null != dls)
				{
					if (-1 == This.D.Index)			// delayed dls? things may have moved on...
						This.D.Index = -3;			// disable self until other code decides otherwise
					if (11 == dls.Length)
						Waiting = false;
/*
						Logging.Current.Info($"Haptics.FetchCarData({id}): not in DB");
					else if (0 < dls.Length)
						Logging.Current.Info($"Haptics.FetchCarData({id}): JsonConvert fail;  result length {dls.Length}: "
											+ dls.Substring(0, dls.Length > 20 ? 20 : dls.Length)); // (<= 20) dls characters
 */
				}
				// else Waiting
			}
			catch (HttpRequestException ex)	//  treat it like not in DB
			{
				Logging.Current.Error("Haptics.FetchCarData() Error: " + ex.Message);
				Waiting = false;
			}
		}		// FetchCarData()

		/// <summary>
		/// Called one time per game data update, contains all normalized game data.
		/// Raw data are intentionnally "hidden" under a generic object type (plugins SHOULD NOT USE)
		/// This method is on the critical path, must execute as fast as possible and avoid throwing any error
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including present and previous data frames.</param> 
		internal GameData Gdat;
		internal PluginManager PM;
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			if (null == data.NewData)
				return;

			Gdat = data;
			PM = pluginManager;

			if (S.Id == data.NewData.CarId || !D.Unlocked)				// DataUpdate()
			{
				if ((data.GameRunning && null != data.OldData)
					&& 1 == (int)PM.GetPropertyValue("DataCorePlugin.GameData.EngineIgnitionOn"))
					D.Refresh(ref data, this);
				return;
			}

			if (Waiting)
			{
				if ((30 * LoadFailCount) > ++D.CarInitCount)
					return;

				Waiting = false;			// CarInitCount timeout
				if (4 > LoadFailCount++)
					return;					// do not give up (yet)

				D.Index = -3;					   // disable FetchCarData(); enable Defaults()
				D.CarInitCount = 0;
				Logging.Current.Info($"Haptics.DataUpdate({data.NewData.CarId}/{S.Id}):  async Waiting timeout" +
									 (Save ? " Save" : "") + (Loaded ? " Loaded" : "")
									+ (Set ? " Set": "") + (Changed ? " Changed" : "" + $" Index = {D.Index}"));
				Changed = false;
			}
			else if (Loaded || Changed)		  // save before SetVehicle()
			{
				if (null == S.Car.name)
					Logging.Current.Info($"Haptics.S.Add({S.Id}) : missing car name");
				else if (S.Add(S.Id))		// DataUpdate():  add or update S.Car in Cars list
					S.LD.Add(S.Car);		// DataUpdate()
				Loaded = false;
			}


			if (data.GameRunning || data.GamePaused || data.GameReplay || data.GameInMenu)
			{
				if (-2 == D.Index)
					Set = Changed = false;
				Logging.Current.Info($"Haptics.DataUpdate({data.NewData.CarId}/{S.Id}): "
									+ (Save ? " Save" : "") + (Loaded ? " Loaded" : "") + (Waiting ? " Waiting" : "")
									+ (Set ? " Set": "") + (Changed ? " Changed" : "") + $" Index = {D.Index}");
				D.SetVehicle(this);
			}
		}	// DataUpdate()

		// remove 0 value ushorts
		private readonly List<string> zero = new() { "ehp", "idlerpm", "maxrpm", "redline" }; // Null0() remove these if 0 value before saving JSON
		private string Null0(string j)
		{
			for (int i = 0; i < zero.Count; i++)
				j = j.Replace($",\r\n	  \"{zero[i]}\": 0,", ",");
			return j;
		}

		public void End(PluginManager pluginManager)
		{
			if (Save || Loaded || Changed)		// End()
			{
				if (Loaded || Changed)
					S.LD.Add(S.Car);			// End()
				string sjs = (null == S.LD) ? "" : Null0(S.LD.Jstring());	// delete 0 ushorts
				if (0 == sjs.Length || "{}" == sjs)
					Logging.Current.Info( $"Haptics.End(): JSON Serializer failure: "
									+ $"{S.LD.Count} games, {S.Cars.Count} {S.Car.game} cars;  "
									+ (Save ? "changes made.." : "(no changes)"));
				else if (Save) { 
					File.WriteAllText(myfile, sjs);
					Logging.Current.Info( $"Haptics.End(): {S.LD.Count} games, including {S.LD.CarCount(GameDBText)} {GameDBText} cars, written to " + myfile);
				}
			}
			if (!Save)
				Logging.Current.Info("Haptics.End():  JSON not saved");

			// Remove default values from Settings per-game dictionaries
			if (Settings.EngineMult.TryGetValue("AllGames", out double _))
				Settings.EngineMult.Remove("AllGames");
			if (Settings.EngineMult.TryGetValue(GameDBText, out double _))
			{
				if (D.EngineMult == 1.0)
					Settings.EngineMult.Remove(GameDBText);
				else Settings.EngineMult[GameDBText] = D.EngineMult;
			}
			else if (D.EngineMult != 1.0)
				Settings.EngineMult.Add(GameDBText, D.EngineMult);
			if (Settings.RumbleMult.TryGetValue(GameDBText, out double _))
			{
				if (D.RumbleMult == 1.0)
					Settings.RumbleMult.Remove(GameDBText);
				else Settings.RumbleMult[GameDBText] = D.RumbleMult;
			}
			else if (D.RumbleMult != 1.0)
				Settings.RumbleMult.Add(GameDBText, D.RumbleMult);
			if (Settings.SuspensionMult.TryGetValue(GameDBText, out double _))
			{
				if (D.SuspensionMult == 1.0)
					Settings.SuspensionMult.Remove(GameDBText);
				else Settings.SuspensionMult[GameDBText] = D.SuspensionMult;
			}
			else if (D.SuspensionMult != 1.0)
				Settings.SuspensionMult.Add(GameDBText, D.SuspensionMult);
			if (Settings.SuspensionGamma.TryGetValue(GameDBText, out double _))
			{
				if (D.SuspensionGamma == 1.0)
					Settings.SuspensionGamma.Remove(GameDBText);
				else Settings.SuspensionGamma[GameDBText] = D.SuspensionGamma;
			}
			else if (D.SuspensionGamma != 1.0)
				Settings.SuspensionGamma.Add(GameDBText, D.SuspensionGamma);
			if (Settings.SlipXMult.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipXMult == 1.0)
					Settings.SlipXMult.Remove(GameDBText);
				else Settings.SlipXMult[GameDBText] = D.SlipXMult;
			}
			else if (D.SlipXMult != 1.0)
				Settings.SlipXMult.Add(GameDBText, D.SlipXMult);
			if (Settings.SlipYMult.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipYMult == 1.0)
					Settings.SlipYMult.Remove(GameDBText);
				else Settings.SlipYMult[GameDBText] = D.SlipYMult;
			}
			else if (D.SlipYMult != 1.0)
				Settings.SlipYMult.Add(GameDBText, D.SlipYMult);
			if (Settings.SlipXGamma.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipXGamma == 1.0)
					Settings.SlipXGamma.Remove(GameDBText);
				else Settings.SlipXGamma[GameDBText] = D.SlipXGamma;
			}
			else if (D.SlipXGamma != 1.0)
				Settings.SlipXGamma.Add(GameDBText, D.SlipXGamma);
			if (Settings.SlipYGamma.TryGetValue(GameDBText, out double _))
			{
				if (D.SlipYGamma == 1.0)
					Settings.SlipYGamma.Remove(GameDBText);
				else Settings.SlipYGamma[GameDBText] = D.SlipYGamma;
			}
			else if (D.SlipYGamma != 1.0)
				Settings.SlipYGamma.Add(GameDBText, D.SlipYGamma);

			// unconditionally save some
			Settings.RumbleMult["AllGames"] = D.RumbleMultAll;
			Settings.SuspensionGamma["AllGames"] = D.SuspensionGammaAll;
			Settings.SuspensionMult["AllGames"] = D.SuspensionMultAll;
			Settings.SlipXMult["AllGames"] = D.SlipXMultAll;
			Settings.SlipYMult["AllGames"] = D.SlipYMultAll;
			Settings.Motion["MotionPitchOffset"] = D.MotionPitchOffset;
			Settings.Motion["MotionPitchMult"] = D.MotionPitchMult;
			Settings.Motion["MotionPitchGamma"] = D.MotionPitchGamma;
			Settings.Motion["MotionRollOffset"] = D.MotionRollOffset;
			Settings.Motion["MotionRollMult"] = D.MotionRollMult;
			Settings.Motion["MotionRollGamma"] = D.MotionRollGamma;
			Settings.Motion["MotionYawOffset"] = D.MotionYawOffset;
			Settings.Motion["MotionYawMult"] = D.MotionYawMult;
			Settings.Motion["MotionYawGamma"] = D.MotionYawGamma;
			Settings.Motion["MotionHeaveOffset"] = D.MotionHeaveOffset;
			Settings.Motion["MotionHeaveMult"] = D.MotionHeaveMult;
			Settings.Motion["MotionHeaveGamma"] = D.MotionHeaveGamma;
			Settings.Motion["MotionSurgeOffset"] = D.MotionSurgeOffset;
			Settings.Motion["MotionSurgeMult"] = D.MotionSurgeMult;
			Settings.Motion["MotionSurgeGamma"] = D.MotionSurgeGamma;
			Settings.Motion["MotionSwayOffset"] = D.MotionSwayOffset;
			Settings.Motion["MotionSwayMult"] = D.MotionSwayMult;
			Settings.Motion["MotionSwayGamma"] = D.MotionSwayGamma;
			this.SaveCommonSettings("Settings", Settings);
		}

		// Init() methods -----------------------------
		public void SetGame(PluginManager pm)
		{
			GameDBText = D.GameAltText = pm.GameName;
			switch (GameDBText)
			{
				case "AssettoCorsa":
					CurrentGame = GameId.AC;
					GameDBText = "AC";
					D.RumbleFromPlugin = true;
					break;
				case "AssettoCorsaCompetizione":
					CurrentGame = GameId.ACC;
					GameDBText = "ACC";
					break;
				case "Automobilista":
					CurrentGame = GameId.AMS1;
					GameDBText = "AMS1";
					break;
				case "Automobilista2":
					CurrentGame = GameId.AMS2;
					GameDBText = "AMS2";
					break;
				case "FH4":
				case "FH5":
				case "FM7":
				case "FM8":
					CurrentGame = GameId.Forza;
					GameDBText = "Forza";
					break;
				case "IRacing":
					CurrentGame = GameId.IRacing;
					D.GameAltText += (string) pm.GetPropertyValue(D.raw+"SessionData.WeekendInfo.Category");
					break;
				case "KartKraft":
					CurrentGame = GameId.KK;
					GameDBText = "KK";
					break;
				case "LFS":
					CurrentGame = GameId.LFS;
					break;
				case "PCars1":
				case "PCars2":
				case "PCars3":
					CurrentGame = GameId.PC2;
					GameDBText = "PC2";
					break;
				case "RBR":
					CurrentGame = GameId.RBR;
					D.TireDiameterSampleCount = -1;
					break;
				case "RFactor1":
					CurrentGame = GameId.RF1;
					GameDBText = "RF1";
					break;
				case "RFactor2":
				case "RFactor2Spectator":
					CurrentGame = GameId.RF2;
					GameDBText = "RF2";
					break;
				case "LMU":
					CurrentGame = GameId.LMU;
					GameDBText = "LMU";
					break;
				case "RRRE":
					CurrentGame = GameId.RRRE;
					break;
				case "SIMBINGTLEGENDS":
					CurrentGame = GameId.GTL;
					GameDBText = "GTL";
					break;
				case "SIMBINGTR2":
					CurrentGame = GameId.GTR2;
					GameDBText = "GTR2";
					break;
				case "SIMBINRACE07":
					CurrentGame = GameId.RACE07;
					GameDBText = "RACE07";
					break;
				case "StockCarExtreme":
					CurrentGame = GameId.GSCE;
					GameDBText = "GSCE";
					break;
				case "CodemastersDirtRally1":
				case "CodemastersDirtRally2":
					CurrentGame = GameId.DR2;
					GameDBText = "DR2";
					D.TireDiameterSampleCount = -1;
					break;
				case "CodemastersDirt2":
				case "CodemastersDirt3":
				case "CodemastersDirtShowdown":
				case "CodemastersDirt4":
					CurrentGame = GameId.D4;
					GameDBText = "D4";
					D.TireDiameterSampleCount = -1;
					break;
				case "EAWRC23":
					CurrentGame = GameId.WRC23;
					GameDBText = "WRC23";
					D.AccSamples = 32;
					D.TireDiameterSampleCount = -1;
					break;
				case "F12012":
				case "F12013":
				case "F12014":
				case "F12015":
				case "F12016":
					CurrentGame = GameId.F12016;
					GameDBText = "F12016";
					break;
				case "F12017":
					CurrentGame = GameId.F12017;
					D.TireDiameterSampleCount = -1;
					break;
				case "F12018":
				case "F12019":
				case "F12020":
				case "F12021":
				case "F12022":
					CurrentGame = GameId.F12022;
					GameDBText = "F12022";
					D.TireDiameterSampleCount = -1;
					break;
				case "F12023":
				case "F12024":
				case "F12025":
				case "F12026":
					CurrentGame = GameId.F12023;
					GameDBText = "F12022";
					D.TireDiameterSampleCount = -1;
					break;
				case "CodemastersGrid2":
				case "CodemastersGrid2019":
				case "CodemastersAutosport":
				case "CodemastersGridLegends":
					CurrentGame = GameId.GLegends;
					GameDBText = "Grid";
					D.TireDiameterSampleCount = -1;
					break;
				case "BeamNgDrive":
					CurrentGame = GameId.BeamNG;
					GameDBText = "BeamNG";
					D.TireDiameterSampleCount = -1;
					break;
				case "GPBikes":
					CurrentGame = GameId.GPBikes;
					D.RumbleFromPlugin = true;
					D.TireDiameterSampleCount = -1;
					break;
				case "MXBikes":
					CurrentGame = GameId.MXBikes;
					D.TireDiameterSampleCount = -1;
					break;
				case "WRCGenerations":
					CurrentGame = GameId.WRCGen;
					D.TireDiameterSampleCount = -1;
					break;
				case "WRCX":
					CurrentGame = GameId.WRCX;
					break;
				case "WRC10":
					CurrentGame = GameId.WRC10;
					break;
				case "ATS":
					CurrentGame = GameId.ATS;
					break;
				case "ETS2":
					CurrentGame = GameId.ETS2;
					break;
				case "GranTurismo7":
				case "GranTurismoSport":
					CurrentGame = GameId.GranTurismo7;
					GameDBText = "GranTurismo7";
					D.TireDiameterSampleCount = -1;
					break;
				default:
					CurrentGame = GameId.Other;
					break;
			}

			D.AccHeave = new double[D.AccSamples];
			D.AccSurge = new double[D.AccSamples];
			D.AccSway = new double[D.AccSamples];
		}	// SetGame()

		public void Init(PluginManager pluginManager)
		{
			This = this;
			LoadFailCount = 1;
			D = new SimData();

            SetGame(pluginManager);
			Settings = this.ReadCommonSettings("Settings", () => new Settings());
			Settings.ABSPulseLength = Settings.ABSPulseLength > 0 ? Settings.ABSPulseLength : 2;
			Settings.DownshiftDurationMs = Settings.DownshiftDurationMs > 0 ? Settings.DownshiftDurationMs : 400;
			Settings.UpshiftDurationMs = Settings.UpshiftDurationMs > 0 ? Settings.UpshiftDurationMs : 400;
			if (Settings.EngineMult == null)
				Settings.EngineMult = new Dictionary<string, double>();
			if (!Settings.EngineMult.TryGetValue("AllGames", out double num))
				Settings.EngineMult.Add("AllGames", 1.0);
			if (Settings.RumbleMult == null)
				Settings.RumbleMult = new Dictionary<string, double>();
			if (!Settings.RumbleMult.TryGetValue("AllGames", out num))
				Settings.RumbleMult.Add("AllGames", 5.0);
			if (Settings.SuspensionMult == null)
				Settings.SuspensionMult = new Dictionary<string, double>();
			if (!Settings.SuspensionMult.TryGetValue("AllGames", out num))
				Settings.SuspensionMult.Add("AllGames", 1.5);
			if (Settings.SuspensionGamma == null)
				Settings.SuspensionGamma = new Dictionary<string, double>();
			if (!Settings.SuspensionGamma.TryGetValue("AllGames", out num))
				Settings.SuspensionGamma.Add("AllGames", 1.75);
			if (Settings.SlipXMult == null)
				Settings.SlipXMult = new Dictionary<string, double>();
			if (!Settings.SlipXMult.TryGetValue("AllGames", out num))
				Settings.SlipXMult.Add("AllGames", 1.6);
			if (Settings.SlipYMult == null)
				Settings.SlipYMult = new Dictionary<string, double>();
			if (!Settings.SlipYMult.TryGetValue("AllGames", out num))
				Settings.SlipYMult.Add("AllGames", 1.0);
			if (Settings.SlipXGamma == null)
				Settings.SlipXGamma = new Dictionary<string, double>();
			if (!Settings.SlipXGamma.TryGetValue("AllGames", out num))
				Settings.SlipXGamma.Add("AllGames", 1.0);
			if (Settings.SlipYGamma == null)
				Settings.SlipYGamma = new Dictionary<string, double>();
			if (!Settings.SlipYGamma.TryGetValue("AllGames", out num))
				Settings.SlipYGamma.Add("AllGames", 1.0);
			if (Settings.Motion == null)
				Settings.Motion = new Dictionary<string, double>();

			string Atlasst = "";
			AtlasCt = -1;				// S.Extract() will attempt extracting List<CarSpec> Atlas from json
			if (File.Exists(Atlasfile))
			{
				S.Extract(JsonConvert.DeserializeObject<Dictionary<string,
							List<CarSpec>>>(File.ReadAllText(Atlasfile)), GameDBText);  // to Atlas
				AtlasCt = Atlas.Count;
//				Logging.Current.Info($"Haptics.Init():  {Atlas.Count} games and "
//								   + $"{AtlasCt} {GameDBText} cars in " + Atlasfile);
				if (0 < AtlasCt)
					Atlasst = $" and {AtlasCt} cars in Atlas";
				else
					Logging.Current.Info($"Haptics.Init(): {Atlasfile} load failure");
			}
			else AtlasCt = 0;			// S.Extract() will attempt setting LD = json
			if (File.Exists(myfile))
			{
				string text = File.ReadAllText(myfile);
				Dictionary<string, List<CarSpec>> json = JsonConvert.DeserializeObject<Dictionary<string, List<CarSpec>>>(text);
				Logging.Current.Info("Haptics.Init():  " + S.LD.Set(json) + myfile + Atlasst);
			}
			else Logging.Current.Info("Haptics.Init():  "+myfile+" not found" + Atlasst);

			D.Init(Settings, this);
			E = new();
			E.Init(this);
			Save = Loaded = Waiting = Set = Changed = false;		// Init()
			this.AttachDelegate("CarName", () => S.CarName);
			this.AttachDelegate("CarId", () => S.Id);
			this.AttachDelegate("Category", () => S.Category);
			this.AttachDelegate("Defaults", () => S.Default);
			this.AttachDelegate("RedlineRPM", () => S.Redline);
			this.AttachDelegate("MaxRPM", () => S.MaxRPM);
			this.AttachDelegate("EngineConfig", () => S.EngineConfiguration);
			this.AttachDelegate("EngineCylinders", () => S.EngineCylinders);
			this.AttachDelegate("EngineLocation", () => S.EngineLocation);
			this.AttachDelegate("PoweredWheels", () => S.PoweredWheels);
			this.AttachDelegate("DisplacementCC", () => S.Displacement);
			this.AttachDelegate("PowerTotalHP", () => S.MaxPower);
			this.AttachDelegate("PowerEngineHP", () => S.MaxPower - S.ElectricMaxPower);
			this.AttachDelegate("PowerMotorHP", () => S.ElectricMaxPower);
			this.AttachDelegate("MaxTorqueNm", () => S.MaxTorque);
			this.AttachDelegate("EngineLoad", () => D.EngineLoad);
			this.AttachDelegate("IdleRPM", () => S.IdleRPM);			// Init()
			this.AttachDelegate("FreqHarmonic", () => D.FreqHarmonic);
			this.AttachDelegate("FreqOctave", () => D.FreqOctave);
			this.AttachDelegate("FreqIntervalA1", () => D.FreqIntervalA1);
			this.AttachDelegate("FreqIntervalA2", () => D.FreqIntervalA2);
			this.AttachDelegate("FreqLFEAdaptive", () => D.FreqLFEAdaptive);
			this.AttachDelegate("FreqPeakA1", () => D.FreqPeakA1);
			this.AttachDelegate("FreqPeakB1", () => D.FreqPeakB1);
			this.AttachDelegate("FreqPeakA2", () => D.FreqPeakA2);
			this.AttachDelegate("FreqPeakB2", () => D.FreqPeakB2);
			this.AttachDelegate("Gain1H", () => D.Gain1H);
			this.AttachDelegate("Gain1H2", () => D.Gain1H2);
			this.AttachDelegate("Gain2H", () => D.Gain2H);
			this.AttachDelegate("Gain4H", () => D.Gain4H);
			this.AttachDelegate("GainOctave", () => D.GainOctave);
			this.AttachDelegate("GainIntervalA1", () => D.GainIntervalA1);
			this.AttachDelegate("GainIntervalA2", () => D.GainIntervalA2);
			this.AttachDelegate("GainPeakA1Front", () => D.GainPeakA1Front);
			this.AttachDelegate("GainPeakA1Middle", () => D.GainPeakA1);
			this.AttachDelegate("GainPeakA1Rear", () => D.GainPeakA1Rear);
			this.AttachDelegate("GainPeakA2Front", () => D.GainPeakA2Front);
			this.AttachDelegate("GainPeakA2Middle", () => D.GainPeakA2);
			this.AttachDelegate("GainPeakA2Rear", () => D.GainPeakA2Rear);
			this.AttachDelegate("GainPeakB1Front", () => D.GainPeakB1Front);
			this.AttachDelegate("GainPeakB1Middle", () => D.GainPeakB1);
			this.AttachDelegate("GainPeakB1Rear", () => D.GainPeakB1Rear);
			this.AttachDelegate("GainPeakB2Front", () => D.GainPeakB2Front);
			this.AttachDelegate("GainPeakB2Middle", () => D.GainPeakB2);
			this.AttachDelegate("GainPeakB2Rear", () => D.GainPeakB2Rear);
			this.AttachDelegate("SlipXFL", () => D.SlipXFL);
			this.AttachDelegate("SlipXFR", () => D.SlipXFR);
			this.AttachDelegate("SlipXRL", () => D.SlipXRL);
			this.AttachDelegate("SlipXRR", () => D.SlipXRR);
			this.AttachDelegate("SlipXAll", () => D.SlipXAll);
			this.AttachDelegate("SlipYFL", () => D.SlipYFL);
			this.AttachDelegate("SlipYFR", () => D.SlipYFR);
			this.AttachDelegate("SlipYRL", () => D.SlipYRL);
			this.AttachDelegate("SlipYRR", () => D.SlipYRR);
			this.AttachDelegate("SlipYAll", () => D.SlipYAll);
			this.AttachDelegate("WheelLockAll", () => D.WheelLockAll);
			this.AttachDelegate("WheelSpinAll", () => D.WheelSpinAll);
			this.AttachDelegate("TireDiameterFL", () => D.TireDiameterFL);
			this.AttachDelegate("TireDiameterFR", () => D.TireDiameterFR);
			this.AttachDelegate("TireDiameterRL", () => D.TireDiameterRL);
			this.AttachDelegate("TireDiameterRR", () => D.TireDiameterRR);
			this.AttachDelegate("TireSpeedFL", () => D.WheelSpeedFL);
			this.AttachDelegate("TireSpeedFR", () => D.WheelSpeedFR);
			this.AttachDelegate("TireSpeedRL", () => D.WheelSpeedRL);
			this.AttachDelegate("TireSpeedRR", () => D.WheelSpeedRR);
			this.AttachDelegate("SpeedMs", () => D.SpeedMs);
			this.AttachDelegate("TireLoadFL", () => D.WheelLoadFL);
			this.AttachDelegate("TireLoadFR", () => D.WheelLoadFR);
			this.AttachDelegate("TireLoadRL", () => D.WheelLoadRL);
			this.AttachDelegate("TireLoadRR", () => D.WheelLoadRR);
			this.AttachDelegate("YawRate", () => D.YawRate);
			this.AttachDelegate("YawRateAvg", () => D.YawRateAvg);
			this.AttachDelegate("SuspensionFreq", () => D.SuspensionFreq);
			this.AttachDelegate("SuspensionFreqR0a", () => D.SuspensionFreqRa);
			this.AttachDelegate("SuspensionFreqR0b", () => D.SuspensionFreqRb);
			this.AttachDelegate("SuspensionFreqR0c", () => D.SuspensionFreqRc);
			this.AttachDelegate("SuspensionFreqR1", () => D.SuspensionFreqR1);
			this.AttachDelegate("SuspensionFreqR2", () => D.SuspensionFreqR2);
			this.AttachDelegate("SuspensionFreqR3", () => D.SuspensionFreqR3);
			this.AttachDelegate("SuspensionFreqR4", () => D.SuspensionFreqR4);
			this.AttachDelegate("SuspensionFreqR5", () => D.SuspensionFreqR5);
			this.AttachDelegate("SuspensionMultR0a", () => D.SuspensionMultRa);
			this.AttachDelegate("SuspensionMultR0b", () => D.SuspensionMultRb);
			this.AttachDelegate("SuspensionMultR0c", () => D.SuspensionMultRc);
			this.AttachDelegate("SuspensionMultR1", () => D.SuspensionMultR1);
			this.AttachDelegate("SuspensionMultR2", () => D.SuspensionMultR2);
			this.AttachDelegate("SuspensionMultR3", () => D.SuspensionMultR3);
			this.AttachDelegate("SuspensionMultR4", () => D.SuspensionMultR4);
			this.AttachDelegate("SuspensionMultR5", () => D.SuspensionMultR5);
			this.AttachDelegate("SuspensionRumbleMultR0b", () => D.SuspensionRumbleMultRb);
			this.AttachDelegate("SuspensionRumbleMultR0c", () => D.SuspensionRumbleMultRc);
			this.AttachDelegate("SuspensionRumbleMultR1", () => D.SuspensionRumbleMultR1);
			this.AttachDelegate("SuspensionRumbleMultR2", () => D.SuspensionRumbleMultR2);
			this.AttachDelegate("SuspensionRumbleMultR3", () => D.SuspensionRumbleMultR3);
			this.AttachDelegate("SuspensionRumbleMultR4", () => D.SuspensionRumbleMultR4);
			this.AttachDelegate("SuspensionRumbleMultR5", () => D.SuspensionRumbleMultR5);
			this.AttachDelegate("SuspensionFL", () => D.SuspensionFL);
			this.AttachDelegate("SuspensionFR", () => D.SuspensionFR);
			this.AttachDelegate("SuspensionRL", () => D.SuspensionRL);
			this.AttachDelegate("SuspensionRR", () => D.SuspensionRR);
			this.AttachDelegate("SuspensionFront", () => D.SuspensionFront);
			this.AttachDelegate("SuspensionRear", () => D.SuspensionRear);
			this.AttachDelegate("SuspensionLeft", () => D.SuspensionLeft);
			this.AttachDelegate("SuspensionRight", () => D.SuspensionRight);
			this.AttachDelegate("SuspensionAll", () => D.SuspensionAll);
			this.AttachDelegate("SuspensionAccAll", () => D.SuspensionAccAll);
			this.AttachDelegate("RumbleFromPlugin", () => D.RumbleFromPlugin);
			this.AttachDelegate("RumbleMult", () => D.RumbleMult);
			this.AttachDelegate("RumbleLeft", () => D.RumbleLeft);
			this.AttachDelegate("RumbleRight", () => D.RumbleRight);
			this.AttachDelegate("ABSPulse", () => D.ABSPulse);
			this.AttachDelegate("Airborne", () => D.Airborne);
			this.AttachDelegate("Gear", () => D.Gear);
			this.AttachDelegate("Gears", () => D.Gears);
			this.AttachDelegate("ShiftDown", () => D.Downshift);
			this.AttachDelegate("ShiftUp", () => D.Upshift);
			this.AttachDelegate("WiperStatus", () => D.WiperStatus);
			this.AttachDelegate("AccHeave", () => D.AccHeave[D.Acc0]);
			this.AttachDelegate("AccSurge", () => D.AccSurge[D.Acc0]);
			this.AttachDelegate("AccSway", () => D.AccSway[D.Acc0]);
			this.AttachDelegate("AccHeave2", () => D.AccHeave2S);
			this.AttachDelegate("AccSurge2", () => D.AccSurge2S);
			this.AttachDelegate("AccSway2", () => D.AccSway2S);
			this.AttachDelegate("AccHeaveAvg", () => D.AccHeaveAvg);
			this.AttachDelegate("AccSurgeAvg", () => D.AccSurgeAvg);
			this.AttachDelegate("AccSwayAvg", () => D.AccSwayAvg);
			this.AttachDelegate("JerkZ", () => D.JerkZ);
			this.AttachDelegate("JerkY", () => D.JerkY);
			this.AttachDelegate("JerkX", () => D.JerkX);
			this.AttachDelegate("JerkYAvg", () => D.JerkYAvg);
			this.AttachDelegate("MPitch", () => D.MotionPitch);
			this.AttachDelegate("MRoll", () => D.MotionRoll);
			this.AttachDelegate("MYaw", () => D.MotionYaw);
			this.AttachDelegate("MHeave", () => D.MotionHeave);
			this.AttachDelegate("MSurge", () => D.MotionSurge);
			this.AttachDelegate("MSway", () => D.MotionSway);
			this.AttachDelegate("TireSamples", () => D.TireDiameterSampleCount);
			this.AttachDelegate("VelocityX", () => D.VelocityX);
			FrameTimeTicks = DateTime.Now.Ticks;
		}
	}
}
