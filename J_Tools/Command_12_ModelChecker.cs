#region Namespaces

using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Linq;

#endregion

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]

    public class Command_12_ModelChecker : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // --- Accessing Revit UI and corresponding databases
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // --- Generate report instance
            CheckReport report = new CheckReport();

            // --- Run checks
            CheckViewsWithoutTemplates(doc, report);
            CheckOrphanedViews(doc, report);

            // --- Launch report form
            ReportForm reportForm = new ReportForm(report);
            reportForm.ShowDialog();

            return Result.Succeeded;
        }

        #region Helper Functions  

        // --- Helper function : Check for views without view templates.
        private List<ElementId> CheckViewsWithoutTemplates(Document doc, CheckReport report)
        {
            // --- List to store view ids without templates.
            List<ElementId> viewsWithoutTemplates = new List<ElementId>();

            // --- Get all views in the model.
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(Autodesk.Revit.DB.View));

            // --- Loop over the views and check if they have view templates.
            foreach (Autodesk.Revit.DB.View view in viewCollector)
            {
                // --- Check if view template is invalid.
                if (view.ViewTemplateId.IntegerValue < 0)
                {
                    viewsWithoutTemplates.Add(view.Id);
                }
            }

            // --- Append to report instance
            report.ViewsWithoutTemplates = viewsWithoutTemplates.Count;

            // --- Return the list of views without templates.
            return viewsWithoutTemplates;
        }

        // --- Helper function : Check for orphaned views
        private void CheckOrphanedViews(Document doc, CheckReport report)
        {
            // --- List to store view ids without templates.
            List<ElementId> orphanedViews = new List<ElementId>();

            // --- List to store view names
            List<string> viewsList = new List<string>();

            // --- Get all views in the model.
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(Autodesk.Revit.DB.View));

            // --- Loop over the views and check if they have view templates.
            foreach (Autodesk.Revit.DB.View view in viewCollector)
            {
                // --- Get view's placed on sheet parameter
                Parameter placedOnSheet = view.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NUMBER);

                // --- Check if sheet is deleted
                if(placedOnSheet.AsInteger() == 0)
                {
                    orphanedViews.Add(view.Id);
                    viewsList.Add(view.Name);
                }
            }

            // --- Append to report instance
            report.OrphanedViews = orphanedViews.Count;

            // --- Concenate ViewList array elements and convert into a single string
            string viewsListString = string.Join(", ", viewsList);

            // --- Append to report instance
            report.ViewsListString = viewsListString;



        }

        #endregion

        // --- Class Definition : Report
        public class CheckReport
        {
            public int ViewsWithoutTemplates { get; set; }
            public int OrphanedViews { get; set; }
            public string ViewsListString { get; set; }
        }
    }
}

