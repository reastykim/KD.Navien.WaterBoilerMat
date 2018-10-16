using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using KD.Navien.WaterBoilerMat.Universal.App.Views;
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

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region Properties

        public ObservableCollection<NavigationViewItemData> NavigationViewItemDataCollection
        {
            get
            {
                if (_navigationViewItemDataCollection == null)
                {
                    _navigationViewItemDataCollection = new ObservableCollection<NavigationViewItemData>();
                    _navigationViewItemDataCollection.Add(new NavigationViewItemData { Name = "Home", TextIcon = "\uE80F", TargetPageType = typeof(HomePage) });
                    _navigationViewItemDataCollection.Add(new NavigationViewItemData { Name = "Sleep", TextIcon = "\uE708", TargetPageType = typeof(SleepPage) }); // uEC46
                    _navigationViewItemDataCollection.Add(new NavigationViewItemData { Name = "Help", TextIcon = "\uE946", TargetPageType = typeof(HelpPage) }); // uE897 uE946 uE82D
                    //_navigationViewItemDataCollection.Add(new NavigationViewItemData { Name = "Settings", TextIcon = "\uE713", TargetPageType = typeof(SettingsPage) });
                }

                return _navigationViewItemDataCollection;
            }
        }
        private ObservableCollection<NavigationViewItemData> _navigationViewItemDataCollection;

        public NavigationViewItemData SelectedNavigationViewItemData
        {
            get => _selectedNavigationViewItemData;
            set => SetProperty(ref _selectedNavigationViewItemData, value);
        }
        private NavigationViewItemData _selectedNavigationViewItemData;

        public WaterBoilerMatDevice Device
        {
            get => _device;
            private set => SetProperty(ref _device, value);
        }
        private WaterBoilerMatDevice _device;

        #endregion

        #region Commands

        protected override void ExecuteLoaded()
        {
            SelectedNavigationViewItemData = NavigationViewItemDataCollection.Single(I => I.TargetPageType == typeof(HomePage));
        }

        public DelegateCommand PowerCommand
        {
            get { return _powerCommand ?? (_powerCommand = new DelegateCommand(ExecutePower)); }
        }
        private DelegateCommand _powerCommand;
        private async void ExecutePower()
        {
            try
            {
                await Device.RequestPowerOnOffAsync();
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
                await Device.RequestLockOnOffAsync();
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

        #endregion

        #region Constructors & Initialize

        public MainPageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
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

        #region Event Handlers

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            if (e.Parameter is WaterBoilerMatDevice connectedDevice)
            {
                Device = connectedDevice;
            }
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            if (Device?.IsConnected == true)
            {
                Device.Disconnect();
                Device.Dispose();
            }
        }

        #endregion

        #region Methods

        #endregion
    }
}
