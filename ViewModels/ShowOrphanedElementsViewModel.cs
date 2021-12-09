using Autodesk.Revit.DB;
using AkryazTools.Messages;
using AkryazTools.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace AkryazTools.ViewModels
{
    public class ShowOrphanedElementsViewModel : ViewModelBase
    {
        private Document _document;

        public ShowOrphanedElementsViewModel(Document document)
        {
            _document = document;

            ShowOrphanedElementsCommand = new RelayCommand(() => SetCategories(), CategoryList.Where(x => x.IsChecked).ToList().Any());

            var tempList = new List<CategoryPickerViewModel>();
            var categories = document.Settings.Categories;
            foreach (var category in categories)
            {
                var cat = category as Category;
                if (cat.CategoryType != CategoryType.Model)
                    continue;
                var catList = new CategoryPickerViewModel { Id = cat.Id, Name = cat.Name };
                tempList.Add(catList);
                //CategoryList.Add(catList);
            }

            tempList.OrderBy(x => x.Name).ToList().ForEach(x => CategoryList.Add(x)); 

            ItemsView = CollectionViewSource.GetDefaultView(CategoryList);
            ItemsView.Filter = FilterItems;
        }

        private bool FilterItems(object obj)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;
            if(!(obj is CategoryPickerViewModel catPicker))
                return false;

            return catPicker.Name.ToLower().Contains(FilterText.ToLower());
        }

        private void SetCategories()
        {
            var sortedCatList = CategoryList.Where(x => x.IsChecked).ToList();
            ShowOrphanedElementsData.ListOfCategories = sortedCatList;
            Messenger.Default.Send(new CloseMessage());
        }
        public ICollectionView ItemsView { get; }
        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                RaisePropertyChanged();
                RefreshFilter();
            }
        }

        private void RefreshFilter()
        {
            ItemsView.Refresh();
        }

        private ObservableCollection<CategoryPickerViewModel> _categoryList = new ObservableCollection<CategoryPickerViewModel>();

        public ObservableCollection<CategoryPickerViewModel> CategoryList
        {
            get { return _categoryList; }
            set 
            {
                _categoryList = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ShowOrphanedElementsCommand { get; set; }

    }
}
