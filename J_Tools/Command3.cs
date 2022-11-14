#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]
    public class Command3 : IExternalCommand
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

            // Access current selection

            Selection sel = uidoc.Selection;

            // Retrieve elements from database

            FilteredElementCollector collection
              = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.INVALID)
                .OfClass(typeof(Wall));

            var formWindow = new FormWindow(collection);
            formWindow.ShowDialog();

            // Filtered element collector is iterable

            foreach (Element e in collection)
            {
                Debug.Print(e.Name);
            }

            // Modify document within a transaction

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("J_Trim");

                Reference refPick = uidoc.Selection.PickObject(ObjectType.Element,"Pick an element");
                Element elem = doc.GetElement(refPick);

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
