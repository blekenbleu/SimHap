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
	[PluginDescription("Car-specific haptic properties")]
	[PluginAuthor("sierses")]
	[PluginName("Haptics")]
	public class Haptics : IPlugin, IDataPlugin, IWPFSettingsV2 //, IWPFSettings
	{
		static readonly string pname = nameof(Haptics);
		public string PluginVersion = FileVersionInfo.GetVersionInfo(
			Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
		public static int LoadFailCount;
		public static GameId CurrentGame = GameId.Other;
		public string GameDBText;
		internal bool Loaded, Waiting, Save, Set, Changed;
		internal string CarId;		// exactly match data.NewData.CarId for DataUpdate()
		private readonly string myfile = $"PluginsData/{pname}.{Environment.UserName}.json";
//		private readonly string Atlasfile = $"PluginsData/{pname}.Atlas.json";
		private readonly string Atlasfile = $"PluginsData/{pname}.Atlas_with_orders.json";
		private static List<CarSpec> Atlas = new();
		public Spec S { get; } = new() { };

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

		// boilerplate SimHub -------------------------------------------
		public string LeftMenuTitle => pname;
		public PluginManager PluginManager { get; set; }

		internal SettingsControl SC;
		public Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			SC = new SettingsControl(this);
			if (null != Settings.Theme)
				SC.ChangeTheme(Settings.Theme);
			return SC;
		}

		public Settings Settings { get; set; }

		// ----------------------------------------------------------------

		private static Haptics H;
		internal static void FetchCarData(			// called from SetCar() switch
			string category,
			ushort redlineFromGame,
			ushort maxRPMFromGame,
			ushort ushortIdleRPM)							// FetchCarData() argument
		{
			Logging.Current.Info(pname + $".FetchCarData({category}):  Index = {H.D.Index}," +
							   (H.Save ? " Save " : "") + (H.Loaded ? " Loaded " : "") + (H.Waiting ? " Waiting" : "")
								+ (H.Set ? " Set": "") + (H.Changed ? "Changed " : ""));

			if (-2 == H.D.Index)	// first time for this CarId change?  Also called for retries with Index = -1
			{
				H.S.Notes = "";
				H.Set = false;
				H.D.Index = H.S.SelectCar(H, Atlas, redlineFromGame, maxRPMFromGame, ushortIdleRPM);	// pass game defaults
/*
				Logging.Current.Info(pname + ".SelectCar(): "
									+ (Save ? " Save " : "") + (Loaded ? " Loaded " : "")
									+ (Waiting ? " Waiting" : "") + (Set ? " Set": "")
									+ (Changed ? "Changed " : "") + $" Index = {H.D.Index}");
 */
				if (0 <= H.D.Index)
					return;
			}
		}	   // FetchCarData()

		/// <summary>
		/// Called one time per game data update, contains all normalized game data.
		/// Raw data are intentionnally "hidden" under a generic object type (plugins SHOULD NOT USE)
		/// This method is on the critical path, must execute as fast as possible and avoid throwing any error
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including present and previous data frames.</param> 
		internal GameData Gdat;
		internal int On;
		internal StatusDataBase N;
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			if (null == (N = data.NewData))
			{
				On = 0;
				return;
			}

			Gdat = data;									// for OldData

			if (CarId == N.CarId || D.Locked)				// DataUpdate()
			{
				if (null != data.OldData && data.GameRunning
					&& 1 == (On = (int)pluginManager.GetPropertyValue("DataCorePlugin.GameData.EngineIgnitionOn")))
					D.Runtime(this, pluginManager);
				return;
			}

			On = 0;

			if (Waiting)		// FetchCarData() has queried online data
			{
				if ((30 * LoadFailCount) > ++D.CarInitCount)
					return;

				H.Waiting = false;			// CarInitCount timeout
				if (4 > LoadFailCount++)
					return;					// do not give up (yet)

				D.Index = -3;				// disable FetchCarData(); enable Defaults()
				D.CarInitCount = 0;
			//	Logging.Current.Info(pname + $".DataUpdate({N.CarId}/{S.Id}):  async Waiting timeout" +
			//						 (Save ? " Save" : "") + (Loaded ? " Loaded" : "")
			//						+ (Set ? " Set": "") + (Changed ? " Changed" : "" + $" Index = {D.Index}"));
				Changed = false;
			}
			else if (Loaded || Changed)		// save before SetCar()
			{
				if (null == S.Car.name)
					Logging.Current.Info(pname + $".S.SaveCar(): {CarId} missing car name");
				else S.SaveCar(pname);		// DataUpdate():  add or update changed S.Car in Cars list;  Loaded = false
				Loaded = false;
			}

			if (data.GameRunning || data.GamePaused || data.GameReplay || data.GameInMenu)
			{
				if (-2 == D.Index)
					Set = Changed = false;
				Logging.Current.Info(pname + $".DataUpdate({N.CarId}/{S.Id}): "
									+ (Save ? " Save" : "") + (Loaded ? " Loaded" : "") + (Waiting ? " Waiting" : "")
									+ (Set ? " Set": "") + (Changed ? " Changed" : "") + $" Index = {D.Index}");
				Logging.Current.Info(pname + ".pluginManager.GetPropertyValue(\"JSONio.JSONio.Gscale\") = "
						 + (string)pluginManager.GetPropertyValue("JSONio.JSONio.Gscale"));
				D.SetCar(pluginManager);
			}
		}	// DataUpdate()

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
			if (Save || Loaded || Changed)		// End()
			{
				if (Loaded || Changed)
					S.LD.AddCar(S.Car);			// End()
				string sjs = (null == S.LD) ? "" : Null0(S.LD.Jstring());	// delete 0 ushorts
				if (0 == sjs.Length || "{}" == sjs)
					Logging.Current.Info(pname + ".End(): JSON Serializer failure: "
									+ $"{S.LD.Count} games, {S.Cars.Count} {S.Car.game} cars;  "
									+ (Save ? "changes made.." : "(no changes)"));
				else if (Save)
				{
					File.WriteAllText(myfile, sjs);
					Logging.Current.Info(pname + $".End(): {S.LD.Count} games, including "
						+ $"{S.LD.CarCount(GameDBText)} {GameDBText} cars, written to " + myfile);
				}
			}

			// Remove default values from Settings per-game dictionaries
			if (Settings.SuspensionMult.ContainsKey(GameDBText))
			{
				if (D.SuspensionMult == 1.0)
					Settings.SuspensionMult.Remove(GameDBText);
				else Settings.SuspensionMult[GameDBText] = D.SuspensionMult;
			}
			else if (D.SuspensionMult != 1.0)
				Settings.SuspensionMult.Add(GameDBText, D.SuspensionMult);
			if (Settings.SuspensionGamma.ContainsKey(GameDBText))
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
		void SetGame(PluginManager pm)
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
				case "RRRE":
					CurrentGame = GameId.RRRE;
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
			string Atlasst = "";

			H = this;								// static pointer to current instance
			LoadFailCount = 1;
			D = new SimData();
			bool ShowFreq = true, ShowSusp = true, ShowPhysics = true;
			SetGame(pluginManager);
			Settings = this.ReadCommonSettings("Settings", () => new Settings());
			if (1 > Settings.DownshiftDurationMs)
				Settings.DownshiftDurationMs = 600;
			if (1 > Settings.UpshiftDurationMs)
				Settings.UpshiftDurationMs = 400;
			if (Settings.SuspensionMult == null)
				Settings.SuspensionMult = new Dictionary<string, double>();
			if (!Settings.SuspensionMult.ContainsKey("AllGames"))
				Settings.SuspensionMult.Add("AllGames", 1.5);
			if (Settings.SuspensionGamma == null)
				Settings.SuspensionGamma = new Dictionary<string, double>();
			if (!Settings.SuspensionGamma.ContainsKey("AllGames"))
				Settings.SuspensionGamma.Add("AllGames", 1.75);

			if (File.Exists(Atlasfile))
			{
				Dictionary<string, List<CarSpec>> JsonDict =
					JsonConvert.DeserializeObject<Dictionary<string, List<CarSpec>>>(File.ReadAllText(Atlasfile));
				if (null == JsonDict)
					Logging.Current.Info(pname + ".Init(): Atlas load failure");
				else if (JsonDict.ContainsKey(GameDBText))
				{
					Atlas = JsonDict[GameDBText];
					Atlasst = $";  {Atlas.Count} cars in Atlas";
				}
				else Logging.Current.Info(pname + $".Init():  {GameDBText} not in Atlas");
			}
			else Logging.Current.Info(pname + $".Init():  no Atlas");

			if (File.Exists(myfile))
			{
				string text = File.ReadAllText(myfile);
				Dictionary<string, List<CarSpec>> json =
					JsonConvert.DeserializeObject<Dictionary<string, List<CarSpec>>>(text);
				Logging.Current.Info(pname + ".Init():  " + S.LD.SetGame(this, json) + myfile + Atlasst);
			}
			else Logging.Current.Info(pname + ".Init():  " + myfile + " not found" + Atlasst);

			D.Init(this);
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
			this.AttachDelegate("FiringOrder", () => S.FiringOrder);
			this.AttachDelegate("PoweredWheels", () => S.PoweredWheels);
			this.AttachDelegate("DisplacementCC", () => S.Displacement);
			this.AttachDelegate("PowerTotalHP", () => S.MaxPower);
			this.AttachDelegate("PowerEngineHP", () => S.MaxPower - S.ElectricMaxPower);
			this.AttachDelegate("PowerMotorHP", () => S.ElectricMaxPower);
			this.AttachDelegate("MaxTorqueNm", () => S.MaxTorque);
			this.AttachDelegate("IdleRPM", () => S.IdleRPM);			// Init()
			if (ShowFreq)
			{
				this.AttachDelegate("FreqHarmonic", () => D.FreqHarmonic);
				this.AttachDelegate("FreqLFEAdaptive", () => D.FreqLFEAdaptive);
				this.AttachDelegate("FreqPeakB1", () => D.FreqPeakB1);
				this.AttachDelegate("FreqPeakA2", () => D.FreqPeakA2);
				this.AttachDelegate("FreqPeakB2", () => D.FreqPeakB2);
				this.AttachDelegate("FreqLFEeq", () => D.LFEeq);
				this.AttachDelegate("rpmMain", () => D.rpmMain);

				this.AttachDelegate("rpmPeakA2Rear", () => D.rpmPeakA2Rear);
				this.AttachDelegate("rpmPeakB1Rear", () => D.rpmPeakB1Rear);
				this.AttachDelegate("rpmPeakA1Rear", () => D.rpmPeakA1Rear);
				this.AttachDelegate("rpmPeakB2Rear", () => D.rpmPeakB2Rear);
				this.AttachDelegate("rpmPeakA2Front", () => D.rpmPeakA2Front);
				this.AttachDelegate("rpmPeakB1Front", () => D.rpmPeakB1Front);
				this.AttachDelegate("rpmPeakA1Front", () => D.rpmPeakA1Front);
				this.AttachDelegate("rpmPeakB2Front", () => D.rpmPeakB2Front);
				this.AttachDelegate("rpmMainEQ", () => D.rpmMainEQ);
				this.AttachDelegate("FreqPeakA1", () => D.FreqPeakA1);
				this.AttachDelegate("LFEhpScale", () => D.LFEhpScale);
				this.AttachDelegate("peakEQ", () => D.peakEQ);
				this.AttachDelegate("peakGearMulti", () => D.peakGearMulti);
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
					this.AttachDelegate("impactsR1Sway", () => D.ImpactsR1Sway());
					this.AttachDelegate("impactsAccSway2S", () => D.BipolarIIR(D.AccSway2S, D.impactsAccSway2S, 1, 15, 0.1));
					this.AttachDelegate("ThrottleIIR", () => D.ThrottleIIR());
				}
			}
		}	// Init()
	}
}
