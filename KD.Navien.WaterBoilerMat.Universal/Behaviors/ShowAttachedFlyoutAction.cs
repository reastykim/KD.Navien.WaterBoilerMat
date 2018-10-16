using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace KD.Navien.WaterBoilerMat.Universal.Behaviors
{
    public class ShowAttachedFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            var owner = sender as FrameworkElement;
            if (owner != null)
            {
                FlyoutBase.ShowAttachedFlyout(owner);
            }

            return owner;
        }
    }
}
