using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
    public enum DeviceStatus : byte
    {
        Off = 0,
        On = 2,
        LeftOnlyOn = 3,
        RightOnlyOn = 4,
    }
}
