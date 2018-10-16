using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace KD.Navien.WaterBoilerMat.Universal.Behaviors
{
    public class HideParentFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var s = sender as FrameworkElement;
            var presenter = s?.FindAscendant<FlyoutPresenter>();
            var popup = presenter?.FindParent<Popup>();
            if (popup != null)
            {
                popup.IsOpen = false;
            }

            return sender;
        }
    }
}
