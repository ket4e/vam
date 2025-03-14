using System.Collections;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class EncryptedXml
{
	public const string XmlEncAES128KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes128";

	public const string XmlEncAES128Url = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";

	public const string XmlEncAES192KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes192";

	public const string XmlEncAES192Url = "http://www.w3.org/2001/04/xmlenc#aes192-cbc";

	public const string XmlEncAES256KeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-aes256";

	public const string XmlEncAES256Url = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";

	public const string XmlEncDESUrl = "http://www.w3.org/2001/04/xmlenc#des-cbc";

	public const string XmlEncElementContentUrl = "http://www.w3.org/2001/04/xmlenc#Content";

	public const string XmlEncElementUrl = "http://www.w3.org/2001/04/xmlenc#Element";

	public const string XmlEncEncryptedKeyUrl = "http://www.w3.org/2001/04/xmlenc#EncryptedKey";

	public const string XmlEncNamespaceUrl = "http://www.w3.org/2001/04/xmlenc#";

	public const string XmlEncRSA15Url = "http://www.w3.org/2001/04/xmlenc#rsa-1_5";

	public const string XmlEncRSAOAEPUrl = "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p";

	public const string XmlEncSHA256Url = "http://www.w3.org/2001/04/xmlenc#sha256";

	public const string XmlEncSHA512Url = "http://www.w3.org/2001/04/xmlenc#sha512";

	public const string XmlEncTripleDESKeyWrapUrl = "http://www.w3.org/2001/04/xmlenc#kw-tripledes";

	public const string XmlEncTripleDESUrl = "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";

	private Evidence documentEvidence;

	private Encoding encoding = Encoding.UTF8;

	internal Hashtable keyNameMapping = new Hashtable();

	private CipherMode mode = CipherMode.CBC;

	private PaddingMode padding = PaddingMode.ISO10126;

	private string recipient;

	private XmlResolver resolver;

	private XmlDocument document;

	public Evidence DocumentEvidence
	{
		get
		{
			return documentEvidence;
		}
		set
		{
			documentEvidence = value;
		}
	}

	public Encoding Encoding
	{
		get
		{
			return encoding;
		}
		set
		{
			encoding = value;
		}
	}

	public CipherMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			mode = value;
		}
	}

	public PaddingMode Padding
	{
		get
		{
			return padding;
		}
		set
		{
			padding = value;
		}
	}

	public string Recipient
	{
		get
		{
			return recipient;
		}
		set
		{
			recipient = value;
		}
	}

	public XmlResolver Resolver
	{
		get
		{
			return resolver;
		}
		set
		{
			resolver = value;
		}
	}

	[System.MonoTODO]
	public EncryptedXml()
	{
	}

	[System.MonoTODO]
	public EncryptedXml(XmlDocument document)
	{
		this.document = document;
	}

	[System.MonoTODO]
	public EncryptedXml(XmlDocument document, Evidence evidence)
	{
		this.document = document;
		DocumentEvidence = evidence;
	}

	public void AddKeyNameMapping(string keyName, object keyObject)
	{
		keyNameMapping[keyName] = keyObject;
	}

	public void ClearKeyNameMappings()
	{
		keyNameMapping.Clear();
	}

	public byte[] DecryptData(EncryptedData encryptedData, SymmetricAlgorithm symAlg)
	{
		if (encryptedData == null)
		{
			throw new ArgumentNullException("encryptedData");
		}
		if (symAlg == null)
		{
			throw new ArgumentNullException("symAlg");
		}
		PaddingMode paddingMode = symAlg.Padding;
		try
		{
			symAlg.Padding = Padding;
			return Transform(encryptedData.CipherData.CipherValue, symAlg.CreateDecryptor(), symAlg.BlockSize / 8, trimPadding: true);
		}
		finally
		{
			symAlg.Padding = paddingMode;
		}
	}

	public void DecryptDocument()
	{
		XmlNodeList elementsByTagName = document.GetElementsByTagName("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
		foreach (XmlNode item in elementsByTagName)
		{
			EncryptedData encryptedData = new EncryptedData();
			encryptedData.LoadXml((XmlElement)item);
			SymmetricAlgorithm decryptionKey = GetDecryptionKey(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
			ReplaceData((XmlElement)item, DecryptData(encryptedData, decryptionKey));
		}
	}

	public virtual byte[] DecryptEncryptedKey(EncryptedKey encryptedKey)
	{
		if (encryptedKey == null)
		{
			throw new ArgumentNullException("encryptedKey");
		}
		object obj = null;
		foreach (KeyInfoClause item in encryptedKey.KeyInfo)
		{
			if (item is KeyInfoName)
			{
				obj = keyNameMapping[((KeyInfoName)item).Value];
				break;
			}
		}
		return encryptedKey.EncryptionMethod.KeyAlgorithm switch
		{
			"http://www.w3.org/2001/04/xmlenc#rsa-1_5" => DecryptKey(encryptedKey.CipherData.CipherValue, (RSA)obj, fOAEP: false), 
			"http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p" => DecryptKey(encryptedKey.CipherData.CipherValue, (RSA)obj, fOAEP: true), 
			_ => DecryptKey(encryptedKey.CipherData.CipherValue, (SymmetricAlgorithm)obj), 
		};
	}

	public static byte[] DecryptKey(byte[] keyData, SymmetricAlgorithm symAlg)
	{
		if (keyData == null)
		{
			throw new ArgumentNullException("keyData");
		}
		if (symAlg == null)
		{
			throw new ArgumentNullException("symAlg");
		}
		if (symAlg is TripleDES)
		{
			return SymmetricKeyWrap.TripleDESKeyWrapDecrypt(symAlg.Key, keyData);
		}
		if (symAlg is Rijndael)
		{
			return SymmetricKeyWrap.AESKeyWrapDecrypt(symAlg.Key, keyData);
		}
		throw new CryptographicException("The specified cryptographic transform is not supported.");
	}

	[System.MonoTODO("Test this.")]
	public static byte[] DecryptKey(byte[] keyData, RSA rsa, bool fOAEP)
	{
		AsymmetricKeyExchangeDeformatter asymmetricKeyExchangeDeformatter = null;
		asymmetricKeyExchangeDeformatter = ((!fOAEP) ? ((AsymmetricKeyExchangeDeformatter)new RSAPKCS1KeyExchangeDeformatter(rsa)) : ((AsymmetricKeyExchangeDeformatter)new RSAOAEPKeyExchangeDeformatter(rsa)));
		return asymmetricKeyExchangeDeformatter.DecryptKeyExchange(keyData);
	}

	public EncryptedData Encrypt(XmlElement inputElement, string keyName)
	{
		SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
		symmetricAlgorithm.KeySize = 256;
		symmetricAlgorithm.GenerateKey();
		symmetricAlgorithm.GenerateIV();
		EncryptedData encryptedData = new EncryptedData();
		EncryptedKey encryptedKey = new EncryptedKey();
		object obj = keyNameMapping[keyName];
		encryptedKey.EncryptionMethod = new EncryptionMethod(GetKeyWrapAlgorithmUri(obj));
		if (obj is RSA)
		{
			encryptedKey.CipherData = new CipherData(EncryptKey(symmetricAlgorithm.Key, (RSA)obj, fOAEP: false));
		}
		else
		{
			encryptedKey.CipherData = new CipherData(EncryptKey(symmetricAlgorithm.Key, (SymmetricAlgorithm)obj));
		}
		encryptedKey.KeyInfo = new KeyInfo();
		encryptedKey.KeyInfo.AddClause(new KeyInfoName(keyName));
		encryptedData.Type = "http://www.w3.org/2001/04/xmlenc#Element";
		encryptedData.EncryptionMethod = new EncryptionMethod(GetAlgorithmUri(symmetricAlgorithm));
		encryptedData.KeyInfo = new KeyInfo();
		encryptedData.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));
		encryptedData.CipherData = new CipherData(EncryptData(inputElement, symmetricAlgorithm, content: false));
		return encryptedData;
	}

	[System.MonoTODO]
	public EncryptedData Encrypt(XmlElement inputElement, X509Certificate2 certificate)
	{
		throw new NotImplementedException();
	}

	public byte[] EncryptData(byte[] plainText, SymmetricAlgorithm symAlg)
	{
		if (plainText == null)
		{
			throw new ArgumentNullException("plainText");
		}
		if (symAlg == null)
		{
			throw new ArgumentNullException("symAlg");
		}
		PaddingMode paddingMode = symAlg.Padding;
		try
		{
			symAlg.Padding = Padding;
			return EncryptDataCore(plainText, symAlg);
		}
		finally
		{
			symAlg.Padding = paddingMode;
		}
	}

	private byte[] EncryptDataCore(byte[] plainText, SymmetricAlgorithm symAlg)
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(symAlg.IV);
		binaryWriter.Write(Transform(plainText, symAlg.CreateEncryptor()));
		binaryWriter.Flush();
		byte[] result = memoryStream.ToArray();
		binaryWriter.Close();
		memoryStream.Close();
		return result;
	}

	public byte[] EncryptData(XmlElement inputElement, SymmetricAlgorithm symAlg, bool content)
	{
		if (inputElement == null)
		{
			throw new ArgumentNullException("inputElement");
		}
		if (content)
		{
			return EncryptData(Encoding.GetBytes(inputElement.InnerXml), symAlg);
		}
		return EncryptData(Encoding.GetBytes(inputElement.OuterXml), symAlg);
	}

	public static byte[] EncryptKey(byte[] keyData, SymmetricAlgorithm symAlg)
	{
		if (keyData == null)
		{
			throw new ArgumentNullException("keyData");
		}
		if (symAlg == null)
		{
			throw new ArgumentNullException("symAlg");
		}
		if (symAlg is TripleDES)
		{
			return SymmetricKeyWrap.TripleDESKeyWrapEncrypt(symAlg.Key, keyData);
		}
		if (symAlg is Rijndael)
		{
			return SymmetricKeyWrap.AESKeyWrapEncrypt(symAlg.Key, keyData);
		}
		throw new CryptographicException("The specified cryptographic transform is not supported.");
	}

	[System.MonoTODO("Test this.")]
	public static byte[] EncryptKey(byte[] keyData, RSA rsa, bool fOAEP)
	{
		AsymmetricKeyExchangeFormatter asymmetricKeyExchangeFormatter = null;
		asymmetricKeyExchangeFormatter = ((!fOAEP) ? ((AsymmetricKeyExchangeFormatter)new RSAPKCS1KeyExchangeFormatter(rsa)) : ((AsymmetricKeyExchangeFormatter)new RSAOAEPKeyExchangeFormatter(rsa)));
		return asymmetricKeyExchangeFormatter.CreateKeyExchange(keyData);
	}

	private static SymmetricAlgorithm GetAlgorithm(string symAlgUri)
	{
		SymmetricAlgorithm symmetricAlgorithm = null;
		switch (symAlgUri)
		{
		case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
		case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
			symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
			symmetricAlgorithm.KeySize = 128;
			break;
		case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
		case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
			symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
			symmetricAlgorithm.KeySize = 192;
			break;
		case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
		case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
			symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
			symmetricAlgorithm.KeySize = 256;
			break;
		case "http://www.w3.org/2001/04/xmlenc#des-cbc":
			symmetricAlgorithm = SymmetricAlgorithm.Create("DES");
			break;
		case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
		case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
			symmetricAlgorithm = SymmetricAlgorithm.Create("TripleDES");
			break;
		default:
			throw new CryptographicException("symAlgUri");
		}
		return symmetricAlgorithm;
	}

	private static string GetAlgorithmUri(SymmetricAlgorithm symAlg)
	{
		if (symAlg is Rijndael)
		{
			switch (symAlg.KeySize)
			{
			case 128:
				return "http://www.w3.org/2001/04/xmlenc#aes128-cbc";
			case 192:
				return "http://www.w3.org/2001/04/xmlenc#aes192-cbc";
			case 256:
				return "http://www.w3.org/2001/04/xmlenc#aes256-cbc";
			}
		}
		else
		{
			if (symAlg is DES)
			{
				return "http://www.w3.org/2001/04/xmlenc#des-cbc";
			}
			if (symAlg is TripleDES)
			{
				return "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";
			}
		}
		throw new ArgumentException("symAlg");
	}

	private static string GetKeyWrapAlgorithmUri(object keyAlg)
	{
		if (keyAlg is Rijndael)
		{
			switch (((Rijndael)keyAlg).KeySize)
			{
			case 128:
				return "http://www.w3.org/2001/04/xmlenc#kw-aes128";
			case 192:
				return "http://www.w3.org/2001/04/xmlenc#kw-aes192";
			case 256:
				return "http://www.w3.org/2001/04/xmlenc#kw-aes256";
			}
		}
		else
		{
			if (keyAlg is RSA)
			{
				return "http://www.w3.org/2001/04/xmlenc#rsa-1_5";
			}
			if (keyAlg is TripleDES)
			{
				return "http://www.w3.org/2001/04/xmlenc#kw-tripledes";
			}
		}
		throw new ArgumentException("keyAlg");
	}

	public virtual byte[] GetDecryptionIV(EncryptedData encryptedData, string symAlgUri)
	{
		if (encryptedData == null)
		{
			throw new ArgumentNullException("encryptedData");
		}
		SymmetricAlgorithm algorithm = GetAlgorithm(symAlgUri);
		byte[] array = new byte[algorithm.BlockSize / 8];
		Buffer.BlockCopy(encryptedData.CipherData.CipherValue, 0, array, 0, array.Length);
		return array;
	}

	public virtual SymmetricAlgorithm GetDecryptionKey(EncryptedData encryptedData, string symAlgUri)
	{
		if (encryptedData == null)
		{
			throw new ArgumentNullException("encryptedData");
		}
		if (symAlgUri == null)
		{
			return null;
		}
		SymmetricAlgorithm algorithm = GetAlgorithm(symAlgUri);
		algorithm.IV = GetDecryptionIV(encryptedData, encryptedData.EncryptionMethod.KeyAlgorithm);
		KeyInfo keyInfo = encryptedData.KeyInfo;
		foreach (KeyInfoClause item in keyInfo)
		{
			if (item is KeyInfoEncryptedKey)
			{
				algorithm.Key = DecryptEncryptedKey(((KeyInfoEncryptedKey)item).EncryptedKey);
				break;
			}
		}
		return algorithm;
	}

	public virtual XmlElement GetIdElement(XmlDocument document, string idValue)
	{
		if (document == null || idValue == null)
		{
			return null;
		}
		XmlElement xmlElement = document.GetElementById(idValue);
		if (xmlElement == null)
		{
			xmlElement = (XmlElement)document.SelectSingleNode("//*[@Id='" + idValue + "']");
		}
		return xmlElement;
	}

	public void ReplaceData(XmlElement inputElement, byte[] decryptedData)
	{
		if (inputElement == null)
		{
			throw new ArgumentNullException("inputElement");
		}
		if (decryptedData == null)
		{
			throw new ArgumentNullException("decryptedData");
		}
		XmlDocument ownerDocument = inputElement.OwnerDocument;
		XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(Encoding.GetString(decryptedData, 0, decryptedData.Length)));
		xmlTextReader.MoveToContent();
		XmlNode newChild = ownerDocument.ReadNode(xmlTextReader);
		inputElement.ParentNode.ReplaceChild(newChild, inputElement);
	}

	public static void ReplaceElement(XmlElement inputElement, EncryptedData encryptedData, bool content)
	{
		if (inputElement == null)
		{
			throw new ArgumentNullException("inputElement");
		}
		if (encryptedData == null)
		{
			throw new ArgumentNullException("encryptedData");
		}
		XmlDocument ownerDocument = inputElement.OwnerDocument;
		inputElement.ParentNode.ReplaceChild(encryptedData.GetXml(ownerDocument), inputElement);
	}

	private byte[] Transform(byte[] data, ICryptoTransform transform)
	{
		return Transform(data, transform, 0, trimPadding: false);
	}

	private byte[] Transform(byte[] data, ICryptoTransform transform, int blockOctetCount, bool trimPadding)
	{
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		cryptoStream.Write(data, 0, data.Length);
		cryptoStream.FlushFinalBlock();
		int num = 0;
		if (trimPadding)
		{
			num = memoryStream.GetBuffer()[memoryStream.Length - 1];
		}
		if (num > blockOctetCount)
		{
			num = 0;
		}
		byte[] array = new byte[memoryStream.Length - blockOctetCount - num];
		Array.Copy(memoryStream.GetBuffer(), blockOctetCount, array, 0, array.Length);
		cryptoStream.Close();
		memoryStream.Close();
		return array;
	}
}
