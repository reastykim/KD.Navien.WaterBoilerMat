using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Services.Protocol
{
    public class CRC8
    {
        private int crc = 0;

        public virtual string Value
        {
            get
            {
                return string.Format("{0:X4}", Convert.ToInt32(this.crc)).Substring(2, 2);
            }
        }

        public void Reset()
        {
            crc = 0;
        }

        public void Update(string input)
        {
            for (int i = 0; i < input.Length; i += 2)
            {
                crc += int.Parse(input.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            }
        }
    }
}
