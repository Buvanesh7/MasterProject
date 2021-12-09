using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AkryazTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace AkryazTools.ExternalCommands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ExportScheduleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                const int col_header_row = 3;
                var time_start = DateTime.Now;
                var doc = commandData.Application.ActiveUIDocument.Document;
                string filename_no_ext = doc.Title;

                ViewScheduleExportOptions opt = new ViewScheduleExportOptions();
                var activeSchedule = doc.ActiveView as ViewSchedule;
                if (activeSchedule is null)
                {
                    DisplayHelper.Display("Active view is not a Schedule", Types.TaskDialogType.Error);
                    return Result.Cancelled;
                }

                if (!filename_no_ext.EndsWith(".rvt"))
                {
                    filename_no_ext = filename_no_ext + ".rvt";
                }

                //var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string documentPath = doc.PathName.Replace(filename_no_ext, "");
                filename_no_ext = filename_no_ext.Replace(".rvt", "");

                Excel.Application xlApp;
                Excel.Workbook xlWorkBook;
                Excel.Worksheet xlWorkSheet;

                Excel.Range xlRange;
                Excel.QueryTable xlQuery;
                xlApp = new Excel.Application();

                if (xlApp == null)
                {
                    TaskDialog.Show("ExportAllSchedulesToOneExcel", "Excel is not installed!!");
                    return Result.Failed;
                }

                object default_value = System.Reflection.Missing.Value;
                xlWorkBook = xlApp.Workbooks.Add(default_value);
                //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                var activeScheduleName = activeSchedule.Name;
                bool nameCheck = false;

                if (activeScheduleName.Length > 31)
                {
                    using var trans1 = new Transaction(doc, "Name Change");
                    trans1.Start();
                    var newScheduleName = activeScheduleName.Substring(0, 30);
                    activeSchedule.Name = newScheduleName;
                    trans1.Commit();
                    nameCheck = true;
                    //TaskDialog.Show("ExportAllSchedulesToOneExcel",
                    //    activeSchedule.Name + "\n" + "Schedule name should not be more than 31 characters!!");
                    ////releaseObject(xlWorkSheet);
                    //releaseObject(xlWorkBook);
                    //releaseObject(xlApp);
                    //return Result.Failed;
                }


                var fileName = activeSchedule.Name
                    .Replace(':', '_')
                    .Replace('*', '_')
                    .Replace('?', '_')
                    .Replace('/', '_')
                    .Replace('\\', '_')
                    .Replace('[', '_')
                    .Replace(']', '_');

                try
                {
                    activeSchedule.Export(documentPath, fileName + ".txt", opt);
                }
                catch (Exception)
                {
                    TaskDialog.Show("Exporting view schedules",
                                    "Errors occurred -\n" +
                                    "possibly the folder is read only\n" +
                                    "e.g. in the case of sample projects provided by Revit,\n" +
                                    "save the project to another folder first.\n\n" +
                                    "Close to exit program.");
                    //releaseObject(xlWorkSheet);
                    //releaseObject(xlWorkBook);
                    releaseObject(xlApp);
                    return Result.Failed;
                }

                if(nameCheck)
                {
                    using var trans2 = new Transaction(doc, "Name Change");
                    trans2.Start();
                    activeSchedule.Name = activeScheduleName;
                    trans2.Commit();
                }

                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.Add(default_value);
                xlWorkSheet.Move(default_value, xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count]);
                xlWorkSheet.Name = fileName;
                xlQuery = xlWorkSheet.QueryTables.Add(
                    "TEXT;" + documentPath + fileName + ".txt",
                    xlWorkSheet.get_Range("A" + 1));
                xlQuery.RefreshStyle = Excel.XlCellInsertionMode.xlInsertEntireRows;
                xlQuery.Refresh(false);
                xlQuery.Delete();

                xlWorkSheet.get_Range("A1", "A" + col_header_row).EntireRow.Font.Bold = false;
                System.IO.File.Delete(documentPath + fileName + ".txt");

                xlRange = xlWorkSheet.get_Range("A1");

                xlWorkSheet.Move(xlWorkBook.Worksheets[1]);

                int loop_A_Mx = xlWorkBook.Worksheets.Count;
                for (int loop_A = loop_A_Mx; loop_A >= 1; loop_A--)
                {
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(loop_A);
                    xlWorkSheet.Activate();
                    xlWorkSheet.Application.ActiveWindow.SplitRow = 2;
                    xlWorkSheet.Application.ActiveWindow.FreezePanes = true;

                    xlRange = xlWorkSheet.get_Range("A1", "A" + xlRowLast(xlWorkSheet));
                    //xlRange.Font.Bold = true;
                    xlRange.DataSeries(default_value,
                      Excel.XlDataSeriesType.xlDataSeriesLinear,
                      Excel.XlDataSeriesDate.xlDay,
                      "1", default_value, default_value);

                    //xlWorkSheet.Range[xlWorkSheet.Cells[1, 1], xlWorkSheet.Cells[1, xlColumnLast(xlWorkSheet)]].Merger();
                    var test = xlWorkSheet.get_Range("A1", letters[xlColumnLast(xlWorkSheet)-1]+"1");
                    test.Merge(true);
                    test.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    //xlRange.Font.Bold = true;

                    xlWorkSheet.Columns.EntireColumn.AutoFit();

                    xlWorkSheet.get_Range("A1").Font.Bold = true;
                    xlWorkSheet.get_Range("A2", letters[xlColumnLast(xlWorkSheet)] + "2").Font.Bold = true;

                    xlWorkSheet.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                    xlWorkSheet.PageSetup.PrintTitleRows = "$1:$" + col_header_row;
                    xlWorkSheet.PageSetup.LeftFooter = filename_no_ext;
                    xlWorkSheet.PageSetup.RightFooter = "&P/&N";
                    xlWorkSheet.PageSetup.Zoom = false;
                    xlWorkSheet.PageSetup.FitToPagesTall = false;
                    xlWorkSheet.PageSetup.FitToPagesWide = 1;
                }

                string filePath = FolderDialogHandler.GetSaveFilePath(FolderDialogHandler.ExcelFilter, fileName);
                if (string.IsNullOrEmpty(filePath))
                {
                    DisplayHelper.Display("Please select a destination", Types.TaskDialogType.Warning);
                    releaseObject(xlWorkSheet);
                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);
                    return Result.Cancelled;
                }

                xlWorkBook.SaveAs(filePath, default_value, default_value, default_value, false, default_value, Excel.XlSaveAsAccessMode.xlNoChange, default_value,
                    default_value, default_value, default_value, default_value);
                //xlWorkBook.SaveAs(folder_name + filename_no_ext,
                //  default_value, default_value, default_value,
                //  default_value, default_value,
                //  Excel.XlSaveAsAccessMode.xlNoChange,
                //  default_value, true, default_value,
                //  default_value, true);

                xlApp.ActiveWindow.WindowState = Excel.XlWindowState.xlMaximized;
                xlApp.Visible = true;

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                return Result.Succeeded;
            }
            catch(Exception ex)
            {
                DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
                return Result.Failed;
            }
        }

        private string xlCellAddress(int row, int col)
        {
            // change cell address from (100, 1) to (A100) style
            string prompt = row + "\n\t" + col + "\n\t";
            if (row < 1 || row > 1048576)
            {
                TaskDialog.Show("Excel Row Number", "Error - must be within 1 - 1048576!!");
                return null;
            }
            // append row number to alphabetical column reference
            return xlColumnAddress(col) + row.ToString();
        }

        private string xlCellValue2(Excel.Worksheet w_s, int row, int col)
        {
            // return value of worksheet cell
            Excel.Range xlRange = (Excel.Range)w_s.Cells[row, col];
            if (xlRange.Value2 != null)
            {
                return xlRange.Value2.ToString();
            }
            else
            {
                return "";
            }
        }

        private string xlColumnAddress(int col)
        {
            // convert column number to alphabetical reference
            if (col < 1 || col > 16384)
            {
                TaskDialog.Show("Excel Column Number", "Error - must be within 1 - 16384!!");
                return null;
            }
            int remainder = 0;
            string result = "";
            for (int loop_A = 0; loop_A < 3; loop_A++)
            {
                // get remainder after division by 26
                remainder = (col - 1) % 26 + 1;
                if (remainder != 0)
                {
                    // match the remainder to alphabets A to Z where A is char 65
                    // precede the alphabet to the previous result
                    result = Convert.ToChar(remainder + 64).ToString() + result;
                }
                col = (col - 1) / 26;
                // do it three times
            }
            return result;
        }

        private int xlColumnFindExact(Excel.Worksheet w_s, string find_what, int which_row)
        {
            object default_value = System.Reflection.Missing.Value;
            var xlFound = w_s.get_Range(which_row + ":" + which_row);
            xlFound = xlFound.Find(find_what, default_value,
              Excel.XlFindLookIn.xlValues,
              Excel.XlLookAt.xlWhole,
              Excel.XlSearchOrder.xlByRows,
              Excel.XlSearchDirection.xlNext,
              false, default_value, default_value);
            if (xlFound != null)
            {
                return xlFound.Column;
            }
            else
            {
                return 0;
            }
        }

        private void xlColumnMove(Excel.Worksheet w_s, int col_from, int col_to)
        {
            if (col_from != 0 & col_to != 0 & col_from != col_to)
            {
                Excel.Range from_range = (Excel.Range)w_s.Cells[1, col_from];
                Excel.Range to_range = (Excel.Range)w_s.Cells[1, col_to];
                to_range.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, from_range.EntireColumn.Cut());
            }
        }

        private int xlRowLast(Excel.Worksheet w_s)
        {
            // return last used row number of worksheet
            return w_s.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;
        }

        private int xlColumnLast(Excel.Worksheet w_s)
        {
            // return last used row number of worksheet
            return w_s.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Column;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                TaskDialog.Show("Excel file created", "Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }


    }
}
