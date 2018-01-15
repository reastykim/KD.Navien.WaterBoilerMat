using KD.Navien.WaterBoilerMat.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.UWP.Models
{
	public class BluetoothGattCharacteristicUwp : IBluetoothGattCharacteristic
	{
		public string UUID => gattCharacteristics.UUID;

		public string Name => gattCharacteristics.Name;

		public ObservableGattCharacteristics gattCharacteristics;

		public BluetoothGattCharacteristicUwp(ObservableGattCharacteristics gattCharacteristics)
		{
			this.gattCharacteristics = gattCharacteristics;
		}
	}
}
