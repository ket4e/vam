using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using Mono.Security;
using Mono.Security.X509;

namespace System.Security.Cryptography.Pkcs;

public sealed class SignedCms
{
	private ContentInfo _content;

	private bool _detached;

	private SignerInfoCollection _info;

	private X509Certificate2Collection _certs;

	private SubjectIdentifierType _type;

	private int _version;

	public X509Certificate2Collection Certificates => _certs;

	public ContentInfo ContentInfo
	{
		get
		{
			if (_content == null)
			{
				Oid oid = new Oid("1.2.840.113549.1.7.1");
				_content = new ContentInfo(oid, new byte[0]);
			}
			return _content;
		}
	}

	public bool Detached => _detached;

	public SignerInfoCollection SignerInfos => _info;

	public int Version => _version;

	public SignedCms()
	{
		_certs = new X509Certificate2Collection();
		_info = new SignerInfoCollection();
	}

	public SignedCms(ContentInfo content)
		: this(content, detached: false)
	{
	}

	public SignedCms(ContentInfo content, bool detached)
		: this()
	{
		if (content == null)
		{
			throw new ArgumentNullException("content");
		}
		_content = content;
		_detached = detached;
	}

	public SignedCms(SubjectIdentifierType signerIdentifierType)
		: this()
	{
		_type = signerIdentifierType;
	}

	public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo content)
		: this(content, detached: false)
	{
		_type = signerIdentifierType;
	}

	public SignedCms(SubjectIdentifierType signerIdentifierType, ContentInfo content, bool detached)
		: this(content, detached)
	{
		_type = signerIdentifierType;
	}

	[System.MonoTODO]
	public void CheckSignature(bool verifySignatureOnly)
	{
		SignerInfoEnumerator enumerator = _info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SignerInfo current = enumerator.Current;
			current.CheckSignature(verifySignatureOnly);
		}
	}

	[System.MonoTODO]
	public void CheckSignature(X509Certificate2Collection extraStore, bool verifySignatureOnly)
	{
		SignerInfoEnumerator enumerator = _info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SignerInfo current = enumerator.Current;
			current.CheckSignature(extraStore, verifySignatureOnly);
		}
	}

	[System.MonoTODO]
	public void CheckHash()
	{
		throw new InvalidOperationException(string.Empty);
	}

	[System.MonoTODO]
	public void ComputeSignature()
	{
		throw new CryptographicException(string.Empty);
	}

	[System.MonoTODO]
	public void ComputeSignature(CmsSigner signer)
	{
		ComputeSignature();
	}

	[System.MonoTODO]
	public void ComputeSignature(CmsSigner signer, bool silent)
	{
		ComputeSignature();
	}

	private string ToString(byte[] array, bool reverse)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (reverse)
		{
			for (int num = array.Length - 1; num >= 0; num--)
			{
				stringBuilder.Append(array[num].ToString("X2"));
			}
		}
		else
		{
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
		}
		return stringBuilder.ToString();
	}

	private byte[] GetKeyIdentifier(Mono.Security.X509.X509Certificate x509)
	{
		Mono.Security.X509.X509Extension x509Extension = x509.Extensions["2.5.29.14"];
		if (x509Extension != null)
		{
			ASN1 aSN = new ASN1(x509Extension.Value.Value);
			return aSN.Value;
		}
		ASN1 aSN2 = new ASN1(48);
		ASN1 aSN3 = aSN2.Add(new ASN1(48));
		aSN3.Add(new ASN1(CryptoConfig.EncodeOID(x509.KeyAlgorithm)));
		aSN3.Add(new ASN1(x509.KeyAlgorithmParameters));
		byte[] publicKey = x509.PublicKey;
		byte[] array = new byte[publicKey.Length + 1];
		Array.Copy(publicKey, 0, array, 1, publicKey.Length);
		aSN2.Add(new ASN1(3, array));
		SHA1 sHA = SHA1.Create();
		return sHA.ComputeHash(aSN2.GetBytes());
	}

	[System.MonoTODO("incomplete - missing attributes")]
	public void Decode(byte[] encodedMessage)
	{
		PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(encodedMessage);
		if (contentInfo.ContentType != "1.2.840.113549.1.7.2")
		{
			throw new Exception(string.Empty);
		}
		PKCS7.SignedData signedData = new PKCS7.SignedData(contentInfo.Content);
		SubjectIdentifierType type = SubjectIdentifierType.Unknown;
		object o = null;
		X509Certificate2 certificate = null;
		if (signedData.SignerInfo.Certificate != null)
		{
			certificate = new X509Certificate2(signedData.SignerInfo.Certificate.RawData);
		}
		else if (signedData.SignerInfo.IssuerName != null && signedData.SignerInfo.SerialNumber != null)
		{
			byte[] serialNumber = signedData.SignerInfo.SerialNumber;
			Array.Reverse(serialNumber);
			type = SubjectIdentifierType.IssuerAndSerialNumber;
			X509IssuerSerial x509IssuerSerial = default(X509IssuerSerial);
			x509IssuerSerial.IssuerName = signedData.SignerInfo.IssuerName;
			x509IssuerSerial.SerialNumber = ToString(serialNumber, reverse: true);
			o = x509IssuerSerial;
			foreach (Mono.Security.X509.X509Certificate certificate2 in signedData.Certificates)
			{
				if (certificate2.IssuerName == signedData.SignerInfo.IssuerName && ToString(certificate2.SerialNumber, reverse: true) == x509IssuerSerial.SerialNumber)
				{
					certificate = new X509Certificate2(certificate2.RawData);
					break;
				}
			}
		}
		else if (signedData.SignerInfo.SubjectKeyIdentifier != null)
		{
			string text = ToString(signedData.SignerInfo.SubjectKeyIdentifier, reverse: false);
			type = SubjectIdentifierType.SubjectKeyIdentifier;
			o = text;
			foreach (Mono.Security.X509.X509Certificate certificate3 in signedData.Certificates)
			{
				if (ToString(GetKeyIdentifier(certificate3), reverse: false) == text)
				{
					certificate = new X509Certificate2(certificate3.RawData);
					break;
				}
			}
		}
		SignerInfo signer = new SignerInfo(signedData.SignerInfo.HashName, certificate, type, o, signedData.SignerInfo.Version);
		_info.Add(signer);
		ASN1 content = signedData.ContentInfo.Content;
		Oid oid = new Oid(signedData.ContentInfo.ContentType);
		_content = new ContentInfo(oid, content[0].Value);
		foreach (Mono.Security.X509.X509Certificate certificate4 in signedData.Certificates)
		{
			_certs.Add(new X509Certificate2(certificate4.RawData));
		}
		_version = signedData.Version;
	}

	[System.MonoTODO]
	public byte[] Encode()
	{
		return null;
	}

	[System.MonoTODO]
	public void RemoveSignature(SignerInfo signerInfo)
	{
	}

	[System.MonoTODO]
	public void RemoveSignature(int index)
	{
	}
}
