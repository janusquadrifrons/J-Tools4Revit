/// 211002 : buton iconlarý çýkmýyor.

#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Media;
#endregion

namespace J_Tools
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            string tabname = "J Tools";
            string panelname = "Janus Tools Panel";

            // --- create the tab
            a.CreateRibbonTab(tabname);
            // --- create the panel
            var panel = a.CreateRibbonPanel(tabname, panelname);
            // --- create buttons
            var button1 = new PushButtonData("J Tools button1", "J_Trim", Assembly.GetExecutingAssembly().Location, "J_Tools.Command7");
            button1.ToolTip = "Trim the ACAD Way...";
            button1.LongDescription = "Trim lines as in AutoCad...";
            // --- create stacked buttons
            var button2 = new PushButtonData("J Tools button2", "MatchProp", Assembly.GetExecutingAssembly().Location, "J_Tools.Command2");
            button2.ToolTip = "Quick Style Painter...";
            button2.LongDescription = "Match style properties of entities as in ArchiCad...";

            var button3 = new PushButtonData("J Tools button3", "J_SelectSimilar", Assembly.GetExecutingAssembly().Location, "J_Tools.Command3");
            button3.ToolTip = "Sibling Selector";
            button3.LongDescription = "Select similar entities with similar styles as in AutoCad...";

            var button4 = new PushButtonData("J Tools button4", "J_SelDel_Pt", Assembly.GetExecutingAssembly().Location, "J_Tools.Command8_J_SelDel_Pt");
            button4.ToolTip = "ooo";
            button4.LongDescription = "xxx";

            var button6 = new PushButtonData("J Tools button6", "J_SelDel_Dim", Assembly.GetExecutingAssembly().Location, "J_Tools.Command6_J_SelDel_Dim");
            button6.ToolTip = "ooo";
            button6.LongDescription = "xxx";

            var button_09 = new PushButtonData("J Tools button_09", "J_ViewToFace", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_09");
            button_09.ToolTip = "Aligns 3D view to a selected planar face.";
            button_09.LongDescription = "xxx";

            var button_00_deneme = new PushButtonData("J Tools Deneme", "J_Deneme", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_00_Deneme");




            // --- add buttons to the panel
            PushButton btn1 = panel.AddItem(button1) as PushButton;
            //panel.AddStackedItems(button2, button3);
            IList<RibbonItem> stackedButtons = panel.AddStackedItems(button4, button3, button6);
            //PushButton btn4 = panel.AddItem(button4) as PushButton;
            PushButton btn9 = panel.AddItem(button_09) as PushButton;
            // PushButton btn0_deneme = panel.AddItem(button_00_deneme) as PushButton;

            // --- creating icons
            BitmapImage icon_button1 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/trim-100c.png"));
            //BitmapImage icon_button1 = new BitmapImage(new Uri("Resources/trim-100c.png", UriKind.Relative));
            //BitmapImage icon_button1 = new BitmapImage(new Uri(@"C:\E:\98 cs\REVIT API\J_Tools\J_Tools\Resources\trim-100c.png"));
            BitmapImage icon_button2 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/injection-12.png"));
            BitmapImage icon_button3 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/multiple-12.png"));
            BitmapImage icon_button9 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/zoom-32.png"));
            // --- assigning icons
            //button1.LargeImage = icon_button1;
            btn1.LargeImage = icon_button1;
            button2.Image = icon_button2;
            button2.LargeImage = icon_button2;
            button3.Image = icon_button3;
            button3.LargeImage = icon_button3;
            button_09.LargeImage = icon_button9;
            

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

    }
}
