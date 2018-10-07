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

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class HomePageViewModel : ViewModelBase, INavigationViewItemPageAware
    {

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
                KDRequest requestData = new KDRequest();
                requestData.Data = _response.Data;
                requestData.Data.Mode = 6;
                requestData.Data.Power = _response.Data.Power == 0 ? 1 : 0;
                requestData.Data.SleepStartButtonEnable = 0;
                requestData.Data.SleepStopButtonEnable = 1;

                var requestDataValue = requestData.GetValue();
                byte[] bytes = requestDataValue.HexStringToByteArray();

                var result = await _connectedDevice.BoilerGattCharacteristic1.SetNotifyAsync(true);
                if (result != true)
                {
                    throw new ApplicationException($"Call BoilerCharacteristic1.SetNotifyAsync(true). Result=[{result}]");
                }

                result = await _connectedDevice.BoilerGattCharacteristic1.WriteValueAsync(bytes);
                if (result)
                {
                    Logger.Log($"BoilerGattCharacteristic1.WriteValueAsync(). Value = [{requestDataValue}]", Category.Debug, Priority.High);
                }
                else
                {
                    throw new ApplicationException($"Call BoilerCharacteristic1.WriteValueAsync(). Result=[{result}], Data=[{requestDataValue}]");
                }
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
        private WaterBoilerMatDevice _connectedDevice;
        private KDResponse _response;

        #endregion

        public HomePageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _alertMessageService = alertMessageService;
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is WaterBoilerMatDevice connectedDevice)
            {
                _connectedDevice = connectedDevice;
                _connectedDevice.BoilerGattCharacteristic1.ValueChanged += BoilerGattCharacteristic1_ValueChanged;
            }
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            _connectedDevice.BoilerGattCharacteristic1.ValueChanged -= BoilerGattCharacteristic1_ValueChanged;
        }

        private void BoilerGattCharacteristic1_ValueChanged(object sender, byte[] e)
        {
            KDResponse response = new KDResponse();
            if (response.SetValue(e))
            {
                _response = response;
            }
        }
    }
}
