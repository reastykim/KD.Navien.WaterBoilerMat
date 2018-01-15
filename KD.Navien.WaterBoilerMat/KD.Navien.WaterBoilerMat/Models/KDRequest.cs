using System;
using System.Collections.Generic;
using System.Text;
using static KD.Navien.WaterBoilerMat.Models.KDData;

namespace KD.Navien.WaterBoilerMat.Models
{
	public class KDRequest// implements Serializable
	{
		public KDData data = new KDData();

		//public string GetValue()
		//{
		//	string result;
		//	if (data.MessageType.Equals(KDMessageType.MAC_REGISTER))
		//	{
		//		result = data.MessageType + this.data.UniqueID;
		//	}
		//	else if (data.MessageType.Equals(KDMessageType.MAC_ACCESS))
		//	{
		//		result = data.MessageType + this.data.UniqueID;
		//	}
		//	else
		//	{
		//		result = ((((data.MessageType + String.Format("%02X", new Object[] { Integer.valueOf(this.data.Mode) })) + "0") + String.Format("%01X", new Object[] { Integer.valueOf(Integer.parseInt(Integer.toString(this.data.DegreeType) + Integer.toString(this.data.KeyLock) + Integer.toString(this.data.Power) + Integer.toString(this.data.MattType), 2)) })) + String.Format("%01X", new Object[] { Integer.valueOf(this.data.Volume) })) + String.Format("%01X", new Object[] { Integer.valueOf(this.data.WaterCapacity) });
		//		if (this.data.Mode == 1)
		//		{
		//			result = ((((((result + String.Format("%02X", new Object[] { Integer.valueOf(this.data.Status) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupply) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnRight) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupplySetting) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSettingLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSettingRight) });
		//		}
		//		else if (this.data.Mode == 2)
		//		{
		//			result = ((((((((result + String.Format("%02X", new Object[] { Integer.valueOf(this.data.Status) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupply) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnRight) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupplySetting) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSettingLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSettingRight) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.ReserveSettingTime) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.ReserveRemainTime) });
		//		}
		//		else if (this.data.Mode == 3)
		//		{
		//			result = (((((((((((result + String.Format("%02X", new Object[] { Integer.valueOf(this.data.Status) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupply) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnRight) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupplySetting) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSettingLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSettingRight) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.SleepSupplySettingTime) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.SleepLeftSettingTime) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.SleepRightSettingTime) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.SleepStartButtonEnable) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.SleepStopButtonEnable) });
		//		}
		//		else if (this.data.Mode == 4)
		//		{
		//			result = ((result + String.Format("%02X", new Object[] { Integer.valueOf(this.data.Status) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupply) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.CleanRemainTime) });
		//		}
		//		else if (this.data.Mode == 5)
		//		{
		//			result = (((result + String.Format("%02X", new Object[] { Integer.valueOf(this.data.Status) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupply) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnRight) });
		//			String code1 = Integer.toString(Integer.parseInt(result.substring(18, 20), 16));
		//			String code2 = Integer.toString(Integer.parseInt(result.substring(20, 22), 16));
		//			this.data.ErrorCode = Integer.parseInt(code1 + code2);
		//		}
		//		else if (this.data.Mode == 6)
		//		{
		//			result = ((((((((((result + "FE") + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureSupply) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnLeft) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.TemperatureReturnRight) })) + "FE") + String.Format("%02X", new Object[] { Integer.valueOf(this.data.PowerReserveOn) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.PowerReserveOnAfterHour) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.PowerReserveOnAfterMinute) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.PowerReserveOnAtHour) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.PowerReserveOnAtMinute) })) + String.Format("%02X", new Object[] { Integer.valueOf(this.data.PowerReserveOnTemperature) });
		//		}
		//	}
		//	for (int i = 0; i < 18 && result.Length < 36; i++)
		//	{
		//		result = result + "FE";
		//	}
		//	CRC8 crc = new CRC8();
		//	crc.reset();
		//	crc.update(result);
		//	result = this.data.STX + (result + crc.getValue());
		//	this.data.DEBUGCode = result;
		//	return result;
		//}
	}
}
