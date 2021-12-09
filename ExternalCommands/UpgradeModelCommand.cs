using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AkryazTools.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class UpgradeModelCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                
                var folderPaths = FolderDialogHandler.OpenMultipleFilePaths("Revit Files(.rvt; .rfa; .rte) | *.rvt; *.rfa; *.rte");//| RFA Files(.rfa) | *.rfa| RTE Files(.rte) | *.rte");//("RVT Files(.rvt)|*.rvt");

                if(folderPaths.Contains("a"))
                {
                    return Result.Cancelled;             
                }

                var openOptions = new OpenOptions 
                {
                    DetachFromCentralOption = DetachFromCentralOption.DoNotDetach,
                    Audit = false
                };

                var workSharingOptions = new WorksharingSaveAsOptions { SaveAsCentral = true };

                var saveAsOptions = new SaveAsOptions 
                {
                    MaximumBackups = 50,
                    OverwriteExistingFile = true,
                };
                saveAsOptions.SetWorksharingOptions(workSharingOptions);

                var saveAsNonWsOpttions = new SaveAsOptions { OverwriteExistingFile = true };

                var relinquishOptions = new RelinquishOptions(false) 
                {
                    StandardWorksets = true,
                    ViewWorksets = true,
                    FamilyWorksets = true,
                    UserWorksets = true,
                    CheckedOutElements = true
                };

                var synchroniseOptions = new SynchronizeWithCentralOptions 
                {
                    Compact = true,
                    SaveLocalBefore = true,
                    SaveLocalAfter = true
                };
                synchroniseOptions.SetRelinquishOptions(relinquishOptions);

                var tOptions = new TransactWithCentralOptions();

                foreach (var path in folderPaths)
                {
                    var modelPath = new FilePath(path);
                    var newDoc = app.OpenDocumentFile(modelPath, openOptions);

                    if(newDoc.IsWorkshared)
                    {
                        newDoc.SaveAs(path, saveAsOptions);
                        newDoc.SynchronizeWithCentral(tOptions, synchroniseOptions);                    
                    }
                    else
                        newDoc.SaveAs(path, saveAsNonWsOpttions);

                    newDoc.Close();
                }
                if(folderPaths.Count() == 0)
                {
                    DisplayHelper.Display("No Revit Models Upgraded", Types.TaskDialogType.Information);
                }
                else
                {
                    DisplayHelper.Display("Upgraded " + folderPaths.Count() + " Models", Types.TaskDialogType.Information);
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
