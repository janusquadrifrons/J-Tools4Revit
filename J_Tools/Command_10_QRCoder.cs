
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
using System.Drawing.Text;
using System.Drawing;

// QR Coder package
using QRCoder;
using System.Drawing.Imaging;
using System.IO;
#endregion


namespace J_Tools
{
    [Transaction(TransactionMode.Manual)]
    public class Command_10_QRCoder : IExternalCommand
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

                // --- Declerations : 
                Bitmap qrCodeImage;

                // --- Declerations :
                SharedParameterElement spe = CommandUtils.CreateSharedParameter(doc, "QRCode", ParameterType.Text, BuiltInParameterGroup.PG_TEXT, true);

                // Bind and name binding
                spe.UniqueId;
                doc.ParameterBindings.Insert(spe, doc.ProjectInformation.Name, doc.ProjectInformation);

                // --- Helper function of acquiring system date time
                string GetDateTimeString()
                {
                    DateTime currentDateTime = DateTime.Now;
                    string datetimeString = currentDateTime.ToString("yyyyMMddHHmmss");

                    return datetimeString;
                }

                // --- Helper function of generating QR code image
                Bitmap GenerateQRCode(string dateTimeString)
                {
                    // --- Create instance of QR code generator
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();

                    // --- Create QR code data
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(dateTimeString, QRCodeGenerator.ECCLevel.Q);

                    // --- Create QR code with specific size
                    QRCode qrCode = new QRCode(qrCodeData);
                    qrCode.GetGraphic(20);

                    // --- Conver QR code to bitmap image
                    Bitmap qrImage = qrCode.GetGraphic(20);

                    return qrImage;
                }

                // --- Helper function of converting image to Base64 string
                string ConvertImageToBase64(Bitmap image)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Png);
                        byte[] imageBytes = ms.ToArray();

                        return Convert.ToBase64String(imageBytes);
                    }
                }

                // --- Systen datetime call
                string dateTime = GetDateTimeString();

                // --- Generate QR Code Image
                qrCodeImage = GenerateQRCode(dateTime);

                // --- Define the QR code family
                string qrCodeFamilyPath = "C:\\ProgramData\\Autodesk\\RVT 2017\\Libraries\\US_Metric\\J_Tools\\QR Code.rfa";
                Family qrCodeFamily = doc.LoadFamily(qrCodeFamilyPath);

                // --- Family and family symbol
                FamilySymbol qrCodeSymbol = null;

                if (qrCodeFamily != null)
                {
                    qrCodeSymbol = new FamilySymbol(doc);

                    /// Look up string parameter 
                    Parameter qrCodeParam = qrCodeSymbol.LookupParameter("QRCode");

                    // Set value to the Base64 encoded image  
                    qrCodeParam.Set(ConvertImageToBase64(qrCodeImage));
                }

                return Result.Succeeded;
            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { return Result.Cancelled; }
            catch (Exception ex) { message = ex.Message; return Result.Failed; }
        }
    }
}
