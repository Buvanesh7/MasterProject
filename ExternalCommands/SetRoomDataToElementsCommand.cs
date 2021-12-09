using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using AkryazTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class SetRoomDataToElementsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                using var transaction = new Transaction(document, "SetRoomDataToElementsCommand");
                transaction.Start();

                var rooms = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().OfType<Room>().ToList();

                var catList = new List<BuiltInCategory> { BuiltInCategory.OST_Rooms, BuiltInCategory.OST_Views, BuiltInCategory.OST_Viewers };
                var exclFilt = new ElementMulticategoryFilter(catList, true);

                var updatedElements = new List<ElementId>();

                var eleCheck = new List<ElementId>();

                foreach (var room in rooms)
                {
                    var shell = room.ClosedShell;
                    var solid = shell.Select(x => x as Solid).FirstOrDefault();
                    if (solid == null)
                        continue;

                    var roomName = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                    var roomNumber = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();

                    var bb = room.get_BoundingBox(null);
                    if (bb == null)
                        continue;
                    var ot = new Outline(bb.Min, bb.Max);
                    var bbFilt = new BoundingBoxIntersectsFilter(ot);

                    var solidFilter = new ElementIntersectsSolidFilter(solid);

                    var clashes = new FilteredElementCollector(document).WherePasses(exclFilt).WhereElementIsNotElementType().WherePasses(bbFilt).WherePasses(solidFilter).ToElements();

                    foreach (var ele in clashes)
                    {
                        eleCheck.Add(ele.Id);
                        var p1 = ele.LookupParameter("Room Name");
                        if (p1 == null)
                            continue;
                        var p2 = ele.LookupParameter("Room Number");
                        if (p2 == null)
                            continue;
                        p1.Set(roomName);
                        p2.Set(roomNumber);
                        updatedElements.Add(ele.Id);
                    }

                }
                if (eleCheck.Count > 0 && updatedElements.Count ==  0)
                    DisplayHelper.Display("Create 'Room Name' and 'Room Number' parameters and try agian!", Types.TaskDialogType.Information);
                else if (eleCheck.Count == 0)
                    DisplayHelper.Display("No Elements Found!", Types.TaskDialogType.Information);
                else
                    DisplayHelper.Display(updatedElements.Count.ToString() + " Elements Updated!", Types.TaskDialogType.Information);
                
                transaction.Commit();

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
