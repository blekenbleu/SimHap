using SimHub;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace blekenbleu.Haptic
{
	// see also ToneControl.cs in master branch

	public partial class SettingsControl : UserControl //, IComponentConnector
	{
		public readonly Haptics Plugin;
		readonly ResourceDictionary Tres;
		public SettingsControl()
		{
			Tres = new ResourceDictionary();
			// VS generates InitializeComponent() from settingscontrol.xaml
			InitializeComponent();
		}


		public SettingsControl(Haptics plugin) : this()
		{
			Plugin = plugin;
			DataContext = Plugin;
			Version.Text = Plugin.PluginVersion;
			SetTheme();
		}

		internal string Theme = "SteelLightTheme.xaml";
		internal void ChangeTheme(string theme)
		{
			Theme = theme;
			SetTheme();
		}

		internal void SetTheme()
		{
			Tres.Source = new Uri("/blekenbleu.Haptic;component/Themes/" + Theme, UriKind.Relative);
			//			Resources.MergedDictionaries.Clear();
			//			Resources.MergedDictionaries.Add(Tres);
			Resources.MergedDictionaries[0] = Tres;
		}

		// called when expanding EQ or Plugin.E.NextUp() 
		internal string ShowEq(ushort[] S)
		{
			//writing to these values provokes recalculations
			if (EQ0_value.Text != S[0].ToString())
				EQ0_value.Text = S[0].ToString();
			if (EQ1_value.Text != S[1].ToString())
				EQ1_value.Text = S[1].ToString();
			if (EQ2_value.Text != S[2].ToString())
				EQ2_value.Text = S[2].ToString();
			if (EQ3_value.Text != S[3].ToString())
				EQ3_value.Text = S[3].ToString();
			if (EQ4_value.Text != S[4].ToString())
				EQ4_value.Text = S[4].ToString();
			if (EQ5_value.Text != S[5].ToString())
				EQ5_value.Text = S[5].ToString();
			if (EQ6_value.Text != S[6].ToString())
				EQ6_value.Text = S[6].ToString();
			if (EQ7_value.Text != S[7].ToString())
				EQ7_value.Text = S[7].ToString();
			if (EQ8_value.Text != S[8].ToString())
				EQ8_value.Text = S[8].ToString();
			return Plugin.E.EQswitch.ToString();
		}

		private void EQ_Expanded(object sender, RoutedEventArgs e)
		{
			if (null != Plugin)
				ShowEq(Plugin.E.Q[Plugin.E.EQswitch].Slider);
		}

		// invoked by UI Eq buttons
		// should provoke equalizer property recalculations
		private void EQ_Click(object sender, RoutedEventArgs e)
		{
			RepeatButton r = sender as RepeatButton;
			string s = r.Name;
			bool up = "i" == s.Substring(4, 1); // increment/decrement
			int index = int.Parse(s.Substring(2, 1));
			ShowEq(0 == index % 8 ? Plugin.E.Pincr(index, up) : Plugin.E.Incr(index, up));
		}

		// this may provoke new equalizer creation
		private void EQswitch_increment_Click(object sender, RoutedEventArgs e)
		{
			EQswitch_value.Text = ShowEq(Plugin.E.NextUp(true));
		}

		private void EQswitch_decrement_Click(object sender, RoutedEventArgs e)
		{
			EQswitch_value.Text = ShowEq(Plugin.E.NextUp(false));
		}

		private void DeleteEQ_Click(object sender, RoutedEventArgs e)
		{
			if (2 > Plugin.E.Q.Count)
				return;
			Plugin.E.Q.RemoveAt(Plugin.E.EQswitch);
			Plugin.E.LUT.RemoveAt(Plugin.E.EQswitch);
			if (Plugin.E.EQswitch >= Plugin.E.Q.Count)
				Plugin.E.EQswitch = Plugin.E.Q.Count - 1;
			EQswitch_value.Text = ShowEq(Plugin.E.Q[Plugin.E.EQswitch].Slider);	
		}

		private void InitEQ_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Q[Plugin.E.EQswitch] = Plugin.E.NewEQ();
			ShowEq(Plugin.E.Q[Plugin.E.EQswitch].Slider);
			Plugin.E.EqSpline(Plugin.E.EQswitch);
		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			Logging.Current.Info($"blekHapt.Refresh_Click()");
			Plugin.Changed = false;
			Haptics.LoadFailCount = 1;
			Plugin.CarId = "";		   // Refresh_Click() force a mismatch
		}

		private void Lock_Click(object sender, RoutedEventArgs e)
		{
			Plugin.D.Locked = !Plugin.D.Locked;
			Plugin.D.LockedText = Plugin.D.Locked ? "Unlock" : "Lock";
		}

		private void MenuTheme(object sender, RoutedEventArgs e)
		{
			MenuItem m = sender as MenuItem;
			ChangeTheme(m.Header.ToString() + "Theme.xaml");
		}

		private void EngineView_Switch_Checked(object sender, RoutedEventArgs e)
		{
			EngineView_Switch.Content = "Expand";
			EngView1.Height = new GridLength(0);
			EngView2.Height = new GridLength(0);
			EngView3.Height = new GridLength(0);
			EngView4.Height = new GridLength(0);
		}

		private void EngineView_Switch_Unchecked(object sender, RoutedEventArgs e)
		{
			EngineView_Switch.Content = "Collapse";
			EngView1.Height = GridLength.Auto;
			EngView2.Height = GridLength.Auto;
			EngView3.Height = GridLength.Auto;
			EngView4.Height = GridLength.Auto;
		}

		private void ButtonResetSuspension_Click(object sender, RoutedEventArgs e)
		{
			SuspensionMultAll.Value = 1.5;
			SuspensionMult.Value = 1;
			SuspensionGammaAll.Value = 1.7;
			SuspensionGamma.Value = 1;
		}

		private void SuspensionMultAll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void SuspensionGammaAll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void SuspensionGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void EngineMulti_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}

		private void ExpanderEngine_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{

			if (e.ClickCount == 2)
			{
				ExpanderEngine.IsExpanded = !ExpanderEngine.IsExpanded;
			}
		}

		private void EQ_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				EQ.IsExpanded = !EQ.IsExpanded;
			}
		}

		private void ExpanderTone_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				ExpanderTone.IsExpanded = !ExpanderTone.IsExpanded;
			}
		}

		private void ExpanderSuspension_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				ExpanderSuspension.IsExpanded = !ExpanderSuspension.IsExpanded;
			}
		}

		private void ExpanderGforce_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				ExpanderGforce.IsExpanded = !ExpanderGforce.IsExpanded;
			}
		}

		private void MotionPitchOffset_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelPitchOffset.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelPitchOffset.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionPitchOffset_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelPitchOffset.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelPitchOffset.FontWeight = FontWeights.Regular;
		}

		private void MotionRollOffset_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelRollOffset.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelRollOffset.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionRollOffset_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelRollOffset.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelRollOffset.FontWeight = FontWeights.Regular;
		}

		private void MotionYawOffset_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelYawOffset.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelYawOffset.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionYawOffset_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelYawOffset.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelYawOffset.FontWeight = FontWeights.Regular;
		}

		private void MotionHeaveOffset_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelHeaveOffset.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelHeaveOffset.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionHeaveOffset_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelHeaveOffset.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelHeaveOffset.FontWeight = FontWeights.Regular;
		}

		private void MotionSurgeOffset_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelSurgeOffset.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelSurgeOffset.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionSurgeOffset_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelSurgeOffset.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelSurgeOffset.FontWeight = FontWeights.Regular;
		}

		private void MotionSwayOffset_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelSwayOffset.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelSwayOffset.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionSwayOffset_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelSwayOffset.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelSwayOffset.FontWeight = FontWeights.Regular;
		}

		private void MotionPitchMult_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelPitchMult.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelPitchMult.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionPitchMult_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelPitchMult.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelPitchMult.FontWeight = FontWeights.Regular;
		}

		private void MotionRollMult_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelRollMult.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelRollMult.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionRollMult_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelRollMult.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelRollMult.FontWeight = FontWeights.Regular;
		}

		private void MotionYawMult_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelYawMult.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelYawMult.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionYawMult_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelYawMult.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelYawMult.FontWeight = FontWeights.Regular;
		}

		private void MotionHeaveMult_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelHeaveMult.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelHeaveMult.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionHeaveMult_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelHeaveMult.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelHeaveMult.FontWeight = FontWeights.Regular;
		}

		private void MotionSurgeMult_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelSurgeMult.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelSurgeMult.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionSurgeMult_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelSurgeMult.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelSurgeMult.FontWeight = FontWeights.Regular;
		}

		private void MotionSwayMult_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelSwayMult.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelSwayMult.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionSwayMult_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelSwayMult.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelSwayMult.FontWeight = FontWeights.Regular;
		}

		private void MotionPitchGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelPitchGamma.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelPitchGamma.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionPitchGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelPitchGamma.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelPitchGamma.FontWeight = FontWeights.Regular;
		}

		private void MotionRollGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelRollGamma.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelRollGamma.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionRollGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelRollGamma.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelRollGamma.FontWeight = FontWeights.Regular;
		}

		private void MotionYawGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelYawGamma.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelYawGamma.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionYawGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelYawGamma.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelYawGamma.FontWeight = FontWeights.Regular;
		}

		private void MotionHeaveGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelHeaveGamma.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelHeaveGamma.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionHeaveGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelHeaveGamma.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelHeaveGamma.FontWeight = FontWeights.Regular;
		}

		private void MotionSurgeGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelSurgeGamma.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelSurgeGamma.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionSurgeGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelSurgeGamma.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelSurgeGamma.FontWeight = FontWeights.Regular;
		}

		private void MotionSwayGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				LabelSwayGamma.Foreground = (SolidColorBrush)this.FindResource("Light");
				LabelSwayGamma.FontWeight = FontWeights.Bold;
			}
		}

		private void MotionSwayGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			LabelSwayGamma.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			LabelSwayGamma.FontWeight = FontWeights.Regular;
		}

		private void CarName_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngNameLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngNameLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void CarName_LostFocus(object sender, RoutedEventArgs e)
		{
			EngNameLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngNameLabel.FontWeight = FontWeights.Regular;
		}

		private void CarID_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngIDLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngIDLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void CarID_LostFocus(object sender, RoutedEventArgs e)
		{
			EngIDLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngIDLabel.FontWeight = FontWeights.Regular;
		}

		private void CarClass_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngCategoryLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngCategoryLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void CarClass_LostFocus(object sender, RoutedEventArgs e)
		{
			EngCategoryLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngCategoryLabel.FontWeight = FontWeights.Regular;
		}

		private void IdleRPM_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngIdleLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngIdleLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void IdleRPM_LostFocus(object sender, RoutedEventArgs e)
		{
			EngIdleLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngIdleLabel.FontWeight = FontWeights.Regular;
		}

		private void Redline_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngRedlineLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngRedlineLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void Redline_LostFocus(object sender, RoutedEventArgs e)
		{
			EngRedlineLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngRedlineLabel.FontWeight = FontWeights.Regular;
		}

		private void MaxRPM_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngMaxLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngMaxLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void MaxRPM_LostFocus(object sender, RoutedEventArgs e)
		{
			EngMaxLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngMaxLabel.FontWeight = FontWeights.Regular;
		}

		private void MaxPowerHP_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngHPLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngHPLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void MaxPowerHP_LostFocus(object sender, RoutedEventArgs e)
		{
			EngHPLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngHPLabel.FontWeight = FontWeights.Regular;
		}

		private void EMaxPowerHP_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngEHPLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngEHPLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void EMaxPowerHP_LostFocus(object sender, RoutedEventArgs e)
		{
			EngEHPLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngEHPLabel.FontWeight = FontWeights.Regular;
		}

		private void MaxTorqueNM_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngNMLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngNMLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void MaxTorqueNM_LostFocus(object sender, RoutedEventArgs e)
		{
			EngNMLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngNMLabel.FontWeight = FontWeights.Regular;
		}

		private void Displacement_GotFocus(object sender, RoutedEventArgs e)
		{
			{
				TextBox textBox = (TextBox)sender;
				textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
				EngCCLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
				EngCCLabel.FontWeight = FontWeights.Bold;
			}
		}

		private void Displacement_LostFocus(object sender, RoutedEventArgs e)
		{
			EngCCLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngCCLabel.FontWeight = FontWeights.Regular;
		}

		private void EngineConfig_GotFocus(object sender, RoutedEventArgs e)
		{
			EngConfigLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngConfigLabel.FontWeight = FontWeights.Bold;
		}

		private void EngineConfig_LostFocus(object sender, RoutedEventArgs e)
		{
			EngConfigLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngConfigLabel.FontWeight = FontWeights.Regular;
		}

		private void EngineCylinders_GotFocus(object sender, RoutedEventArgs e)
		{
			EngCylindersLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngCylindersLabel.FontWeight = FontWeights.Bold;
		}

		private void EngineCylinders_LostFocus(object sender, RoutedEventArgs e)
		{
			EngCylindersLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngCylindersLabel.FontWeight = FontWeights.Regular;
		}

		private void Drivetrain_GotFocus(object sender, RoutedEventArgs e)
		{
			EngDrivetrainLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngDrivetrainLabel.FontWeight = FontWeights.Bold;
		}

		private void Drivetrain_LostFocus(object sender, RoutedEventArgs e)
		{
			EngDrivetrainLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngDrivetrainLabel.FontWeight = FontWeights.Regular;
		}

		private void Englocation_GotFocus(object sender, RoutedEventArgs e)
		{
			EngLocationLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			EngLocationLabel.FontWeight = FontWeights.Bold;
		}

		private void Englocation_LostFocus(object sender, RoutedEventArgs e)
		{
			EngLocationLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			EngLocationLabel.FontWeight = FontWeights.Regular;
		}

		private void FiringOrder_GotFocus(object sender, RoutedEventArgs e)
		{
			FiringOrderLabel1.Foreground = (SolidColorBrush)this.FindResource("Light");
			FiringOrderLabel1.FontWeight = FontWeights.Bold;
            FiringOrderLabel2.Foreground = (SolidColorBrush)this.FindResource("Light");
            FiringOrderLabel2.FontWeight = FontWeights.Bold;
            FiringOrderLabel3.Foreground = (SolidColorBrush)this.FindResource("Light");
            FiringOrderLabel3.FontWeight = FontWeights.Bold;
        }

		private void FiringOrder_LostFocus(object sender, RoutedEventArgs e)
		{
			FiringOrderLabel1.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			FiringOrderLabel1.FontWeight = FontWeights.Regular;
            FiringOrderLabel2.Foreground = (SolidColorBrush)this.FindResource("Foreground");
            FiringOrderLabel2.FontWeight = FontWeights.Regular;
            FiringOrderLabel3.Foreground = (SolidColorBrush)this.FindResource("Foreground");
            FiringOrderLabel3.FontWeight = FontWeights.Regular;
        }

		private void SuspensionMultAll_GotFocus(object sender, RoutedEventArgs e)
		{
			SusMultAllLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusMultAllLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionMultAll_LostFocus(object sender, RoutedEventArgs e)
		{
			SusMultAllLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusMultAllLabel.FontWeight = FontWeights.Regular;
		}

		private void SuspensionMult_GotFocus(object sender, RoutedEventArgs e)
		{
			SusMultLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusMultLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionMult_LostFocus(object sender, RoutedEventArgs e)
		{
			SusMultLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusMultLabel.FontWeight = FontWeights.Regular;
		}

		private void SuspensionGammaAll_GotFocus(object sender, RoutedEventArgs e)
		{
			SusGammaAllLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusGammaAllLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionGammaAll_LostFocus(object sender, RoutedEventArgs e)
		{
			SusGammaAllLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusGammaAllLabel.FontWeight = FontWeights.Regular;
		}

		private void SuspensionGamma_GotFocus(object sender, RoutedEventArgs e)
		{
			SusGammaLabel.Foreground = (SolidColorBrush)this.FindResource("Light");
			SusGammaLabel.FontWeight = FontWeights.Bold;
		}

		private void SuspensionGamma_LostFocus(object sender, RoutedEventArgs e)
		{
			SusGammaLabel.Foreground = (SolidColorBrush)this.FindResource("Foreground");
			SusGammaLabel.FontWeight = FontWeights.Regular;
		}
	}
}
