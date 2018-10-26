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
        bool IsOpened { get; }

        Task OpenAsync();

        Task CloseAsync();

        Task<string> ConnectToDeviceAsync(string deviceID, string uniqueID);

        Task<IList<IBluetoothLEDevice>> ScanAsync(int scanTimeout);

        Task RequestPowerOnOffAsync(string deviceID);
    }
}
