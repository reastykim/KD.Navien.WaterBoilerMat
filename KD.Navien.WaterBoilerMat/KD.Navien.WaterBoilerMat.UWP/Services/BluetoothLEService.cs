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

[assembly: Dependency(typeof(BluetoothLEService))]
namespace KD.Navien.WaterBoilerMat.UWP.Services
{
	public class BluetoothLEService : IBluetoothLEService<WaterBoilerMatDevice>
	{
		#region Properties

		public bool IsScanning => bluetoothLEHelper.IsEnumerating;

		#endregion

		#region Fields

		private ILoggerFacade logger;
		private BluetoothLEHelper bluetoothLEHelper;

		#endregion

		#region Constructors & Initialize

		public BluetoothLEService(ILoggerFacade logger)
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

		public Task<IEnumerable<WaterBoilerMatDevice>> ScanAsync(int timeoutMilliseconds)
		{
            logger.Log($"Call ScanAsync({timeoutMilliseconds})", Category.Debug, Priority.Medium);
			var tcs = new TaskCompletionSource<IEnumerable<WaterBoilerMatDevice>>();

			// check if BluetoothLE APIs are available
			if (BluetoothLEHelper.IsBluetoothLESupported != true)
			{
				logger.Log($"BluetoothLE APIs are not available", Category.Warn, Priority.High);
				return Task.FromResult(Enumerable.Empty<WaterBoilerMatDevice>());
			}

            // Start the Enumeration
            bluetoothLEHelper.EnumerationCompleted += (s, e) =>
            {
                logger.Log($"Stop the BluetoothLE device Enumeration. Found {bluetoothLEHelper.BluetoothLeDevices.Count} devices", Category.Info, Priority.High);

                tcs.SetResult(bluetoothLEHelper.BluetoothLeDevices.Where(d => WaterBoilerMatDevice.IsNavienDevice(d.BluetoothAddressAsString))
                                                                  .Select(d => new WaterBoilerMatDeviceUwp(d)));
            };
            bluetoothLEHelper.StartEnumeration();
            logger.Log($"Start the BluetoothLE device Enumeration", Category.Info, Priority.High);

            return tcs.Task;
		}
    }
}
