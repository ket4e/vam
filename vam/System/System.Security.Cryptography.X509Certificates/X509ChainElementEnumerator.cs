using System.Collections;

namespace System.Security.Cryptography.X509Certificates;

public sealed class X509ChainElementEnumerator : IEnumerator
{
	private IEnumerator enumerator;

	object IEnumerator.Current => enumerator.Current;

	public X509ChainElement Current => (X509ChainElement)enumerator.Current;

	internal X509ChainElementEnumerator(IEnumerable enumerable)
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
