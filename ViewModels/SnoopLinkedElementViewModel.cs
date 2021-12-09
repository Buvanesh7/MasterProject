using AkryazTools.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ViewModels
{
    public class SnoopLinkedElementViewModel : ViewModelBase
    {
        public SnoopLinkedElementViewModel(SnoopLinkedElementModel infoData)
        {
            Id = infoData.Id;
            Category = infoData.Category;
            Level = infoData.Level;
            ModelName = infoData.ModelName;
            ModelPath = infoData.ModelPath;
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set 
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                RaisePropertyChanged();
            }
        }

        private string _level;
        public string Level
        {
            get { return _level; }
            set
            {
                _level = value;
                RaisePropertyChanged();
            }
        }

        private string _modelName;
        public string ModelName
        {
            get { return _modelName; }
            set
            {
                _modelName = value;
                RaisePropertyChanged();
            }
        }

        private string _modelPath;
        public string ModelPath
        {
            get { return _modelPath; }
            set
            {
                _modelPath = value;
                RaisePropertyChanged();
            }
        }

    }
}
