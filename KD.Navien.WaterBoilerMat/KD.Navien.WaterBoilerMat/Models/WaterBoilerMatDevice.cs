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
		protected const string NavienDeviceMacPrefix = "2C:E2:A8";
		protected const string BoilerGattServiceUuid = "00001c0d-d102-11e1-9b23-2ce2a80000dd";

		#region Properties

		public abstract string Name { get; }

		public abstract string Address { get; }

		public ObservableCollection<IBluetoothGattService> Services
		{
			get { return services ?? (services = new ObservableCollection<IBluetoothGattService>()); }
		}
		private ObservableCollection<IBluetoothGattService> services;

		public bool IsReadyForBoilerService
		{
			get => isReadyForBoilerService;
			protected set => SetProperty(ref isReadyForBoilerService, value);
		}
		private bool isReadyForBoilerService;

		#endregion

		public virtual Task ConnectAsync()
		{
			return Task.CompletedTask;
		}

		public static bool IsNavienDevice(string address)
		{
			return address.StartsWith(NavienDeviceMacPrefix, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}
