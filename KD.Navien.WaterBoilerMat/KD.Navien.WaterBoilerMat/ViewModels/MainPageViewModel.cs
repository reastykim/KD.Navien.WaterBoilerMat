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

		private IBluetoothLEService<WaterBoilerMatDevice> bluetoothService;
		private IPageDialogService dialogService;

		#endregion

		public MainPageViewModel(IBluetoothLEService<WaterBoilerMatDevice> bluetoothService, IPageDialogService dialogService, INavigationService navigationService, ILoggerFacade logger) 
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
			if (ConnectedWaterBoilerMatDevice != null)
			{
				ConnectedWaterBoilerMatDevice.ServicesUpdated -= ConnectedWaterBoilerMatDevice_ServicesUpdated;
			}
			ConnectedWaterBoilerMatDevice = null;


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

		public DelegateCommand<WaterBoilerMatDevice> ConnectCommand
		{
			get { return connectCommand ?? (connectCommand = new DelegateCommand<WaterBoilerMatDevice>(ExecuteConnect)); }
		}
		private DelegateCommand<WaterBoilerMatDevice> connectCommand;
		private async void ExecuteConnect(WaterBoilerMatDevice waterBoilerMatDevice)
		{
			try
			{
				await waterBoilerMatDevice.ConnectAsync();
				ConnectedWaterBoilerMatDevice = waterBoilerMatDevice;
				ConnectedWaterBoilerMatDevice.ServicesUpdated += ConnectedWaterBoilerMatDevice_ServicesUpdated;
				Logger.Log($"BluetoothLE Device Name=[{waterBoilerMatDevice.Name}], Address=[{waterBoilerMatDevice.Address}] Connect success.", Category.Info, Priority.Medium);
			}
			catch (Exception e)
			{
				Logger.Log($"BluetoothLE Device Name=[{waterBoilerMatDevice?.Name}], Address=[{waterBoilerMatDevice?.Address}] Connect fail. Exception=[{e.Message}]", Category.Exception, Priority.High);
			}
		}

		#endregion

		#region Event Handlers
		
		private async void ConnectedWaterBoilerMatDevice_ServicesUpdated(object sender, EventArgs e)
		{
			var device = sender as WaterBoilerMatDevice;
			var boilerGattService = device.Services.FirstOrDefault(S => S.UUID == WaterBoilerMatDevice.BoilerGattServiceUuid);
			var boilerGattCharacteristic1 = boilerGattService.GattCharacteristics.FirstOrDefault(C => C.UUID == WaterBoilerMatDevice.BoilerGattCharacteristic1Uuid);
			var boilerGattCharacteristic2 = boilerGattService.GattCharacteristics.FirstOrDefault(C => C.UUID == WaterBoilerMatDevice.BoilerGattCharacteristic2Uuid);

			if (boilerGattService == null || boilerGattCharacteristic1 == null || boilerGattCharacteristic2 == null)
			{
				Logger.Log($"BoilerGattService is not ready.", Category.Debug, Priority.Medium);
				return;
			}

			Logger.Log($"BoilerGattService is Ready.", Category.Info, Priority.Medium);

			try
			{
				var requestData = new KDRequest();
				requestData.Data.MessageType = KDMessageType.MAC_REGISTER;
				requestData.Data.UniqueID = "";
				byte[] bytes = requestData.GetValue().HexStringToByteArray();

				var result = await boilerGattCharacteristic2.SetNotifyAsync();
				Logger.Log($"Call BoilerCharacteristic2.SetNotifyAsync(). Result=[{result}]", Category.Debug, Priority.None);
				result = await boilerGattCharacteristic2.WriteValueAsync(bytes);
				Logger.Log($"Call BoilerCharacteristic2.WriteValueAsync(). Result=[{result}]", Category.Debug, Priority.None);
			}
			catch (Exception ex)
			{
				Logger.Log($"WaterBoilerMatDevice Communication fail. Exception=[{ex.Message}]", Category.Exception, Priority.High);
			}
		}

		#endregion
	}
}
