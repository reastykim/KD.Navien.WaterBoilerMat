using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Universal.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace KD.Navien.WaterBoilerMat.Universal.Extensions
{
	public static class BluetoothExtensions
	{
        public static Task<IEnumerable<IBluetoothGattService>> GetGattServicesAsync(this ObservableBluetoothLEDevice device)
        {
            return device.BluetoothLEDevice.GetGattServicesAsync().AsTask()
                                           .ContinueWith(T =>
                                           {
                                               if (T.IsCompletedSuccessfully != true)
                                                   throw T.Exception;

                                               if (T.Result.Status != GattCommunicationStatus.Success)
                                                   throw new Exception($"ProtocolError=[{T.Result.ProtocolError}]");


                                               return T.Result.Services.Select(S => (IBluetoothGattService)new BluetoothGattServiceUwp(S));
                                           });
        }

        public static Task<IEnumerable<GattDeviceService>> GetGattServicesUwpAsync(this BluetoothLEDevice device)
        {
            return device.GetGattServicesAsync().AsTask()
                                           .ContinueWith(T =>
                                           {
                                               if (T.IsCompletedSuccessfully != true)
                                                   throw T.Exception;

                                               if (T.Result.Status != GattCommunicationStatus.Success)
                                                   throw new Exception($"ProtocolError=[{T.Result.ProtocolError}]");


                                               return T.Result.Services.AsEnumerable();
                                           });
        }

        public static Task<bool> WriteValueAsync(this ObservableGattCharacteristics characteristics, byte[] data)
		{
            return characteristics.Characteristic.WriteValueAsync(data.ToBuffer()).AsTask()
                                                 .ContinueWith(T =>
                                                 {
                                                     if (T.Exception != null)
                                                     {
                                                         return false;
                                                     }
                                                     else
                                                     {
                                                         return T.Result == GattCommunicationStatus.Success; 
                                                     }
                                                 });
		}

        public static void Disconnect(this ObservableBluetoothLEDevice device)
        {
            if (device.IsConnected)
            {
                device.BluetoothLEDevice.Dispose();
            }
        }
    }
}
