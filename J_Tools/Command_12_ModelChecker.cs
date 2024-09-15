/* 230812 TODOS : Implement : Check for undefined or improperly applied materials, for rooms without closed boundaries,
 * for overlapping elements like walls and floors, for elements with invalid/corrupt geometries,
 * for elements with missing required parameters, for inconsistencies between linked models,
 * for incorrect family categories usage, for modelspace view deletions.
 * 230815 TODOS : Inconsistencty in modelchecker() : CheckOerlappingW() 
*/

#region Namespaces

using System.Collections.Generic;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Linq;
// using System.Windows.Controls;

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
            CheckAdoptedViews(doc, report);
            CheckElementsFarFromOrigin(doc, report);
            CheckRelatedWalls(doc, report);
            //CheckOverlappingWalls(doc, report);
            CheckOverlappingW(doc, report);

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
        private void CheckAdoptedViews(Document doc, CheckReport report)
        {
            // --- List to store all views in the model.
            List<string> allViews = new List<string>();
            
            // --- List to store view ids without templates.
            List<ElementId> adoptedViews = new List<ElementId>();

            // --- List to store view names
            List<string> viewsList = new List<string>();

            // --- List to store VIEWPORT_SHEET_NUMBER parameter values
            List<string> sheetNumbers = new List<string>();

            // --- Get all views in the model.
            FilteredElementCollector viewCollector = new FilteredElementCollector(doc);
            viewCollector.OfClass(typeof(Autodesk.Revit.DB.View));

            // --- Loop over the views and check if they have view templates.
            foreach (Autodesk.Revit.DB.View view in viewCollector)
            {
                // --- Add view name to allViews list
                allViews.Add(view.Name);

                // --- Get view's placed on sheet parameter
                Parameter placedOnSheet = view.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NUMBER);

                // --- Check if sheet is deleted
                //if(placedOnSheet.AsInteger() == 0)
                if(/*placedOnSheet != null &&*/ placedOnSheet.HasValue /*&& placedOnSheet.AsString() == ""*/) // -→ Another way to check for empty sheet number
                {
                    adoptedViews.Add(view.Id);
                    viewsList.Add(view.Name);
                    sheetNumbers.Add(placedOnSheet.AsString());
                }
            }

            // --- Cooncenate AllViews array elements and convert into a single string
            string allViewsString = string.Join(", ", allViews);

            // --- Append to report instance
            report.AllViewsString = allViewsString;

            // --- Append to report instance
            report.AdoptedViews = adoptedViews.Count;

            // --- Concenate ViewList array elements and convert into a single string
            string viewsListString = string.Join(", ", viewsList);

            // --- Append to report instance
            report.ViewsListString = viewsListString;

            // --- Concenate SheetNumbers array elements and convert into a single string
            string sheetNumbersString = string.Join(", ", sheetNumbers);

            // --- Append to report instance
            report.SheetNumbersString = sheetNumbersString;
        }

        // --- Helper function : Check for elements far from origin
        private void CheckElementsFarFromOrigin(Document doc, CheckReport report)
        {
            // --- List to store elements far from origin
            List<ElementId> elementsFarFromOrigin = new List<ElementId>();

            // --- Get all elements using FilteredElementCollector
            FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();

            // --- Loop through the elements
            foreach (Element e in collector)
            {
                // --- Get element's location
                Location loc = e.Location;

                // --- Check if element has a locationElementsFarFromOriginNames
                if (loc != null)
                {
                    // --- Get element's location point
                    LocationPoint lp = loc as LocationPoint;
                    LocationCurve lc = loc as LocationCurve;

                    // --- Check if element's location is a point
                    if (lp != null)
                    {
                        // --- Get element's location point
                        XYZ p = lp.Point;

                        // --- Check if element's location point is far from origin
                        if (p.X > 100000 || p.Y > 100000 || p.Z > 100000)
                        {
                            // --- Add element to list
                            elementsFarFromOrigin.Add(e.Id);

                        }
                    }
                    else if (lc != null)
                    {
                        // --- Get element's location curve
                        Curve c = lc.Curve; // -→ May be the curve is unbound

                        // --- Check if element's location curve is bound
                        if (c.IsBound)
                        {
                            // --- Check if element's location curve is far from origin
                            if (c.GetEndPoint(0).X > 1000 || c.GetEndPoint(0).Y > 1000 || c.GetEndPoint(0).Z > 1000) // -→ index 0 : starting point, index 1 : end point
                            {
                                // --- Add element to list
                                elementsFarFromOrigin.Add(e.Id);
                            }
                        }
                    }
                }
            }

            // --- Concenate element list to string
            string elementsFarFromOriginIds = string.Join(", ", elementsFarFromOrigin);

            // --- Append to report instance
            report.ElementsFarFromOrigin = elementsFarFromOrigin.Count;
            report.ElementsFarFromOriginIds = elementsFarFromOriginIds;
        }

        // --- Helper function : Check overlapping elements
        private void CheckRelatedWalls(Document doc, CheckReport report)
        {
            // --- List to store overlapping elements
            HashSet<ElementId> relatedWallElementIds = new HashSet<ElementId>();

            // --- Get all wall & floor elements of document
            FilteredElementCollector collectorWall = new FilteredElementCollector(doc).OfClass(typeof(Wall));

            // --- Loop through the wall elements collectorWall
            foreach(Element e in collectorWall)
            {
                // --- Get element's bounding box
                BoundingBoxXYZ bb = e.get_BoundingBox(null);

                // --- Get element's bounding box outline
                Outline outline = new Outline(bb.Min, bb.Max);

                // --- Get element's bounding box outline intersection
                BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);

                // --- Get all elements intersecting with the element's bounding box outline
                FilteredElementCollector relatedElements = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();

                // --- Loop through the intersecting elements collectorWall
                foreach(Element i in relatedElements)
                {
                    // --- Check if element is not the same as the intersecting element
                    if(e.Id != i.Id)
                    {
                        // --- Add element to list
                        relatedWallElementIds.Add(e.Id);
                    }
                }
            }  
            
            // --- Concenate element list to string
            string relatedWallElementIdsString = string.Join(", ", relatedWallElementIds);

            // --- Append to report instance
            report.RelatedWalls = relatedWallElementIds.Count;
            report.RelatedWallsIds = relatedWallElementIdsString;

        }

        // --- Helper function : Check overlapping walls & floors

        private void CheckOverlappingWalls(Document doc, CheckReport report)
        {
            // --- List to store overlapping elements
            HashSet<ElementId> overlappingWallSolids = new HashSet<ElementId>();

            // --- Get all wall & floor elements of document in the same collector
            FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(Wall));
            // --- Get all elements into a list in order to use with foreach loops multpile times : Otherwise Collector does not allowed to iterate twice.
            List<Element> elements = collector.ToList();

            // --- Loop through the wall elements of the collector
            foreach(Element e1 in elements)
            // --- if e1 is a wall
            if (e1 is Wall)
                {
                    // --- For each wall, get its Geometry property to access the solid/volume
                    GeometryElement geomElem1 = e1.get_Geometry(new Options());
                    // --- Loop through the geometry elements
                    foreach (GeometryObject geomObj1 in geomElem1)
                    {
                        // --- Check if geometry element is a solid
                        Solid geomSolid1 = geomObj1 as Solid;
                        if (geomSolid1 != null)
                        {
                           foreach (Element e2 in elements)
                            {
                                if (e2 is Wall && e1.Id != e2.Id)
                                {
                                    // --- For each wall, get its Geometry property to access the solid/volume
                                    GeometryElement geomElem2 = e2.get_Geometry(new Options());
                                    // --- Loop thru the geomrtry elements
                                    foreach (GeometryObject geomObj2 in geomElem2)
                                    {
                                        Solid geomSolid2 = geomObj2 as Solid;
                                        if (geomSolid2 != null)
                                        {
                                            Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(geomSolid1, geomSolid2, BooleanOperationsType.Intersect);
                                            if (intersection != null && intersection.Volume > 0)
                                            {
                                                overlappingWallSolids.Add(e1.Id);
                                                overlappingWallSolids.Add(e2.Id);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            
            
            // --- Concenate element list to string
            string overlappingWallElementIdsString = string.Join(", ", overlappingWallSolids);

            // --- Append to report instance
            report.OverlappingWallElements = overlappingWallSolids.Count;
            report.OverlappingWallElementsIds = overlappingWallElementIdsString;

        }

        // --- Helper function : Get elements of wall type
        List<Element> GetWalls(Document doc)
        {
            return new FilteredElementCollector(doc)
                .WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(Wall)).ToList();
        }

        // --- Helper function : Validate solid
        Solid GetSolid(Element e)
        {
            // --- Get element's geometry
            GeometryElement geoElem = e.get_Geometry(new Options());

            // --- Loop through the geometry elements
            foreach (GeometryObject geoObj in geoElem)
            {
                Solid solid = geoObj as Solid;
                if (solid != null && solid.Volume > 0)
                {
                    return solid;
                }
            }   
            return null;
        }

        void CheckOverlappingW(Document doc, CheckReport report)
        {
            List<Element> walls = GetWalls(doc);
            HashSet<ElementId> overlappingWallElementIds = new HashSet<ElementId>();

            foreach (Element e1 in walls)
            {
                Solid solid1 = GetSolid(e1);
                if(solid1 != null)
                {
                    foreach (Element e2 in walls)
                    {
                        if(e1.Id != e2.Id)
                        {
                            Solid solid2 = GetSolid(e2);
                            if(solid2 != null)
                            {
                                // --- Check if solid1 and solid2 intersects
                                Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
                                if(intersection != null && intersection.Volume > 0)
                                {
                                    // --- Add element to list
                                    overlappingWallElementIds.Add(e1.Id);
                                    overlappingWallElementIds.Add(e2.Id);
                                }
                            }
                        }
                    }
                }
            }

            // --- Concenate element list to string
            string overlappingWallElementIdsString = string.Join(", ", overlappingWallElementIds);

            // --- Append to report instance
            report.OverlappingWallElements = overlappingWallElementIds.Count;
            report.OverlappingWallElementsIds = overlappingWallElementIdsString;

        }


        #endregion

        // --- Class Definition : Report
        public class CheckReport
        {
            public int ViewsWithoutTemplates { get; set; }
            public string AllViewsString { get; set; }
            public int AdoptedViews { get; set; }
            public string ViewsListString { get; set; }
            public string SheetNumbersString { get; set; }
            public int ElementsFarFromOrigin { get; set; }
            public string ElementsFarFromOriginIds { get; set; }
            public int RelatedWalls { get; set; }
            public string RelatedWallsIds { get; set; }
            public int OverlappingWallElements { get; set; }
            public string OverlappingWallElementsIds { get; set; }
        }
    }
}

