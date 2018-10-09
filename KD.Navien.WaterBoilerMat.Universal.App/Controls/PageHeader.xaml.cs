using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace KD.Navien.WaterBoilerMat.Universal.App.Controls
{
    public sealed partial class PageHeader : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(PageHeader), new PropertyMetadata(null));
        public object Title
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public double BackgroundColorOpacity
        {
            get { return (double)GetValue(BackgroundColorOpacityProperty); }
            set { SetValue(BackgroundColorOpacityProperty, value); }
        }
        public static readonly DependencyProperty BackgroundColorOpacityProperty =
            DependencyProperty.Register("BackgroundColorOpacity", typeof(double), typeof(PageHeader), new PropertyMetadata(0.0));

        public double AcrylicOpacity
        {
            get { return (double)GetValue(AcrylicOpacityProperty); }
            set { SetValue(AcrylicOpacityProperty, value); }
        }
        public static readonly DependencyProperty AcrylicOpacityProperty =
            DependencyProperty.Register("AcrylicOpacity", typeof(double), typeof(PageHeader), new PropertyMetadata(0.3));

        public ICommand PowerCommand
        {
            get { return (ICommand)GetValue(PowerCommandProperty); }
            set { SetValue(PowerCommandProperty, value); }
        }
        public static readonly DependencyProperty PowerCommandProperty =
            DependencyProperty.Register("PowerCommand", typeof(ICommand), typeof(PageHeader), new PropertyMetadata(null));

        public ICommand LockCommand
        {
            get { return (ICommand)GetValue(LockCommandProperty); }
            set { SetValue(LockCommandProperty, value); }
        }
        public static readonly DependencyProperty LockCommandProperty =
            DependencyProperty.Register("LockCommand", typeof(ICommand), typeof(PageHeader), new PropertyMetadata(null));



        public CommandBar TopCommandBar
        {
            get { return topCommandBar; }
        }

        public UIElement TitlePanel
        {
            get { return pageTitle; }
        }

        public PageHeader()
        {
            this.InitializeComponent();
        }

        public void UpdateBackground(bool isFilteredPage)
        {
            VisualStateManager.GoToState(this, isFilteredPage ? "FilteredPage" : "NonFilteredPage", false);
        }

        private void Layout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 593)
            {
                headerRoot.VerticalAlignment = VerticalAlignment.Center;
            }
            else
            {
                headerRoot.VerticalAlignment = VerticalAlignment.Bottom;
            }
        }
    }
}
