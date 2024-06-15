// Decompiled with JetBrains decompiler
// Type: sierses.Haptics.SettingsControl
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
using SimHub;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace sierses.Sim
{
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
			Tres.Source = new Uri("/sierses.Sim;component/Themes/" + Theme, UriKind.Relative);
//			Resources.MergedDictionaries.Clear();
//			Resources.MergedDictionaries.Add(Tres);
			Resources.MergedDictionaries[0] = Tres;
		}

		// called when expanding EQ or Plugin.E.NextUp() 
		internal void InitEq(ushort[] S)
		{
			EQ0_value.Text = S[0].ToString();
			EQ1_value.Text = S[1].ToString();
			EQ2_value.Text = S[2].ToString();
			EQ3_value.Text = S[3].ToString();
			EQ4_value.Text = S[4].ToString();
			EQ5_value.Text = S[5].ToString();
			EQ6_value.Text = S[6].ToString();
			EQ7_value.Text = S[7].ToString();
			EQ8_value.Text = S[8].ToString();
		}

		// invoked by UI Eq buttons
		// should provoke equalizer property recalculations
		private void EQ_Click(object sender, RoutedEventArgs e)
		{
			RepeatButton r = sender as RepeatButton;
			string s = r.Name;
            bool up = "i" == s.Substring(4, 1); // increment/decrement
			int index = int.Parse(s.Substring(2, 1));
			InitEq(0 == index % 8 ? Plugin.E.Pincr(index, up) : Plugin.E.Incr(index, up));
		}

		// see ToneControl.cs
		internal void InitHarmonics(Tone[] harmonic)
		{
			H1_value.Text = harmonic[1].Freq[2].ToString();
			H2_value.Text = harmonic[1].Freq[3].ToString();
			H3_value.Text = harmonic[1].Freq[4].ToString();
			H4_value.Text = harmonic[1].Freq[5].ToString();
			H5_value.Text = harmonic[1].Freq[6].ToString();
			H6_value.Text = harmonic[1].Freq[7].ToString();
			F1_value.Value = harmonic[1].Freq[0];
			F2_value.Value = harmonic[1].Freq[1];
			H1_factor.Value = harmonic[0].Freq[2];
			H2_factor.Value = harmonic[0].Freq[3];
			H3_factor.Value = harmonic[0].Freq[4];
			H4_factor.Value = harmonic[0].Freq[5];
			H5_factor.Value = harmonic[0].Freq[6];
			H6_factor.Value = harmonic[0].Freq[7];
		}

		// this may provoke new equalizer creation
		private void EQswitch_increment_Click(object sender, RoutedEventArgs e)
		{
			EQswitch_value.Text = Plugin.E.NextUp(true);
		}

		private void EQswitch_decrement_Click(object sender, RoutedEventArgs e)
		{
			EQswitch_value.Text = Plugin.E.NextUp(false);
		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			Plugin.S.Id = "";			// Refresh_Click() force a mismatch
			Logging.Current.Info($"Haptics.Refresh_Click()");
			Haptics.LoadFailCount = 1;
		}

		private void Lock_Click(object sender, RoutedEventArgs e)
		{
			Plugin.D.Unlocked = !Plugin.D.Unlocked;
			Plugin.D.LockedText = Plugin.D.Unlocked ? "Lock" : "Unlock";
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

        private void ExpanderEngine_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ExpanderEngine.IsExpanded = !ExpanderEngine.IsExpanded;
        }

        private void EQ_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EQ.IsExpanded = !EQ.IsExpanded;
        }

        private void ExpanderTone_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ExpanderTone.IsExpanded = !ExpanderTone.IsExpanded;
        }

        private void ExpanderSuspension_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ExpanderSuspension.IsExpanded = !ExpanderSuspension.IsExpanded;
        }

        private void ExpanderGforce_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ExpanderGforce.IsExpanded = !ExpanderGforce.IsExpanded;
        }
	}
}
