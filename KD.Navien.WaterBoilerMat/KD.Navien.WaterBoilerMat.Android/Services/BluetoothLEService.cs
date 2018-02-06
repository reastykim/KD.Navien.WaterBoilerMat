using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KD.Navien.WaterBoilerMat.Droid.Models;
using KD.Navien.WaterBoilerMat.Droid.Services;
using KD.Navien.WaterBoilerMat.Droid.Services.LE;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using Prism.Logging;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothLEService))]
namespace KD.Navien.WaterBoilerMat.Droid.Services
{
	public class BluetoothLEService : IBluetoothLEService<WaterBoilerMatDevice>
	{
		#region Properties

		public bool IsScanning => false;// bluetoothLEHelper.IsEnumerating;

		#endregion

		#region Fields

		private ILoggerFacade logger;
		private Handler handler;
		private BluetoothAdapter bluetoothAdapter;
		private BluetoothLeScanner leScanner;

		#endregion

		#region Constructors & Initialize

		public BluetoothLEService(ILoggerFacade logger)
		{
			this.logger = logger;

			Initialize();
		}

		private void Initialize()
		{
			handler = new Handler();
			bluetoothAdapter = ((BluetoothManager)Android.App.Application.Context.GetSystemService(Context.BluetoothService)).Adapter;
			leScanner = bluetoothAdapter?.BluetoothLeScanner;
		}

		#endregion

		public Task<IEnumerable<WaterBoilerMatDevice>> ScanAsync(int timeoutMilliseconds)
		{
			logger.Log($"Call ScanAsync({timeoutMilliseconds})", Category.Debug, Priority.Medium);
			var tcs = new TaskCompletionSource<IEnumerable<WaterBoilerMatDevice>>();

			// check if BluetoothLE APIs are available
			if (Android.App.Application.Context.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureBluetoothLe) != true ||
				leScanner == null)
			{
				logger.Log($"BluetoothLE APIs are not available", Category.Warn, Priority.High);
				return Task.FromResult(Enumerable.Empty<WaterBoilerMatDevice>());
			}

			var result = new List<BluetoothDevice>();
			var scanCallback = new LE.ScanCallback((device, rssi, record) =>
			{
				if (result.Any(D => D.Address.Equals(device.Address)))
					return;

				result.Add(device);
			});

			// Start the Enumeration
			leScanner.StartScan(scanCallback);
			logger.Log($"Start the BluetoothLE device Enumeration", Category.Info, Priority.High);

			handler.PostDelayed(() =>
			{
				leScanner.StopScan(scanCallback);
				logger.Log($"Stop the BluetoothLE device Enumeration. Found {result.Count} devices.", Category.Info, Priority.High);

				tcs.SetResult(result//.Where(D => WaterBoilerMatDevice.IsNavienDevice(D.Address))
									.Where(D => String.IsNullOrWhiteSpace(D.Name) != true)
									.Select(D => new WaterBoilerMatDeviceAndroid(D, bluetoothAdapter)));

			}, timeoutMilliseconds);

			return tcs.Task;
		}
	}
}
