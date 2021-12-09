using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
    public class HideLinkedCategoriesCommand : IExternalCommand
    {
		public string _visibilityParameterName { get; set; } = "Type Name";


		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
				var mainWindow = new HideLinkedCategoriesWindow(document);
				mainWindow.ShowDialog();

				if(HideLinkedCategoriesModel.ListOfCategories.Count == 0)
                {
					return Result.Cancelled;
                }

				using var transaction = new Transaction(document, "HideLinkedCategories");
                transaction.Start();

                var ifilter = "HideLinked_";
                var activeView = document.ActiveView;
                var found = false;


				var gtypeElements = new List<Element>();
				var cateIds = new List<ElementId>();

				foreach (var cat in HideLinkedCategoriesModel.ListOfCategories)
                {
					ifilter += cat.Name;
					cateIds.Add(cat.Id);
					var eleTypes = new FilteredElementCollector(document).OfCategoryId(cat.Id).WhereElementIsElementType().ToList();//.GetElementIterator();
                    foreach (var eleType in eleTypes)
                    {
						gtypeElements.Add(eleType);

					}
     //               while (eleTypes.MoveNext())
     //               {
					//	var e = eleTypes.Current;
					//	var paramCheck = e.LookupParameter(_visibilityParameterName);
					//	if (paramCheck == null)
     //                   {
					//		//var tempCatList = new List<ElementId> { cat.Id };
					//		CreateProjectParameters(document, new List<ElementId> { cat.Id });
					//	}
					//	gtypeElements.Add(e);
					//	if (e.LookupParameter(_visibilityParameterName).AsInteger() != 1)
					//	{
					//		e.LookupParameter(_visibilityParameterName).Set(1);
					//	}
					//}
                }

				if (HideLinkedCategoriesModel.SelectedViewPickingOption == "Active View")
                {
					var accView = document.ActiveView;
					SetFilter(document, accView, ifilter, cateIds, gtypeElements, found);
				}
				if (HideLinkedCategoriesModel.SelectedViewPickingOption == "All Views")
                {
					var views = new FilteredElementCollector(document).OfClass(typeof(View)).OfType<View>().Where(v1 => !v1.IsTemplate).Where(v1 => v1.CanUseTemporaryVisibilityModes());
                    foreach (var view in views)
                    {
						SetFilter(document, view, ifilter, cateIds, gtypeElements, found);
					}
				}
				if (HideLinkedCategoriesModel.SelectedViewPickingOption == "All Views on Sheet")
				{
					var sheets = new FilteredElementCollector(document).OfClass(typeof(ViewSheet)).OfType<ViewSheet>().GetEnumerator();

					while (sheets.MoveNext())
					{
						var sheet = sheets.Current;
						var viewportIds = sheet.GetAllViewports();//.Select(x => document.GetElement(x) as Viewport);
						foreach (var viewportId in viewportIds)
						{
							var viewport = document.GetElement(viewportId) as Viewport;
							var view = document.GetElement(viewport.ViewId) as View;

							if (view.ViewType == ViewType.Legend)
								continue;
							
							SetFilter(document, view, ifilter, cateIds, gtypeElements, found);

						}
					}
				}
				transaction.Commit();
				HideLinkedCategoriesModel.ListOfCategories.Clear();
				return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
                return Result.Failed;
            }
        }

		public void SetFilter(Document document, View view, string ifilter, List<ElementId> cateIds, List<Element> gtypeElements, bool found)
        {
			var allFilters = new FilteredElementCollector(document).OfClass(typeof(FilterElement)).OfType<FilterElement>().ToList();

			var viewFilters = view.GetFilters();
			var viewFiltersName = viewFilters.Select(x => document.GetElement(x).Name).ToList();

			foreach (var fter in allFilters)
			{
				if (ifilter == fter.Name.ToString() && !viewFiltersName.Contains(ifilter))
				{
					var pFilter = fter as ParameterFilterElement;
					if (pFilter == null)
						continue;
					pFilter.SetCategories(cateIds);
					view.AddFilter(fter.Id);
					view.SetFilterVisibility(fter.Id, false);
					found = true;
				}
				if (ifilter == fter.Name.ToString() && viewFiltersName.Contains(ifilter))
				{
					var pFilter = fter as ParameterFilterElement;
					if (pFilter == null)
						continue;
					pFilter.SetCategories(cateIds);
					view.SetFilterVisibility(fter.Id, false);
					found = true;
				}
			}
			if (!found)
			{
				var paramId = gtypeElements.First().LookupParameter(_visibilityParameterName).Id;
				var ruleSet = new List<FilterRule>();
				var notEqualsRule = ParameterFilterRuleFactory.CreateNotEndsWithRule(paramId, "_BimEra", true);
				ruleSet.Add(notEqualsRule);
				var paramFilter = new ElementParameterFilter(ruleSet, false);
				var paramFilterElem = ParameterFilterElement.Create(document, ifilter, (ICollection<ElementId>)cateIds, paramFilter as ElementFilter); //ruleSet); 
				var ogs = new OverrideGraphicSettings();
				view.SetFilterOverrides(((Element)paramFilterElem).Id, ogs);
				view.SetFilterVisibility(((Element)paramFilterElem).Id, false);
				document.Regenerate();
			}
		}

		private void CreateProjectParameters(Document doc, List<ElementId> selectedCatIds)
		{
			var sharedParameterFile = doc.Application.SharedParametersFilename;
			var tempSharedParameterFile = System.IO.Path.GetTempFileName() + ".txt";
			using (System.IO.File.Create(tempSharedParameterFile)) { }
			doc.Application.SharedParametersFilename = tempSharedParameterFile;

			var parameterType = Autodesk.Revit.DB.ParameterType.YesNo;
			var parameterGroup = Autodesk.Revit.DB.BuiltInParameterGroup.PG_DISPLAY;

			var catSet = new CategorySet();
            foreach (var selectedCatId in selectedCatIds)
            {
				catSet.Insert(Category.GetCategory(doc, selectedCatId));
			}

			var bin = doc.Application.Create.NewTypeBinding(catSet);

			var definitionGroup = doc.Application.OpenSharedParameterFile().Groups.Create("BimEra");

			ExternalDefinition def = null;

			foreach (var parameterName in GetListOfParameters())
			{
				def = definitionGroup.Definitions.Create(new ExternalDefinitionCreationOptions(parameterName, parameterType)) as ExternalDefinition;
				doc.ParameterBindings.Insert(def, bin, parameterGroup);
			}

			doc.Application.SharedParametersFilename = sharedParameterFile;

			System.IO.File.Delete(tempSharedParameterFile);
		}

		private List<string> GetListOfParameters()
		{
			return new List<string>
			{
				_visibilityParameterName
			};
		}
	}
}
