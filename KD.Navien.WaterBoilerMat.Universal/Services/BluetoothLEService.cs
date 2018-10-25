using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
using KD.Navien.WaterBoilerMat.Universal.Models;
using KD.Navien.WaterBoilerMat.Universal.Services;
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
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Core;

namespace KD.Navien.WaterBoilerMat.Universal.Services
{
    public class BluetoothLEService : IBluetoothLEService<WaterBoilerMatDevice>
    {
        #region Properties

        public bool IsScanning => _bluetoothLEAdvertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started ||
                                  _bluetoothLEAdvertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Stopping;

        #endregion

        #region Fields

        private readonly ILoggerFacade _logger;
        private readonly BluetoothLEAdvertisementWatcher _bluetoothLEAdvertisementWatcher;
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        #endregion

        #region Constructors & Initialize

        public BluetoothLEService(ILoggerFacade logger)
        {
            _logger = logger;
            _bluetoothLEAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();

            Initialize();
        }

        private void Initialize()
        {

        }

        #endregion

        public Task<IEnumerable<WaterBoilerMatDevice>> ScanAsync(int timeoutMilliseconds)
        {
            _logger.Log($"Call ScanAsync({timeoutMilliseconds})", Category.Debug, Priority.None);

            if (IsScanning)
            {
                _logger.Log($"BluetoothLEAdvertisementWatcher is already scanning. Status=[{_bluetoothLEAdvertisementWatcher.Status}]", Category.Warn, Priority.None);
                return Task.FromResult(Enumerable.Empty<WaterBoilerMatDevice>());
            }

            var tcs = new TaskCompletionSource<IEnumerable<WaterBoilerMatDevice>>();
            var bluetoothAddresses = new List<ulong>();

            // Start the Advertisement packet capture
            TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementReceivedEventArgs> OnAdvertisementPacketReceived = null;
            OnAdvertisementPacketReceived = new TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementReceivedEventArgs>((s, e) =>
            {
                if (_readerWriterLockSlim.TryEnterReadLock(TimeSpan.FromSeconds(1)))
                {
                    if (bluetoothAddresses.Contains(e.BluetoothAddress) != true && WaterBoilerMatDevice.IsNavienDevice(e.BluetoothAddress))
                    {
                        bluetoothAddresses.Add(e.BluetoothAddress);
                    }

                    _readerWriterLockSlim.ExitReadLock();
                }
            });
            TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementWatcherStoppedEventArgs> OnAdvertisementWatcherStopped = null;
            OnAdvertisementWatcherStopped = async (s, e) =>
            {
                _bluetoothLEAdvertisementWatcher.Received -= OnAdvertisementPacketReceived;
                _bluetoothLEAdvertisementWatcher.Stopped -= OnAdvertisementWatcherStopped;
                _logger.Log($"Stopped the BluetoothLEAdvertisementWatcher", Category.Info, Priority.None);

                var bluetoothLEDevices = new List<WaterBoilerMatDeviceUwp>();
                foreach (var bluetoothAddress in bluetoothAddresses)
                {
                    var bluetoothLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);
                    bluetoothLEDevices.Add(new WaterBoilerMatDeviceUwp(bluetoothLEDevice, _logger));
                }

                tcs.SetResult(bluetoothLEDevices);
            };
            _bluetoothLEAdvertisementWatcher.Received += OnAdvertisementPacketReceived;
            _bluetoothLEAdvertisementWatcher.Stopped += OnAdvertisementWatcherStopped;
            _bluetoothLEAdvertisementWatcher.Start();
            _logger.Log($"Started the BluetoothLEAdvertisementWatcher", Category.Info, Priority.None);

            // Start a timer
            Timer timer = null;
            timer = new Timer(delegate
            {
                timer.Dispose();
                _logger.Log($"The timer is expired. TaskStatus=[{tcs.Task.Status}]", Category.Info, Priority.None);

                if (IsScanning && tcs.Task.Status != TaskStatus.RanToCompletion)
                {
                    // Stop the Advertisement packet capture
                    _bluetoothLEAdvertisementWatcher?.Stop();
                }
            }, null, timeoutMilliseconds, Timeout.Infinite);

            return tcs.Task;
        }

        private object _locker = new object();
        public Task<IEnumerable<ulong>> ScanWaterBoilerMatDeviceAddressAsync(int timeoutMilliseconds)
        {
            _logger.Log($"Start ScanAsync2()", Category.Info, Priority.None);
            var tcs = new TaskCompletionSource<IEnumerable<ulong>>();

            try
            {
                var foundAddresses = new List<ulong>();
                var bleAdvWatcher = new BluetoothLEAdvertisementWatcher();
                bleAdvWatcher.ScanningMode = BluetoothLEScanningMode.Active;

                bleAdvWatcher.Received += (s, e) =>
                {
                    try
                    {
                        lock (foundAddresses)
                        {
                            if (foundAddresses.Any(A => A == e.BluetoothAddress) != true)
                            {
                                lock (foundAddresses)
                                {
                                    foundAddresses.Add(e.BluetoothAddress);
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        _logger.Log(error.Message, Category.Exception, Priority.Medium);
                    }
                };
                bleAdvWatcher.Stopped += (s, e) =>
                {
                    tcs.SetResult(foundAddresses.Where(A => WaterBoilerMatDevice.IsNavienDevice(A)));
                };

                Timer timer = null;
                timer = new Timer(delegate
                {
                    timer.Dispose();
                    bleAdvWatcher.Stop();

                }, null, timeoutMilliseconds, Timeout.Infinite);

                bleAdvWatcher.Start();
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }

            return tcs.Task;
        }
    }
}
