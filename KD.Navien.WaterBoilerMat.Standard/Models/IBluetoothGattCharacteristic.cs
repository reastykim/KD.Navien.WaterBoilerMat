using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothGattCharacteristic
    {
		string UUID { get; }

		string Name { get; }

		List<IBluetoothGattDescriptor> GattDescriptor { get; }

		Task<bool> SetNotifyAsync(bool isEnable);

		Task<bool> WriteValueAsync(byte[] data);

        event EventHandler<string> ValueChanged;
	}
}
