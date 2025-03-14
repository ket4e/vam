namespace System.Text;

[Serializable]
public sealed class EncoderExceptionFallback : EncoderFallback
{
	public override int MaxCharCount => 0;

	public override EncoderFallbackBuffer CreateFallbackBuffer()
	{
		return new EncoderExceptionFallbackBuffer();
	}

	public override bool Equals(object value)
	{
		return value is EncoderExceptionFallback;
	}

	public override int GetHashCode()
	{
		return 0;
	}
}
