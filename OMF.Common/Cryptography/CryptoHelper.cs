using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
//using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
//using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace OMF.Common.Cryptography
{
    public class CryptoHelper
    {
        public static Stream Encrypt(Stream stream, string encryptionPassword)
        {
            encryptionPassword = (encryptionPassword + "        ").Substring(0, 8);
            byte[] bytes = Encoding.UTF8.GetBytes(encryptionPassword);
            byte[] rgbIV = new byte[8]
            {
        (byte) 18,
        (byte) 52,
        (byte) 86,
        (byte) 120,
        (byte) 144,
        (byte) 171,
        (byte) 205,
        (byte) 239
            };
            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            if (stream.CanSeek)
                stream.Seek(0L, SeekOrigin.Begin);
            return (Stream)new CryptoStream(stream, cryptoServiceProvider.CreateEncryptor(bytes, rgbIV), CryptoStreamMode.Read);
        }

        public static string Encrypt(string text, string encryptionPassword)
        {
            MemoryStream memoryStream1 = new MemoryStream();
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            memoryStream1.Write(bytes, 0, bytes.Length);
            Stream stream = CryptoHelper.Encrypt((Stream)memoryStream1, encryptionPassword);
            MemoryStream memoryStream2 = new MemoryStream();
            stream.CopyTo((Stream)memoryStream2);
            return Convert.ToBase64String(memoryStream2.ToArray());
        }

        public static Stream Decrypt(Stream stream, string encryptionPassword)
        {
            encryptionPassword = (encryptionPassword + "        ").Substring(0, 8);
            byte[] bytes = Encoding.UTF8.GetBytes(encryptionPassword);
            byte[] rgbIV = new byte[8]
            {
        (byte) 18,
        (byte) 52,
        (byte) 86,
        (byte) 120,
        (byte) 144,
        (byte) 171,
        (byte) 205,
        (byte) 239
            };
            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            if (stream.CanSeek)
                stream.Seek(0L, SeekOrigin.Begin);
            return (Stream)new CryptoStream(stream, cryptoServiceProvider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Read);
        }

        public static string Decrypt(string text, string encryptionPassword)
        {
            MemoryStream memoryStream1 = new MemoryStream();
            byte[] buffer = Convert.FromBase64String(text);
            memoryStream1.Write(buffer, 0, buffer.Length);
            Stream stream = CryptoHelper.Decrypt((Stream)memoryStream1, encryptionPassword);
            MemoryStream memoryStream2 = new MemoryStream();
            stream.CopyTo((Stream)memoryStream2);
            return Encoding.Unicode.GetString(memoryStream2.ToArray());
        }

        public static string ComputeHash(byte[] byteArray, string hashAlgorithm = "SHA256", byte[] saltBytes = null)
        {
            if (saltBytes == null)
            {
                saltBytes = new byte[new Random().Next(4, 8)];
                new RNGCryptoServiceProvider().GetNonZeroBytes(saltBytes);
            }
            byte[] buffer = new byte[byteArray.Length + saltBytes.Length];
            for (int index = 0; index < byteArray.Length; ++index)
                buffer[index] = byteArray[index];
            for (int index = 0; index < saltBytes.Length; ++index)
                buffer[byteArray.Length + index] = saltBytes[index];
            if (hashAlgorithm == null)
                hashAlgorithm = "";
            string upper = hashAlgorithm.ToUpper();
            byte[] hash = (upper == "SHA1" ? (HashAlgorithm)new SHA1Managed() : (upper == "SHA256" ? (HashAlgorithm)new SHA256Managed() : (upper == "SHA384" ? (HashAlgorithm)new SHA384Managed() : (upper == "SHA512" ? (HashAlgorithm)new SHA512Managed() : (HashAlgorithm)new MD5CryptoServiceProvider())))).ComputeHash(buffer);
            byte[] inArray = new byte[hash.Length + saltBytes.Length];
            for (int index = 0; index < hash.Length; ++index)
                inArray[index] = hash[index];
            for (int index = 0; index < saltBytes.Length; ++index)
                inArray[hash.Length + index] = saltBytes[index];
            return Convert.ToBase64String(inArray);
        }

        public static string ComputeHash(string plainText, string hashAlgorithm = "SHA256", byte[] saltBytes = null)
        {
            return CryptoHelper.ComputeHash(Encoding.UTF8.GetBytes(plainText), hashAlgorithm, saltBytes);
        }

        public static bool VerifyHash(string plainText, string hashAlgorithm = "SHA256", string hashValue = null)
        {
            byte[] numArray = Convert.FromBase64String(hashValue);
            if (hashAlgorithm == null)
                hashAlgorithm = "";
            string upper = hashAlgorithm.ToUpper();
            int num = (upper == "SHA1" ? 160 : (upper == "SHA256" ? 256 : (upper == "SHA384" ? 384 : (upper == "SHA512" ? 512 : 128)))) / 8;
            if (numArray.Length < num)
                return false;
            byte[] saltBytes = new byte[numArray.Length - num];
            for (int index = 0; index < saltBytes.Length; ++index)
                saltBytes[index] = numArray[num + index];
            string hash = CryptoHelper.ComputeHash(plainText, hashAlgorithm, saltBytes);
            return hashValue == hash;
        }

        //public static byte[] SignByCertificate(byte[] data, string certSubjectName, string tokenPin)
        //{
        //    X509Certificate2 certificateBySubjectName = CryptoHelper.GetCertificateBySubjectName(certSubjectName);
        //    if (certificateBySubjectName == null)
        //        throw new Exception("The certificate could not be found");
        //    //if (!string.IsNullOrWhiteSpace(tokenPin))
        //    //    certificateBySubjectName.SetPinCode(tokenPin);
        //    XmlDocument document = new XmlDocument();
        //    document.PreserveWhitespace = true;
        //    document.Load((Stream)new MemoryStream(data));
        //    SignedXml signedXml = new SignedXml(document);
        //    signedXml.SigningKey = certificateBySubjectName.PrivateKey;
        //    Reference reference = new Reference();
        //    reference.Uri = "";
        //    XmlDsigEnvelopedSignatureTransform signatureTransform = new XmlDsigEnvelopedSignatureTransform();
        //    reference.AddTransform((Transform)signatureTransform);
        //    signedXml.AddReference(reference);
        //    KeyInfo keyInfo = new KeyInfo();
        //    keyInfo.AddClause((KeyInfoClause)new KeyInfoX509Data((X509Certificate)certificateBySubjectName));
        //    signedXml.KeyInfo = keyInfo;
        //    signedXml.ComputeSignature();
        //    XmlElement xml = signedXml.GetXml();
        //    document.DocumentElement.AppendChild(document.ImportNode((XmlNode)xml, true));
        //    MemoryStream memoryStream = new MemoryStream();
        //    document.Save((Stream)memoryStream);
        //    byte[] array = memoryStream.ToArray();
        //    memoryStream.Close();
        //    return array;
        //}

        //public static bool VerifySignedXml(byte[] data, bool rootCertificateVerify)
        //{
        //    XmlDocument document = new XmlDocument();
        //    document.PreserveWhitespace = true;
        //    document.Load((Stream)new MemoryStream(data));
        //    XmlNodeList elementsByTagName = document.GetElementsByTagName("Signature");
        //    if (elementsByTagName.Count <= 0)
        //        throw new Exception("No signature has been attached to data");
        //    if (elementsByTagName.Count >= 2)
        //        throw new Exception("The data has more than one signature");
        //    SignedXml signedXml = new SignedXml(document);
        //    signedXml.LoadXml((XmlElement)elementsByTagName[0]);
        //    AsymmetricAlgorithm signingKey = (AsymmetricAlgorithm)null;
        //    bool flag = signedXml.CheckSignatureReturningKey(out signingKey);
        //    if (flag & rootCertificateVerify)
        //    {
        //        X509Chain x509Chain = new X509Chain();
        //        X509Certificate2 certificate = signedXml.KeyInfo.OfType<KeyInfoX509Data>().First<KeyInfoX509Data>().Certificates.OfType<X509Certificate2>().First<X509Certificate2>();
        //        x509Chain.Build(certificate);
        //        x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        //        x509Chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
        //        foreach (X509ChainElement chainElement in x509Chain.ChainElements)
        //        {
        //            if (x509Chain.ChainStatus.Length != 0)
        //            {
        //                foreach (X509ChainStatus chainStatu in x509Chain.ChainStatus)
        //                {
        //                    if (!chainStatu.Status.Equals((object)X509ChainStatusFlags.RevocationStatusUnknown) && !chainStatu.Status.Equals((object)X509ChainStatusFlags.OfflineRevocation))
        //                        return false;
        //                }
        //            }
        //        }
        //    }
        //    return flag;
        //}

        //public static bool Verify(byte[] data, bool rootCertificateVerify)
        //{
        //    SignedCms signedCms = new SignedCms();
        //    signedCms.Decode(Convert.FromBase64String(Encoding.UTF8.GetString(data)));
        //    signedCms.CheckSignature(true);
        //    if (rootCertificateVerify)
        //    {
        //        X509Chain x509Chain = new X509Chain();
        //        x509Chain.Build(signedCms.SignerInfos[0].Certificate);
        //        x509Chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        //        x509Chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
        //        foreach (X509ChainElement chainElement in x509Chain.ChainElements)
        //        {
        //            if (x509Chain.ChainStatus.Length != 0)
        //            {
        //                foreach (X509ChainStatus chainStatu in x509Chain.ChainStatus)
        //                {
        //                    X509ChainStatusFlags status = chainStatu.Status;
        //                    int num;
        //                    if (!status.Equals((object)X509ChainStatusFlags.RevocationStatusUnknown))
        //                    {
        //                        status = chainStatu.Status;
        //                        num = status.Equals((object)X509ChainStatusFlags.OfflineRevocation) ? 1 : 0;
        //                    }
        //                    else
        //                        num = 1;
        //                    if (num == 0)
        //                        return false;
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}

        private static X509Certificate2 GetCertificateBySubjectName(string subjectName)
        {
            if (string.IsNullOrWhiteSpace(subjectName))
                return (X509Certificate2)null;
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            x509Store.Open(OpenFlags.OpenExistingOnly);
            X509Certificate2 x509Certificate2 = (X509Certificate2)null;
            try
            {
                if (x509Store.Certificates.Count == 0)
                {
                    x509Store.Close();
                    return x509Certificate2;
                }
                X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, (object)subjectName, false);
                if (certificate2Collection.Count > 0)
                    x509Certificate2 = certificate2Collection[0];
            }
            finally
            {
                x509Store.Close();
            }
            return x509Certificate2;
        }
    }
}
