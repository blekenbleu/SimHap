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

	public class Geq
	{
		internal int EQswitch = 0;
		// an array of sliders for each equalizer
		internal ObservableCollection<Eq> Sliders = new();

		internal Haptics H;
		internal void Init(Settings Settings, Haptics h)
		{
			H = h;
  			if (null == Settings.Engine || null == Settings.Engine.Tones
			 || null == Settings.Engine.Tones[1].Freq || 0 == Settings.Engine.Tones[1].Freq.Count)
  			{
				Tones[0] = new();
				Tones[0].Freq.Add(1);	// engine RPM / 60
				Tones[0].Freq.Add(1);	// power stroke: cylinders * engine RPM / 120
				Tones[0].Freq.Add(3);	// first power stroke harmonic
				Tones[0].Freq.Add(5);
				Tones[0].Freq.Add(7);
				Tones[0].Freq.Add(9);
				Tones[0].Freq.Add(11);
				Tones[0].Freq.Add(13);	// sixth power stroke harmonic
				Tones[1] = new();		// harmonic amplitudes
				Tones[1].Freq.Add(1000);
				Tones[1].Freq.Add(1000);
				Tones[1].Freq.Add(333);
				Tones[1].Freq.Add(111);
				Tones[1].Freq.Add(37);
				Tones[1].Freq.Add(12);
				Tones[1].Freq.Add(4);
				Tones[1].Freq.Add(1);
  			}
  			else Tones = Settings.Engine.Tones;

			if (null == Settings.Engine || null == Settings.Engine.Sliders)
				Sliders = new();
			else Sliders = Settings.Engine.Sliders;
            if (1 > Sliders.Count || 9 != Sliders[0].Slider.Count)
			{
				ushort s = 50, highpass = 20, lowpass = 900;
				ObservableCollection<ushort> S = new() { highpass, s, s, s, s, s, s, s, lowpass };
				if (1 > Sliders.Count)
                	Sliders.Add(new Eq() { Slider = S });
				else Sliders[0] = new Eq() { Slider = S };
			}
//			H.SC.Init(Sliders[0].Slider);
			H.AttachDelegate("Fr0", () => Fr(0));
			H.AttachDelegate("Fr1", () => Fr(1));
			H.AttachDelegate("Fr2", () => Fr(2));
			H.AttachDelegate("Fr3", () => Fr(3));
			H.AttachDelegate("Fr4", () => Fr(4));
			H.AttachDelegate("Fr5", () => Fr(5));
			H.AttachDelegate("Fr6", () => Fr(6));
			H.AttachDelegate("Fr7", () => Fr(7));
			H.AttachDelegate("Fa0", () => Tones[1].Freq[0]);
			H.AttachDelegate("Fa1", () => Tones[1].Freq[1]);
			H.AttachDelegate("Fa2", () => Tones[1].Freq[2]);
			H.AttachDelegate("Fa3", () => Tones[1].Freq[3]);
			H.AttachDelegate("Fa4", () => Tones[1].Freq[4]);
			H.AttachDelegate("Fa5", () => Tones[1].Freq[5]);
			H.AttachDelegate("Fa6", () => Tones[1].Freq[6]);
			H.AttachDelegate("Fa7", () => Tones[1].Freq[7]);
		}

		internal int Incr(int s, bool up)
		{
			if (up && 100 > Sliders[EQswitch].Slider[s])
				Sliders[EQswitch].Slider[s]++;
			else if ((!up) & 0 < Sliders[EQswitch].Slider[s])
				Sliders[EQswitch].Slider[s]--;
			return Sliders[EQswitch].Slider[s];
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

		public string Broadcast(Haptics This, int L)
		{
			string s;
			switch (L)
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
