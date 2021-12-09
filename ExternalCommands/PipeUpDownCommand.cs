using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AkryazTools.Enum;
using AkryazTools.External_Events;
using AkryazTools.Helpers;
using AkryazTools.Models;
using AkryazTools.ViewModels;
using AkryazTools.Views;
using AkryazTools.WindowHandle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class PipeUpDownCommand : IExternalCommand
    {
        public static Window ActiveWindow;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {

                PipeUpDownExternalEvent.CreateEvent();
                
                if (ActiveWindow == null)
                {
                    var testVm = new PipeUpDownViewModel();
                    ActiveWindow = testVm.MainWindow;
                    WindowHandler.SetWindowOwner(commandData.Application, ActiveWindow);
                }

                if (PipeUpDownData.OffsetValue == 0 || PipeUpDownData.AngleValue == 0)
                {
                    return Result.Cancelled;
                }

                //var window = new PipeUpDownWindow(document);
                //window.ShowDialog();



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
