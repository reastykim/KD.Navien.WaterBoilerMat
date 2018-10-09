using KD.Navien.WaterBoilerMat.Extensions;
using KD.Navien.WaterBoilerMat.Services.Protocol;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
		
		public event EventHandler ServicesUpdated;

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
        
        protected async void RaiseServicesUpdated()
		{
			ServicesUpdated?.Invoke(this, EventArgs.Empty);

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
            _logger.Log($"BoilerGattCharacteristic1_ValueChanged. Value = [{dataValue}]", Category.Debug, Priority.High);

            try
            {
                KDResponse response = new KDResponse();
                if (response.SetValue(value))
                {
                    _response = response;
                    _logger.Log($"{response.Data}", Category.Info, Priority.Medium);

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
            _logger.Log($"BoilerGattCharacteristic2_ValueChanged. Value = [{dataValue}]", Category.Debug, Priority.High);

            try
            {
                KDResponse responseData = new KDResponse();
                if (responseData.SetValue(value))
                {// Connect & MAC_REGISTER Success.
                    if (String.IsNullOrWhiteSpace(responseData.Data.UniqueID) != true)
                    {
                        _uniqueID = responseData.Data.UniqueID;
                    }
                    _logger.Log($"Set UniqueID = [{_uniqueID}]", Category.Info, Priority.High);

                    // Write a dummy packet. This is the end of connecting step.
                    await BoilerGattCharacteristic1.WriteValueAsync(new byte[0]);
                }
                else
                {
                    _logger.Log($"Connect fail. Raw=[{responseData.Data.DEBUGCode}]", Category.Debug, Priority.High);
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
            var requestData = new KDRequest();
            requestData.Data.MessageType = KDMessageType.MAC_REGISTER;
            // check, is the device pairing mode : if the actual MatDevice is on BLE PairingMode set the UniqueID to String.Empty, otherwise set the already stored UniqueID.
            if (String.IsNullOrWhiteSpace(uniqueID))
            {
                requestData.Data.UniqueID = "";
            }
            else
            {
                requestData.Data.UniqueID = uniqueID;
            }

            var requestDataValue = requestData.GetValue();
            byte[] bytes = requestDataValue.HexStringToByteArray();

            // Write a request packet
            return BoilerGattCharacteristic2.WriteValueAsync(bytes);
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

        public async Task RequestPowerOnOffAsync(bool isOn)
        {
            KDRequest request = new KDRequest();
            request.Data = _response.Data;
            request.Data.MessageType = KDMessageType.STATUS_CHANGE;
            request.Data.Mode = 6;
            request.Data.Power = Convert.ToInt32(isOn);
            request.Data.SleepStartButtonEnable = 0;
            request.Data.SleepStopButtonEnable = 1;

            var requestDataValue = request.GetValue();
            byte[] bytes = requestDataValue.HexStringToByteArray();
            await BoilerGattCharacteristic1.WriteValueAsync(bytes);
            _logger.Log($"BoilerGattCharacteristic1.WriteValueAsync(). Value = [{requestDataValue}]", Category.Info, Priority.Medium);
        }

        public async Task RequestLockOnOffAsync(bool isLock)
        {
            KDRequest request = new KDRequest();
            request.Data = _response.Data;
            request.Data.MessageType = KDMessageType.STATUS_CHANGE;
            request.Data.KeyLock = Convert.ToInt32(isLock);
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
                //request.Data.SleepSupplySettingTime = MainFragment.this.mResponseData.mData.SleepSupplySettingTime;
                //request.Data.SleepLeftSettingTime = MainFragment.this.mResponseData.mData.SleepLeftSettingTime;
                //request.Data.SleepRightSettingTime = MainFragment.this.mResponseData.mData.SleepRightSettingTime;
                //request.Data.SleepStartButtonEnable = MainFragment.this.mResponseData.mData.SleepStartButtonEnable;
            }
            request.Data.SleepStartButtonEnable = 0;
            request.Data.SleepStopButtonEnable = 1;

            var requestDataValue = request.GetValue();
            byte[] bytes = requestDataValue.HexStringToByteArray();
            await BoilerGattCharacteristic1.WriteValueAsync(bytes);
            _logger.Log($"BoilerGattCharacteristic1.WriteValueAsync(). Value = [{requestDataValue}]", Category.Info, Priority.Medium);
        }

        public async Task RequestLeftPowerOnOff(bool isOn)
        {
            if (MainFragment.this.mResponseData.mData.Mode == 4) {
                Toast.makeText(MainFragment.this.getActivity(), MainFragment.this.getResources().getString(R.string.message_cleanmode_fail), 0).show();
            } else if (MainFragment.this.mResponseData.mData.Mode == 7) {
                Toast.makeText(MainFragment.this.getActivity(), MainFragment.this.getResources().getString(R.string.message_wateroutmode_fail), 0).show();
            } else if ((MainFragment.this.mResponseData.mData.Status != 3 || MainFragment.this.mResponseData.mData.ModelType == 1 || MainFragment.this.mResponseData.mData.ModelType == 17) && MainFragment.this.mResponseData.mData.KeyLock != 1) {
                int value;
                int i;
                KDRequest requestData = new KDRequest();
                if (MainFragment.this.mResponseData.mData.ModelType != 1 && MainFragment.this.mResponseData.mData.ModelType != 17) {
                    value = MainFragment.this.mResponseData.mData.Status == 4 ? 2 : 4;
                } else if (MainFragment.this.mResponseData.mData.MattType == 1) {
                    if (MainFragment.this.mResponseData.mData.Status == 2) {
                        value = 0;
                    } else {
                        value = 2;
                    }
                } else if (MainFragment.this.mResponseData.mData.Status == 4) {
                    value = 2;
                } else if (MainFragment.this.mResponseData.mData.Status == 0) {
                    value = 3;
                } else if (MainFragment.this.mResponseData.mData.Status == 1 || MainFragment.this.mResponseData.mData.Status == 2) {
                    value = 4;
                } else {
                    value = 0;
                }
                requestData.mData = MainFragment.this.mResponseData.mData;
                requestData.mData.Mode = MainFragment.this.mResponseData.mData.Mode;
                requestData.mData.MattType = MainFragment.this.mResponseData.mData.MattType;
                requestData.mData.Power = MainFragment.this.mResponseData.mData.Power;
                requestData.mData.KeyLock = MainFragment.this.mResponseData.mData.KeyLock;
                requestData.mData.DegreeType = MainFragment.this.mResponseData.mData.DegreeType;
                requestData.mData.WaterCapacity = MainFragment.this.mResponseData.mData.WaterCapacity;
                requestData.mData.Volume = MainFragment.this.mResponseData.mData.Volume;
                requestData.mData.Status = value;
                requestData.mData.TemperatureSupply = MainFragment.this.mResponseData.mData.TemperatureSupply;
                requestData.mData.TemperatureReturnLeft = MainFragment.this.mResponseData.mData.TemperatureReturnLeft;
                requestData.mData.TemperatureReturnRight = MainFragment.this.mResponseData.mData.TemperatureReturnRight;
                requestData.mData.TemperatureSupplySetting = MainFragment.this.mResponseData.mData.TemperatureSupplySetting;
                KDData kDData = requestData.mData;
                if (value == 4)
                {
                    i = 0;
                }
                else
                {
                    i = MainFragment.this.mResponseData.mData.TemperatureSettingLeft;
                }
                kDData.TemperatureSettingLeft = i;
                requestData.mData.TemperatureSettingRight = MainFragment.this.mResponseData.mData.TemperatureSettingRight;
                requestData.mData.SleepStartButtonEnable = 0;
                requestData.mData.SleepStopButtonEnable = 1;

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
