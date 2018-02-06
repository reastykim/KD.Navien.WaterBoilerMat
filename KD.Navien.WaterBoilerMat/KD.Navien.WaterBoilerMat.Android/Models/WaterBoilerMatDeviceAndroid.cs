using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KD.Navien.WaterBoilerMat.Droid.Services.LE;
using KD.Navien.WaterBoilerMat.Models;

namespace KD.Navien.WaterBoilerMat.Droid.Models
{
	public class WaterBoilerMatDeviceAndroid : WaterBoilerMatDevice
	{
		#region Properties

		public override string Name => device.Name;

		public override string Address => device.Address;

		#endregion

		#region Fields

		private BluetoothDevice device;
		private BluetoothAdapter bluetoothAdapter;
		private BluetoothGatt bluetoothGatt;
		private GattCallbacks gattCallback;

		#endregion

		public WaterBoilerMatDeviceAndroid(BluetoothDevice device, BluetoothAdapter bluetoothAdapter)
		{
			this.device = device;
			this.bluetoothAdapter = bluetoothAdapter;

			Initialize();
		}

		private void Initialize()
		{

		}

		private void Service_GattCharacteristicsUpdated(object sender, EventArgs e)
		{
			RaiseServicesUpdated();
		}

		public override Task ConnectAsync()
		{
			if (bluetoothAdapter == null)
			{
				return Task.FromException(new BluetoothLEException("BluetoothAdapter not initialized."));
			}

			if (bluetoothGatt == null)
			{
				gattCallback = new GattCallbacks(
					(gatt, status, newState) => // onConnectionStateChange
					{
						if (newState == ProfileState.Connected)
						{
							//intentAction = BluetoothLeService.ACTION_GATT_CONNECTED;
							//BluetoothLeService.this.mConnectionState = 2;
							//BluetoothLeService.this.broadcastUpdate(intentAction);
							System.Diagnostics.Debug.WriteLine("Connected to GATT server.");
							System.Diagnostics.Debug.WriteLine($"Attempting to start service discovery:{gatt?.DiscoverServices()}");
						}
						else if (newState == ProfileState.Disconnected)
						{
							//intentAction = BluetoothLeService.ACTION_GATT_DISCONNECTED;
							//BluetoothLeService.this.mConnectionState = 0;
							System.Diagnostics.Debug.WriteLine("Disconnected from GATT server.");
							//BluetoothLeService.this.broadcastUpdate(intentAction);
						}
					},
					(gatt, status) => // onServicesDiscovered
					{
						if (status == GattStatus.Success)
						{
							//BluetoothLeService.this.broadcastUpdate(BluetoothLeService.ACTION_GATT_SERVICES_DISCOVERED);
							foreach (var service in gatt.Services)
							{
								System.Diagnostics.Debug.WriteLine($"GattService, UUID=[{service.Uuid}]");
							}

							Services.AddRange(gatt.Services.Select(S => new BluetoothGattServiceAndroid(this, S)));
							RaiseServicesUpdated();
						}
						else
						{
							System.Diagnostics.Debug.WriteLine($"onServicesDiscovered received: {status}");
						}
					},
					(gatt, characteristic, status) => // onCharacteristicRead
					{
						//BluetoothLeService.this.broadcastUpdate(BTConstants.responseMapMapLookup(status, "UnKown Response"), characteristic);
					},
					(gatt, characteristic) => // onCharacteristicChanged
					{
						//BluetoothLeService.this.broadcastUpdate(BluetoothLeService.ACTION_CHAR_CHANGED, characteristic);
					},
					(gatt, characteristic, status) => // onCharacteristicWrite
					{
						//BluetoothLeService.this.broadcastUpdate(BTConstants.responseMapMapLookup(status, "UnKown Response"), characteristic);
					},
					(gatt, descriptor, status) => // onDescriptorRead
					{
						//BluetoothLeService.this.broadcastUpdate(BTConstants.responseMapMapLookup(status, "UnKown Response"), descriptor);
					},
					(gatt, descriptor, status) => // onDescriptorWrite
					{
						//BluetoothLeService.this.broadcastUpdate(BTConstants.responseMapMapLookup(status, "UnKown Response"), descriptor);
					},
					(gatt, status) => //onReliableWriteCompleted
					{
						//BluetoothLeService.this.broadcastUpdate(BTConstants.responseMapMapLookup(status, "UnKown Response"));
					},
					(gatt, rssi, status) => //onReadRemoteRssi
					{
						//BluetoothLeService.this.broadcastUpdate(BTConstants.responseMapMapLookup(status, "UnKown Response"), rssi);
					});
				bluetoothGatt = device.ConnectGatt(Android.App.Application.Context, false, gattCallback);
				//Log.d(TAG, "Trying to create a new connection.");
				//this.mBluetoothDeviceAddress = address;
				//this.mConnectionState = 1;
				//return true;
				return Task.CompletedTask;
			}
			else
			{
				//Log.d(TAG, "Trying to use an existing mBluetoothGatt for connection.");
				if (bluetoothGatt.Connect() != true)
				{
					return Task.FromException(new BluetoothLEException("BluetoothGatt connect fail."));
				}
				else
				{
					//this.mConnectionState = 1;
					return Task.CompletedTask;
				}
			}
		}

		internal bool SetCharacteristicNotification(BluetoothGattCharacteristic characteristic, bool enable)
		{
			return bluetoothGatt.SetCharacteristicNotification(characteristic, enable);
		}

		internal bool WriteCharacteristic(BluetoothGattCharacteristic characteristic)
		{
			return bluetoothGatt.WriteCharacteristic(characteristic);
		}
	}
}