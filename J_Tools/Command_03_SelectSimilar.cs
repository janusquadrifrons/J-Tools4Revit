#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace J_Tools
{
     
    [Transaction(TransactionMode.Manual)]
    public class Command_03_SelectSimilar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the current document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Ask user to select an object
            Reference selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Select an element");
            Element selectedElement = doc.GetElement(selectedRef);

            // Get the category and parameters of the selected object
            Category selectedCategory = selectedElement.Category;
            ParameterSet selectedParameters = selectedElement.Parameters;

            // Find all elements of the same category
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategoryId(selectedCategory.Id);
            collector.WhereElementIsNotElementType(); // Filter only instances

            // Create a list to store similar objects
            List<Element> similarElements = new List<Element>();

            // Iterate through the elements
            foreach (Element elem in collector)
            {
                // Check if the element has the same parameter values as the selected element
                bool isSimilar = true;
                foreach (Parameter param in selectedParameters)
                {
                    Parameter elemParam = elem.LookupParameter(param.Definition.Name);
                    if (elemParam != null && !AreParametersEqual(param, elemParam))
                    {
                        isSimilar = false;
                        break;
                    }
                }

                if (isSimilar)
                {
                    similarElements.Add(elem);
                }
            }

            // Select the similar elements
            uidoc.Selection.SetElementIds(similarElements.Select(e => e.Id).ToList());

            TaskDialog.Show("Result", $"Found {similarElements.Count} similar objects.");

            return Result.Succeeded;
        }
        
        // Helper method - Compare two parameters
        private bool AreParametersEqual(Parameter param1, Parameter param2)
        {
            if (param1.StorageType != param2.StorageType)
            {
                return false;
            }

            switch (param1.StorageType)
            {
                case StorageType.Double:
                    return param1.AsDouble() == param2.AsDouble();
                case StorageType.ElementId:
                    return param1.AsElementId() == param2.AsElementId();
                case StorageType.Integer:
                    return param1.AsInteger() == param2.AsInteger();
                case StorageType.String:
                    return param1.AsString() == param2.AsString();
                default:
                    return false;
            }
        }
    }
}
