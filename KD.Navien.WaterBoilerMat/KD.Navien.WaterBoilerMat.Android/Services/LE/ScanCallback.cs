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
	public class ScanCallback : Android.Bluetooth.LE.ScanCallback
	{
		readonly Action<BluetoothDevice, int, ScanRecord> onScanResult;
		
		public ScanCallback(Action<BluetoothDevice, int, ScanRecord> onScanResult)
			=> this.onScanResult = onScanResult;
		
		public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
			=> onScanResult?.Invoke(result.Device, result.Rssi, result.ScanRecord);
	}
}