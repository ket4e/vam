using Mono.Security;

namespace System.Security.Cryptography.Pkcs;

public sealed class ContentInfo
{
	private Oid _oid;

	private byte[] _content;

	public byte[] Content => (byte[])_content.Clone();

	public Oid ContentType => _oid;

	public ContentInfo(byte[] content)
		: this(new Oid("1.2.840.113549.1.7.1"), content)
	{
	}

	public ContentInfo(Oid oid, byte[] content)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		if (content == null)
		{
			throw new ArgumentNullException("content");
		}
		_oid = oid;
		_content = content;
	}

	~ContentInfo()
	{
	}

	[System.MonoTODO("MS is stricter than us about the content structure")]
	public static Oid GetContentType(byte[] encodedMessage)
	{
		if (encodedMessage == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		try
		{
			PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(encodedMessage);
			switch (contentInfo.ContentType)
			{
			case "1.2.840.113549.1.7.1":
			case "1.2.840.113549.1.7.2":
			case "1.2.840.113549.1.7.3":
			case "1.2.840.113549.1.7.5":
			case "1.2.840.113549.1.7.6":
				return new Oid(contentInfo.ContentType);
			default:
			{
				string text = global::Locale.GetText("Bad ASN1 - invalid OID '{0}'");
				throw new CryptographicException(string.Format(text, contentInfo.ContentType));
			}
			}
		}
		catch (Exception inner)
		{
			throw new CryptographicException(global::Locale.GetText("Bad ASN1 - invalid structure"), inner);
		}
	}
}
