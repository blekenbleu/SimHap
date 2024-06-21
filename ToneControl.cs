using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace sierses.Sim
{
	// these methods are invoked by UI Engine harmonic buttons and sliders
	public partial class SettingsControl : UserControl
	{
		internal void InitHarmonics(Tone[] harmonic)
		{
			//writing to these values provokes recalculations
			int a = harmonic[0].Freq[2];			// Tone bank selector
			int b = 1 + a;
			if (F0.Value != harmonic[b].Freq[0])
				F0.Value = harmonic[b].Freq[0];		// RPM / 60
			if (F1.Value != harmonic[b].Freq[1])   
				F1.Value = harmonic[b].Freq[1];		// RPM * cyl / 120
			if (F2.Value != harmonic[b].Freq[2])	// RPM / 120 modulation
				F2.Value = harmonic[b].Freq[2];
			if (H1_value.Text != harmonic[b].Freq[3].ToString())
				H1_value.Text = harmonic[b].Freq[3].ToString();
			if (H2_value.Text != harmonic[b].Freq[4].ToString())
				H2_value.Text = harmonic[b].Freq[4].ToString();
			if (H3_value.Text != harmonic[b].Freq[5].ToString())
				H3_value.Text = harmonic[b].Freq[5].ToString();
			if (H4_value.Text != harmonic[b].Freq[6].ToString())
				H4_value.Text = harmonic[b].Freq[6].ToString();
			if (H5_value.Text != harmonic[b].Freq[7].ToString())
				H5_value.Text = harmonic[b].Freq[7].ToString();
			if (H6_value.Text != harmonic[b].Freq[8].ToString())
				H6_value.Text = harmonic[b].Freq[8].ToString();
			if (H1_factor.Value != harmonic[a].Freq[3])	// first harmonic of F1
				H1_factor.Value = harmonic[a].Freq[3];
			if (H2_factor.Value != harmonic[a].Freq[4])
				H2_factor.Value = harmonic[a].Freq[4];
			if (H3_factor.Value != harmonic[a].Freq[5])
				H3_factor.Value = harmonic[a].Freq[5];
			if (H4_factor.Value != harmonic[a].Freq[6])
				H4_factor.Value = harmonic[a].Freq[6];
			if (H5_factor.Value != harmonic[a].Freq[7])
				H5_factor.Value = harmonic[a].Freq[7];
			if (H6_factor.Value != harmonic[a].Freq[8])
				H6_factor.Value = harmonic[a].Freq[8];
			ToneMode.Text = (2 == Plugin.E.Tones[0].Freq[2]) ? "  Full Throttle" : "";
		}

		private void ThrottleLoadSwitch_Click(object sender, RoutedEventArgs e)
		{
			Plugin.E.Tones[0].Freq[2] = (ushort)(2 == Plugin.E.Tones[0].Freq[2] ? 0 : 2);	// switch tone sets
			InitHarmonics(Plugin.E.Tones);
		}

		private void TC_Expanded(object sender, RoutedEventArgs e)
		{
			if (null != Plugin)
				InitHarmonics(Plugin.E.Tones);
		}

		private void H_Click(object sender, RoutedEventArgs e)
		{
			RepeatButton r = sender as RepeatButton;
			string s = r.Name;
			bool up = "i" == s.Substring(3, 1);	// increment/decrement
			int index = 2 + int.Parse(s.Substring(1, 1));
			int b = 1 + Plugin.E.Tones[0].Freq[2];
			if (up && 1000 > Plugin.E.Tones[b].Freq[index])
				Plugin.E.Tones[b].Freq[index]++;
			else if ((!up) && 0 < Plugin.E.Tones[b].Freq[index])
				Plugin.E.Tones[b].Freq[index]--;

		    InitHarmonics(Plugin.E.Tones);
		}

		private void HSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (null == Plugin)
				return;

			Slider s = sender as Slider;
			string h = s.Name.ToString();	// F[12] or H[1-6]
			int i = int.Parse(h.Substring(1, 1)) + ("H" == h.Substring(0, 1) ? 2 : 0);
			int a = Plugin.E.Tones[0].Freq[2];	// index to pair of Tones

			// limit check Engine Tone Factor sliders
			// index 0 and 1 Factors are fixed at one
			// max factor is 13 for index 7 (6 harmonics)
			// min factor is 2 for index 2
			Tone[] harmonic = Plugin.E.Tones;

			if (2 < i)	// sliders [<2] are modulation and fundamental peak amplitudes, NOT harmonic factors
			{
				harmonic[a].Freq[i] = Convert.ToUInt16(0.1 + s.Value);
				for (int j = i - 1; j > 2; j--)
					if (harmonic[a].Freq[j] >= harmonic[a].Freq[j + 1])
						harmonic[a].Freq[j] = (ushort)(harmonic[a].Freq[j + 1] - 1);
				for (int j = i + 1; j < 9; j++)
					if (harmonic[a].Freq[j] <= harmonic[a].Freq[j - 1])
						harmonic[a].Freq[j] = (ushort)(1 + harmonic[a].Freq[j - 1]);
			}
			else harmonic[a + 1].Freq[i] = Convert.ToUInt16(0.1 + s.Value);	// Mod, RPM or Fund Peak value
			InitHarmonics(Plugin.E.Tones);
		}
	}
}
