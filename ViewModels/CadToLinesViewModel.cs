using AkryazTools.Messages;
using AkryazTools.Models;
using AkryazTools.ViewModels;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace AkryazTools.ViewModels
{
    internal class CadToLinesViewModel :ViewModelBase
    {
        private Document _document;

        public CadToLinesViewModel(Document document)
        {
            _document = document;

            CreateLinesCommand = new RelayCommand(() => SetCreateFixtureData(), CheckForDataSelection);
            CancelCommand = new RelayCommand(CloseWindow, true);

            var importInstances = new FilteredElementCollector(document).OfClass(typeof(ImportInstance)).Cast<ImportInstance>().ToList();
            foreach (var importInstance in importInstances)
            {
                var name = importInstance.get_Parameter(BuiltInParameter.IMPORT_SYMBOL_NAME).AsString();
                var cadFile = new ImportedCadFilesModel { CADFile = importInstance, Name = name.Remove(name.Length - 4) };
                ImportedCadFiles.Add(cadFile);
            }

            var lines = document.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
            var subCats = lines.SubCategories;

            foreach (var subCat in subCats)
            {               
                LineCategories.Add(subCat as Category);
            }
        }

        private void CloseWindow()
        {
            CreateDetailLinesData.CadLayers = null;
            CreateDetailLinesData.ImportedCadFile = null;
            CreateDetailLinesData.SelectedLineCategory = null;
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
            if (SelectedCadFile != null && LayerList.Where(x => x.IsChecked).ToList().Count>0 && SelectedLineCategory != null)
                return true;
            return false;
        }

        private void SetCreateFixtureData()
        {
            CreateDetailLinesData.CadLayers = LayerList.Where(x => x.IsChecked).ToList();
            CreateDetailLinesData.ImportedCadFile = SelectedCadFile;
            CreateDetailLinesData.SelectedLineCategory = SelectedLineCategory;

            Messenger.Default.Send(new CloseMessage());
        }

        private ObservableCollection<Category> _lineCategories = new ObservableCollection<Category>();

        public ObservableCollection<Category> LineCategories
        {
            get { return _lineCategories; }
            set 
            {
                _lineCategories = value;
                RaisePropertyChanged();
            }
        }

        private Category _selectedLineCategory;

        public Category SelectedLineCategory
        {
            get { return _selectedLineCategory; }
            set 
            {
                _selectedLineCategory = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CreateLinesCommand { get; set; }
        public ICommand CancelCommand { get; set; }


    }
}