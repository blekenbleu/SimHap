using GameReaderCommon;
using Newtonsoft.Json;
using sierses.Sim.Properties;
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

namespace sierses.Sim
{
	[PluginDescription("Properties for haptic feedback and more")]
	[PluginAuthor("sierses")]
	[PluginName("Haptics")]
	public class Haptics : IPlugin, IDataPlugin, IWPFSettingsV2 //, IWPFSettings
	{
		public string PluginVersion = FileVersionInfo.GetVersionInfo(
			Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
		public static int LoadFailCount;
		public static long FrameTimeTicks = 0;
		public static long FrameCountTicks = 0;
		public static GameId CurrentGame = GameId.Other;
		public static string GameDBText;
		internal static bool Loaded, Waiting, Save, Set, Changed;
		private static readonly HttpClient client = new();
		private readonly string myfile = $"PluginsData/{nameof(Haptics)}.{Environment.UserName}.json";
		//		private readonly string Atlasfile = $"PluginsData/{nameof(Haptics)}.Atlas.json";
		private readonly string Atlasfile = $"PluginsData/{nameof(Haptics)}.json_with_orders.json";
		internal static List<CarSpec> Atlas;
		internal static int AtlasCt;        // use this to force Atlas
		public Spec S { get; } = new() { };

		public SimData D { get; set; }

		//		public Geq E { get; set; } 

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

		// boilerplate SimHub -------------------------------------------
		public string LeftMenuTitle => "Haptics";

		internal SettingsControl SC;
		public Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			SC = new SettingsControl(this);
			if (null != Settings.Theme)
				SC.ChangeTheme(Settings.Theme);
			return SC;
		}

		public Settings Settings { get; set; }

		public PluginManager PluginManager { get; set; }
		// ----------------------------------------------------------------

		// must be void and static;  invoked by D.SetVehicle()
		private static Haptics This;

		internal static void FetchCarData(    // called from SetVehicle() switch
			string id,
			string category,
			ushort redlineFromGame,
			ushort maxRPMFromGame,
			ushort ushortIdleRPM)                           // FetchCarData() argument
		{
			Logging.Current.Info($"Haptics.FetchCarData({id}/{category}):  Index = {This.D.Index}," +
							   (Save ? " Save " : "") + (Loaded ? " Loaded " : "") + (Waiting ? " Waiting" : "")
								+ (Set ? " Set" : "") + (Changed ? "Changed " : ""));

			if (-2 == This.D.Index) // first time for this CarId change?  Also called for retries with Index = -1
			{
				This.S.Notes = "";
				Set = false;
				StatusDataBase db = This.Gdat.NewData;
				string sid = (GameId.Forza == CurrentGame && "" == db.CarId.Substring(0, 4))
							? db.CarId.Substring(4) : db.CarId;
                This.D.Index = This.S.SelectCar(sid, // set game RPM defaults
                                                    redlineFromGame, maxRPMFromGame, ushortIdleRPM);
                /*
								Logging.Current.Info($"Haptics.SelectCar({sid}): "
													+ (Save ? " Save " : "") + (Loaded ? " Loaded " : "")
													+ (Waiting ? " Waiting" : "") + (Set ? " Set": "")
													+ (Changed ? "Changed " : "") + $" Index = {This.D.Index}");
				 */
                if (0 <= This.D.Index)
					return;
			}
            // Index should be -1; non-negative values returned
            This.D.Index = -3;        // disable online database
            This.D.CarInitCount = 0;
            return;
#if server
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
					Waiting = false;    // ReadAsStringAsync() success
					Download dljc = JsonConvert.DeserializeObject<Download>(dls,
										new JsonSerializerSettings
										{
											NullValueHandling = NullValueHandling.Ignore,
											MissingMemberHandling = MissingMemberHandling.Ignore
										}
									);
					if (null != dljc && null != dljc.data && 0 < dljc.data.Count
					 && null != dljc.data[0].id && null != dljc.data[0].game && null != dljc.data[0].name)
					{
						CarSpec car = dljc.data[0];
						car.defaults = "DB";
						This.S.Cache(car);                  // FetchCarData(): Set(id) at the end of SetVehicle()
						LoadFailCount = 1;
						//	Logging.Current.Info($"Haptics.FetchCarData({car.name}): Successfully loaded; "
						//						+ $" CarInitCount = {This.D.CarInitCount}");
						This.D.CarInitCount = 0;
						This.D.Index = -4;
						return;
					}
				}
				else if (null != dls)
				{
					if (-1 == This.D.Index)         // delayed dls? things may have moved on...
						This.D.Index = -3;          // disable self until other code decides otherwise
					if (11 == dls.Length)
						Waiting = false;
					/*
											Logging.Current.Info($"Haptics.FetchCarData({id}): not in DB");
										else if (0 < dls.Length)
											Logging.Current.Info($"Haptics.FetchCarData({id}): JsonConvert fail;  length {dls.Length}: "
																+ dls.Substring(0, dls.Length > 20 ? 20 : dls.Length));
					 */
				}
				// else Waiting
			}
			catch (HttpRequestException ex) //  treat it like not in DB
			{
				Logging.Current.Error("Haptics.FetchCarData() Error: " + ex.Message);
				Waiting = false;
			}
#endif
		}       // FetchCarData()

		/// <summary>
		/// Called one time per game data update, contains all normalized game data.
		/// Raw data are intentionnally "hidden" under a generic object type (plugins SHOULD NOT USE)
		/// This method is on the critical path, must execute as fast as possible and avoid throwing any error
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including present and previous data frames.</param> 
		internal GameData Gdat;
		internal PluginManager PM;
		internal int On;
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			if (null == data.NewData)
			{
				On = 0;
				return;
			}

			Gdat = data;
			PM = pluginManager;

			if (S.Id == data.NewData.CarId || !D.Unlocked)              // DataUpdate()
			{
				if (CurrentGame != GameId.BeamNG && null != data.OldData && data.GameRunning
                    && 1 == (On = (int)PM.GetPropertyValue("DataCorePlugin.GameData.EngineIgnitionOn"))
                || (CurrentGame == GameId.BeamNG && null != data.OldData && data.GameRunning
                    && 1 == (On = (int)PM.GetPropertyValue("DataCorePlugin.GameRawData.ignitionOn"))))
					D.Refresh(ref data, this);
				return;
			}

			On = 0;

			if (Waiting)
			{
				if ((30 * LoadFailCount) > ++D.CarInitCount)
					return;

				Waiting = false;            // CarInitCount timeout
				if (4 > LoadFailCount++)
					return;                 // do not give up (yet)

				D.Index = -3;                      // disable FetchCarData(); enable Defaults()
				D.CarInitCount = 0;
				//	Logging.Current.Info($"Haptics.DataUpdate({data.NewData.CarId}/{S.Id}):  async Waiting timeout" +
				//						 (Save ? " Save" : "") + (Loaded ? " Loaded" : "")
				//						+ (Set ? " Set": "") + (Changed ? " Changed" : "" + $" Index = {D.Index}"));
				Changed = false;
			}
			else if (Loaded || Changed)       // save before SetVehicle()
			{
				if (null == S.Car.name)
					Logging.Current.Info($"Haptics.S.Add({S.Id}) : missing car name");
				else if (S.Add(S.Id))       // DataUpdate():  add or update S.Car in Cars list
					S.LD.Add(S.Car);        // DataUpdate()
				Loaded = false;
			}


			if (data.GameRunning || data.GamePaused || data.GameReplay || data.GameInMenu)
			{
				if (-2 == D.Index)
					Set = Changed = false;
				Logging.Current.Info($"Haptics.DataUpdate({data.NewData.CarId}/{S.Id}): "
									+ (Save ? " Save" : "") + (Loaded ? " Loaded" : "") + (Waiting ? " Waiting" : "")
									+ (Set ? " Set" : "") + (Changed ? " Changed" : "") + $" Index = {D.Index}");
				D.SetVehicle(this);
			}
		}   // DataUpdate()

		// Null0() remove these if 0 value before saving JSON
		private readonly List<string> zero = new() { "ehp", "idlerpm", "maxrpm", "redline" };
		private string Null0(string j)
		{
			for (int i = 0; i < zero.Count; i++)
				j = j.Replace($",\r\n	  \"{zero[i]}\": 0,", ",");
			return j;
		}

		public void End(PluginManager pluginManager)
		{
			Settings.Theme = SC.Theme;

			if (Save || Loaded || Changed)      // End()
			{
				if (Loaded || Changed)
					S.LD.Add(S.Car);            // End()
				string sjs = (null == S.LD) ? "" : Null0(S.LD.Jstring());   // delete 0 ushorts
				if (0 == sjs.Length || "{}" == sjs)
					Logging.Current.Info($"Haptics.End(): JSON Serializer failure: "
									+ $"{S.LD.Count} games, {S.Cars.Count} {S.Car.game} cars;  "
									+ (Save ? "changes made.." : "(no changes)"));
				else if (Save)
				{
					File.WriteAllText(myfile, sjs);
					Logging.Current.Info($"Haptics.End(): {S.LD.Count} games, including "
						+ $"{S.LD.CarCount(GameDBText)} {GameDBText} cars, written to " + myfile);
				}
			}

			// Remove default values from Settings per-game dictionaries
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

			// unconditionally save some
			Settings.SuspensionGamma["AllGames"] = D.SuspensionGammaAll;
			Settings.SuspensionMult["AllGames"] = D.SuspensionMultAll;
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
					break;
				case "AssettoCorsaCompetizione":
					CurrentGame = GameId.ACC;
					GameDBText = "ACC";
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
					D.GameAltText += (string)pm.GetPropertyValue(D.raw + "SessionData.WeekendInfo.Category");
					break;
				case "LMU":
					CurrentGame = GameId.LMU;
					GameDBText = "LMU";
					break;
				case "CodemastersDirtRally2":
					CurrentGame = GameId.DR2;
					GameDBText = "DR2";
					break;
				case "EAWRC23":
					CurrentGame = GameId.WRC23;
					GameDBText = "WRC23";
					D.AccSamples = 32;
					break;
				case "BeamNgDrive":
					CurrentGame = GameId.BeamNG;
					GameDBText = "BeamNG";
					break;
				default:
					CurrentGame = GameId.Other;
					break;
			}

			D.AccHeave = new double[D.AccSamples];
			D.AccSurge = new double[D.AccSamples];
			D.AccSway = new double[D.AccSamples];
		}   // SetGame()

		public void Init(PluginManager pluginManager)
		{
			This = this;                                // static pointer to current instance
			LoadFailCount = 1;
			D = new SimData();
			//			E = new();
			bool ShowFreq = true, ShowSusp = true, ShowPhysics = true;
			SetGame(pluginManager);

			Settings = this.ReadCommonSettings("Settings", () => new Settings());

			if (1 > Settings.DownshiftDurationMs)
				Settings.DownshiftDurationMs = 600;
			if (1 > Settings.UpshiftDurationMs)
				Settings.UpshiftDurationMs = 400;
			if (Settings.SuspensionMult == null)
				Settings.SuspensionMult = new Dictionary<string, double>();
			if (!Settings.SuspensionMult.TryGetValue("AllGames", out double num))
				Settings.SuspensionMult.Add("AllGames", 1.5);
			if (Settings.SuspensionGamma == null)
				Settings.SuspensionGamma = new Dictionary<string, double>();
			if (!Settings.SuspensionGamma.TryGetValue("AllGames", out num))
				Settings.SuspensionGamma.Add("AllGames", 1.75);

			string Atlasst = "";
			AtlasCt = -1;               // S.Extract() will attempt extracting List<CarSpec> Atlas from json
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
			else AtlasCt = 0;           // S.Extract() will attempt setting LD = json
			if (File.Exists(myfile))
			{
				string text = File.ReadAllText(myfile);
				Dictionary<string, List<CarSpec>> json =
					JsonConvert.DeserializeObject<Dictionary<string, List<CarSpec>>>(text);
				Logging.Current.Info("Haptics.Init():  " + S.LD.Set(json) + myfile + Atlasst);
			}
			else Logging.Current.Info("Haptics.Init():  " + myfile + " not found" + Atlasst);

			D.Init(Settings, this);
			//			E.Init(Settings.Engine, this);
			Save = Loaded = Waiting = Set = Changed = false;        // Init()
			this.AttachDelegate("CarName", () => S.CarName);
			this.AttachDelegate("CarId", () => S.Id);
			this.AttachDelegate("Category", () => S.Category);
			this.AttachDelegate("Defaults", () => S.Default);
			this.AttachDelegate("RedlineRPM", () => S.Redline);
			this.AttachDelegate("MaxRPM", () => S.MaxRPM);
			this.AttachDelegate("EngineConfig", () => S.EngineConfiguration);
			this.AttachDelegate("EngineCylinders", () => S.EngineCylinders);
			this.AttachDelegate("EngineLocation", () => S.EngineLocation);
			this.AttachDelegate("FiringOrder", () => S.FiringOrder);
			this.AttachDelegate("PoweredWheels", () => S.PoweredWheels);
			this.AttachDelegate("DisplacementCC", () => S.Displacement);
			this.AttachDelegate("PowerTotalHP", () => S.MaxPower);
			this.AttachDelegate("PowerEngineHP", () => S.MaxPower - S.ElectricMaxPower);
			this.AttachDelegate("PowerMotorHP", () => S.ElectricMaxPower);
			this.AttachDelegate("MaxTorqueNm", () => S.MaxTorque);
			this.AttachDelegate("IdleRPM", () => S.IdleRPM);            // Init()
			if (ShowFreq)
			{
				this.AttachDelegate("FreqHarmonic", () => D.FreqHarmonic);
				this.AttachDelegate("FreqLFEAdaptive", () => D.FreqLFEAdaptive);
                this.AttachDelegate("FreqLFEeq", () => D.LFEeq);
                //this.AttachDelegate("LFEhpScale", () => D.LFEhpScale);
                this.AttachDelegate("rpmMain", () => D.rpmMain);
                this.AttachDelegate("rpmPeakA2Rear", () => D.rpmPeakA2Rear);
                this.AttachDelegate("rpmPeakB1Rear", () => D.rpmPeakB1Rear);
                this.AttachDelegate("rpmPeakA1Rear", () => D.rpmPeakA1Rear);
                this.AttachDelegate("rpmPeakB2Rear", () => D.rpmPeakB2Rear);
                this.AttachDelegate("rpmPeakA2Front", () => D.rpmPeakA2Front);
                this.AttachDelegate("rpmPeakB1Front", () => D.rpmPeakB1Front);
                this.AttachDelegate("rpmPeakA1Front", () => D.rpmPeakA1Front);
                this.AttachDelegate("rpmPeakB2Front", () => D.rpmPeakB2Front);
                //this.AttachDelegate("peakEQ", () => D.peakEQ);
                this.AttachDelegate("rpmMainEQ", () => D.rpmMainEQ);
                //this.AttachDelegate("peakGearMulti", () => D.peakGearMulti);
                this.AttachDelegate("FreqPeakA1", () => D.FreqPeakA1);
				this.AttachDelegate("FreqPeakB1", () => D.FreqPeakB1);
				this.AttachDelegate("FreqPeakA2", () => D.FreqPeakA2);
				this.AttachDelegate("FreqPeakB2", () => D.FreqPeakB2);
				//this.AttachDelegate("Gain1H", () => D.Gain1H);
				//this.AttachDelegate("GainPeakA1Front", () => D.GainPeakA1Front);
				//this.AttachDelegate("GainPeakA1Rear", () => D.GainPeakA1Rear);
				//this.AttachDelegate("GainPeakA2Front", () => D.GainPeakA2Front);
				//this.AttachDelegate("GainPeakA2Rear", () => D.GainPeakA2Rear);
				//this.AttachDelegate("GainPeakB1Front", () => D.GainPeakB1Front);
				//this.AttachDelegate("GainPeakB1Rear", () => D.GainPeakB1Rear);
				//this.AttachDelegate("GainPeakB2Front", () => D.GainPeakB2Front);
				//this.AttachDelegate("GainPeakB2Rear", () => D.GainPeakB2Rear);
			}

			if (ShowSusp)
			{
				this.AttachDelegate("SuspensionFreq", () => D.SuspensionFreq);
				this.AttachDelegate("SuspensionFreqR1", () => D.SuspensionFreqR1);
				this.AttachDelegate("SuspensionFreqR2", () => D.SuspensionFreqR2);
				this.AttachDelegate("SuspensionFreqR3", () => D.SuspensionFreqR3);
				this.AttachDelegate("SuspensionMultR1", () => D.SuspensionMultR1);
				this.AttachDelegate("SuspensionMultR2", () => D.SuspensionMultR2);
				this.AttachDelegate("SuspensionMultR3", () => D.SuspensionMultR3);
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
				this.AttachDelegate("Gear", () => D.Gear);
				this.AttachDelegate("Gears", () => D.Gears);
				this.AttachDelegate("ShiftDown", () => D.Downshift);
				this.AttachDelegate("ShiftUp", () => D.Upshift);
				this.AttachDelegate("WiperStatus", () => D.WiperStatus);
				if (ShowPhysics)
				{
					this.AttachDelegate("Airborne", () => D.Airborne);
					this.AttachDelegate("YawRate", () => D.YawRate);
					this.AttachDelegate("YawRateAvg", () => D.YawRateAvg);
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
					this.AttachDelegate("Throttle", () => D.Accelerator);
					this.AttachDelegate("VelocityX", () => D.VelocityX);
				}
				FrameTimeTicks = DateTime.Now.Ticks;
			}
		}
	}
}
