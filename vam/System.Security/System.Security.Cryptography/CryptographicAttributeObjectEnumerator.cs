using System.Collections;

namespace System.Security.Cryptography;

public sealed class CryptographicAttributeObjectEnumerator : IEnumerator
{
	private IEnumerator enumerator;

	object IEnumerator.Current => enumerator.Current;

	public CryptographicAttributeObject Current => (CryptographicAttributeObject)enumerator.Current;

	internal CryptographicAttributeObjectEnumerator(IEnumerable enumerable)
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
