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
    public class RoomToViewsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            var app = commandData.Application.Application;
            var uidoc = commandData.Application.ActiveUIDocument;
            try
            {
                var maniWindow = new RoomsToViewsWindow(document);
                maniWindow.ShowDialog();

                if(RoomToViewsData.RoomElements.Count == 0)
                {
                    return Result.Cancelled;
                }

                var offset = 0.0;

                if(!string.IsNullOrEmpty(RoomToViewsData.Offset))
                    offset = double.Parse(RoomToViewsData.Offset)/304.8;

                using var transaction = new Transaction(document, "Name");
                transaction.Start();

                var vv = new FilteredElementCollector(document).OfClass(typeof(ViewFamilyType)).OfType<ViewFamilyType>().Where(x => x.ViewFamily == ViewFamily.ThreeDimensional).FirstOrDefault();                var view3dType = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Views).Select(x => document.GetElement(x.GetTypeId())).ToList();//.OfType<ViewFamilyType>().ToList();//.Where(x => x.ViewFamily == ViewFamily.ThreeDimensional).FirstOrDefault();
           
                var all3DViews = new FilteredElementCollector(document).OfClass(typeof(View3D)).ToList();

                var viewsToDelete = new List<ElementId>();

                var viewsList = new List<View3D>();

                foreach (var room in RoomToViewsData.RoomElements)
                {
                    //var room = document.GetElement(roomId);// as Room;
                    if (room == null)
                        continue;

                    var viewExist = all3DViews.Where(x => x.Name == room.Name).FirstOrDefault();
                    if (viewExist != null)
                    {
                        var tempName = viewExist.Name + DateTime.Now.ToString();
                        var correctedViewName = tempName
                            .Replace(':', '_')
                            .Replace('*', '_')
                            .Replace('?', '_')
                            .Replace('/', '_')
                            .Replace('\\', '_')
                            .Replace('[', '_')
                            .Replace(']', '_');
                        viewExist.Name = correctedViewName;
                        viewsToDelete.Add(viewExist.Id);
                    }
                    //document.Delete(viewExist.Id);

                    var bb = room.get_BoundingBox(null);

                    var newMin = new XYZ(bb.Min.X - offset, bb.Min.Y - offset, bb.Min.Z - offset);
                    var newMax = new XYZ(bb.Max.X + offset, bb.Max.Y + offset, bb.Max.Z + offset);

                    var newBb = app.Create.NewBoundingBoxXYZ();
                    newBb.Min = newMin;
                    newBb.Max = newMax;

                    var newView3D = View3D.CreateIsometric(document, vv.Id);
                    newView3D.SetSectionBox(newBb);

                    newView3D.Name = room.Name;

                    newView3D.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(4095);
                    newView3D.SetCategoryHidden(new ElementId(BuiltInCategory.OST_SectionBox), false);
                    newView3D.DisplayStyle = DisplayStyle.Shading;

                    viewsList.Add(newView3D);
                }

                transaction.Commit();

                uidoc.ActiveView = viewsList.First() as View;

                if(viewsToDelete.Count != 0)
                {
                    using var deleteTrans = new Transaction(document, "Delete View");
                    deleteTrans.Start();

                    document.Delete(viewsToDelete);

                    deleteTrans.Commit();
                }

                RoomToViewsData.RoomElements.Clear();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
                return Result.Failed;
            }
        }
        public Solid ScaleSolid(Solid solid, double scale)
        {
            try
            {
                var centroid = solid.ComputeCentroid();
                var centroidToProjectCenterVector = XYZ.Zero - centroid;
                var moveToCenterTransform = Transform.CreateTranslation(centroidToProjectCenterVector);
                solid = SolidUtils.CreateTransformed(solid, moveToCenterTransform);
                using var scaleTransform = Transform.CreateTranslation(XYZ.Zero).ScaleBasis(scale);
                solid = SolidUtils.CreateTransformed(solid, scaleTransform);
                solid = SolidUtils.CreateTransformed(solid, moveToCenterTransform.Inverse);
                return solid;
            }
            catch (System.Exception)
            {
                return solid;
            }
        }
    }
}
