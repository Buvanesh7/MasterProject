using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Helpers
{
	public class ConstantMembers
	{
		public static string PluginName { get; set; }
		public static UIDocument ActiveUIDocument { get; set; }

		public static void Initialize(string pluginName, UIDocument uiDoc)
		{
			PluginName = pluginName;
			ActiveUIDocument = uiDoc;
		}
	}
}
