using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using AkryazTools.ExternalCommands;

namespace AkryazTools.Helpers
{
    public static class RevitUiHelper
    {
        public static bool AddRibbonTab(UIControlledApplication application, string tabName)
        {
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static RibbonPanel AddRibbonPanel(UIControlledApplication application, string tabName, string panelName, bool addSeperator)
        {
            List<RibbonPanel> panels = application.GetRibbonPanels(tabName);
            RibbonPanel panel = panels.Where(x => x.Name == panelName).FirstOrDefault();

            if (panel == null)
            {
                panel = application.CreateRibbonPanel(tabName, panelName);
            }
            else if (addSeperator)
            {
                panel.AddSeparator();
            }

            return panel;
        }


        public static PushButton AddPushButton(RibbonPanel panel, string name, string title, Type targetClass, string path, bool zeroDocVisibility)
        {
            var pb = new PushButtonData(name, title, path, targetClass.FullName);
            if(zeroDocVisibility)
                pb.AvailabilityClassName = typeof(Availability).FullName;
            var button = panel.AddItem(pb) as PushButton;
            return button;
        }
    }
}