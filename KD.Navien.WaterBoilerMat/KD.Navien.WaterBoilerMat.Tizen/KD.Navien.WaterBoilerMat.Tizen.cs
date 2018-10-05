using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Tizen.Logging;
using KD.Navien.WaterBoilerMat.Tizen.Services;
using Prism;
using Prism.Ioc;
using Prism.Logging;
using Prism.Unity;
using System;

namespace KD.Navien.WaterBoilerMat.Tizen
{
    class Program : global::Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            LoadApplication(new App(new TizenInitializer()));
        }

        static void Main(string[] args)
        {
            var app = new Program();
            global::Xamarin.Forms.Platform.Tizen.Forms.Init(app);
            app.Run(args);
        }
    }

	public class TizenInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register any platform specific implementations
			containerRegistry.RegisterSingleton<ILoggerFacade, TizenLogger>();
            containerRegistry.Register<IBluetoothLEService<WaterBoilerMatDevice>, BluetoothLEService>();
        }
	}
}
