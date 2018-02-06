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
		
		public BluetoothGattServiceAndroid GattService { get; private set; }

		private BluetoothGattCharacteristic gattCharacteristics;

		public BluetoothGattCharacteristicAndroid(BluetoothGattServiceAndroid gattService, BluetoothGattCharacteristic gattCharacteristics)
		{
			this.GattService = gattService;
			this.gattCharacteristics = gattCharacteristics;
		}

		public Task<bool> SetNotifyAsync()
		{
			var result = GattService.WaterBoilerMatDevice.SetCharacteristicNotification(gattCharacteristics, true);
			return Task.FromResult(result);
		}

		public Task<bool> WriteValueAsync(byte[] data)
		{
			var result = gattCharacteristics.SetValue(data);
			result = GattService.WaterBoilerMatDevice.WriteCharacteristic(gattCharacteristics);
			return Task.FromResult(result);
		}
	}
}