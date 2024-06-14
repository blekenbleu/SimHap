using SimHub;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace sierses.Sim
{
	// these methods are invoked by UI Engine harmonic buttons and sliders
	public partial class SettingsControl : UserControl
	{
		private void EQ_Expanded(object sender, RoutedEventArgs e)
		{
			InitEq(Plugin.E.Q[Plugin.E.EQswitch].Slider);
		}

		private void TC_Expanded(object sender, RoutedEventArgs e)
		{
			if (null != Plugin)
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

		private void FSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Slider s = sender as Slider;
			string h = s.Name.ToString();
			int i = int.Parse(h.Substring(1, 1));
			ReFactor(i - 1, s.Value);
		}

		private void HSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Slider s = sender as Slider;
			string h = s.Name.ToString();
			int i = int.Parse(h.Substring(1, 1));
			ReFactor(1 + i, s.Value);
		}
	}
}
