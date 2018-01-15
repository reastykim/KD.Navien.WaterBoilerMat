using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothGattService
    {
		string Name { get; }

		string UUID { get; }
	}
}
