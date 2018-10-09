using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothLEDevice : IDisposable
    {
		string Name { get; }

		string Address { get; }

		List<IBluetoothGattService> Services { get; }

        Task<string> ConnectAsync(string uniqueID);

        void Disconnect();
	}
}
