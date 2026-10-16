// Digitally sign an existing PDF with a self-signed certificate created at run time.
// Sample: https://selectpdf.com/demo-universal/DigitalSignatures
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// a self-signed certificate, written to a PKCS#12 file (the certificate store reads a file)
using RSA rsa = RSA.Create(2048);
var request = new CertificateRequest("CN=Sample Signer", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
using X509Certificate2 cert = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1));
const string password = "sample";
string pfxFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".pfx");
File.WriteAllBytes(pfxFile, cert.Export(X509ContentType.Pfx, password));

// get the #PKCS12 certificate from file
using PdfDigitalCertificatesCollection certificates = PdfDigitalCertificatesStore.GetCertificates(pfxFile, password);
File.Delete(pfxFile);

// load the document to sign
PdfDocument doc = new PdfDocument("invoice.pdf");
PdfPage page = doc.Pages[0];

// create the digital signature object and add it to the first page (IronPDF: new PdfSignature(pfx, password)
// and pdf.Sign); SelectPdf places the signature on a page, in the given rectangle (points)
PdfDigitalSignatureElement signature = new PdfDigitalSignatureElement(
    new SelectPdf.Universal.Drawing.RectangleF(0, 0, 200, 50), certificates[0]);
page.Add(signature);

// save pdf document (it is signed now) and close it
doc.Save(output);
doc.Close();
