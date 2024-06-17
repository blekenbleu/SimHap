﻿using System;
using System.Windows;

namespace sierses.Sim
{
    public partial class Geq
    {
		internal ushort UTfreq = 0, UTcount = 0;
        public void UnitTest()
        {
			if (40 > UTcount++)
				return;
			UTcount = 0;
            // D.Refresh() updates D.Rpms when GameRunning
            H.D.Rpms += (ushort)((Q[EQswitch].Slider[8] - Q[EQswitch].Slider[0]) >> 1);
            if ((60 * Q[EQswitch].Slider[8]) < H.D.Rpms)
				H.D.Rpms = (ushort)(0.5 + Q[EQswitch].Slider[0] * 60);
        }
    }
}
