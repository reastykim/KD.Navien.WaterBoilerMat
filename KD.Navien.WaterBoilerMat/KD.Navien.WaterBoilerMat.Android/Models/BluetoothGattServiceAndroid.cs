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
using Prism.Mvvm;

namespace KD.Navien.WaterBoilerMat.Droid.Models
{
	public class BluetoothGattServiceAndroid : BindableBase, IBluetoothGattService
	{
		public event EventHandler GattCharacteristicsUpdated;

		public string UUID => gattService.Uuid.ToString();

		public string Name => "";

		public WaterBoilerMatDeviceAndroid WaterBoilerMatDevice { get; private set; }

		public List<IBluetoothGattCharacteristic> GattCharacteristics
		{
			get => gattCharacteristics;
			private set => SetProperty(ref gattCharacteristics, value);
		}
		private List<IBluetoothGattCharacteristic> gattCharacteristics = new List<IBluetoothGattCharacteristic>();

		#region Fields
		
		private BluetoothGattService gattService;

		#endregion

		public BluetoothGattServiceAndroid(WaterBoilerMatDeviceAndroid waterBoilerMatDevice, BluetoothGattService gattService)
		{
			this.WaterBoilerMatDevice = waterBoilerMatDevice;
			this.gattService = gattService;

			Initialize();
		}

		private void Initialize()
		{
			GattCharacteristics.AddRange(gattService.Characteristics.Select(C => new BluetoothGattCharacteristicAndroid(this, C)));
			GattCharacteristicsUpdated?.Invoke(this, EventArgs.Empty);
		}

        public void Dispose()
        {

        }
    }
}