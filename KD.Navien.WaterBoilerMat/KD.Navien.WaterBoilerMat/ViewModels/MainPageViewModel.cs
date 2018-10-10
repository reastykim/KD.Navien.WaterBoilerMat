using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using KD.Navien.WaterBoilerMat.Views;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.ViewModels
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

        #region Fields

        private IBluetoothLEService<WaterBoilerMatDevice> _bluetoothService;

        public WaterBoilerMatDevice Device { get; private set; }

        #endregion

        #region Constructors & Initialize

        public MainPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothService, INavigationService navigationService, ILoggerFacade logger) 
            : base (navigationService, logger)
        {
			_bluetoothService = bluetoothService;

            Initialize();
        }

		private void Initialize()
		{
			Title = "NAVIEN MATE";
		}

        #endregion

        #region Event Handlers

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("DEVICE"))
            {
                Device = parameters["DEVICE"] as WaterBoilerMatDevice;
            }
        }

        #endregion
    }
}
