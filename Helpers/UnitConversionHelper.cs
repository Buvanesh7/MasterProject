using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Helpers
{
    public static class UnitConversionHelper
    {
        public static double ConvertUnits(Document document, Parameter parameter, double value)
        {
            var unitType = parameter.Definition.UnitType;
            var dutDisplay = document.GetUnits().GetFormatOptions(unitType).DisplayUnits;
            var parameterValue = UnitUtils.ConvertFromInternalUnits(value, dutDisplay);
            return parameterValue;
        }

        public static string GetUnitName(Document document, Parameter parameter, double value)
        {
            var unitType = parameter.Definition.UnitType;
            var dutDisplay = document.GetUnits().GetFormatOptions(unitType).DisplayUnits;
            //var valid_unit_symbols = FormatOptions.GetValidUnitSymbols(dutDisplay).Select<UnitSymbolType, string>(u => Util.UnitSymbolTypeString(u));
            var result = UnitFormatUtils.Format(document.GetUnits(), unitType, value, false, false);
            return result;
            //return document.GetUnits().GetFormatOptions(unitType).DisplayUnits.ToString();
        }
    }
}
