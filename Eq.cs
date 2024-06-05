using System.Collections.Generic;

namespace sierses.Sim
{
	public class Tone
	{
		internal ushort[] Freq;
	}

	public class Eq : NotifyPropertyChanged
	{
		internal Tone Tone;
        internal ushort Min; internal ushort Max;
    }

	public class Geq
	{
		public ushort Equalizer(ushort rpm, ushort[][] LUT)
	
		if (rpm < LUT[0] || rpm > LUT[1])
			return 0;

		// LUT is unequally-spaced values)
		for (int i = 2; i < LUT[1].Length; i++
			if (rpm >= LUT[1][i])
				break;
		if (i == (LUT[1].Length - 1))
			return LUT[0][i];

		ushort range = LUT[1][0] - LUT[0][0], l = LUT[0].Length - 1;
		ushort below = LUT[0][i];
		ushort above = LUT[0][i + 1];
		ushort step = LUT[1][i + 1] - LUT[1][i];
		// linear interpolation among non=linear points.
		// better would be precalculated circle centers,
		// then use Bresenham’s circle drawing algorithm at run time
		return below + (step + 2 * (rpm - LUT[1][i]) * (above - below))
						/  (step * 2);
	}
}
