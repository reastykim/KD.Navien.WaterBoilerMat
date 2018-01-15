using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.ViewModels;
using KD.Navien.WaterBoilerMat.Views;
using Prism;
using Prism.Ioc;
using Prism.Logging;
using Prism.Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace KD.Navien.WaterBoilerMat
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer)
		{

		}

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			var container = containerRegistry.GetContainer();
			foreach (var registration in container.Registrations.Where(p => p.RegisteredType == typeof(ILoggerFacade)))
			{
				registration.LifetimeManager.RemoveValue();
			}
			containerRegistry.RegisterSingleton<ILoggerFacade, DebugLogger>();
			containerRegistry.RegisterForNavigation<NavigationPage>();
			containerRegistry.RegisterForNavigation<MainPage>();
		}
	}
}
