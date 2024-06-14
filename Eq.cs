using MathNet.Numerics.Interpolation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace sierses.Sim
{
	public class Tone	// array of frequency component properties
	{	// one for frequency harmonics, another for amplitudes
		internal ObservableCollection<ushort> Freq = new();
	}

	// array of 6 slider values, then min, max frequencies
	public class Eq : NotifyPropertyChanged
	{	// one for each slider + 2 for min and max frequency
		internal ushort[] Slider = new ushort[9];
	}

	public class Engine
	{
		public Tone[] Tones;						// engine fundamental and harmonic
		public ObservableCollection<Eq> Sliders;	// graphic equalizer
		public string Theme;
	}

	public partial class Geq
	{
		internal Haptics H;	// set in Init()
		internal ObservableCollection<Eq> Q = new();	// EQ Slider array
		internal string Feedback;
		internal int EQswitch = 0;

		// increment EQ high-/low-pass frequency
		internal ushort[] Pincr(int s, bool up)
		{	// Slider 0, 8 are min, max frequency
			if (!up && Q[EQswitch].Slider[s] > (0 == s ? 10 : 100))
			{
				Q[EQswitch].Slider[s]--;
				Feedback = "decremented";
				// interpolation over power-of-2 LUTs
				// range >= 10 supports 8 == LUT.Length
				if (Q[EQswitch].Slider[8] <= 10 * Q[EQswitch].Slider[0]
				 && 10 < Q[EQswitch].Slider[0])
				{
					Q[EQswitch].Slider[0]--;
					Feedback = "High pass also decremented";
				}
				else
				{
					Q[EQswitch].Slider[8] =	(ushort)(10 * Q[EQswitch].Slider[0]);
					Feedback = $"Low pass set to {Q[EQswitch].Slider[8]}";
				}
			}
			else if(up)
			{
				Feedback = "limit imposed";
				if (Q[EQswitch].Slider[s] < (8 == s ? 10000 : 1000))
				{
					Q[EQswitch].Slider[s]++;
					Feedback = "incremented";
				}
				// interpolation over power-of-2 LUTs
				// range >= 10 supports 8 == LUT.Length
				if (0 == s && (Q[EQswitch].Slider[0] * 10) > Q[EQswitch].Slider[8])
				{
					Q[EQswitch].Slider[8] = (ushort)(10 * Q[EQswitch].Slider[0]);
					Feedback = $"Low pass set to {Q[EQswitch].Slider[8]}";
				}
			}
			return Q[EQswitch].Slider;
		}

		// increment some EQ gain
		internal ushort[] Incr(int s, bool up)
		{	// Slider 0, 8 are min, max frequency
			int end = Q[EQswitch].Slider.Length - 1;
			int sum = 0;

			if (up && 100 > Q[EQswitch].Slider[s])
			{	// EQ gains are Slider[1-7]
				for (int i = 1; i < end; i++)
				{
					if (i == s)
						continue;
				 	if (0 < Q[EQswitch].Slider[i])
						Q[EQswitch].Slider[i]--;
					sum += Q[EQswitch].Slider[i];
				}
				Q[EQswitch].Slider[s] = (ushort)((350 < sum)
						? 0 : (sum < 250) ? 100 : (350 - sum));
				Feedback = "incremented";
			}
			else if ((!up) & 0 < Q[EQswitch].Slider[s])
			{
				for (int i = 1; i < end; i++)
				{
					if (i == s)
					continue;
 					if (100 > Q[EQswitch].Slider[i])
						Q[EQswitch].Slider[i]++;
					sum += Q[EQswitch].Slider[i];
				}
				
				Q[EQswitch].Slider[s] = (ushort)((350 < sum)
						? 0 : (sum < 250) ? 100 : (350 - sum));
				Feedback = "decremented";
			}
			else Feedback = up ? "100 is max gain" : "0 is min gain";
			return Q[EQswitch].Slider;
		}

		private Eq NewEQ()
		{
			ushort s = 50, highpass = 20, lowpass = 900;
			ushort[] S = { highpass, s, s, s, s, s, s, s, lowpass };
			return new Eq() { Slider = S };
		}

		// select another EQ Slider set
		internal string NextUp(bool up)
		{
			if ((up && 2 == EQswitch) || (0 == EQswitch && !up))
			{
				Feedback = "limits are >= 0 and <= 2";
				return $"{EQswitch}";
			}
			EQswitch += up ? +1 : -1;
			if (EQswitch == Q.Count || 0 == Q[EQswitch].Slider.Length || 0 == Q[EQswitch].Slider[0])
			{
				if (EQswitch == Q.Count)
					Q.Add(NewEQ());
				else Q[EQswitch] = NewEQ();
				H.SC.InitEq(Q[EQswitch].Slider);
				Feedback = "Initialized";
			}
			else Feedback = up ? "Incremented" : "Decremented";
			return $"{EQswitch}";
		}

		public Tone[] Tones = new Tone[2];			// engine frequency harmonic, amplitude

		/* an array of LUT[][s interpolated from Sliders
		 ; 36 is the lowest frequency for which
		 ; 1 <= the smallest step in 32 == LUT.Count with only an octave range
		 */
		private List<ushort[][]> lUT = new() { };	// EQ

		public List<ushort[][]> LUT { get => lUT; set => lUT = value; }

		/* EQ gains interpolated by rpm frequency harmonics
		 ; pitch is fundamental (rpm/60)
		 ; or power stroke harmonic: (integer * rpm * cylinders) / 120)
		 ; piecewise linear interpolation wants power-of-2 LUT.Length
		 ; 10 <= max/min frequency ratio supports LUT.Length = 8
		 */
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
	23/24 (1): (odd values 31 to 1)
 */
			// Luts have power-of-2 length
			// Lut[0] has harmonic scaling values;
			// Lut[1] has power-spaced frequency values
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
			ushort range = (ushort)(Lut[0][i] - here);
			int interval = (Lut[1][i] - Lut[1][i - 1]);

			return (ushort)(Tones[0].Freq[harmonic]	// amplitude
					// linearly interpolate on non-linear frequency intervals.
					* (here + (interval + 2 * (freq - Lut[1][i]) * range)
							/ (interval * 2))); 
		}	// Play()

		// convert 7 slider values
		// to (Eq.Slider.Length - 2) lookup table pairs for Play()
		public ushort[][] EqSpline(ushort[] slider)
		{
			// slider.Length should be 9
			int L = slider.Length;
			double[] xdata = new double[L];
			// ydata has slider values; 0 first and last entries
			double[] ydata = new double[L];
			xdata[0] = 1;	// frequency step
			ydata[0] = 0;	// corresponding gain
			for (int i = 1; i < L; i++)
			{
				xdata[i] = i;			// equal increments
				ydata[i] = slider[i];
			}
			L--;
			ydata[L] = 0;
			var q = CubicSpline.InterpolateAkimaSorted(xdata, ydata);

			// now, generate an interpolation LUT with power-of-2 length
			ushort min = slider[0];
			ushort max = slider[L];
			ushort F;			// interpolation increment factor: 1, 2 or 4

			// interpolation lut[].Length depends on min value and max/min range 
			double ff = System.Math.Log10(((double)max) / min);
			double f = min;
			ushort[][] lut = new ushort[2][];

			// find largest F for which lut[1][1] > lut[1][0] by at least 1
			for (F = 1; 4 >= F && min < (ushort)((f * ff) / (F * L)); F <<= 1);

			lut[0] = new ushort[F*L];
			lut[1] = new ushort[F*L];
			double inc = 1.0;
			inc /= F;
			for (int i = 0; i < F*L; i++)
			{	// equally spaced interpolated values
				lut[0][i] = (ushort)(0.5 + q.Interpolate(1 + inc * i));
				lut[1][i] = (ushort)(f);
				f *= ff;			// geometric frequency progression
			}
			return lut;
		}

		// for properties
		public ushort Fr(byte i)
		{
			int f = Tones[0].Freq[i] * H.D.Rpms;
			return (ushort)((0 == i) ? ((30 + f) / 60)
									 : ((60 + f * H.S.Car.cyl) / 120));
		}

		// AddProps() should be called by UI to add equalizer instances,
		// which are Tone components played thru Play() using that LUT
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
