using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace KD.Navien.WaterBoilerMat.Universal.Common
{
    public interface INavigationViewItemPageAware
    {
        void OnNavigatedTo(NavigationEventArgs e);

        void OnNavigatedFrom(NavigationEventArgs e);
    }
}
