using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
    public enum TemperatureInfoType
    {
        Type4 = 4,
        Type3 = 3,
        Type2 = 2, 
        Type1 = 1,
        Unknown
    }

    public class TemperatureInfo
    {
        private readonly Dictionary<TemperatureInfoType, int> MaximumTemperatures = new Dictionary<TemperatureInfoType, int>()
        {
            { TemperatureInfoType.Type4, 45 },
            { TemperatureInfoType.Type3, 50 },
            { TemperatureInfoType.Type2, 48 },
            { TemperatureInfoType.Type1, 45 },
            //{ TemperatureInfoType.Unknown, 43 }
        };
        private readonly Dictionary<TemperatureInfoType, int> MinimumTemperatures = new Dictionary<TemperatureInfoType, int>()
        {
            { TemperatureInfoType.Type4, 28 },
            { TemperatureInfoType.Type3, 25 },
            { TemperatureInfoType.Type2, 25 },
            { TemperatureInfoType.Type1, 25 },
            //{ TemperatureInfoType.Unknown, 25 }
        };

        public int MaximumTemperature { get; }
        public int MinimumTemperature { get; }

        public int OverHeatTemperature => 37;

        public TemperatureInfo(int maxTemperatureHighLow)
        {
            var temperatureInfoType = (TemperatureInfoType)maxTemperatureHighLow;
            if (temperatureInfoType == TemperatureInfoType.Unknown)
                throw new ArgumentOutOfRangeException(nameof(maxTemperatureHighLow));

            MaximumTemperature = MaximumTemperatures[temperatureInfoType];
            MinimumTemperature = MinimumTemperatures[temperatureInfoType];
        }
    }
}
