using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Services.Protocol
{
    public class KDData// implements Serializable
    {
        public int CRC;
        public int CleanRemainTime;
        public string DEBUGCode;
        public int DegreeType;
        public int ErrorCode;
        public int KeyLock;
        public int MattType;
        public int MaxTemperatureHighLow;
        public string MessageType;
        public int Mode;
        public int ModelType;
        public int Power;
        public int PowerReserveOn;
        public int PowerReserveOnAfterHour;
        public int PowerReserveOnAfterMinute;
        public int PowerReserveOnAtHour;
        public int PowerReserveOnAtMinute;
        public int PowerReserveOnTemperature;
        public int ReserveRemainTime;
        public int ReserveSettingTime;
        public string STX = "B2";
        public int SleepLeftSettingTime;
        public int SleepRightSettingTime;
        public int SleepStartButtonEnable;
        public int SleepStopButtonEnable;
        public int SleepSupplySettingTime;
        public int Status;
        public int TemperatureReturnLeft;
        public int TemperatureReturnRight;
        public int TemperatureSettingLeft;
        public int TemperatureSettingRight;
        public int TemperatureSupply;
        public int TemperatureSupplySetting;
        public string UniqueID;
        public int Volume;
        public int WaterCapacity;

        public override string ToString()
        {
            return $"KD.Navien.WaterBoilerMat.Services.Protocol.KDData, MessageType=[{MessageType}], Mode=[{Mode}], Status=[{Status}], " +
                $"TemperatureSettingLeft=[{TemperatureSettingLeft}], TemperatureSettingRight=[{TemperatureSettingRight}], " +
                $"DEBUGCode=[{DEBUGCode}]";
        }

        public class KDMessageType
        {
            public const string MAC_ACCESS = "00";
            public const string MAC_REGISTER = "01";
            public const string STATUS_CHANGE = "80";
            public const string STATUS_REFRESH = "30";
        }

        const int MODE_NORMAL = 1;
        const int MODE_OFF = 6;

        enum eStatus : byte
        {
            Off = 0,
            On = 2,
            LeftOn = 3, // LeftOnly On
            RightOn = 4,// RightOnly On
        }
    }
}
