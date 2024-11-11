using GameReaderCommon;
using Newtonsoft.Json;
using blekenbleu.Haptic.Properties;
using SimHub;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using Atlas;

namespace blekenbleu.Haptic
{
	[PluginDescription("Car-specific haptic properties")]
	[PluginAuthor("blekenbleu")]
	[PluginName("BlekHapt")]
	public partial class BlekHapt : IPlugin, IDataPlugin, IWPFSettingsV2 //, IWPFSettings
	{
		internal static readonly string pname = "BlekHapt";
		public string PluginVersion = FileVersionInfo.GetVersionInfo(
			Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
		public static int LoadFailCount;
		public static GameId CurrentGame = GameId.Other;
		public string GameDBText;
		internal bool Save, Set, Changed;
		internal string CarId;		// exactly match data.NewData.CarId for DataUpdate()
		// PluginsData/BlekHapt.bleke.json AC has bad CRX del Sol for testing
		internal readonly string myfile = $"PluginsData/{pname}.{Environment.UserName}.json";
		internal static List<CarSpec> Atlas = new();
		public Spec S { get; } = new() { };

		public SimData D { get; set; }
#if !slim
		public Geq E { get; set; } 
#endif
		public ImageSource PictureIcon => this.ToIcon(Resources.SimHapticsShakerStyleIcon_alt012);

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

		internal SettingsControl View;
		public Control GetWPFSettingsControl(PluginManager pluginManager)
		{
			View = new SettingsControl(this);
#if slim
			if (null != Settings.Theme)
				View.ChangeTheme(Settings.Theme);
#else
			if (null != Settings.Engine)
			{
				if (null != Settings.Engine.Theme)
					View.ChangeTheme(Settings.Engine.Theme);
				if (null != Settings.Engine.Tones)
				{
					if (0 < Settings.Engine.Tones.Length)
					{
						if (8 != Settings.Engine.Tones[0].Length)
							Logging.Current.Info(pname + ": Settings.Engine.Tones[0].Freq.Count = "
							+ Settings.Engine.Tones[0].Length.ToString());
					} else Logging.Current.Info(pname + ": zero Settings.Engine.Tones.Length");
				}
				else Logging.Current.Info(pname  + ":  null Settings.Engine.Tones");
				if (null == Settings.Engine.Sliders)
					Logging.Current.Info(pname + ":  null Settings.Engine.Sliders");
				else if (1 > Settings.Engine.Sliders.Count)
					Logging.Current.Info(pname + ": zero Settings.Engine.Sliders.Count");
				else if (9 != Settings.Engine.Sliders[0].Length)
					Logging.Current.Info(pname + ": Settings.Engine.Sliders[0].Length = "
					  + Settings.Engine.Sliders[0].Length.ToString());
			}
#endif
			return View;
		}

		public Settings Settings { get; set; }

		// ----------------------------------------------------------------
		internal					// called from SetCar() switch
		void FetchCarData(ushort redlineFromGame, ushort maxRPMFromGame, ushort ushortIdleRPM)
		{
/*
			Logging.Current.Info(pname + $".FetchCarData():  Index = {D.Index}," + (Save ? " Save " : "")
								+ (Set ? " Set": "") + (Changed ? "Changed " : ""));
 */
			if (-2 == D.Index)	// first time for this CarId change?  Also called for retries with Index = -1
			{
				S.Notes = "";
				Set = false;
				D.Index = S.SelectCar(Atlas, redlineFromGame, maxRPMFromGame, ushortIdleRPM);	// pass game defaults
				Changed = false;
			}
		}	   // FetchCarData()

		/// <summary>
		/// Called one time per game data update, contains all normalized game data.
		/// Raw data are intentionnally "hidden" under a generic object type (plugins Should NOT USE)
		/// This method is on the critical path, must execute as fast as possible and avoid throwing any error
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including present and previous data frames.</param> 
		internal GameData Gdat;
		internal int IgnOn;
		internal StatusDataBase N;
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			if (null == (N = data.NewData))
			{
				IgnOn = 0;
				return;
			}

			Gdat = data;									// for OldData

			if (CarId == N.CarId || D.Locked)				// DataUpdate()
			{
				if (null != data.OldData && data.GameRunning
					&& 1 == (IgnOn = (int)pluginManager.GetPropertyValue("DataCorePlugin.GameData.EngineIgnitionOn")))
					D.Runtime(pluginManager);
				return;
			}

			IgnOn = 0;

			if (Changed)		// save before SetCar()
			{
				if (null == S.Car.name)
					Logging.Current.Info($"\t{pname}.S.SaveCar(): {CarId} missing car name");
				else if (S.SaveCar())			// DataUpdate():  add or update changed S.Car in Cache
				{
				//	Logging.Current.Info($"\t{pname}.S.SaveCar(): updated {S.Car.id} Index = {S.CacheIndex}/{S.Cars.Count}");
				}
				else if (0 < S.CacheIndex)
					Logging.Current.Info($"\t{pname}.S.SaveCar():  {S.Car.id} makes {S.Cars.Count} {GameDBText} cars");
			}

			if (data.GameRunning || data.GamePaused || data.GameReplay || data.GameInMenu)
			{
				if (-2 == D.Index)
					Set = Changed = false;
				D.SetCar(pluginManager);
			}
		}	// DataUpdate()

		public void End(PluginManager pluginManager)
		{
#if slim
			Settings.Theme = View.Theme;
#else
			if (null == Settings.Engine)
				Settings.Engine = new();
			Settings.Engine.Theme = View.Theme;			// easier to stuff here
			if (null == Settings.Engine.Tones)
			{ 
				Settings.Engine.Tones = new ushort[4][];
				Settings.Engine.Tones[0] = new ushort[9]; // 2 fundamentals, 6 harmonics, 1 modulator
				Settings.Engine.Tones[1] = new ushort[9];
				Settings.Engine.Tones[2] = new ushort[9]; // second pair: full throttle
				Settings.Engine.Tones[3] = new ushort[9];
			}

			for (int i = 0; i < E.Tones.Length; i++)
				for (int j = 0; j < E.Tones[i].Freq.Length; j++)
					Settings.Engine.Tones[i][j] = E.Tones[i].Freq[j];
			Settings.Engine.Sliders = new() { E.Q[0].Slider };
			for (int i = 1; i < E.Q.Count; i++)
				Settings.Engine.Sliders.Add(E.Q[i].Slider);
#endif
			if (Save || Changed)		// End()
				Logging.Current.Info(pname + ".End(): " + S.End(myfile, GameDBText, this));

			// Remove default values from Settings per-game dictionaries
#if !slim
			if (Settings.EngineMult.ContainsKey("AllGames"))
				Settings.EngineMult.Remove("AllGames");
			if (Settings.EngineMult.ContainsKey(GameDBText))
			{
				if (D.EngineMult == 1.0)
					Settings.EngineMult.Remove(GameDBText);
				else Settings.EngineMult[GameDBText] = D.EngineMult;
			}
			else if (D.EngineMult != 1.0)
				Settings.EngineMult.Add(GameDBText, D.EngineMult);
			if (Settings.RumbleMult.ContainsKey(GameDBText))
			{
				if (D.RumbleMult == 1.0)
					Settings.RumbleMult.Remove(GameDBText);
				else Settings.RumbleMult[GameDBText] = D.RumbleMult;
			}
			else if (D.RumbleMult != 1.0)
				Settings.RumbleMult.Add(GameDBText, D.RumbleMult);
#endif
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
#if slip
			if (Settings.SlipXMult.ContainsKey(GameDBText))
			{
				if (D.SlipXMult == 1.0)
					Settings.SlipXMult.Remove(GameDBText);
				else Settings.SlipXMult[GameDBText] = D.SlipXMult;
			}
			else if (D.SlipXMult != 1.0)
				Settings.SlipXMult.Add(GameDBText, D.SlipXMult);
			if (Settings.SlipYMult.ContainsKey(GameDBText))
			{
				if (D.SlipYMult == 1.0)
					Settings.SlipYMult.Remove(GameDBText);
				else Settings.SlipYMult[GameDBText] = D.SlipYMult;
			}
			else if (D.SlipYMult != 1.0)
				Settings.SlipYMult.Add(GameDBText, D.SlipYMult);
			if (Settings.SlipXGamma.ContainsKey(GameDBText))
			{
				if (D.SlipXGamma == 1.0)
					Settings.SlipXGamma.Remove(GameDBText);
				else Settings.SlipXGamma[GameDBText] = D.SlipXGamma;
			}
			else if (D.SlipXGamma != 1.0)
				Settings.SlipXGamma.Add(GameDBText, D.SlipXGamma);
			if (Settings.SlipYGamma.ContainsKey(GameDBText))
			{
				if (D.SlipYGamma == 1.0)
					Settings.SlipYGamma.Remove(GameDBText);
				else Settings.SlipYGamma[GameDBText] = D.SlipYGamma;
			}
			else if (D.SlipYGamma != 1.0)
				Settings.SlipYGamma.Add(GameDBText, D.SlipYGamma);
			Settings.SlipXMult["AllGames"] = D.SlipXMultAll;
			Settings.SlipYMult["AllGames"] = D.SlipYMultAll;
#endif
			// unconditionally save some
#if !slim
			Settings.RumbleMult["AllGames"] = D.RumbleMultAll;
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
#endif
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
#if !slim
					D.RumbleFromPlugin = true;
#endif
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
#if !slim
				case "CodemastersDirtRally1":
					D.TireDiameterSampleCount = -1;
#endif
					CurrentGame = GameId.DR2;
					GameDBText = "DR2";
					break;
				case "EAWRC23":
					CurrentGame = GameId.WRC23;
					GameDBText = "WRC23";
					D.AccSamples = 32;
#if !slim
					D.TireDiameterSampleCount = -1;
#endif
					break;
				case "BeamNgDrive":
					CurrentGame = GameId.BeamNG;
					GameDBText = "BeamNG";
#if !slim
					D.TireDiameterSampleCount = -1;
					break;
				case "Automobilista":
					CurrentGame = GameId.AMS1;
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
				case "CodemastersDirt2":
				case "CodemastersDirt3":
				case "CodemastersDirtShowdown":
				case "CodemastersDirt4":
					CurrentGame = GameId.D4;
					GameDBText = "D4";
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
#endif
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
			CarSpecAtlas CSA = new();

			LoadFailCount = 1;
			D = new SimData();
			bool ShowFreq = true, ShowSusp = true, ShowPhysics = true;
			SetGame(pluginManager);
			Settings = this.ReadCommonSettings("Settings", () => new Settings());
#if !slim
			bool ShowTire = false;
			E = new();
			if (null == Settings.Engine || null == Settings.Engine.Sliders || null == Settings.Engine.Tones
			 || 1 > Settings.Engine.Sliders.Count || 9 != Settings.Engine.Sliders[0].Length
			 || 4 != Settings.Engine.Tones.Length || 9 != Settings.Engine.Tones[0].Length)
			{
				if (null == Settings.Engine)
					Logging.Current.Info(pname + ".Init(): null Settings.Engine");
				else {
					if (null == Settings.Engine.Sliders)
						Logging.Current.Info(pname + $".Init(): null Settings.Engine.Sliders");
					else if (1 > Settings.Engine.Sliders.Count)
						Logging.Current.Info(pname + $".Init(): 0 Settings.Engine.Sliders");
					else if (9 != Settings.Engine.Sliders[0].Length)
						Logging.Current.Info(pname + $".Init(): Settings.Engine.Sliders[0].Slider.Length = "
								+ Settings.Engine.Sliders[0].Length.ToString());
					if (null == Settings.Engine.Tones)
						Logging.Current.Info(pname + $".Init(): null Settings.Engine.Tones");
					else {
						if (4 != Settings.Engine.Tones.Length)
							Logging.Current.Info(pname + $".Init():  Settings.Engine.Tones.Length = "
								+ Settings.Engine.Tones.Length.ToString());
						if (9 != Settings.Engine.Tones[0].Length)
							Logging.Current.Info(pname + $".Init():  Settings.Engine.Tones[0].Freq.Length = "
								+ Settings.Engine.Tones[0].Length.ToString()); 
					}
				}
				Settings = new();	// Settings.Engine will be initialized in End()
				Logging.Current.Info(pname + $".Init(): re-initializing Settings");
			}

			if (1 > Settings.ABSPulseLength)
				Settings.ABSPulseLength = 2;
#endif
			if (1 > Settings.DownshiftDurationMs)
				Settings.DownshiftDurationMs = 600;
			if (1 > Settings.UpshiftDurationMs)
				Settings.UpshiftDurationMs = 400;
			if (null == Settings.SuspensionMult)
				Settings.SuspensionMult = new();
#if !slim
			if (Settings.EngineMult == null)
				Settings.EngineMult = new Dictionary<string, double>();
			if (!Settings.EngineMult.ContainsKey("AllGames"))
				Settings.EngineMult.Add("AllGames", 1.0);
			if (Settings.RumbleMult == null)
				Settings.RumbleMult = new Dictionary<string, double>();
			if (!Settings.RumbleMult.ContainsKey("AllGames"))
				Settings.RumbleMult.Add("AllGames", 5.0);
#endif
			if (!Settings.SuspensionMult.ContainsKey("AllGames"))
				Settings.SuspensionMult.Add("AllGames", 1.5);
			if (null == Settings.SuspensionGamma)
				Settings.SuspensionGamma = new();
			if (!Settings.SuspensionGamma.ContainsKey("AllGames"))
				Settings.SuspensionGamma.Add("AllGames", 1.75);
#if !slim
			if (Settings.SlipXMult == null)
				Settings.SlipXMult = new Dictionary<string, double>();
			if (!Settings.SlipXMult.ContainsKey("AllGames"))
				Settings.SlipXMult.Add("AllGames", 1.6);
			if (Settings.SlipYMult == null)
				Settings.SlipYMult = new Dictionary<string, double>();
			if (!Settings.SlipYMult.ContainsKey("AllGames"))
				Settings.SlipYMult.Add("AllGames", 1.0);
			if (Settings.SlipXGamma == null)
				Settings.SlipXGamma = new Dictionary<string, double>();
			if (!Settings.SlipXGamma.ContainsKey("AllGames"))
				Settings.SlipXGamma.Add("AllGames", 1.0);
			if (Settings.SlipYGamma == null)
				Settings.SlipYGamma = new Dictionary<string, double>();
			if (!Settings.SlipYGamma.ContainsKey("AllGames"))
				Settings.SlipYGamma.Add("AllGames", 1.0);
			if (Settings.Motion == null)
				Settings.Motion = new Dictionary<string, double>();
#endif

			if (0 <= CSA.InDict(GameDBText))
			{
				Atlas = CSA.Atlas(GameDBText);
				Atlasst = $";  {Atlas.Count} cars in game Atlas" + CSA.Oops;
			}
			else Logging.Current.Info(pname + $".Init():  {GameDBText} not in Atlas");

			Save = Set = Changed = false;		// Init()
			string text;
			if (File.Exists(myfile))
			{
				text = File.ReadAllText(myfile);
				Dictionary<string, List<CarSpec>> json =
					JsonConvert.DeserializeObject<Dictionary<string, List<CarSpec>>>(text);
				text = S.LD.Init(this, json);
			}
			else text = ":  not found";
			Logging.Current.Info(pname + ".Init() " + myfile + text + Atlasst);

			D.Init(this);
			S.Init(this);
#if !slim
			E.Init(Settings.Engine, this);
			this.AttachDelegate("EngineLoad", () => D.EngineLoad);
			this.AttachDelegate("ABSPulse", () => D.ABSPulse);
#endif
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
#if slim
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
#else
				this.AttachDelegate("FreqOctave", () => D.FreqOctave);
				this.AttachDelegate("FreqIntervalA1", () => D.FreqIntervalA1);
				this.AttachDelegate("FreqIntervalA2", () => D.FreqIntervalA2);
				this.AttachDelegate("Gain1H2", () => D.Gain1H2);
				this.AttachDelegate("Gain2H", () => D.Gain2H);
				this.AttachDelegate("Gain4H", () => D.Gain4H);
				this.AttachDelegate("GainOctave", () => D.GainOctave);
				this.AttachDelegate("GainIntervalA1", () => D.GainIntervalA1);
				this.AttachDelegate("GainIntervalA2", () => D.GainIntervalA2);
				this.AttachDelegate("GainPeakA1Middle", () => D.GainPeakA1);
				this.AttachDelegate("GainPeakA2Middle", () => D.GainPeakA2);
				this.AttachDelegate("GainPeakB1Middle", () => D.GainPeakB1);
				this.AttachDelegate("GainPeakB2Middle", () => D.GainPeakB2);
				this.AttachDelegate("FreqPeakA1", () => D.FreqPeakA1);
				this.AttachDelegate("Gain1H", () => D.Gain1H);
				this.AttachDelegate("GainPeakA1Front", () => D.GainPeakA1Front);
				this.AttachDelegate("GainPeakA1Rear", () => D.GainPeakA1Rear);
				this.AttachDelegate("GainPeakA2Front", () => D.GainPeakA2Front);
				this.AttachDelegate("GainPeakA2Rear", () => D.GainPeakA2Rear);
				this.AttachDelegate("GainPeakB1Front", () => D.GainPeakB1Front);
				this.AttachDelegate("GainPeakB1Rear", () => D.GainPeakB1Rear);
				this.AttachDelegate("GainPeakB2Front", () => D.GainPeakB2Front);
				this.AttachDelegate("GainPeakB2Rear", () => D.GainPeakB2Rear);
#endif
			}
#if !slim
			if (ShowTire) {
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
				this.AttachDelegate("TireSamples", () => D.TireDiameterSampleCount);
			}
#endif
			if (ShowSusp)
			{
#if !slim
				this.AttachDelegate("SuspensionFreqR0a", () => D.SuspensionFreqRa);
				this.AttachDelegate("SuspensionFreqR0b", () => D.SuspensionFreqRb);
				this.AttachDelegate("SuspensionFreqR0c", () => D.SuspensionFreqRc);
				this.AttachDelegate("SuspensionFreqR4", () => D.SuspensionFreqR4);
				this.AttachDelegate("SuspensionFreqR5", () => D.SuspensionFreqR5);
				this.AttachDelegate("SuspensionMultR0a", () => D.SuspensionMultRa);
				this.AttachDelegate("SuspensionMultR0b", () => D.SuspensionMultRb);
				this.AttachDelegate("SuspensionMultR0c", () => D.SuspensionMultRc);
				this.AttachDelegate("SuspensionMultR4", () => D.SuspensionMultR4);
				this.AttachDelegate("SuspensionMultR5", () => D.SuspensionMultR5);
				this.AttachDelegate("SuspensionRumbleMultR0b", () => D.SuspensionRumbleMultRb);
				this.AttachDelegate("SuspensionRumbleMultR0c", () => D.SuspensionRumbleMultRc);
				this.AttachDelegate("SuspensionRumbleMultR4", () => D.SuspensionRumbleMultR4);
				this.AttachDelegate("SuspensionRumbleMultR5", () => D.SuspensionRumbleMultR5);
				this.AttachDelegate("SuspensionRumbleMultR1", () => D.SuspensionRumbleMultR1);
				this.AttachDelegate("SuspensionRumbleMultR2", () => D.SuspensionRumbleMultR2);
				this.AttachDelegate("SuspensionRumbleMultR3", () => D.SuspensionRumbleMultR3);
				this.AttachDelegate("RumbleFromPlugin", () => D.RumbleFromPlugin);
				this.AttachDelegate("RumbleMult", () => D.RumbleMult);
				this.AttachDelegate("RumbleLeft", () => D.RumbleLeft);
				this.AttachDelegate("RumbleRight", () => D.RumbleRight);
#endif
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
#if slim
					this.AttachDelegate("impactsR1Sway", () => D.ImpactsR1Sway());
					this.AttachDelegate("impactsAccSway2S", () => D.BipolarIIR(D.AccSway2S, D.impactsAccSway2S, 1, 15, 0.1));
					this.AttachDelegate("ThrottleIIR", () => D.ThrottleIIR());
#else
					this.AttachDelegate("MPitch", () => D.MotionPitch);
					this.AttachDelegate("MRoll", () => D.MotionRoll);
					this.AttachDelegate("MYaw", () => D.MotionYaw);
					this.AttachDelegate("MHeave", () => D.MotionHeave);
					this.AttachDelegate("MSurge", () => D.MotionSurge);
					this.AttachDelegate("MSway", () => D.MotionSway);
#endif
				}
			}
		}	// Init()
	}
}
