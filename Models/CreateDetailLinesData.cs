using AkryazTools.Models;
using AkryazTools.ViewModels;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Models
{
    public static class CreateDetailLinesData
    {
        public static ImportedCadFilesModel ImportedCadFile { get; set; }
        public static List<CadLayersViewModel> CadLayers { get; set; }
        public static Category SelectedLineCategory { get; set; }

    }
}
