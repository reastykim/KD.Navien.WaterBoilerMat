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

		public class KDMessageType
		{
			public const string MAC_ACCESS = "00";
			public const string MAC_REGISTER = "01";
			public const string STATUS_CHANGE = "80";
			public const string STATUS_REFRESH = "30";
		}
	}
}
