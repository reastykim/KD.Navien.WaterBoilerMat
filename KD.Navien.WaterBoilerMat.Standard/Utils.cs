using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KD.Navien.WaterBoilerMat
{
    public static class Utils
    {
        public static string ToMacAddress(this ulong address)
        {
            var tempMac = address.ToString("X");

            var regex = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})";
            var replace = "$1:$2:$3:$4:$5:$6";
            return Regex.Replace(tempMac, regex, replace);
        }

        public static ulong ToBluetoothAddress(this string macAddress)
        {
            macAddress = macAddress.Replace(":", "");
            return ulong.Parse(macAddress, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
		/// Converts the value of the current System.Byte [] object to its equivalent string representation using the specified format.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static string ToString(this IEnumerable<byte> bytes, string format, string separator = "")
        {
            return String.Join(separator, bytes.Select(B => B.ToString(format)));
        }
    }
}
