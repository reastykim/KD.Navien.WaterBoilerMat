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

        private async Task<AppServiceResponseStatus> ConnectAsync(string deviceId, string uniqueId, AppServiceRequest request)
        {
            var device = _devices.FirstOrDefault(D => D.Id == deviceId);
            if (device == null)
            {
                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, $"Not found device by DeviceId. DeviceId=[{deviceId}]" },
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
                    device.DisconnectAsync();
                }

                return await request.SendResponseAsync(new ValueSet()
                {
                    { Parameters.Result, false },
                    { Parameters.Details, e.Message },
                });
            }
        }

        private void OnDeviceStatusUpdated(object sender, EventArgs e)
        {
            string jsonResult = JsonConvert.SerializeObject(_connectedDevice, Formatting.Indented);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _connection.SendMessageAsync(new ValueSet()
            {
                { Parameters.Device, jsonResult },
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void CleanUp()
        {
            _devices = null;

            if (_connectedDevice != null)
            {
                _connectedDevice.DeviceStatusUpdated -= OnDeviceStatusUpdated;
                _connectedDevice.DisconnectAsync();
                _connectedDevice.Dispose();
                _connectedDevice = null;
            }
        }
    }
}
