using KD.Navien.WaterBoilerMat.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.UWP.Models
{
	public class WaterBoilerMatDeviceUwp : WaterBoilerMatDevice
	{
		#region Properties

		public override string Name => device?.Name;
		public override string Address => device?.BluetoothAddressAsString;

		#endregion

		#region Fields

		private ObservableBluetoothLEDevice device;

		#endregion

		#region Constructors & Initialize

		public WaterBoilerMatDeviceUwp(ObservableBluetoothLEDevice device)
		{
			this.device = device;

			Initialize();
		}

		private void Initialize()
		{
			device.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
		}

		#endregion

		public override Task ConnectAsync()
		{
			return device.ConnectAsync();
		}
	}
}
