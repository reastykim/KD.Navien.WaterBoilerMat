using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using KD.Navien.WaterBoilerMat.Universal.RemoteApp.Views;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
using KD.Navien.WaterBoilerMat.Universal.Models;
using KD.Navien.WaterBoilerMat.Universal.Services;
using Microsoft.Toolkit.Uwp.Helpers;
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

namespace KD.Navien.WaterBoilerMat.Universal.RemoteApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Properties

        public IWaterBoilerMatDeviceInformation DeviceInformation
        {
            get => _devicInformatione;
            private set => SetProperty(ref _devicInformatione, value);
        }
        private IWaterBoilerMatDeviceInformation _devicInformatione;

        #endregion

        #region Commands

        public DelegateCommand PowerCommand
        {
            get { return _powerCommand ?? (_powerCommand = new DelegateCommand(ExecutePower)); }
        }
        private DelegateCommand _powerCommand;
        private async void ExecutePower()
        {
            try
            {
                await _appServiceClient.RequestPowerOnOffAsync();
            }
            catch (Exception ex)
            {
                Logger.Log($"PowerCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice Power command execute fail.", "Error");
                });
            }
        }

        public DelegateCommand LockCommand
        {
            get { return _lockCommand ?? (_lockCommand = new DelegateCommand(ExecuteLock)); }
        }
        private DelegateCommand _lockCommand;
        private async void ExecuteLock()
        {
            try
            {
                await _appServiceClient.RequestLockOnOffAsync();
            }
            catch (Exception ex)
            {
                Logger.Log($"PowerCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice Power command execute fail.", "Error");
                });
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

        private readonly IAlertMessageService _alertMessageService;
        private readonly IAppServiceClient _appServiceClient;

        #endregion

        #region Constructors & Initialize

        public MainPageViewModel(IAppServiceClient appServiceClient, INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _appServiceClient = appServiceClient;
            _alertMessageService = alertMessageService;

            Initialize();
        }

        private void Initialize()
        {
            Title = "Home";
        }

        #endregion

        #region Event Handlers

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _appServiceClient.WaterBoilerMatDeviceInformationUpdated += OnWaterBoilerMatDeviceInformationUpdated;

            base.OnNavigatedTo(e, viewModelState);
        }

        public override async void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            _appServiceClient.WaterBoilerMatDeviceInformationUpdated -= OnWaterBoilerMatDeviceInformationUpdated;

            if (_appServiceClient.ConnectedBluetoothLEDeviceInformation != null)
            {
                await _appServiceClient.DisconnectToDeviceAsync(_appServiceClient.ConnectedBluetoothLEDeviceInformation.Id);
            }

            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        private void OnWaterBoilerMatDeviceInformationUpdated(object sender, WaterBoilerMatDeviceInformation e)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                DeviceInformation = e;
            });
        }

        #endregion

        #region Methods

        #endregion
    }
}
