using System.Xml;

namespace System.Security.Cryptography.Xml;

public class KeyInfoEncryptedKey : KeyInfoClause
{
	private EncryptedKey encryptedKey;

	public EncryptedKey EncryptedKey
	{
		get
		{
			return encryptedKey;
		}
		set
		{
			encryptedKey = value;
		}
	}

	public KeyInfoEncryptedKey()
	{
	}

	public KeyInfoEncryptedKey(EncryptedKey ek)
	{
		EncryptedKey = ek;
	}

	public override XmlElement GetXml()
	{
		return GetXml(new XmlDocument());
	}

	internal XmlElement GetXml(XmlDocument document)
	{
		if (encryptedKey != null)
		{
			return encryptedKey.GetXml(document);
		}
		return null;
	}

	public override void LoadXml(XmlElement value)
	{
		EncryptedKey = new EncryptedKey();
		EncryptedKey.LoadXml(value);
	}
}
