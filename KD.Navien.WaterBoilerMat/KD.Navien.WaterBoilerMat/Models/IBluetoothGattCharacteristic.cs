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

		Task<bool> SetNotifyAsync();

		Task<bool> WriteValueAsync(byte[] data);
	}
}
