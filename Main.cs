using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using AkryazTools.AssemblyResolvers;
using AkryazTools.ExternalCommands;
using AkryazTools.Helpers;
using AkryazTools.Properties;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace AkryazTools
{

    /// <summary>
    /// Main application entry point, representing a revit ExternalApplication
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.IExternalApplication" />
    [Regeneration(RegenerationOption.Manual)]
    public class Main : IExternalApplication
    {
        /// <summary>
        /// Called when [startup].
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// 
        private readonly AssemblyResolver _resolver = new AssemblyResolver();
        public Result OnStartup(UIControlledApplication application)
        {

            try
            {
                var thisAssemblyLocation = GetType().Assembly.Location;
                var installDir = Path.GetDirectoryName(GetType().Assembly.Location);
                var dlls = Directory.GetFiles(installDir, "*.dll", SearchOption.AllDirectories);
                var exes = Directory.GetFiles(installDir, "*.exe", SearchOption.AllDirectories);
                var files = new List<string>(dlls);
                files.AddRange(exes);

                _resolver.Register();
                foreach (var file in files)
                {
                    if (file == thisAssemblyLocation || IsRevitAssembly(file))
                        continue;

                    try
                    {
                        Assembly.LoadFrom(file);
                    }
                    catch { /* Ignored */ }

                }
            }

            catch (Exception )
            {
                return Result.Failed;
            }

            try
            {
                string pluginName = application.ActiveAddInId.GetAddInName();
                var tabName = "BIM Era";

                RevitUiHelper.AddRibbonTab(application, tabName);
                string path = Assembly.GetExecutingAssembly().Location;

                RibbonPanel generalPannel = RevitUiHelper.AddRibbonPanel(application, tabName, "General", true);
                var updateStatusButton = RevitUiHelper.AddPushButton(generalPannel, "UpdateWsStatusInfo", "Who is Owner", typeof(UpdateWsStatusInfo), path, false);
                updateStatusButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Who_is_Owner_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                updateStatusButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Who_is_Owner_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                updateStatusButton.ToolTip = "Updates Worksharing info on elements";

                var hideLinkedCategoriesButton = RevitUiHelper.AddPushButton(generalPannel, "HideLinkedCategies", "Hide Linked" + "\n" + " Categories", typeof(HideLinkedCategoriesCommand), path, false);
                hideLinkedCategoriesButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Off_LinkedGrids_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                hideLinkedCategoriesButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Off_LinkedGrids_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                hideLinkedCategoriesButton.ToolTip = "Hides linked categories in all views placed in sheets";

                var linkLessButton = RevitUiHelper.AddPushButton(generalPannel, "LinkLessButton", "Open without" +"\n" + "Links", typeof(LinklessCommand), path, true);
                linkLessButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.LinkLess_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                linkLessButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.LinkLess_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                linkLessButton.ToolTip = "Opens the selected model with all Revit links unloaded";

                var upgradeModelsButton = RevitUiHelper.AddPushButton(generalPannel, "upgradeModelsButton", "Upgrade Models", typeof(UpgradeModelCommand), path, true);
                upgradeModelsButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Upgrade_Icon_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                upgradeModelsButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Upgrade_Icon_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                upgradeModelsButton.ToolTip = "Upgrades all Revit files from the selected folder path";

                var orphanedElementsButton = RevitUiHelper.AddPushButton(generalPannel, "showOprhanedElementsCommand", "Show" + "\n" + "Orphaned Elements", typeof(ShowOrphanedElementsCommand), path, false);
                orphanedElementsButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Orphaned_Elements_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                orphanedElementsButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Orphaned_Elements_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                orphanedElementsButton.ToolTip = "Isolates Orphaned Elements of the Selected Categories";

                var orient3dViewButton = RevitUiHelper.AddPushButton(generalPannel, "Orient3dViewCommand", "Rotate 3D" + "\n" + "view", typeof(Orient3dViewCommand), path, false);
                orient3dViewButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.RoateSectionBox_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                orient3dViewButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.RoateSectionBox_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                orient3dViewButton.ToolTip = "Rotates the 3D view parallel to the selected face";

                var syncViewsButton = RevitUiHelper.AddPushButton(generalPannel, "SyncViewsCommand", "Sync Views", typeof(SyncViewsCommand), path, false);
                syncViewsButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.SyncViews_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                syncViewsButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.SyncViews_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                syncViewsButton.ToolTip = "Sync all open views with the active view";

                RibbonPanel roomPanel = RevitUiHelper.AddRibbonPanel(application, tabName, "Rooms/Spaces", true);

                //var testButton = RevitUiHelper.AddPushButton(roomPanel, "TestButton", "Test Button", typeof(TestCommand), path, false);
                //testButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.WIP_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                //testButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.WIP_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                //testButton.ToolTip = "Test Button";

                var roomToViewsButton = RevitUiHelper.AddPushButton(roomPanel, "RoomToViewsButton", "Room To" + "\n" + "3D Views", typeof(RoomToViewsCommand), path, false);
                roomToViewsButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Rooms_to_3D_Views_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                roomToViewsButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Rooms_to_3D_Views_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                roomToViewsButton.ToolTip = "Creates individual 3d views for the rooms";

                var setRoomDataButton = RevitUiHelper.AddPushButton(roomPanel, "setRoomDataButton", "Set Room Info" + "\n" + "To Elements", typeof(SetRoomDataToElementsCommand), path, false);
                setRoomDataButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Set_Room_Info_to_Elements_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                setRoomDataButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Set_Room_Info_to_Elements_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                setRoomDataButton.ToolTip = "Sets room name and number info to elements within the room";

                RibbonPanel clashCoordinationPannel = RevitUiHelper.AddRibbonPanel(application, tabName, "Clash Coordination", true);

                var pipeUpDownButton = RevitUiHelper.AddPushButton(clashCoordinationPannel, "PipeUpDown", "MEP Re-Router", typeof(PipeUpDownCommand), path, false);
                pipeUpDownButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.PipeUpDown_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                pipeUpDownButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.PipeUpDown_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                pipeUpDownButton.ToolTip = "Moves Pipe Up and Down";

                var clashToViewsButton = RevitUiHelper.AddPushButton(clashCoordinationPannel, "ClashToViews", "Clash To Views", typeof(ClashToViewsCommand), path, false);
                clashToViewsButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Clash_to_Views_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                clashToViewsButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Clash_to_Views_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                clashToViewsButton.ToolTip = "Create individual views for clashes between selected categories";

                var flagClashButton = RevitUiHelper.AddPushButton(clashCoordinationPannel, "flagClashButton", "Flag at Clash Points", typeof(FlagCalshCommand), path, false);
                flagClashButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Flag_at_Clash_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                flagClashButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Flag_at_Clash_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                flagClashButton.ToolTip = "Place flags at the clash points in the plan view";

                var snoopLinkedElementButton = RevitUiHelper.AddPushButton(clashCoordinationPannel, "snoopLinkedElement", "Investigate"+"\n"+"Linked Element", typeof(SnoopLinkedElementCommand), path, false);
                snoopLinkedElementButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Investigate_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                snoopLinkedElementButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Investigate_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                snoopLinkedElementButton.ToolTip = "Provides information about the linked element";

                RibbonPanel sixDPanel = RevitUiHelper.AddRibbonPanel(application, tabName, "6D Panel", true);
                var roomToSolidButton = RevitUiHelper.AddPushButton(sixDPanel, "RoomToSolidButton", "Room To" + "\n" + "Mass", typeof(RoomToSloidCommand), path, false);
                roomToSolidButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Rooms_to_Mass_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                roomToSolidButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Rooms_to_Mass_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                roomToSolidButton.ToolTip = "Creates solids inplace of the rooms";

                RibbonPanel cadPannel = RevitUiHelper.AddRibbonPanel(application, tabName, "CAD", true);
                var cadToLinesButton = RevitUiHelper.AddPushButton(cadPannel, "cadToLinesButton", "CAD to Lines", typeof(CadTolinesCommand), path, false);
                cadToLinesButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.CAD_to_Revit_Lines_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                cadToLinesButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.CAD_to_Revit_Lines_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                cadToLinesButton.ToolTip = "Converts Cad lines to Detail lines";

                var cadToFixtureButton = RevitUiHelper.AddPushButton(cadPannel, "cadToFixtureCommand", "CAD To Fixtures", typeof(CreateFixturesCommand), path, false);
                cadToFixtureButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.CAD_to_Fixtures_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                cadToFixtureButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.CAD_to_Fixtures_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                cadToFixtureButton.ToolTip = "Places fixtures from CAD";

                RibbonPanel excelPannel = RevitUiHelper.AddRibbonPanel(application, tabName, "Excel", true);
                var exportScheduleButton = RevitUiHelper.AddPushButton(excelPannel, "ExportExcelCommand", "Export" + "\n" + "Schedule", typeof(ExportScheduleCommand), path, false);
                exportScheduleButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.ExportExcel_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                exportScheduleButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.ExportExcel_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                exportScheduleButton.ToolTip = "Exports the active schedule to excel";

                var panleScheduleButton = RevitUiHelper.AddPushButton(excelPannel, "exportPanelSchedulesCommand", "Export Panel " + "\n" +"Schedule", typeof(ExportElectricalPanelSchedule), path, false);
                panleScheduleButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.Panel_Schedule_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                panleScheduleButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.Panel_Schedule_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                panleScheduleButton.ToolTip = "Exports all electrical panel schedules";

                RibbonPanel pharmaAccessPannel = RevitUiHelper.AddRibbonPanel(application, tabName, "Developer", true);
                var pharmaAccessButton = RevitUiHelper.AddPushButton(pharmaAccessPannel, "pharmaAccessCommand", "BIM Era", typeof(ContactUsCommand), path, false);
                pharmaAccessButton.LargeImage = Imaging.CreateBitmapSourceFromHBitmap(Resources.icon_32x32.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                pharmaAccessButton.Image = Imaging.CreateBitmapSourceFromHBitmap(Resources.icon_16x16.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                pharmaAccessButton.ToolTip = "Opens Pharma Access Website";

                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
            
        private static bool IsRevitAssembly(string filePath)
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            return name == "AdWindows" || name.Contains("RevitAPI");
        }


        /// <summary>
        /// Called when [shutdown].
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Result OnShutdown(UIControlledApplication application)
        {
            _resolver.Unregister();
            return Result.Succeeded;
        }

    }
}
