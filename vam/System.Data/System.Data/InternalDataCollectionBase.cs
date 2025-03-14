using System.Collections;
using System.ComponentModel;

namespace System.Data;

public class InternalDataCollectionBase : IEnumerable, ICollection
{
	private ArrayList list;

	private bool readOnly;

	private bool synchronized;

	[Browsable(false)]
	public virtual int Count => list.Count;

	[Browsable(false)]
	public bool IsReadOnly => readOnly;

	[Browsable(false)]
	public bool IsSynchronized => synchronized;

	protected virtual ArrayList List => list;

	[Browsable(false)]
	public object SyncRoot => this;

	public InternalDataCollectionBase()
	{
		list = new ArrayList();
	}

	public virtual void CopyTo(Array ar, int index)
	{
		list.CopyTo(ar, index);
	}

	public virtual IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	internal Array ToArray(Type type)
	{
		return list.ToArray(type);
	}
}
