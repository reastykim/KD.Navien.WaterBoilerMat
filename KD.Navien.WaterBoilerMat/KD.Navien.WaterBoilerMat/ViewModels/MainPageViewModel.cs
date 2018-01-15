﻿using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
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

		public WaterBoilerMatDevice SelectedFoundDevice
		{
			get => selectedFoundDevice;
			set => SetProperty(ref selectedFoundDevice, value);
		}
		private WaterBoilerMatDevice selectedFoundDevice;

		#endregion

		#region Fields

		private IBluetoothService bluetoothService;
		private IPageDialogService dialogService;

		#endregion

		public MainPageViewModel(IBluetoothService bluetoothService, IPageDialogService dialogService, INavigationService navigationService, ILoggerFacade logger) 
            : base (navigationService, logger)
        {
			this.bluetoothService = bluetoothService;
			this.dialogService = dialogService;

			Initialize();
        }

		private void Initialize()
		{
			Title = "Main Page";

			
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
			SelectedFoundDevice = null;

			IsAvailableBluetoothLEScan = false;
			var devices = await bluetoothService.ScanAsync(5000);
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

		public DelegateCommand ConnectCommand
		{
			get { return connectCommand ?? (connectCommand = new DelegateCommand(ExecuteConnect, CanExecuteConnect).ObservesProperty(() => SelectedFoundDevice)); }
		}
		private DelegateCommand connectCommand;
		private async void ExecuteConnect()
		{
			try
			{
				await SelectedFoundDevice.ConnectAsync();
				Logger.Log($"BluetoothLE Device Name=[{SelectedFoundDevice.Name}, Address=[{SelectedFoundDevice.Address}] Connect success.", Category.Info, Priority.Medium);
			}
			catch (Exception e)
			{
				Logger.Log($"BluetoothLE Device Name=[{SelectedFoundDevice.Name}, Address=[{SelectedFoundDevice.Address}] Connect fail. Exception = [{e.Message}]", Category.Exception, Priority.High);
			}
		}
		private bool CanExecuteConnect()
		{
			return SelectedFoundDevice != null;
		}

		#endregion
	}
}
