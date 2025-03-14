using System.Collections;
using System.ComponentModel;

namespace System.Data.SqlClient;

[Serializable]
[ListBindable(false)]
public sealed class SqlErrorCollection : IEnumerable, ICollection
{
	private ArrayList list = new ArrayList();

	bool ICollection.IsSynchronized => list.IsSynchronized;

	object ICollection.SyncRoot => list.SyncRoot;

	public int Count => list.Count;

	public SqlError this[int index] => (SqlError)list[index];

	internal SqlErrorCollection()
	{
	}

	internal SqlErrorCollection(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
	{
		Add(theClass, lineNumber, message, number, procedure, server, source, state);
	}

	internal void Add(SqlError error)
	{
		list.Add(error);
	}

	internal void Add(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
	{
		SqlError error = new SqlError(theClass, lineNumber, message, number, procedure, server, source, state);
		Add(error);
	}

	public void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public void CopyTo(SqlError[] array, int index)
	{
		list.CopyTo(array, index);
	}
}
