using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.UWP.Extensions;
using Microsoft.Toolkit.Uwp.Connectivity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace KD.Navien.WaterBoilerMat.UWP.Models
{
	public class BluetoothGattCharacteristicUwp : BindableBase, IBluetoothGattCharacteristic
	{
		public string UUID => gattCharacteristics.UUID;

		public string Name => gattCharacteristics.Name;

		public string Value
		{
			get => _value;
			private set => SetProperty(ref _value, value);
		}
		private string _value;

		public ObservableGattCharacteristics gattCharacteristics;

		public BluetoothGattCharacteristicUwp(ObservableGattCharacteristics gattCharacteristics)
		{
			this.gattCharacteristics.PropertyChanged += GattCharacteristics_PropertyChanged;
		}
		public BluetoothGattCharacteristicUwp(GattCharacteristic gattCharacteristics, ObservableGattDeviceService parent)
			:this(new ObservableGattCharacteristics(gattCharacteristics, parent))
		{

		}

		public Task<bool> SetNotifyAsync()
		{
			return gattCharacteristics.SetNotifyAsync();
		}

		public Task<bool> WriteValueAsync(byte[] data)
		{
			return gattCharacteristics.WriteValueAsync(data);
		}

		private void GattCharacteristics_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Value")
			{
				Value = gattCharacteristics.Value;
				Debug.WriteLine($"GattCharacteristics_ValueChanged. Value=[{Value}]");
			}
		}
	}
}
