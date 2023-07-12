#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


//  Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using J_Tools.Extensions.SelectionExtensions;
#endregion


namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]
    public class Command_09 : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData, 
            ref string message, 
            ElementSet elements)
        {
            try
            {
                // --- Declerations : Accesing Revit Document and UI
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                // Application app = uiapp.Application;
                Document doc = uidoc.Document;

                // --- Prompt user to select a face/plane
                Reference reference = uidoc.Selection.PickObject(
                    ObjectType.Face,                     
                    "Select a plane to orient 3D View");
                Element element = doc.GetElement(reference);

                // --- Query & Action
                if (element != null)
                {
                    // --- Get necessary vectors from the planar face picked by user
                    PlanarFace planarFace = element.GetGeometryObjectFromReference(reference) as PlanarFace;

                    XYZ faceNormal = planarFace.FaceNormal;
                    XYZ faceNormalInverse = faceNormal.Negate(); // --- Reverse the normal vector
                    XYZ faceOrigin = planarFace.Origin;
                    XYZ faceVectorY = planarFace.YVector;

                    XYZ eyePosition = faceOrigin + faceNormalInverse * 10.0; // --- Set the eye position 10 units away from the face
                    XYZ upDirection = faceVectorY; // --- Set the up direction to Y vector of tha face
                    XYZ forwardDirection = faceNormalInverse; // ---  Set the forward direction

                    // --- Create a new orientation
                    ViewOrientation3D viewOrientation3D = new ViewOrientation3D(eyePosition, upDirection, forwardDirection);

                    // --- Get the active 3D view
                    View3D activeView = doc.ActiveView as View3D;

                    // --- Set the view 
                    using (Transaction tx = new Transaction (doc, "3D View to Face"))
                    {
                        tx.Start("J_3DViewTo_Face");

                        // --- Set view orientation
                        activeView.SetOrientation(viewOrientation3D);

                        // --- Refresh the active view to update the display
                        uidoc.RefreshActiveView();
                        uidoc.ShowElements(element);

                        tx.Commit();
                    }

                    TaskDialog.Show("Success", "3D View updated successfully face!");
                }
                else
                {
                    TaskDialog.Show("Error", "Please select a face!");
                }

                return Result.Succeeded;
            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Result.Cancelled; }
            catch (Exception ex) { message = ex.Message; return Result.Failed; }
        }
    }
}
