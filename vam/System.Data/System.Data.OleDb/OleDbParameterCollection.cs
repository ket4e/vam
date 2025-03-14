using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb;

[ListBindable(false)]
[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public sealed class OleDbParameterCollection : DbParameterCollection, IList, IDataParameterCollection, IEnumerable, ICollection
{
	private ArrayList list = new ArrayList();

	public override int Count => list.Count;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new OleDbParameter this[int index]
	{
		get
		{
			return (OleDbParameter)list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new OleDbParameter this[string parameterName]
	{
		get
		{
			foreach (OleDbParameter item in list)
			{
				if (item.ParameterName.Equals(parameterName))
				{
					return item;
				}
			}
			throw new IndexOutOfRangeException("The specified name does not exist: " + parameterName);
		}
		set
		{
			if (!Contains(parameterName))
			{
				throw new IndexOutOfRangeException("The specified name does not exist: " + parameterName);
			}
			this[IndexOf(parameterName)] = value;
		}
	}

	public override bool IsFixedSize => list.IsFixedSize;

	public override bool IsReadOnly => list.IsReadOnly;

	public override bool IsSynchronized => list.IsSynchronized;

	public override object SyncRoot => list.SyncRoot;

	internal IntPtr GdaParameterList
	{
		[System.MonoTODO]
		get
		{
			return libgda.gda_parameter_list_new();
		}
	}

	internal OleDbParameterCollection()
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int Add(object value)
	{
		if (!(value is OleDbParameter))
		{
			throw new InvalidCastException("The parameter was not an OleDbParameter.");
		}
		Add((OleDbParameter)value);
		return IndexOf(value);
	}

	public OleDbParameter Add(OleDbParameter value)
	{
		if (value.Container != null)
		{
			throw new ArgumentException("The OleDbParameter specified in the value parameter is already added to this or another OleDbParameterCollection.");
		}
		value.Container = this;
		list.Add(value);
		return value;
	}

	[Obsolete("OleDbParameterCollection.Add(string, value) is now obsolete. Use OleDbParameterCollection.AddWithValue(string, object) instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public OleDbParameter Add(string parameterName, object value)
	{
		return Add(new OleDbParameter(parameterName, value));
	}

	public OleDbParameter AddWithValue(string parameterName, object value)
	{
		return Add(new OleDbParameter(parameterName, value));
	}

	public OleDbParameter Add(string parameterName, OleDbType oleDbType)
	{
		return Add(new OleDbParameter(parameterName, oleDbType));
	}

	public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size)
	{
		return Add(new OleDbParameter(parameterName, oleDbType, size));
	}

	public OleDbParameter Add(string parameterName, OleDbType oleDbType, int size, string sourceColumn)
	{
		return Add(new OleDbParameter(parameterName, oleDbType, size, sourceColumn));
	}

	public override void AddRange(Array values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		foreach (object value in values)
		{
			Add(value);
		}
	}

	public void AddRange(OleDbParameter[] values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		foreach (OleDbParameter value in values)
		{
			Add(value);
		}
	}

	public override void Clear()
	{
		foreach (OleDbParameter item in list)
		{
			item.Container = null;
		}
		list.Clear();
	}

	public override bool Contains(object value)
	{
		if (!(value is OleDbParameter))
		{
			throw new InvalidCastException("The parameter was not an OleDbParameter.");
		}
		return Contains(((OleDbParameter)value).ParameterName);
	}

	public override bool Contains(string value)
	{
		foreach (OleDbParameter item in list)
		{
			if (item.ParameterName.Equals(value))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(OleDbParameter value)
	{
		return IndexOf(value) != -1;
	}

	public override void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public void CopyTo(OleDbParameter[] array, int index)
	{
		CopyTo(array, index);
	}

	public override IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	[System.MonoTODO]
	protected override DbParameter GetParameter(int index)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected override DbParameter GetParameter(string parameterName)
	{
		throw new NotImplementedException();
	}

	public override int IndexOf(object value)
	{
		if (!(value is OleDbParameter))
		{
			throw new InvalidCastException("The parameter was not an OleDbParameter.");
		}
		return IndexOf(((OleDbParameter)value).ParameterName);
	}

	public int IndexOf(OleDbParameter value)
	{
		return IndexOf(value);
	}

	public override int IndexOf(string parameterName)
	{
		for (int i = 0; i < Count; i++)
		{
			if (this[i].ParameterName.Equals(parameterName))
			{
				return i;
			}
		}
		return -1;
	}

	public override void Insert(int index, object value)
	{
		list.Insert(index, value);
	}

	public void Insert(int index, OleDbParameter value)
	{
		Insert(index, value);
	}

	public override void Remove(object value)
	{
		((OleDbParameter)value).Container = null;
		list.Remove(value);
	}

	public void Remove(OleDbParameter value)
	{
		Remove(value);
	}

	public override void RemoveAt(int index)
	{
		this[index].Container = null;
		list.RemoveAt(index);
	}

	public override void RemoveAt(string parameterName)
	{
		RemoveAt(IndexOf(parameterName));
	}

	[System.MonoTODO]
	protected override void SetParameter(int index, DbParameter value)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	protected override void SetParameter(string parameterName, DbParameter value)
	{
		throw new NotImplementedException();
	}
}
