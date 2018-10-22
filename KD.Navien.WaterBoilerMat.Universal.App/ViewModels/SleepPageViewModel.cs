using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Universal.Services;
using Prism.Logging;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class SleepPageViewModel : ViewModelBase
    {
        #region Properties

        public IWaterBoilerMatDevice Device
        {
            get => _device;
            private set => SetProperty(ref _device, value);
        }
        private IWaterBoilerMatDevice _device;

        public string StatusDescription
        {
            get { return _statusDescription; }
            private set { SetProperty(ref _statusDescription, value); }
        }
        private string _statusDescription;

        #endregion

        #region Fields

        private readonly IAlertMessageService _alertMessageService;

        #endregion

        #region Constructors & Initialize

        public SleepPageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _alertMessageService = alertMessageService;

            Initialize();
        }

        private void Initialize()
        {
            Title = "Sleep";
        }

        #endregion

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            if (e.Parameter is IWaterBoilerMatDevice device)
            {
                Device = device;

                Device.PropertyChanged += OnDevice_PropertyChanged;

                UpdateStatusDescription();
            }
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatingFrom(e, viewModelState, suspending);

            Device.PropertyChanged -= OnDevice_PropertyChanged;
        }

        private void OnDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateStatusDescription();
        }

        private void UpdateStatusDescription()
        {
            switch (Device.Status)
            {
                case DeviceStatus.Off:
                    StatusDescription = "POWER OFF";
                    break;
                case DeviceStatus.LeftOnlyOn:
                    StatusDescription = "현재 좌측 영역만 수면 모드를 설정할 수 있습니다.";
                    break;
                case DeviceStatus.RightOnlyOn:
                    StatusDescription = "현재 우측 영역만 수면 모드를 설정할 수 있습니다.";
                    break;
                case DeviceStatus.On:
                    StatusDescription = "";
                    break;
            }
        }
    }
}
