using KD.Navien.WaterBoilerMat.Services;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KD.Navien.WaterBoilerMat.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
		#region Properties



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

		private async void Initialize()
		{
			Title = "Main Page";

			
		}

		#region Commands

		public DelegateCommand ScanCommand
		{
			get { return scanCommand ?? (scanCommand = new DelegateCommand(ExecuteScan, CanExecuteScan)); }
		}
		private DelegateCommand scanCommand;
		private async void ExecuteScan()
		{
			var devices = await bluetoothService.ScanAsync(5000);
			foreach (var device in devices)
			{
				Logger.Log($"Find BLE Device. Name=[{device.Name}, Address=[{device.Address}]]", Category.Debug, Priority.None);
			}
		}
		private bool CanExecuteScan()
		{
			return true;
		}

		#endregion
	}
}
