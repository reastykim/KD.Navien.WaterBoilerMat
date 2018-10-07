using KD.Navien.WaterBoilerMat.Universal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KD.Navien.WaterBoilerMat.Universal.Views
{
    public abstract class NavigationViewItemPage : Page
    {
        protected INavigationViewItemPageAware NavigationViewItemPageAware => DataContext as INavigationViewItemPageAware;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationViewItemPageAware?.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationViewItemPageAware?.OnNavigatedFrom(e);
        }
    }
}
