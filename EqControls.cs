using System.Windows;
using System.Windows.Controls;

namespace sierses.Sim
{
	// these methods are invoked by UI Eq buttons
	public partial class SettingsControl : UserControl
	{
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
    }
}
