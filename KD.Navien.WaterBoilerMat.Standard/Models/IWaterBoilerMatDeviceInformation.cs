using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IWaterBoilerMatDeviceInformation : INotifyPropertyChanged
    {
        string Id { get; }

        bool IsPowerOn { get; }
        bool IsLock { get; }

        WaterCapacities WaterCapacity { get; }

        VolumeLevels VolumeLevel { get; }

        DeviceStatus Status { get; }

        bool IsLeftPartsPowerOn { get; }

        bool IsRightPartsPowerOn { get; }

        TemperatureInfo TemperatureInfo { get; }

        int CurrentLeftTemperature { get; }
        int CurrentRightTemperature { get; }

        int SetupLeftTemperature { get; }
        int SetupRightTemperature { get; }
    }
}
