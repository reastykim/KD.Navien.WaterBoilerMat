using KD.Navien.WaterBoilerMat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using static KD.Navien.WaterBoilerMat.Standard.Services.AppServiceCommands;

namespace KD.Navien.WaterBoilerMat.Universal.BackgroundApp
{
    public partial class StartupTask
    {
        #region Fields

        private IEnumerable<WaterBoilerMatDevice> _devices;
        private WaterBoilerMatDevice _connectedDevice;

        #endregion

        private async Task<AppServiceResponseStatus> ScanAsync(int timeoutMilliseconds, AppServiceRequest request)
        {
            _devices = await _bluetoothLEService.ScanAsync(timeoutMilliseconds);

            var jsonResult = JsonConvert.SerializeObject(_devices, Formatting.Indented);

            return await request.SendResponseAsync(new ValueSet()
            {
                { Parameters.Devices, jsonResult },
            });
        }

        private async Task<AppServiceResponseStatus> ConnectAsync(string deviceID, string uniqueId, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceID=[{deviceID}]" },
                });
            }

            try
            {
                uniqueId = await device.ConnectAsync(uniqueId);
                _connectedDevice = device;
                _connectedDevice.DeviceStatusUpdated += OnDeviceStatusUpdated;

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, JsonConvert.SerializeObject(device, Formatting.Indented) },
                    { Parameters.UniqueID, uniqueId },
                });
            }
            catch (Exception e)
            {
                if (device.IsConnected)
                {
                    await device.DisconnectAsync();
                }

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private async Task<AppServiceResponseStatus> DisconnectAsync(string deviceID, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceID=[{deviceID}]" },
                });
            }

            try
            {
                device.DeviceStatusUpdated -= OnDeviceStatusUpdated;
                await device.DisconnectAsync();
                _connectedDevice = null;

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, String.Empty },
                });
            }
            catch (Exception e)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private async Task<AppServiceResponseStatus> RequestPowerOnOffAsync(string deviceID, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceId=[{deviceID}]" },
                });
            }

            try
            {
                await device.RequestPowerOnOffAsync();

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, device.IsPowerOn.ToString() },
                });
            }
            catch (Exception e)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private async Task<AppServiceResponseStatus> RequestLockOnOffAsync(string deviceID, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceId=[{deviceID}]" },
                });
            }

            try
            {
                await device.RequestLockOnOffAsync();

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, device.IsLock.ToString() },
                });
            }
            catch (Exception e)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private async Task<AppServiceResponseStatus> RequestLeftPartsPowerOnOffAsync(string deviceID, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceId=[{deviceID}]" },
                });
            }

            try
            {
                await device.RequestLeftPartsPowerOnOffAsync();

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, device.IsLeftPartsPowerOn.ToString() },
                });
            }
            catch (Exception e)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private async Task<AppServiceResponseStatus> RequestRightPartsPowerOnOffAsync(string deviceID, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceId=[{deviceID}]" },
                });
            }

            try
            {
                await device.RequestRightPartsPowerOnOffAsync();

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, device.IsRightPartsPowerOn.ToString() },
                });
            }
            catch (Exception e)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private async Task<AppServiceResponseStatus> RequestVolumeChangeAsync(string deviceID, VolumeLevels volumeLevel, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceId=[{deviceID}]" },
                });
            }

            try
            {
                await device.RequestVolumeChangeAsync(volumeLevel);

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, device.VolumeLevel.ToString() },
                });
            }
            catch (Exception e)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private async Task<AppServiceResponseStatus> RequestSetupTemperatureChangeAsync(string deviceID, int leftTemperature, int rightTemperature, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceID);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceID. DeviceId=[{deviceID}]" },
                });
            }

            try
            {
                await device.RequestSetupTemperatureChangeAsync(leftTemperature, rightTemperature);

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, true },
                    { Parameters.Details, JsonConvert.SerializeObject(new int[] { device.SetupLeftTemperature, device.SetupRightTemperature }, Formatting.Indented) },
                });
            }
            catch (Exception e)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private void OnDeviceStatusUpdated(object sender, EventArgs e)
        {
            if (_connection == null || _connectedDevice == null)
                return;

            string jsonResult = JsonConvert.SerializeObject(_connectedDevice, Formatting.Indented);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _connection.SendMessageAsync(new ValueSet()
            {
                { Parameters.DeviceInformation, jsonResult },
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async void CleanUp()
        {
            _devices = null;

            if (_connectedDevice != null)
            {
                _connectedDevice.DeviceStatusUpdated -= OnDeviceStatusUpdated;
                await _connectedDevice.DisconnectAsync();
                _connectedDevice.Dispose();
                _connectedDevice = null;
            }
        }
    }
}
