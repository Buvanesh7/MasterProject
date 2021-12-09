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
    public class CreateFixturesViewModel : ViewModelBase
    {
        private Document _document;

        public CreateFixturesViewModel(Document document)
        {
            _document = document;

            CreateFixturesCommand = new RelayCommand(() => SetCreateFixtureData(), CheckForDataSelection);
            CancelCommand = new RelayCommand(CloseWindow, true);

            var importInstances = new FilteredElementCollector(document).OfClass(typeof(ImportInstance)).Cast<ImportInstance>().ToList();
            foreach (var importInstance in importInstances)
            {
                var name = importInstance.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME).AsString();
                var cadFile = new ImportedCadFilesModel { CADFile = importInstance, Name = name.Remove(name.Length - 4) };
                ImportedCadFiles.Add(cadFile);
            }

            using var collector = new FilteredElementCollector(document);
            var multiCategoryList = new List<BuiltInCategory> { BuiltInCategory.OST_CommunicationDevices, BuiltInCategory.OST_FireAlarmDevices, BuiltInCategory.OST_LightingDevices, BuiltInCategory.OST_LightingFixtures };
            var catFilter = new ElementMulticategoryFilter(multiCategoryList);
            var getFamilySymbols = collector.WherePasses(catFilter).GetElementIterator();
            while (getFamilySymbols.MoveNext())
            {
                var famSym = getFamilySymbols.Current as FamilySymbol;
                if (famSym == null)
                    continue;
                FamilySymbols.Add(famSym);
            }
            ItemsView = CollectionViewSource.GetDefaultView(FamilySymbols);
            var groupDescription = new PropertyGroupDescription(nameof(FamilySymbol.FamilyName));
            ItemsView.GroupDescriptions.Add(groupDescription);
        }

        private void CloseWindow()
        {
            CreateFixtureData.CadLayers = null;
            CreateFixtureData.ImportedCadFile = null;
            CreateFixtureData.SelectedFamilySymbol = null;
            Messenger.Default.Send(new CloseMessage());
        }

        private ObservableCollection<ImportedCadFilesModel> _importedCadFiles = new ObservableCollection<ImportedCadFilesModel>();
        public ObservableCollection<ImportedCadFilesModel> ImportedCadFiles
        {
            get { return _importedCadFiles; }
            set
            {
                _importedCadFiles = value;
                RaisePropertyChanged();
            }
        }

        private ImportedCadFilesModel _selectedCadFile;
        public ImportedCadFilesModel SelectedCadFile
        {
            get { return _selectedCadFile; }
            set
            {
                _selectedCadFile = value;
                SetNewLayerList(_document);
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<CadLayersViewModel> _layerList = new ObservableCollection<CadLayersViewModel>();
        public ObservableCollection<CadLayersViewModel> LayerList
        {
            get { return _layerList; }
            set
            {
                _layerList = value;
                RaisePropertyChanged();
            }
        }

        private void SetNewLayerList(Document document)
        {
            var layerNames = new List<string>();
            var cadModels = new List<CadLayersViewModel>();
            var geos = SelectedCadFile.CADFile.get_Geometry(new Options());
            foreach (var geo in geos)
            {
                var geoInstance = ((GeometryInstance)geo).GetInstanceGeometry();
                if (geoInstance == null)
                    continue;
                foreach (var gInst in geoInstance)
                {
                    var gStyle = document.GetElement(gInst.GraphicsStyleId) as GraphicsStyle;
                    if (gStyle == null)
                        continue;
                    var layerName = gStyle.GraphicsStyleCategory.Name;
                    if (!layerNames.Contains(layerName))
                    {
                        layerNames.Add(layerName);
                        var cadlayerModel = new CadLayersViewModel { LayerName = layerName };
                        cadModels.Add(cadlayerModel);
                    }
                }

            }
            var tModel = cadModels.OrderBy(x => x.LayerName).ToList();
            tModel.ForEach(x => LayerList.Add(x));
        }

        private bool CheckForDataSelection()
        {
            if (SelectedCadFile != null && LayerList.Where(x => x.IsChecked).ToList().Count > 0 && SelectedFixtureType != null)
                return true;
            return false;
        }

        private void SetCreateFixtureData()
        {
            CreateFixtureData.CadLayers = LayerList.Where(x => x.IsChecked).ToList();
            CreateFixtureData.ImportedCadFile = SelectedCadFile;
            CreateFixtureData.SelectedFamilySymbol = SelectedFixtureType;

            Messenger.Default.Send(new CloseMessage());
        }

        private ObservableCollection<FamilySymbol> _familySymbols = new ObservableCollection<FamilySymbol>();

        public ObservableCollection<FamilySymbol> FamilySymbols
        {
            get { return _familySymbols; }
            set
            {
                _familySymbols = value;
                RaisePropertyChanged();
            }
        }

        private FamilySymbol _selectedFixtureType;

        public FamilySymbol SelectedFixtureType
        {
            get { return _selectedFixtureType; }
            set
            {
                _selectedFixtureType = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CreateFixturesCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ICollectionView ItemsView { get; set; }
    }
}
