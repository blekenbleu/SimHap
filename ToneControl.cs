using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace sierses.Sim
{
	// these methods are invoked by UI Engine harmonic buttons and sliders
	public partial class SettingsControl : UserControl
	{
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
