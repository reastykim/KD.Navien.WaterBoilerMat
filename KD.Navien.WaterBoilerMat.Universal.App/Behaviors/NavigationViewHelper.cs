using KD.Navien.WaterBoilerMat.Universal.App.Models;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace KD.Navien.WaterBoilerMat.Universal.App.Behaviors
{
    public class NavigationViewHelper : Behavior<NavigationView>
    {
        #region Properties

        public Type SettingsPageType { get; set; }

        public Frame Frame
        {
            get { return (Frame)GetValue(FrameProperty); }
            set
            {
                if (Frame != null)
                {
                    Frame.Navigated -= Frame_Navigated;
                }
                SetValue(FrameProperty, value);
            }
        }
        public static readonly DependencyProperty FrameProperty =
            DependencyProperty.Register("Frame", typeof(Frame), typeof(NavigationViewHelper), new PropertyMetadata(null, OnFramePropertyChanged));
        private static void OnFramePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var navigationViewHelper = d as NavigationViewHelper;
            if (e.NewValue is Frame newFrame)
            {
                newFrame.Navigated += navigationViewHelper.Frame_Navigated;
            }
        }

        #endregion

        #region Fields

        private SystemNavigationManager systemNavigationManager;

        #endregion

        protected override void OnAttached()
        {
            AssociatedObject.SelectionChanged += NavigationView_SelectionChanged;

            // Handle keyboard and mouse navigation requests
            systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += SystemNavigationManager_BackRequested;

            // must register back requested on navview
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                AssociatedObject.BackRequested += NavigationView_BackRequested;
            }

            // Listen to the window directly so we will respond to hotkeys regardless of which element has focus.
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
        }

        protected override void OnDetaching()
        {
            Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;

            // must unregister back requested on navview
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                AssociatedObject.BackRequested -= NavigationView_BackRequested;
            }

            AssociatedObject.SelectionChanged -= NavigationView_SelectionChanged;
        }

        #region Methods

        public void Navigate(Type sourcePageType, object parameter = null)
        {
            Frame.Navigate(sourcePageType, parameter);
        }

        private bool TryGoBack()
        {
            // don't go back if the nav pane is overlayed
            if (AssociatedObject.IsPaneOpen &&
                (AssociatedObject.DisplayMode == NavigationViewDisplayMode.Compact || AssociatedObject.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return false;
            }

            bool navigated = false;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
                navigated = true;
            }

            return navigated;
        }

        private bool TryGoForward()
        {
            bool navigated = false;
            if (Frame.CanGoForward)
            {
                Frame.GoForward();
                navigated = true;
            }
            return navigated;
        }

        private void UpdateBackButton()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                AssociatedObject.IsBackEnabled = Frame.CanGoBack ? true : false;
                AssociatedObject.IsBackButtonVisible = AssociatedObject.IsBackEnabled ? NavigationViewBackButtonVisible.Visible : 
                                                                                        NavigationViewBackButtonVisible.Collapsed;
            }
            else
            {
                systemNavigationManager.AppViewBackButtonVisibility = Frame.CanGoBack ? AppViewBackButtonVisibility.Visible :
                                                                                        AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void UpdateNavViewSelectedItem(NavigationEventArgs e)
        {
            // When be called GoBack() method, set the IsSelected for target menu.
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (e.SourcePageType == SettingsPageType)
                {
                    AssociatedObject.SelectedItem = AssociatedObject.SettingsItem;
                }
                else if (AssociatedObject.MenuItemsSource is IList<NavigationViewItemData> itemSource)
                {
                    AssociatedObject.SelectedItem = itemSource.Single(I => I.TargetPageType == e.SourcePageType);
                }
            }
        }

        #endregion

        #region Event Handlers

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                Navigate(SettingsPageType);
            }
            else
            {
                if (args.SelectedItem is NavigationViewItemData item)
                {
                    Navigate(item.TargetPageType);
                }
            }
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations.
        /// Used to detect keyboard navigation between pages even when the page itself
        /// doesn't have focus.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Only investigate further when Left, Right, or the dedicated Previous or Next keys
            // are pressed
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // When the previous key or Alt+Left are pressed navigate back
                    e.Handled = TryGoBack();
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    e.Handled = TryGoForward();
                }
            }
        }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction.
        /// Used to detect browser-style next and previous mouse button clicks
        /// to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || 
                properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed)
                return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) TryGoBack();
                if (forwardPressed) TryGoForward();
            }
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // Update the Back button whenever a navigation occurs.
            UpdateBackButton();

            UpdateNavViewSelectedItem(e);
        }

        #endregion
    }
}
