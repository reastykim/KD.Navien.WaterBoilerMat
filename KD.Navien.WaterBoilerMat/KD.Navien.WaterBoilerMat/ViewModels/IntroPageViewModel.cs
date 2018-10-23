using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Views;
using Prism.Commands;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Unity;

namespace KD.Navien.WaterBoilerMat.ViewModels
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

        public DelegateCommand ScanCommand
        {
            get
            {
                return _scanCommand ?? (_scanCommand = new DelegateCommand(ExecuteScan, CanExecuteScan)
                    .ObservesProperty(() => IsScanning)
                    .ObservesProperty(() => IsConnecting));
            }
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
                IsConnecting = true;

                var uniqueID = _pairingList[device.Address];

                Logger.Log($"Connect to Device. Address=[{device.Address}] UniqueID=[{uniqueID}]", Category.Info, Priority.High);

                uniqueID = await device.ConnectAsync(uniqueID);
                _pairingList[device.Address] = uniqueID;

                Logger.Log($"Connected to Device. Address=[{device.Address}] UniqueID=[]", Category.Info, Priority.High);

                _container.RegisterInstance<IWaterBoilerMatDevice>(device);

                // Navigate to MainPage
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add("DEVICE", device);
                await NavigationService.NavigateAsync($"{nameof(MainPage)}", navigationParameters);
            }
            catch (Exception e)
            {
                Logger.Log($"BluetoothLE Device Name=[{device?.Name}], Address=[{device?.Address}] Connect fail. Exception=[{e.Message}]", Category.Exception, Priority.High);

                await _dialogService.DisplayAlertAsync("Error", "WaterBoilerMatDevice connect fail.", "OK");
            }
            finally
            {
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
        private readonly IPageDialogService _dialogService;
        private readonly IPairingList _pairingList;
        private readonly IUnityContainer _container;

        #endregion

        #region Constructors & Initialize

        public IntroPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothLEService, 
            IPairingList pairingList, IPageDialogService dialogService,
            INavigationService navigationService, ILoggerFacade logger, IUnityContainer container)
            : base(navigationService, logger)
        {
            _bluetoothLEService = bluetoothLEService;
            _dialogService = dialogService;
            _pairingList = pairingList;
            _container = container;

            Initialize();
        }

        private void Initialize()
        {
            Title = "NAVIEN MATE";
        }

        #endregion

        #region Event Handlers

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (ScanCommand.CanExecute())
            {
                ScanCommand.Execute();
            }
        }

        #endregion
    }
}
