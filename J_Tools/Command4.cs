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
#endregion

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Command4 : IExternalCommand
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

            //// Get Parameters

            /// Collecting Elements

            ////////////////////////////  ALT-1  ////////////////////////////
            
            // Declaration : ElementId Collection
            ICollection<ElementId> familyIds = uidoc.Selection.GetElementIds();

            // Iteration in collection
            List<Element> families = new List<Element>();

            foreach(ElementId familyId in familyIds)
            {
                families.Add(doc.GetElement(familyId)); // --- Reach element of this Id
            }

            ////////////////////////////  ALT-2  ////////////////////////////

            // Decelration : Reach element directly via Linq / Extension Methods
            Element element = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).First(); // --- Extension method : requires Linq

            /// Get Some Parameters

            ////////////////////////////  ALT-1  ////////////////////////////

            Parameter value_1 = element.LookupParameter("Comments");

            ////////////////////////////  ALT-2  ////////////////////////////

            string value = element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString();

            ////////////////////////////  ALT-3  ////////////////////////////
            
            var value_2 = element.GetParameters("Janus").Select(x => x.Definition.Name);
            
            /// Display

            TaskDialog.Show("Message", value);
            
            return Result.Succeeded;
        }
    }
}
