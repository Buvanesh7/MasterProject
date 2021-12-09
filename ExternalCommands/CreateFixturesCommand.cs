using AkryazTools.Helpers;
using AkryazTools.Models;
using AkryazTools.Views;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.ExternalCommands
{
    [Transaction(TransactionMode.Manual)]
    public class CreateFixturesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDocument = commandData.Application.ActiveUIDocument;
            Document document = uiDocument.Document;

            var placedSmokeDetectors = new List<FamilyInstance>();

            try
            {
                var window = new CreateFixturesWindow(document);
                window.ShowDialog();

                if (CreateFixtureData.CadLayers == null || CreateFixtureData.ImportedCadFile == null || CreateFixtureData.SelectedFamilySymbol == null)
                {
                    return Result.Cancelled;
                }

                using var transaction = new Transaction(document, "Create Smoke Detectors");
                transaction.Start();

                var collector = new FilteredElementCollector(document);

                var ceilingFilter = new ElementCategoryFilter(BuiltInCategory.OST_Ceilings);

                var fixtureSymbol = CreateFixtureData.SelectedFamilySymbol as FamilySymbol;
                if (fixtureSymbol == null)
                {
                    transaction.RollBack();
                    return Result.Cancelled;
                }

                if (!fixtureSymbol.IsActive)
                    fixtureSymbol.Activate();

                var types = collector.OfClass(typeof(ViewFamilyType)).OfType<ViewFamilyType>().ToList();
                var view3Dtype = types.Where(x => x.ViewFamily == ViewFamily.ThreeDimensional).FirstOrDefault();
                var tpId = view3Dtype.Id;
                var temp3Dview = View3D.CreateIsometric(document, tpId);
                var cad = CreateFixtureData.ImportedCadFile.CADFile;
                var geometry = cad.get_Geometry(new Options());
                var listOfPoints = new List<XYZ>();

                foreach (var geoObj in geometry)
                {
                    var geoIntsance = geoObj as GeometryInstance;
                    var instanceGeometry = geoIntsance.GetInstanceGeometry();
                    foreach (var geo in instanceGeometry)
                    {
                        var line = geo as Arc;
                        if (line == null)
                            continue;
                        var style = document.GetElement(line.GraphicsStyleId) as GraphicsStyle;
                        var styleCategory = style.GraphicsStyleCategory.Name;
                        if (CreateFixtureData.CadLayers.Where(x => x.LayerName == styleCategory).ToList().Count > 0)
                        {
                            var midPoint = line.Center;
                            if (listOfPoints.Where(x => x.IsAlmostEqualTo(midPoint)).ToList().Count == 0)
                                listOfPoints.Add(midPoint);
                        }
                    }
                }
                foreach (var point in listOfPoints)
                {
                    var reference = GetIntersectionPoint(document, ceilingFilter, FindReferenceTarget.Element, temp3Dview, point);

                    if (reference == null)
                        continue;

                    var ceilingId = reference.ElementId;
                    var ceiling = document.GetElement(ceilingId);
                    var level = document.GetElement(ceiling.LevelId) as Level;
                    var intersectionPoint = (reference.GlobalPoint) as XYZ;

                    var offset = intersectionPoint.Z - level.Elevation;

                    if (fixtureSymbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBased)
                    {
                        var instance = document.Create.NewFamilyInstance(intersectionPoint, fixtureSymbol, level, StructuralType.NonStructural);
                        var param = instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                        if (!param.IsReadOnly)
                            param.Set(offset);
                        placedSmokeDetectors.Add(instance);
                    }
                    else if (fixtureSymbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBasedHosted)
                    {
                        var instance = document.Create.NewFamilyInstance(intersectionPoint, fixtureSymbol, ceiling, level, StructuralType.NonStructural);
                        var param = instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                        if (!param.IsReadOnly)
                            param.Set(offset);
                        placedSmokeDetectors.Add(instance);
                    }
                    else if (fixtureSymbol.Family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)
                    {
                        reference = GetIntersectionPoint(document, ceilingFilter, FindReferenceTarget.Face, temp3Dview, point);
                        intersectionPoint = (reference.GlobalPoint) as XYZ;
                        var instance = document.Create.NewFamilyInstance(reference, intersectionPoint, XYZ.BasisX, fixtureSymbol);
                        placedSmokeDetectors.Add(instance);
                    }
                    else
                    {
                        DisplayHelper.Display("Family Type Not Supported", Types.TaskDialogType.Error);
                        transaction.RollBack();
                        return Result.Failed;
                    }

                }

                document.Delete(temp3Dview.Id);

                DisplayHelper.Display(placedSmokeDetectors.Count.ToString() + " Elements Created!", Types.TaskDialogType.Information);
                transaction.Commit();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
                return Result.Cancelled;
            }
        }

        private Reference GetIntersectionPoint(Document document, ElementCategoryFilter ceilingFilter, FindReferenceTarget type, View3D temp3Dview, XYZ point)
        {
            var ceilingIntersector = new ReferenceIntersector(ceilingFilter, type, temp3Dview);
            ceilingIntersector.FindReferencesInRevitLinks = false;

            var referenceZAxis = ceilingIntersector.FindNearest(point, XYZ.BasisZ);
            if (referenceZAxis == null)
                return null;

            var reference = referenceZAxis.GetReference() as Reference;
            return reference;
        }

    }
}
