using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class SignedXml
{
	public const string XmlDsigCanonicalizationUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

	public const string XmlDsigCanonicalizationWithCommentsUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

	public const string XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";

	public const string XmlDsigHMACSHA1Url = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";

	public const string XmlDsigMinimalCanonicalizationUrl = "http://www.w3.org/2000/09/xmldsig#minimal";

	public const string XmlDsigNamespaceUrl = "http://www.w3.org/2000/09/xmldsig#";

	public const string XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

	public const string XmlDsigSHA1Url = "http://www.w3.org/2000/09/xmldsig#sha1";

	public const string XmlDecryptionTransformUrl = "http://www.w3.org/2002/07/decrypt#XML";

	public const string XmlDsigBase64TransformUrl = "http://www.w3.org/2000/09/xmldsig#base64";

	public const string XmlDsigC14NTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

	public const string XmlDsigC14NWithCommentsTransformUrl = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

	public const string XmlDsigEnvelopedSignatureTransformUrl = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

	public const string XmlDsigExcC14NTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#";

	public const string XmlDsigExcC14NWithCommentsTransformUrl = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";

	public const string XmlDsigXPathTransformUrl = "http://www.w3.org/TR/1999/REC-xpath-19991116";

	public const string XmlDsigXsltTransformUrl = "http://www.w3.org/TR/1999/REC-xslt-19991116";

	public const string XmlLicenseTransformUrl = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";

	private EncryptedXml encryptedXml;

	protected Signature m_signature;

	private AsymmetricAlgorithm key;

	protected string m_strSigningKeyName;

	private XmlDocument envdoc;

	private IEnumerator pkEnumerator;

	private XmlElement signatureElement;

	private Hashtable hashes;

	private XmlResolver xmlResolver = new XmlUrlResolver();

	private ArrayList manifests;

	private IEnumerator _x509Enumerator;

	private static readonly char[] whitespaceChars = new char[4] { ' ', '\r', '\n', '\t' };

	[ComVisible(false)]
	public EncryptedXml EncryptedXml
	{
		get
		{
			return encryptedXml;
		}
		set
		{
			encryptedXml = value;
		}
	}

	public KeyInfo KeyInfo
	{
		get
		{
			if (m_signature.KeyInfo == null)
			{
				m_signature.KeyInfo = new KeyInfo();
			}
			return m_signature.KeyInfo;
		}
		set
		{
			m_signature.KeyInfo = value;
		}
	}

	public Signature Signature => m_signature;

	public string SignatureLength => m_signature.SignedInfo.SignatureLength;

	public string SignatureMethod => m_signature.SignedInfo.SignatureMethod;

	public byte[] SignatureValue => m_signature.SignatureValue;

	public SignedInfo SignedInfo => m_signature.SignedInfo;

	public AsymmetricAlgorithm SigningKey
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
		}
	}

	public string SigningKeyName
	{
		get
		{
			return m_strSigningKeyName;
		}
		set
		{
			m_strSigningKeyName = value;
		}
	}

	[ComVisible(false)]
	public XmlResolver Resolver
	{
		set
		{
			xmlResolver = value;
		}
	}

	public SignedXml()
	{
		m_signature = new Signature();
		m_signature.SignedInfo = new SignedInfo();
		hashes = new Hashtable(2);
	}

	public SignedXml(XmlDocument document)
		: this()
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		envdoc = document;
	}

	public SignedXml(XmlElement elem)
		: this()
	{
		if (elem == null)
		{
			throw new ArgumentNullException("elem");
		}
		envdoc = new XmlDocument();
		envdoc.LoadXml(elem.OuterXml);
	}

	public void AddObject(DataObject dataObject)
	{
		m_signature.AddObject(dataObject);
	}

	public void AddReference(Reference reference)
	{
		if (reference == null)
		{
			throw new ArgumentNullException("reference");
		}
		m_signature.SignedInfo.AddReference(reference);
	}

	private Stream ApplyTransform(Transform t, XmlDocument input)
	{
		if (t is XmlDsigXPathTransform || t is XmlDsigEnvelopedSignatureTransform || t is XmlDecryptionTransform)
		{
			input = (XmlDocument)input.Clone();
		}
		t.LoadInput(input);
		if (t is XmlDsigEnvelopedSignatureTransform)
		{
			return CanonicalizeOutput(t.GetOutput());
		}
		object output = t.GetOutput();
		if (output is Stream)
		{
			return (Stream)output;
		}
		if (output is XmlDocument)
		{
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
			((XmlDocument)output).WriteTo(xmlTextWriter);
			xmlTextWriter.Flush();
			memoryStream.Position = 0L;
			return memoryStream;
		}
		if (output == null)
		{
			throw new NotImplementedException(string.Concat("This should not occur. Transform is ", t, "."));
		}
		return CanonicalizeOutput(output);
	}

	private Stream CanonicalizeOutput(object obj)
	{
		Transform c14NMethod = GetC14NMethod();
		c14NMethod.LoadInput(obj);
		return (Stream)c14NMethod.GetOutput();
	}

	private XmlDocument GetManifest(Reference r)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.PreserveWhitespace = true;
		if (r.Uri[0] == '#')
		{
			if (signatureElement != null)
			{
				XmlElement idElement = GetIdElement(signatureElement.OwnerDocument, r.Uri.Substring(1));
				if (idElement == null)
				{
					throw new CryptographicException("Manifest targeted by Reference was not found: " + r.Uri.Substring(1));
				}
				xmlDocument.LoadXml(idElement.OuterXml);
				FixupNamespaceNodes(idElement, xmlDocument.DocumentElement, ignoreDefault: false);
			}
		}
		else if (xmlResolver != null)
		{
			Stream inStream = (Stream)xmlResolver.GetEntity(new Uri(r.Uri), null, typeof(Stream));
			xmlDocument.Load(inStream);
		}
		if (xmlDocument.FirstChild != null)
		{
			if (manifests == null)
			{
				manifests = new ArrayList();
			}
			manifests.Add(xmlDocument);
			return xmlDocument;
		}
		return null;
	}

	private void FixupNamespaceNodes(XmlElement src, XmlElement dst, bool ignoreDefault)
	{
		foreach (XmlAttribute item in src.SelectNodes("namespace::*"))
		{
			if (!(item.LocalName == "xml") && (!ignoreDefault || !(item.LocalName == "xmlns")))
			{
				dst.SetAttributeNode(dst.OwnerDocument.ImportNode(item, deep: true) as XmlAttribute);
			}
		}
	}

	private byte[] GetReferenceHash(Reference r, bool check_hmac)
	{
		Stream stream = null;
		XmlDocument xmlDocument = null;
		if (r.Uri == string.Empty)
		{
			xmlDocument = envdoc;
		}
		else if (r.Type == "http://www.w3.org/2000/09/xmldsig#Manifest")
		{
			xmlDocument = GetManifest(r);
		}
		else
		{
			xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			string text = null;
			if (r.Uri.StartsWith("#xpointer"))
			{
				string text2 = string.Join(string.Empty, r.Uri.Substring(9).Split(whitespaceChars));
				text2 = ((text2.Length >= 2 && text2[0] == '(' && text2[text2.Length - 1] == ')') ? text2.Substring(1, text2.Length - 2) : string.Empty);
				if (text2 == "/")
				{
					xmlDocument = envdoc;
				}
				else if (text2.Length > 6 && text2.StartsWith("id(") && text2[text2.Length - 1] == ')')
				{
					text = text2.Substring(4, text2.Length - 6);
				}
			}
			else if (r.Uri[0] == '#')
			{
				text = r.Uri.Substring(1);
			}
			else if (xmlResolver != null)
			{
				try
				{
					Uri absoluteUri = new Uri(r.Uri);
					stream = (Stream)xmlResolver.GetEntity(absoluteUri, null, typeof(Stream));
				}
				catch
				{
					stream = File.OpenRead(r.Uri);
				}
			}
			if (text != null)
			{
				XmlElement xmlElement = null;
				foreach (DataObject @object in m_signature.ObjectList)
				{
					if (!(@object.Id == text))
					{
						continue;
					}
					xmlElement = @object.GetXml();
					xmlElement.SetAttribute("xmlns", "http://www.w3.org/2000/09/xmldsig#");
					xmlDocument.LoadXml(xmlElement.OuterXml);
					foreach (XmlNode childNode in xmlElement.ChildNodes)
					{
						if (childNode.NodeType == XmlNodeType.Element)
						{
							FixupNamespaceNodes(childNode as XmlElement, xmlDocument.DocumentElement, ignoreDefault: true);
						}
					}
					break;
				}
				if (xmlElement == null && envdoc != null)
				{
					xmlElement = GetIdElement(envdoc, text);
					if (xmlElement != null)
					{
						xmlDocument.LoadXml(xmlElement.OuterXml);
					}
				}
				if (xmlElement == null)
				{
					throw new CryptographicException($"Malformed reference object: {text}");
				}
			}
		}
		if (r.TransformChain.Count > 0)
		{
			foreach (Transform item in r.TransformChain)
			{
				if (stream == null)
				{
					stream = ApplyTransform(item, xmlDocument);
					continue;
				}
				item.LoadInput(stream);
				object output = item.GetOutput();
				stream = ((!(output is Stream)) ? CanonicalizeOutput(output) : ((Stream)output));
			}
		}
		else if (stream == null)
		{
			if (r.Uri[0] != '#')
			{
				stream = new MemoryStream();
				xmlDocument.Save(stream);
			}
			else
			{
				stream = ApplyTransform(new XmlDsigC14NTransform(), xmlDocument);
			}
		}
		return GetHash(r.DigestMethod, check_hmac)?.ComputeHash(stream);
	}

	private void DigestReferences()
	{
		foreach (Reference reference in m_signature.SignedInfo.References)
		{
			if (reference.DigestMethod == null)
			{
				reference.DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
			}
			reference.DigestValue = GetReferenceHash(reference, check_hmac: false);
		}
	}

	private Transform GetC14NMethod()
	{
		Transform transform = (Transform)CryptoConfig.CreateFromName(m_signature.SignedInfo.CanonicalizationMethod);
		if (transform == null)
		{
			throw new CryptographicException("Unknown Canonicalization Method {0}", m_signature.SignedInfo.CanonicalizationMethod);
		}
		return transform;
	}

	private Stream SignedInfoTransformed()
	{
		Transform c14NMethod = GetC14NMethod();
		if (signatureElement == null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.LoadXml(m_signature.SignedInfo.GetXml().OuterXml);
			if (envdoc != null)
			{
				foreach (XmlAttribute item in envdoc.DocumentElement.SelectNodes("namespace::*"))
				{
					if (!(item.LocalName == "xml") && !(item.Prefix == xmlDocument.DocumentElement.Prefix))
					{
						xmlDocument.DocumentElement.SetAttributeNode(xmlDocument.ImportNode(item, deep: true) as XmlAttribute);
					}
				}
			}
			c14NMethod.LoadInput(xmlDocument);
		}
		else
		{
			XmlElement xmlElement = signatureElement.GetElementsByTagName("SignedInfo", "http://www.w3.org/2000/09/xmldsig#")[0] as XmlElement;
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.WriteStartElement(xmlElement.Prefix, xmlElement.LocalName, xmlElement.NamespaceURI);
			XmlNodeList xmlNodeList = xmlElement.SelectNodes("namespace::*");
			foreach (XmlAttribute item2 in xmlNodeList)
			{
				if (item2.ParentNode != xmlElement && !(item2.LocalName == "xml") && !(item2.Prefix == xmlElement.Prefix))
				{
					item2.WriteTo(xmlTextWriter);
				}
			}
			foreach (XmlNode attribute in xmlElement.Attributes)
			{
				attribute.WriteTo(xmlTextWriter);
			}
			foreach (XmlNode childNode in xmlElement.ChildNodes)
			{
				childNode.WriteTo(xmlTextWriter);
			}
			xmlTextWriter.WriteEndElement();
			byte[] bytes = Encoding.UTF8.GetBytes(stringWriter.ToString());
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(bytes, 0, bytes.Length);
			memoryStream.Position = 0L;
			c14NMethod.LoadInput(memoryStream);
		}
		return (Stream)c14NMethod.GetOutput();
	}

	private HashAlgorithm GetHash(string algorithm, bool check_hmac)
	{
		HashAlgorithm hashAlgorithm = (HashAlgorithm)hashes[algorithm];
		if (hashAlgorithm == null)
		{
			hashAlgorithm = HashAlgorithm.Create(algorithm);
			if (hashAlgorithm == null)
			{
				throw new CryptographicException("Unknown hash algorithm: {0}", algorithm);
			}
			hashes.Add(algorithm, hashAlgorithm);
		}
		else
		{
			hashAlgorithm.Initialize();
		}
		if (check_hmac && hashAlgorithm is KeyedHashAlgorithm)
		{
			return null;
		}
		return hashAlgorithm;
	}

	public bool CheckSignature()
	{
		return CheckSignatureInternal(null) != null;
	}

	private bool CheckReferenceIntegrity(ArrayList referenceList)
	{
		if (referenceList == null)
		{
			return false;
		}
		foreach (Reference reference in referenceList)
		{
			byte[] referenceHash = GetReferenceHash(reference, check_hmac: true);
			if (!Compare(reference.DigestValue, referenceHash))
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckSignature(AsymmetricAlgorithm key)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}
		return CheckSignatureInternal(key) != null;
	}

	private AsymmetricAlgorithm CheckSignatureInternal(AsymmetricAlgorithm key)
	{
		pkEnumerator = null;
		if (key != null)
		{
			if (!CheckSignatureWithKey(key))
			{
				return null;
			}
		}
		else
		{
			if (Signature.KeyInfo == null)
			{
				return null;
			}
			while ((key = GetPublicKey()) != null && !CheckSignatureWithKey(key))
			{
			}
			pkEnumerator = null;
			if (key == null)
			{
				return null;
			}
		}
		if (!CheckReferenceIntegrity(m_signature.SignedInfo.References))
		{
			return null;
		}
		if (manifests != null)
		{
			for (int i = 0; i < manifests.Count; i++)
			{
				Manifest manifest = new Manifest((manifests[i] as XmlDocument).DocumentElement);
				if (!CheckReferenceIntegrity(manifest.References))
				{
					return null;
				}
			}
		}
		return key;
	}

	private bool CheckSignatureWithKey(AsymmetricAlgorithm key)
	{
		if (key == null)
		{
			return false;
		}
		SignatureDescription signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(m_signature.SignedInfo.SignatureMethod);
		if (signatureDescription == null)
		{
			return false;
		}
		AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(signatureDescription.DeformatterAlgorithm);
		if (asymmetricSignatureDeformatter == null)
		{
			return false;
		}
		try
		{
			asymmetricSignatureDeformatter.SetKey(key);
			asymmetricSignatureDeformatter.SetHashAlgorithm(signatureDescription.DigestAlgorithm);
			HashAlgorithm hash = GetHash(signatureDescription.DigestAlgorithm, check_hmac: true);
			MemoryStream inputStream = (MemoryStream)SignedInfoTransformed();
			byte[] rgbHash = hash.ComputeHash(inputStream);
			return asymmetricSignatureDeformatter.VerifySignature(rgbHash, m_signature.SignatureValue);
		}
		catch
		{
			return false;
		}
	}

	private bool Compare(byte[] expected, byte[] actual)
	{
		bool flag = expected != null && actual != null;
		if (flag)
		{
			int num = expected.Length;
			flag = num == actual.Length;
			if (flag)
			{
				for (int i = 0; i < num; i++)
				{
					if (expected[i] != actual[i])
					{
						return false;
					}
				}
			}
		}
		return flag;
	}

	public bool CheckSignature(KeyedHashAlgorithm macAlg)
	{
		if (macAlg == null)
		{
			throw new ArgumentNullException("macAlg");
		}
		pkEnumerator = null;
		Stream stream = SignedInfoTransformed();
		if (stream == null)
		{
			return false;
		}
		byte[] array = macAlg.ComputeHash(stream);
		if (m_signature.SignedInfo.SignatureLength != null)
		{
			int num = int.Parse(m_signature.SignedInfo.SignatureLength);
			if (((uint)num & 7u) != 0)
			{
				throw new CryptographicException("Signature length must be a multiple of 8 bits.");
			}
			num >>= 3;
			if (num != m_signature.SignatureValue.Length)
			{
				throw new CryptographicException("Invalid signature length.");
			}
			int num2 = Math.Max(10, array.Length / 2);
			if (num < num2)
			{
				throw new CryptographicException("HMAC signature is too small");
			}
			if (num < array.Length)
			{
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(array, 0, array2, 0, num);
				array = array2;
			}
		}
		if (Compare(m_signature.SignatureValue, array))
		{
			return CheckReferenceIntegrity(m_signature.SignedInfo.References);
		}
		return false;
	}

	[System.MonoTODO]
	[ComVisible(false)]
	public bool CheckSignature(X509Certificate2 certificate, bool verifySignatureOnly)
	{
		throw new NotImplementedException();
	}

	public bool CheckSignatureReturningKey(out AsymmetricAlgorithm signingKey)
	{
		signingKey = CheckSignatureInternal(null);
		return signingKey != null;
	}

	public void ComputeSignature()
	{
		if (key != null)
		{
			if (m_signature.SignedInfo.SignatureMethod == null)
			{
				m_signature.SignedInfo.SignatureMethod = key.SignatureAlgorithm;
			}
			else if (m_signature.SignedInfo.SignatureMethod != key.SignatureAlgorithm)
			{
				throw new CryptographicException("Specified SignatureAlgorithm is not supported by the signing key.");
			}
			DigestReferences();
			AsymmetricSignatureFormatter asymmetricSignatureFormatter = null;
			if (key is DSA)
			{
				asymmetricSignatureFormatter = new DSASignatureFormatter(key);
			}
			else if (key is RSA)
			{
				asymmetricSignatureFormatter = new RSAPKCS1SignatureFormatter(key);
			}
			if (asymmetricSignatureFormatter != null)
			{
				SignatureDescription signatureDescription = (SignatureDescription)CryptoConfig.CreateFromName(m_signature.SignedInfo.SignatureMethod);
				HashAlgorithm hash = GetHash(signatureDescription.DigestAlgorithm, check_hmac: false);
				byte[] rgbHash = hash.ComputeHash(SignedInfoTransformed());
				asymmetricSignatureFormatter.SetHashAlgorithm("SHA1");
				m_signature.SignatureValue = asymmetricSignatureFormatter.CreateSignature(rgbHash);
			}
			return;
		}
		throw new CryptographicException("signing key is not specified");
	}

	public void ComputeSignature(KeyedHashAlgorithm macAlg)
	{
		if (macAlg == null)
		{
			throw new ArgumentNullException("macAlg");
		}
		string text = null;
		if (macAlg is HMACSHA1)
		{
			text = "http://www.w3.org/2000/09/xmldsig#hmac-sha1";
		}
		else if (macAlg is HMACSHA256)
		{
			text = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256";
		}
		else if (macAlg is HMACSHA384)
		{
			text = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha384";
		}
		else if (macAlg is HMACSHA512)
		{
			text = "http://www.w3.org/2001/04/xmldsig-more#hmac-sha512";
		}
		else if (macAlg is HMACRIPEMD160)
		{
			text = "http://www.w3.org/2001/04/xmldsig-more#hmac-ripemd160";
		}
		if (text == null)
		{
			throw new CryptographicException("unsupported algorithm");
		}
		DigestReferences();
		m_signature.SignedInfo.SignatureMethod = text;
		m_signature.SignatureValue = macAlg.ComputeHash(SignedInfoTransformed());
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

	protected virtual AsymmetricAlgorithm GetPublicKey()
	{
		if (m_signature.KeyInfo == null)
		{
			return null;
		}
		if (pkEnumerator == null)
		{
			pkEnumerator = m_signature.KeyInfo.GetEnumerator();
		}
		if (_x509Enumerator != null)
		{
			if (_x509Enumerator.MoveNext())
			{
				X509Certificate x509Certificate = (X509Certificate)_x509Enumerator.Current;
				return new X509Certificate2(x509Certificate.GetRawCertData()).PublicKey.Key;
			}
			_x509Enumerator = null;
		}
		while (pkEnumerator.MoveNext())
		{
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			KeyInfoClause keyInfoClause = (KeyInfoClause)pkEnumerator.Current;
			if (keyInfoClause is DSAKeyValue)
			{
				asymmetricAlgorithm = DSA.Create();
			}
			else if (keyInfoClause is RSAKeyValue)
			{
				asymmetricAlgorithm = RSA.Create();
			}
			if (asymmetricAlgorithm != null)
			{
				asymmetricAlgorithm.FromXmlString(keyInfoClause.GetXml().InnerXml);
				return asymmetricAlgorithm;
			}
			if (keyInfoClause is KeyInfoX509Data)
			{
				_x509Enumerator = ((KeyInfoX509Data)keyInfoClause).Certificates.GetEnumerator();
				if (_x509Enumerator.MoveNext())
				{
					X509Certificate x509Certificate2 = (X509Certificate)_x509Enumerator.Current;
					return new X509Certificate2(x509Certificate2.GetRawCertData()).PublicKey.Key;
				}
			}
		}
		return null;
	}

	public XmlElement GetXml()
	{
		return m_signature.GetXml(envdoc);
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		signatureElement = value;
		m_signature.LoadXml(value);
		foreach (Reference reference in m_signature.SignedInfo.References)
		{
			foreach (Transform item in reference.TransformChain)
			{
				if (item is XmlDecryptionTransform)
				{
					((XmlDecryptionTransform)item).EncryptedXml = EncryptedXml;
				}
			}
		}
	}
}
