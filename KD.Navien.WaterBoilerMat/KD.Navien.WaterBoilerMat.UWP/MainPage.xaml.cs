using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Prism.Unity;
using Prism;
using Unity;
using Prism.Ioc;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.UWP.Services;

namespace KD.Navien.WaterBoilerMat.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new KD.Navien.WaterBoilerMat.App(new UwpInitializer()));
        }
    }

    public class UwpInitializer : IPlatformInitializer
    {
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.Register<IBluetoothService, BluetoothService>();
		}
	}
}
