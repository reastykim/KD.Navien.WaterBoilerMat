using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
    public class NavigationViewItemData : BindableBase
    {
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _name;

        public string TextIcon
        {
            get => _textIcon;
            set => SetProperty(ref _textIcon, value);
        }
        private string _textIcon;

        public Type TargetPageType
        {
            get => _targetPageType;
            set => SetProperty(ref _targetPageType, value);
        }
        private Type _targetPageType;
    }
}
