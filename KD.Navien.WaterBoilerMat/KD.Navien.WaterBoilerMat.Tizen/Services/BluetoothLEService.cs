using System;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Tizen.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using Prism.Logging;
using KD.Navien.WaterBoilerMat.Models;

[assembly: Dependency(typeof(BluetoothLEService))]
namespace KD.Navien.WaterBoilerMat.Tizen.Services
{
    public class BluetoothLEService : IBluetoothLEService<WaterBoilerMatDevice>
    {
        private ILoggerFacade logger;

        public BluetoothLEService(ILoggerFacade logger)
        {
            this.logger = logger;
        }

        public bool IsScanning => throw new NotImplementedException();
        
        public Task<IEnumerable<WaterBoilerMatDevice>> ScanAsync(int timeoutMilliseconds)
        {
            throw new NotImplementedException();
        }
    }
}