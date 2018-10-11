using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IWaterBoilerMatDevice : IDisposable
    {
        IBluetoothGattService BoilerGattService { get; }

        IBluetoothGattCharacteristic BoilerGattCharacteristic1 { get; }

        IBluetoothGattCharacteristic BoilerGattCharacteristic2 { get; }

        bool IsPowerOn { get; }
        bool IsLock { get; }

        bool IsLeftPartsPowerOn { get; }

        bool IsRightPartsPowerOn { get; }

        TemperatureInfo TemperatureInfo { get; }

        Task<string> ConnectAsync(string uniqueID);
        void Disconnect();

        Task RequestPowerOnOffAsync();
        Task RequestLockOnOffAsync();

        Task RequestLeftPartsPowerOnOffAsync();

        Task RequestRightPartsPowerOnOffAsync();
    }
}
