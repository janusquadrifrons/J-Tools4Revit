using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using QRCoder;

namespace RevitAddin
{
    [Transaction(TransactionMode.Manual)]
    public class AddinCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Get the current date and time
                DateTime now = DateTime.Now;

                // Generate the QR code image
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(now.ToString(), QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                BitmapByteQRCode qrCodeImage = qrCode.GetGraphic(20);

                // Get the active document and view
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;
                View view = doc.ActiveView;

                // Create a new transaction
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Add QR Code");

                    // Create a new bitmap image element
                    BitmapImage bitmapImage = BitmapImage.Create(qrCodeImage.Width, qrCodeImage.Height, 96, 96, System.Windows.Media.PixelFormats.Bgr32, null, qrCodeImage.GetGraphic(20).Data, qrCodeImage.Width * 4);

                    // Create a new image element
                    ImageType imageType = new ImageType();
                    ElementId imageTypeId = imageType.GetTypeId();
                    ImageElement imageElement = ImageElement.Create(doc, view.Id, imageTypeId, bitmapImage);

                    // Set the position of the image element
                    XYZ position = new XYZ(0, 0, 0);
                    ElementTransformUtils.MoveElement(doc, imageElement.Id, position);

                    tx.Commit();
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
