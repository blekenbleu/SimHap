using MathNet.Numerics.Interpolation;
using SimHub.Plugins;
using System.Collections.Generic;

namespace sierses.Sim
{
    public class Tone	// array of frequency component properties
	{
		internal ushort[] Freq = new ushort[6];	// one for frequency harmonics, another for amplitudes
	}

	// array of 6 slider values, then min, max frequencies
	public class Eq : NotifyPropertyChanged
	{
		internal ushort[] Slider = new ushort[8];	// one for each Tone.Freq + 2 for min and max
	}

	public class Geq
	{
		// an array of sliders for each equalizer
		internal List<Eq> Sliders = new();

		// an array of LUT[][s interpolated from Sliders
		private List<ushort[][]> lUT = new() { };

		internal Tone[] Tones = new Tone[2];	// frequency harmonic and amplitude

        public List<ushort[][]> LUT { get => lUT; set => lUT = value; }

		// set a Tone harmonic amplitude for a Tone frequency
		// Shape() depends on Play() to mute out-of-band frequencies
        public ushort Shape(int hz, ushort[][] Lut)
		{
            int l = Lut[1].Length - 1;
			int i;

            // Lut has power-spaced values)
            for (i = 1; i <= l; i++)
				if (hz >= Lut[1][i])	// Lut interval for this hz?
					break;

			ushort here = Lut[0][i];
			ushort next = Lut[0][i + 1];
			int interval = (Lut[1][i + 1] - Lut[1][i]);

			// linear interpolation among non-linear points.
			// better would be precalculated circle centers,
			// then Bresenham’s circle drawing algorithm at run time
			return (ushort)(here + (interval + 2 * (hz - Lut[1][i]) * (next - here)) / (interval * 2));
		}

		// populate a Tone's amplitudes from rpm
		// pitch is fundamental (rpm/60) or power stroke harmonic (integer * rpm * cylinders) / 120)
		public ushort Play(Haptics This, byte destination)
		{
			byte harmonic = (byte)(destination & 7);
			destination >>= 3;
			int rate = harmonic * This.D.Rpms;
			int l = LUT[1].Length - 1;
			ushort freq;

			if (0 < harmonic)
				freq = (ushort)((60 + rate * This.S.Car.cyl)/120);
			else freq = (ushort)((30 + rate) / 60);
			if (freq < LUT[destination][1][0] || freq >= LUT[destination][1][l])
				return 0;
			//				amplitude							harmonic factor           
			return (ushort)(Tones[1].Freq[harmonic] * Shape(freq * Tones[0].Freq[harmonic], LUT[destination]));
		}

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
