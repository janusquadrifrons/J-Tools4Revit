using System;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;

namespace J_Tools
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.ReadOnly)]

    /// Selection and manipulation ALT-2

     class Command7 : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            try
            {
                // Select some elements in Revit before invoking this command

                // Get the handle of current document.
                UIDocument uidoc = commandData.Application.ActiveUIDocument;

                // Get the element selection of current document.
                Selection selection = uidoc.Selection;
                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

                if (0 == selectedIds.Count)
                {
                    // If no elements selected.
                    TaskDialog.Show("Revit", "You haven't selected any elements.");
                }
                else
                {
                    String info = "Ids of selected elements in the document are: ";
                    foreach (ElementId id in selectedIds)
                    {
                        info += "\n\t" + id.IntegerValue;
                    }

                    TaskDialog.Show("Revit", info);

                    foreach (ElementId id in selectedIds)
                    {
                        selectedIds.Remove(id); // --- Collection was modified; enumeration operation may not execute. hatası döndü
                    }
                }
            }
            catch (Exception e)
            {
                message = e.Message;
                return Autodesk.Revit.UI.Result.Failed;
            }

            return Autodesk.Revit.UI.Result.Succeeded;
        }
        /// 
    }
}