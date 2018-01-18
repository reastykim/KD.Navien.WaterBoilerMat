using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothGattService
    {
		event EventHandler GattCharacteristicsUpdated;

		string UUID { get; }
		string Name { get; }

		List<IBluetoothGattCharacteristic> GattCharacteristics { get; }
	}
}
