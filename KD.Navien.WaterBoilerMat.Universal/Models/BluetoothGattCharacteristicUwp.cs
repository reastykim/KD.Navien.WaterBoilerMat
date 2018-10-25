using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
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

        public string UUID => _gattCharacteristics.Uuid.ToString();

        public string Name => _gattCharacteristics.UserDescription;

        public string Value => null;

		public List<IBluetoothGattDescriptor> GattDescriptor
		{
			get => gattDescriptor;
			private set => SetProperty(ref gattDescriptor, value);
		}
		private List<IBluetoothGattDescriptor> gattDescriptor = new List<IBluetoothGattDescriptor>();

        #endregion

        #region Fields

        private readonly GattCharacteristic _gattCharacteristics;

        #endregion

		public BluetoothGattCharacteristicUwp(GattCharacteristic gattCharacteristics)
		{
            _gattCharacteristics = gattCharacteristics;

            Initialize();
        }

		private void Initialize()
		{
            _gattCharacteristics.ValueChanged += (s, e) => ValueChanged?.Invoke(this, e.CharacteristicValue.ToBytes());
		}

		public async Task<bool> SetNotifyAsync(bool isEnable)
		{
            GattCommunicationStatus result;
            if (isEnable)
            {
                result = await _gattCharacteristics.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }
			else
            {
                result = await _gattCharacteristics.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            }

            return result == GattCommunicationStatus.Success;
        }

		public async Task<bool> WriteValueAsync(byte[] data)
		{
            var result = await _gattCharacteristics.WriteValueAsync(data.ToBuffer());
            return result == GattCommunicationStatus.Success;
		}

        public async Task<byte[]> ReadValueAsync()
        {
            var result = await _gattCharacteristics.ReadValueAsync();
            if (result.Status == GattCommunicationStatus.Success)
            {
                return result.Value.ToBytes();
            }
            else
            {
                if (result.ProtocolError != null)
                {
                    throw new Exception($"ReadValueAsync Exception=[{result.ProtocolError.GetErrorString()}]");
                }
                else
                {
                    throw new Exception($"ReadValueAsync Exception=[{result.Status}]");
                }
            }
        }
	}
}
