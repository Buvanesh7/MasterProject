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
    public class UpdateWsStatusInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                var isWorkShred = document.IsWorkshared;
                if (!isWorkShred)
                {
                    DisplayHelper.Display("Worksharing is not enabled !", Types.TaskDialogType.Error);
                    return Result.Failed;
                }

                var mainWindow = new UpdateWsStatusInfoWindow(document);
                mainWindow.ShowDialog();

                if (UpdateWsStatusInfoModel.ListOfCategories.Count == 0)
                    return Result.Cancelled;

                var catList = new List<ElementId>();

                UpdateWsStatusInfoModel.ListOfCategories.ForEach(x => catList.Add(x.Id));

                var catFilter = new ElementMulticategoryFilter(catList);

                var eles = new FilteredElementCollector(document).WherePasses(catFilter).WhereElementIsNotElementType().ToList();

                if(eles.Count == 0)
                {
                    DisplayHelper.Display("No elements found in the selcted category", Types.TaskDialogType.Information);
                    return Result.Cancelled;
                }

                var param1Id = GetSharedParameterId(document, "Element Created By");
                using var transaction = new Transaction(document, "Update Element Status");
                transaction.Start();
                if (param1Id == null)
                {
                    //DisplayHelper.Display("No Shared Parameter in the name of 'Created By'. Please create the shared parameter and run the command!", Types.TaskDialogType.Error);
                    //return Result.Failed;
                    SharedParameterHelper.CreateProjectParameters(document, catList);
                    param1Id = GetSharedParameterId(document, "Element Created By");
                }

                var param2Id = GetSharedParameterId(document, "Element Last Updated By");
                if (param2Id == null)
                {
                    DisplayHelper.Display("Required Shared Parameters are not created yet", Types.TaskDialogType.Error);
                    return Result.Failed;
                }

                var param3Id = GetSharedParameterId(document, "Element ID");
                if (param3Id == null)
                {
                    DisplayHelper.Display("Required Shared Parameters are not created yet", Types.TaskDialogType.Error);
                    return Result.Failed;
                }

                //var paramFilter1 = CreateParameterFilter(param1Id);
                //var paramFilter2 = CreateParameterFilter(param2Id);

                //var logicalFilter = new LogicalOrFilter(paramFilter1, paramFilter2);

                //var eles = new FilteredElementCollector(document).WherePasses(logicalFilter).ToElements();

                foreach (var ele in eles)
                {
                    var WsInfo = WorksharingUtils.GetWorksharingTooltipInfo(document, ele.Id);
                    var craetedByParam = ele.LookupParameter("Element Created By");
                    if(craetedByParam == null)
                    {
                        SharedParameterHelper.CreateProjectParameters(document, catList);
                    }
                    craetedByParam = ele.LookupParameter("Element Created By");
                    if (craetedByParam.IsReadOnly)
                        continue;
                    if (craetedByParam == null)
                        continue;
                    craetedByParam.Set(WsInfo.Creator);

                    var lastUpdatedParam = ele.LookupParameter("Element Last Updated By");
                    if (lastUpdatedParam.IsReadOnly)
                        continue;
                    if (lastUpdatedParam == null)
                        continue;
                    lastUpdatedParam.Set(WsInfo.LastChangedBy);

                    var elementIdParam = ele.LookupParameter("Element ID");
                    if (elementIdParam.IsReadOnly)
                        continue;
                    if (elementIdParam == null)
                        continue;
                    elementIdParam.Set(ele.Id.IntegerValue.ToString());
                }

                transaction.Commit();

                DisplayHelper.Display("Worksharing Information has been updated successfully!", Types.TaskDialogType.Information);

                UpdateWsStatusInfoModel.ListOfCategories = new List<ViewModels.CategoryPickerViewModel>();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
                return Result.Failed;
            }
        }
        //public ElementFilter CreateParameterFilter(ElementId paramId)
        //{
        //    var paramRule1 = ParameterFilterRuleFactory.CreateHasNoValueParameterRule(paramId);
        //    var paramFilter1 = new ElementParameterFilter(paramRule1);

        //    var paramRule2 = ParameterFilterRuleFactory.CreateHasValueParameterRule(paramId);
        //    var paramFilter2 = new ElementParameterFilter(paramRule2);

        //    var logicalFilter = new LogicalOrFilter(paramFilter1, paramFilter2);

        //    return logicalFilter;
        //}

        public ElementId GetSharedParameterId(Document document, string paramName)
        {
            var bindingMap = document.ParameterBindings;
            var it = bindingMap.ForwardIterator();
            it.Reset();

            while (it.MoveNext())
            {
                var definition = it.Key as InternalDefinition;
                var sharedParameterElement = document.GetElement(definition.Id);
                if (sharedParameterElement == null)
                    continue;

                if (definition.Name == paramName)
                    return sharedParameterElement.Id;
            }
            return null;
        }
    }
}
