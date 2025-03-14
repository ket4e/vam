using System.Collections;

namespace System.Security.AccessControl;

public sealed class AceEnumerator : IEnumerator
{
	private GenericAcl owner;

	private int current = -1;

	object IEnumerator.Current => Current;

	public GenericAce Current => (current >= 0) ? owner[current] : null;

	internal AceEnumerator(GenericAcl owner)
	{
		this.owner = owner;
	}

	public bool MoveNext()
	{
		if (current + 1 == owner.Count)
		{
			return false;
		}
		current++;
		return true;
	}

	public void Reset()
	{
		current = -1;
	}
}
