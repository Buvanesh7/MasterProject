using AkryazTools.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Models
{
    public static class HideLinkedCategoriesModel
    {
        public static List<CategoryPickerViewModel> ListOfCategories { get; set; } = new List<CategoryPickerViewModel>();
        public static string SelectedViewPickingOption { get; set; }

    }
}
