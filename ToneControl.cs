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
            InitHarmonics(Plugin.E.Tones);
		}

		private void F1_increment_Click(object sender, RoutedEventArgs e)
		{
			F1_value.Text = Plugin.E.Hval(0, true);
		}

		private void F1_decrement_Click(object sender, RoutedEventArgs e)
		{
			F1_value.Text = Plugin.E.Hval(0, false);
		}

		private void F2_increment_Click(object sender, RoutedEventArgs e)
		{
			F2_value.Text = Plugin.E.Hval(1, true);
		}

		private void F2_decrement_Click(object sender, RoutedEventArgs e)
		{
			F2_value.Text = Plugin.E.Hval(1, false);
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

/*		private void F1slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(0, F1_factor.Value);
		}

		private void F2slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(1, F2_factor.Value);
		}
 */
		private void H1slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(2, H1_factor.Value);
		}

		private void H2slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(3, H2_factor.Value);
		}

		private void H3slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(4, H3_factor.Value);
		}

		private void H4slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(5, H4_factor.Value);
		}

		private void H5slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(6, H5_factor.Value);
		}

		private void H6slider(object sender, MouseButtonEventArgs e)
		{
			ReFactor(7, H6_factor.Value);
		}
    }
}
