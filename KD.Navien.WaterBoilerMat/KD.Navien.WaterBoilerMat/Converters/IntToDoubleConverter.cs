using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace KD.Navien.WaterBoilerMat.Converters
{
    public class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var intValue = (int)value;
                var strValue = intValue.ToString();
                var doubleValue = intValue / Math.Pow(10, strValue.Length);
                return doubleValue;
            }
            catch
            {
                return 0d;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var doubleValue = (double)value;
                var strValue = doubleValue.ToString();
                var intValue = int.Parse(strValue.Replace("0.", ""));
                return intValue;
            }
            catch
            {
                return 0;
            }
        }
    }
}
