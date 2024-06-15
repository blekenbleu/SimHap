using SimHub.Plugins;

namespace sierses.Sim
{
	public partial class Geq
	{
		internal void Init(Engine Engine, Haptics h)
		{
			H = h;
  			if (null == Engine || null == Engine.Tones
			 || null == Engine.Tones[1].Freq || 0 == Engine.Tones[1].Freq.Count)
  			{
				Tones[0] = new();
				Tones[0].Freq.Add(1);	// engine RPM / 60
				Tones[0].Freq.Add(1);	// power stroke: cylinders * engine RPM / 120
				Tones[0].Freq.Add(3);	// first power stroke harmonic -default to odd
				Tones[0].Freq.Add(5);
				Tones[0].Freq.Add(7);
				Tones[0].Freq.Add(9);
				Tones[0].Freq.Add(11);
				Tones[0].Freq.Add(13);	// sixth power stroke harmonic
				Tones[1] = new();		// harmonic amplitudes
				Tones[1].Freq.Add(1000);
				Tones[1].Freq.Add(1000);
				Tones[1].Freq.Add(333);	// default to square wave
				Tones[1].Freq.Add(111);
				Tones[1].Freq.Add(37);
				Tones[1].Freq.Add(12);
				Tones[1].Freq.Add(4);
				Tones[1].Freq.Add(1);
  			}
  			else Tones = Engine.Tones;
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

			if (null != Engine && null != Engine.Sliders)
				Q = Engine.Sliders;
			if (1 > Q.Count || 9 != Q[0].Slider.Length || 0 == Q[0].Slider[0])
			{
				if (1 > Q.Count)
					Q.Add(NewEQ());
				else Q[0] = NewEQ();
			}
			AddProps(H, EqSpline(Q[0].Slider));
		}

		public void Broadcast(Haptics This, int L)
		{
			string s;
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
					s = "E.Q0 added";
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
					s = "E.Q1 added";
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
					s = "E.Q2 added; max equalizer supported...";
					break;
				default:
					s = "equalizer limit exceeded";
					break;
			}
			H.D.LoadText = s;
		}
	}
}
