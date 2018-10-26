using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public class WaterBoilerMatDeviceInformation : BindableBase, IWaterBoilerMatDeviceInformation
    {
        #region Properties

        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private string _id;

        public bool IsPowerOn
        {
            get { return _isPowerOn; }
            set { SetProperty(ref _isPowerOn, value); }
        }
        private bool _isPowerOn;

        public bool IsLock
        {
            get { return _isLock; }
            set { SetProperty(ref _isLock, value); }
        }
        private bool _isLock;

        public WaterCapacities WaterCapacity
        {
            get { return _waterCapacity; }
            set { SetProperty(ref _waterCapacity, value); }
        }
        private WaterCapacities _waterCapacity;

        public VolumeLevels VolumeLevel
        {
            get { return _volumeLevel; }
            set { SetProperty(ref _volumeLevel, value); }
        }
        private VolumeLevels _volumeLevel;

        public DeviceStatus Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        private DeviceStatus _status;

        public bool IsLeftPartsPowerOn
        {
            get { return _isLeftPartsPowerOn; }
            set { SetProperty(ref _isLeftPartsPowerOn, value); }
        }
        private bool _isLeftPartsPowerOn;

        public bool IsRightPartsPowerOn
        {
            get { return _IsRightPartsPowerOn; }
            set { SetProperty(ref _IsRightPartsPowerOn, value); }
        }
        private bool _IsRightPartsPowerOn;

        public TemperatureInfo TemperatureInfo
        {
            get { return _temperatureInfo; }
            set { SetProperty(ref _temperatureInfo, value); }
        }
        private TemperatureInfo _temperatureInfo;

        public int CurrentLeftTemperature
        {
            get { return _currentLeftTemperature; }
            set { SetProperty(ref _currentLeftTemperature, value); }
        }
        private int _currentLeftTemperature;

        public int CurrentRightTemperature
        {
            get { return _currentRightTemperature; }
            set { SetProperty(ref _currentRightTemperature, value); }
        }
        private int _currentRightTemperature;

        public int SetupLeftTemperature
        {
            get { return _setupLeftTemperature; }
            set { SetProperty(ref _setupLeftTemperature, value); }
        }
        private int _setupLeftTemperature;

        public int SetupRightTemperature
        {
            get { return _setupRightTemperature; }
            set { SetProperty(ref _setupRightTemperature, value); }
        }
        private int _setupRightTemperature;

        #endregion
    }
}
