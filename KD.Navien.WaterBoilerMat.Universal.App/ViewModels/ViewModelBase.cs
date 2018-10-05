﻿using Prism;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public abstract class ViewModelBase : Prism.Windows.Mvvm.ViewModelBase
    {
        protected INavigationService NavigationService { get; private set; }
		protected ILoggerFacade Logger { get; private set; }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _title;

        protected ViewModelBase(INavigationService navigationService, ILoggerFacade logger)
        {
            NavigationService = navigationService;
			Logger = logger;
		}
    }
}
