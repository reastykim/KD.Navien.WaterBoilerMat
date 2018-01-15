using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothGattCharacteristic
    {
		string UUID { get; }

		string Name { get; }

		//ObservableCollection<IBluetoothGattCharacteristic> GattCharacteristics { get; }
	}
}
