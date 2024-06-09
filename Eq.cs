using MathNet.Numerics.Interpolation;
using SimHub.Plugins;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace sierses.Sim
{
    public class Tone	// array of frequency component properties
	{
		internal ObservableCollection<ushort> Freq = new();	// one for frequency harmonics, another for amplitudes
	}

	// array of 6 slider values, then min, max frequencies
	public class Eq : NotifyPropertyChanged
	{
		internal ObservableCollection<ushort> Slider = new();	// one for each slider + 2 for min and max frequency
	}

	public class Engine
	{
		public Tone[] Tones;		// engine fundamental and harmonic settings
		public ObservableCollection<Eq> Sliders;	// graphic equalizer settings
	}

	public partial class Geq
	{
		internal int EQswitch = 0;
		// an array of sliders for each equalizer
		internal ObservableCollection<Eq> Sliders = new();

		internal Haptics H;
		internal int Incr(int s, bool up)
		{
			if (up && 100 > Sliders[EQswitch].Slider[s])
				Sliders[EQswitch].Slider[s]++;
			else if ((!up) & 0 < Sliders[EQswitch].Slider[s])
				Sliders[EQswitch].Slider[s]--;
			return Sliders[EQswitch].Slider[s];
		}

		internal string NextUp(bool up)
		{
			if ((up && 2 == EQswitch) || (0 == EQswitch && !up))
				return $"{EQswitch}"; 
			EQswitch += up ? +1 : -1;
			return $"{EQswitch}";
		}

		// an array of LUT[][s interpolated from Sliders
		private List<ushort[][]> lUT = new() { };

		public Tone[] Tones = new Tone[2];	// frequency harmonic and amplitude

        public List<ushort[][]> LUT { get => lUT; set => lUT = value; }

		// populate  Tones' amplitudes from rpm
		// pitch is fundamental (rpm/60) or power stroke harmonic (integer * rpm * cylinders) / 120)
		public ushort Play(Haptics This, byte destination)
		{
			H = This;
			byte harmonic = (byte)(destination & 7);
            ushort[][] Lut = LUT[destination >> 3];
			int L = Lut[1].Length - 1;
			ushort freq;

			if (0 < harmonic)
				freq = (ushort)((60 + harmonic * This.D.Rpms * This.S.Car.cyl)/120);
			else freq = (ushort)((30 + This.D.Rpms) / 60);
			// mute out-of-range frequencies
			if (freq <= Lut[1][0] || freq > Lut[1][L])
				return 0;
			
			// set a Tone harmonic amplitude for a Tone frequency
			int i;
/* binary search increments
    16/32 (8): 24 :  8
    24/32 (4): 28 : 20 : 12 : 4
    28/32 (2): 30 : 26 : 22 : 18 : 14 : 10 : 6 : 2
	23/24 (1): 31 : 29 : 27 : 25 : 23 : 21 : 19 : 17 : 15 : 13 : 11 : 9 : 7 : 5 : 3 : 1
 */

			// Lut[0] has 32 harmonic scaling values;  Lut[1] has power-based frequency values
/* binary search maybe faster?...
			for (int j = (i = L >> 1) >> 1 ; 0 < j; j >>= 1) 
			{
				if (freq < Lut[1][i - 1])
					i -= j;
				else if (freq >= Lut[1][i])
					i += j;
				else break;
			}
 */
            for (i = 1; i <= L; i++)
				if (freq <= Lut[1][i])	// Lut interval for this Hz?
					break;

			ushort here = Lut[0][i - 1];
			ushort next = Lut[0][i];
			int interval = (Lut[1][i] - Lut[1][i - 1]);

			return (ushort)(Tones[1].Freq[harmonic]	// amplitude
					// linear interpolation of scaling values on not-quite-linear frequency intervals.
					* (here + (interval + 2 * (freq - Lut[1][i]) * (next - here)) / (interval * 2))); 
		}	// Play()

		// convert 7 slider values to paired 4 * (Eq.Slider.Length - 2) lookup table for Shape()
		public ushort[][] EqSpline(ushort[] slider)
		{
			ushort min = slider[0];
			ushort max = slider[slider.Length - 1];
			double[] xdata = new double[slider.Length];
			// ydata has slider values, except with first and last entries replaced by 0
			double[] ydata = new double[slider.Length];
			xdata[0] = 1;
			ydata[0] = 0;
			for (int i = 1; i < slider.Length; i++)
			{
				xdata[i] = i;			// equal increments
				ydata[i] = slider[i];
			}
			ydata[slider.Length - 1] = 0;
			var q = CubicSpline.InterpolateAkimaSorted(xdata, ydata);
			ushort[][] lut = new ushort[2][];
			double ff = System.Math.Log10(((double)max) / min) / (4 * xdata.Length - 4);
			double f = min;

			lut[0][0] = min;  lut[1][0] = max;
			for (int i = 0; i < (xdata.Length - 1); i++)
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
			if (null != that)
			{
				if (3 <= LUT.Count)
					return("already have 3 equalizers");
				LUT.Add(that);
				return Broadcast(This, LUT.Count);
			}
			return "null LUT";
		}
	}
}
