using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AkryazTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class Orient3dViewCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var document = uidoc.Document;
            try
            {
                if (document.ActiveView.ViewType != ViewType.ThreeD)
                {
                    DisplayHelper.Display("Please go to a 3D view and try again!", Types.TaskDialogType.Error);
                    return Result.Failed;
                }

                var seletedRef = uidoc.Selection.PickObject(ObjectType.Face, "Pick a Face");
                var selectedElement = document.GetElement(seletedRef.ElementId);

                var selectedFace = selectedElement.GetGeometryObjectFromReference(seletedRef) as Face;

                using var transaction = new Transaction(document, "Orient3dViewCommand");
                transaction.Start();

                var view3d = document.ActiveView as View3D;
                view3d.IsSectionBoxActive = true;

                var secBox = view3d.GetSectionBox();
                var boxNormal = secBox.Transform.BasisX.Normalize();

                var origin = new XYZ(secBox.Min.X + (secBox.Max.X - secBox.Min.X) / 2, secBox.Min.Y + (secBox.Max.Y - secBox.Min.Y) / 2, 0.0);
                var axis = new XYZ(0, 0, 1);

                var normal = selectedFace.ComputeNormal(new UV(0, 0));
                var angle = normal.AngleTo(boxNormal);

                var rotate = Transform.Identity;
                if (normal.Y * boxNormal.X < 0)
                {
                    rotate = Transform.CreateRotationAtPoint(axis, Math.PI / 2 - angle, origin);
                }
                else
                {
                    rotate = Transform.CreateRotationAtPoint(axis, angle, origin);
                }

                secBox.Transform = secBox.Transform.Multiply(rotate);

                view3d.SetSectionBox(secBox);

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

    public class FaceSelector : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            throw new NotImplementedException();
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
