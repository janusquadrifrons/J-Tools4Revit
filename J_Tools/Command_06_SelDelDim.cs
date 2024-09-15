#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using J_Tools.Extensions.SelectionExtensions;
#endregion

// Delete the Filtered / Selected Dimensions

namespace J_Tools
{
    // Declera : Tx
    [Transaction(TransactionMode.Manual)] 
    [Regeneration(RegenerationOption.Manual)] 
    
    class Command_06_SelDelDim : IExternalCommand
    {
            public Result Execute(
              ExternalCommandData commandData,
              ref string message,
              ElementSet elements)
            {
                try
                {
                    // Declerations : Accessing Revit UI and corresponding databases

                    UIApplication uiapp = commandData.Application;
                    UIDocument uidoc = uiapp.ActiveUIDocument;
                    Application app = uiapp.Application;
                    Document doc = uidoc.Document;

                    // Select with filtering 

                    var references = uidoc.Selection.PickObjects(
                        ObjectType.Element, new ElementsSelectionFilter(
                            e => e is Dimension)); // --- via this Lambda expression will allow selectiong objects that only ones classified at ElementSelectionFilter

                    /*
                    var references_2 = uidoc.Selection.PickObjects(
                        ObjectType.PointOnElement, new ElementsSelectionFilter(
                            e => e is Dimension)); // --- via this Lambda expression will allow selectiong objects that only ones classified at ElementSelectionFilter
                    */

                    // Listing objects to user

                    var formWindow = new FormWindow(references);
                    formWindow.ShowDialog();

                    // Deletion

                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("J_SelDel_Dim");
                        foreach (Reference r in references)
                        {
                            Debug.Print(r.ElementId.ToString() + "\n Deleted...!");
                            ICollection<ElementId> deletedSet = doc.Delete(r.ElementId);
                        }
                        tx.Commit();
                    }

                    return Result.Succeeded;
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) {return Result.Cancelled;}
                catch (Exception ex) { message = ex.Message; return Result.Failed; } 
                
            }
    }
}
