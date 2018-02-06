using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat
{
    public class BluetoothLEException : Exception
    {
		public BluetoothLEException(string message, Exception innerException = null) 
			: base(message, innerException)
		{

		}
	}
}
