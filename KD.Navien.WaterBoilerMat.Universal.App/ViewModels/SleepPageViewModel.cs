using KD.Navien.WaterBoilerMat.Universal.Services;
using Prism.Logging;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class SleepPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly IAlertMessageService _alertMessageService;

        #endregion

        #region Constructors & Initialize

        public SleepPageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _alertMessageService = alertMessageService;

            Initialize();
        }

        private void Initialize()
        {
            Title = "Sleep";
        }

        #endregion
    }
}
