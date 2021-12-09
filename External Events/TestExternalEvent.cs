using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.External_Events
{
    internal class TestExternalEvent : IExternalEventHandler
    {
        public static ExternalEvent HandlerEvent = null;
        public static TestExternalEvent HandlerInstance = null;

        public void Execute(UIApplication app)
        {
            var doc = app.ActiveUIDocument.Document;
            TaskDialog.Show("Test", "hello World");
            //var pipes = new FilteredElementCollector(doc).OfClass(typeof(Pipe)).ToElementIds();
            //app.ActiveUIDocument.Selection.SetElementIds(pipes);
        }

        public string GetName()
        {
            return "Show Message";
        }

        public static void CreateEvent()
        {
            if (HandlerInstance == null)
            {
                HandlerInstance = new TestExternalEvent();
                HandlerEvent = ExternalEvent.Create(HandlerInstance);
            }
        }
    }
}
