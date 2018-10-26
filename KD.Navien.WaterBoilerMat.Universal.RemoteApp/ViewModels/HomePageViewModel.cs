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

        public IWaterBoilerMatDevice Device
        {
            get => _device;
            private set => SetProperty(ref _device, value);
        }
        private IWaterBoilerMatDevice _device;

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
                    Device.RequestVolumeChangeAsync(value);
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
                return _leftPartsPowerCommand ?? (_leftPartsPowerCommand = new DelegateCommand(ExecuteLeftPartsPower).ObservesCanExecute(() => Device.IsPowerOn));
            }
        }
        private DelegateCommand _leftPartsPowerCommand;
        private async void ExecuteLeftPartsPower()
        {
            try
            {
                await Device.RequestLeftPartsPowerOnOffAsync();
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
                return _rightPartsPowerCommand ?? (_rightPartsPowerCommand = new DelegateCommand(ExecuteRightPartsPower).ObservesCanExecute(() => Device.IsPowerOn));
            }
        }
        private DelegateCommand _rightPartsPowerCommand;
        private async void ExecuteRightPartsPower()
        {
            try
            {
                await Device.RequestRightPartsPowerOnOffAsync();
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
                return _setTemperatureCommand ?? (_setTemperatureCommand = new DelegateCommand(ExecuteSetTemperature).ObservesCanExecute(() => Device.IsPowerOn));
            }
        }
        private DelegateCommand _setTemperatureCommand;
        private async void ExecuteSetTemperature()
        {
            try
            {
                await Device.RequestSetupTemperatureChangeAsync(SetupLeftTemperature, SetupRightTemperature);

                Logger.Log($"SetupLeftTemperature=[{Device.SetupLeftTemperature}], SetupRightTemperature=[{Device.SetupRightTemperature}]", Category.Info, Priority.None);
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
                return _setVolumeLevelCommand ?? (_setVolumeLevelCommand = new DelegateCommand<Object>(ExecuteSetVolumeLevel).ObservesCanExecute(() => Device.IsPowerOn));
            }
        }
        private DelegateCommand<Object> _setVolumeLevelCommand;
        private async void ExecuteSetVolumeLevel(Object args)
        {
            try
            {
                await Device.RequestVolumeChangeAsync(SelectedVolumeLevel);

                Logger.Log($"VolumeLevel=[{Device.VolumeLevel}]", Category.Info, Priority.None);
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

        private readonly IAlertMessageService _alertMessageService;

        #endregion

        #region Constructors & Initialize

        public HomePageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
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
            base.OnNavigatedTo(e, viewModelState);
            if (e.Parameter is IWaterBoilerMatDevice device)
            {
                Device = device;
                SetupLeftTemperature = Device.SetupLeftTemperature;
                SetupRightTemperature = Device.SetupRightTemperature;
                _selectedVolumeLevel = Device.VolumeLevel;

                Device.PropertyChanged += OnDevice_PropertyChanged;
            }
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatingFrom(e, viewModelState, suspending);

            Device.PropertyChanged -= OnDevice_PropertyChanged;
        }

        private void OnDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Device.IsPowerOn):
                case nameof(Device.SetupLeftTemperature):
                case nameof(Device.SetupRightTemperature):
                    SetupLeftTemperature = Device.SetupLeftTemperature;
                    SetupRightTemperature = Device.SetupRightTemperature;
                    break;
            }
        }
    }
}
