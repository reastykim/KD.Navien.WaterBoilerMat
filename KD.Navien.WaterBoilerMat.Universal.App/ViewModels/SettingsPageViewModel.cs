using KD.Navien.WaterBoilerMat.Universal.Services;
using Prism.Logging;
using Prism.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace KD.Navien.WaterBoilerMat.Universal.App.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        #region Properties

        public string VersionDescriptions
        {
            get => _versionDescriptions;
            private set => SetProperty(ref _versionDescriptions, value);
        }
        private string _versionDescriptions;

        #endregion

        #region Fields

        private readonly IAlertMessageService _alertMessageService;

        #endregion

        #region Constructors & Initialize

        public SettingsPageViewModel(INavigationService navigationService, IAlertMessageService alertMessageService, ILoggerFacade logger)
            : base(navigationService, logger)
        {
            _alertMessageService = alertMessageService;

            Initialize();
        }

        private void Initialize()
        {
            Title = "Settings";
            VersionDescriptions = GetVersionDescription();
        }

        #endregion

        private string GetVersionDescription()
        {
            var appName = "NAVIEN MATE";
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
