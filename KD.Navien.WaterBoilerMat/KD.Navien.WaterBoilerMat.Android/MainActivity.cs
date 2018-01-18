using Android.App;
using Android.Content.PM;
using Android.OS;
using KD.Navien.WaterBoilerMat.Droid.Services;
using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using Prism;
using Prism.Ioc;
using Prism.Unity;

namespace KD.Navien.WaterBoilerMat.Droid
{
    [Activity(Label = "KD.Navien.WaterBoilerMat", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			// Register any platform specific implementations
			containerRegistry.Register<IBluetoothLEService<WaterBoilerMatDevice>, BluetoothLEService>();
		}
	}
}

