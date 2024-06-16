using MathNet.Numerics.Interpolation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace sierses.Sim
{
	public class Tone	// array of frequency component properties
	{	// one for frequency harmonics, another for amplitudes
		internal ushort[] Freq;
	}

	// array of 6 slider values, then min, max frequencies
	public class Eq : NotifyPropertyChanged
	{	// one for each slider + 2 for min and max frequency
		internal ushort[] Slider;
	}

	public class Engine
	{
		public ushort[][] Tones;		// engine fundamental and harmonic
		public List<ushort[]> Sliders;	// graphic equalizer
		public string Theme;
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
				H.SC.InitEq(Q[EQswitch].Slider);
				H.D.LoadText = "Initialized";
			}
			else H.D.LoadText = up ? "Incremented" : "Decremented";
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
		public void AddProps(Haptics This,  ushort[][] that)
		{
			if (null != that)
			{
				if (3 <= LUT.Count)
				{
					H.D.LoadText = "already have 3 equalizers";
					return;
				}
				LUT.Add(that);
				Broadcast(This, LUT.Count);
				return;
			}
			H.D.LoadText = "null LUT";
		}
	}
}

