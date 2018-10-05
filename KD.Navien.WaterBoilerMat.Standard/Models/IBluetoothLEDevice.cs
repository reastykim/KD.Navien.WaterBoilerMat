using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothLEDevice
    {
		string Name { get; }

		string Address { get; }

		List<IBluetoothGattService> Services { get; }

		Task ConnectAsync();

        void Disconnect();
	}
}
