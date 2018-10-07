using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
	public class WaterBoilerMatDeviceUwp : WaterBoilerMatDevice
	{
		#region Properties

		public override string Name => device?.Name;
		public override string Address => device?.BluetoothAddressAsString?.ToUpper();
        public override bool IsConnected => device?.IsConnected == true;

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
        public WaterBoilerMatDeviceUwp(BluetoothLEDevice device)
            :this(new ObservableBluetoothLEDevice(device.DeviceInformation))
        {

        }

        private void Initialize()
		{
            device.PropertyChanged += (s, e) =>
            {
                RaisePropertyChanged(e.PropertyName);
                Type type = typeof(ObservableBluetoothLEDevice);
                var value = type.GetProperty(e.PropertyName).GetValue(device);
                System.Diagnostics.Debug.WriteLine($"ObservableBluetoothLEDevice.PropertyChanged. {e.PropertyName}=[{value}]");
            };

            device.Services.CollectionChanged += Services_CollectionChanged;
		}

        private void Services_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (var service in Services)
			{
                service.GattCharacteristicsUpdated -= Service_GattCharacteristicsUpdated;
			}

			Services = device.Services.Select(S => new BluetoothGattServiceUwp(S)).ToList<IBluetoothGattService>();
			foreach (var service in Services)
			{
				service.GattCharacteristicsUpdated += Service_GattCharacteristicsUpdated;
			}

			RaiseServicesUpdated();
		}

		private void Service_GattCharacteristicsUpdated(object sender, EventArgs e)
		{
			RaiseServicesUpdated();
		}

		#endregion

		public override async Task ConnectAsync()
		{
            if (device.IsConnected != true)
            {
                await device.ConnectAsync();
            }
		}

        public override void Disconnect()
        {
            if (device.IsConnected)
            {
                device.Services.CollectionChanged -= Services_CollectionChanged;

                foreach (var service in Services)
                {
                    service.GattCharacteristicsUpdated -= Service_GattCharacteristicsUpdated;
                    service.Dispose();
                }
                Services.Clear();

                IsBoilerServiceReady = false;
                device.Disconnect();
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        public override async Task<T> GetNativeBluetoothLEDeviceObjectAsync<T>()
        {
            if (device.BluetoothLEDevice != null)
            {
                return device.BluetoothLEDevice as T;
            }
            else
            {
                var bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(device.BluetoothAddressAsUlong);
                return bleDevice as T;
            }
            
        }
    }
}
