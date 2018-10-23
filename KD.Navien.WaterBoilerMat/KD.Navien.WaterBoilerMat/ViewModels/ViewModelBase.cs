using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.ViewModels
{
    public abstract class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
		protected ILoggerFacade Logger { get; private set; }

		private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, ILoggerFacade logger)
        {
            NavigationService = navigationService;
			Logger = logger;
		}

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
            
        }

        public virtual void Destroy()
        {
            
        }
    }
}
