using KD.Navien.WaterBoilerMat.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace KD.Navien.WaterBoilerMat.UWP.Models
{
	public class BluetoothLEDeviceDisplay : KD.Navien.WaterBoilerMat.Models.IBluetoothDevice
	{
		public string Name => device?.Name;
		public string Address => device?.BluetoothAddressAsString;



		private ObservableBluetoothLEDevice device;

		public BluetoothLEDeviceDisplay(ObservableBluetoothLEDevice device)
		{
			this.device = device;
		}
	}
}
