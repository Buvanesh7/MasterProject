using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AkryazTools.Helpers
{
	public static class ParameterHelper
	{
		public static Parameter GetParameter(Element element, int parameterId)
		{
			Parameter parameter = element.get_Parameter((BuiltInParameter)parameterId);
			return parameter;
		}

		public static Parameter GetParameter(Element element, string parameterName)
		{
			Parameter parameter = element.LookupParameter(parameterName);
			return parameter;
		}

		public static object GetParameterValue(Element element, int parameterId)
		{
			Parameter parameter = element.get_Parameter((BuiltInParameter)parameterId);
			return parameter == null ? "" : GetParameterByStorage(element.Document, parameter);
		}

		public static object GetParameterValue(Element element, string parameterName)
		{
			Parameter parameter = element.LookupParameter(parameterName);
			return parameter == null ? "" : GetParameterByStorage(element.Document, parameter);
		}

		private static object GetParameterByStorage(Document document, Parameter para)
		{
			object parameterValue = null;
			// Use different method to get parameter data according to the storage type
			switch (para.StorageType)
			{
				case StorageType.Double:
					//covert the number into Metric
					double result = 0;
					try
					{
						double asDouble = para.AsDouble();
						if (asDouble != 0 && para.Definition.UnitType != UnitType.UT_Number)
						{
							asDouble = UnitUtils.ConvertFromInternalUnits(asDouble, para.DisplayUnitType);
						}

						result = Math.Round(asDouble, 8);
					}
					catch (Autodesk.Revit.Exceptions.InvalidOperationException)
					{
					}

					parameterValue = result;// para.AsValueString();
					break;
				case StorageType.ElementId:
					//find out the name of the element
					Autodesk.Revit.DB.ElementId id = para.AsElementId();
					parameterValue = id.IntegerValue >= 0 ? document.GetElement(id).Name : id.IntegerValue.ToString();
					break;
				case StorageType.Integer:
					if (ParameterType.YesNo == para.Definition.ParameterType)
					{
						parameterValue = para.AsInteger() == 0 ? "False" : "True";
					}
					else
					{
						parameterValue = para.AsInteger();
					}
					break;
				case StorageType.String:
					parameterValue = para.AsString();
					break;
				default:
					parameterValue = "";
					break;
			}

			return parameterValue;
		}

		public static void SetParameterValue(Element element, int parameterId, object paramValue)
		{
			Parameter parameter = element.get_Parameter((BuiltInParameter)parameterId);
			if (parameter != null)
			{
				SetParameter(parameter, paramValue.ToString(), element.Document);
			}
		}

		public static void SetParameterValue(Element element, string parameterName, object paramValue)
		{
			Parameter parameter = element.LookupParameter(parameterName);
			if (parameter != null)
			{
				SetParameter(parameter, paramValue.ToString(), element.Document);
			}
		}

		private static void SetParameter(Parameter prm, string value, Document document)
		{
			if (prm.StorageType == StorageType.Double)
			{
				double d;
				double.TryParse(value, out d);
				if (d == 0)
				{
					d = GetDoubleValue(value);
				}
				SetParameterValue(prm, d);
			}
			else if (prm.StorageType == StorageType.Integer)
			{
				int d;
				int.TryParse(value, out d);

				if (d == 0)
				{
					double ddd = Math.Round(GetDoubleValue(value), 0);
					int.TryParse(ddd.ToString(), out d);
				}
				if (prm.Definition.ParameterType == ParameterType.YesNo)
				{
					prm.Set(value.Trim().ToLower() == "no" ? 0 : 1);
				}
				else
				{
					prm.Set(d);
				}
			}
			else if (prm.StorageType == StorageType.String)
			{
				SetParameterValue(prm, value);
			}
		}

		private static double GetDoubleValue(string value)
		{
			double dvalue = 0;
			string re1 = "(\\d+)";  // Integer Number 1
			string re2 = "(\\.)";   // Any Single Character 1
			string re3 = "(\\d+)";  // Integer Number 2

			Regex r = new Regex(re1 + re2 + re3, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Match m = r.Match(value);
			if (m.Success)
			{
				String int1 = m.Groups[1].ToString();
				String c1 = m.Groups[2].ToString();
				String int2 = m.Groups[3].ToString();
				dvalue = Convert.ToDouble(int1 + c1 + int2);
			}

			if (dvalue == 0)
			{
				string val = Regex.Match(value, @"\d+").Value;

				double.TryParse(val, out dvalue);
			}

			return dvalue;
		}

		private static void SetParameterValue(Parameter prm, string value)
		{
			if (prm != null && prm.IsReadOnly != true)
				prm.Set(value);
		}

		private static void SetParameterValue(Parameter prm, double value)
		{
			value = UnitUtils.ConvertToInternalUnits(value, prm.DisplayUnitType);
			prm.Set(value);
		}

		private static void SetParameterValue(Parameter prm, int value)
		{
			double dValue = UnitUtils.ConvertToInternalUnits(value, prm.DisplayUnitType);
			prm.Set(dValue);
		}
	}
}
