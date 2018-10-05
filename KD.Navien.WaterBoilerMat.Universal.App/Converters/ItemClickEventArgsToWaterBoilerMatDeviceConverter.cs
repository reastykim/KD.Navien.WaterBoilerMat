using KD.Navien.WaterBoilerMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace KD.Navien.WaterBoilerMat.Universal.App.Converters
{
    public class ItemClickEventArgsToWaterBoilerMatDeviceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ItemClickEventArgs args)
            {
                return args.ClickedItem as WaterBoilerMatDevice;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
