using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AkryazTools.External_Events;
using AkryazTools.Helpers;
using AkryazTools.ViewModels;
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
    public class TestCommand : IExternalCommand
    {

        public static Window ActiveWindow;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                TestExternalEvent.CreateEvent();

                if (ActiveWindow == null)
                {
                    var testVm = new TestViewModel();
                    ActiveWindow = testVm.MainWindow;
                    WindowHandler.SetWindowOwner(commandData.Application, ActiveWindow);
                }

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
