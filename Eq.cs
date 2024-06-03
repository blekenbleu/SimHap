namespace sierses.Sim
{
	public class Tone
	{
		ushort Fundamental;
		ushort[7] Harmonic;
	}

	public class Eq : NotifyPropertyChanged
    {
		List<Tone>;
	}

	public class Eq
    {
      List<ushort, ushort, Tones> { ushort min, ushort max, Tone slider }
    }
}
