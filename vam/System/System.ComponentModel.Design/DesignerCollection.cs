using System.Collections;

namespace System.ComponentModel.Design;

public class DesignerCollection : ICollection, IEnumerable
{
	private ArrayList designers;

	int ICollection.Count => Count;

	bool ICollection.IsSynchronized => designers.IsSynchronized;

	object ICollection.SyncRoot => designers.SyncRoot;

	public int Count => designers.Count;

	public virtual IDesignerHost this[int index] => (IDesignerHost)designers[index];

	public DesignerCollection(IDesignerHost[] designers)
	{
		this.designers = new ArrayList(designers);
	}

	public DesignerCollection(IList designers)
	{
		this.designers = new ArrayList(designers);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		designers.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return designers.GetEnumerator();
	}
}
