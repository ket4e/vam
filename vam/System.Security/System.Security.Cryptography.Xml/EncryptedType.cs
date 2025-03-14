using System.Xml;

namespace System.Security.Cryptography.Xml;

public abstract class EncryptedType
{
	private CipherData cipherData;

	private string encoding;

	private EncryptionMethod encryptionMethod;

	private EncryptionPropertyCollection encryptionProperties;

	private string id;

	private KeyInfo keyInfo;

	private string mimeType;

	private string type;

	public virtual CipherData CipherData
	{
		get
		{
			return cipherData;
		}
		set
		{
			cipherData = value;
		}
	}

	public virtual string Encoding
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

	public virtual EncryptionMethod EncryptionMethod
	{
		get
		{
			return encryptionMethod;
		}
		set
		{
			encryptionMethod = value;
		}
	}

	public virtual EncryptionPropertyCollection EncryptionProperties => encryptionProperties;

	public virtual string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public KeyInfo KeyInfo
	{
		get
		{
			return keyInfo;
		}
		set
		{
			keyInfo = value;
		}
	}

	public virtual string MimeType
	{
		get
		{
			return mimeType;
		}
		set
		{
			mimeType = value;
		}
	}

	public virtual string Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	protected EncryptedType()
	{
		cipherData = new CipherData();
		encryptionProperties = new EncryptionPropertyCollection();
		keyInfo = new KeyInfo();
	}

	public void AddProperty(EncryptionProperty ep)
	{
		EncryptionProperties.Add(ep);
	}

	public abstract XmlElement GetXml();

	public abstract void LoadXml(XmlElement value);
}
