using System.Collections;

namespace System.Security.Cryptography.Xml;

public class TransformChain
{
	private ArrayList chain;

	public int Count => chain.Count;

	public Transform this[int index] => (Transform)chain[index];

	public TransformChain()
	{
		chain = new ArrayList();
	}

	public void Add(Transform transform)
	{
		chain.Add(transform);
	}

	public IEnumerator GetEnumerator()
	{
		return chain.GetEnumerator();
	}
}
