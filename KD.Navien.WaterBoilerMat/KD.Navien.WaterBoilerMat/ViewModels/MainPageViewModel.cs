using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Services.Protocol;
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

		public bool IsAvailableBluetoothLEScan
		{
			get => isAvailableBluetoothLEScan;
			set => SetProperty(ref isAvailableBluetoothLEScan, value);
		}
		private bool isAvailableBluetoothLEScan = true;
		
		public ObservableCollection<WaterBoilerMatDevice> FoundDevices
		{
			get { return foundDevices ?? (foundDevices = new ObservableCollection<WaterBoilerMatDevice>()); }
		}
		private ObservableCollection<WaterBoilerMatDevice> foundDevices;

		public WaterBoilerMatDevice ConnectedWaterBoilerMatDevice
		{
			get => connectedWaterBoilerMatDevice;
			set => SetProperty(ref connectedWaterBoilerMatDevice, value);
		}
		private WaterBoilerMatDevice connectedWaterBoilerMatDevice;

		#endregion

		#region Fields

		private IBluetoothLEService<WaterBoilerMatDevice> _bluetoothService;
		private IPageDialogService _dialogService;
        private IPairingList _pairingList;

        #endregion

        public MainPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothService, IPageDialogService dialogService, 
            IPairingList pairingList,
            INavigationService navigationService, ILoggerFacade logger) 
            : base (navigationService, logger)
        {
			_bluetoothService = bluetoothService;
			_dialogService = dialogService;
            _pairingList = pairingList;

            Initialize();
        }

		private void Initialize()
		{
			Title = "Main Page";
			ExecuteScan(); // for remote-test
		}

		#region Commands

		public DelegateCommand ScanCommand
		{
			get { return scanCommand ?? (scanCommand = new DelegateCommand(ExecuteScan, CanExecuteScan).ObservesProperty(() => IsAvailableBluetoothLEScan)); }
		}
		private DelegateCommand scanCommand;
		private async void ExecuteScan()
		{
			FoundDevices.Clear();
			if (ConnectedWaterBoilerMatDevice != null)
			{
                ConnectedWaterBoilerMatDevice.Dispose();
                ConnectedWaterBoilerMatDevice = null;
            }

			IsAvailableBluetoothLEScan = false;
			var devices = await _bluetoothService.ScanAsync(5000);
			IsAvailableBluetoothLEScan = true;

			foreach (var device in devices)
			{
				Logger.Log($"Found a BLE Device. Name=[{device.Name}, Address=[{device.Address}]]", Category.Debug, Priority.Low);
				FoundDevices.Add(device);
			}
		}
		private bool CanExecuteScan()
		{
			return IsAvailableBluetoothLEScan;
		}

		public DelegateCommand<WaterBoilerMatDevice> ConnectCommand
		{
			get { return connectCommand ?? (connectCommand = new DelegateCommand<WaterBoilerMatDevice>(ExecuteConnect)); }
		}
		private DelegateCommand<WaterBoilerMatDevice> connectCommand;
		private async void ExecuteConnect(WaterBoilerMatDevice waterBoilerMatDevice)
		{
			try
			{
                if (ConnectedWaterBoilerMatDevice != null)
                {
                    ConnectedWaterBoilerMatDevice.Dispose();
                    ConnectedWaterBoilerMatDevice = null;
                }

                var uniqueID = await waterBoilerMatDevice.ConnectAsync(_pairingList[waterBoilerMatDevice.Address]);

                Logger.Log($"BluetoothLE Device Name=[{ConnectedWaterBoilerMatDevice.Name}], Address=[{ConnectedWaterBoilerMatDevice.Address}], UniqueID=[{uniqueID}] Connect success.", Category.Info, Priority.Medium);
                //Logger.Log($"BluetoothLE Device Name=[{ConnectedWaterBoilerMatDevice.Name}], Address=[{ConnectedWaterBoilerMatDevice.Address}] Connect fail.", Category.Info, Priority.Medium);
            }
			catch (Exception e)
			{
				Logger.Log($"BluetoothLE Device Name=[{waterBoilerMatDevice?.Name}], Address=[{waterBoilerMatDevice?.Address}] Connect fail. Exception=[{e.Message}]", Category.Exception, Priority.High);
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

        #region Event Handlers

		#endregion
	}
}
