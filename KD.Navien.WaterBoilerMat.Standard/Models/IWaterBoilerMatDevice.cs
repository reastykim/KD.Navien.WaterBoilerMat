using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IWaterBoilerMatDevice : INotifyPropertyChanged, IDisposable
    {
        IBluetoothGattService BoilerGattService { get; }

        IBluetoothGattCharacteristic BoilerGattCharacteristic1 { get; }

        IBluetoothGattCharacteristic BoilerGattCharacteristic2 { get; }

        bool IsPowerOn { get; }
        bool IsLock { get; }

        int VolumeLevel { get; }

        bool IsLeftPartsPowerOn { get; }

        bool IsRightPartsPowerOn { get; }

        TemperatureInfo TemperatureInfo { get; }

        int CurrentLeftTemperature { get; }
        int CurrentRightTemperature { get; }

        int SetupLeftTemperature { get; }
        int SetupRightTemperature { get; }

        Task<string> ConnectAsync(string uniqueID);
        void Disconnect();

        Task RequestPowerOnOffAsync();
        Task RequestLockOnOffAsync();

        Task RequestLeftPartsPowerOnOffAsync();

        Task RequestRightPartsPowerOnOffAsync();

        Task RequestSetupTemperatureChangeAsync(int leftTemperature, int rightTemperature);
    }
}
