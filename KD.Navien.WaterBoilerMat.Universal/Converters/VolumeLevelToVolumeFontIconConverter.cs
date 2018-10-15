using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace KD.Navien.WaterBoilerMat.Universal.Converters
{
    public class VolumeLevelToVolumeFontIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var volume = (int)value;
                switch (volume)
                {
                    default:
                    case 0:
                        return "\uE992";
                    case 1:
                        return "\uE993";
                    case 2:
                        return "\uE994";
                    case 3:
                        return "\uE995";
                }
            }
            catch
            {
                return "\uE992";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
