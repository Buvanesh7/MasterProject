using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ViewModels
{
    public class RoomsDataViewModel : ViewModelBase
    {
        private ElementId _id;

        public ElementId Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }

        private Element _roomElement;

        public Element RoomElement
        {
            get { return _roomElement; }
            set 
            {
                _roomElement = value;
                RaisePropertyChanged();
            }
        }

    }
}
