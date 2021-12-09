using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkryazTools.Helpers
{
    public static class FolderDialogHandler
    {
        public static string ExcelFilter = "Excel Files(.xlsx)|*.xlsx|Excel 2010(.xls)|*.xls";
        public static string CsvFilter = "CSV Files(.csv)|*.csv";
        public static string GetSaveFilePath(string filter, string fileName)
        {
            string filePath = "";
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.FileName = fileName;
            fileDialog.Filter = filter;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = fileDialog.FileName;
            }
            return filePath;
        }

        public static string GetOpenFilePath(string filter)
        {
            string filePath = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = filter;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = fileDialog.FileName;
            }
            return filePath;
        }

        public static string[] OpenMultipleFilePaths(string filter)
        {
            string[] filePaths = { "a" };
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = filter;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePaths = fileDialog.FileNames;
            }
            return filePaths;
        }

        public static string GetOpenFolderPath()
        {
            string filePath = "";
            var folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
            {
                filePath = folderDialog.SelectedPath;
            }
            return filePath;

        }

        public static string GetRevitDocumentName(Document document)
        {
            string fileName = string.Empty;
            try
            {
                fileName = Path.GetFileNameWithoutExtension(document.PathName);
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Path.GetFileNameWithoutExtension(document.Title);
                }
            }
            catch (Exception)
            {

            }

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar.ToString(), "");
            }

            return fileName;
        }
    }
}
