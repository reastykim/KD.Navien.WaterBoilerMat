using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static KD.Navien.WaterBoilerMat.Services.Protocol.KDData;

namespace KD.Navien.WaterBoilerMat.Services.Protocol
{
	public class KDResponse// implements Serializable
	{
		public KDData Data { get; set; } = new KDData();

		public bool SetValue(byte[] pData)
		{
			String result = String.Join("", pData.Select(d => d.ToString("X2")));
			Data.DEBUGCode = result;
			if (result.Length != 40)
			{
				return false;
			}
			CRC8 crc = new CRC8();
			crc.Reset();
			crc.Update(result.Substring(2));
			String crcValue = crc.GetValue();
			if (!Data.STX.Equals(result.Substring(0, 2)))
			{
				return false;
			}
			Data.MessageType = result.Substring(2, 2);
			if (Data.MessageType.Equals("F1")) // TODO : need to debug, only response
			{
				if (result.Substring(4, 2).Equals(KDMessageType.MAC_ACCESS))
				{
					return false;
				}
				if (result.Substring(4, 2).Equals(KDMessageType.MAC_REGISTER))
				{
					Data.UniqueID = result.Substring(6, 12);
				}
				else if (result.Substring(4, 2).Equals("02"))
				{
				}
			}
			else if (!Data.MessageType.Equals("F0"))
			{
				Data.Mode = Convert.ToInt32(result.Substring(4, 2), 16);

				string parseHex = Convert.ToString(Convert.ToInt32(result.Substring(6, 2), 16), 2).PadLeft(4, '0');

				Data.MattType = Convert.ToInt32(parseHex.Substring(3, 1), 2);
				Data.Power = Convert.ToInt32(parseHex.Substring(2, 1), 2);
				Data.KeyLock = Convert.ToInt32(parseHex.Substring(1, 1), 2);
				Data.DegreeType = Convert.ToInt32(parseHex.Substring(0, 1), 2);
				Data.Volume = Convert.ToInt32(result.Substring(8, 1), 16);
				Data.WaterCapacity = Convert.ToInt32(result.Substring(9, 1), 16);
				Data.MaxTemperatureHighLow = Convert.ToInt32(result.Substring(36, 2), 16);
				Data.ModelType = Convert.ToInt32(result.Substring(34, 2), 16);

				if (Data.Mode == 1)
				{
					Data.Status = Convert.ToInt32(result.Substring(10, 2), 16);
					Data.TemperatureSupply = Convert.ToInt32(result.Substring(12, 2), 16);
					Data.TemperatureReturnLeft = Convert.ToInt32(result.Substring(14, 2), 16);
					Data.TemperatureReturnRight = Convert.ToInt32(result.Substring(16, 2), 16);
					Data.TemperatureSupplySetting = Convert.ToInt32(result.Substring(18, 2), 16);
					Data.TemperatureSettingLeft = Convert.ToInt32(result.Substring(20, 2), 16);
					Data.TemperatureSettingRight = Convert.ToInt32(result.Substring(22, 2), 16);
				}
				else if (Data.Mode == 2)
				{
					Data.Status = Convert.ToInt32(result.Substring(10, 2), 16);
					Data.TemperatureSupply = Convert.ToInt32(result.Substring(12, 2), 16);
					Data.TemperatureReturnLeft = Convert.ToInt32(result.Substring(14, 2), 16);
					Data.TemperatureReturnRight = Convert.ToInt32(result.Substring(16, 2), 16);
					Data.TemperatureSupplySetting = Convert.ToInt32(result.Substring(18, 2), 16);
					Data.TemperatureSettingLeft = Convert.ToInt32(result.Substring(20, 2), 16);
					Data.TemperatureSettingRight = Convert.ToInt32(result.Substring(22, 2), 16);
					Data.ReserveSettingTime = Convert.ToInt32(result.Substring(24, 2), 16);
					Data.ReserveRemainTime = Convert.ToInt32(result.Substring(26, 2), 16);
				}
				else if (Data.Mode == 3)
				{
					Data.Status = Convert.ToInt32(result.Substring(10, 2), 16);
					Data.TemperatureSupply = Convert.ToInt32(result.Substring(12, 2), 16);
					Data.TemperatureReturnLeft = Convert.ToInt32(result.Substring(14, 2), 16);
					Data.TemperatureReturnRight = Convert.ToInt32(result.Substring(16, 2), 16);
					Data.TemperatureSupplySetting = Convert.ToInt32(result.Substring(18, 2), 16);
					Data.TemperatureSettingLeft = Convert.ToInt32(result.Substring(20, 2), 16);
					Data.TemperatureSettingRight = Convert.ToInt32(result.Substring(22, 2), 16);
					Data.SleepSupplySettingTime = Convert.ToInt32(result.Substring(24, 2), 16);
					Data.SleepLeftSettingTime = Convert.ToInt32(result.Substring(26, 2), 16);
					Data.SleepRightSettingTime = Convert.ToInt32(result.Substring(28, 2), 16);
					Data.SleepStartButtonEnable = Convert.ToInt32(result.Substring(30, 2), 16);
					Data.SleepStopButtonEnable = Convert.ToInt32(result.Substring(32, 2), 16);
				}
				else if (Data.Mode == 4)
				{
					Data.Status = Convert.ToInt32(result.Substring(10, 2), 16);
					Data.TemperatureSupply = Convert.ToInt32(result.Substring(12, 2), 16);
					Data.CleanRemainTime = Convert.ToInt32(result.Substring(14, 2), 16);
				}
				else if (Data.Mode == 5) // TODO : need to debug, only response
				{
					Data.Status = Convert.ToInt32(result.Substring(10, 2), 16);
					Data.TemperatureSupply = Convert.ToInt32(result.Substring(12, 2), 16);
					Data.TemperatureReturnLeft = Convert.ToInt32(result.Substring(14, 2), 16);
					Data.TemperatureReturnRight = Convert.ToInt32(result.Substring(16, 2), 16);
					string code1 = Convert.ToInt32(result.Substring(18, 2), 16).ToString();
					string code2 = Convert.ToInt32(result.Substring(20, 2), 16).ToString();
					Data.ErrorCode = int.Parse(code1 + code2);
				}
				else if (Data.Mode == 6)
				{
					Data.TemperatureSupply = Convert.ToInt32(result.Substring(12, 2), 16);
					Data.TemperatureReturnLeft = Convert.ToInt32(result.Substring(14, 2), 16);
					Data.TemperatureReturnRight = Convert.ToInt32(result.Substring(16, 2), 16);
					Data.PowerReserveOn = Convert.ToInt32(result.Substring(20, 2), 16);
					Data.PowerReserveOnAfterHour = Convert.ToInt32(result.Substring(22, 2), 16);
					Data.PowerReserveOnAfterMinute = Convert.ToInt32(result.Substring(24, 2), 16);
					Data.PowerReserveOnAtHour = Convert.ToInt32(result.Substring(26, 2), 16);
					Data.PowerReserveOnAtMinute = Convert.ToInt32(result.Substring(28, 2), 16);
					Data.PowerReserveOnTemperature = Convert.ToInt32(result.Substring(30, 2), 16);
				}
			}
			else if (result.Substring(4, 2).Equals(KDMessageType.MAC_ACCESS))
			{
				return false;
			}
			return true;
		}
	}
}
