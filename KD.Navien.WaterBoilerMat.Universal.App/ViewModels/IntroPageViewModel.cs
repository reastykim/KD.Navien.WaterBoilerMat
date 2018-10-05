using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using KD.Navien.WaterBoilerMat.Universal.App.Services;
using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Logging;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class IntroPageViewModel : ViewModelBase
    {
        const int ScanTimeout = 5000;

        #region Properties

        /// <summary>
        /// Gets whether BLE is scanning.
        /// </summary>
        public bool IsScanning
        {
            get => _isScanning;
            private set => SetProperty(ref _isScanning, value);
        }
        private bool _isScanning;

        /// <summary>
        /// Gets whether the BLE is working on connect to the WaterBoilerMatDevice.
        /// </summary>
        public bool IsConnecting
        {
            get => _isConnecting;
            private set => SetProperty(ref _isConnecting, value);
        }
        private bool _isConnecting;

        /// <summary>
        /// Gets the list of WaterBoilerMatDevice that discovered on scanning. This property will be bind to the ItemsSource of ListView.
        /// </summary>
        public ObservableCollection<WaterBoilerMatDevice> Devices
        {
            get { return _foundDevices ?? (_foundDevices = new ObservableCollection<WaterBoilerMatDevice>()); }
        }
        private ObservableCollection<WaterBoilerMatDevice> _foundDevices;

        #endregion

        #region Commands

        protected override void ExecuteLoaded()
        {
            // When the IntroPage loads, it scan for WaterBoilerMatDevice.
            if (ScanCommand.CanExecute())
            {
                ScanCommand.Execute();
            }
        }

        public DelegateCommand ScanCommand
        {
            get { return _scanCommand ?? (_scanCommand = new DelegateCommand(ExecuteScan, CanExecuteScan)
                    .ObservesProperty(() => IsScanning)
                    .ObservesProperty(() => IsConnecting)); }
        }
        private DelegateCommand _scanCommand;
        private async void ExecuteScan()
        {
            // Deletes a list of previously discovered WaterBoilerMatDevice.
            Devices.Clear();

            // Scan
            IsScanning = true;
            var devices = await _bluetoothLEService.ScanAsync(ScanTimeout);
            IsScanning = false;

            // Add to list of 
            foreach (var device in devices)
            {
                Logger.Log($"Discovered a BLE Device. Name=[{device.Name}, Address=[{device.Address}]]", Category.Debug, Priority.Low);
                Devices.Add(device);
            }
        }
        private bool CanExecuteScan()
        {
            return IsScanning != true && IsConnecting != true;
        }

        public DelegateCommand<WaterBoilerMatDevice> ConnectCommand
        {
            get { return _connectCommand ?? (_connectCommand = new DelegateCommand<WaterBoilerMatDevice>(ExecuteConnect, CanExecuteConnect).ObservesProperty(() => IsConnecting)); }
        }
        private DelegateCommand<WaterBoilerMatDevice> _connectCommand;
        private async void ExecuteConnect(WaterBoilerMatDevice device)
        {
            try
            {
                if (_connectingDevice != null)
                {
                    _connectingDevice.Disconnect();
                    _connectingDevice = null;
                }

                _connectingDevice = device;
                IsConnecting = true;

                // Call to ConnectAsync method. If device is connected to WaterBoilerMatDevice, raise a BoilerServiceReady event.
                await _connectingDevice.ConnectAsync();
                _connectingDevice.BoilerServiceReady += OnDeviceBoilerServiceReady;
            }
            catch (Exception e)
            {
                Logger.Log($"BluetoothLE Device Name=[{device?.Name}], Address=[{device?.Address}] Connect fail. Exception=[{e.Message}]", Category.Exception, Priority.High);

                await _alertMessageService.ShowAsync("WaterBoilerMatDevice connect fail.", "Error");
                IsConnecting = false;
            }
        }
        private bool CanExecuteConnect(WaterBoilerMatDevice device)
        {
            return IsConnecting != true;
        }

        #endregion

        #region Fields

        private readonly IBluetoothLEService<WaterBoilerMatDevice> _bluetoothLEService;
        private readonly IAlertMessageService _alertMessageService;
        private readonly IPairingList _pairingList;

        private WaterBoilerMatDevice _connectingDevice;

        #endregion

        #region Constructors & Initialize

        public IntroPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothLEService,
            INavigationService navigationService, IAlertMessageService alertMessageService,
            IPairingList pairingList,
            ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _bluetoothLEService = bluetoothLEService;
            _alertMessageService = alertMessageService;
            _pairingList = pairingList;
        }

        #endregion

        #region Event Handlers

        private async void OnDeviceBoilerServiceReady(object sender, EventArgs e)
        {
            try
            {
                _connectingDevice.BoilerServiceReady -= OnDeviceBoilerServiceReady;

                // BLE Connection is ok now. 
                // But if we want to keep BLE connection, we must request a MAC_REGISTER.
                // If not, BLE connection will be lost.

                // Create a request
                var requestData = new KDRequest();
                requestData.Data.MessageType = KDMessageType.MAC_REGISTER;
                // check, is the device pairing mode : if the actual MatDevice is on BLE PairingMode set the UniqueID to String.Empty, otherwise set the already stored UniqueID.
                if (_pairingList.Contains(_connectingDevice.Address))
                {
                    requestData.Data.UniqueID = _pairingList[_connectingDevice.Address];
                }
                else
                {
                    requestData.Data.UniqueID = "";
                }

                var requestDataValue = requestData.GetValue();
                byte[] bytes = requestDataValue.HexStringToByteArray();

                var result = await _connectingDevice.BoilerGattCharacteristic2.SetNotifyAsync(true);
                if (result != true)
                {
                    throw new ApplicationException($"Call BoilerCharacteristic2.SetNotifyAsync(true). Result=[{result}]");
                }

                _connectingDevice.BoilerGattCharacteristic2.ValueChanged += OnBoilerGattCharacteristic2ValueChanged;
                // Write a request packet
                result = await _connectingDevice.BoilerGattCharacteristic2.WriteValueAsync(bytes);
                if (result)
                {
                    Logger.Log($"BoilerGattCharacteristic2.WriteValueAsync(). Value = [{requestDataValue}]", Category.Debug, Priority.High);
                }
                else
                {
                    throw new ApplicationException($"Call BoilerCharacteristic2.WriteValueAsync(). Result=[{result}], Data=[{requestDataValue}]");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ConnectedWaterBoilerMatDevice_BoilerServiceReady. Exception=[{ex.Message}]", Category.Exception, Priority.High);
                
                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice connect fail.", "Error");
                    IsConnecting = false;
                });
            }
        }

        private async void OnBoilerGattCharacteristic2ValueChanged(object sender, byte[] data)
        {
            var gattCharacteristic = sender as IBluetoothGattCharacteristic;
            var dataValue = data.ToString("X02");
            Logger.Log($"BoilerGattCharacteristic2_ValueChanged. Value = [{dataValue}]", Category.Debug, Priority.High);

            try
            {
                gattCharacteristic.ValueChanged -= OnBoilerGattCharacteristic2ValueChanged;

                KDResponse responseData = new KDResponse();
                if (responseData.SetValue(data))
                {// Connect & MAC_REGISTER Success.
                    if (responseData.Data.UniqueID != null)
                    {
                        _pairingList.Add(_connectingDevice.Address, responseData.Data.UniqueID);
                    }

                    Logger.Log($"Response Received. Data=[{dataValue}]", Category.Info, Priority.None);
                    // Navigate to MainPage
                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    {
                        NavigationService.Navigate("Main", _connectingDevice);
                        IsConnecting = false;
                    });
                }
                else
                {
                    Logger.Log($"Connect fail. Raw=[{responseData.Data.DEBUGCode}]", Category.Info, Priority.High);

                    var result = await _connectingDevice.BoilerGattCharacteristic2.SetNotifyAsync(false);
                    Logger.Log($"Call BoilerCharacteristic2.SetNotifyAsync(false). Result=[{result}]", Category.Debug, Priority.None);

                    _connectingDevice.Disconnect();
                    _connectingDevice = null;

                    throw new ApplicationException($"KDResponse.SetValue() fail.");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"BoilerGattCharacteristic2_ValueChanged. Exception=[{ex.Message}], Response = [{dataValue}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice connect fail.", "Error");
                    IsConnecting = false;
                });
            }
        }

        #endregion
    }
}
