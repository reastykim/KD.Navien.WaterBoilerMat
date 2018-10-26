using KD.Navien.WaterBoilerMat.Universal.Extensions;
using Microsoft.Xaml.Interactivity;
using Prism.Windows.Navigation;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KD.Navien.WaterBoilerMat.Universal.RemoteApp.Behaviors
{
    public class PivotNavigationBehavior : Behavior<Pivot>
    {
        public object NavigationParameter
        {
            get { return (object)GetValue(NavigationParameterProperty); }
            set { SetValue(NavigationParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationParameterProperty =
            DependencyProperty.Register("NavigationParameter", typeof(object), typeof(PivotNavigationBehavior), new PropertyMetadata(null));


        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItem = e.RemovedItems.Cast<PivotItem>()
                .Select(i => i.GetPage<INavigationAware>()).FirstOrDefault();

            var addedItem = e.AddedItems.Cast<PivotItem>()
                .Select(i => i.GetPage<INavigationAware>()).FirstOrDefault();

            removedItem?.OnNavigatingFrom(new NavigatingFromEventArgs(), null, false);
            addedItem?.OnNavigatedTo(new NavigatedToEventArgs { NavigationMode = NavigationMode.New, Parameter = NavigationParameter }, null);
        }
    }
}
