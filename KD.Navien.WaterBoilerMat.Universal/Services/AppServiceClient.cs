using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Universal.Extensions;
using KD.Navien.WaterBoilerMat.Universal.Models;
using Newtonsoft.Json;
using Prism.Logging;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using static KD.Navien.WaterBoilerMat.Standard.Services.AppServiceCommands;

namespace KD.Navien.WaterBoilerMat.Universal.Services
{
    public class AppServiceClient : IAppServiceClient
    {
        const string AppServiceName = "kd.navien.waterboilermat.service";
        const string PackageFamilyName = "KD.Navien.WaterBoilerMat.BackgroundApp-uwp_kkkr88vwcx6t6";

        public event EventHandler<WaterBoilerMatDeviceInformation> WaterBoilerMatDeviceInformationUpdated;

        #region Properties

        public bool IsOpened { get; private set; }
        public IBluetoothLEDevice ConnectedBluetoothLEDeviceInformation { get; private set; }

        #endregion

        #region Fields

        private readonly ILoggerFacade _logger;
        private readonly AppServiceConnection _appServiceConnection;

        #endregion

        #region Constructors & Initialize

        public AppServiceClient(ILoggerFacade logger)
        {
            _logger = logger;
            _appServiceConnection = new AppServiceConnection();            

            Initialize();
        }

        private void Initialize()
        {
            _appServiceConnection.AppServiceName = AppServiceName;
            _appServiceConnection.PackageFamilyName = PackageFamilyName;
            _appServiceConnection.ServiceClosed += OnServiceClosed;
        }

        #endregion

        #region AppServiceConnection Event Handlers

        private void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var requestDeferral = args.GetDeferral();

            _logger.Log($"AppServiceClient.OnRequestReceived() Message=[{args.Request.Message.ToValueSetString()}]", Category.Info, Priority.High);

            try
            {
                if (args.Request.Message.ContainsKey(Parameters.DeviceInformation))
                {
                    var deviceInformationJsonText = (string)args.Request.Message[Parameters.DeviceInformation];
                    var deviceInformation = JsonConvert.DeserializeObject<WaterBoilerMatDeviceInformation>(deviceInformationJsonText);

                    WaterBoilerMatDeviceInformationUpdated?.Invoke(this, deviceInformation);
                }
            }
            catch (Exception e)
            {
                _logger.Log($"AppServiceClient.OnRequestReceived. Exception=[{e.Message}]", Category.Info, Priority.High);
            }
            finally
            {
                requestDeferral.Complete();
            }
        }

        private void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            ConnectedBluetoothLEDeviceInformation = null;
            IsOpened = false;
        }

        #endregion

        public async Task OpenAsync()
        {
            var status = await _appServiceConnection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                switch (status)
                {
                    case AppServiceConnectionStatus.AppNotInstalled:
                        throw new Exception($"The app AppServicesProvider is not installed. Deploy AppServicesProvider to this device and try again.");

                    case AppServiceConnectionStatus.AppUnavailable:
                        throw new Exception($"The app AppServicesProvider is not available. This could be because it is currently being updated or was installed to a removable device that is no longer available.");

                    case AppServiceConnectionStatus.AppServiceUnavailable:
                        throw new Exception($"The app AppServicesProvider is installed but it does not provide the app service {AppServiceName}.");
                   
                    case AppServiceConnectionStatus.Unknown:
                    default:
                        throw new Exception($"An unkown error occurred while we were trying to open an AppServiceConnection.");
                }
            }

            IsOpened = true;
            _appServiceConnection.RequestReceived += OnRequestReceived;
        }

        public Task CloseAsync()
        {
            ConnectedBluetoothLEDeviceInformation = null;
            IsOpened = false;

            _appServiceConnection.RequestReceived -= OnRequestReceived;
            _appServiceConnection.Dispose();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            CloseAsync();
        }

        public async Task<IList<IBluetoothLEDevice>> ScanAsync(int timeoutMilliseconds)
        {
            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.Scan);
            message.Add(Parameters.TimeoutMilliseconds, timeoutMilliseconds);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Devices) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Devices}\"");
            }

            var devicesJsonText = (string)responseMessage[Parameters.Devices];
            var devices = JsonConvert.DeserializeObject<IList<BluetoothLEDeviceInformation>>(devicesJsonText);
            
            return devices.OfType<IBluetoothLEDevice>().ToList();
        }

        public async Task<string> ConnectToDeviceAsync(string uniqueID, string deviceID)
        {
            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.Connect);
            message.Add(Parameters.DeviceID, deviceID);
            message.Add(Parameters.UniqueID, uniqueID);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }

            ConnectedBluetoothLEDeviceInformation = JsonConvert.DeserializeObject<BluetoothLEDeviceInformation>(detail);
            uniqueID = (string)responseMessage[Parameters.UniqueID];

            return uniqueID;
        }

        public async Task DisconnectToDeviceAsync(string deviceID = null)
        {
            deviceID = deviceID ?? ConnectedBluetoothLEDeviceInformation.Id;

            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.Disconnect);
            message.Add(Parameters.DeviceID, deviceID);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }
        }

        public async Task RequestPowerOnOffAsync(string deviceID = null)
        {
            deviceID = deviceID ?? ConnectedBluetoothLEDeviceInformation.Id;

            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.RequestPowerOnOff);
            message.Add(Parameters.DeviceID, deviceID);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }
        }

        public async Task RequestLockOnOffAsync(string deviceID = null)
        {
            deviceID = deviceID ?? ConnectedBluetoothLEDeviceInformation.Id;

            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.RequestLockOnOff);
            message.Add(Parameters.DeviceID, deviceID);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }
        }

        public async Task RequestLeftPartsPowerOnOffAsync(string deviceID = null)
        {
            deviceID = deviceID ?? ConnectedBluetoothLEDeviceInformation.Id;

            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.RequestLeftPartsPowerOnOff);
            message.Add(Parameters.DeviceID, deviceID);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }
        }

        public async Task RequestRightPartsPowerOnOffAsync(string deviceID = null)
        {
            deviceID = deviceID ?? ConnectedBluetoothLEDeviceInformation.Id;

            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.RequestRightPartsPowerOnOff);
            message.Add(Parameters.DeviceID, deviceID);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }
        }

        public async Task RequestVolumeChangeAsync(VolumeLevels value, string deviceID = null)
        {
            deviceID = deviceID ?? ConnectedBluetoothLEDeviceInformation.Id;

            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.RequestVolumeChange);
            message.Add(Parameters.DeviceID, deviceID);
            message.Add(Parameters.Value, (int)value);

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }
        }

        public async Task RequestSetupTemperatureChangeAsync(int setupLeftTemperature, int setupRightTemperature, string deviceID = null)
        {
            deviceID = deviceID ?? ConnectedBluetoothLEDeviceInformation.Id;

            //Send a message to the app service
            var message = new ValueSet();
            message.Add(Commands.Command, Commands.RequestSetupTemperatureChange);
            message.Add(Parameters.DeviceID, deviceID);
            message.Add(Parameters.Value, new int[] { setupLeftTemperature, setupRightTemperature });

            var responseMessage = await SendMessageAsync(message);

            // check the response message
            if (responseMessage.ContainsKey(Parameters.Result) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Result}\"");
            }
            if (responseMessage.ContainsKey(Parameters.Details) != true)
            {
                throw new Exception($"The app service response message does not contain a key called \"{Parameters.Details}\"");
            }

            var result = (bool)responseMessage[Parameters.Result];
            var detail = (string)responseMessage[Parameters.Details];
            if (result != true)
            {
                throw new Exception(detail);
            }
        }


        private async Task<ValueSet> SendMessageAsync(ValueSet message)
        {
            var sendMessageOperation = _appServiceConnection.SendMessageAsync(message);
            _logger.Log($"AppServiceClient.SendMessageAsync() Message=[{message.ToValueSetString()}]", Category.Info, Priority.High);

            // check the response status
            var response = await sendMessageOperation;
            _logger.Log($"AppServiceClient.SendMessageAsync() Response.Status=[{response.Status}], Response.Message=[{response.Message.ToValueSetString()}]", Category.Info, Priority.High);

            if (response.Status != AppServiceResponseStatus.Success)
            {
                switch (response.Status)
                {
                    case AppServiceResponseStatus.Failure:
                        throw new Exception($"The service failed to acknowledge the message we sent it. It may have been terminated because the client was suspended.");

                    case AppServiceResponseStatus.ResourceLimitsExceeded:
                        throw new Exception($"The service exceeded the resources allocated to it and had to be terminated.");

                    case AppServiceResponseStatus.Unknown:
                    default:
                        throw new Exception($"An unkown error occurred while we were trying to send a message to the service.");
                }
            }

            return response.Message;
        }
    }
}
