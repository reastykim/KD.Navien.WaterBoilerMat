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
	public class GattCallbacks : BluetoothGattCallback
	{
		private readonly Action<BluetoothGatt, GattStatus, ProfileState> onConnectionStateChange;
		private readonly Action<BluetoothGatt, GattStatus> onServicesDiscovered;
		private readonly Action<BluetoothGatt, BluetoothGattCharacteristic, GattStatus> onCharacteristicRead;
		private readonly Action<BluetoothGatt, BluetoothGattCharacteristic> onCharacteristicChanged;
		private readonly Action<BluetoothGatt, BluetoothGattCharacteristic, GattStatus> onCharacteristicWrite;
		private readonly Action<BluetoothGatt, BluetoothGattDescriptor, GattStatus> onDescriptorRead;
		private readonly Action<BluetoothGatt, BluetoothGattDescriptor, GattStatus> onDescriptorWrite;
		private readonly Action<BluetoothGatt, GattStatus> onReliableWriteCompleted;
		private readonly Action<BluetoothGatt, int, GattStatus> onReadRemoteRssi;



		public GattCallbacks(Action<BluetoothGatt, GattStatus, ProfileState> onConnectionStateChange,
							 Action<BluetoothGatt, GattStatus> onServicesDiscovered,
							 Action<BluetoothGatt, BluetoothGattCharacteristic, GattStatus> onCharacteristicRead,
							 Action<BluetoothGatt, BluetoothGattCharacteristic> onCharacteristicChanged,
							 Action<BluetoothGatt, BluetoothGattCharacteristic, GattStatus> onCharacteristicWrite,
							 Action<BluetoothGatt, BluetoothGattDescriptor, GattStatus> onDescriptorRead,
							 Action<BluetoothGatt, BluetoothGattDescriptor, GattStatus> onDescriptorWrite,
							 Action<BluetoothGatt, GattStatus> onReliableWriteCompleted,
							 Action<BluetoothGatt, int, GattStatus> onReadRemoteRssi)
		{
			this.onConnectionStateChange = onConnectionStateChange;
			this.onServicesDiscovered = onServicesDiscovered;
			this.onCharacteristicRead = onCharacteristicRead;
			this.onCharacteristicChanged = onCharacteristicChanged;
			this.onCharacteristicWrite = onCharacteristicWrite;
			this.onDescriptorRead = onDescriptorRead;
			this.onDescriptorWrite = onDescriptorWrite;
			this.onReliableWriteCompleted = onReliableWriteCompleted;
			this.onReadRemoteRssi = onReadRemoteRssi;
		}

		public override void OnConnectionStateChange(BluetoothGatt gatt, [GeneratedEnum] GattStatus status, [GeneratedEnum] ProfileState newState)
			=> onConnectionStateChange?.Invoke(gatt, status, newState);

		public override void OnServicesDiscovered(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
			=> onServicesDiscovered?.Invoke(gatt, status);

		public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
			=> onCharacteristicRead?.Invoke(gatt, characteristic, status);

		public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
			=> onCharacteristicChanged?.Invoke(gatt, characteristic);

		public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
			=> onCharacteristicWrite?.Invoke(gatt, characteristic, status);

		public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
			=> onDescriptorRead?.Invoke(gatt, descriptor, status);

		public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
			=> onDescriptorWrite?.Invoke(gatt, descriptor, status);

		public override void OnReliableWriteCompleted(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
			=> onReliableWriteCompleted?.Invoke(gatt, status);

		public override void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, [GeneratedEnum] GattStatus status)
			=> onReadRemoteRssi?.Invoke(gatt, rssi, status);
	}
}