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
        #region Fields

        private IBluetoothLEService<WaterBoilerMatDevice> _bluetoothService;

        public IWaterBoilerMatDevice Device { get; private set; }

        #endregion

        #region Constructors & Initialize

        public MainPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothService, INavigationService navigationService, ILoggerFacade logger,
            IWaterBoilerMatDevice device) 
            : base (navigationService, logger)
        {
			_bluetoothService = bluetoothService;
            Device = device;

            Initialize();
        }

		private void Initialize()
		{
			Title = "NAVIEN MATE";
		}

        #endregion

        #region Event Handlers
        
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            Logger.Log("MainPageViewModel.OnNavigatedFrom", Category.Debug, Priority.High);

            Device.Disconnect();
            Device.Dispose();
        }

        #endregion
    }
}
