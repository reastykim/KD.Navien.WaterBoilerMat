using System;
using System.Collections.Generic;
using System.Text;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.Services.Protocol
{
	public class KDRequest// implements Serializable
	{
		public KDData Data { get; set; } = new KDData();

		public string GetValue()
		{
			string result = String.Empty;

			switch (Data.MessageType)
			{
				case KDMessageType.MAC_REGISTER:
				case KDMessageType.MAC_ACCESS:
					result = Data.MessageType + Data.UniqueID;
					break;
				default:
					result = Data.MessageType + Data.Mode.ToString("X02") + "0" +
							 Convert.ToInt32(Data.DegreeType + Data.KeyLock.ToString() + Data.Power.ToString() + Data.MattType.ToString(), 2).ToString("X01") +
							 Data.Volume.ToString("X01") +
							 Data.WaterCapacity.ToString("X01");

					if (Data.Mode == 1)
					{
						result = result + Data.Status.ToString("X02") +
										  Data.TemperatureSupply.ToString("X02") +
										  Data.TemperatureReturnLeft.ToString("X02") +
										  Data.TemperatureReturnRight.ToString("X02") +
										  Data.TemperatureSupplySetting.ToString("X02") +
										  Data.TemperatureSettingLeft.ToString("X02") +
										  Data.TemperatureSettingRight.ToString("X02");

					}
					else if (Data.Mode == 2)
					{
						result = result + Data.Status.ToString("X02") +
										  Data.TemperatureSupply.ToString("X02") +
										  Data.TemperatureReturnLeft.ToString("X02") +
										  Data.TemperatureReturnRight.ToString("X02") +
										  Data.TemperatureSupplySetting.ToString("X02") +
										  Data.TemperatureSettingLeft.ToString("X02") +
										  Data.TemperatureSettingRight.ToString("X02") +
										  Data.ReserveSettingTime.ToString("X02") +
										  Data.ReserveRemainTime.ToString("X02");
					}
					else if (Data.Mode == 3)
					{
						result = result + Data.Status.ToString("X02") +
										  Data.TemperatureSupply.ToString("X02") +
										  Data.TemperatureReturnLeft.ToString("X02") +
										  Data.TemperatureReturnRight.ToString("X02") +
										  Data.TemperatureSupplySetting.ToString("X02") +
										  Data.TemperatureSettingLeft.ToString("X02") +
										  Data.TemperatureSettingRight.ToString("X02") +
										  Data.SleepSupplySettingTime.ToString("X02") +
										  Data.SleepLeftSettingTime.ToString("X02") +
										  Data.SleepRightSettingTime.ToString("X02") +
										  Data.SleepStartButtonEnable.ToString("X02") +
										  Data.SleepStopButtonEnable.ToString("X02");
					}
					else if (Data.Mode == 4)
					{
						result = result + Data.Status.ToString("X02") +
										  Data.TemperatureSupply.ToString("X02") +
										  Data.CleanRemainTime.ToString("X02");
					}
					else if (Data.Mode == 5)
					{
						result = result + Data.Status.ToString("X02") +
										  Data.TemperatureSupply.ToString("X02") +
										  Data.TemperatureReturnLeft.ToString("X02") +
										  Data.TemperatureReturnRight.ToString("X02");

						string code1 = Convert.ToInt32(result.Substring(18, 2), 16).ToString();
						string code2 = Convert.ToInt32(result.Substring(20, 2), 16).ToString();
						Data.ErrorCode = int.Parse(code1 + code2);
					}
					else if (Data.Mode == 6)
					{
						result = result + "FE" +
										  Data.TemperatureSupply.ToString("X02") +
										  Data.TemperatureReturnLeft.ToString("X02") +
										  Data.TemperatureReturnRight.ToString("X02") +
										  "FE" +
										  Data.PowerReserveOn.ToString("X02") +
										  Data.PowerReserveOnAfterHour.ToString("X02") +
										  Data.PowerReserveOnAfterMinute.ToString("X02") +
										  Data.PowerReserveOnAtHour.ToString("X02") +
										  Data.PowerReserveOnAtMinute.ToString("X02") +
										  Data.PowerReserveOnTemperature.ToString("X02");
					}
					break;

			}

			for (int i = 0; i < 18 && result.Length < 36; i++)
			{
				result = result + "FE";
			}

			CRC8 crc = new CRC8();
			crc.Reset();
			crc.Update(result);
			result = Data.STX + (result + crc.GetValue());
			Data.DEBUGCode = result;

			return result;
		}
	}
}
