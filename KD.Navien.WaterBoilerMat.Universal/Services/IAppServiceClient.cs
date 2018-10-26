using KD.Navien.WaterBoilerMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Universal.Services
{
    public interface IAppServiceClient : IDisposable
    {
        event EventHandler<WaterBoilerMatDeviceInformation> WaterBoilerMatDeviceInformationUpdated;

        IBluetoothLEDevice ConnectedBluetoothLEDeviceInformation { get; }

        bool IsOpened { get; }

        Task OpenAsync();

        Task CloseAsync();

        Task<string> ConnectToDeviceAsync(string uniqueID, string deviceID);

        Task DisconnectToDeviceAsync(string deviceID = null);

        Task<IList<IBluetoothLEDevice>> ScanAsync(int scanTimeout);

        Task RequestPowerOnOffAsync(string deviceID = null);

        Task RequestLockOnOffAsync(string deviceID = null);

        Task RequestLeftPartsPowerOnOffAsync(string deviceID = null);
        Task RequestRightPartsPowerOnOffAsync(string deviceID = null);

        Task RequestVolumeChangeAsync(VolumeLevels value, string deviceID = null);

        Task RequestSetupTemperatureChangeAsync(int setupLeftTemperature, int setupRightTemperature, string deviceID = null);
    }
}
