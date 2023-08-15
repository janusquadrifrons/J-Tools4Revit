/* Parameter Reporter - An add-in that analyzes a model and generates a report about all the parameters, families, types, etc. Useful for understanding an unfamiliar project.
 * by extracting data from the model via API parameters. Moderate coding complexity.
 */

#region Namespaces
using System;
using System.Drawing;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]

    public class Command_00_Deneme : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            if (doc == null) { return Result.Failed; }

            /// --- Define Report data structure
            // --- We need a way to store and organize the parameter data we extract. This could be a custom class, dictionary, database table, etc.
            // --- For this example, we'll use a simple list of dictionaries. Each dictionary will represent a single parameter, and will contain the parameter name, value, type, etc.
            // --- Dictionary definition
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            // --- List definition
            List<Dictionary<string, string>> paramList = new List<Dictionary<string, string>>();
            // --- Add a dictionary to the list
            paramList.Add(paramDict);




            /// --- Get all elements in the model
            // --- Use a FilteredElementCollector to retrieve all Elements. We want to inspect every element.
            
            // --- Parameter report instance
            ParameterReport report = new ParameterReport();

            // --- Get all elements in model
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IEnumerable<Element> elementsCollector = collector.WhereElementIsNotElementType().ToElements();

            // --- Loop over elements in the collector
            foreach (Element e in elementsCollector)
            {
                // --- Get element category
                Category cat = e.Category;

                // --- Check if category already exists in report
                CategoryReport catReport = report.Categories.Find(x => x.Name == cat.Name);
                // Alternative : CategoryReport catReport = report.Categories.FirstOrDefault(cr => cr.Name == cat.Name);

                // --- If category doesn't exist, create it
                if (catReport == null)
                {
                    catReport = new CategoryReport();
                    catReport.Name = cat.Name;
                    // catReport.Families = new List<FamilyReport>();
                    report.Categories.Add(catReport);
                }



                /// --- Extract parameter data from each element
                // --- Alternative 1 : Use the built-in Element.ParametersMap property to get a ParameterMap object. This object contains all the parameters for a given element.

                // --- Alternative 2 : Use the built-in Element.GetOrderedParameters() method to get a list of all parameters for a given element.
                IList<Parameter> parameters = e.GetOrderedParameters();

                foreach (Parameter p in parameters)
                {
                    ParameterData paramData = new ParameterData();
                    paramData.Name = p.Definition.Name; // -→ via definition object of the parameter
                    paramData.Value = p.AsValueString();

                    /// --- Store / Record parameter data in the report data structure
                    // --- For each parameter, record relevant data like name, value, type, category, etc into our Report structure.
                    catReport.Parameters.Add(paramData);
                }

            }

            /// --- Process / analyze parameters
            // --- Once we have all parameter data, analyze and process it to generate useful info and statistics.
            // Things like: Duplicate names, most frequently used parameters, parameters with inconsistent naming, parameters used by certain categories, Identifying shared vs family-specific parameters



            /// --- Generate report output
            // --- Display the report data in a formatted and useful way. This could be a simple text file, a database table, a spreadsheet, a Revit schedule, etc.





            return Result.Succeeded;

        }

        // --- Alternatively, declare a data structure with independent classes
        public class ParameterReport
        {
            public List<CategoryReport> Categories { get; set; }
        }

        public class CategoryReport
        {             
            public string Name { get; set; }
            public List<FamilyReport> Families { get; set; }
            public List<ParameterData> Parameters { get; set; }
        }

        public class FamilyReport
        {
            public string Name { get; set; }
            public List<ParameterData> Parameters { get; set; }
        }

        public class ParameterData
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public ParameterType Type { get; set; }
        }
    }
}

