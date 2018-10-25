using KD.Navien.WaterBoilerMat.Models;
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

        /// <summary>
        /// Helper function to convert a UUID to a name
        /// </summary>
        /// <param name="uuid">The UUID guid.</param>
        /// <returns>Name of the UUID</returns>
        public static string ConvertUuidToName(Guid uuid)
        {
            GattNativeUuid name;

            if (Enum.TryParse(ConvertUuidToShortId(uuid).ToString(), out name))
            {
                return name.ToString();
            }

            return uuid.ToString();
        }

        /// <summary>
        /// Converts from standard 128bit UUID to the assigned 32bit UUIDs. Makes it easy to compare services
        /// that devices expose to the standard list.
        /// </summary>
        /// <param name="uuid">UUID to convert to 32 bit</param>
        /// <returns>32bit version of the input UUID</returns>
        public static ushort ConvertUuidToShortId(Guid uuid)
        {
            var bytes = uuid.ToByteArray();
            var shortUuid = (ushort)(bytes[0] | (bytes[1] << 8));

            return shortUuid;
        }
    }
}
