using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IWaterBoilerMatDevice : IWaterBoilerMatDeviceInformation, IDisposable
    {
        event EventHandler DeviceStatusUpdated;

        IBluetoothGattService BoilerGattService { get; }

        IBluetoothGattCharacteristic BoilerGattCharacteristic1 { get; }

        IBluetoothGattCharacteristic BoilerGattCharacteristic2 { get; }

        Task<string> ConnectAsync(string uniqueID);
        Task DisconnectAsync();

        Task RequestPowerOnOffAsync();
        Task RequestLockOnOffAsync();

        Task RequestLeftPartsPowerOnOffAsync();

        Task RequestRightPartsPowerOnOffAsync();

        Task RequestSetupTemperatureChangeAsync(int leftTemperature, int rightTemperature);

        Task RequestVolumeChangeAsync(VolumeLevels value);
    }
}
