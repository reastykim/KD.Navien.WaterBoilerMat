using KD.Navien.WaterBoilerMat.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.UWP.Models
{
	public class BluetoothGattServiceUwp : IBluetoothGattService
	{
		public string Name => gattDeviceService.Name;

		public string UUID => gattDeviceService.UUID;



		private ObservableGattDeviceService gattDeviceService;

		public BluetoothGattServiceUwp(ObservableGattDeviceService gattDeviceService)
		{
			this.gattDeviceService = gattDeviceService;
		}
	}
}
