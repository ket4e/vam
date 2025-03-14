using System.Collections;

namespace System.Security.Cryptography.Pkcs;

public sealed class SignerInfoEnumerator : IEnumerator
{
	private IEnumerator enumerator;

	object IEnumerator.Current => enumerator.Current;

	public SignerInfo Current => (SignerInfo)enumerator.Current;

	internal SignerInfoEnumerator(IEnumerable enumerable)
	{
		enumerator = enumerable.GetEnumerator();
	}

	public bool MoveNext()
	{
		return enumerator.MoveNext();
	}

	public void Reset()
	{
		enumerator.Reset();
	}
}
