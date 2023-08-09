#region Namespaces
using System;
using System.Drawing;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

using System.IO;
using System.Windows;
using System.Collections.Generic;
#endregion

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]

    public class Command_00_Deneme : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // --- Accessing Revit UI and corresponding databases
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // --- List to store initial element ids.
            List<ElementId> viewsWithoutTemplates = ChechViewsWithoutTemplates(doc);

            // --- Display Results
            TaskDialog.Show("Views without templates", "There are " + viewsWithoutTemplates.Count + " views without templates in the model.");

            return Result.Succeeded;
        }

        // --- Helper function : Check for views without view templatesi
        private List<ElementId> ChechViewsWithoutTemplates(Document doc)
        {
            // --- List to store view ids without templates.
            List<ElementId> viewsWithoutTemplates = new List<ElementId>();

            // --- Get all views in the model.
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(View));

            // --- Loop over the views and check if they have view templates.
            foreach (View view in viewCollector)
            {
                // --- Check if view template is invalid.
                if (view.ViewTemplateId.IntegerValue < 0)
                {
                    viewsWithoutTemplates.Add(view.Id);
                }
            }

            // Return the list of views without templates.
            return viewsWithoutTemplates;

        }
    }
}

