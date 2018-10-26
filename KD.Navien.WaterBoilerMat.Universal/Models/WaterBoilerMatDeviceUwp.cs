using KD.Navien.WaterBoilerMat.Universal.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Logging;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
	public sealed class WaterBoilerMatDeviceUwp : WaterBoilerMatDevice
	{
        #region Properties

        public override string Id => _device.DeviceId;
        public override string Name => _device.Name;
		public override string Address => _device.BluetoothAddress.ToMacAddress().ToUpper();
        public override bool IsConnected => _device.ConnectionStatus == BluetoothConnectionStatus.Connected;

        #endregion

        #region Fields

        private readonly BluetoothLEDevice _device;

        #endregion

        #region Constructors & Initialize & Dispose

        public WaterBoilerMatDeviceUwp(BluetoothLEDevice device, ILoggerFacade logger)
        {
            _device = device;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
		{
            _device.ConnectionStatusChanged += (s, e) =>
            {
                _logger.Log($"ConnectionStatusChanged. ConnectionStatus=[{_device.ConnectionStatus}]", Category.Debug, Priority.High);

                if (IsBackgroundRunning == false)
                {
                    DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        RaisePropertyChanged(nameof(IsConnected));
                    });
                }
            };
            _device.NameChanged += (s, e) =>
            {
                _logger.Log($"NameChanged. Name=[{_device.Name}]", Category.Debug, Priority.High);

                if (IsBackgroundRunning == false)
                {
                    DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        RaisePropertyChanged(nameof(Name));
                    });
                }
            };
		}

        protected override async void Dispose(bool disposing)
        {
            if (_disposed) return;
            try
            {
                if (disposing)
                {
                    await DisconnectAsync();
                }
            }
            finally
            {
                _disposed = true;
            }
        }

        #endregion

        protected override async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            // Get all the services for this device
            var getGattServicesAsyncTokenSource = new CancellationTokenSource(5000);
            var getGattServicesAsyncTask = await Task.Run(() => _device.GetGattServicesAsync(BluetoothCacheMode.Uncached), getGattServicesAsyncTokenSource.Token);
            var gattDeviceServiceResult = await getGattServicesAsyncTask;

            if (gattDeviceServiceResult.Status == GattCommunicationStatus.Success)
            {
                // In case we connected before, clear the service list and recreate it
                Services.Clear();

                foreach (var gattDeviceService in gattDeviceServiceResult.Services)
                {
                    var bluetoothGattServiceUwp = new BluetoothGattServiceUwp(gattDeviceService);
                    bluetoothGattServiceUwp.GattCharacteristicsUpdated += Service_GattCharacteristicsUpdated;

                    Services.Add(bluetoothGattServiceUwp);
                }

                RaiseServicesUpdated();
            }
            else
            {
                if (gattDeviceServiceResult.ProtocolError != null)
                {
                    throw new Exception(gattDeviceServiceResult.ProtocolError.GetErrorString());
                }
            }
        }

        protected async override void UpdateDeviceStatus(KDData data)
        {
            if (IsBackgroundRunning)
            {
                TemperatureInfo = new TemperatureInfo(data.MaxTemperatureHighLow);

                CurrentLeftTemperature = data.TemperatureReturnLeft;
                CurrentRightTemperature = data.TemperatureReturnRight;

                SetupLeftTemperature = data.TemperatureSettingLeft;
                SetupRightTemperature = data.TemperatureSettingRight;

                IsLeftPartsPowerOn = (data.Status == 2 || data.Status == 3);
                IsRightPartsPowerOn = (data.Status == 2 || data.Status == 4);

                Status = (DeviceStatus)data.Status;
                VolumeLevel = (VolumeLevels)data.Volume;
                WaterCapacity = (WaterCapacities)data.WaterCapacity;
                IsLock = Convert.ToBoolean(data.KeyLock);
                IsPowerOn = Convert.ToBoolean(data.Power);
            }
            else
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    TemperatureInfo = new TemperatureInfo(data.MaxTemperatureHighLow);

                    CurrentLeftTemperature = data.TemperatureReturnLeft;
                    CurrentRightTemperature = data.TemperatureReturnRight;

                    SetupLeftTemperature = data.TemperatureSettingLeft;
                    SetupRightTemperature = data.TemperatureSettingRight;

                    IsLeftPartsPowerOn = (data.Status == 2 || data.Status == 3);
                    IsRightPartsPowerOn = (data.Status == 2 || data.Status == 4);

                    Status = (DeviceStatus)data.Status;
                    VolumeLevel = (VolumeLevels)data.Volume;
                    WaterCapacity = (WaterCapacities)data.WaterCapacity;
                    IsLock = Convert.ToBoolean(data.KeyLock);
                    IsPowerOn = Convert.ToBoolean(data.Power);
                });
            }
        }

        public override async Task DisconnectAsync()
        {
            await base.DisconnectAsync();

            if (IsConnected)
            {
                foreach (var service in Services)
                {
                    service.GattCharacteristicsUpdated -= Service_GattCharacteristicsUpdated;
                    service.Dispose();
                }
                Services.Clear();

                _device.Dispose();
            }
        }

        public override Task<T> GetNativeBluetoothLEDeviceObjectAsync<T>()
        {
            return Task.FromResult(_device as T);
        }

        private void Service_GattCharacteristicsUpdated(object sender, EventArgs e)
        {
            RaiseServicesUpdated();
        }

        private static bool IsBackgroundRunning
        {
            get { return Windows.ApplicationModel.Core.CoreApplication.Views.Count == 0; }
        }
    }
}