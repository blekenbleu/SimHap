using MathNet.Numerics.Interpolation;
using SimHub.Plugins;
using System.Collections.Generic;

namespace sierses.Sim
{
    public class Tone	// array of frequency component properties
	{
		internal ushort[] Freq;	// one for frequency values, another for amplitudes
	}

	// array of 6 slider values, then min, max frequencies
	public class Eq : NotifyPropertyChanged
	{
		internal ushort[] Slider = new ushort[8];
	}

	public class Geq
	{
		// an array of sliders for each equalizer
		internal List<Eq> Sliders = new();

		// an array of LUT[][s interpolated from Sliders
		private List<ushort[][]> lUT = new() { };

        public List<ushort[][]> LUT { get => lUT; set => lUT = value; }

		// set a Tone harmonic amplitude for a Tone frequency
        public ushort Shape(int rpm, int amplitude, ushort[][] Lut)
		{
            int l = Lut[1].Length - 1;
            if (rpm < Lut[1][0] || rpm > Lut[1][l])
				return 0;

			int i;
            // Lut is unequally-spaced values)
            for (i = 1; i <= l; i++)
				if (rpm >= Lut[1][i])
					break;
			if (i == l)
				return Lut[0][i];

			ushort below = Lut[0][i];
			ushort above = Lut[0][i + 1];
			int step = (Lut[1][i + 1] - Lut[1][i]);
			// linear interpolation among non-linear points.
			// better would be precalculated circle centers,
			// then Bresenham’s circle drawing algorithm at run time
			return (ushort)(amplitude * (below + (step + 2 * (rpm - Lut[1][i]) * (above - below)) / (step * 2)));
		}

		// populate a Tone's amplitudes from rpm
		// pitch is fundamental (rpm/60) or power stroke harmonic (integer * rpm * cylinders) / 120)
		public ushort Play(Haptics This, int destination, int pitch)
		{
			int rate = pitch * This.D.Rpms;
			int l = LUT[1].Length - 1;
			ushort freq;

			if (0 < pitch)
				freq = (ushort)((60 + rate * This.S.Car.cyl)/120);
			else freq = (ushort)((30 + rate) / 60);
			if (freq < LUT[destination][1][0] || freq >= LUT[destination][1][l])
				return 0;
			return Shape(freq, Tones[destination].Freq[pitch], LUT[destination]);
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

		public List<Tone> Tones;
		public string AddProps(Haptics This, Tone that)
		{
			Tones.Add(that);
			string s;
			switch (Tones.Count)
			{
				case 1:
					This.AttachDelegate("Eq0.0", () => Play(This, 0, 0));
					This.AttachDelegate("Eq0.1", () => Play(This, 0, 1));
					This.AttachDelegate("Eq0.2", () => Play(This, 0, 2));
					This.AttachDelegate("Eq0.3", () => Play(This, 0, 3));
					This.AttachDelegate("Eq0.4", () => Play(This, 0, 4));
					This.AttachDelegate("Eq0.5", () => Play(This, 0, 5));
					This.AttachDelegate("Eq0.6", () => Play(This, 0, 6));
					This.AttachDelegate("Eq0.7", () => Play(This, 0, 7));
					s = "Eq0 added";
					break;
				case 1:
					This.AttachDelegate("Eq1.0", () => Play(This, 1, 0));
					This.AttachDelegate("Eq1.1", () => Play(This, 1, 1));
					This.AttachDelegate("Eq1.2", () => Play(This, 1, 2));
					This.AttachDelegate("Eq1.3", () => Play(This, 1, 3));
					This.AttachDelegate("Eq1.4", () => Play(This, 1, 4));
					This.AttachDelegate("Eq1.5", () => Play(This, 1, 5));
					This.AttachDelegate("Eq1.6", () => Play(This, 1, 6));
					This.AttachDelegate("Eq1.7", () => Play(This, 1, 7));
					s = "Eq1 added";
					break;
				case 1:
					This.AttachDelegate("Eq2.0", () => Play(This, 2, 0));
					This.AttachDelegate("Eq2.1", () => Play(This, 2, 1));
					This.AttachDelegate("Eq2.2", () => Play(This, 2, 2));
					This.AttachDelegate("Eq2.3", () => Play(This, 2, 3));
					This.AttachDelegate("Eq2.4", () => Play(This, 2, 4));
					This.AttachDelegate("Eq2.5", () => Play(This, 2, 5));
					This.AttachDelegate("Eq2.6", () => Play(This, 2, 6));
					This.AttachDelegate("Eq2.7", () => Play(This, 2, 7));
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
