using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KD.Navien.WaterBoilerMat.Models;
using Prism.Mvvm;

namespace KD.Navien.WaterBoilerMat.Droid.Models
{
	public class BluetoothGattCharacteristicAndroid : BindableBase, IBluetoothGattCharacteristic
	{
		public string UUID => gattCharacteristics.Uuid.ToString();

		public string Name => "";

		public List<IBluetoothGattDescriptor> GattDescriptor { get; }

		

		private BluetoothGattCharacteristic gattCharacteristics;

		public BluetoothGattCharacteristicAndroid(BluetoothGattCharacteristic gattCharacteristics)
		{
			this.gattCharacteristics = gattCharacteristics;
		}

		public Task<bool> SetNotifyAsync()
		{
			return Task.FromResult(true);
		}

		public Task<bool> WriteValueAsync(byte[] data)
		{
			return Task.FromResult(true);
		}
	}
}