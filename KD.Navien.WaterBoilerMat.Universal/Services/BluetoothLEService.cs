using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
using KD.Navien.WaterBoilerMat.Universal.Models;
using KD.Navien.WaterBoilerMat.Universal.Services;
using Microsoft.Toolkit.Uwp.Connectivity;
using Prism.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.UI.Core;

namespace KD.Navien.WaterBoilerMat.Universal.Services
{
    public class BluetoothLEService : IBluetoothLEService<WaterBoilerMatDevice>
    {
        #region Properties

        public bool IsScanning => _bluetoothLEHelper.IsEnumerating;

        #endregion

        #region Fields

        private ILoggerFacade _logger;
        private BluetoothLEHelper _bluetoothLEHelper;

        #endregion

        #region Constructors & Initialize

        public BluetoothLEService(ILoggerFacade logger)
        {
            this._logger = logger;

            Initialize();
        }

        private void Initialize()
        {
            // Get a local copy of the context for easier reading
            _bluetoothLEHelper = BluetoothLEHelper.Context;
        }

        #endregion

        public Task<IEnumerable<WaterBoilerMatDevice>> ScanAsync(int timeoutMilliseconds)
        {
            _logger.Log($"Call ScanAsync({timeoutMilliseconds})", Category.Debug, Priority.Medium);
            var tcs = new TaskCompletionSource<IEnumerable<WaterBoilerMatDevice>>();

            // check if BluetoothLE APIs are available
            if (BluetoothLEHelper.IsBluetoothLESupported != true)
            {
                _logger.Log($"BluetoothLE APIs are not available", Category.Warn, Priority.High);
                return Task.FromResult(Enumerable.Empty<WaterBoilerMatDevice>());
            }

            // Start the Enumeration
            EventHandler<EventArgs> onEnumerationCompleted = null;
            onEnumerationCompleted = new EventHandler<EventArgs>((s, e) =>
            {
                _bluetoothLEHelper.EnumerationCompleted -= onEnumerationCompleted;
                _logger.Log($"Stop the BluetoothLE device Enumeration. Found {_bluetoothLEHelper.BluetoothLeDevices.Count} devices", Category.Info, Priority.High);

                tcs.SetResult(_bluetoothLEHelper.BluetoothLeDevices.Where(d => WaterBoilerMatDevice.IsNavienDevice(d.BluetoothAddressAsString))
                                                                  .Select(d => new WaterBoilerMatDeviceUwp(d)));
            });
            _bluetoothLEHelper.EnumerationCompleted += onEnumerationCompleted;
            _bluetoothLEHelper.StartEnumeration();
            _logger.Log($"Start the BluetoothLE device Enumeration", Category.Info, Priority.High);

            Timer timer = null;
            timer = new Timer(delegate
            {
                timer.Dispose();
                // Stop the Enumeration
                _bluetoothLEHelper.StopEnumeration();
                _logger.Log($"Stop the BluetoothLE device Enumeration. Found {_bluetoothLEHelper.BluetoothLeDevices.Count} devices.", Category.Info, Priority.High);

                tcs.SetResult(_bluetoothLEHelper.BluetoothLeDevices.Where(d => WaterBoilerMatDevice.IsNavienDevice(d.BluetoothAddressAsString))
                                                                   .Select(d => new WaterBoilerMatDeviceUwp(d)));

            }, null, timeoutMilliseconds, Timeout.Infinite);

            return tcs.Task;
        }
    }
}
