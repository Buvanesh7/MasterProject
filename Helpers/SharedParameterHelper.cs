using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Helpers
{
    public static class SharedParameterHelper
    {
        public static void CreateProjectParameters(Document doc, List<ElementId> catList)
        {
            var sharedParameterFile = doc.Application.SharedParametersFilename;
            var tempSharedParameterFile = System.IO.Path.GetTempFileName() + ".txt";
            using (System.IO.File.Create(tempSharedParameterFile)) { }
            doc.Application.SharedParametersFilename = tempSharedParameterFile;

            var parameterType = Autodesk.Revit.DB.ParameterType.Text;
            var parameterGroup = Autodesk.Revit.DB.BuiltInParameterGroup.PG_IDENTITY_DATA;


            var cats = doc.Settings.Categories;
            var catSet = new CategorySet();
            foreach (var cat in cats)
            {
                var c = cat as Category;
                if (!catList.Any(x => x == c.Id))
                    continue;
                if (!c.AllowsBoundParameters)
                    continue;
                catSet.Insert(c);
            }

            var bin = doc.Application.Create.NewInstanceBinding(catSet);

            var definitionGroup = doc.Application.OpenSharedParameterFile().Groups.Create("Worsharing User Info");

            ExternalDefinition def = null;

            foreach (var parameterName in GetListOfParameters())
            {
                def = definitionGroup.Definitions.Create(new ExternalDefinitionCreationOptions(parameterName, parameterType)) as ExternalDefinition;
                doc.ParameterBindings.Insert(def, bin, parameterGroup);
            }

            doc.Application.SharedParametersFilename = sharedParameterFile;

            System.IO.File.Delete(tempSharedParameterFile);
        }

        private static List<string> GetListOfParameters()
        {
            return new List<string>
            {
                "Element Created By",
                "Element Last Updated By",
                "Element ID"
            };
        }
    }
}
