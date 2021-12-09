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
    public class LinklessCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //var document = commandData.Application.ActiveUIDocument.Document;
            var app = commandData.Application.Application;
            var uiapp = commandData.Application;
            try
            {
                var filePath = FolderDialogHandler.GetOpenFilePath("RVT Files(.rvt)|*.rvt");
                if(string.IsNullOrEmpty(filePath))
                    return Result.Cancelled;

                var file = new FileInfo(filePath);
                if (IsFileLocked(file))
                {
                    DisplayHelper.Display("The File is locked by another application or it is open by another user.Can't access file", Types.TaskDialogType.Warning);
                    return Result.Cancelled;
                }

                var fileInfo = BasicFileInfo.Extract(filePath);
                if(fileInfo.IsSavedInLaterVersion)
                {
                    DisplayHelper.Display("File saved in later version. Can't be opened.", Types.TaskDialogType.Error);
                    return Result.Failed;
                }

                FilePath val2 = new FilePath(filePath);
                TransmissionData val3 = TransmissionData.ReadTransmissionData((ModelPath)(object)val2);
                Dictionary<int, string> dictionary = new Dictionary<int, string>();
                if (val3 != null)
                {
                    ICollection<ElementId> allExternalFileReferenceIds = val3.GetAllExternalFileReferenceIds();
                    foreach (ElementId item in allExternalFileReferenceIds)
                    {
                        ExternalFileReference lastSavedReferenceData = val3.GetLastSavedReferenceData(item);
                        ExternalFileReferenceType externalFileReferenceType = lastSavedReferenceData.ExternalFileReferenceType;
                        LinkedFileStatus linkedFileStatus;
                        if ((int)lastSavedReferenceData.ExternalFileReferenceType == 1)
                        {
                            int integerValue = item.IntegerValue;
                            linkedFileStatus = lastSavedReferenceData.GetLinkedFileStatus();
                            dictionary.Add(integerValue, ((object)(LinkedFileStatus)(linkedFileStatus)).ToString());
                            val3.SetDesiredReferenceData(item, lastSavedReferenceData.GetPath(), lastSavedReferenceData.PathType, false);
                        }
                        if ((int)lastSavedReferenceData.ExternalFileReferenceType == 2)
                        {
                            int integerValue2 = item.IntegerValue;
                            linkedFileStatus = lastSavedReferenceData.GetLinkedFileStatus();
                            dictionary.Add(integerValue2, ((object)(LinkedFileStatus)(linkedFileStatus)).ToString());
                            val3.SetDesiredReferenceData(item, lastSavedReferenceData.GetPath(), lastSavedReferenceData.PathType, false);
                        }
                        if ((int)lastSavedReferenceData.ExternalFileReferenceType == 3)
                        {
                            int integerValue3 = item.IntegerValue;
                            linkedFileStatus = lastSavedReferenceData.GetLinkedFileStatus();
                            dictionary.Add(integerValue3, ((object)(LinkedFileStatus)(linkedFileStatus)).ToString());
                            val3.SetDesiredReferenceData(item, lastSavedReferenceData.GetPath(), lastSavedReferenceData.PathType, false);
                        }
                    }
                }

                val3.IsTransmitted = true;
                TransmissionData.WriteTransmissionData((ModelPath)(object)val2, val3);

                var newUiDoc = uiapp.OpenAndActivateDocument(filePath);
                var newDoc = newUiDoc.Document;

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
                return Result.Failed;
            }
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                fileStream?.Close();
            }
            return false;
        }

        //public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        //{
        //    return true;
        //}
    }
}
