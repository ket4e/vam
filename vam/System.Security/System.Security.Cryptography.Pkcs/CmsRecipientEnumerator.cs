using System.Collections;

namespace System.Security.Cryptography.Pkcs;

public sealed class CmsRecipientEnumerator : IEnumerator
{
	private IEnumerator enumerator;

	object IEnumerator.Current => enumerator.Current;

	public CmsRecipient Current => (CmsRecipient)enumerator.Current;

	internal CmsRecipientEnumerator(IEnumerable enumerable)
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
