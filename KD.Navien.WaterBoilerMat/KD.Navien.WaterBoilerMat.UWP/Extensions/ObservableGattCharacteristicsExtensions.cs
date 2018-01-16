using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.UWP.Extensions
{
	public static class ObservableGattCharacteristicsExtensions
	{
		public static Task<bool> WriteValueAsync(this ObservableGattCharacteristics characteristics, byte[] data)
		{
			return characteristics.Characteristic.WriteValueAsync(data.ToBuffer()).AsTask()
											     .ContinueWith(T => T.Result == Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success);
		}
	}
}
