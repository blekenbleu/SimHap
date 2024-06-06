using MathNet.Numerics.Interpolation;
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
		private List<ushort[][]> lUT = new List<ushort[][]> { };

        public List<ushort[][]> LUT { get => lUT; set => lUT = value; }

		// set a Tone harmonic amplitude for a Tone frequency
        public ushort Shape(ushort rpm, ushort[][] LUT)
		{
			int l = LUT[1].Length - 1;
            if (rpm < LUT[1][0] || rpm > LUT[1][l])
				return 0;

			int i;
            // LUT is unequally-spaced values)
            for (i = 1; i <= l; i++)
				if (rpm >= LUT[1][i])
					break;
			if (i == l)
				return LUT[0][i];

			ushort below = LUT[0][i];
			ushort above = LUT[0][i + 1];
			int step = (LUT[1][i + 1] - LUT[1][i]);
			// linear interpolation among non-linear points.
			// better would be precalculated circle centers,
			// then Bresenham’s circle drawing algorithm at run time
			return (ushort)(below + (step + 2 * (rpm - LUT[1][i]) * (above - below)) / (step * 2));
		}

		// populate a Tone's amplitudes from rpm
		// pitch is fundamental (rpm/60) or power stroke harmonic (integer * rpm * cylinders) / 120)
		public void Play(List<ushort[][]> LUT, List<Tone> A, int destination, Eq pitch)
		{
			for (int i = 0; i < A[i].Freq.Length - 2; i++)
				A[destination].Freq[i] = Shape(pitch.Slider[i], LUT[destination]);
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
	}
}
