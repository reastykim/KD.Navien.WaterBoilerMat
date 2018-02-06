using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KD.Navien.WaterBoilerMat.Models;

namespace KD.Navien.WaterBoilerMat.Droid.Models
{
	public class BluetoothGattServiceAndroid : IBluetoothGattService
	{
		public string UUID => gattService.Uuid.ToString();

		public string Name => "";

		public List<IBluetoothGattCharacteristic> GattCharacteristics => throw new NotImplementedException();

		public event EventHandler GattCharacteristicsUpdated;

		private BluetoothGattService gattService;

		public BluetoothGattServiceAndroid(BluetoothGattService gattService)
		{
			this.gattService = gattService;
		}

		
	}
}