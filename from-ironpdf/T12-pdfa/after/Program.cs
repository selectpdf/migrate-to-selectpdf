// Convert an existing PDF to PDF/A-2b.
// Docs: https://selectpdf.com/pdf-library/html/M_SelectPdf_Universal_PdfDocument_ConvertToPdfA.htm
// Sample (PDF/A at creation time): https://selectpdf.com/demo-universal/PdfAStandards
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// load the existing pdf document
PdfDocument doc = new PdfDocument("invoice.pdf");

// convert it to PDF/A-2b (IronPDF: SaveAsPdfA(path, PdfAVersions.PdfA2b))
doc.ConvertToPdfA(PdfStandard.PdfA2B);

// save and close the pdf document
doc.Save(output);
doc.Close();
