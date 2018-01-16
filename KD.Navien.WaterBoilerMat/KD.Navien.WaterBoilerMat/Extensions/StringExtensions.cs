using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Extensions
{
    public static class StringExtensions
    {
		public static byte[] HexStringToByteArray(this string hex)
		{
			return Enumerable.Range(0, hex.Length)
							 .Where(x => x % 2 == 0)
							 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
							 .ToArray();
		}
	}
}
