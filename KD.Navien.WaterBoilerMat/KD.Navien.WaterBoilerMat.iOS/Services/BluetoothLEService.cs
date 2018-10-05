using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using KD.Navien.WaterBoilerMat.iOS.Services;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(BluetoothLEService))]
namespace KD.Navien.WaterBoilerMat.iOS.Services
{
    public class BluetoothLEService : IBluetoothLEService<WaterBoilerMatDevice>
    {
        public bool IsScanning => throw new NotImplementedException();

        public Task<IEnumerable<WaterBoilerMatDevice>> ScanAsync(int timeoutMilliseconds)
        {
            throw new NotImplementedException();
        }
    }
}