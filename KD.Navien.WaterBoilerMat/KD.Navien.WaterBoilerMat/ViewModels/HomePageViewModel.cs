using KD.Navien.WaterBoilerMat.Models;
using Prism;
using Prism.Commands;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KD.Navien.WaterBoilerMat.ViewModels
{
    public class HomePageViewModel : TabbedItemViewModelBase
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

                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    _dialogService.DisplayAlertAsync("Error", "WaterBoilerMatDevice LeftPartsPower command execute fail.", "OK");
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

                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    _dialogService.DisplayAlertAsync("Error", "WaterBoilerMatDevice RightPartsPower command execute fail.", "OK");
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

                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    _dialogService.DisplayAlertAsync("Error", "WaterBoilerMatDevice SetTemperature command execute fail.", "OK");
                });
            }
        }

        public DelegateCommand<Object> SetVolumeLevelCommand
        {
            get
            {
                return _setVolumeLevelCommand ?? (_setVolumeLevelCommand = new DelegateCommand<Object>(ExecuteSetVolumeLevel).ObservesCanExecute(() => Device.IsPowerOn == true));
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

                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    _dialogService.DisplayAlertAsync("Error", "WaterBoilerMatDevice SetVolumeLevel command execute fail.", "OK");
                });
            }
        }

        #endregion

        #region Fields

        private readonly IPageDialogService _dialogService;

        #endregion

        #region Constructors & Initialize & Destroy

        public HomePageViewModel(IPageDialogService dialogService, INavigationService navigationService, ILoggerFacade logger,
            IWaterBoilerMatDevice device)
            : base(navigationService, logger)
        {
            _dialogService = dialogService;
            Device = device;

            Initialize();
        }

        private void Initialize()
        {
            IsActiveChanged += HandleIsActiveTrue;
            IsActiveChanged += HandleIsActiveFalse;

            Title = "Home";
        }

        public override void Destroy()
        {
            IsActiveChanged -= HandleIsActiveTrue;
            IsActiveChanged -= HandleIsActiveFalse;
        }

        #endregion

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (HasInitialized) return;

            HasInitialized = true;
        }

        private void HandleIsActiveTrue(object sender, EventArgs args)
        {
            if (IsActive == false) return;

            // Handle Logic Here
        }

        private void HandleIsActiveFalse(object sender, EventArgs args)
        {
            if (IsActive == true) return;

            // Handle Logic Here
        }
    }
}
