using Prism;
using Prism.Logging;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.ViewModels
{
    public abstract class TabbedItemViewModelBase : ViewModelBase, IActiveAware
    {
        // NOTE: Prism.Forms only sets IsActive, and does not do anything with the event.
        public event EventHandler IsActiveChanged;

        #region Properties

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); }
        }
        private bool _isActive;

        protected bool HasInitialized { get; set; }

        #endregion

        #region Constructors

        protected TabbedItemViewModelBase(INavigationService navigationService, ILoggerFacade logger) : base(navigationService, logger)
        {

        }

        #endregion

        protected virtual void RaiseIsActiveChanged()
        {
            // NOTE: You must either subscribe to the event or handle the logic here.
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
