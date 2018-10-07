using KD.Navien.WaterBoilerMat.Models;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Universal.Models;
using KD.Navien.WaterBoilerMat.Universal.Services;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Prism.Windows.AppModel;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace KD.Navien.WaterBoilerMat.Universal.App
{
    /// <summary>
    /// This class uses the MvvmAppBase class to bootstrap this Windows Store App with Mvvm support
    /// http://go.microsoft.com/fwlink/?LinkID=288809&clcid=0x409
    /// </summary>
    public sealed partial class App : PrismUnityApplication
    {
        public new IEventAggregator EventAggregator { get; set; }
                
        // Documentation on navigation between pages is at http://go.microsoft.com/fwlink/?LinkID=288815&clcid=0x409
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            if (args != null && !string.IsNullOrEmpty(args.Arguments))
            {
                // The app was launched from a Secondary Tile
                // Navigate to the item's page
                NavigationService.Navigate("Intro", args.Arguments);
            }
            else
            {
                // Navigate to the initial page
                NavigationService.Navigate("Intro", null);
            }

            Window.Current.Activate();
            return Task.FromResult<object>(null);
        }

        protected override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            EventAggregator = new EventAggregator();

            Container.RegisterInstance<INavigationService>(NavigationService);
            Container.RegisterInstance<ISessionStateService>(SessionStateService);
            Container.RegisterInstance<IEventAggregator>(EventAggregator);
            //Container.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            Container.RegisterType<IBluetoothLEService<WaterBoilerMatDevice>, BluetoothLEService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAlertMessageService, AlertMessageService>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IPairingList, PairingList>(new ContainerControlledLifetimeManager());

            

            //// Register child view models
            //Container.RegisterType<IShippingAddressUserControlViewModel, ShippingAddressUserControlViewModel>();
            //Container.RegisterType<IBillingAddressUserControlViewModel, BillingAddressUserControlViewModel>();
            //Container.RegisterType<IPaymentMethodUserControlViewModel, PaymentMethodUserControlViewModel>();
            //Container.RegisterType<ISignInUserControlViewModel, SignInUserControlViewModel>();

            //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            //{
            //    var viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "AdventureWorks.UILogic.ViewModels.{0}ViewModel, AdventureWorks.UILogic, Version=1.1.0.0, Culture=neutral", viewType.Name);
            //    var viewModelType = Type.GetType(viewModelTypeName);
            //    if (viewModelType == null)
            //    {
            //        viewModelTypeName = string.Format(CultureInfo.InvariantCulture, "AdventureWorks.UILogic.ViewModels.{0}ViewModel, AdventureWorks.UILogic.Windows, Version=1.0.0.0, Culture=neutral", viewType.Name);
            //        viewModelType = Type.GetType(viewModelTypeName);
            //    }

            //    return viewModelType;
            //});
            //var resourceLoader = Container.Resolve<IResourceLoader>();

            return base.OnInitializeAsync(args);
        }
    }
}
