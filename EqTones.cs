using SimHub.Plugins;

namespace sierses.Sim
{
	public class Tone	// array of frequency component properties
	{	// one for frequency harmonics, another for amplitudes
		internal ushort[] Freq;
	}

	public partial class Geq
	{
		public Tone[] Tones;			// engine frequency harmonic, amplitude

		public void Publish(Haptics This, int L)
		{
			switch (L)
			{
				case 1:
					This.AttachDelegate("E.Q0.0", () => Play(This, 0));
					This.AttachDelegate("E.Q0.1", () => Play(This, 1));
					This.AttachDelegate("E.Q0.2", () => Play(This, 2));
					This.AttachDelegate("E.Q0.3", () => Play(This, 3));
					This.AttachDelegate("E.Q0.4", () => Play(This, 4));
					This.AttachDelegate("E.Q0.5", () => Play(This, 5));
					This.AttachDelegate("E.Q0.6", () => Play(This, 6));
					This.AttachDelegate("E.Q0.7", () => Play(This, 7));
					H.D.LoadText += ";  E.Q0 added";
					break;
				case 2:
					This.AttachDelegate("E.Q1.0", () => Play(This, 8));
					This.AttachDelegate("E.Q1.1", () => Play(This, 9));
					This.AttachDelegate("E.Q1.2", () => Play(This, 10));
					This.AttachDelegate("E.Q1.3", () => Play(This, 11));
					This.AttachDelegate("E.Q1.4", () => Play(This, 12));
					This.AttachDelegate("E.Q1.5", () => Play(This, 13));
					This.AttachDelegate("E.Q1.6", () => Play(This, 14));
					This.AttachDelegate("E.Q1.7", () => Play(This, 15));
					break;
				case 3:
					This.AttachDelegate("E.Q2.0", () => Play(This, 16));
					This.AttachDelegate("E.Q2.1", () => Play(This, 17));
					This.AttachDelegate("E.Q2.2", () => Play(This, 18));
					This.AttachDelegate("E.Q2.3", () => Play(This, 19));
					This.AttachDelegate("E.Q2.4", () => Play(This, 20));
					This.AttachDelegate("E.Q2.5", () => Play(This, 21));
					This.AttachDelegate("E.Q2.6", () => Play(This, 22));
					This.AttachDelegate("E.Q2.7", () => Play(This, 23));
					break;
				default:
					break;
			}
		}	// Publish()

		// AddProps() is called by UI to add equalizer LUT instances,
		// which are frequency component gains from Play() using those LUTs
		// e.g. AddProps(This, EqSpline(Q[i].Slider));
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
				Publish(This, LUT.Count);
				return;
			}
			H.D.LoadText = "null LUT";
		}

		// for Fr[0-7] properties in Init()
		public ushort Fr(byte i)
		{
			int f = Tones[0].Freq[i] * H.D.Rpms;
			return (ushort)((0 == i) ? ((30 + f) / 60)					// RPM Hz
									 : ((60 + f * H.S.Car.cyl) / 120));	// power stroke harmonic Hz
		}

		internal void Init(Engine Engine, Haptics h)
		{
			H = h;
			Tones = new Tone[2];
			if (null == Engine || null == Engine.Tones || 2 > Engine.Tones.Length
				|| null == Engine.Tones[1] || 8 != Engine.Tones[1].Length)
			{
				Tones[0] = new()
				{
					Freq = new ushort[8] {	1,		// engine RPM / 60
											1,		// power stroke: cylinders * engine RPM / 120
											3,		// power stroke 1st harmonic, for square wave
											5, 7, 9, 11,
											13 }	// power stroke 6th harmonic
				};
				Tones[1] = new() { Freq = new ushort[8] {		// harmonic amplitudes
									1000, 1000,
									333, 111, 37, 12, 4, 1 }	// square wave
				};
			} else {
				Tones[0] = new() { Freq = Engine.Tones[0] };
				Tones[1] = new() { Freq = Engine.Tones[1] };
			}

			H.AttachDelegate("E.Fr0", () => Fr(0));
			H.AttachDelegate("E.Fr1", () => Fr(1));
			H.AttachDelegate("E.Fr2", () => Fr(2));
			H.AttachDelegate("E.Fr3", () => Fr(3));
			H.AttachDelegate("E.Fr4", () => Fr(4));
			H.AttachDelegate("E.Fr5", () => Fr(5));
			H.AttachDelegate("E.Fr6", () => Fr(6));
			H.AttachDelegate("E.Fr7", () => Fr(7));
			H.AttachDelegate("E.Fa0", () => Tones[1].Freq[0]);
			H.AttachDelegate("E.Fa1", () => Tones[1].Freq[1]);
			H.AttachDelegate("E.Fa2", () => Tones[1].Freq[2]);
			H.AttachDelegate("E.Fa3", () => Tones[1].Freq[3]);
			H.AttachDelegate("E.Fa4", () => Tones[1].Freq[4]);
			H.AttachDelegate("E.Fa5", () => Tones[1].Freq[5]);
			H.AttachDelegate("E.Fa6", () => Tones[1].Freq[6]);
			H.AttachDelegate("E.Fa7", () => Tones[1].Freq[7]);

			Q = new();
			if (null == Engine | null == Engine.Sliders || 1 > Engine.Sliders.Count)
			{
				Q.Add(NewEQ());
				AddProps(H, EqSpline(Q[0].Slider));
			} else for (int i = 0; i < Engine.Sliders.Count; i++)
			{
				Q.Add(new() { Slider = Engine.Sliders[i] });
				AddProps(H, EqSpline(Q[i].Slider));
			}
		}	// Init()
	}
}