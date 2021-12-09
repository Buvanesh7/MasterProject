using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AkryazTools.Enum;
using AkryazTools.Helpers;
using AkryazTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.External_Events
{
    internal class PipeUpDownExternalEvent : IExternalEventHandler
    {
        public static ExternalEvent HandlerEvent = null;
        public static PipeUpDownExternalEvent HandlerInstance = null;

        public void Execute(UIApplication app)
        {

            var uidoc = app.ActiveUIDocument;
            var document = uidoc.Document;
         
            var offsetDir = PipeUpDownData.Direction;

            var value = PipeUpDownData.OffsetValue / 304.8;
            var angleOfInclination = PipeUpDownData.AngleValue * 0.0174533;

            var selectionFilter = new SelectionFilter();

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, selectionFilter, "Select an MEP component");

            if (selectedRef == null)
            {
                DisplayHelper.Display("No item seleted", Types.TaskDialogType.Warning);
                return;// Result.Cancelled;
            }

            var selectedElement = document.GetElement(selectedRef.ElementId);

            if (selectedElement.Category.Id == new ElementId(BuiltInCategory.OST_PipeCurves))
            {
                var ppHelper = new PipesUpDownHelper(uidoc);
                ppHelper.OffsetPipe(selectedElement as Pipe, value, angleOfInclination, offsetDir);
            }

            if (selectedElement.Category.Id == new ElementId(BuiltInCategory.OST_DuctCurves))
            {
                var ppHelper = new DuctUpDownHelper(uidoc);
                ppHelper.OffsetDuct(selectedElement as Duct, value, angleOfInclination, offsetDir);
            }

            if (selectedElement.Category.Id == new ElementId(BuiltInCategory.OST_CableTray))
            {
                var ppHelper = new CableTrayUpDownHelper(uidoc);
                ppHelper.OffsetCableTray(selectedElement as CableTray, value, angleOfInclination, offsetDir);
            }

            if (selectedElement.Category.Id == new ElementId(BuiltInCategory.OST_Conduit))
            {
                var ppHelper = new ConduitUpDownHelper(uidoc);
                ppHelper.OffsetConduit(selectedElement as Conduit, value, angleOfInclination, offsetDir);
            }


            PipeUpDownData.OffsetValue = 0;
            PipeUpDownData.AngleValue = 0;
        }

        public string GetName()
        {
            return "PipeUpDownExternalEvent";
        }

        public static void CreateEvent()
        {
            if (HandlerInstance == null)
            {
                HandlerInstance = new PipeUpDownExternalEvent();
                HandlerEvent = ExternalEvent.Create(HandlerInstance);
            }
        }
    }

    public class CableTrayUpDownHelper
    {
        private UIDocument uidoc;
        private Document document;

        public CableTrayUpDownHelper(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            document = uidoc.Document;
        }

        public void OffsetCableTray(CableTray cableTray, double value, double angleOfInclination, MepReRouterEnum offsetDir)
        {
            using var transaction = new Transaction(document, "MepUpDown Command");

            var selectedCt = cableTray;

            transaction.Start();

            var tanOfAngle = Math.Tan(angleOfInclination);
            var absValue = Math.Abs(value);
            var lenToDeduct = absValue / tanOfAngle;

            var ctFittings = new List<Element>();
            var ctConnectorPairs = new List<List<Connector>>();
            var ctAndFittingConnectorPair = new List<List<Connector>>();

            var ctConns = selectedCt.ConnectorManager.Connectors;

            foreach (Connector ctConn in ctConns)
            {
                var connAllRefs = ctConn.AllRefs;
                foreach (Connector connRef in connAllRefs)
                {
                    var connOwner = connRef.Owner;
                    if (connOwner.Id == selectedCt.Id)
                        continue;

                    ctFittings.Add(connOwner);
                    ctAndFittingConnectorPair.Add(new List<Connector> { ctConn, connRef });

                    var fittingConns = ((FamilyInstance)connOwner).MEPModel.ConnectorManager.Connectors;

                    foreach (Connector fittingConn in fittingConns)
                    {
                        var fittingAllRefs = fittingConn.AllRefs;
                        foreach (Connector fittingRef in fittingAllRefs)
                        {
                            var fittingConnectedTo = fittingRef.Owner;
                            if (fittingConnectedTo.Id == connOwner.Id)
                                continue;
                            if (fittingConnectedTo.Id == selectedCt.Id)
                                continue;

                            var fittingConnectedToConns = ((CableTray)fittingConnectedTo).ConnectorManager.Connectors;
                            foreach (Connector fittingConnectedToConn in fittingConnectedToConns)
                            {
                                if (fittingConnectedToConn.Origin.IsAlmostEqualTo(fittingConn.Origin))
                                    ctConnectorPairs.Add(new List<Connector> { ctConn, fittingConnectedToConn });
                            }
                        }
                    }
                }
            }

            foreach (var ctAndFittingPair in ctAndFittingConnectorPair)
            {
                ctAndFittingPair[0].DisconnectFrom(ctAndFittingPair[1]);
            }

            var locaCurve = ((LocationCurve)selectedCt.Location).Curve as Line;
            var curveDir = locaCurve.Direction.Normalize();
            var newSp = locaCurve.GetEndPoint(0).Add(curveDir * lenToDeduct);
            var newEp = locaCurve.GetEndPoint(1).Add(-curveDir * lenToDeduct);

            var newPipeLoc = Line.CreateBound(newSp, newEp);

            ((LocationCurve)selectedCt.Location).Curve = Line.CreateBound(newSp, newEp);

            switch (offsetDir)
            {
                case MepReRouterEnum.Up:
                    ElementTransformUtils.MoveElement(document, selectedCt.Id, new XYZ(0, 0, value));
                    break;
                case MepReRouterEnum.Down:
                    ElementTransformUtils.MoveElement(document, selectedCt.Id, new XYZ(0, 0, -value));
                    break;
                case MepReRouterEnum.Right:
                    var leftNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ);
                    ElementTransformUtils.MoveElement(document, selectedCt.Id, leftNormal * value);
                    break;
                case MepReRouterEnum.Left:
                    var rightNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ).Negate();
                    ElementTransformUtils.MoveElement(document, selectedCt.Id, rightNormal * value);
                    break;
                default:
                    break;
            }

            foreach (var pipeFitting in ctFittings)
            {
                document.Delete(pipeFitting.Id);
            }

            foreach (var pipeConnectorPair in ctConnectorPairs)
            {
                var tempLine = Line.CreateBound(pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin);

                var newCt = CableTray.Create(document, selectedCt.GetTypeId(), pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin, selectedCt.ReferenceLevel.Id);
                newCt.get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM).Set(selectedCt.Width);
                newCt.get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM).Set(selectedCt.Height);

                //newPipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).Set(selectedCt.Diameter);

                var newCtLocCurve = ((LocationCurve)newCt.Location).Curve as Line;
                var newCtLen = newCtLocCurve.Length;
                var deductionInLength = newCtLen * 0.2;

                var newCtCurveDir = newCtLocCurve.Direction.Normalize();
                var newCtNewSp = newCtLocCurve.GetEndPoint(0).Add(newCtCurveDir * deductionInLength);
                var newCtNewEp = newCtLocCurve.GetEndPoint(1).Add(-newCtCurveDir * deductionInLength);

                var newLoc = Line.CreateBound(newCtNewSp, newCtNewEp);

                var newCtFresh = document.GetElement(newCt.Id) as CableTray;

                var newCtConns = newCtFresh.ConnectorManager.Connectors;
                var newCtConnsList = new List<Connector>();

                foreach (Connector newCtConn in newCtConns)
                {
                    newCtConnsList.Add(newCtConn);
                    var selectedCtUpdatedConns = selectedCt.ConnectorManager.Connectors;
                    foreach (Connector selectedCtUpdatedConn in selectedCtUpdatedConns)
                    {
                        if (selectedCtUpdatedConn.Origin.IsAlmostEqualTo(newCtConn.Origin))
                            document.Create.NewElbowFitting(selectedCtUpdatedConn, newCtConn);
                    }

                    var adjCtPipe = document.GetElement(pipeConnectorPair[1].Owner.Id) as CableTray;
                    var adjCtFreshConns = adjCtPipe.ConnectorManager.Connectors;

                    foreach (Connector adjCtFreshConn in adjCtFreshConns)
                    {
                        if (adjCtFreshConn.Origin.IsAlmostEqualTo(newCtConn.Origin))
                            document.Create.NewElbowFitting(adjCtFreshConn, newCtConn);
                    }
                }
            }

            transaction.Commit();

        }
    }

    public class ConduitUpDownHelper
    {
        private Document document;
        private UIDocument uidoc;

        public ConduitUpDownHelper(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            document = uidoc.Document;
        }

        public void OffsetConduit(Conduit conduit, double value, double angleOfInclination, MepReRouterEnum offsetDir)
        {
            using var transaction = new Transaction(document, "MepUpDown Command");

            var selectedPipe = conduit;

            transaction.Start();

            //var pipe = document.GetElement(new ElementId(718045)) as Pipe;

            var tanOfAngle = Math.Tan(angleOfInclination);
            var absValue = Math.Abs(value);
            var lenToDeduct = absValue / tanOfAngle;

            var pipeFittings = new List<Element>();
            var pipeConnectorPairs = new List<List<Connector>>();
            var pipeAndFittingConnectorPair = new List<List<Connector>>();

            var pipeConns = selectedPipe.ConnectorManager.Connectors;

            foreach (Connector pipeConn in pipeConns)
            {
                var connAllRefs = pipeConn.AllRefs;
                foreach (Connector connRef in connAllRefs)
                {
                    var connOwner = connRef.Owner;
                    if (connOwner.Id == selectedPipe.Id)
                        continue;

                    pipeFittings.Add(connOwner);
                    pipeAndFittingConnectorPair.Add(new List<Connector> { pipeConn, connRef });

                    var fittingConns = ((FamilyInstance)connOwner).MEPModel.ConnectorManager.Connectors;

                    foreach (Connector fittingConn in fittingConns)
                    {
                        var fittingAllRefs = fittingConn.AllRefs;
                        foreach (Connector fittingRef in fittingAllRefs)
                        {
                            var fittingConnectedTo = fittingRef.Owner;
                            if (fittingConnectedTo.Id == connOwner.Id)
                                continue;

                            if (fittingConnectedTo.Id == selectedPipe.Id)
                                continue;

                            var fittingConnectedToConns = ((Conduit)fittingConnectedTo).ConnectorManager.Connectors;
                            foreach (Connector fittingConnectedToConn in fittingConnectedToConns)
                            {
                                if (fittingConnectedToConn.Origin.IsAlmostEqualTo(fittingConn.Origin))
                                    pipeConnectorPairs.Add(new List<Connector> { pipeConn, fittingConnectedToConn });
                            }
                        }
                    }
                }
            }

            foreach (var pipeAndFittingPair in pipeAndFittingConnectorPair)
            {
                pipeAndFittingPair[0].DisconnectFrom(pipeAndFittingPair[1]);
            }

            var locaCurve = ((LocationCurve)selectedPipe.Location).Curve as Line;
            var curveDir = locaCurve.Direction.Normalize();
            var newSp = locaCurve.GetEndPoint(0).Add(curveDir * lenToDeduct);
            var newEp = locaCurve.GetEndPoint(1).Add(-curveDir * lenToDeduct);

            var newPipeLoc = Line.CreateBound(newSp, newEp);

            ((LocationCurve)selectedPipe.Location).Curve = Line.CreateBound(newSp, newEp);

            switch (offsetDir)
            {
                case MepReRouterEnum.Up:
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, new XYZ(0, 0, value));
                    break;
                case MepReRouterEnum.Down:
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, new XYZ(0, 0, -value));
                    break;
                case MepReRouterEnum.Right:
                    var leftNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ);
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, leftNormal * value);
                    break;
                case MepReRouterEnum.Left:
                    var rightNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ).Negate();
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, rightNormal * value);
                    break;
                default:
                    break;
            }

            foreach (var pipeFitting in pipeFittings)
            {
                document.Delete(pipeFitting.Id);
            }

            foreach (var pipeConnectorPair in pipeConnectorPairs)
            {
                var tempLine = Line.CreateBound(pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin);

                var newPipe = Conduit.Create(document, selectedPipe.GetTypeId(), pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin, selectedPipe.ReferenceLevel.Id);

                newPipe.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM).Set(selectedPipe.Diameter);

                var newPipeLocCurve = ((LocationCurve)newPipe.Location).Curve as Line;
                var newPipeLen = newPipeLocCurve.Length;
                var deductionInLength = newPipeLen * 0.2;

                var newPipeCurveDir = newPipeLocCurve.Direction.Normalize();
                var newPipeNewSp = newPipeLocCurve.GetEndPoint(0).Add(newPipeCurveDir * deductionInLength);
                var newPipeNewEp = newPipeLocCurve.GetEndPoint(1).Add(-newPipeCurveDir * deductionInLength);

                var newLoc = Line.CreateBound(newPipeNewSp, newPipeNewEp);

                var newPipeFresh = document.GetElement(newPipe.Id) as Conduit;

                var newPipeConns = newPipeFresh.ConnectorManager.Connectors;
                var newPipeConnsList = new List<Connector>();

                foreach (Connector newPipeConn in newPipeConns)
                {
                    newPipeConnsList.Add(newPipeConn);
                    var selectedPipeUpdatedConns = selectedPipe.ConnectorManager.Connectors;
                    foreach (Connector selectedPipeUpdatedConn in selectedPipeUpdatedConns)
                    {
                        if (selectedPipeUpdatedConn.Origin.IsAlmostEqualTo(newPipeConn.Origin))
                            document.Create.NewElbowFitting(selectedPipeUpdatedConn, newPipeConn);
                    }

                    var adjFreshPipe = document.GetElement(pipeConnectorPair[1].Owner.Id) as Conduit;
                    var adjPipeFreshConns = adjFreshPipe.ConnectorManager.Connectors;

                    foreach (Connector adjPipeFreshConn in adjPipeFreshConns)
                    {
                        if (adjPipeFreshConn.Origin.IsAlmostEqualTo(newPipeConn.Origin))
                            document.Create.NewElbowFitting(adjPipeFreshConn, newPipeConn);
                    }
                }
            }

            transaction.Commit();
        }
    }

    public class DuctUpDownHelper
    {
        private UIDocument uidoc;
        private Document document;
        public DuctUpDownHelper(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            document = uidoc.Document;
        }

        public void OffsetDuct(Duct duct, double value, double angleOfInclination, MepReRouterEnum offsetDir)
        {
            using var transaction = new Transaction(document, "MepUpDown Command");

            var selectedPipe = duct;

            transaction.Start();

            //var pipe = document.GetElement(new ElementId(718045)) as Pipe;

            var tanOfAngle = Math.Tan(angleOfInclination);
            var absValue = Math.Abs(value);
            var lenToDeduct = absValue / tanOfAngle;

            var pipeFittings = new List<Element>();
            var pipeConnectorPairs = new List<List<Connector>>();
            var pipeAndFittingConnectorPair = new List<List<Connector>>();

            var pipeConns = selectedPipe.ConnectorManager.Connectors;


            foreach (Connector pipeConn in pipeConns)
            {
                var connAllRefs = pipeConn.AllRefs;
                foreach (Connector connRef in connAllRefs)
                {
                    var connOwner = connRef.Owner;
                    if (connOwner.Id == selectedPipe.Id)
                        continue;

                    pipeFittings.Add(connOwner);
                    pipeAndFittingConnectorPair.Add(new List<Connector> { pipeConn, connRef });

                    var fittingConns = ((FamilyInstance)connOwner).MEPModel.ConnectorManager.Connectors;

                    foreach (Connector fittingConn in fittingConns)
                    {
                        var fittingAllRefs = fittingConn.AllRefs;
                        foreach (Connector fittingRef in fittingAllRefs)
                        {
                            var fittingConnectedTo = fittingRef.Owner;
                            if (fittingConnectedTo.Id == connOwner.Id)
                                continue;

                            if (fittingConnectedTo.Id == selectedPipe.Id)
                                continue;

                            var fittingConnectedToConns = ((Duct)fittingConnectedTo).ConnectorManager.Connectors;
                            foreach (Connector fittingConnectedToConn in fittingConnectedToConns)
                            {
                                if (fittingConnectedToConn.Origin.IsAlmostEqualTo(fittingConn.Origin))
                                    pipeConnectorPairs.Add(new List<Connector> { pipeConn, fittingConnectedToConn });
                            }
                        }
                    }
                }
            }

            foreach (var pipeAndFittingPair in pipeAndFittingConnectorPair)
            {
                pipeAndFittingPair[0].DisconnectFrom(pipeAndFittingPair[1]);
            }

            var locaCurve = ((LocationCurve)selectedPipe.Location).Curve as Line;
            var curveDir = locaCurve.Direction.Normalize();
            var newSp = locaCurve.GetEndPoint(0).Add(curveDir * lenToDeduct);
            var newEp = locaCurve.GetEndPoint(1).Add(-curveDir * lenToDeduct);

            var newPipeLoc = Line.CreateBound(newSp, newEp);

            ((LocationCurve)selectedPipe.Location).Curve = Line.CreateBound(newSp, newEp);

            switch (offsetDir)
            {
                case MepReRouterEnum.Up:
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, new XYZ(0, 0, value));
                    break;
                case MepReRouterEnum.Down:
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, new XYZ(0, 0, -value));
                    break;
                case MepReRouterEnum.Right:
                    var leftNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ);
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, leftNormal * value);
                    break;
                case MepReRouterEnum.Left:
                    var rightNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ).Negate();
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, rightNormal * value);
                    break;
                default:
                    break;
            }

            var sysTypeId = selectedPipe.get_Parameter(BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM).AsElementId();

            foreach (var pipeFitting in pipeFittings)
            {
                document.Delete(pipeFitting.Id);
            }

            foreach (var pipeConnectorPair in pipeConnectorPairs)
            {
                var tempLine = Line.CreateBound(pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin);

                var newPipe = Duct.Create(document, sysTypeId, selectedPipe.GetTypeId(), selectedPipe.ReferenceLevel.Id, pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin);

                newPipe.get_Parameter(BuiltInParameter.RBS_CURVE_WIDTH_PARAM).Set(selectedPipe.Width);
                newPipe.get_Parameter(BuiltInParameter.RBS_CURVE_HEIGHT_PARAM).Set(selectedPipe.Height);

                var newPipeLocCurve = ((LocationCurve)newPipe.Location).Curve as Line;
                var newPipeLen = newPipeLocCurve.Length;
                var deductionInLength = newPipeLen * 0.2;

                var newPipeCurveDir = newPipeLocCurve.Direction.Normalize();
                var newPipeNewSp = newPipeLocCurve.GetEndPoint(0).Add(newPipeCurveDir * deductionInLength);
                var newPipeNewEp = newPipeLocCurve.GetEndPoint(1).Add(-newPipeCurveDir * deductionInLength);

                var newLoc = Line.CreateBound(newPipeNewSp, newPipeNewEp);

                var newPipeFresh = document.GetElement(newPipe.Id) as Duct;

                var newPipeConns = newPipeFresh.ConnectorManager.Connectors;
                var newPipeConnsList = new List<Connector>();

                foreach (Connector newPipeConn in newPipeConns)
                {
                    newPipeConnsList.Add(newPipeConn);
                    var selectedPipeUpdatedConns = selectedPipe.ConnectorManager.Connectors;
                    foreach (Connector selectedPipeUpdatedConn in selectedPipeUpdatedConns)
                    {
                        if (selectedPipeUpdatedConn.Origin.IsAlmostEqualTo(newPipeConn.Origin))
                            document.Create.NewElbowFitting(selectedPipeUpdatedConn, newPipeConn);
                    }

                    var adjFreshPipe = document.GetElement(pipeConnectorPair[1].Owner.Id) as Duct;
                    var adjPipeFreshConns = adjFreshPipe.ConnectorManager.Connectors;

                    foreach (Connector adjPipeFreshConn in adjPipeFreshConns)
                    {
                        if (adjPipeFreshConn.Origin.IsAlmostEqualTo(newPipeConn.Origin))
                            document.Create.NewElbowFitting(adjPipeFreshConn, newPipeConn);
                    }
                }
            }

            transaction.Commit();
        }
    }

    public class PipesUpDownHelper
    {
        private Document document;
        private UIDocument uidoc;

        public PipesUpDownHelper(UIDocument uidoc)
        {
            this.uidoc = uidoc;
            document = uidoc.Document;
        }

        public void OffsetPipe(Pipe pipe, double value, double angleOfInclination, MepReRouterEnum offsetDir)
        {
            using var transaction = new Transaction(document, "MepUpDown Command");

            var selectedPipe = pipe;

            transaction.Start();

            //var pipe = document.GetElement(new ElementId(718045)) as Pipe;

            var tanOfAngle = Math.Tan(angleOfInclination);
            var absValue = Math.Abs(value);
            var lenToDeduct = absValue / tanOfAngle;

            var pipeFittings = new List<Element>();
            var pipeConnectorPairs = new List<List<Connector>>();
            var pipeAndFittingConnectorPair = new List<List<Connector>>();
            var adjacentPipes = new List<Pipe>();

            var pipeConns = selectedPipe.ConnectorManager.Connectors;


            foreach (Connector pipeConn in pipeConns)
            {
                var connAllRefs = pipeConn.AllRefs;
                foreach (Connector connRef in connAllRefs)
                {
                    var connOwner = connRef.Owner;
                    if (connOwner.Id == selectedPipe.Id)
                        continue;

                    if (connOwner.GetType() == typeof(PipingSystem))
                        continue;

                    pipeFittings.Add(connOwner);
                    pipeAndFittingConnectorPair.Add(new List<Connector> { pipeConn, connRef });

                    var fittingConns = ((FamilyInstance)connOwner).MEPModel.ConnectorManager.Connectors;

                    foreach (Connector fittingConn in fittingConns)
                    {
                        var fittingAllRefs = fittingConn.AllRefs;
                        foreach (Connector fittingRef in fittingAllRefs)
                        {
                            var fittingConnectedTo = fittingRef.Owner;
                            if (fittingConnectedTo.Id == connOwner.Id)
                                continue;
                            if (fittingConnectedTo.GetType() == typeof(PipingSystem))
                                continue;
                            if (fittingConnectedTo.Id == selectedPipe.Id)
                                continue;

                            adjacentPipes.Add(fittingConnectedTo as Pipe);
                            var fittingConnectedToConns = ((Pipe)fittingConnectedTo).ConnectorManager.Connectors;
                            foreach (Connector fittingConnectedToConn in fittingConnectedToConns)
                            {
                                if (fittingConnectedToConn.Origin.IsAlmostEqualTo(fittingConn.Origin))
                                    pipeConnectorPairs.Add(new List<Connector> { pipeConn, fittingConnectedToConn });
                            }
                        }
                    }
                }
            }

            foreach (var pipeAndFittingPair in pipeAndFittingConnectorPair)
            {
                pipeAndFittingPair[0].DisconnectFrom(pipeAndFittingPair[1]);
            }

            var locaCurve = ((LocationCurve)selectedPipe.Location).Curve as Line;
            var curveDir = locaCurve.Direction.Normalize();
            var newSp = locaCurve.GetEndPoint(0).Add(curveDir * lenToDeduct);
            var newEp = locaCurve.GetEndPoint(1).Add(-curveDir * lenToDeduct);

            var newPipeLoc = Line.CreateBound(newSp, newEp);

            ((LocationCurve)selectedPipe.Location).Curve = Line.CreateBound(newSp, newEp);

            switch (offsetDir)
            {
                case MepReRouterEnum.Up:
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, new XYZ(0, 0, value));
                    break;
                case MepReRouterEnum.Down:
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, new XYZ(0, 0, -value));
                    break;
                case MepReRouterEnum.Right:
                    var leftNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ);
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, leftNormal * value);
                    break;
                case MepReRouterEnum.Left:
                    var rightNormal = newPipeLoc.Direction.Normalize().CrossProduct(XYZ.BasisZ).Negate();
                    ElementTransformUtils.MoveElement(document, selectedPipe.Id, rightNormal * value);
                    break;
                default:
                    break;
            }

            var sysTypeId = selectedPipe.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsElementId();

            foreach (var pipeFitting in pipeFittings)
            {
                document.Delete(pipeFitting.Id);
            }

            foreach (var pipeConnectorPair in pipeConnectorPairs)
            {
                var tempLine = Line.CreateBound(pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin);

                var newPipe = Pipe.Create(document, sysTypeId, selectedPipe.GetTypeId(), selectedPipe.ReferenceLevel.Id, pipeConnectorPair[0].Origin, pipeConnectorPair[1].Origin);

                newPipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM).Set(selectedPipe.Diameter);

                var newPipeLocCurve = ((LocationCurve)newPipe.Location).Curve as Line;
                var newPipeLen = newPipeLocCurve.Length;
                var deductionInLength = newPipeLen * 0.2;

                var newPipeCurveDir = newPipeLocCurve.Direction.Normalize();
                var newPipeNewSp = newPipeLocCurve.GetEndPoint(0).Add(newPipeCurveDir * deductionInLength);
                var newPipeNewEp = newPipeLocCurve.GetEndPoint(1).Add(-newPipeCurveDir * deductionInLength);

                var newLoc = Line.CreateBound(newPipeNewSp, newPipeNewEp);

                var newPipeFresh = document.GetElement(newPipe.Id) as Pipe;

                var newPipeConns = newPipeFresh.ConnectorManager.Connectors;
                var newPipeConnsList = new List<Connector>();

                foreach (Connector newPipeConn in newPipeConns)
                {
                    newPipeConnsList.Add(newPipeConn);
                    var selectedPipeUpdatedConns = selectedPipe.ConnectorManager.Connectors;
                    foreach (Connector selectedPipeUpdatedConn in selectedPipeUpdatedConns)
                    {
                        if (selectedPipeUpdatedConn.Origin.IsAlmostEqualTo(newPipeConn.Origin))
                            document.Create.NewElbowFitting(selectedPipeUpdatedConn, newPipeConn);
                    }

                    var adjFreshPipe = document.GetElement(pipeConnectorPair[1].Owner.Id) as Pipe;
                    var adjPipeFreshConns = adjFreshPipe.ConnectorManager.Connectors;

                    foreach (Connector adjPipeFreshConn in adjPipeFreshConns)
                    {
                        if (adjPipeFreshConn.Origin.IsAlmostEqualTo(newPipeConn.Origin))
                            document.Create.NewElbowFitting(adjPipeFreshConn, newPipeConn);
                    }
                }
            }

            transaction.Commit();
        }
    }


    public class SelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            var c1 = elem.Category.Id == new ElementId(BuiltInCategory.OST_PipeCurves);
            var c2 = elem.Category.Id == new ElementId(BuiltInCategory.OST_DuctCurves);
            var c3 = elem.Category.Id == new ElementId(BuiltInCategory.OST_CableTray);
            var c4 = elem.Category.Id == new ElementId(BuiltInCategory.OST_Conduit);


            if (c1 || c2 || c3 || c4)
                return true;

            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
