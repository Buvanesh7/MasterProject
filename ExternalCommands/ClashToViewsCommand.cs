using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AkryazTools.Helpers;
using AkryazTools.Models;
using AkryazTools.Views;
using AkryazTools.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class ClashToViewsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            var app = commandData.Application.Application;
            try
            {
                var mainWindow = new ClashToViewsWindow(document);
                mainWindow.ShowDialog();

                var doc1 = ClashToViewsModel.DocumentA;
                var doc2 = ClashToViewsModel.DocumentB;

                var cats1 = ClashToViewsModel.ListOfCategoriesA;
                var cats2 = ClashToViewsModel.ListOfCategoriesB;

                if (doc1 == null || doc2 == null || cats1 == null || cats2 == null)
                    return Result.Cancelled;

                var patterns = new FilteredElementCollector(document).OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().ToList();
                var solidPattern = patterns.Where(x => x.GetFillPattern().IsSolidFill).FirstOrDefault();

                var catList1 = cats1.Select(x => x.Id).ToList();
                var catList2 = cats2.Select(x => x.Id).ToList();

                var multiCat1 = new ElementMulticategoryFilter(catList1);
                var multiCat2 = new ElementMulticategoryFilter(catList2);

                var view3Ds = new FilteredElementCollector(document).OfClass(typeof(View3D)).Cast<View3D>().ToList();
                var view3D = view3Ds.FirstOrDefault();
                if(!view3D.CanViewBeDuplicated(ViewDuplicateOption.Duplicate))
                {
                    view3D = null;
                    foreach (var v3 in view3Ds)
                    {
                        if (v3.CanViewBeDuplicated(ViewDuplicateOption.Duplicate))
                        {
                            view3D = v3;
                            break;
                        }
                    }
                }
                if(view3D == null)
                {
                    view3D = View3D.CreateIsometric(document, view3Ds.FirstOrDefault().GetTypeId());
                }
                var view3dNames = view3Ds.Select(x => x.Name).ToList();

                var set1 = new FilteredElementCollector(doc1).WherePasses(multiCat1).WhereElementIsNotElementType().ToList();

                var viewFilter = new ElementCategoryFilter(BuiltInCategory.OST_Views, true);

                using var transaction = new Transaction(document, "Isolate Clashing elements");
                transaction.Start();

                int i = 1;

                foreach (var element in set1)
                {
                    var bb1 = element.get_BoundingBox(null);
                    var bb1Min = bb1.Min;
                    var bb1Max = bb1.Max;

                    var bbFilter = new BoundingBoxIntersectsFilter(new Outline(bb1Min, bb1Max));
                    var intFilter = new ElementIntersectsElementFilter(element);
                    var set2 = new FilteredElementCollector(doc2).WherePasses(multiCat2).WhereElementIsNotElementType().WherePasses(bbFilter).WherePasses(intFilter).ToList();
                    if (set2.Count == 0)
                        continue;

                    foreach (var ele2 in set2)
                    {
                        var bbExpander = new BoundingBoxHelper();

                        var bb2 = ele2.get_BoundingBox(null);
                        var bb2Min = bb2.Min;
                        var bb2Max = bb2.Max;

                        bbExpander.ExpandToContain(bb1Min);
                        bbExpander.ExpandToContain(bb1Max);
                        bbExpander.ExpandToContain(bb2Min);
                        bbExpander.ExpandToContain(bb2Max);

                        var duplicatedView = document.GetElement(view3D.Duplicate(ViewDuplicateOption.Duplicate)) as View3D;
                        SetColor(app, duplicatedView, element, new Color(230, 113, 26), solidPattern);
                        SetColor(app, duplicatedView, ele2, new Color(14, 214, 150), solidPattern);

                        var cb = duplicatedView.GetSectionBox();
                        cb.Max = bbExpander.Max;
                        cb.Min = bbExpander.Min;

                        var newBb = app.Create.NewBoundingBoxXYZ();
                        newBb.Min = bbExpander.Min;
                        newBb.Max = bbExpander.Max;
                        duplicatedView.SetSectionBox(newBb);

                        duplicatedView.get_Parameter(BuiltInParameter.VIEWER_MODEL_CLIP_BOX_ACTIVE).Set(1);

                        //duplicatedView.CropBox = cb;
                        //duplicatedView.CropBoxActive = true;
                        //duplicatedView.CropBoxVisible = true;

                        //var isolationList = new List<ElementId> { ele2.Id, element.Id };
                        //var exclusionFilter = new ExclusionFilter(isolationList);
                        //var otherElements = new FilteredElementCollector(document, duplicatedView.Id).WherePasses(exclusionFilter).WherePasses(viewFilter).WhereElementIsNotElementType().ToElementIds();

                        //foreach (var otherEle in otherElements)
                        //{
                        //    var hideList = new List<ElementId> { otherEle };
                        //    try
                        //    {
                        //        duplicatedView.HideElements(hideList);
                        //    }
                        //    catch { }
                        //}

                        var endValue = GetUniqueSuffix(i, "Clash", view3dNames);

                        duplicatedView.Name = element.Category.Name+"Vs"+ ele2.Category.Name+"_" + endValue.ToString();
                        duplicatedView.get_Parameter(BuiltInParameter.VIEW_DISCIPLINE).Set(4095);
                        duplicatedView.SetCategoryHidden(new ElementId(BuiltInCategory.OST_SectionBox), false);
                        i = endValue + 1;
                        //duplicatedView.IsolateElementsTemporary(new List<ElementId> { element.Id, ele2.Id });
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

        public void SetColor(Autodesk.Revit.ApplicationServices.Application app, View duplicatedView, Element element, Color color, FillPatternElement solidPattern)
        {
            var ovrSettings = new OverrideGraphicSettings();
            var vNum = int.Parse(app.VersionNumber);
            if(vNum >= 2019)
            {
                new SetElementColorWrapper(ovrSettings, color, solidPattern.Id);
            }
            else
            {
                new SetElementColorWrapper(ovrSettings, color, solidPattern.Id, true);
            }
            duplicatedView.SetElementOverrides(element.Id, ovrSettings);
        }

        public int GetUniqueSuffix(int value, string viewName, List<string> view3dNames)
        {
            var sampleName = viewName + "_" + value.ToString();
            if (!view3dNames.Contains(sampleName))
                return value;
            else
                return GetUniqueSuffix(value + 1, viewName, view3dNames);
        }


    }
}
