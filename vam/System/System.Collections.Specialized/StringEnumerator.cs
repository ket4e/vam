namespace System.Collections.Specialized;

public class StringEnumerator
{
	private IEnumerator enumerable;

	public string Current => (string)enumerable.Current;

	internal StringEnumerator(StringCollection coll)
	{
		enumerable = ((IEnumerable)coll).GetEnumerator();
	}

	public bool MoveNext()
	{
		return enumerable.MoveNext();
	}

	public void Reset()
	{
		enumerable.Reset();
	}
}
