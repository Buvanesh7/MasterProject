using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Wrappers
{
    public class SetElementColorWrapper
    {
        public SetElementColorWrapper(OverrideGraphicSettings ovrSettings, Color color, ElementId id)
        {
            ovrSettings.SetSurfaceForegroundPatternColor(color);
            ovrSettings.SetSurfaceForegroundPatternId(id);
        }

        public SetElementColorWrapper(OverrideGraphicSettings ovrSettings, Color color, ElementId id, bool older)
        {
            //ovrSettings.SetProjectionFillColor(color);
            //ovrSettings.SetProjectionFillPatternId(id);
        }
    }
}
