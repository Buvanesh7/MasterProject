using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ViewModels
{
    public class CadLayersViewModel : ViewModelBase
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }

        private string _layerName;
        public string LayerName
        {
            get { return _layerName; }
            set
            {
                _layerName = value;
                RaisePropertyChanged();
            }
        }


    }
}
