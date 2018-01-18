using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace KD.Navien.WaterBoilerMat.Droid.Services.LE
{
	public class PreLollipopScanCallback : Java.Lang.Object, BluetoothAdapter.ILeScanCallback
	{
		readonly Action<BluetoothDevice, int, byte[]> callback;


		public PreLollipopScanCallback(Action<BluetoothDevice, int, byte[]> callback)
			=> this.callback = callback;


		public void OnLeScan(BluetoothDevice device, int rssi, byte[] scanRecord)
			=> this.callback(device, rssi, scanRecord);
	}
}