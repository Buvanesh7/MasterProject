using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using AkryazTools.Helpers;
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
    public class RoomToSolidViewModel : ViewModelBase
    {
        private Document _document;

        public RoomToSolidViewModel(Document document)
        {
            _document = document;

            RoomToViewsData.RoomIds.Clear();

            CreateViewCommand = new RelayCommand(() => CreateSolids(), RoomList.Any(x => x.IsChecked));

            FilterRuleList.Add("Equal To");
            FilterRuleList.Add("Not Equal To");
            FilterRuleList.Add("Contains");
            FilterRuleList.Add("Does Not Contains");
            FilterRuleList.Add("Begins With");
            FilterRuleList.Add("Does Not Begins With");
            FilterRuleList.Add("Ends With");
            FilterRuleList.Add("Does Not Ends With");

            var tempDocList = new List<Document>();
            tempDocList.Add(document);
            var revitLinks = new FilteredElementCollector(document).OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();
            foreach (var revitLink in revitLinks)
            {
                var linkDocument = revitLink.GetLinkDocument();
                tempDocList.Add(linkDocument);
            }
            tempDocList.OrderBy(x => x.Title).ToList().ForEach(x => DocumentList.Add(x));

            ItemsView = CollectionViewSource.GetDefaultView(RoomList);
            ItemsView.Filter = FilterItems;
        }

        private void CreateSolids()
        {
            foreach (var room in RoomList)
            {
                if (!room.IsChecked)
                    continue;
                RoomToViewsData.RoomNames.Add(room.Name);
                RoomToViewsData.RoomIds.Add(room.Id);
                RoomToViewsData.RoomElements.Add(room.RoomElement);
            }
            Messenger.Default.Send(new CloseMessage());
        }

        private bool FilterItems(object obj)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;
            if (!(obj is RoomsDataViewModel roomData))
                return false;

            return FilterHelper.FilterItems(SelectedFilterRule, roomData.Name, FilterText);
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

        private string _offset;
        public string Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                RaisePropertyChanged();
            }
        }

        private void RefreshRoomList()
        {
            RoomList.Clear();
            var rooms = new FilteredElementCollector(SelectedDocument).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().OfType<Room>().OrderBy(x => x.Name).ToList();
            rooms.ForEach(x => RoomList.Add(new RoomsDataViewModel { Id = x.Id, Name = x.Name, IsChecked = false, RoomElement = x }));

        }

        private string _selectedFilterRule;
        public string SelectedFilterRule
        {
            get { return _selectedFilterRule; }
            set
            {
                _selectedFilterRule = value;
                RefreshFilter();
                RaisePropertyChanged();
            }
        }

        private Document _selectedDocument;
        public Document SelectedDocument
        {
            get { return _selectedDocument; }
            set
            {
                _selectedDocument = value;
                RefreshRoomList();
                RaisePropertyChanged();
            }
        }

        private bool _selcetAll;
        public bool SelectAll
        {
            get { return _selcetAll; }
            set 
            {
                _selcetAll = value;
                UpdateSelection();
                RaisePropertyChanged();
            }
        }

        private void UpdateSelection()
        {
            if (RoomList.Count == 0)
                return;
            if (SelectAll)
            {
                foreach (var room in RoomList)
                {
                    room.IsChecked = true;
                }
            }
            else
            {
                foreach (var room in RoomList)
                {
                    room.IsChecked = false;
                }
            }
        }

        private ObservableCollection<RoomsDataViewModel> _roomList = new ObservableCollection<RoomsDataViewModel>();
        public ObservableCollection<RoomsDataViewModel> RoomList
        {
            get { return _roomList; }
            set
            {
                _roomList = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Document> _documentList = new ObservableCollection<Document>();

        public ObservableCollection<Document> DocumentList
        {
            get { return _documentList; }
            set
            {
                _documentList = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> _filterRuleList = new ObservableCollection<string>();

        public ObservableCollection<string> FilterRuleList
        {
            get { return _filterRuleList; }
            set
            {
                _filterRuleList = value;
                RaisePropertyChanged();
            }
        }
        public ICommand CreateViewCommand { get; set; }
    }
}
