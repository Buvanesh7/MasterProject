using Autodesk.Revit.DB;
using AkryazTools.Enum;
using AkryazTools.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Models
{
    public static class PipeUpDownData
    {
        public static double OffsetValue { get; set; }
        public static double AngleValue { get; set; }
        public static MepReRouterEnum Direction { get; set; }
    }
}
