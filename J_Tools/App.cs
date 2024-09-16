/// 230707 : TODO : button9 icon

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

            // --- Create the tab
            a.CreateRibbonTab(tabname);
            // --- Create the panel
            var panel = a.CreateRibbonPanel(tabname, panelname);

            #region Buttons
            // --- Create buttons
            var button1 = new PushButtonData("J Tools button1", "J_Trim", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_07_Trim");
            button1.ToolTip = "Trim the ACAD Way";
            button1.LongDescription = "Trim lines as in AutoCad...";
            // --- Create stacked buttons
            var button2 = new PushButtonData("J Tools button2", "J_MatchProp", Assembly.GetExecutingAssembly().Location, "J_Tools.Command2");
            button2.ToolTip = "Quick Style Painter";
            button2.LongDescription = "Match style properties of entities as in ArchiCad...";

            var button3 = new PushButtonData("J Tools button3", "J_SelectSimilar", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_03_SelectSimilar");
            button3.ToolTip = "Sibling Selector";
            button3.LongDescription = "Select similar entities with similar parameters.";

            var button4 = new PushButtonData("J Tools button4", "J_SelDelPt", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_08_SelDelPt");
            button4.ToolTip = "Delete points in selected area.";
            // button4.LongDescription = "xxx";

            var button6 = new PushButtonData("J Tools button6", "J_SelDelDim", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_06_SelDelDim");
            button6.ToolTip = "Delete dimensions in selected area.";
            // button6.LongDescription = "xxx";

            var button_09 = new PushButtonData("J Tools button_09", "J_ViewToFace", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_09_ViewToFace");
            button_09.ToolTip = "Aligns 3D view to a selected planar face.";
            // button_09.LongDescription = "xxx";

            var button_10 = new PushButtonData("J Tools button_10", "J_QRCoder", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_10_QRCoder");
            button_10.ToolTip = "Stigmatize model with a QR Code";
            button_10.LongDescription = "Run immeadiately before/after saving your model. Guarantee the authanticity of your model. Prevent vendor/contractor malpractice. ";

            var button_11 = new PushButtonData("J Tools button_11", "J_QRDecoder", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_11_QRDecoder");
            button_11.ToolTip = "Validate the QR Code Timestamp";
            button_11.LongDescription = "Check vendor/contractor malpractice of invalid versioning by catching valid QR coded timestamp.";

            var button_12 = new PushButtonData("J Tools button_12", "J_ModelChecker", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_12_ModelChecker");
            button_12.ToolTip = "Check the model for errors";
            button_12.LongDescription = "Check the model for views without view templates, views placed on deleted sheets etc...";

            var button_00_deneme = new PushButtonData("J Tools Deneme", "J_Deneme", Assembly.GetExecutingAssembly().Location, "J_Tools.Command_00_Deneme");
            #endregion


            // --- Add buttons to the panel
            PushButton btn1 = panel.AddItem(button1) as PushButton;
            //panel.AddStackedItems(button2, button3);
            //IList<RibbonItem> stackedButtons = panel.AddStackedItems(button4, button3, button6);
            PushButton btn9 = panel.AddItem(button_09) as PushButton;
            PushButton btn10 = panel.AddItem(button_10) as PushButton;
            PushButton btn11 = panel.AddItem(button_11) as PushButton;
            PushButton btn12 = panel.AddItem(button_12) as PushButton;

            PushButton btn0_deneme = panel.AddItem(button_00_deneme) as PushButton;

            // --- Creating icons
            BitmapImage icon_button1 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/trim-100c.png"));
            //BitmapImage icon_button1 = new BitmapImage(new Uri("Resources/trim-100c.png", UriKind.Relative));
            //BitmapImage icon_button1 = new BitmapImage(new Uri(@"C:\E:\98 cs\REVIT API\J_Tools\J_Tools\Resources\trim-100c.png"));
            BitmapImage icon_button2 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/injection-12.png"));
            BitmapImage icon_button3 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/multiple-12.png"));
            BitmapImage icon_button9 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/rotate-32.png"));
            BitmapImage icon_button10 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/qr-code-32.png"));
            BitmapImage icon_button11 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/qr-read-32.png"));
            BitmapImage icon_button12 = new BitmapImage(new Uri("pack://application:,,,/J_Tools;component/Resources/modelchecker-32.png"));
            
            
            // --- Assigning icons
            btn1.LargeImage = icon_button1;
            btn9.LargeImage = icon_button9;
            btn10.LargeImage = icon_button10;
            btn11.LargeImage = icon_button11;
            btn12.LargeImage = icon_button12;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

    }
}
