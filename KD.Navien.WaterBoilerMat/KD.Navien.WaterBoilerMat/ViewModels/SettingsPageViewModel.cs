using KD.Navien.WaterBoilerMat.Models;
using Prism;
using Prism.Commands;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.ViewModels
{
    public class SettingsPageViewModel : TabbedItemViewModelBase
    {
        #region Properties
        
        public IWaterBoilerMatDevice Device
        {
            get => _device;
            private set => SetProperty(ref _device, value);
        }
        private IWaterBoilerMatDevice _device;

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
                await _device.RequestPowerOnOffAsync();
            }
            catch (Exception ex)
            {
                Logger.Log($"PowerCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await _dialogService.DisplayAlertAsync("Error", "WaterBoilerMatDevice Power command execute fail.", "OK");
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
                await _device.RequestLockOnOffAsync();
            }
            catch (Exception ex)
            {
                Logger.Log($"PowerCommand execute fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await _dialogService.DisplayAlertAsync("Error", "WaterBoilerMatDevice Power command execute fail.", "OK");
            }
        }

        #endregion

        #region Fields

        private readonly IPageDialogService _dialogService;

        #endregion

        #region Constructors & Initialize & Destroy

        public SettingsPageViewModel(IPageDialogService dialogService, INavigationService navigationService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _dialogService = dialogService;

            Initialize();
        }

        private void Initialize()
        {
            IsActiveChanged += HandleIsActiveTrue;
            IsActiveChanged += HandleIsActiveFalse;

            Title = "Settings";
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

            // Implement your implementation logic here...
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            //if (e.Parameter is IWaterBoilerMatDevice device)
            //{
            //    _device = device;
            //}
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
