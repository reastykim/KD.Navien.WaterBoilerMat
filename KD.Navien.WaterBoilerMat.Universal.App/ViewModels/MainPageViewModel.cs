using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using KD.Navien.WaterBoilerMat.Universal.App.Services;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
using Prism.Commands;
using Prism.Logging;
using Prism.Windows.AppModel;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        const int ScanTimeout = 5000;
        #region Properties

        public bool IsAvailableBluetoothLEScan
        {
            get => _isAvailableBluetoothLEScan;
            set => SetProperty(ref _isAvailableBluetoothLEScan, value);
        }
        private bool _isAvailableBluetoothLEScan = true;

        public ObservableCollection<WaterBoilerMatDevice> FoundDevices
        {
            get { return _foundDevices ?? (_foundDevices = new ObservableCollection<WaterBoilerMatDevice>()); }
        }
        private ObservableCollection<WaterBoilerMatDevice> _foundDevices;

        public WaterBoilerMatDevice ConnectedWaterBoilerMatDevice
        {
            get => _connectedWaterBoilerMatDevice;
            set => SetProperty(ref _connectedWaterBoilerMatDevice, value);
        }
        private WaterBoilerMatDevice _connectedWaterBoilerMatDevice;

        public bool LoadingData
        {
            get { return _loadingData; }
            private set { SetProperty(ref _loadingData, value); }
        }
        private bool _loadingData;

        #endregion

        #region Commands

        public DelegateCommand ScanCommand
        {
            get { return _scanCommand ?? (_scanCommand = new DelegateCommand(ExecuteScan, CanExecuteScan).ObservesProperty(() => IsAvailableBluetoothLEScan)); }
        }
        private DelegateCommand _scanCommand;
        private async void ExecuteScan()
        {
            FoundDevices.Clear();

            IsAvailableBluetoothLEScan = false;
            var devices = await _bluetoothLEService.ScanAsync(ScanTimeout);
            IsAvailableBluetoothLEScan = true;

            foreach (var device in devices)
            {
                _logger.Log($"Found a BLE Device. Name=[{device.Name}, Address=[{device.Address}]]", Category.Debug, Priority.Low);
                FoundDevices.Add(device);
            }
        }
        private bool CanExecuteScan()
        {
            return IsAvailableBluetoothLEScan;
        }

        public DelegateCommand<WaterBoilerMatDevice> ConnectCommand
        {
            get { return _connectCommand ?? (_connectCommand = new DelegateCommand<WaterBoilerMatDevice>(ExecuteConnect)); }
        }
        private DelegateCommand<WaterBoilerMatDevice> _connectCommand;
        private async void ExecuteConnect(WaterBoilerMatDevice waterBoilerMatDevice)
        {
            try
            {
                if (ConnectedWaterBoilerMatDevice != null)
                {
                    ConnectedWaterBoilerMatDevice.BoilerServiceReady -= ConnectedWaterBoilerMatDevice_BoilerServiceReady;
                    ConnectedWaterBoilerMatDevice.Disconnect();
                    ConnectedWaterBoilerMatDevice = null;
                }

                await waterBoilerMatDevice.ConnectAsync();
                ConnectedWaterBoilerMatDevice = waterBoilerMatDevice;
                ConnectedWaterBoilerMatDevice.BoilerServiceReady += ConnectedWaterBoilerMatDevice_BoilerServiceReady;
            }
            catch (Exception e)
            {
                _logger.Log($"BluetoothLE Device Name=[{waterBoilerMatDevice?.Name}], Address=[{waterBoilerMatDevice?.Address}] Connect fail. Exception=[{e.Message}]", Category.Exception, Priority.High);
            }
        }

        public DelegateCommand DebugCommand
        {
            get { return _debugCommand ?? (_debugCommand = new DelegateCommand(ExecuteDebug)); }
        }
        private DelegateCommand _debugCommand;
        private void ExecuteDebug()
        {

        }

        #endregion

        #region Fields

        private readonly IBluetoothLEService<WaterBoilerMatDevice> _bluetoothLEService;
        private readonly IAlertMessageService _alertMessageService;
        private readonly INavigationService _navigationService;
        private readonly IPairingList _pairingList;
        private readonly ILoggerFacade _logger;

        #endregion

        #region Constructors & Initialize

        public MainPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothLEService, 
            INavigationService navigationService, IAlertMessageService alertMessageService,
            IPairingList pairingList,
            ILoggerFacade logger)
        {
            _bluetoothLEService = bluetoothLEService;
            _navigationService = navigationService;
            _alertMessageService = alertMessageService;
            _pairingList = pairingList;
            _logger = logger;
        }

        #endregion

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            string errorMessage = string.Empty;

            try
            {
                LoadingData = true;
            }
            catch (Exception ex)
            {
                //errorMessage = string.Format(CultureInfo.CurrentCulture,
                //                             _resourceLoader.GetString("GeneralServiceErrorMessage"),
                //                             Environment.NewLine,
                //                             ex.Message);
            }
            finally
            {
                LoadingData = false;
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                //await _alertMessageService.ShowAsync(errorMessage, _resourceLoader.GetString("ErrorServiceUnreachable"));
                return;
            }
        }

        #region Event Handlers

        private async void ConnectedWaterBoilerMatDevice_BoilerServiceReady(object sender, EventArgs e)
        {
            try
            {
                // Create a request
                var requestData = new KDRequest();
                requestData.Data.MessageType = KDMessageType.MAC_REGISTER;
                // check, is the pairing mode
                if (_pairingList.Contains(ConnectedWaterBoilerMatDevice.Address))
                {
                    requestData.Data.UniqueID = _pairingList[ConnectedWaterBoilerMatDevice.Address];
                }
                else
                {
                    requestData.Data.UniqueID = "";
                }
                // to bytes
                var requestDataValue = requestData.GetValue();
                byte[] bytes = requestDataValue.HexStringToByteArray();
                
                var result = await ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.SetNotifyAsync(true);
                if (result != true)
                {
                    throw new ApplicationException($"Call BoilerCharacteristic2.SetNotifyAsync(true). Result=[{result}]");
                }

                ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.ValueChanged += BoilerGattCharacteristic2_ValueChanged;

                result = await ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.WriteValueAsync(bytes);
                if (result)
                {
                    _logger.Log($"BoilerGattCharacteristic2.WriteValueAsync(). Value = [{requestDataValue}]", Category.Debug, Priority.High);
                }
                else
                {
                    throw new ApplicationException($"Call BoilerCharacteristic2.WriteValueAsync(). Result=[{result}], Data=[{requestDataValue}]");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"ConnectedWaterBoilerMatDevice_BoilerServiceReady. Exception=[{ex.Message}]", Category.Exception, Priority.High);
                await _alertMessageService.ShowAsync("WaterBoilerMatDevice connect fail.", "Error");
            }
        }

        private async void BoilerGattCharacteristic2_ValueChanged(object sender, byte[] data)
        {
            var dataValue = data.ToString("X02");
            _logger.Log($"BoilerGattCharacteristic2_ValueChanged. Value = [{dataValue}]", Category.Debug, Priority.High);

            try
            {
                ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.ValueChanged -= BoilerGattCharacteristic2_ValueChanged;

                KDResponse responseData = new KDResponse();
                if (responseData.SetValue(data))
                {
                    if (responseData.Data.UniqueID != null)
                    {
                        _pairingList.Add(ConnectedWaterBoilerMatDevice.Address, responseData.Data.UniqueID);
                    }

                    _logger.Log($"Response Received. Data=[{dataValue}]", Category.Info, Priority.None);
                }
                else
                {
                    _logger.Log($"Connect fail. Raw=[{responseData.Data.DEBUGCode}]", Category.Info, Priority.High);

                    var result = await ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.SetNotifyAsync(false);
                    _logger.Log($"Call BoilerCharacteristic2.SetNotifyAsync(false). Result=[{result}]", Category.Debug, Priority.None);

                    ConnectedWaterBoilerMatDevice.Disconnect();
                    ConnectedWaterBoilerMatDevice = null;

                    throw new ApplicationException($"KDResponse.SetValue() fail.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"BoilerGattCharacteristic2_ValueChanged. Exception=[{ex.Message}], Response = [{dataValue}]", Category.Exception, Priority.High);
                await _alertMessageService.ShowAsync("WaterBoilerMatDevice connect fail.", "Error");
            }
        }

        #endregion
    }
}
