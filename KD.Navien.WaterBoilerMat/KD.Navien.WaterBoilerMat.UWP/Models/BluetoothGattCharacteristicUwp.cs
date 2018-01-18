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

		public string Value => gattCharacteristics.Value;

		public List<IBluetoothGattDescriptor> GattDescriptor
		{
			get => gattDescriptor;
			private set => SetProperty(ref gattDescriptor, value);
		}
		private List<IBluetoothGattDescriptor> gattDescriptor = new List<IBluetoothGattDescriptor>();

		public ObservableGattCharacteristics gattCharacteristics;

		public BluetoothGattCharacteristicUwp(ObservableGattCharacteristics gattCharacteristics)
		{
			this.gattCharacteristics = gattCharacteristics;

			Initialize();
		}

		public BluetoothGattCharacteristicUwp(GattCharacteristic gattCharacteristics, ObservableGattDeviceService parent)
			:this(new ObservableGattCharacteristics(gattCharacteristics, parent))
		{

		}

		private void Initialize()
		{
			gattCharacteristics.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == "Value")
				{
					Debug.WriteLine($"GattCharacteristics_ValueChanged. Value=[{Value}]");
				}

				RaisePropertyChanged(e.PropertyName);
			};

			//gattCharacteristics.Characteristic.GetDescriptorsAsync();
		}

		public Task<bool> SetNotifyAsync()
		{
			return gattCharacteristics.SetNotifyAsync();
		}

		public Task<bool> WriteValueAsync(byte[] data)
		{
			return gattCharacteristics.WriteValueAsync(data);
		}
	}
}
