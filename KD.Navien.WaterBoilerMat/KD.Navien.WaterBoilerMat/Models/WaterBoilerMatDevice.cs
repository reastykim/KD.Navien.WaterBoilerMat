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
		protected const string BoilerGattCharacteristic1Uuid = "00001c0d-d102-11e1-9b23-2ce2a80100dd";
		protected const string BoilerGattCharacteristic2Uuid = "00001c0d-d102-11e1-9b23-2ce2a80200dd";

		public event EventHandler<bool> IsReadyForBoilerServiceChanged;

		#region Properties

		public abstract string Name { get; }

		public abstract string Address { get; }

		public ObservableCollection<IBluetoothGattService> Services
		{
			get { return services ?? (services = new ObservableCollection<IBluetoothGattService>()); }
		}
		private ObservableCollection<IBluetoothGattService> services;

		public IBluetoothGattService BoilerGattService
		{
			get => Services.FirstOrDefault(S => S.UUID == BoilerGattServiceUuid);
		}

		public IBluetoothGattCharacteristic BoilerGattCharacteristic1
		{
			get => BoilerGattService?.GattCharacteristics.FirstOrDefault(C => C.UUID == BoilerGattCharacteristic1Uuid);
		}

		public IBluetoothGattCharacteristic BoilerGattCharacteristic2
		{
			get => BoilerGattService?.GattCharacteristics.FirstOrDefault(C => C.UUID == BoilerGattCharacteristic2Uuid);
		}

		public bool IsReadyForBoilerService
		{
			get => isReadyForBoilerService;
			protected set
			{
				if (SetProperty(ref isReadyForBoilerService, value))
				{
					IsReadyForBoilerServiceChanged?.Invoke(this, value);
				}
			}
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
