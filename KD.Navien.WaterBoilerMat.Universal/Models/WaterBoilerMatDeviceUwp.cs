using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using Microsoft.Toolkit.Uwp.Connectivity;
using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Logging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
	public sealed class WaterBoilerMatDeviceUwp : WaterBoilerMatDevice
	{
		#region Properties

		public override string Name => _device?.Name;
		public override string Address => _device?.BluetoothAddressAsString?.ToUpper();
        public override bool IsConnected => _device?.IsConnected == true;

        #endregion

        #region Fields

        private ObservableBluetoothLEDevice _device;

        #endregion

        #region Constructors & Initialize & Dispose

        public WaterBoilerMatDeviceUwp(ObservableBluetoothLEDevice device, ILoggerFacade logger)
        {
            _device = device;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
		{
            _device.PropertyChanged += (s, e) =>
            {
                if (_device != null)
                {
                    RaisePropertyChanged(e.PropertyName);
                    Type type = typeof(ObservableBluetoothLEDevice);
                    var value = type.GetProperty(e.PropertyName).GetValue(_device);
                    _logger.Log($"WaterBoilerMatDeviceUwp.PropertyChanged. [{e.PropertyName}]=[{value}]", Category.Debug, Priority.High);
                }
            };

            _device.Services.CollectionChanged += Services_CollectionChanged;
		}

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            try
            {
                if (disposing)
                {
                    _device.Services.CollectionChanged -= Services_CollectionChanged;
                    Disconnect();
                }
            }
            finally
            {
                _disposed = true;
            }
        }

        #endregion

        private void Services_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    foreach (var service in Services)
                    {
                        service.GattCharacteristicsUpdated -= Service_GattCharacteristicsUpdated;
                    }
                    Services.Clear();
                    break;
                case NotifyCollectionChangedAction.Add:
                    var addedServices = e.NewItems.OfType<ObservableGattDeviceService>().Select(S => new BluetoothGattServiceUwp(S)).ToList<IBluetoothGattService>();
                    foreach (var service in addedServices)
                    {
                        service.GattCharacteristicsUpdated += Service_GattCharacteristicsUpdated;
                        Services.Add(service);
                    }
                    break;
            }

            RaiseServicesUpdated();
        }

        private void Service_GattCharacteristicsUpdated(object sender, EventArgs e)
        {
            RaiseServicesUpdated();
        }

        protected override Task ConnectAsync()
        {
            if (_device.IsConnected)
            {
                return Task.CompletedTask;
            }
            else
            {
                return _device.ConnectAsync();
            }
        }

        protected async override void UpdateDeviceStatus(KDData data)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                IsPowerOn = Convert.ToBoolean(data.Power);
                IsLock = Convert.ToBoolean(data.KeyLock);
                IsLeftPartsPowerOn = (data.Status == 2 || data.Status == 3);
                IsRightPartsPowerOn = (data.Status == 2 || data.Status == 4);
            });
        }

        public override void Disconnect()
        {
            if (_device.IsConnected)
            {
                foreach (var service in Services)
                {
                    service.GattCharacteristicsUpdated -= Service_GattCharacteristicsUpdated;
                    service.Dispose();
                }
                Services.Clear();

                _device.BluetoothLEDevice.Dispose();
            }
        }

        public override async Task<T> GetNativeBluetoothLEDeviceObjectAsync<T>()
        {
            if (_device.BluetoothLEDevice != null)
            {
                return _device.BluetoothLEDevice as T;
            }
            else
            {
                var bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(_device.BluetoothAddressAsUlong);
                return bleDevice as T;
            }
        }
    }
}
