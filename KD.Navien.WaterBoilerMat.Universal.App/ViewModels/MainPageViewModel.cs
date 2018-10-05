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
            var devices = await _bluetoothLEService.ScanAsync(5000);
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
                    //ConnectedWaterBoilerMatDevice.ServicesUpdated -= ConnectedWaterBoilerMatDevice_ServicesUpdated;
                    ConnectedWaterBoilerMatDevice.BoilerServiceReady -= ConnectedWaterBoilerMatDevice_BoilerServiceReady;
                    ConnectedWaterBoilerMatDevice = null;
                }

                await waterBoilerMatDevice.ConnectAsync();
                ConnectedWaterBoilerMatDevice = waterBoilerMatDevice;
                ConnectedWaterBoilerMatDevice.BoilerServiceReady += ConnectedWaterBoilerMatDevice_BoilerServiceReady;
                //ConnectedWaterBoilerMatDevice.ServicesUpdated += ConnectedWaterBoilerMatDevice_ServicesUpdated;
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
        private INavigationService _navigationService;
        private ILoggerFacade _logger;

        #endregion

        #region Constructors & Initialize

        public MainPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothLEService, 
            INavigationService navigationService, IAlertMessageService alertMessageService,
            ILoggerFacade logger)
        {
            _bluetoothLEService = bluetoothLEService;
            _navigationService = navigationService;
            _alertMessageService = alertMessageService;
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
                var requestData = new KDRequest();
                requestData.Data.MessageType = KDMessageType.MAC_REGISTER;
                requestData.Data.UniqueID = "";
                var requestDataValue = requestData.GetValue();
                byte[] bytes = requestDataValue.HexStringToByteArray();
                
                var result = await ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.SetNotifyAsync(true);
                _logger.Log($"Call BoilerCharacteristic2.SetNotifyAsync(true). Result=[{result}]", Category.Debug, Priority.None);

                ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.ValueChanged += BoilerGattCharacteristic2_ValueChanged;

                result = await ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.WriteValueAsync(bytes);
                _logger.Log($"Call BoilerCharacteristic2.WriteValueAsync(). Result=[{result}], Data=[{requestDataValue}]", Category.Debug, Priority.None);
            }
            catch (Exception ex)
            {
                _logger.Log($"WaterBoilerMatDevice Communication fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);
            }
        }

        private async void BoilerGattCharacteristic2_ValueChanged(object sender, string e)
        {
            _logger.Log($"BoilerGattCharacteristic2_ValueChanged. Value = [{e}]", Category.Debug, Priority.None);

            try
            {
                KDResponse responseData = new KDResponse();
                byte[] getData = e.HexStringToByteArray("-", ":", " ");
                if (responseData.SetValue(getData))
                {
                    if (responseData.Data.UniqueID != null)
                    {
                        //new PairingList(IntroActivity.this.mContext).setPairing(IntroActivity.this.mDeviceAddress, IntroActivity.this.mDeviceName, responseData.mData.UniqueID, IntroActivity.D);
                    }
                    //Intent serverIntent = new Intent(IntroActivity.this.mContext, MainActivity.class);
                    //serverIntent.setFlags(67108864);
                    //serverIntent.putExtra(Constants.DEVICE_ADDRESS, IntroActivity.this.mDeviceAddress);
                    //IntroActivity.this.startActivity(serverIntent);
                    //IntroActivity.this.unregisterReceiver(IntroActivity.this.mGattUpdateReceiver);
                    //IntroActivity.this.finish();
                    _logger.Log($"Response Received. Data=[{e}]", Category.Info, Priority.None);
                }
                else
                {
                    _logger.Log($"Connect fail. Raw=[{responseData.Data.DEBUGCode}]", Category.Info, Priority.High);

                    var result = await ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.SetNotifyAsync(false);
                    _logger.Log($"Call BoilerCharacteristic2.SetNotifyAsync(false). Result=[{result}]", Category.Debug, Priority.None);

                    ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.ValueChanged -= BoilerGattCharacteristic2_ValueChanged;
                    ConnectedWaterBoilerMatDevice.Disconnect();
                    ConnectedWaterBoilerMatDevice = null;
                }
                //var requestData = new KDRequest();
                //requestData.Data.MessageType = KDMessageType.MAC_REGISTER;
                //requestData.Data.UniqueID = ConnectedWaterBoilerMatDevice.Address.ToUpper().Replace(":", "");
                //var requestDataValue = requestData.GetValue();
                //byte[] bytes = requestDataValue.HexStringToByteArray();

                //var result = await ConnectedWaterBoilerMatDevice.BoilerGattCharacteristic2.WriteValueAsync(bytes);
                //_logger.Log($"Call BoilerCharacteristic2.WriteValueAsync(). Result=[{result}], Data=[{requestDataValue}]", Category.Debug, Priority.None);
            }
            catch (Exception ex)
            {
                _logger.Log($"BoilerGattCharacteristic2 Unknown response. Exception=[{ex.Message}], Response = [{e}]", Category.Exception, Priority.High);
            }
        }
//            if (IntroActivity.this.mBluetoothLeService != null) 
//              {
//                if (Boolean.valueOf(responseData.SetValue(getData)).booleanValue())
//                {
//                    if (responseData.mData.UniqueID != null)
//                    {
//                        new PairingList(IntroActivity.this.mContext).setPairing(IntroActivity.this.mDeviceAddress, IntroActivity.this.mDeviceName, responseData.mData.UniqueID, IntroActivity.D);
//                    }
//                    Intent serverIntent = new Intent(IntroActivity.this.mContext, MainActivity.class);
//                        serverIntent.setFlags(67108864);
//                        serverIntent.putExtra(Constants.DEVICE_ADDRESS, IntroActivity.this.mDeviceAddress);
//                        IntroActivity.this.startActivity(serverIntent);
//                    IntroActivity.this.unregisterReceiver(IntroActivity.this.mGattUpdateReceiver);
//                    IntroActivity.this.finish();
//                } else {
//                                    Toast.makeText(IntroActivity.this.mContext, IntroActivity.this.getResources().getString(R.string.message_waterboiler_connected_fail), 0).show();
//                                    if (IntroActivity.this.mBluetoothLeService != null) {
//                    IntroActivity.this.mBluetoothLeService.close();
//                }
//                }
//                IntroActivity.this.mTextview_intro_button.setEnabled(IntroActivity.D);

//                IntroActivity.this.mImageButton_intro_refresh.setEnabled(IntroActivity.D);

//                IntroActivity.this.mPairedListView.setEnabled(IntroActivity.D);

//                IntroActivity.this.mCustomProgress.dismiss();

//                IntroActivity.this.TimeHandlerRefresh(false);
//                Log.e("broad cast", "action gatt changed");
//                }

//}

        #endregion
    }
}
