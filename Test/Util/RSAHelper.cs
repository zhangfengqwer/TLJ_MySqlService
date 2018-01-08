using System;
using System.Text;
using System.Xml;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

/// <summary>
/// RSA密钥格式转换
/// </summary>
public class RSAHelper
{
    /// <summary>
    /// RSA私钥格式转换，java->.net
    /// </summary>
    /// <param name="privateKey">java生成的RSA私钥</param>
    /// <returns></returns>
    public static string RSAPrivateKeyJava2DotNet(string privateKey)
    {
        RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

        return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
            Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
            Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
    }

    /// <summary>
    /// RSA私钥格式转换，.net->java
    /// </summary>
    /// <param name="privateKey">.net生成的私钥</param>
    /// <returns></returns>
    public static string RSAPrivateKeyDotNet2Java(string privateKey)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(privateKey);
        BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
        BigInteger exp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
        BigInteger d = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("D")[0].InnerText));
        BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("P")[0].InnerText));
        BigInteger q = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
        BigInteger dp = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
        BigInteger dq = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
        BigInteger qinv = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));

        RsaPrivateCrtKeyParameters privateKeyParam = new RsaPrivateCrtKeyParameters(m, exp, d, p, q, dp, dq, qinv);

        PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);
        byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetEncoded();
        return Convert.ToBase64String(serializedPrivateBytes);
    }

    /// <summary>
    /// RSA公钥格式转换，java->.net
    /// </summary>
    /// <param name="publicKey">java生成的公钥</param>
    /// <returns></returns>
    public static string RSAPublicKeyJava2DotNet(string publicKey)
    {
        RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
        return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
            Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
            Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
    }

    /// <summary>
    /// RSA公钥格式转换，.net->java
    /// </summary>
    /// <param name="publicKey">.net生成的公钥</param>
    /// <returns></returns>
    public static string RSAPublicKeyDotNet2Java(string publicKey)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(publicKey);
        BigInteger m = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
        BigInteger p = new BigInteger(1, Convert.FromBase64String(doc.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
        RsaKeyParameters pub = new RsaKeyParameters(false, m, p);

        SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pub);
        byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
        return Convert.ToBase64String(serializedPublicBytes);
    }

    #region 私钥加密  
    /// <summary>  
    /// 基于BouncyCastle的RSA私钥加密  
    /// </summary>  
    /// <param name="privateKeyJava"></param>  
    /// <param name="data"></param>  
    /// <returns></returns>  
    public static string EncryptPrivateKeyJava(string privateKeyJava, string data, string encoding = "UTF-8")
    {
        RsaKeyParameters privateKeyParam = (RsaKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKeyJava));
        byte[] cipherbytes = Encoding.GetEncoding(encoding).GetBytes(data);
        RsaEngine rsa = new RsaEngine();
        rsa.Init(true, privateKeyParam);//参数true表示加密/false表示解密。  
        cipherbytes = rsa.ProcessBlock(cipherbytes, 0, cipherbytes.Length);
        return Convert.ToBase64String(cipherbytes);
    }
    #endregion

    #region  公钥解密  
    /// <summary>  
    /// 基于BouncyCastle的RSA公钥解密  
    /// </summary>  
    /// <param name="publicKeyJava"></param>  
    /// <param name="data"></param>  
    /// <param name="encoding"></param>  
    /// <returns></returns>  
    public static string DecryptPublicKeyJava(string publicKeyJava, string data, string encoding = "UTF-8")
    {
        RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKeyJava));
        byte[] cipherbytes = Convert.FromBase64String(data);
        RsaEngine rsa = new RsaEngine();
        rsa.Init(false, publicKeyParam);//参数true表示加密/false表示解密。  
        cipherbytes = rsa.ProcessBlock(cipherbytes, 0, cipherbytes.Length);
        return Encoding.GetEncoding(encoding).GetString(cipherbytes);
    }
    #endregion

    #region 加签 
    public static string RSASignJavaBouncyCastle(string data, string privateKeyJava, string hashAlgorithm = "SHA256WithRSA", string encoding = "UTF-8")
    {
        RsaKeyParameters privateKeyParam = (RsaKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKeyJava));
        ISigner signer = SignerUtilities.GetSigner(hashAlgorithm);
        signer.Init(true, privateKeyParam);//参数为true加签，参数为false验签  
        var dataByte = Encoding.GetEncoding(encoding).GetBytes(data);
        signer.BlockUpdate(dataByte, 0, dataByte.Length);
        //return Encoding.GetEncoding(encoding).GetString(signer.GenerateSignature()); //签名结果 非Base64String  
        return Convert.ToBase64String(signer.GenerateSignature());
    }

    #endregion
    #region 验签  
    /// <summary>  
    /// 基于BouncyCastle的RSA签名  
    /// </summary>  
    /// <param name="data">源数据</param>  
    /// <param name="publicKeyJava"></param>  
    /// <param name="signature">base64签名</param>  
    /// <param name="hashAlgorithm">JAVA的和.NET的不一样，如：MD5(.NET)等同于MD5withRSA(JAVA)</param>  
    /// <param name="encoding"></param>  
    /// <returns></returns>  
    public static bool VerifyJavaBouncyCastle(string data, string publicKeyJava, string signature, string hashAlgorithm = "SHA256WithRSA", string encoding = "UTF-8")
    {
        RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKeyJava));
        ISigner signer = SignerUtilities.GetSigner(hashAlgorithm);
        signer.Init(false, publicKeyParam);
        byte[] dataByte = Encoding.GetEncoding(encoding).GetBytes(data);
        signer.BlockUpdate(dataByte, 0, dataByte.Length);
        //byte[] signatureByte = Encoding.GetEncoding(encoding).GetBytes(signature);// 非Base64String  
        byte[] signatureByte = Convert.FromBase64String(signature);
        return signer.VerifySignature(signatureByte);
    }
    #endregion
}