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

    public class Command5 : IExternalCommand
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
            Element element = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).First();

            //// Set Parameters

            using (var transaction = new Transaction(doc, "Set values"))
            {
                transaction.Start();
                element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("NEW VALUE");
                transaction.Commit(); 
            }

            /////////////////////////  GET LIST OF WALLS  /////////////////////////
            
            var builtInCategoryId = new ElementId(BuiltInCategory.OST_Walls);                                               // --- Get list of obj Ids in wall category
            var builtInCategory   = Enum.GetValues(typeof(BuiltInCategory)).OfType<BuiltInCategory>();                      // --- Cast array of Ids
            var builtIncAtegory_2 = Enum.GetValues(typeof(BuiltInCategory)).OfType<BuiltInCategory>().Select(x=>(int)x);    // --- Cast array of Ids as integer
            var builtIncAtegory_3 = Enum.GetValues(typeof(BuiltInCategory)).OfType<BuiltInCategory>().Where(x => (int)x == builtInCategoryId.IntegerValue);             // --- Cast array of Ids as integer which has a specific value
            var builtIncAtegory_4 = Enum.GetValues(typeof(BuiltInCategory)).OfType<BuiltInCategory>().FirstOrDefault(x => (int)x == builtInCategoryId.IntegerValue);    // --- Casts a specific element's Ids as integer


            var window = new FormWindow(builtInCategory);
            window.ShowDialog();

            ///////////////////////////////////////////////////////////////////////



            return Result.Succeeded;
        }
    }
}
