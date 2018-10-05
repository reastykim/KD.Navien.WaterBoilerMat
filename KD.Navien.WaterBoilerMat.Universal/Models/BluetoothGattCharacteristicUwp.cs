using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
using Microsoft.Toolkit.Uwp.Connectivity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
	public class BluetoothGattCharacteristicUwp : BindableBase, IBluetoothGattCharacteristic
	{
        #region Events

        public event EventHandler<byte[]> ValueChanged;

        #endregion

        #region Properties

        public string UUID => gattCharacteristics.UUID;

		public string Name => gattCharacteristics.Name;

		public string Value => gattCharacteristics.Value;

		public List<IBluetoothGattDescriptor> GattDescriptor
		{
			get => gattDescriptor;
			private set => SetProperty(ref gattDescriptor, value);
		}
		private List<IBluetoothGattDescriptor> gattDescriptor = new List<IBluetoothGattDescriptor>();

        #endregion

        #region Fields

        private ObservableGattCharacteristics gattCharacteristics;

        #endregion

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
            gattCharacteristics.Characteristic.ValueChanged += (s, e) => ValueChanged?.Invoke(this, e.CharacteristicValue.ToBytes());
            gattCharacteristics.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
		}

		public Task<bool> SetNotifyAsync(bool isEnable)
		{
            if (isEnable)
            {
                return gattCharacteristics.SetNotifyAsync();
            }
			else
            {
                return gattCharacteristics.StopNotifyAsync();
            }
		}

		public Task<bool> WriteValueAsync(byte[] data)
		{
			return gattCharacteristics.WriteValueAsync(data);
		}
	}
}
