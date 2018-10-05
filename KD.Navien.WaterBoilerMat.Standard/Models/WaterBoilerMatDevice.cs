using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public abstract class WaterBoilerMatDevice : BindableBase, IBluetoothLEDevice
	{
		protected const string NavienDeviceMacPrefix = "2C:E2:A8";
		public const string BoilerGattServiceUuid = "00001c0d-d102-11e1-9b23-2ce2a80000dd";
		public const string BoilerGattCharacteristic1Uuid = "00001c0d-d102-11e1-9b23-2ce2a80100dd";
		public const string BoilerGattCharacteristic2Uuid = "00001c0d-d102-11e1-9b23-2ce2a80200dd";
		
		public event EventHandler ServicesUpdated;
        public event EventHandler BoilerServiceReady;

        #region Properties

        public abstract string Name { get; }

		public abstract string Address { get; }

        public abstract bool IsConnected { get; }

        public bool IsBoilerServiceReady
        {
            get { return isBoilerServiceReady; }
            protected set { SetProperty(ref isBoilerServiceReady, value); }
        }
        private bool isBoilerServiceReady;

        public List<IBluetoothGattService> Services
		{
			get => services;
			protected set => SetProperty(ref services, value);
		}
		private List<IBluetoothGattService> services = new List<IBluetoothGattService>();

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

        #endregion

        protected void RaiseServicesUpdated()
		{
			ServicesUpdated?.Invoke(this, EventArgs.Empty);

            if (BoilerGattCharacteristic1 != null && BoilerGattCharacteristic2 != null && IsBoilerServiceReady != true)
            {
                IsBoilerServiceReady = true;
                BoilerServiceReady?.Invoke(this, EventArgs.Empty);
            }
        }

        public abstract Task ConnectAsync();

        public abstract void Disconnect();

        public abstract Task<T> GetNativeBluetoothLEDeviceObjectAsync<T>() where T : class;

        public static bool IsNavienDevice(string address)
        {
            return address.StartsWith(NavienDeviceMacPrefix, StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool IsNavienDevice(ulong address)
        {
            return address.ToMacAddress().StartsWith(NavienDeviceMacPrefix, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
