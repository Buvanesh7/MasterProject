using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using AkryazTools.Helpers;
//using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;


namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class ExportElectricalPanelSchedule : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                //                var panelSchedules = new FilteredElementCollector(document).OfClass(typeof(PanelScheduleView)).Cast<PanelScheduleView>().ToList();

                //                ExcelTextFormat format = new ExcelTextFormat();
                //                format.Delimiter = ';';
                //                format.Culture = new CultureInfo(Thread.CurrentThread.CurrentCulture.ToString());
                //                format.Culture.DateTimeFormat.ShortDatePattern = "dd-mm-yyyy";
                //                format.Encoding = new UTF8Encoding();

                //                var activePannelSchedule = document.ActiveView as PanelScheduleView;
                //                if (activePannelSchedule is null)
                //                {
                //                    DisplayHelper.Display("Active view is not a Electrical Pannel Schedule", Types.TaskDialogType.Error);
                //                    return Result.Cancelled;
                //                }
                //                var ps = activePannelSchedule;
                //                //foreach (var ps in panelSchedules)
                //                //{
                //                //    if (ps.IsPanelScheduleTemplate())
                //                //        continue;
                //                var correctedName = ReplaceIllegalCharacters(ps.Name);
                //                string filePath = FolderDialogHandler.GetSaveFilePath(FolderDialogHandler.CsvFilter, correctedName);
                //                //var csvPath = filePath.Replace(".xlsx", ".csv");
                //                FileInfo csvFile = new FileInfo(filePath);
                //                using StreamWriter sw = File.CreateText(filePath);
                //                DumpPanelScheduleData(sw, ps);
                //                sw.Close();

                //                //if (string.IsNullOrEmpty(filePath))
                //                //    continue;

                //                if (string.IsNullOrEmpty(filePath))
                //                {
                //                    return Result.Cancelled;
                //                }

                //                //Excel.Application xlApp;
                //                //Excel.Workbook xlWorkBook;
                //                //Excel.Worksheet xlWorkSheet;

                //                //Excel.Range xlRange;
                //                //Excel.QueryTable xlQuery;
                //                //xlApp = new Excel.Application();

                //                //if (xlApp == null)
                //                //{
                //                //    TaskDialog.Show("ExportAllSchedulesToOneExcel", "Excel is not installed!!");
                //                //    return Result.Failed;
                //                //}

                //                //const int col_header_row = 3;

                //                //var documentPath = csvPath.Replace(correctedName+".csv", "");

                //                //object default_value = System.Reflection.Missing.Value;
                //                //xlWorkBook = xlApp.Workbooks.Add(default_value);

                //                //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.Add(default_value);
                //                //xlWorkSheet.Move(default_value, xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count]);
                //                //xlWorkSheet.Name = activePannelSchedule.Name;
                //                //xlQuery = xlWorkSheet.QueryTables.Add(
                //                //    "TEXT;" + documentPath + correctedName + ".csv",
                //                //    xlWorkSheet.get_Range("A" + 1));
                //                //xlQuery.RefreshStyle = Excel.XlCellInsertionMode.xlInsertEntireRows;
                //                //xlQuery.Refresh(false);
                //                //xlQuery.Delete();

                //                //xlWorkSheet.Move(xlWorkBook.Worksheets[1]);

                //                //int loop_A_Mx = xlWorkBook.Worksheets.Count;
                //                //for (int loop_A = loop_A_Mx; loop_A >= 1; loop_A--)
                //                //{
                //                //    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(loop_A);
                //                //    xlWorkSheet.Activate();


                //                //    xlRange = xlWorkSheet.get_Range("A1", "A" + xlRowLast(xlWorkSheet));

                //                //    xlRange.DataSeries(default_value,
                //                //      Excel.XlDataSeriesType.xlDataSeriesLinear,
                //                //      Excel.XlDataSeriesDate.xlDay,
                //                //      "1", default_value, default_value);


                //                //    xlWorkSheet.PageSetup.PrintTitleRows = "$1:$" + col_header_row;
                //                //    xlWorkSheet.PageSetup.LeftFooter = correctedName;
                //                //    xlWorkSheet.PageSetup.RightFooter = "&P/&N";
                //                //    xlWorkSheet.PageSetup.Zoom = false;
                //                //    xlWorkSheet.PageSetup.FitToPagesTall = false;
                //                //    xlWorkSheet.PageSetup.FitToPagesWide = 1;
                //                //}

                //                //xlWorkBook.SaveAs(filePath, default_value, default_value, default_value, false, default_value, Excel.XlSaveAsAccessMode.xlNoChange, default_value,
                //                //    default_value, default_value, default_value, default_value);

                //                //xlApp.ActiveWindow.WindowState = Excel.XlWindowState.xlMaximized;
                //                //xlApp.Visible = true;

                //                //releaseObject(xlWorkSheet);
                //                //releaseObject(xlWorkBook);
                //                //releaseObject(xlApp);

                //                //FileInfo exxfile = new FileInfo(csvPath);
                //                //var excelPath = filePath.Replace(".csv", ".xls");
                //                //var excelFileInfo = new FileInfo(excelPath);
                //                //using ExcelPackage excelPackage = new ExcelPackage(excelFileInfo);
                //                //ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add(correctedName);
                //                //worksheet.Cells["A1"].LoadFromText(exxfile, format);
                //                //excelPackage.Save();

                //                //csvFile.Delete();


                //                System.Diagnostics.Process.Start(filePath);

                //                //}
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

//        private void DumpPanelScheduleData(StreamWriter sw, PanelScheduleView m_psView)
//        {
//            DumpSectionData(sw, m_psView, SectionType.Header);
//            DumpSectionData(sw, m_psView, SectionType.Body);
//            DumpSectionData(sw, m_psView, SectionType.Summary);
//            DumpSectionData(sw, m_psView, SectionType.Footer);
//        }

//        private void DumpSectionData(StreamWriter sw, PanelScheduleView m_psView, SectionType sectionType)
//        {
//            int nRows_Section = 0;
//            int nCols_Section = 0;
//            getNumberOfRowsAndColumns(m_psView.Document, m_psView, sectionType, ref nRows_Section, ref nCols_Section);

//            for (int ii = 0; ii < nRows_Section; ++ii)
//            {
//                StringBuilder oneRow = new StringBuilder();
//                for (int jj = 0; jj < nCols_Section; ++jj)
//                {
//                    try
//                    {
//                        oneRow.AppendFormat("{0},", m_psView.GetCellText(sectionType, ii, jj));
//                    }
//                    catch (Exception)
//                    {
//                        // do nothing.
//                    }
//                }

//                sw.WriteLine(oneRow.ToString());
//            }
//        }

//        public void getNumberOfRowsAndColumns(Autodesk.Revit.DB.Document doc, PanelScheduleView psView, SectionType sectionType, ref int nRows, ref int nCols)
//        {
//            Transaction openSectionData = new Transaction(doc, "openSectionData");
//            openSectionData.Start();

//            TableSectionData sectionData = psView.GetSectionData(sectionType);
//            nRows = sectionData.NumberOfRows;
//            nCols = sectionData.NumberOfColumns;

//            openSectionData.RollBack();
//        }

//        private string ReplaceIllegalCharacters(string name)
//        {
//            char[] illegalChars = System.IO.Path.GetInvalidFileNameChars();

//            string updated = name;
//            foreach (char ch in illegalChars)
//            {
//                updated = updated.Replace(ch, '_');
//            }

//            return updated;
//        }

//        private string xlCellAddress(int row, int col)
//        {
//            // change cell address from (100, 1) to (A100) style
//            string prompt = row + "\n\t" + col + "\n\t";
//            if (row < 1 || row > 1048576)
//            {
//                TaskDialog.Show("Excel Row Number", "Error - must be within 1 - 1048576!!");
//                return null;
//            }
//            // append row number to alphabetical column reference
//            return xlColumnAddress(col) + row.ToString();
//        }

//        private string xlCellValue2(Excel.Worksheet w_s, int row, int col)
//        {
//            // return value of worksheet cell
//            Excel.Range xlRange = (Excel.Range)w_s.Cells[row, col];
//            if (xlRange.Value2 != null)
//            {
//                return xlRange.Value2.ToString();
//            }
//            else
//            {
//                return "";
//            }
//        }

//        private string xlColumnAddress(int col)
//        {
//            // convert column number to alphabetical reference
//            if (col < 1 || col > 16384)
//            {
//                TaskDialog.Show("Excel Column Number", "Error - must be within 1 - 16384!!");
//                return null;
//            }
//            int remainder = 0;
//            string result = "";
//            for (int loop_A = 0; loop_A < 3; loop_A++)
//            {
//                // get remainder after division by 26
//                remainder = (col - 1) % 26 + 1;
//                if (remainder != 0)
//                {
//                    // match the remainder to alphabets A to Z where A is char 65
//                    // precede the alphabet to the previous result
//                    result = Convert.ToChar(remainder + 64).ToString() + result;
//                }
//                col = (col - 1) / 26;
//                // do it three times
//            }
//            return result;
//        }

//        private int xlColumnFindExact(Excel.Worksheet w_s, string find_what, int which_row)
//        {
//            object default_value = System.Reflection.Missing.Value;
//            var xlFound = w_s.get_Range(which_row + ":" + which_row);
//            xlFound = xlFound.Find(find_what, default_value,
//              Excel.XlFindLookIn.xlValues,
//              Excel.XlLookAt.xlWhole,
//              Excel.XlSearchOrder.xlByRows,
//              Excel.XlSearchDirection.xlNext,
//              false, default_value, default_value);
//            if (xlFound != null)
//            {
//                return xlFound.Column;
//            }
//            else
//            {
//                return 0;
//            }
//        }

//        private void xlColumnMove(Excel.Worksheet w_s, int col_from, int col_to)
//        {
//            if (col_from != 0 & col_to != 0 & col_from != col_to)
//            {
//                Excel.Range from_range = (Excel.Range)w_s.Cells[1, col_from];
//                Excel.Range to_range = (Excel.Range)w_s.Cells[1, col_to];
//                to_range.EntireColumn.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, from_range.EntireColumn.Cut());
//            }
//        }

//        private int xlRowLast(Excel.Worksheet w_s)
//        {
//            // return last used row number of worksheet
//            return w_s.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;
//        }

//        private int xlColumnLast(Excel.Worksheet w_s)
//        {
//            // return last used row number of worksheet
//            return w_s.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Column;
//        }

//        private void releaseObject(object obj)
//        {
//            try
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
//                obj = null;
//            }
//            catch (Exception ex)
//            {
//                obj = null;
//                TaskDialog.Show("Excel file created", "Exception Occurred while releasing object " + ex.ToString());
//            }
//            finally
//            {
//                GC.Collect();
//            }
//        }
//    }
//}
