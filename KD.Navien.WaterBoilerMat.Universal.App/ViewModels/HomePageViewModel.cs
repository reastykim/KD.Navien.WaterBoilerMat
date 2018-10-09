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

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class HomePageViewModel : ViewModelBase, INavigationViewItemPageAware
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
            get
            {
                return _powerCommand ?? (_powerCommand = new DelegateCommand(ExecutePower));
            }
        }
        private DelegateCommand _powerCommand;
        private async void ExecutePower()
        {
            try
            {
                await _device.RequestPowerOnOffAsync(!_device.IsPowerOn);
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

        #endregion

        #region Fields

        private readonly IAlertMessageService _alertMessageService;

        #endregion

        public HomePageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _alertMessageService = alertMessageService;
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is IWaterBoilerMatDevice device)
            {
                _device = device;
            }
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {

        }
    }
}
