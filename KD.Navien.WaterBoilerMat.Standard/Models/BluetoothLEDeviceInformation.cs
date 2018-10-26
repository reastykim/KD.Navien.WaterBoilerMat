using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Models
{
    public class BluetoothLEDeviceInformation : IBluetoothLEDevice
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public List<IBluetoothGattService> Services { get; set; }
    }
}
