// Digitally sign an existing PDF with a self-signed certificate created at run time.
// Docs: https://ironpdf.com/how-to/signing/
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using IronPdf;
using IronPdf.Signing;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

// a self-signed certificate, written to a .pfx file
using RSA rsa = RSA.Create(2048);
var request = new CertificateRequest("CN=Sample Signer", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
using X509Certificate2 selfSigned = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1));
const string password = "sample";
string pfxFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pfx");
File.WriteAllBytes(pfxFile, selfSigned.Export(X509ContentType.Pfx, password));

// Load the existing PDF document to be signed
var pdf = PdfDocument.FromFile("invoice.pdf");

// Create a PdfSignature object directly from the certificate file and password, and apply it
var signature = new PdfSignature(pfxFile, password);
pdf.Sign(signature);

// Save the signed PDF document
pdf.SaveAs(output);
File.Delete(pfxFile);
