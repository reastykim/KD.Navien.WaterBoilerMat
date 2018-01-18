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
using KD.Navien.WaterBoilerMat.Models;

namespace KD.Navien.WaterBoilerMat.Droid.Models
{
	public class WaterBoilerMatDeviceAndroid : WaterBoilerMatDevice
	{
		public override string Name => device.Name;

		public override string Address => device.Address;


		private BluetoothDevice device;

		public WaterBoilerMatDeviceAndroid(BluetoothDevice device)
		{
			this.device = device;
		}
	}
}