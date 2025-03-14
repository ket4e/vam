using System.Collections;
using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeNamespaceImportCollection : IList, ICollection, IEnumerable
{
	private Hashtable keys;

	private ArrayList data;

	int ICollection.Count => data.Count;

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => false;

	object IList.this[int index]
	{
		get
		{
			return data[index];
		}
		set
		{
			this[index] = (CodeNamespaceImport)value;
		}
	}

	object ICollection.SyncRoot => null;

	bool ICollection.IsSynchronized => data.IsSynchronized;

	public int Count => data.Count;

	public CodeNamespaceImport this[int index]
	{
		get
		{
			return (CodeNamespaceImport)data[index];
		}
		set
		{
			CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)data[index];
			keys.Remove(codeNamespaceImport.Namespace);
			data[index] = value;
			keys[value.Namespace] = value;
		}
	}

	public CodeNamespaceImportCollection()
	{
		data = new ArrayList();
		keys = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
	}

	void IList.Clear()
	{
		Clear();
	}

	int IList.Add(object value)
	{
		Add((CodeNamespaceImport)value);
		return data.Count - 1;
	}

	bool IList.Contains(object value)
	{
		return data.Contains(value);
	}

	int IList.IndexOf(object value)
	{
		return data.IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		data.Insert(index, value);
		CodeNamespaceImport codeNamespaceImport = (CodeNamespaceImport)value;
		keys[codeNamespaceImport.Namespace] = codeNamespaceImport;
	}

	void IList.Remove(object value)
	{
		string @namespace = ((CodeNamespaceImport)value).Namespace;
		data.Remove(value);
		foreach (CodeNamespaceImport datum in data)
		{
			if (datum.Namespace == @namespace)
			{
				keys[@namespace] = datum;
				return;
			}
		}
		keys.Remove(@namespace);
	}

	void IList.RemoveAt(int index)
	{
		string @namespace = this[index].Namespace;
		data.RemoveAt(index);
		foreach (CodeNamespaceImport datum in data)
		{
			if (datum.Namespace == @namespace)
			{
				keys[@namespace] = datum;
				return;
			}
		}
		keys.Remove(@namespace);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		data.CopyTo(array, index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return data.GetEnumerator();
	}

	public void Add(CodeNamespaceImport value)
	{
		if (value == null)
		{
			throw new NullReferenceException();
		}
		if (!keys.ContainsKey(value.Namespace))
		{
			keys[value.Namespace] = value;
			data.Add(value);
		}
	}

	public void AddRange(CodeNamespaceImport[] value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		foreach (CodeNamespaceImport value2 in value)
		{
			Add(value2);
		}
	}

	public void Clear()
	{
		data.Clear();
		keys.Clear();
	}

	public IEnumerator GetEnumerator()
	{
		return data.GetEnumerator();
	}
}
