using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Models
{
    public static class RoomToViewsData
    {
        public static List<Element> RoomElements { get; set; } = new List<Element>();
        //public static Transform LinkTransform { get; set; } = Transform.Identity;
        public static string Offset { get; set; }
        public static List<string> RoomNames { get; set; } = new List<string>();
        public static List<ElementId> RoomIds { get; set; } = new List<ElementId>();

    }
}
