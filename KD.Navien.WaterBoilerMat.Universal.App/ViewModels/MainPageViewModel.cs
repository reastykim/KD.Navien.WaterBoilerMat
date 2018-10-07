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

        #endregion

        #region Commands

        protected override void ExecuteLoaded()
        {
            SelectedNavigationViewItemData = NavigationViewItemDataCollection.Single(I => I.TargetPageType == typeof(HomePage));
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

        private WaterBoilerMatDevice _connectedDevice;

        #endregion

        #region Constructors & Initialize

        public MainPageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _alertMessageService = alertMessageService;
        }

        #endregion

        #region Event Handlers

        public override void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            if (e.Parameter is WaterBoilerMatDevice connectedDevice)
            {
                _connectedDevice = connectedDevice;
                InitializeDevice(connectedDevice);

                foreach (var itemData in NavigationViewItemDataCollection)
                {
                    itemData.Tag = _connectedDevice;
                }
            }
        }

        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            if (_connectedDevice?.IsConnected == true)
            {
                _connectedDevice.Disconnect();
                _connectedDevice = null;
            }
        }

        private void OnBoilerGattCharacteristic1_ValueChanged(object sender, byte[] e)
        {
            KDResponse response = new KDResponse();
            if (response.SetValue(e))
            {
                System.Diagnostics.Debug.WriteLine($"DEBUGCode = [{response.Data.DEBUGCode}]");
            }
        }

        #endregion

        #region Methods

        private async void InitializeDevice(WaterBoilerMatDevice device)
        {
            try
            {
                var result = await device.BoilerGattCharacteristic1.SetNotifyAsync(true);
                if (result != true)
                {
                    throw new ApplicationException($"Call BoilerCharacteristic1.SetNotifyAsync(true). Result=[{result}]");
                }

                device.BoilerGattCharacteristic1.ValueChanged += OnBoilerGattCharacteristic1_ValueChanged;
                // Write a dummy packet
                result = await device.BoilerGattCharacteristic1.WriteValueAsync(new byte[0]);
                if (result)
                {
                    Logger.Log($"BoilerGattCharacteristic1.WriteValueAsync(). Value = []", Category.Debug, Priority.High);
                }
                else
                {
                    throw new ApplicationException($"Call BoilerCharacteristic1.WriteValueAsync(). Result=[{result}], Data=[]");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"InitializeDevice. Exception=[{ex.Message}]", Category.Exception, Priority.High);

                await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                {
                    await _alertMessageService.ShowAsync("WaterBoilerMatDevice initialize fail.", "Error");
                });
            }
        }

        #endregion
    }
}
