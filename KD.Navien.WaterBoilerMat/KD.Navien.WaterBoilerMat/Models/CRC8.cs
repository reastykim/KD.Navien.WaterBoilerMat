﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
	public class CRC8
	{
		private int crc = 0;

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

		public string GetValue()
		{
			return crc.ToString("X4").Substring(2, 2);
		}
	}
}
