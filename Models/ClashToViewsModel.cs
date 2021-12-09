using Autodesk.Revit.DB;
using AkryazTools.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Models
{
    public static class ClashToViewsModel
    {
        public static List<CategoryPickerViewModel> ListOfCategoriesA { get; set; } = new List<CategoryPickerViewModel>();
        public static List<CategoryPickerViewModel> ListOfCategoriesB { get; set; } = new List<CategoryPickerViewModel>();

        public static Document DocumentA { get; set; }
        public static Document DocumentB { get; set; }

        public static FamilySymbol AnnotationFamily { get; set; }

    }
}
