using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Standard.Services
{
    public class AppServiceCommands
    {
        public class Commands
        {
            public const string Command = "Command";
            public const string Scan = "ScanCommand";
            public const string Connect = "ConnectCommand";
        }

        public class Parameters
        {
            public const string TimeoutMilliseconds = "timeoutMilliseconds";
            public const string Devices = "devices";
            public const string DeviceID = "deviceID";
            public const string UniqueID = "uniqueID";
            public const string Result = "result";
            public const string Details = "details";
            public const string Device = "device";
        }
    }
}
