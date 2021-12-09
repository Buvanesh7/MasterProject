using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AkryazTools.Helpers;
using AkryazTools.Models;
using AkryazTools.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class SnoopLinkedElementCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            var uiDoc = commandData.Application.ActiveUIDocument;
            try
            {
                using var transaction = new Transaction(document, "Name");
                transaction.Start();

                Reference refElemLinked = uiDoc.Selection.PickObject(ObjectType.LinkedElement,"Please pick an element in the linked model");
                var rvtLinkInstance = document.GetElement(refElemLinked.ElementId) as RevitLinkInstance;
                var linkedDoc = rvtLinkInstance.GetLinkDocument();

                Element elem = linkedDoc.GetElement(refElemLinked.LinkedElementId);

                //var id      = "ID         - " + elem.Id.ToString();
                //var cat     = "Category   - " + elem.Category.Name;
                //var level   = "Level      - " + linkedDoc.GetElement(elem.LevelId).Name;
                //var docName = "Model Name - " + linkedDoc.Title;
                //var path    = "Path       - " + linkedDoc.PathName;

                var id = elem.Id.ToString();
                var cat = elem.Category.Name;
                var levelId = elem.LevelId;
                var level =  "";
                if (levelId == null || levelId.IntegerValue == -1)
                {
                    var levelParam = elem.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                    if (levelParam == null)
                        level = "Not Found";
                    else
                        level = levelParam.AsValueString();
                }              
                else
                    level = linkedDoc.GetElement(levelId).Name;
                var docName = linkedDoc.Title;
                var path = linkedDoc.PathName;

                var infoData = new SnoopLinkedElementModel 
                {
                    Id = id,
                    Category = cat,
                    Level = level,
                    ModelName = docName,
                    ModelPath = path
                };

                transaction.Commit();


                var mainWindow = new SnoopLinkedElementWindow(infoData);
                mainWindow.ShowDialog();
                //var elementInfo = new StringBuilder();
                //elementInfo.AppendLine(id);
                //elementInfo.AppendLine(cat);
                //elementInfo.AppendLine(level);
                //elementInfo.AppendLine(docName);
                //elementInfo.AppendLine(path);

                //var td = new TaskDialog("Element Info");
                //td.MainContent = elementInfo.ToString();
                //td.Title = "Element Info";
                //td.MainIcon = TaskDialogIcon.TaskDialogIconInformation;
                //td.Show();
                ////TaskDialog.Show("Element Info", elementInfo.ToString());


                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
                return Result.Failed;
            }
        }
    }
}
