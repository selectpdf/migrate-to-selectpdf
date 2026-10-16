// iText Core (sign module): PdfSigner.SignDetached with a PrivateKeySignature, the key and chain read from a PKCS#12
// store with BouncyCastle, as in iText's C2_01_SignHelloWorld sample. The certificate is a self-signed one created
// here with CertificateRequest and exported to PKCS#12.
// https://github.com/itext/itext-publications-samples-dotnet/blob/develop/itext/itext.publications/itext.publications.signatures/itext/samples/signatures/chapter02/C2_01_SignHelloWorld.cs
// https://kb.itextpdf.com/itext/digital-signatures-hub
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Kernel.Crypto;
using iText.Kernel.Pdf;
using iText.Signatures;
using Org.BouncyCastle.Pkcs;

var output = args.Length > 0 ? args[0] : "output.pdf";

using var rsa = RSA.Create(2048);
var request = new CertificateRequest("CN=Sample Signer", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
using var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1));
var password = "sample";
var pkcs12 = new Pkcs12StoreBuilder().Build();
pkcs12.Load(new MemoryStream(certificate.Export(X509ContentType.Pfx, password)), password.ToCharArray());
var alias = pkcs12.Aliases.Cast<string>().First(pkcs12.IsKeyEntry);
var key = new PrivateKeyBC(pkcs12.GetKey(alias).Key);
var chain = pkcs12.GetCertificateChain(alias)
    .Select(entry => (IX509Certificate)new X509CertificateBC(entry.Certificate)).ToArray();

using var stream = new FileStream(output, FileMode.Create);
var signer = new PdfSigner(new PdfReader("invoice.pdf"), stream, new StampingProperties());
signer.SetSignerProperties(new SignerProperties().SetFieldName("Signature1").SetReason("Approved"));
signer.SignDetached(new PrivateKeySignature(key, DigestAlgorithms.SHA256), chain, null, null, null, 0,
    PdfSigner.CryptoStandard.CMS);
