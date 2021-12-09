using AkryazTools.Helpers;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AkryazTools.Views;
using AkryazTools.Models;
using AkryazTools.ViewModels;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class ShowOrphanedElementsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uidoc = commandData.Application.ActiveUIDocument;
                var document = uidoc.Document;

                var mainWindow = new ShowOrphanedElementsWindow(document);
                mainWindow.ShowDialog();

                var a = mainWindow.DialogResult;
                if (ShowOrphanedElementsData.ListOfCategories.Count == 0)
                    return Result.Cancelled;

                using var transaction = new Transaction(document, "Show Orphaned Elements");
                transaction.Start();

                var catList = new List<ElementId>();

                ShowOrphanedElementsData.ListOfCategories.ForEach(x => catList.Add(x.Id));
                //var catList = new List<BuiltInCategory> { BuiltInCategory.OST_LightingFixtures, BuiltInCategory.OST_ElectricalEquipment, BuiltInCategory.OST_ElectricalFixtures };
                var catFilter = new ElementMulticategoryFilter(catList);

                var allElements = new FilteredElementCollector(document).WherePasses(catFilter).WhereElementIsNotElementType().ToList();

                var ophanedElements = new List<ElementId>();
                foreach (var element in allElements)
                {
                    var type = document.GetElement(element.GetTypeId()) as FamilySymbol;
                    if (type == null)
                        continue;

                    var family = type.Family.FamilyPlacementType;

                    var fi = element as FamilyInstance;
                    var hostParam = fi.get_Parameter(BuiltInParameter.INSTANCE_FREE_HOST_PARAM);
                    var hostvalue = "123";
                    if (hostParam != null)
                        hostvalue = hostParam.AsString();

                    switch (family)
                    {
                        case FamilyPlacementType.OneLevelBased:
                            continue;
                        case FamilyPlacementType.OneLevelBasedHosted:
                            var host = fi.Host;
                            if (host == null)                          
                                ophanedElements.Add(fi.Id);
                            else
                            {
                                if (hostvalue == "123")
                                    continue;
                                if (hostvalue == "Orphaned")
                                    ophanedElements.Add(fi.Id);
                            }
                            break;
                        case FamilyPlacementType.TwoLevelsBased:
                            continue;
                        case FamilyPlacementType.ViewBased:
                            continue;
                        case FamilyPlacementType.WorkPlaneBased:
                            host = fi.Host;
                            if (host == null)
                                ophanedElements.Add(fi.Id);
                            else
                            {
                                if (hostvalue == "123")
                                    continue;
                                if (hostvalue == "Orphaned")
                                    ophanedElements.Add(fi.Id);
                            }
                            break;
                        case FamilyPlacementType.CurveBased:
                            continue;
                        case FamilyPlacementType.CurveBasedDetail:
                            continue;
                        case FamilyPlacementType.CurveDrivenStructural:
                            continue;
                        case FamilyPlacementType.Adaptive:
                            continue;
                        case FamilyPlacementType.Invalid:
                            continue;
                        default:
                            break;
                    }

                }

                if(ophanedElements.Count == 0)
                {
                    DisplayHelper.Display("No orphaned elements in the selected categories!", Types.TaskDialogType.Information);
                    transaction.RollBack();
                    return Result.Succeeded;
                }
                var date = DateTime.Now.ToString();

                var view3Dtype = new FilteredElementCollector(document).OfClass(typeof(View3D)).Where(x => x.GetTypeId().IntegerValue != -1).First();
                var tpId = view3Dtype.GetTypeId();
                var temp3Dview = View3D.CreateIsometric(document, tpId);
                temp3Dview.IsolateElementsTemporary(ophanedElements);

                var newViewName = "Orphaned Elements_" + date;
                var correctedViewName = newViewName
                    .Replace(':', '_')
                    .Replace('*', '_')
                    .Replace('?', '_')
                    .Replace('/', '_')
                    .Replace('\\', '_')
                    .Replace('[', '_')
                    .Replace(']', '_');

                temp3Dview.Name = correctedViewName;

                transaction.Commit();

                uidoc.ActiveView = temp3Dview as View;

                
                ShowOrphanedElementsData.ListOfCategories = new List<CategoryPickerViewModel>();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DisplayHelper.Display(ex.Message, AkryazTools.Types.TaskDialogType.Error);
                return Result.Failed;
            }
        }
    }
}
