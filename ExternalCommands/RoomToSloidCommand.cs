using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
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
    public class RoomToSloidCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                using var transaction = new Transaction(document, "Name");
                transaction.Start();

                var mainWindow = new RoomToSolidWindow(document);
                mainWindow.ShowDialog();

                if (RoomToViewsData.RoomElements.Count == 0)
                {
                    transaction.RollBack();
                    return Result.Cancelled;
                }

                //var rooms = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().GetEnumerator();

                foreach (var roomData in RoomToViewsData.RoomElements)
                //while (rooms.MoveNext())
                {
                    var room = roomData as Room;
                    if (room == null)
                        continue;

                    var solid = room.ClosedShell.Select(x => x as Solid).Where(x => x != null).FirstOrDefault();

                    if (solid == null)
                        continue;

                    try
                    {
                        var ds = DirectShape.CreateElement(document, new ElementId(BuiltInCategory.OST_Mass));
                        ds.SetShape(new List<GeometryObject> { solid });
                    }
                    catch 
                    {  }
                    
                }

                transaction.Commit();

                RoomToViewsData.RoomElements.Clear();

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
