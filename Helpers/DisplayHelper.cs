using AkryazTools.Types;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Helpers
{
    public static class DisplayHelper
    {
        public static void Display(string message, TaskDialogType type)
        {
            string title = "";
            var icon = TaskDialogIcon.TaskDialogIconNone;

            // Customize window based on type of message.
            switch (type)
            {
                case TaskDialogType.Information:
                    title = " INFORMATION";
                    //icon = TaskDialogIcon.TaskDialogIconInformation;
                    break;
                case TaskDialogType.Warning:
                    title = " WARNING";
                    icon = TaskDialogIcon.TaskDialogIconWarning;
                    break;
                case TaskDialogType.Error:
                    title = " ERROR";
                    icon = TaskDialogIcon.TaskDialogIconWarning;
                    break;
                default:
                    break;
            }

            // Construct window to display specified message.
            var window = new TaskDialog(title)
            {
                MainContent = message,
                MainIcon = icon,
                CommonButtons = TaskDialogCommonButtons.Ok
            };
            window.Show();
        }
    }
}
