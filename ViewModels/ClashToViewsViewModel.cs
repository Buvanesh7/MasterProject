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
    public class ClashToViewsViewModel : ViewModelBase
    {
        private Document _document;

        public ClashToViewsViewModel(Document document)
        {
            _document = document;
            CreateClashViewsCommand = new RelayCommand(() => CreateClashViews(), CheckForData);// 

            var tempDocList = new List<Document>();
            tempDocList.Add(document);

            var revitLinks = new FilteredElementCollector(document).OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();

            foreach (var revitLink in revitLinks)
            {
                var linkDocument = revitLink.GetLinkDocument();
                tempDocList.Add(linkDocument);
            }

            tempDocList.OrderBy(x => x.Title).ToList().ForEach(x => DocumentListA.Add(x));
            tempDocList.OrderBy(x => x.Title).ToList().ForEach(x => DocumentListB.Add(x));

            var tempListA = new List<CategoryPickerViewModel>();
            var tempListB = new List<CategoryPickerViewModel>();
            var categories = document.Settings.Categories;
            foreach (var category in categories)
            {
                var cat = category as Category;
                var catList1 = new CategoryPickerViewModel { Id = cat.Id, Name = cat.Name };
                var catList2 = new CategoryPickerViewModel { Id = cat.Id, Name = cat.Name };

                tempListA.Add(catList1);
                tempListB.Add(catList2);
                //CategoryList.Add(catList);
            }
            CategoryListA.Clear();
            CategoryListB.Clear();
            tempListA.OrderBy(x => x.Name).ToList().ForEach(x => CategoryListA.Add(x));
            tempListB.OrderBy(x => x.Name).ToList().ForEach(x => CategoryListB.Add(x));

            ItemsView1 = CollectionViewSource.GetDefaultView(CategoryListA); //new CollectionViewSource { Source = CategoryListA }.View;// 
            ItemsView1.Filter = FilterItemsA;

            ItemsView2 = CollectionViewSource.GetDefaultView(CategoryListB); //new CollectionViewSource { Source = CategoryListB }.View;//
            ItemsView2.Filter = FilterItemsB;
        }

        private bool CheckForData()
        {
            var test1 = CategoryListA.Where(x => x.IsChecked).ToList().Any() && CategoryListB.Where(x => x.IsChecked).ToList().Any();
            var test2 = SelectedDocumentA != null && SelectedDocumentB != null;
            if (test1 && test2)
                return true;
            return false;
        }

        private bool FilterItemsA(object obj)
        {
            if (string.IsNullOrEmpty(FilterTextA))
                return true;
            if (!(obj is CategoryPickerViewModel catPicker))
                return false;

            return catPicker.Name.ToLower().Contains(FilterTextA.ToLower());
        }

        private bool FilterItemsB(object obj)
        {
            if (string.IsNullOrEmpty(FilterTextA))
                return true;
            if (!(obj is CategoryPickerViewModel catPicker))
                return false;

            return catPicker.Name.ToLower().Contains(FilterTextB.ToLower());
        }

        private void CreateClashViews()
        {
            var sortedCatListA = CategoryListA.Where(x => x.IsChecked).ToList();
            var sortedCatListB = CategoryListB.Where(x => x.IsChecked).ToList();
            ClashToViewsModel.ListOfCategoriesA = sortedCatListA;
            ClashToViewsModel.ListOfCategoriesB = sortedCatListB;
            ClashToViewsModel.DocumentA = SelectedDocumentA;
            ClashToViewsModel.DocumentB = SelectedDocumentB;
            Messenger.Default.Send(new CloseMessage());
        }

        public ICollectionView ItemsView1 { get; }
        public ICollectionView ItemsView2 { get; }

        private string _filterTextA;
        public string FilterTextA
        {
            get => _filterTextA;
            set
            {
                _filterTextA = value;
                RaisePropertyChanged();
                RefreshFilterA();
            }
        }

        private string _filterTextB;
        public string FilterTextB
        {
            get => _filterTextB;
            set
            {
                _filterTextB = value;
                RaisePropertyChanged();
                RefreshFilterB();
            }
        }

        private void RefreshFilterA()
        {
            ItemsView1.Refresh();
        }

        private void RefreshFilterB()
        {
            ItemsView2.Refresh();
        }

        //public ObservableCollection<CategoryPickerViewModel> CategoryListA { get; } = new ObservableCollection<CategoryPickerViewModel>();
        //public ObservableCollection<CategoryPickerViewModel> CategoryListB { get; } = new ObservableCollection<CategoryPickerViewModel>();

        private ObservableCollection<CategoryPickerViewModel> _categoryListA = new ObservableCollection<CategoryPickerViewModel>();

        public ObservableCollection<CategoryPickerViewModel> CategoryListA
        {
            get { return _categoryListA; }
            set
            {
                _categoryListA = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<CategoryPickerViewModel> _categoryListB = new ObservableCollection<CategoryPickerViewModel>();

        public ObservableCollection<CategoryPickerViewModel> CategoryListB
        {
            get { return _categoryListB; }
            set
            {
                _categoryListB = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Document> _documentListA = new ObservableCollection<Document>();

        public ObservableCollection<Document> DocumentListA
        {
            get { return _documentListA; }
            set 
            {
                _documentListA = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Document> _documentListB = new ObservableCollection<Document>();

        public ObservableCollection<Document> DocumentListB
        {
            get { return _documentListB; }
            set
            {
                _documentListB = value;
                RaisePropertyChanged();
            }
        }

        private Document _selectedDocumentA;

        public Document SelectedDocumentA
        {
            get { return _selectedDocumentA; }
            set 
            {
                _selectedDocumentA = value;
                RaisePropertyChanged();
            }
        }

        private Document _selectedDocumentB;

        public Document SelectedDocumentB
        {
            get { return _selectedDocumentB; }
            set
            {
                _selectedDocumentB = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CreateClashViewsCommand { get; set; }
    }
}
