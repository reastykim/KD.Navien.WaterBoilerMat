﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using KD.Navien.WaterBoilerMat.Universal.Services;
using Prism.Logging;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using Windows.Devices.Bluetooth.Advertisement;
using System.Threading.Tasks;
using System.Threading;
using static KD.Navien.WaterBoilerMat.Standard.Services.AppServiceCommands;

namespace KD.Navien.WaterBoilerMat.Universal.BackgroundApp
{
    public sealed partial class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private AppServiceConnection _connection;
        

        private static ILoggerFacade _logger;
        private static IBluetoothLEService<WaterBoilerMatDevice> _bluetoothLEService;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskInstanceCanceled;

            Initialize();

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _connection = details.AppServiceConnection;

            //Listen for incoming app service requests
            _connection.RequestReceived += OnRequestReceived;
        }

        private void OnTaskInstanceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_deferral != null)
            {
                CleanUp();

                _connection.RequestReceived -= OnRequestReceived;
                _connection.Dispose();
                _connection = null;

                //Complete the service deferral
                _deferral.Complete();
                _deferral = null;
            }
        }

        private void Initialize()
        {
            _logger = _logger ?? new DebugLogger();
            _bluetoothLEService = _bluetoothLEService ?? new BluetoothLEService(_logger);
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //Get a deferral so we can use an awaitable API to respond to the message
            var messageDeferral = args.GetDeferral();

            try
            {
                AppServiceResponseStatus responseStatus;
                var message = args.Request.Message;
                string command = message[Commands.Command].ToString();

                switch (command)
                {
                    case Commands.Scan:
                        var timeoutMilliseconds = (int)message[Parameters.TimeoutMilliseconds];
                        responseStatus = await ScanAsync(timeoutMilliseconds, args.Request);
                        break;
                    case Commands.Connect:
                        var deviceId = (string)message[Parameters.DeviceID];
                        var uniqueId = (string)message[Parameters.UniqueID];
                        responseStatus = await ConnectAsync(deviceId, uniqueId, args.Request);
                        break;
                    //Other commands
                    default:
                        return;
                }
            }
            finally
            {
                //Complete the message deferral so the operating system knows we're done responding
                messageDeferral.Complete();
            }
        }
    }
}
