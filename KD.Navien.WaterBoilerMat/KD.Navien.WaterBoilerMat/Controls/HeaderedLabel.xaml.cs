using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KD.Navien.WaterBoilerMat.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HeaderedLabel : ContentView
	{
        public object Header
        {
            set { SetValue(HeaderProperty, value); }
            get { return GetValue(HeaderProperty); }
        }
        public static BindableProperty HeaderProperty =
            BindableProperty.Create("Header", typeof(object), typeof(HeaderedLabel), false);

        public string Text
        {
            set { SetValue(TextProperty, value); }
            get { return (string)GetValue(TextProperty); }
        }
        public static BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(object), typeof(HeaderedLabel), false);


        public HeaderedLabel()
		{
			InitializeComponent();
		}
	}
}