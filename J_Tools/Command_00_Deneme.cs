#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

using QRCoder;
#endregion

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]

    public class Command_00_Deneme : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            using (Transaction tx = new Transaction(doc, "Create QR Code Stamp"))
            {
                try
                {
                    tx.Start();

                    // xxx

                    tx.Commit();

                    return Result.Succeeded;
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Result.Cancelled; }
                catch (Exception ex) { message = ex.Message; return Result.Failed; }
            }
        }
    }
}
