using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothGattDescriptor
    {
		Task<byte[]> ReadValueAsync();

		Task<bool> WriteValueAsync(byte[] value);

		string UUID { get; }
	}
}
