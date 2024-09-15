// 240915_TODO : 

#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
#endregion

namespace J_Tools
{
    // Command for trim objects as in the usual way of Autocad. 
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Command_07_Trim : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the current document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // Ask user to select a "trimmer" object
                Reference trimmerRef = uidoc.Selection.PickObject(ObjectType.Element, "Select an element to trim with");
                Element trimmerElement = doc.GetElement(trimmerRef);
                GeometryElement trimmerGeometry = trimmerElement.get_Geometry(new Options());

                // Ask user to select objects to trim
                IList<Reference> objectsToTrimRefs = uidoc.Selection.PickObjects(ObjectType.Element, "Select elements to trim");

                // Start a transaction to modify the document
                using (Transaction tx = new Transaction(doc, "Trim Elements"))
                {
                    tx.Start();

                    // Iterate through objectsToTrimRefs
                    foreach (Reference refToTrimRef in objectsToTrimRefs)
                    {
                        Element elementToTrim = doc.GetElement(refToTrimRef);
                        GeometryElement trimElementGeometry = elementToTrim.get_Geometry(new Options());

                        // Find the intersection between the trimmer and the element to trim
                        Solid trimmerSolid = GetSolidFromGeometry(trimmerGeometry);
                        Solid trimSolid = GetSolidFromGeometry(trimElementGeometry);

                        // Trim the element
                        if (trimmerSolid != null && trimSolid != null)
                        {
                            BooleanOperationsUtils.ExecuteBooleanOperation(trimmerSolid, trimSolid, BooleanOperationsType.Difference);
                            doc.Delete(elementToTrim.Id);
                        }
                        else continue;
                    }

                    tx.Commit();
                }

                TaskDialog.Show("Result", "Trimming completed successfully.");
            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private Solid GetSolidFromGeometry(GeometryElement geometryElement)
        {
            foreach (GeometryObject geomObj in geometryElement)
            {
                Solid solid = geomObj as Solid;

                if (solid != null && solid.Volume > 0)
                {
                    return solid;
                }
            }
            return null;

            
        }
    }

    
}