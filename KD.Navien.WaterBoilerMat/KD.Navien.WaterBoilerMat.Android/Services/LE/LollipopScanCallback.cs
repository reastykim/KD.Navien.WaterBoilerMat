using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KD.Navien.WaterBoilerMat.Droid.Services.LE
{
	public class LollipopScanCallback : ScanCallback
	{
		readonly Action<BluetoothDevice, int, ScanRecord> callback;
		
		public LollipopScanCallback(Action<BluetoothDevice, int, ScanRecord> callback)
			=> this.callback = callback;
		
		public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
			=> this.callback(result.Device, result.Rssi, result.ScanRecord);
	}
}