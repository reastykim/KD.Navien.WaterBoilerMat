using KD.Navien.WaterBoilerMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace KD.Navien.WaterBoilerMat.Universal.Converters
{
    public class ItemClickEventArgsConverter<T> : IValueConverter where T : class
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ItemClickEventArgs args)
            {
                return args.ClickedItem as T;
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

    public class ItemClickEventArgsToWaterBoilerMatDeviceConverter : ItemClickEventArgsConverter<WaterBoilerMatDevice>
    {
    }    
}
