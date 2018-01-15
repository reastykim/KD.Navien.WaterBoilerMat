using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.UWP.Models;
using KD.Navien.WaterBoilerMat.UWP.Services;
using Microsoft.Toolkit.Uwp.Connectivity;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothService))]
namespace KD.Navien.WaterBoilerMat.UWP.Services
{
	public class BluetoothService : IBluetoothService
	{
		#region Properties

		public bool IsScanning => bluetoothLEHelper.IsEnumerating;

		#endregion

		#region Fields

		private ILoggerFacade logger;
		private BluetoothLEHelper bluetoothLEHelper;

		#endregion

		#region Constructors & Initialize

		public BluetoothService(ILoggerFacade logger)
		{
			this.logger = logger;

			Initialize();
		}

		private void Initialize()
		{
			// Get a local copy of the context for easier reading
			bluetoothLEHelper = BluetoothLEHelper.Context;
		}

		#endregion

		public Task<IEnumerable<KD.Navien.WaterBoilerMat.Models.IBluetoothDevice>> ScanAsync(int timeoutMilliseconds)
		{
			logger.Log($"Call ScanAsync({timeoutMilliseconds})", Category.Debug, Priority.Medium);
			var tcs = new TaskCompletionSource<IEnumerable<KD.Navien.WaterBoilerMat.Models.IBluetoothDevice>>();

			// check if BluetoothLE APIs are available
			if (BluetoothLEHelper.IsBluetoothLESupported != true)
			{
				logger.Log($"BluetoothLE APIs are not available.)", Category.Warn, Priority.High);
				return Task.FromResult(Enumerable.Empty<KD.Navien.WaterBoilerMat.Models.IBluetoothDevice>());
			}			

			// Start the Enumeration
			bluetoothLEHelper.StartEnumeration();
			logger.Log($"Start the BluetoothLE device Enumeration.)", Category.Info, Priority.High);

			Timer timer = null;
			timer = new Timer(delegate
			{
				timer.Dispose();
				// Stop the Enumeration
				bluetoothLEHelper.StopEnumeration();
				logger.Log($"Stop the BluetoothLE device Enumeration. Found {bluetoothLEHelper.BluetoothLeDevices.Count} devices.)", Category.Info, Priority.High);

				tcs.SetResult(bluetoothLEHelper.BluetoothLeDevices.Select(d => new BluetoothLEDeviceDisplay(d)));

			}, null, timeoutMilliseconds, Timeout.Infinite);

			return tcs.Task;
		}
	}
}
