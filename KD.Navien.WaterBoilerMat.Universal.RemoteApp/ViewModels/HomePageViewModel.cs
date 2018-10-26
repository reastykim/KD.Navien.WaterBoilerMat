using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using KD.Navien.WaterBoilerMat.Universal.Common;
using KD.Navien.WaterBoilerMat.Universal.Services;
using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Logging;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.Universal.RemoteApp.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        const int MinimumSleepModeTemperature = 30;
        const int MaximumSleepModeTemperature = 35;

        #region Properties

        public IWaterBoilerMatDeviceInformation DeviceInformation
        {
            get => _deviceInformation;
            private set => SetProperty(ref _deviceInformation, value);
        }
        private IWaterBoilerMatDeviceInformation _deviceInformation;

        public int SetupLeftTemperature
        {
            get => _setupLeftTemperature;
            set
            {
                if (SetProperty(ref _setupLeftTemperature, value))
                {
                    CanSleepModeLeft = value >= MinimumSleepModeTemperature && value <= MaximumSleepModeTemperature;

                    if (SetChangeAllTemperatures)
                    {
                        SetupRightTemperature = value;
                    }
                }
            }
        }
        private int _setupLeftTemperature;

        public int SetupRightTemperature
        {
            get => _setupRightTemperature;
            set
            {
                if (SetProperty(ref _setupRightTemperature, value))
                {
                    CanSleepModeRight = value >= MinimumSleepModeTemperature && value <= MaximumSleepModeTemperature;

                    if (SetChangeAllTemperatures)
                    {
                        SetupLeftTemperature = value;
                    }
                }
            }
        }
        private int _setupRightTemperature;

        public bool CanSleepModeLeft
        {
            get => _canSleepModeLeft;
            private set => SetProperty(ref _canSleepModeLeft, value);
        }
        private bool _canSleepModeLeft;

        public bool CanSleepModeRight
        {
            get => _canSleepModeRight;
            private set => SetProperty(ref _canSleepModeRight, value);
        }
        private bool _canSleepModeRight;

        public bool SetChangeAllTemperatures
        {
            get => _setChangeAllTemperatures;
            set => SetProperty(ref _setChangeAllTemperatures, value);
        }
        private bool _setChangeAllTemperatures;

        public IEnumerable<VolumeLevels> VolumeLevels => Enum.GetValues(typeof(VolumeLevels)).OfType<VolumeLevels>();

        public VolumeLevels SelectedVolumeLevel
        {
            get => _selectedVolumeLevel;
            set
            {
                if (SetProperty(ref _selectedVolumeLevel, value))
                {
                    _appServiceClient.RequestVolumeChangeAsync(value);
                }
            }
        }
        private VolumeLevels _selectedVolumeLevel;

        #endregion

        #region Commands

        public DelegateCommand LeftPartsPowerCommand
        {
            get
            {
                return _leftPartsPowerCommand ?? (_leftPartsPowerCommand = new DelegateCommand(ExecuteLeftPartsPower).ObservesCanExecute(() => DeviceInformation.IsPowerOn));
            }
        }
        private DelegateCommand _leftPartsPowerCommand;
        private async void ExecuteLeftPartsPower()
        {
            try
            {
                await _appServiceClient.RequestLeftPartsPowerOnOffAsync();
            }
            catch (Exception ex)
            {
                Logger.Log($"LeftPartsPowerCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice LeftPartsPower command execute fail.", "Error");
                });
            }
        }

        public DelegateCommand RightPartsPowerCommand
        {
            get
            {
                return _rightPartsPowerCommand ?? (_rightPartsPowerCommand = new DelegateCommand(ExecuteRightPartsPower).ObservesCanExecute(() => DeviceInformation.IsPowerOn));
            }
        }
        private DelegateCommand _rightPartsPowerCommand;
        private async void ExecuteRightPartsPower()
        {
            try
            {
                await _appServiceClient.RequestRightPartsPowerOnOffAsync();
            }
            catch (Exception ex)
            {
                Logger.Log($"RightPartsPowerCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice RightPartsPower command execute fail.", "Error");
                });
            }
        }

        public DelegateCommand SetTemperatureCommand
        {
            get
            {
                return _setTemperatureCommand ?? (_setTemperatureCommand = new DelegateCommand(ExecuteSetTemperature).ObservesCanExecute(() => DeviceInformation.IsPowerOn));
            }
        }
        private DelegateCommand _setTemperatureCommand;
        private async void ExecuteSetTemperature()
        {
            try
            {
                await _appServiceClient.RequestSetupTemperatureChangeAsync(SetupLeftTemperature, SetupRightTemperature);

                Logger.Log($"SetupLeftTemperature=[{DeviceInformation.SetupLeftTemperature}], SetupRightTemperature=[{DeviceInformation.SetupRightTemperature}]", Category.Info, Priority.None);
            }
            catch (Exception ex)
            {
                Logger.Log($"SetTemperatureCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice SetTemperature command execute fail.", "Error");
                });
            }
        }

        public DelegateCommand<Object> SetVolumeLevelCommand
        {
            get
            {
                return _setVolumeLevelCommand ?? (_setVolumeLevelCommand = new DelegateCommand<Object>(ExecuteSetVolumeLevel).ObservesCanExecute(() => DeviceInformation.IsPowerOn));
            }
        }
        private DelegateCommand<Object> _setVolumeLevelCommand;
        private async void ExecuteSetVolumeLevel(Object args)
        {
            try
            {
                //await _appServiceClient.RequestVolumeChangeAsync(SelectedVolumeLevel);

                //Logger.Log($"VolumeLevel=[{DeviceInformation.VolumeLevel}]", Category.Info, Priority.None);
            }
            catch (Exception ex)
            {
                Logger.Log($"SetVolumeLevelCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice SetVolumeLevel command execute fail.", "Error");
                });
            }
        }

        #endregion

        #region Fields

        private readonly IAppServiceClient _appServiceClient;
        private readonly IAlertMessageService _alertMessageService;

        #endregion

        #region Constructors & Initialize

        public HomePageViewModel(IAppServiceClient appServiceClient, INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
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
        
        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            _appServiceClient.WaterBoilerMatDeviceInformationUpdated += OnWaterBoilerMatDeviceInformationUpdated;

            base.OnNavigatedTo(e, viewModelState);
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            _appServiceClient.WaterBoilerMatDeviceInformationUpdated -= OnWaterBoilerMatDeviceInformationUpdated;

            base.OnNavigatingFrom(e, viewModelState, suspending);
        }

        private void OnDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DeviceInformation.IsPowerOn):
                case nameof(DeviceInformation.SetupLeftTemperature):
                case nameof(DeviceInformation.SetupRightTemperature):
                    SetupLeftTemperature = DeviceInformation.SetupLeftTemperature;
                    SetupRightTemperature = DeviceInformation.SetupRightTemperature;
                    break;
            }
        }

        private void OnWaterBoilerMatDeviceInformationUpdated(object sender, WaterBoilerMatDeviceInformation e)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                if (DeviceInformation == null)
                {
                    SetupLeftTemperature = e.SetupLeftTemperature;
                    SetupRightTemperature = e.SetupRightTemperature;
                }
                else
                {
                    if (DeviceInformation.IsPowerOn != e.IsPowerOn ||
                        DeviceInformation.SetupLeftTemperature != e.SetupLeftTemperature ||
                        DeviceInformation.SetupRightTemperature != e.SetupRightTemperature)
                    {
                        SetupLeftTemperature = DeviceInformation.SetupLeftTemperature;
                        SetupRightTemperature = DeviceInformation.SetupRightTemperature;
                    }
                }

                DeviceInformation = e;
            });
        }
    }
}
