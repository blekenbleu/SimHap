using MathNet.Numerics.Interpolation;
using SimHub.Plugins;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace sierses.Sim
{
	public partial class Geq
	{
		internal void Init(Settings Settings, Haptics h)
		{
			H = h;
  			if (null == Settings.Engine || null == Settings.Engine.Tones
			 || null == Settings.Engine.Tones[1].Freq || 0 == Settings.Engine.Tones[1].Freq.Count)
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
  			else Tones = Settings.Engine.Tones;

			if (null != Settings.Engine && null != Settings.Engine.Sliders)
				Q = Settings.Engine.Sliders;
            if (1 > Q.Count || 9 != Q[0].Slider.Count)
			{
				ushort s = 50, highpass = 20, lowpass = 900;
				ObservableCollection<ushort> S = new() { highpass, s, s, s, s, s, s, s, lowpass };
				if (1 > Q.Count)
                	Q.Add(new Eq() { Slider = S });
				else Q[0] = new Eq() { Slider = S };
			}
//			H.SC.Init(Q[0].Slider);
			H.AttachDelegate("Fr0", () => Fr(0));
			H.AttachDelegate("Fr1", () => Fr(1));
			H.AttachDelegate("Fr2", () => Fr(2));
			H.AttachDelegate("Fr3", () => Fr(3));
			H.AttachDelegate("Fr4", () => Fr(4));
			H.AttachDelegate("Fr5", () => Fr(5));
			H.AttachDelegate("Fr6", () => Fr(6));
			H.AttachDelegate("Fr7", () => Fr(7));
			H.AttachDelegate("Fa0", () => Tones[1].Freq[0]);
			H.AttachDelegate("Fa1", () => Tones[1].Freq[1]);
			H.AttachDelegate("Fa2", () => Tones[1].Freq[2]);
			H.AttachDelegate("Fa3", () => Tones[1].Freq[3]);
			H.AttachDelegate("Fa4", () => Tones[1].Freq[4]);
			H.AttachDelegate("Fa5", () => Tones[1].Freq[5]);
			H.AttachDelegate("Fa6", () => Tones[1].Freq[6]);
			H.AttachDelegate("Fa7", () => Tones[1].Freq[7]);
		}

		public string Broadcast(Haptics This, int L)
		{
			string s;
			switch (L)
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
