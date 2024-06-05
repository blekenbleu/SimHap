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
	}
}
