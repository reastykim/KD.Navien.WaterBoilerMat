using KD.Navien.WaterBoilerMat.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Services
{
    public interface IBluetoothService
    {
		Task<IEnumerable<IBluetoothDevice>> ScanAsync(int timeoutMilliseconds);
	}
}
