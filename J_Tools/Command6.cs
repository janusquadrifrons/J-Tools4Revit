#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using System.Text;
using J_Tools.Extensions.SelectionExtensions;
#endregion

// Master Selection Filter

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    class Command6 : IExternalCommand
    {
        
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // Declerations : Accessing Revit UI and corresponding databases

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;


            var references = uidoc.Selection.PickObjects(
                ObjectType.Element, new ElementsSelectionFilter(
                    e => e is Dimension)); // --- via this Lambda expression will allow selectiong objects that only ones classified at ElementSelectionFilter
            
            /*
            var references_2 = uidoc.Selection.PickObjects(
                ObjectType.PointOnElement, new ElementsSelectionFilter(
                    e => e is Dimension)); // --- via this Lambda expression will allow selectiong objects that only ones classified at ElementSelectionFilter
            */

            return Result.Succeeded;
        }

    }
}
