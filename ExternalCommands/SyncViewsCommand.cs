using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
    public class SyncViewsCommand : IExternalCommand
    {
        public ViewOrientation3D ViewOrientation3D { get; set; }
        public XYZ ViewDirection { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var document = uidoc.Document;
            try
            {
                var openViews = uidoc.GetOpenUIViews();

                UIView currentUiView = openViews[0];

                foreach (var openView in openViews)
                {
                    if (openView.ViewId != document.ActiveView.Id)
                        continue;
                    currentUiView = openView;
                    break;
                }

                if (currentUiView.ViewId == ElementId.InvalidElementId)
                {
                    DisplayHelper.Display("Active View is not Valid!", Types.TaskDialogType.Error);
                    return Result.Failed;
                }

                var rect = currentUiView.GetZoomCorners();

                if (document.ActiveView.GetType() == typeof(View3D))
                {
                    var acc3dView = document.ActiveView as View3D;
                    ViewOrientation3D = acc3dView.GetOrientation();
                }
                else if (document.ActiveView.GetType() == typeof(ViewSection))
                {
                    var acc3dView = document.ActiveView as ViewSection;
                    ViewDirection = acc3dView.ViewDirection;
                }

                using var transaction = new Transaction(document, "Name");
                transaction.Start();

                foreach (var openView in openViews)
                {
                    if (openView.ViewId == document.ActiveView.Id)
                        continue;
                    //if (document.ActiveView.GetType() == typeof(View3D))
                    //{
                    //    if (ViewOrientation3D == null)
                    //        continue;
                    //    var acc3dView = document.ActiveView as View3D;
                    //    acc3dView.SetOrientation(ViewOrientation3D);
                    //}
                    
                    openView.ZoomAndCenterRectangle(rect[0], rect[1]);
                }

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
