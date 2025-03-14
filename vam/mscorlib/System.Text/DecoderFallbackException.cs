namespace System.Text;

[Serializable]
public sealed class DecoderFallbackException : ArgumentException
{
	private const string defaultMessage = "Failed to decode the input byte sequence to Unicode characters.";

	private byte[] bytes_unknown;

	private int index = -1;

	[MonoTODO]
	public byte[] BytesUnknown => bytes_unknown;

	[MonoTODO]
	public int Index => index;

	public DecoderFallbackException()
		: this(null)
	{
	}

	public DecoderFallbackException(string message)
		: base(message)
	{
	}

	public DecoderFallbackException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public DecoderFallbackException(string message, byte[] bytesUnknown, int index)
		: base(message)
	{
		bytes_unknown = bytesUnknown;
		this.index = index;
	}
}
