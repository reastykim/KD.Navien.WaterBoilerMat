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
		public override string Name => device.Name;

		public override string Address => device.Address;


		private BluetoothDevice device;
		private BluetoothAdapter bluetoothAdapter;
		private BluetoothGatt bluetoothGatt;

		public WaterBoilerMatDeviceAndroid(BluetoothDevice device, BluetoothAdapter bluetoothAdapter)
		{
			this.device = device;
			this.bluetoothAdapter = bluetoothAdapter;

			Initialize();
		}

		private void Initialize()
		{
			//device.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
			//device.Services.CollectionChanged += Services_CollectionChanged;
		}

		//private void Services_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		//{
		//	foreach (var service in Services)
		//	{
		//		service.GattCharacteristicsUpdated -= Service_GattCharacteristicsUpdated;
		//	}

		//	Services = device.Services.Select(S => new BluetoothGattServiceUwp(S)).ToList<IBluetoothGattService>();
		//	foreach (var service in Services)
		//	{
		//		service.GattCharacteristicsUpdated += Service_GattCharacteristicsUpdated;
		//	}

		//	RaiseServicesUpdated();
		//}

		private GattCallbacks gattCallback;
		private void Service_GattCharacteristicsUpdated(object sender, EventArgs e)
		{
			RaiseServicesUpdated();
		}

		public override Task ConnectAsync()
		{
			if (bluetoothAdapter == null)
			{
				//Log.w(TAG, "BluetoothAdapter not initialized or unspecified address.");
				return Task.FromException(new Exception("BluetoothAdapter not initialized."));
			}

			if (bluetoothGatt == null)
			{
				gattCallback = new GattCallbacks(
					(gatt, status, newState) => // onConnectionStateChange
					{
						string intentAction;
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

							Services = gatt.Services.Select(S => new BluetoothGattServiceAndroid(S)).ToList<IBluetoothGattService>();
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
					return Task.FromException(new Exception("BluetoothGatt connect fail."));
				}
				else
				{
					//this.mConnectionState = 1;
					return Task.CompletedTask;
				}
			}
		}
	}
}