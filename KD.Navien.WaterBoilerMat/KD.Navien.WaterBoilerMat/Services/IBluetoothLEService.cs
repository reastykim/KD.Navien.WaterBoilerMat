using KD.Navien.WaterBoilerMat.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Services
{
    public interface IBluetoothLEService<T> where T : IBluetoothLEDevice
	{
		bool IsScanning { get; }

		Task<IEnumerable<T>> ScanAsync(int timeoutMilliseconds);
	}
}
