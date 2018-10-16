using Prism.Logging;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class HelpPageViewModel : ViewModelBase
    {
        #region Constructors & Initialize

        public HelpPageViewModel(INavigationService navigationService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            Initialize();
        }

        private void Initialize()
        {
            Title = "Help";
        }

        #endregion
    }
}
