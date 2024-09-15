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

    public class Command_10_QRCoder : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // --- Get the current date and time.
            DateTime currentDateTime = DateTime.Now;
            string dateTimeString = currentDateTime.ToString("yyyyMMddHHmmss");

            // --- Generate the QR code data.
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(dateTimeString, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            // --- Save Bitmap qrCodeImage to a file.
            string fileName = "E:\\QRCode.png";
            qrCodeImage.Save(fileName, ImageFormat.Png);

            using (Transaction tx = new Transaction(doc, "Create QR Code Stamp"))
            {
                try
                {
                    tx.Start();

                    // --- Check model if an image contain "QRCode" string at its name. 
                    FilteredElementCollector collector2 = new FilteredElementCollector(doc);
                    collector2.OfClass(typeof(ImageType)); // -→ Get all image types in the model.
                    List<Element> imageTypes2 = collector2.ToElements().ToList(); // -→ Convert the collector to a list.

                    foreach (Element image in imageTypes2) // -→ Loop through the list.
                    {
                        if (image.Name.Contains("QRCode")) // -→ If an image contains "QRCode" string in its name, delete it.
                        {
                            doc.Delete(image.Id);
                        }
                    }

                    // --- Get the view name of current view
                    View currentView = doc.ActiveView;

                    // --- Create an image type
                    ImageType imageType = ImageType.Create(doc, fileName);

                    // --- Get ElementId of the imageType then convert it to Element
                    ElementId imageTypeId = imageType.Id;
                    Element element = imageType as Element;

                    // --- Import image 
                    ImageImportOptions imageImportOptions = new ImageImportOptions();
                    doc.Import(fileName, imageImportOptions, currentView, out element);

                    // --- Delete QRCode.png from the disk
                    System.IO.File.Delete(fileName);

                    tx.Commit();

                    return Result.Succeeded;
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Result.Cancelled; }
                catch (Exception ex) { message = ex.Message; return Result.Failed; }
            }
        }
    }
}
