// Decompiled with JetBrains decompiler
// Type: SimHaptics.SimHapticsPlugin
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267

using GameReaderCommon;
using Newtonsoft.Json;
using sierses.SimHap.Properties;
using SimHub;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace sierses.SimHap
{
	[PluginDescription("Properties for haptic feedback and more")]
	[PluginAuthor("sierses")]
	[PluginName("SimHap")]
	public class SimHap : IPlugin, IDataPlugin, IWPFSettingsV2, IWPFSettings
	{
		public string PluginVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
		public static int LoadFailCount;
		public static bool LoadFinish;
		public static bool Save, Changed;
		public static DataStatus LoadStatus;
		public static APIStatus FetchStatus;
		public static long FrameTimeTicks = 0;
		public static long FrameCountTicks = 0;
		public static GameId CurrentGame = GameId.Other;
		public static string GameDBText;
		public static string FailedId = "";
		public static string FailedCategory = "";
		private static readonly HttpClient client = new();
		private readonly string myfile = "PluginsData/blekenbleu.SimHap.json";

		public Spec S { get; set; }

		public SimData D { get; set; }

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
				new string[] { "name", data.name },
				new string[] { "id", data.id },
				new string[] { "game", data.game }
			};
		}
 */

		// boilerplate SimHub -------------------------------------------
		public string LeftMenuTitle => "SimHap";

		public Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			return new SettingsControl(this);
		}

		public Settings Settings { get; set; }

		public PluginManager PluginManager { get; set; }
		// ----------------------------------------------------------------

		// called by SetVehicle() in DataUpdate() when car not found
		internal void SetDefaultVehicle(StatusDataBase db)
		{
			string status = S.Defaults(GameDBText, db, CurrentGame);
			if (0 < status.Length)
				Logging.Current.Info($"SimHap.SetDefaultVehicle({CurrentGame}, "
					+ $"{db.CarModel}) {FetchStatus} {LoadStatus}:  "
                    + (D.LoadText = status));
            FetchStatus = APIStatus.None;
            if (CurrentGame == GameId.RRRE || CurrentGame == GameId.D4 || CurrentGame == GameId.DR2)
				S.Id = db.CarModel;
		}

		// returning true forces false from Wait(),
		// false then allows Refresh() or SetVehicle()
		internal bool Fail(string id, string fcat)
		{
			Logging.Current.Info("SimHap.FetchCarData(): Failed to load " + id + " : " + fcat);
			if (3 < LoadFailCount++)
			{
				LoadFailCount = 0;
				FailedId = CurrentGame != GameId.Forza ? id : "Car_" + id;
				FailedCategory = fcat;
				FetchStatus = APIStatus.Fail;
				return true;
			}
			FetchStatus = APIStatus.Retry;
			return false;					// do not give up (yet)
		}

		// returning true precludes Refresh() or SetVehicle(), i.e. just wait
		internal bool Wait(StatusDataBase db, ref string cat, ref string id)
		{
			if (FetchStatus == APIStatus.Waiting && 20 > D.CarInitCount++)
				return true;
			D.CarInitCount = 0;
			GameId g = CurrentGame;
			cat = (g == GameId.RF2 || g == GameId.LMU || g == GameId.AMS1)  ? db.CarClass : "0";
			id = g == GameId.RRRE ? db.CarModel : g == GameId.Forza ? db.CarId.Substring(4) : db.CarId;
			if (FetchStatus != APIStatus.None && FetchStatus != APIStatus.Success)
				Logging.Current.Info($"SimHap.Wait({cat}, {g}.{id}):  {FetchStatus} {LoadStatus}");
			return (FetchStatus == APIStatus.Waiting && !Fail(id, cat));
		}

		// must be void and static;  invoked by D.SetVehicle()
		static Download dljc;
		internal static async void FetchCarData(
//			SimData SD,
			string id,
			string category,
			Spec v,
			double doubleRedline,
			double doubleMaxRPM)
		{
            Logging.Current.Info($"SimHap.FetchCarData({id}/{category}):  {FetchStatus} {LoadStatus}");
            if (DataStatus.NotAPI == LoadStatus || APIStatus.Fail == FetchStatus)
				return;
			try
			{
				FetchStatus = APIStatus.Waiting;
				LoadFinish = false;
				id ??= "0";
				category ??= "0";
				Uri requestUri = new("https://api.simhaptics.com/data/" + GameDBText
								 + "/" + Uri.EscapeDataString(id) + "/" + Uri.EscapeDataString(category));
				HttpResponseMessage async = await client.GetAsync(requestUri);
				async.EnsureSuccessStatusCode();
				string dls = async.Content.ReadAsStringAsync().Result;
				dljc = JsonConvert.DeserializeObject<Download>(dls, new JsonSerializerSettings
								{
									NullValueHandling = NullValueHandling.Ignore,
									MissingMemberHandling = MissingMemberHandling.Ignore
								});
				if (v.Set(dljc, Convert.ToUInt16(0.5 + doubleRedline), Convert.ToUInt16(0.5 + doubleMaxRPM)))
				{
					Logging.Current.Info("SimHap.FetchCarData(): Successfully loaded " + v.CarName);
					LoadFinish = false;
					LoadFailCount = 0;

					// FetchCarData() seeminly does not return to SetVehicle()
					// before NEXT CarId change.
					//  Consequently, call SetVehicle() now!
					LoadStatus = DataStatus.NotAPI;	// new status to avoid looping here
					FetchStatus = APIStatus.Loaded;
//					SD.SetVehicle(null);			// null avoids attempting JSON lookups
				}
//				else SD.SHP.Fail(id, category);
			}
			catch (HttpRequestException ex)
			{
				Logging.Current.Error("SimHap: " + ex.Message);
				LoadFailCount = 0;
				FetchStatus = APIStatus.Retry;
			}
		}		// FetchCarData()

		/// <summary>
		/// Called one time per game data update, contains all normalized game data,
		/// raw data are intentionnally "hidden" under a generic object type (plugins SHOULD NOT USE)
		/// This method is on the critical path, must execute as fast as possible and avoid throwing any error
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including present and previous data frames.</param> 
		internal GameData Gdat;
		internal PluginManager PM;
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			string cat = "", id = "";

			if (null == data.NewData || Wait(data.NewData, ref cat, ref id))
				return;

			Gdat = data;
			PM = pluginManager;

			if (S.Id == data.NewData.CarId && LoadStatus != DataStatus.NotAPI)
			{
				if (data.GameRunning && null != data.OldData)
					D.Refresh(ref data, this);
			}
			else if (Settings.Unlocked && (data.GameRunning || data.GamePaused || data.GameReplay || data.GameInMenu))
			{
				if (Changed && APIStatus.Success == FetchStatus)	// don't save for APIStatus.None
					S.Add(S.Car);
				D.SetVehicle(this);
			}
		}

		// remove 0 value ushorts
		private readonly List<string> zero = new() { "ehp", "idlerpm", "maxrpm", "redline" };
		private string Null0(string j)
		{
			for (int i = 0; i < zero.Count; i++)
				j = j.Replace($",\r\n      \"{zero[i]}\": 0,", ",");
			return j;
		}

		public void End(PluginManager pluginManager)
		{
			if (FetchStatus == APIStatus.Success)
			{
				Settings.Vehicle = new Spec(S);
				Save = S.LD.Add(S.Car) || Save;			// update game in dictionary
			}
			string sjs = Null0(S.Jstring());			// delete 0 ushorts
			if (0 == sjs.Length || "{}" == sjs)
				Logging.Current.Info("SimHap.End(): Download Json Serializer failure:  "
									 + (Save ? "changes made.." : "(no changes)"));
			else if (Save)
				File.WriteAllText(myfile, sjs);

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
			IPluginExtensions.SaveCommonSettings(this, "Settings", Settings);
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
					D.GameAltText += (string) pm.GetPropertyValue("DataCorePlugin.GameRawData.SessionData.WeekendInfo.Category");
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
			LoadFailCount = 0;
			Save = Changed = LoadFinish = false;
			LoadStatus = DataStatus.None;
			FetchStatus = APIStatus.None;
			S = new Spec();
			D = new SimData();
			S.Init(/*this*/);
			SetGame(pluginManager);
			Settings = IPluginExtensions.ReadCommonSettings(this, "Settings", () => new Settings());
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
			if (File.Exists(myfile))
			{
				 var foo = JsonConvert.DeserializeObject<Dictionary<string, List<CarSpec>>>(File.ReadAllText(myfile));
				if (null != foo && 0 < foo.Count && S.LD.Load(foo))
				{
					S.LD.Extract(GameDBText);	// to S.Lcars
					Logging.Current.Info($"SimHap.Init():  {S.LD.Count} games in " + myfile
									   + $", with {S.Lcars.Count} {GameDBText} cars");
				}
				else Logging.Current.Info("SimHap.Init(): "+myfile+" load failure");
			}
			else Logging.Current.Info("SimHap.Init():  "+myfile+" not found"); 
			D.Init(Settings, this);
			IPluginExtensions.AttachDelegate(this, "CarName", () => S.CarName);
			IPluginExtensions.AttachDelegate(this, "CarId", () => S.Id);
			IPluginExtensions.AttachDelegate(this, "Category", () => S.Category);
			IPluginExtensions.AttachDelegate(this, "RedlineRPM", () => S.Redline);
			IPluginExtensions.AttachDelegate(this, "MaxRPM", () => S.MaxRPM);
			IPluginExtensions.AttachDelegate(this, "EngineConfig", () => S.EngineConfiguration);
			IPluginExtensions.AttachDelegate(this, "EngineCylinders", () => S.EngineCylinders);
			IPluginExtensions.AttachDelegate(this, "EngineLocation", () => S.EngineLocation);
			IPluginExtensions.AttachDelegate(this, "PoweredWheels", () => S.PoweredWheels);
			IPluginExtensions.AttachDelegate(this, "DisplacementCC", () => S.Displacement);
			IPluginExtensions.AttachDelegate(this, "PowerTotalHP", () => S.MaxPower);
			IPluginExtensions.AttachDelegate(this, "PowerEngineHP", () => S.MaxPower - S.ElectricMaxPower);
			IPluginExtensions.AttachDelegate(this, "PowerMotorHP", () => S.ElectricMaxPower);
			IPluginExtensions.AttachDelegate(this, "MaxTorqueNm", () => S.MaxTorque);
			IPluginExtensions.AttachDelegate(this, "LoadStatus", () => (int)LoadStatus);
			IPluginExtensions.AttachDelegate(this, "EngineLoad", () => D.EngineLoad);
			IPluginExtensions.AttachDelegate(this, "IdleRPM", () => S.IdleRPM);
			IPluginExtensions.AttachDelegate(this, "FreqHarmonic", () => D.FreqHarmonic);
			IPluginExtensions.AttachDelegate(this, "FreqOctave", () => D.FreqOctave);
			IPluginExtensions.AttachDelegate(this, "FreqIntervalA1", () => D.FreqIntervalA1);
			IPluginExtensions.AttachDelegate(this, "FreqIntervalA2", () => D.FreqIntervalA2);
			IPluginExtensions.AttachDelegate(this, "FreqLFEAdaptive", () => D.FreqLFEAdaptive);
			IPluginExtensions.AttachDelegate(this, "FreqPeakA1", () => D.FreqPeakA1);
			IPluginExtensions.AttachDelegate(this, "FreqPeakB1", () => D.FreqPeakB1);
			IPluginExtensions.AttachDelegate(this, "FreqPeakA2", () => D.FreqPeakA2);
			IPluginExtensions.AttachDelegate(this, "FreqPeakB2", () => D.FreqPeakB2);
			IPluginExtensions.AttachDelegate(this, "Gain1H", () => D.Gain1H);
			IPluginExtensions.AttachDelegate(this, "Gain1H2", () => D.Gain1H2);
			IPluginExtensions.AttachDelegate(this, "Gain2H", () => D.Gain2H);
			IPluginExtensions.AttachDelegate(this, "Gain4H", () => D.Gain4H);
			IPluginExtensions.AttachDelegate(this, "GainOctave", () => D.GainOctave);
			IPluginExtensions.AttachDelegate(this, "GainIntervalA1", () => D.GainIntervalA1);
			IPluginExtensions.AttachDelegate(this, "GainIntervalA2", () => D.GainIntervalA2);
			IPluginExtensions.AttachDelegate(this, "GainPeakA1Front", () => D.GainPeakA1Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakA1Middle", () => D.GainPeakA1);
			IPluginExtensions.AttachDelegate(this, "GainPeakA1Rear", () => D.GainPeakA1Rear);
			IPluginExtensions.AttachDelegate(this, "GainPeakA2Front", () => D.GainPeakA2Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakA2Middle", () => D.GainPeakA2);
			IPluginExtensions.AttachDelegate(this, "GainPeakA2Rear", () => D.GainPeakA2Rear);
			IPluginExtensions.AttachDelegate(this, "GainPeakB1Front", () => D.GainPeakB1Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakB1Middle", () => D.GainPeakB1);
			IPluginExtensions.AttachDelegate(this, "GainPeakB1Rear", () => D.GainPeakB1Rear);
			IPluginExtensions.AttachDelegate(this, "GainPeakB2Front", () => D.GainPeakB2Front);
			IPluginExtensions.AttachDelegate(this, "GainPeakB2Middle", () => D.GainPeakB2);
			IPluginExtensions.AttachDelegate(this, "GainPeakB2Rear", () => D.GainPeakB2Rear);
			IPluginExtensions.AttachDelegate(this, "SlipXFL", () => D.SlipXFL);
			IPluginExtensions.AttachDelegate(this, "SlipXFR", () => D.SlipXFR);
			IPluginExtensions.AttachDelegate(this, "SlipXRL", () => D.SlipXRL);
			IPluginExtensions.AttachDelegate(this, "SlipXRR", () => D.SlipXRR);
			IPluginExtensions.AttachDelegate(this, "SlipXAll", () => D.SlipXAll);
			IPluginExtensions.AttachDelegate(this, "SlipYFL", () => D.SlipYFL);
			IPluginExtensions.AttachDelegate(this, "SlipYFR", () => D.SlipYFR);
			IPluginExtensions.AttachDelegate(this, "SlipYRL", () => D.SlipYRL);
			IPluginExtensions.AttachDelegate(this, "SlipYRR", () => D.SlipYRR);
			IPluginExtensions.AttachDelegate(this, "SlipYAll", () => D.SlipYAll);
			IPluginExtensions.AttachDelegate(this, "WheelLockAll", () => D.WheelLockAll);
			IPluginExtensions.AttachDelegate(this, "WheelSpinAll", () => D.WheelSpinAll);
			IPluginExtensions.AttachDelegate(this, "TireDiameterFL", () => D.TireDiameterFL);
			IPluginExtensions.AttachDelegate(this, "TireDiameterFR", () => D.TireDiameterFR);
			IPluginExtensions.AttachDelegate(this, "TireDiameterRL", () => D.TireDiameterRL);
			IPluginExtensions.AttachDelegate(this, "TireDiameterRR", () => D.TireDiameterRR);
			IPluginExtensions.AttachDelegate(this, "TireSpeedFL", () => D.WheelSpeedFL);
			IPluginExtensions.AttachDelegate(this, "TireSpeedFR", () => D.WheelSpeedFR);
			IPluginExtensions.AttachDelegate(this, "TireSpeedRL", () => D.WheelSpeedRL);
			IPluginExtensions.AttachDelegate(this, "TireSpeedRR", () => D.WheelSpeedRR);
			IPluginExtensions.AttachDelegate(this, "SpeedMs", () => D.SpeedMs);
			IPluginExtensions.AttachDelegate(this, "TireLoadFL", () => D.WheelLoadFL);
			IPluginExtensions.AttachDelegate(this, "TireLoadFR", () => D.WheelLoadFR);
			IPluginExtensions.AttachDelegate(this, "TireLoadRL", () => D.WheelLoadRL);
			IPluginExtensions.AttachDelegate(this, "TireLoadRR", () => D.WheelLoadRR);
			IPluginExtensions.AttachDelegate(this, "YawRate", () => D.YawRate);
			IPluginExtensions.AttachDelegate(this, "YawRateAvg", () => D.YawRateAvg);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreq", () => D.SuspensionFreq);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR0a", () => D.SuspensionFreqRa);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR0b", () => D.SuspensionFreqRb);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR0c", () => D.SuspensionFreqRc);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR1", () => D.SuspensionFreqR1);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR2", () => D.SuspensionFreqR2);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR3", () => D.SuspensionFreqR3);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR4", () => D.SuspensionFreqR4);
			IPluginExtensions.AttachDelegate(this, "SuspensionFreqR5", () => D.SuspensionFreqR5);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR0a", () => D.SuspensionMultRa);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR0b", () => D.SuspensionMultRb);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR0c", () => D.SuspensionMultRc);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR1", () => D.SuspensionMultR1);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR2", () => D.SuspensionMultR2);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR3", () => D.SuspensionMultR3);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR4", () => D.SuspensionMultR4);
			IPluginExtensions.AttachDelegate(this, "SuspensionMultR5", () => D.SuspensionMultR5);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR0b", () => D.SuspensionRumbleMultRb);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR0c", () => D.SuspensionRumbleMultRc);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR1", () => D.SuspensionRumbleMultR1);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR2", () => D.SuspensionRumbleMultR2);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR3", () => D.SuspensionRumbleMultR3);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR4", () => D.SuspensionRumbleMultR4);
			IPluginExtensions.AttachDelegate(this, "SuspensionRumbleMultR5", () => D.SuspensionRumbleMultR5);
			IPluginExtensions.AttachDelegate(this, "SuspensionFL", () => D.SuspensionFL);
			IPluginExtensions.AttachDelegate(this, "SuspensionFR", () => D.SuspensionFR);
			IPluginExtensions.AttachDelegate(this, "SuspensionRL", () => D.SuspensionRL);
			IPluginExtensions.AttachDelegate(this, "SuspensionRR", () => D.SuspensionRR);
			IPluginExtensions.AttachDelegate(this, "SuspensionFront", () => D.SuspensionFront);
			IPluginExtensions.AttachDelegate(this, "SuspensionRear", () => D.SuspensionRear);
			IPluginExtensions.AttachDelegate(this, "SuspensionLeft", () => D.SuspensionLeft);
			IPluginExtensions.AttachDelegate(this, "SuspensionRight", () => D.SuspensionRight);
			IPluginExtensions.AttachDelegate(this, "SuspensionAll", () => D.SuspensionAll);
			IPluginExtensions.AttachDelegate(this, "SuspensionAccAll", () => D.SuspensionAccAll);
			IPluginExtensions.AttachDelegate(this, "RumbleFromPlugin", () => D.RumbleFromPlugin);
			IPluginExtensions.AttachDelegate(this, "RumbleMult", () => D.RumbleMult);
			IPluginExtensions.AttachDelegate(this, "RumbleLeft", () => D.RumbleLeft);
			IPluginExtensions.AttachDelegate(this, "RumbleRight", () => D.RumbleRight);
			IPluginExtensions.AttachDelegate(this, "ABSPulse", () => D.ABSPulse);
			IPluginExtensions.AttachDelegate(this, "Airborne", () => D.Airborne);
			IPluginExtensions.AttachDelegate(this, "Gear", () => D.Gear);
			IPluginExtensions.AttachDelegate(this, "Gears", () => D.Gears);
			IPluginExtensions.AttachDelegate(this, "ShiftDown", () => D.Downshift);
			IPluginExtensions.AttachDelegate(this, "ShiftUp", () => D.Upshift);
			IPluginExtensions.AttachDelegate(this, "WiperStatus", () => D.WiperStatus);
			IPluginExtensions.AttachDelegate(this, "AccHeave", () => D.AccHeave[D.Acc0]);
			IPluginExtensions.AttachDelegate(this, "AccSurge", () => D.AccSurge[D.Acc0]);
			IPluginExtensions.AttachDelegate(this, "AccSway", () => D.AccSway[D.Acc0]);
			IPluginExtensions.AttachDelegate(this, "AccHeave2", () => D.AccHeave2S);
			IPluginExtensions.AttachDelegate(this, "AccSurge2", () => D.AccSurge2S);
			IPluginExtensions.AttachDelegate(this, "AccSway2", () => D.AccSway2S);
			IPluginExtensions.AttachDelegate(this, "AccHeaveAvg", () => D.AccHeaveAvg);
			IPluginExtensions.AttachDelegate(this, "AccSurgeAvg", () => D.AccSurgeAvg);
			IPluginExtensions.AttachDelegate(this, "AccSwayAvg", () => D.AccSwayAvg);
			IPluginExtensions.AttachDelegate(this, "JerkZ", () => D.JerkZ);
			IPluginExtensions.AttachDelegate(this, "JerkY", () => D.JerkY);
			IPluginExtensions.AttachDelegate(this, "JerkX", () => D.JerkX);
			IPluginExtensions.AttachDelegate(this, "JerkYAvg", () => D.JerkYAvg);
			IPluginExtensions.AttachDelegate(this, "MPitch", () => D.MotionPitch);
			IPluginExtensions.AttachDelegate(this, "MRoll", () => D.MotionRoll);
			IPluginExtensions.AttachDelegate(this, "MYaw", () => D.MotionYaw);
			IPluginExtensions.AttachDelegate(this, "MHeave", () => D.MotionHeave);
			IPluginExtensions.AttachDelegate(this, "MSurge", () => D.MotionSurge);
			IPluginExtensions.AttachDelegate(this, "MSway", () => D.MotionSway);
			IPluginExtensions.AttachDelegate(this, "TireSamples", () => D.TireDiameterSampleCount);
			IPluginExtensions.AttachDelegate(this, "VelocityX", () => D.VelocityX);
			FrameTimeTicks = DateTime.Now.Ticks;
		}
	}
}
