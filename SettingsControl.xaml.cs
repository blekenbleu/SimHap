﻿// Decompiled with JetBrains decompiler
// Type: sierses.Haptics.SettingsControl
// MVID: E01F66FE-3F59-44B4-8EBC-5ABAA8CD8267
using SimHub;
using System;
using System.Windows;
using System.Windows.Controls;

namespace sierses.Sim
{
	public partial class SettingsControl : UserControl //, IComponentConnector
	{
		public Haptics Plugin { get; }
		// this.InitializeComponent() gets generated by vs from SettingsControl.xaml
		ResourceDictionary Tres;
        public SettingsControl()
		{
			Tres = new ResourceDictionary();
            InitializeComponent();
		}
		public string Theme = "SteelLightTheme.xaml";

        internal void ChangeTheme(string theme)
		{
			Theme = theme;
			SetTheme();
		}

        internal void SetTheme()
		{
			Tres.Source = new Uri("/sierses.Sim;component/Themes/" + Theme, UriKind.Relative);
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(Tres);
        }

		public SettingsControl(Haptics plugin) : this()
		{
			Plugin = plugin;
			DataContext = Plugin;
			Version.Text = Plugin.PluginVersion;
			SetTheme();
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

		internal void InitHarmonics(Tone[] harmonic)
		{
			F1_value.Text = harmonic[1].Freq[0].ToString();
			F2_value.Text = harmonic[1].Freq[1].ToString();
			H1_value.Text = harmonic[1].Freq[2].ToString();
			H2_value.Text = harmonic[1].Freq[3].ToString();
			H3_value.Text = harmonic[1].Freq[4].ToString();
			H4_value.Text = harmonic[1].Freq[5].ToString();
			H5_value.Text = harmonic[1].Freq[6].ToString();
			H6_value.Text = harmonic[1].Freq[7].ToString();
			H1_factor.Value = harmonic[0].Freq[2];
			H2_factor.Value = harmonic[0].Freq[3];
			H3_factor.Value = harmonic[0].Freq[4];
			H4_factor.Value = harmonic[0].Freq[5];
			H5_factor.Value = harmonic[0].Freq[6];
			H6_factor.Value = harmonic[0].Freq[7];
		}

		private void EQ_Expanded(object sender, RoutedEventArgs e)
		{
			InitEq(Plugin.E.Q[Plugin.E.EQswitch].Slider);
		}

		private void TC_Expanded(object sender, RoutedEventArgs e)
		{
            InitHarmonics(Plugin.E.Tones);
		}

		private void H1_increment_Click(object sender, RoutedEventArgs e)
		{
			H1_value.Text = Plugin.E.Hval(2, true);
		}

		private void H1_decrement_Click(object sender, RoutedEventArgs e)
		{
			H1_value.Text = Plugin.E.Hval(2, false);
		}

		private void H2_increment_Click(object sender, RoutedEventArgs e)
		{
			H2_value.Text = Plugin.E.Hval(3, true);
		}

		private void H2_decrement_Click(object sender, RoutedEventArgs e)
		{
			H2_value.Text = Plugin.E.Hval(3, false);
		}

		private void H3_increment_Click(object sender, RoutedEventArgs e)
		{
			H3_value.Text = Plugin.E.Hval(4, true);
		}

		private void H3_decrement_Click(object sender, RoutedEventArgs e)
		{
			H3_value.Text = Plugin.E.Hval(4, false);
		}

		private void H4_increment_Click(object sender, RoutedEventArgs e)
		{
			H4_value.Text = Plugin.E.Hval(5, true);
		}

		private void H4_decrement_Click(object sender, RoutedEventArgs e)
		{
			H4_value.Text = Plugin.E.Hval(5, false);
		}

		private void H5_increment_Click(object sender, RoutedEventArgs e)
		{
			H5_value.Text = Plugin.E.Hval(6, true);
		}

		private void H5_decrement_Click(object sender, RoutedEventArgs e)
		{
			H5_value.Text = Plugin.E.Hval(6, false);
		}

		private void H6_increment_Click(object sender, RoutedEventArgs e)
		{
			H6_value.Text = Plugin.E.Hval(7, true);
		}

		private void H6_decrement_Click(object sender, RoutedEventArgs e)
		{
			H6_value.Text = Plugin.E.Hval(7, false);
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

		private void ButtonResetSuspension_Click(object sender, RoutedEventArgs e)
		{
			SuspensionMultAll.Value = 1.5;
			SuspensionMult.Value = 1;
			SuspensionGammaAll.Value = 1.7;
			SuspensionGamma.Value = 1;
		}

		// these should provoke equalizer property recalculations
		private void EQ0_increment_Click(object sender, RoutedEventArgs e)
		{
			EQ0_value.Text = $"{Plugin.E.Pincr(0, true)}";
		}

		private void EQ0_decrement_Click(object sender, RoutedEventArgs e)
		{
			EQ0_value.Text = $"{Plugin.E.Pincr(0, false)}";
		}

		private void EQ1_increment_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(1, true);
		}

		private void EQ1_decrement_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(1, false);
		}

		private void EQ2_increment_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(2, true);
		}

		private void EQ2_decrement_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(2, false);
		}

		private void EQ3_increment_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(3, true);
		}

		private void EQ3_decrement_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(3, false);
		}

		private void EQ4_increment_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(4, true);
		}

		private void EQ4_decrement_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(4, false);
		}

		private void EQ5_increment_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(5, true);
		}

		private void EQ5_decrement_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(5, false);
		}

		private void EQ6_increment_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(6, true);
		}

		private void EQ6_decrement_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(6, false);
		}

		private void EQ7_increment_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(7, true);
		}

		private void EQ7_decrement_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Incr(7, false);
		}

		private void EQ8_increment_Click(object sender, RoutedEventArgs e)
		{
			EQ8_value.Text = $"{Plugin.E.Pincr(8, true)}";
		}

		private void EQ8_decrement_Click(object sender, RoutedEventArgs e)
		{
			EQ8_value.Text = $"{Plugin.E.Pincr(8, false)}";
		}

        private void AmberTheme(object sender, RoutedEventArgs e)
        {
			ChangeTheme("AmberTheme.xaml");
        }

        private void BlueTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("BlueTheme.xaml");
        }

        private void CrimsonTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("CrimsonTheme.xaml");
        }
        private void GreenTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("GreenTheme.xaml");
        }

        private void OliveTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("OliveTheme.xaml");
        }

        private void OrangeTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("OrangeTheme.xaml");
        }

        private void PurpleTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("PurpleTheme.xaml");
        }

        private void SteelTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("SteelTheme.xaml");
        }

        private void BlueLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("BlueLightTheme.xaml");
        }

        private void AmberLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("AmberLightTheme.xaml");
        }

        private void CrimsonLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("CrimsonLightTheme.xaml");
        }

        private void GreenLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("GreenLightTheme.xaml");
        }

        private void OliveLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("OliveLightTheme.xaml");
        }

        private void OrangeLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("OrangeLightTheme.xaml");
        }

        private void PurpleLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("PurpleLightTheme.xaml");
        }

        private void SteelLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("SteelLightTheme.xaml");
        }

        private void MauveTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("MauveTheme.xaml");
        }

        private void MauveLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("MauveLightTheme.xaml");
        }

        private void CobaltTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("CobaltTheme.xaml");
        }

        private void CobaltLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("CobaltLightTheme.xaml");
        }

        private void TaupeTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("TaupeTheme.xaml");
        }

        private void TaupeLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("TaupeLightTheme.xaml");
        }

        private void EmeraldTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("EmeraldTheme.xaml");
        }

        private void EmeraldLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("EmeraldLightTheme.xaml");
        }
        private void TealTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("TealTheme.xaml");
        }

        private void TealLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("TealLightTheme.xaml");
        }

		private void MagentaTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("MagentaTheme.xaml");
        }

        private void MagentaLightTheme(object sender, RoutedEventArgs e)
        {
            ChangeTheme("MagentaLightTheme.xaml");
        }

    }
}
