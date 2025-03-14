using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Policy;

[ComVisible(true)]
public sealed class ApplicationTrustEnumerator : IEnumerator
{
	private IEnumerator e;

	object IEnumerator.Current => e.Current;

	public ApplicationTrust Current => (ApplicationTrust)e.Current;

	internal ApplicationTrustEnumerator(ApplicationTrustCollection collection)
	{
		e = collection.GetEnumerator();
	}

	public bool MoveNext()
	{
		return e.MoveNext();
	}

	public void Reset()
	{
		e.Reset();
	}
}
