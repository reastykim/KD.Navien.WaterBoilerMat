using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.Models
{
    public abstract class WaterBoilerMatDevice : BindableBase, IBluetoothLEDevice, IWaterBoilerMatDevice
    {
		protected const string NavienDeviceMacPrefix = "2C:E2:A8";
		public const string BoilerGattServiceUuid = "00001c0d-d102-11e1-9b23-2ce2a80000dd";
		public const string BoilerGattCharacteristic1Uuid = "00001c0d-d102-11e1-9b23-2ce2a80100dd";
		public const string BoilerGattCharacteristic2Uuid = "00001c0d-d102-11e1-9b23-2ce2a80200dd";

        #region Properties

        public abstract string Name { get; }

		public abstract string Address { get; }

        public abstract bool IsConnected { get; }

        public List<IBluetoothGattService> Services
		{
			get => _services;
			protected set => SetProperty(ref _services, value);
		}
		private List<IBluetoothGattService> _services = new List<IBluetoothGattService>();

        public IBluetoothGattService BoilerGattService
        {
            get => Services.FirstOrDefault(S => S.UUID == BoilerGattServiceUuid);
        }

        public IBluetoothGattCharacteristic BoilerGattCharacteristic1
        {
            get => BoilerGattService?.GattCharacteristics.FirstOrDefault(C => C.UUID == BoilerGattCharacteristic1Uuid);
        }

        public IBluetoothGattCharacteristic BoilerGattCharacteristic2
        {
            get => BoilerGattService?.GattCharacteristics.FirstOrDefault(C => C.UUID == BoilerGattCharacteristic2Uuid);
        }

        public bool IsPowerOn
        {
            get => _isPowerOn;
            protected set => SetProperty(ref _isPowerOn, value);
        }
        private bool _isPowerOn;

        public bool IsLock
        {
            get => _isLock;
            protected set => SetProperty(ref _isLock, value);
        }
        private bool _isLock;

        public bool IsLeftPartsPowerOn
        {
            get => _isLeftPartsPowerOn;
            protected set => SetProperty(ref _isLeftPartsPowerOn, value);
        }
        private bool _isLeftPartsPowerOn;


        public bool IsRightPartsPowerOn
        {
            get => _isRightPartsPowerOn;
            protected set => SetProperty(ref _isRightPartsPowerOn, value);
        }
        private bool _isRightPartsPowerOn;

        public TemperatureInfo TemperatureInfo
        {
            get => _temperatureInfo;
            protected set => SetProperty(ref _temperatureInfo, value);
        }
        private TemperatureInfo _temperatureInfo;

        #endregion

        #region Fields

        protected ILoggerFacade _logger;
        protected KDResponse _response;
        protected bool _disposed;

        private bool _isReady;
        private string _uniqueID;
        private TaskCompletionSource<string> _connectTcs = new TaskCompletionSource<string>();

        #endregion

        #region Constructors & Initialize, Dispose, Destructors

        ~WaterBoilerMatDevice()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            try
            {
                if (disposing)
                {/* You need to clean up external resources managed by the .NET Framework at here. */
                    _isReady = false;
                    _uniqueID = null;
                    _response = null;
                    BoilerGattCharacteristic2.ValueChanged -= BoilerGattCharacteristic2_ValueChanged;
                    BoilerGattCharacteristic1.ValueChanged -= BoilerGattCharacteristic1_ValueChanged;

                    Disconnect();
                }

                try { /* You need to clean up external resources didn't managed by the .NET Framework at here. */ }
                catch { }
                finally
                {
                    _disposed = true;
                }
            }
            finally { }
        }

        #endregion

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(IsConnected) && IsConnected == false)
            {
                _connectTcs.TrySetException(new ApplicationException("Connection is lost by native object."));
            }
        }

        protected async void RaiseServicesUpdated()
		{
            if (BoilerGattCharacteristic1 != null && BoilerGattCharacteristic2 != null && _isReady == false)
            {
                _isReady = true;

                BoilerGattCharacteristic1.ValueChanged += BoilerGattCharacteristic1_ValueChanged;
                await BoilerGattCharacteristic1.SetNotifyAsync(true);

                BoilerGattCharacteristic2.ValueChanged += BoilerGattCharacteristic2_ValueChanged;
                await BoilerGattCharacteristic2.SetNotifyAsync(true);

                await RequestMacRegisterAsync(_uniqueID);
            }
        }

        private void BoilerGattCharacteristic1_ValueChanged(object sender, byte[] value)
        {
            var dataValue = value.ToString("X02");

            try
            {
                KDResponse response = new KDResponse();
                if (response.SetValue(value))
                {
                    _logger.Log($"RECV KDResponse. {response.Data}", Category.Info, Priority.High);
                    _response = response;
                    

                    // Update
                    UpdateDeviceStatus(response.Data);

                    _connectTcs.TrySetResult(_uniqueID);
                }
                else
                {
                    _logger.Log($"Connect fail. Raw=[{response.Data.DEBUGCode}]", Category.Debug, Priority.High);
                    throw new ApplicationException($"KDResponse.SetValue() fail.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"BoilerGattCharacteristic2_ValueChanged. Exception=[{ex.Message}], Response = [{dataValue}]", Category.Exception, Priority.High);

                _connectTcs.TrySetException(ex);
            }
        }

        private async void BoilerGattCharacteristic2_ValueChanged(object sender, byte[] value)
        {
            var dataValue = value.ToString("X02");

            try
            {
                KDResponse response = new KDResponse();
                if (response.SetValue(value))
                {// Connect & MAC_REGISTER Success.
                    _logger.Log($"RECV KDResponse.\t{response.Data}", Category.Info, Priority.High);

                    if (String.IsNullOrWhiteSpace(response.Data.UniqueID) != true)
                    {
                        _uniqueID = response.Data.UniqueID;
                    }
                    _logger.Log($"Set UniqueID = [{_uniqueID}]", Category.Info, Priority.High);

                    // Write a dummy packet. This is the end of connecting step.
                    await BoilerGattCharacteristic1.WriteValueAsync(new byte[0]);
                    _logger.Log($"SEND KDRequest. \t[0]", Category.Info, Priority.High);
                }
                else
                {
                    _logger.Log($"Connect fail. Raw=[{response.Data.DEBUGCode}]", Category.Debug, Priority.High);
                    throw new ApplicationException($"KDResponse.SetValue() fail.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"BoilerGattCharacteristic2_ValueChanged. Exception=[{ex.Message}], Response = [{dataValue}]", Category.Exception, Priority.High);

                _connectTcs.TrySetException(ex);
            }
        }

        private Task<bool> RequestMacRegisterAsync(string uniqueID)
        {
            // Create a request
            var request = new KDRequest();
            request.Data.MessageType = KDMessageType.MAC_REGISTER;
            // check, is the device pairing mode : if the actual MatDevice is on BLE PairingMode set the UniqueID to String.Empty, otherwise set the already stored UniqueID.
            if (String.IsNullOrWhiteSpace(uniqueID))
            {
                request.Data.UniqueID = "";
            }
            else
            {
                request.Data.UniqueID = uniqueID;
            }

            var requestDataValue = request.GetValue();
            byte[] bytes = requestDataValue.HexStringToByteArray();

            // Write a request packet
            var writeValueTask = BoilerGattCharacteristic2.WriteValueAsync(bytes);
            _logger.Log($"SEND KDRequest. \t{request.Data}", Category.Info, Priority.High);

            return writeValueTask;
        }

        protected abstract Task ConnectAsync();

        public abstract void Disconnect();

        protected abstract void UpdateDeviceStatus(KDData data);

        public abstract Task<T> GetNativeBluetoothLEDeviceObjectAsync<T>() where T : class;

        #region Implements IWaterBoilerMatDevice

        public Task<string> ConnectAsync(string uniqueID)
        {
            if (IsConnected)
            {
                return Task.FromResult(uniqueID);
            }

            _connectTcs.TrySetException(new InvalidOperationException());

            _uniqueID = uniqueID;
            _connectTcs = new TaskCompletionSource<string>();

            ConnectAsync();

            return _connectTcs.Task;
        }

        public async Task RequestPowerOnOffAsync()
        {
            if (_response == null)
                return;

            KDRequest request = new KDRequest();
            request.Data = _response.Data;
            request.Data.MessageType = KDMessageType.STATUS_CHANGE;
            request.Data.Mode = 6;
            request.Data.Power = Convert.ToInt32(!IsPowerOn);
            request.Data.SleepStartButtonEnable = 0;
            request.Data.SleepStopButtonEnable = 1;

            var requestDataValue = request.GetValue();
            byte[] bytes = requestDataValue.HexStringToByteArray();
            await BoilerGattCharacteristic1.WriteValueAsync(bytes);
            _logger.Log($"SEND KDRequest. \t{request.Data}", Category.Info, Priority.High);
        }

        public async Task RequestLockOnOffAsync()
        {
            if (_response == null)
                return;

            KDRequest request = new KDRequest();
            request.Data = _response.Data;
            request.Data.MessageType = KDMessageType.STATUS_CHANGE;
            request.Data.KeyLock = Convert.ToInt32(!IsLock);
            if (request.Data.Status == 4)
            {
                request.Data.TemperatureSettingLeft = 0;
            }
            if (request.Data.Status == 3)
            {
                request.Data.TemperatureSettingRight = 0;
            }
            if (request.Data.Mode == 3)
            {
                //request.Data.SleepSupplySettingTime = _response.Data.SleepSupplySettingTime;
                //request.Data.SleepLeftSettingTime = _response.Data.SleepLeftSettingTime;
                //request.Data.SleepRightSettingTime = _response.Data.SleepRightSettingTime;
                //request.Data.SleepStartButtonEnable = _response.Data.SleepStartButtonEnable;
            }
            request.Data.SleepStartButtonEnable = 0;
            request.Data.SleepStopButtonEnable = 1;

            var requestDataValue = request.GetValue();
            byte[] bytes = requestDataValue.HexStringToByteArray();
            await BoilerGattCharacteristic1.WriteValueAsync(bytes);
            _logger.Log($"SEND KDRequest. \t{request.Data}", Category.Info, Priority.High);
        }

        public async Task RequestLeftPartsPowerOnOffAsync()
        {
            if (_response == null)
                return;

            if (_response.Data.Mode == 4)
            {
                //Toast.makeText(MainFragment.this.getActivity(), MainFragment.this.getResources().getString(R.string.message_cleanmode_fail), 0).show();
                return;
            }
            if (_response.Data.Mode == 7)
            {
                //Toast.makeText(MainFragment.this.getActivity(), MainFragment.this.getResources().getString(R.string.message_wateroutmode_fail), 0).show();
                return;
            }


            if ((_response.Data.Status != 3 || _response.Data.ModelType == 1 || _response.Data.ModelType == 17)
                && _response.Data.KeyLock != 1)
            {
                int value;
                KDRequest request = new KDRequest();
                request.Data = _response.Data;
                request.Data.MessageType = KDMessageType.STATUS_CHANGE;

                if (_response.Data.ModelType != 1 && _response.Data.ModelType != 17)
                {
                    value = _response.Data.Status == 4 ? 2 : 4;
                }
                else if (_response.Data.MattType == 1)
                {
                    if (_response.Data.Status == 2)
                    {
                        value = 0;
                    }
                    else
                    {
                        value = 2;
                    }
                }
                else if (_response.Data.Status == 4)
                {
                    value = 2;
                }
                else if (_response.Data.Status == 0)
                {
                    value = 3;
                }
                else if (_response.Data.Status == 1 || _response.Data.Status == 2)
                {
                    value = 4;
                }
                else
                {
                    value = 0;
                }

                request.Data.Status = value;
                if (value == 4)
                {
                    request.Data.TemperatureSettingLeft = 0;
                }
                request.Data.SleepStartButtonEnable = 0;
                request.Data.SleepStopButtonEnable = 1;

                var requestDataValue = request.GetValue();
                byte[] bytes = requestDataValue.HexStringToByteArray();
                await BoilerGattCharacteristic1.WriteValueAsync(bytes);
                _logger.Log($"SEND KDRequest. \t{request.Data}", Category.Info, Priority.High);
            }
        }

        public async Task RequestRightPartsPowerOnOffAsync()
        {
            if (_response == null)
                return;

            if (_response.Data.Mode == 4)
            {
                //Toast.makeText(MainFragment.this.getActivity(), MainFragment.this.getResources().getString(R.string.message_cleanmode_fail), 0).show();
                return;
            }
            if (_response.Data.Mode == 7)
            {
                //Toast.makeText(MainFragment.this.getActivity(), MainFragment.this.getResources().getString(R.string.message_wateroutmode_fail), 0).show();
                return;
            }


            if ((_response.Data.Status != 4 || _response.Data.ModelType == 1 || _response.Data.ModelType == 17)
                && _response.Data.KeyLock != 1)
            {
                int value;
                KDRequest request = new KDRequest();
                request.Data = _response.Data;
                request.Data.MessageType = KDMessageType.STATUS_CHANGE;

                if (_response.Data.ModelType != 1 && _response.Data.ModelType != 17)
                {
                    value = _response.Data.Status == 3 ? 2 : 3;
                }
                else if (_response.Data.MattType == 1)
                {
                    if (_response.Data.Status == 2)
                    {
                        value = 0;
                    }
                    else
                    {
                        value = 2;
                    }
                }
                else if (_response.Data.Status == 3)
                {
                    value = 2;
                }
                else if (_response.Data.Status == 1 || _response.Data.Status == 2)
                {
                    value = 3;
                }
                else if (_response.Data.Status == 0)
                {
                    value = 4;
                }
                else
                {
                    value = 0;
                }

                request.Data.Status = value;
                if (value == 3)
                {
                    request.Data.TemperatureSettingRight = 0;
                }
                request.Data.SleepStartButtonEnable = 0;
                request.Data.SleepStopButtonEnable = 1;

                var requestDataValue = request.GetValue();
                byte[] bytes = requestDataValue.HexStringToByteArray();
                await BoilerGattCharacteristic1.WriteValueAsync(bytes);
                _logger.Log($"SEND KDRequest. \t{request.Data}", Category.Info, Priority.High);
            }
        }

        #endregion

        #region Static Methods

        public static bool IsNavienDevice(string address)
        {
            return address.StartsWith(NavienDeviceMacPrefix, StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool IsNavienDevice(ulong address)
        {
            return address.ToMacAddress().StartsWith(NavienDeviceMacPrefix, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }
}
