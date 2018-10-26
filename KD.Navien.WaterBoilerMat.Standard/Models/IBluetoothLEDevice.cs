using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothLEDevice
    {
        string Id { get; }

		string Name { get; }

		string Address { get; }

		List<IBluetoothGattService> Services { get; }
	}
}
