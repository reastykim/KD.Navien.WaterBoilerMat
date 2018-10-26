using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using KD.Navien.WaterBoilerMat.Universal.Services;
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
using Windows.UI.Core;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.Universal.RemoteApp.ViewModels
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
        public ObservableCollection<IBluetoothLEDevice> Devices
        {
            get { return _foundDevices ?? (_foundDevices = new ObservableCollection<IBluetoothLEDevice>()); }
        }
        private ObservableCollection<IBluetoothLEDevice> _foundDevices;

        #endregion

        #region Commands

        public DelegateCommand ScanCommand
        {
            get { return _scanCommand ?? (_scanCommand = new DelegateCommand(ExecuteScan, CanExecuteScan)
                    .ObservesProperty(() => IsScanning)
                    .ObservesProperty(() => IsConnecting)); }
        }
        private DelegateCommand _scanCommand;
        private async void ExecuteScan()
        {
            try
            {
                // Deletes a list of previously discovered BluetoothLEDeviceInformation.
                Devices.Clear();

                // Scan
                IsScanning = true;
                var devices = await _appServiceClient.ScanAsync(ScanTimeout);
                IsScanning = false;

                // Add to list of 
                foreach (var device in devices)
                {
                    Logger.Log($"Discovered a BLE Device. Id=[{device.Id}], Name=[{device.Name}], Address=[{device.Address}]", Category.Debug, Priority.None);
                    Devices.Add(device);
                }
            }
            catch (Exception e)
            {
                Logger.Log($"AppServiceClient.ScanAsync() fail. Exception=[{e.Message}]", Category.Exception, Priority.High);

                await _alertMessageService.ShowAsync("AppServiceClient scan fail.", "Error");
            }
        }
        private bool CanExecuteScan()
        {
            return IsScanning != true && IsConnecting != true;
        }

        public DelegateCommand<IBluetoothLEDevice> ConnectCommand
        {
            get { return _connectCommand ?? (_connectCommand = new DelegateCommand<IBluetoothLEDevice>(ExecuteConnect, CanExecuteConnect).ObservesProperty(() => IsConnecting)); }
        }
        private DelegateCommand<IBluetoothLEDevice> _connectCommand;
        private async void ExecuteConnect(IBluetoothLEDevice device)
        {
            try
            {
                IsConnecting = true;

                var uniqueID = _pairingList[device.Address];

                Logger.Log($"Connecting to Device. Id=[{device.Id}] UniqueID=[{uniqueID}]", Category.Info, Priority.High);

                uniqueID = await _appServiceClient.ConnectToDeviceAsync(device.Id, uniqueID);
                _pairingList[device.Address] = uniqueID;

                Logger.Log($"Connected to Device. Address=[{device.Address}] UniqueID=[{uniqueID}]", Category.Info, Priority.High);

                // Navigate to MainPage
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    NavigationService.Navigate("Main", device);
                });
            }
            catch (Exception e)
            {
                Logger.Log($"BluetoothLE Device Name=[{device?.Name}], Address=[{device?.Address}] Connect fail. Exception=[{e.Message}]", Category.Exception, Priority.High);

                await _alertMessageService.ShowAsync("WaterBoilerMatDevice connect fail.", "Error");
            }
            finally
            {
                IsConnecting = false;
            }
        }
        private bool CanExecuteConnect(IBluetoothLEDevice device)
        {
            return IsConnecting != true;
        }

        #endregion

        #region Fields

        private readonly IAppServiceClient _appServiceClient;
        private readonly IAlertMessageService _alertMessageService;
        private readonly IPairingList _pairingList;

        #endregion

        #region Constructors & Initialize

        public IntroPageViewModel(IAppServiceClient appServiceClient,
            INavigationService navigationService, IAlertMessageService alertMessageService,
            IPairingList pairingList,
            ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _appServiceClient = appServiceClient;
            _alertMessageService = alertMessageService;
            _pairingList = pairingList;
        }

        #endregion

        #region Event Handlers

        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            // When the IntroPage is newly navigated, it scan for WaterBoilerMatDevice.
            if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.New)
            {
                try
                {
                    if (_appServiceClient.IsOpened != true)
                    {
                        await _appServiceClient.OpenAsync();
                    }

                    if (ScanCommand.CanExecute())
                    {
                        ScanCommand.Execute();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"IntroPage.OnNavigatedTo() Exception=[{ex.Message}]", Category.Exception, Priority.High);
                }
            }
        }

        #endregion
    }
}
