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
    public class CadTolinesCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			var app = commandData.Application.Application;
			UIDocument uiDocument = commandData.Application.ActiveUIDocument;
			Document document = uiDocument.Document;

			try
			{
				var window = new CadToLinesWindow(document);
				window.ShowDialog();

				if (CreateDetailLinesData.CadLayers == null || CreateDetailLinesData.ImportedCadFile == null || CreateDetailLinesData.SelectedLineCategory == null)
				{
					return Result.Cancelled;
				}

				using var transaction = new Transaction(document, "Create Smoke Detectors");
				transaction.Start();

				var accView = document.ActiveView;
				var viewType = accView.ViewType;

				if(viewType != ViewType.CeilingPlan && viewType != ViewType.Detail && viewType != ViewType.DraftingView && viewType != ViewType.FloorPlan)
                {
					DisplayHelper.Display("Detail lines can not be drawn in the active view.\n" + "Please open a 2D View and run the command!", Types.TaskDialogType.Error);
					return Result.Failed;
                }

				var collector = new FilteredElementCollector(document);

				var lineCat = CreateDetailLinesData.SelectedLineCategory;

				var gs = lineCat.GetGraphicsStyle(GraphicsStyleType.Projection);

				var lineType = collector.OfCategoryId(lineCat.Id).WhereElementIsElementType().FirstOrDefault();

				var cad = CreateDetailLinesData.ImportedCadFile.CADFile;
				var geometry = cad.get_Geometry(new Options());
				var listOfPoints = new List<XYZ>();

				foreach (var geoObj in geometry)
				{
					var geoIntsance = geoObj as GeometryInstance;
					var instanceGeometry = geoIntsance.GetInstanceGeometry();
					foreach (var geo in instanceGeometry)
					{
						if (geo is PolyLine)
						{
							var stylePolyLine = document.GetElement(((PolyLine)geo).GraphicsStyleId) as GraphicsStyle;
							var stylePolyLineCategory = stylePolyLine.GraphicsStyleCategory.Name;
							if (CreateDetailLinesData.CadLayers.Any(x => x.LayerName == stylePolyLineCategory))
							{
								var points = ((PolyLine)geo).GetCoordinates();
								for (int i = 0; i < points.Count; i++)
								{
									if (i == points.Count - 1)
										break;
									try
									{
										var tempLine = Line.CreateBound(points[i], points[i + 1]);
										if (tempLine.Length <= app.ShortCurveTolerance)
											continue;
										var ps = document.Create.NewDetailCurve(document.ActiveView, tempLine);
										ps.LineStyle = gs;
									}
									catch { }
								}
							}
							continue;
						}
						var line = geo as Curve;
						if (line == null)
							continue;
						var style = document.GetElement(line.GraphicsStyleId) as GraphicsStyle;
						var styleCategory = style.GraphicsStyleCategory.Name;
						if (CreateDetailLinesData.CadLayers.Where(x => x.LayerName == styleCategory).ToList().Count > 0)
						{
							var dc = document.Create.NewDetailCurve(accView, line);
							//dc.ChangeTypeId(lineType.Id);
							dc.LineStyle = gs;
						}
					}
				}

				transaction.Commit();

				return Result.Succeeded;
			}
			catch (Exception ex)
			{
				DisplayHelper.Display(ex.Message, Types.TaskDialogType.Error);
				return Result.Cancelled;
			}
		}
		
	}
    
}
