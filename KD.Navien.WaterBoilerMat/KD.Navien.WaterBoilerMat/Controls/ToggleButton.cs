using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KD.Navien.WaterBoilerMat.Controls
{
    public class ToggleButton : Button
    {
        public event EventHandler<ToggledEventArgs> Toggled;

        public bool IsToggled
        {
            set { SetValue(IsToggledProperty, value); }
            get { return (bool)GetValue(IsToggledProperty); }
        }
        public static BindableProperty IsToggledProperty =
            BindableProperty.Create("IsToggled", typeof(bool), typeof(ToggleButton), false,
                                    propertyChanged: OnIsToggledChanged);

        public ToggleButton()
        {
            Clicked += (sender, args) => IsToggled ^= true;

            UpdateColor();
        }        

        static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ToggleButton toggleButton = (ToggleButton)bindable;
            bool isToggled = (bool)newValue;

            toggleButton.UpdateColor();

            // Fire event
            toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));
        }

        private void UpdateColor()
        {
            BackgroundColor = IsToggled ? Color.Accent : Color.Default;
            TextColor = IsToggled ? Color.White : Color.Default;
        }
    }
}
