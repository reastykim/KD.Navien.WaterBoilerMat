using System;
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

namespace KD.Navien.WaterBoilerMat.Universal.BackgroundApp
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private AppServiceConnection _connection;
        private Random _randomNumberGenerator;
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
                var message = args.Request.Message;
                string command = message["Command"] as string;

                switch (command)
                {
                    case "Scan":
                        {
                            var timeout = (int)message["timeout"];
                            var devices = await _bluetoothLEService.ScanWaterBoilerMatDeviceAddressAsync(timeout);



                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(devices, Newtonsoft.Json.Formatting.Indented);



                            var result = new ValueSet();
                            result.Add("devices", json);

                            await args.Request.SendResponseAsync(result);
                        }
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

        private void AdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {

        }
    }
}
