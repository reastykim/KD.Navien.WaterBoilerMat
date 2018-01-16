using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.UWP.Extensions;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

		public override Task<IEnumerable<IBluetoothGattService>> GetGattServicesAsync()
		{
			return device.GetGattServicesAsync();
		}

		#endregion

		public override Task ConnectAsync()
		{
			return device.ConnectAsync();
		}
	}
}
