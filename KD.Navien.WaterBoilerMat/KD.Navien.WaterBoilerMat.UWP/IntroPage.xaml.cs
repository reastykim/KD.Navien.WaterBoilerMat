﻿using System;
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
using KD.Navien.WaterBoilerMat.Universal.Services;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Universal.Models;

namespace KD.Navien.WaterBoilerMat.UWP
{
    public sealed partial class IntroPage
    {
        public IntroPage()
        {
            this.InitializeComponent();

            LoadApplication(new KD.Navien.WaterBoilerMat.App(new UwpInitializer()));
        }
    }

    public class UwpInitializer : IPlatformInitializer
    {
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
            // Register any platform specific implementations
            containerRegistry.Register<IBluetoothLEService<WaterBoilerMatDevice>, BluetoothLEService>();
            containerRegistry.Register<IPairingList, PairingList>();
        }
	}
}
