#region Namespaces
using System;
using System.Drawing;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

using ZXing;
using System.IO;
using System.Windows;
using System.Collections.Generic;
#endregion

namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]

    public class Command_11_QRDecoder : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            /// --- Declerations 
            // --- Accessing Revit UI and corresponding databases
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // --- Select with filtering
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ImageType)); // -→ Get all image types in the model.

            // --- List to store initial element ids.
            List<ElementId> initialElementIds = new List<ElementId>();

            // --- Global variables
            ImageType qrCodeImage = null;
            Bitmap bitmap = null;

            /// --- Action

            // --- Find QR Code image element in the model.
            // --- Loop through the elements in the collector. Check if there is "QRCode.png" image in the model. If it exists assign it to qrCodeImage variable.

            foreach (Element e in collector.ToElements())
            {
                if (e.Name.Contains("QRCode.png"))
                {
                    initialElementIds.Add(e.Id);
                    qrCodeImage = e as ImageType;
                    bitmap = qrCodeImage.GetImage();
                    break;
                }
            }

            using (Transaction tx = new Transaction(doc, "Create QR Code Stamp"))
            {
                try
                {
                    tx.Start();

                    // Check if the QR Code is missing.
                    if (qrCodeImage == null || IsElementDeletedDuringTransaction(doc, initialElementIds, qrCodeImage.Id))
                    {
                        TaskDialog.Show("Error", "No QR code image found.");
                        return Autodesk.Revit.UI.Result.Failed;
                    }


                    // Read the QR Code and store as a string.
                    BarcodeReader reader = new BarcodeReader();
                    var res = reader.Decode(bitmap);
                    string decodedString = res.ToString();

                    // Cast decoded string to datetime type
                    DateTime decodedDateTime = DateTime.ParseExact(decodedString, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

                    // Get last modified time of model file.
                    DateTime lastModifiedDateTime = File.GetLastWriteTime(doc.PathName);

                    // Get time difference.
                    TimeSpan timeDifference = decodedDateTime - lastModifiedDateTime;
                    double timeDifferenceInMinutes = timeDifference.TotalMinutes;

                    // If time difference is below 5 minutes, show a message box.
                    if (timeDifferenceInMinutes < 5)
                    {
                        TaskDialog.Show("QR Code", "QR Code is valid.");
                    }
                    else
                    {
                        TaskDialog.Show("QR Code", "QR Code is not valid.");
                    }

                    tx.Commit();

                    return Autodesk.Revit.UI.Result.Succeeded;
                }

                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Autodesk.Revit.UI.Result.Cancelled; }
                catch (Exception ex) { message = ex.Message; return Autodesk.Revit.UI.Result.Failed; }
            }
        }

        // Helper function : Check if the element is deleted during the transaction.
        private bool IsElementDeletedDuringTransaction(Document doc, List<ElementId> initialElementIds, ElementId elementId)
        {
            List<ElementId> currentElementIds = new List<ElementId>(); // -→ List to store current element ids.
            FilteredElementCollector collector = new FilteredElementCollector(doc); // -→ Filtered element collector to get all image types in the model.
            collector.OfClass(typeof(ImageType)); // -→ Get all image types in the model. // -→ Get all image types in the model.

            foreach (Element e in collector.ToElements())
            {
                if (e.Name.Contains("QRCode.png"))
                {
                    currentElementIds.Add(e.Id); // -→ Add current element ids to the list.
                }
            }

            return !currentElementIds.Contains(elementId) && initialElementIds.Contains(elementId); // -→ If the element is not in the current list but it is in the initial list, it means that the element is deleted during the transaction.
        }
    }
}

