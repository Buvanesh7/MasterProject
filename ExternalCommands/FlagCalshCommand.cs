using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
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
    public class FlagCalshCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                var mainWindow = new FlagClashWindow(document);
                mainWindow.ShowDialog();

                var doc1 = ClashToViewsModel.DocumentA;
                var doc2 = ClashToViewsModel.DocumentB;

                var cats1 = ClashToViewsModel.ListOfCategoriesA;
                var cats2 = ClashToViewsModel.ListOfCategoriesB;

                var fam = ClashToViewsModel.AnnotationFamily;

                if (doc1 == null || doc2 == null || cats1 == null || cats2 == null)
                    return Result.Cancelled;

                var patterns = new FilteredElementCollector(document).OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().ToList();
                var solidPattern = patterns.Where(x => x.GetFillPattern().IsSolidFill).FirstOrDefault();

                var catList1 = cats1.Select(x => x.Id).ToList();
                var catList2 = cats2.Select(x => x.Id).ToList();

                var allCats = catList1.Concat(catList2).ToList();

                var multiCatFilter = new ElementMulticategoryFilter(allCats);
                var activeViewEles = new FilteredElementCollector(document, document.ActiveView.Id).WherePasses(multiCatFilter).ToElementIds();

                var multiCat1 = new ElementMulticategoryFilter(catList1);
                var multiCat2 = new ElementMulticategoryFilter(catList2);

                var set1 = new FilteredElementCollector(doc1).WherePasses(multiCat1).WhereElementIsNotElementType().ToList();

                using var transaction = new Transaction(document, "Isolate Clashing elements");
                transaction.Start();

                foreach (var element in set1)
                {
                    var bb1 = element.get_BoundingBox(null);
                    var bb1Min = bb1.Min;
                    var bb1Max = bb1.Max;

                    var geoElement = element.get_Geometry(new Options());
                    var solid1 = geoElement.Select(x => x as Solid).FirstOrDefault();
                    if (solid1 == null)
                        continue;

                    var bbFilter = new BoundingBoxIntersectsFilter(new Outline(bb1Min, bb1Max));
                    var intFilter = new ElementIntersectsElementFilter(element);
                    var set2 = new FilteredElementCollector(doc2).WherePasses(multiCat2).WhereElementIsNotElementType().WherePasses(bbFilter).WherePasses(intFilter).ToList();
                    if (set2.Count == 0)
                        continue;
                    foreach (var ele2 in set2)
                    {
                        if (!activeViewEles.Contains(ele2.Id) && !activeViewEles.Contains(element.Id))
                            continue;
                        
                        var geoElement2 = ele2.get_Geometry(new Options());
                        var solid2 = geoElement2.Select(x => x as Solid).FirstOrDefault();
                        if (solid2 == null)
                            continue;
                        var intSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
                        if (intSolid.Volume == 0)
                            continue;
                        var centroid = intSolid.ComputeCentroid();
                        //var fs = document.GetElement(new ElementId(971166)) as FamilySymbol;

                        document.Create.NewFamilyInstance(centroid, fam, document.ActiveView);

                    }

                }
                transaction.Commit();
                doc1 = null;
                doc2 = null;
                cats1 = null;
                cats2 = null;

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
