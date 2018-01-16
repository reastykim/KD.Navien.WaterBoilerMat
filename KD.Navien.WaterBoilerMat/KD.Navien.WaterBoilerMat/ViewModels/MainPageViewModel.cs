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
			if (ConnectedWaterBoilerMatDevice != null)
			{
				ConnectedWaterBoilerMatDevice.IsReadyForBoilerServiceChanged -= ConnectedWaterBoilerMatDevice_IsReadyForBoilerServiceChanged;
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
				ConnectedWaterBoilerMatDevice.IsReadyForBoilerServiceChanged += ConnectedWaterBoilerMatDevice_IsReadyForBoilerServiceChanged;
				Logger.Log($"BluetoothLE Device Name=[{waterBoilerMatDevice.Name}, Address=[{waterBoilerMatDevice.Address}] Connect success.", Category.Info, Priority.Medium);
			}
			catch (Exception e)
			{
				Logger.Log($"BluetoothLE Device Name=[{waterBoilerMatDevice.Name}, Address=[{waterBoilerMatDevice.Address}] Connect fail. Exception = [{e.Message}]", Category.Exception, Priority.High);
			}
		}

		#endregion

		#region Event Handlers
		
		private async void ConnectedWaterBoilerMatDevice_IsReadyForBoilerServiceChanged(object sender, bool e)
		{
			var device = sender as WaterBoilerMatDevice;
			Logger.Log($"IsReadyForBoilerServiceChanged. [{device.IsReadyForBoilerService}] Name=[{device.Name}, Address=[{device.Address}]]", Category.Debug, Priority.Low);
			
			var requestData = new KDRequest();
			requestData.Data.MessageType = KDMessageType.MAC_REGISTER;
			requestData.Data.UniqueID = "";
			byte[] bytes = requestData.GetValue().HexStringToByteArray();

			Logger.Log($"Connected BluetoothLE Device GattService Count={device.Services.Count}", Category.Debug, Priority.None);

			if (device.Services.Count >= 6)
			{
				IBluetoothGattService gattService = device.BoilerGattService;
				Logger.Log($"BoilerGattService Characteristics Count={gattService.GattCharacteristics.Count}", Category.Debug, Priority.None);

				var gattCharacteristic = await gattService.GetGattCharacteristicAsync()
														  .ContinueWith(T => T.Result.FirstOrDefault(C => C.UUID.Equals(WaterBoilerMatDevice.BoilerGattCharacteristic2Uuid)));

				Logger.Log($"GetGattCharacteristicAsync by [{WaterBoilerMatDevice.BoilerGattCharacteristic2Uuid}], Value=[gattCharacteristic]", Category.Debug, Priority.None);


				var result = await gattCharacteristic?.SetNotifyAsync();
				Logger.Log($"Call gattCharacteristic.SetNotifyAsync(). Result=[{result}]", Category.Debug, Priority.None);
				result = await gattCharacteristic?.WriteValueAsync(bytes);
				Logger.Log($"Call gattCharacteristic.WriteValueAsync(). Result=[{result}]", Category.Debug, Priority.None);
				//IntroActivity.this.mBluetoothLeService.setCharacteristicNotification(gattCharacteristic, IntroActivity.D);
				//IntroActivity.this.mBluetoothLeService.writeCharacteristic(gattCharacteristic);
			}
		}

		#endregion
	}
}
