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
			int index = 1 + int.Parse(s.Substring(1, 1));
			if (up && 1000 > Plugin.E.Tones[1].Freq[index])
				Plugin.E.Tones[1].Freq[index]++;
			else if ((!up) && 0 < Plugin.E.Tones[1].Freq[index])
				Plugin.E.Tones[1].Freq[index]--;

		    InitHarmonics(Plugin.E.Tones);
		}

		private void HSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (null == Plugin)
				return;

			Slider s = sender as Slider;
			string h = s.Name.ToString();	// F[12] or H[1-6]
			int i = int.Parse(h.Substring(1, 1)) + ("H" == h.Substring(0, 1) ? 1 : -1);

			// limit check Engine Tone Factor sliders
			// index 0 and 1 Factors are fixed at one
			// max factor is 13 for index 7 (6 harmonics)
			// min factor is 2 for index 2
			Tone[] harmonic = Plugin.E.Tones;

			if (1 < i)	// sliders [0-1] are fundamental peak amplitudes, not harmonic factors
			{
				harmonic[0].Freq[i] = Convert.ToUInt16(0.1 + s.Value);
				for (int j = i - 1; j > 1; j--)
					if (harmonic[0].Freq[j] >= harmonic[0].Freq[j + 1])
						harmonic[0].Freq[j] = (ushort)(harmonic[0].Freq[j + 1] - 1);
				for (int j = i + 1; j < 8; j++)
					if (harmonic[0].Freq[j] <= harmonic[0].Freq[j - 1])
						harmonic[0].Freq[j] = (ushort)(1 + harmonic[0].Freq[j - 1]);
			}
			else harmonic[1].Freq[i] = Convert.ToUInt16(0.1 + s.Value);

			InitHarmonics(Plugin.E.Tones);
		}
	}
}
