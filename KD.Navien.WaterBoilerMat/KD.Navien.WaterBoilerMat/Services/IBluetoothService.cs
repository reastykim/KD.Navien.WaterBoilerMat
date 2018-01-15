using KD.Navien.WaterBoilerMat.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Services
{
    public interface IBluetoothService
    {
		bool IsScanning { get; }

		Task<IEnumerable<WaterBoilerMatDevice>> ScanAsync(int timeoutMilliseconds);
	}
}
