using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public abstract class WaterBoilerMatDevice : BindableBase
	{
		const string NavienDeviceMacPrefix = "2C:E2:A8";

		public abstract string Name { get; }

		public abstract string Address { get; }

		public abstract ObservableCollection<IBluetoothGattService> Services { get; }

		public static bool IsNavienDevice(string address)
		{
			return address.StartsWith(NavienDeviceMacPrefix, StringComparison.CurrentCultureIgnoreCase);
		}

		public virtual Task ConnectAsync()
		{
			return Task.CompletedTask;
		}

		//public virtual Task<IEnumerable<IBluetoothGattService>> GetBluetoothGattServiceAsync()
		//{
		//	return Task.FromResult(Enumerable.Empty<IBluetoothGattService>());
		//}
	}
}
