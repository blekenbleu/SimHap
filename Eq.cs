using MathNet.Numerics.Interpolation;
using SimHub.Plugins;
using System.Collections.Generic;

namespace sierses.Sim
{
    public class Tone	// array of frequency component properties
	{
		internal ushort[] Freq = new ushort[8];	// one for frequency harmonics, another for amplitudes
	}

	// array of 6 slider values, then min, max frequencies
	public class Eq : NotifyPropertyChanged
	{
		internal ushort[] Slider = new ushort[8];	// one for each slider + 2 for min and max frequency
	}

	public class Geq
	{
		// an array of sliders for each equalizer
		internal List<Eq> Sliders = new();

		internal Haptics H;
		internal void Init(Haptics h)
		{
			H = h;
			Tones[0] = new();
			Tones[0].Freq[0] = 1;
			Tones[0].Freq[1] = 1;
			Tones[0].Freq[2] = 1;
			Tones[0].Freq[3] = 1;
			Tones[0].Freq[4] = 1;
			Tones[0].Freq[5] = 1;
			Tones[0].Freq[6] = 1;
			Tones[0].Freq[7] = 1;
			Tones[1] = new();
			H.AttachDelegate("Fr0", () => Fr(0));
			H.AttachDelegate("Fr1", () => Fr(1));
			H.AttachDelegate("Fr2", () => Fr(2));
			H.AttachDelegate("Fr3", () => Fr(3));
			H.AttachDelegate("Fr4", () => Fr(4));
			H.AttachDelegate("Fr5", () => Fr(5));
			H.AttachDelegate("Fr6", () => Fr(6));
			H.AttachDelegate("Fr7", () => Fr(7));
		}

		// an array of LUT[][s interpolated from Sliders
		private List<ushort[][]> lUT = new() { };

		internal Tone[] Tones = new Tone[2];	// frequency harmonic and amplitude

        public List<ushort[][]> LUT { get => lUT; set => lUT = value; }

		// populate  Tones' amplitudes from rpm
		// pitch is fundamental (rpm/60) or power stroke harmonic (integer * rpm * cylinders) / 120)
		public ushort Play(Haptics This, byte destination)
		{
			H = This;
			byte harmonic = (byte)(destination & 7);
            ushort[][] Lut = LUT[destination >> 3];
			int l = Lut[1].Length - 1;
			ushort freq;

			if (0 < harmonic)
				freq = (ushort)((60 + harmonic * This.D.Rpms * This.S.Car.cyl)/120);
			else freq = (ushort)((30 + This.D.Rpms) / 60);
			// mute out-of-range frequencies
			if (freq < Lut[1][0] || freq >= Lut[1][l])
				return 0;
			
			// set a Tone harmonic amplitude for a Tone frequency
			int i;
/* binary search increments
   12/24 (6): 18 :  6
    18/24 (3): 21 : 15 : 9 : 3
    21/24 (2): 23 : 19
	23/24 (1): 24 : 22
	6:24 (3) 3 : 9
	3:24 (2) 1 : 5
	1:24 (1) 0 : 2
 */

			// Lut[0] has 24 harmonic scaling values;  Lut[1] has power-based frequency values
/* binary search maybe faster?...
			for (int j = i = (1 + l) >> 1 ; 0 < j; j = (j + (1 < j) ? 1 : 0) >> 1) 
			{
				if (freq < Lut[1][i])
					i -= j;
				else if (freq >= Lut[1][i + 1])
					i += j;
				else break;
			}
 */
            for (i = 1; i <= l; i++)
				if (freq >= Lut[1][i])	// Lut interval for this Hz?
					break;

			ushort here = Lut[0][i];
			ushort next = Lut[0][i + 1];
			int interval = (Lut[1][i + 1] - Lut[1][i]);

			return (ushort)(Tones[1].Freq[harmonic]	// amplitude
					// linear interpolation of scaling values on not-quite-linear frequency intervals.
					* (here + (interval + 2 * (freq - Lut[1][i]) * (next - here)) / (interval * 2))); 
		}	// Play()

		// convert 6 slider values to paired 4 * (Eq.Slider.Length - 2) lookup table for Shape()
		public ushort[][] EqSpline(ushort[] slider)
		{
			ushort min = slider[6];
			ushort max = slider[7];
			double[] xdata = new double[] { 1, 2, 3, 4, 5, 6 };	// equal increments
			double[] ydata = new double[] { slider[0], slider[1], slider[2], slider[3], slider[4], slider[5] };
			var q = CubicSpline.InterpolateAkimaSorted(xdata, ydata);
			ushort[][] lut = new ushort[2][];
			double ff = System.Math.Log10(((double)max) / min);
			double f = min;

			lut[0][0] = min;  lut[1][0] = max;
			for (int i = 0; i < lut[0].Length; i++)
			{
				lut[0][1 + i] = (ushort)(0.5 + q.Interpolate(1 + 0.25 * i)); // equally spaced interpolated values
				lut[1][1 + i] = (ushort)(0.5 + f);
				f *= ff;			// geometric frequency progression
			}
			return lut;
		}

		// for properties
		public ushort Fr(byte i)
		{
			return (ushort)((0 == i) ? ((30 + Tones[0].Freq[i] * H.D.Rpms) / 60)
									 : ((60 + Tones[0].Freq[i] * H.D.Rpms * H.S.Car.cyl) / 120));
		}

		// AddProps() should be called by UI to add equalizer instances,
		// which are Tone components played thru Shape() using that LUT
		// e.g. AddProps(This, EqSpline(sliders[n]));
		public string AddProps(Haptics This,  ushort[][] that)
		{
			LUT.Add(that);
			string s;
			switch (LUT.Count)
			{
				case 1:
					This.AttachDelegate("Eq0.0", () => Play(This, 0));
					This.AttachDelegate("Eq0.1", () => Play(This, 1));
					This.AttachDelegate("Eq0.2", () => Play(This, 2));
					This.AttachDelegate("Eq0.3", () => Play(This, 3));
					This.AttachDelegate("Eq0.4", () => Play(This, 4));
					This.AttachDelegate("Eq0.5", () => Play(This, 5));
					This.AttachDelegate("Eq0.6", () => Play(This, 6));
					This.AttachDelegate("Eq0.7", () => Play(This, 7));
					s = "Eq0 added";
					break;
				case 2:
					This.AttachDelegate("Eq1.0", () => Play(This, 8));
					This.AttachDelegate("Eq1.1", () => Play(This, 9));
					This.AttachDelegate("Eq1.2", () => Play(This, 10));
					This.AttachDelegate("Eq1.3", () => Play(This, 11));
					This.AttachDelegate("Eq1.4", () => Play(This, 12));
					This.AttachDelegate("Eq1.5", () => Play(This, 13));
					This.AttachDelegate("Eq1.6", () => Play(This, 14));
					This.AttachDelegate("Eq1.7", () => Play(This, 15));
					s = "Eq1 added";
					break;
				case 3:
					This.AttachDelegate("Eq2.0", () => Play(This, 16));
					This.AttachDelegate("Eq2.1", () => Play(This, 17));
					This.AttachDelegate("Eq2.2", () => Play(This, 18));
					This.AttachDelegate("Eq2.3", () => Play(This, 19));
					This.AttachDelegate("Eq2.4", () => Play(This, 20));
					This.AttachDelegate("Eq2.5", () => Play(This, 21));
					This.AttachDelegate("Eq2.6", () => Play(This, 22));
					This.AttachDelegate("Eq2.7", () => Play(This, 23));
					s = "Eq2 added; max equalizer supported...";
					break;
				default:
					s = "equalizer limit exceeded";
					break;
			}
			return s;
		}
	}
}
