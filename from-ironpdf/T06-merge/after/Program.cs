// Merge two PDF files.
// Sample: https://selectpdf.com/demo-universal/PdfMerge
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// load the 2 pdf documents
PdfDocument doc1 = new PdfDocument("invoice.pdf");
PdfDocument doc2 = new PdfDocument("accessible.pdf");

// create a new pdf document and add the pages of those 2 documents to it
PdfDocument doc = new PdfDocument();
doc.Append(doc1);
doc.Append(doc2);

// save the new document, then close all three
doc.Save(output);
doc.Close();
doc1.Close();
doc2.Close();
