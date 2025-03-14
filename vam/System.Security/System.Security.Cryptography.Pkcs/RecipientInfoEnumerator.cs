using System.Collections;

namespace System.Security.Cryptography.Pkcs;

public sealed class RecipientInfoEnumerator : IEnumerator
{
	private IEnumerator enumerator;

	object IEnumerator.Current => enumerator.Current;

	public RecipientInfo Current => (RecipientInfo)enumerator.Current;

	internal RecipientInfoEnumerator(IEnumerable enumerable)
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
