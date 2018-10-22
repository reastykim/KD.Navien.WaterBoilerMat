using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace KD.Navien.WaterBoilerMat.Universal.Converters
{
    public class WaterCapacityToFontIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var waterCapacity = (int)value;
                switch (waterCapacity)
                {
                    default:
                    case 0:
                        return "\uF5F2";
                    case 1:
                        return "\uF5F4";
                    case 2:
                        return "\uF5F8";
                    case 3:
                        return "\uF5FC";
                }
            }
            catch
            {
                return "\uF5F2";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
