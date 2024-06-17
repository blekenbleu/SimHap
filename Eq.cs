using MathNet.Numerics.Interpolation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace sierses.Sim
{
	// for Settings
	public class Engine
	{
		public ushort[][] Tones;		// engine fundamental and harmonic
		public List<ushort[]> Sliders;	// graphic equalizer
		public string Theme;
	}

	// array of 6 slider values, then min, max frequencies
	public class Eq : NotifyPropertyChanged
	{	// one for each slider + 2 for min and max frequency
		internal ushort[] Slider;
	}

	public partial class Geq
	{
		internal Haptics H;	// set in Init()
		internal List<Eq> Q;	// EQ Slider array
		internal int EQswitch = 0;

		// increment EQ high-/low-pass frequency
		internal ushort[] Pincr(int s, bool up)
		{	// Slider 0, 8 are min, max frequency
			ushort bump = (ushort)(Q[EQswitch].Slider[s] >> 4);

			if (0 == bump)
				bump++;

			if (!up && Q[EQswitch].Slider[s] > bump + (0 == s ? 9  : 100))
			{
				Q[EQswitch].Slider[s] -= bump;
				H.D.LoadText = "decremented";
				// interpolation over power-of-2 LUTs
				// range >= 10 supports 8 == LUT.Length
				if (Q[EQswitch].Slider[8] <= 10 * Q[EQswitch].Slider[0])
				{
					Q[EQswitch].Slider[0] = (ushort)(0.5 + 0.1 * Q[EQswitch].Slider[8]);
					H.D.LoadText = "High pass also decremented";
				}
			}
			else if(up)
			{
				H.D.LoadText = "limit imposed";
				if (Q[EQswitch].Slider[s] < (8 == s ? 9999 : 999) - bump)
				{
					Q[EQswitch].Slider[s] += bump;
					H.D.LoadText = "incremented";
				}
				// interpolation over power-of-2 LUTs
				// range >= 10 supports 8 == LUT.Length
				if (Q[EQswitch].Slider[0] * 10 > Q[EQswitch].Slider[8])
				{
					Q[EQswitch].Slider[8] = (ushort)(10 * Q[EQswitch].Slider[0]);
					H.D.LoadText = $"Low pass set to {Q[EQswitch].Slider[8]}";
				}
			}
			EqSpline(Q[EQswitch].Slider);
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
				H.D.LoadText = "incremented";
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
				H.D.LoadText = "decremented";
			}
			else H.D.LoadText = up ? "100 is max gain" : "0 is min gain";
			EqSpline(Q[EQswitch].Slider);
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
				H.D.LoadText = "limits are >= 0 and <= 2";
				return $"{EQswitch}";
			}
			EQswitch += up ? +1 : -1;
			if (EQswitch == Q.Count || 0 == Q[EQswitch].Slider.Length || 0 == Q[EQswitch].Slider[0])
			{
				if (EQswitch == Q.Count)
					Q.Add(NewEQ());
				else Q[EQswitch] = NewEQ();
				H.D.LoadText = $"LUT {EQswitch} initialized";
			}
			else H.D.LoadText = up ? "Incremented" : "Decremented";
			H.SC.ShowEq(Q[EQswitch].Slider);
			return $"{EQswitch}";
		}

		/* an array of LUT[][s interpolated from Sliders
		 ; 36 is the lowest frequency for which
		 ; 1 <= the smallest step in 32 == LUT.Count with only an octave range
		 */
		private List<ushort[][]> lUT = new() { };	// EQ

		public List<ushort[][]> LUT { get => lUT; set => lUT = value; }

		/* EQ gains interpolated by rpm frequency harmonics
		 ; executes at 60Hz for each frequency component and equalizer LUT combination
		 ; used in EqProps.cs Publish()
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
			
			// set a amplitude for a harmonic frequency
			int i;
/* binary search increments
	16/32 (8): 24 :  8
	24/32 (4): 28 : 20 : 12 : 4
	28/32 (2): 30 : 26 : 22 : 18 : 14 : 10 : 6 : 2
	23/24 (1): (odd values 31 to 1)
 */
			// LUTs have power-of-2 length
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
			for (i = 1; i < L; i++)
				if (freq <= Lut[1][i])	// Lut interval for this Hz?
					break;

			ushort here = Lut[0][i - 1];
			short range = (short)(Lut[0][i] - here);  // gains increase or decrease
			ushort interval = (ushort)(Lut[1][i] - Lut[1][i - 1]); // monotonic frequencies

			// linearly interpolate on non-linear frequency intervals.
			return (ushort)(here + (interval + 2 * (freq - Lut[1][i - 1]) * range)
							/ (interval * 2)); 
		}	// Play()

		// convert 7 slider values
		// to  8, 13 or 32 lookup table pairs for Play()
		public ushort[][] EqSpline(ushort[] slider)
		{
			int L = slider.Length;				// should be 9; first and last are min and max frequencies
                                                // 7 slider gain values; 0 first and last entries for spline
			int N = L - 1;							// should be 8
			double[] xdata = new double[L + 1]; // linear steps for spline generation
            double[] ydata = new double[L + 1]; // corresponding gains
			int i;
			xdata[0] = 1;
			ydata[0] = 0;
			for (i = 1; i < N; i++)
			{
				xdata[i] = i + 1;			// equal steps
				ydata[i] = slider[i];		// slider values
			}
			xdata[i] = ++i;					// zero final 2 ydata[]
            xdata[i] = ++i;
            var q = CubicSpline.InterpolateAkimaSorted(xdata, ydata);

			// sample interpolation LUT with power-of-2 length from spline
			// interpolation lut[].Length depends on max - min range
			ushort min = slider[0];
			double max = slider[N];
			ushort F = 1;					// interpolation increment factor: 1, 2 or 4
			for (double range = max - min; F < 4; F <<= 1)
				if (20 > range / (8 * F))
					break;

			N *= F;							// LUT lengths
			ushort[][] lut = new ushort[2][];
			lut[0] = new ushort[N];
			lut[1] = new ushort[N];
			double ff = System.Math.Pow(max / min, 1.0 / (N - 1));
			double f = min;
			double inc = 1.0 /N;			// initial and final 0 sample offset
			double d = 1 + inc;				// first sample just after 0 initial value
			inc = 9.0 - inc;				// last sample just before 0 final value
			inc /= (N + F - 1);
			for (int j = 0; j < N; d += inc)
			{								// equally spaced interpolated values
				lut[0][j] = (ushort)(0.5 + q.Interpolate(d));
				lut[1][j++] = (ushort)(0.5 + f);
				f *= ff;					// geometric frequency progression
			}
			return lut;
		}
	}
}

