using System.Collections;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc;

[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ListBindable(false)]
public sealed class OdbcParameterCollection : DbParameterCollection
{
	private readonly ArrayList list = new ArrayList();

	private int nullParamCount = 1;

	public override int Count => list.Count;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public new OdbcParameter this[int index]
	{
		get
		{
			return (OdbcParameter)list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public new OdbcParameter this[string parameterName]
	{
		get
		{
			foreach (OdbcParameter item in list)
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

	public override bool IsFixedSize => false;

	public override bool IsReadOnly => false;

	public override bool IsSynchronized => list.IsSynchronized;

	public override object SyncRoot => list.SyncRoot;

	internal OdbcParameterCollection()
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override int Add(object value)
	{
		if (!(value is OdbcParameter))
		{
			throw new InvalidCastException("The parameter was not an OdbcParameter.");
		}
		Add((OdbcParameter)value);
		return IndexOf(value);
	}

	public OdbcParameter Add(OdbcParameter value)
	{
		if (value.Container != null)
		{
			throw new ArgumentException("The OdbcParameter specified in the value parameter is already added to this or another OdbcParameterCollection.");
		}
		if (value.ParameterName == null || value.ParameterName.Length == 0)
		{
			value.ParameterName = "Parameter" + nullParamCount;
			nullParamCount++;
		}
		value.Container = this;
		list.Add(value);
		return value;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Add(String parameterName, Object value) has been deprecated.  Use AddWithValue(String parameterName, Object value).")]
	public OdbcParameter Add(string parameterName, object value)
	{
		return Add(new OdbcParameter(parameterName, value));
	}

	public OdbcParameter Add(string parameterName, OdbcType odbcType)
	{
		return Add(new OdbcParameter(parameterName, odbcType));
	}

	public OdbcParameter Add(string parameterName, OdbcType odbcType, int size)
	{
		return Add(new OdbcParameter(parameterName, odbcType, size));
	}

	public OdbcParameter Add(string parameterName, OdbcType odbcType, int size, string sourceColumn)
	{
		return Add(new OdbcParameter(parameterName, odbcType, size, sourceColumn));
	}

	public override void Clear()
	{
		foreach (OdbcParameter item in list)
		{
			item.Container = null;
		}
		list.Clear();
	}

	public override bool Contains(object value)
	{
		if (value == null)
		{
			return false;
		}
		if (!(value is OdbcParameter))
		{
			throw new InvalidCastException("The parameter was not an OdbcParameter.");
		}
		return Contains(((OdbcParameter)value).ParameterName);
	}

	public override bool Contains(string value)
	{
		if (value == null || value.Length == 0)
		{
			return false;
		}
		string value2 = value.ToUpper();
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				OdbcParameter odbcParameter = (OdbcParameter)enumerator.Current;
				if (odbcParameter.ParameterName.ToUpper().Equals(value2))
				{
					return true;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return false;
	}

	public override void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public override IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public override int IndexOf(object value)
	{
		if (value == null)
		{
			return -1;
		}
		if (!(value is OdbcParameter))
		{
			throw new InvalidCastException("The parameter was not an OdbcParameter.");
		}
		return list.IndexOf(value);
	}

	public override int IndexOf(string parameterName)
	{
		if (parameterName == null || parameterName.Length == 0)
		{
			return -1;
		}
		string value = parameterName.ToUpper();
		for (int i = 0; i < Count; i++)
		{
			if (this[i].ParameterName.ToUpper().Equals(value))
			{
				return i;
			}
		}
		return -1;
	}

	public override void Insert(int index, object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (!(value is OdbcParameter))
		{
			throw new InvalidCastException("The parameter was not an OdbcParameter.");
		}
		Insert(index, (OdbcParameter)value);
	}

	public override void Remove(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (!(value is OdbcParameter))
		{
			throw new InvalidCastException("The parameter was not an OdbcParameter.");
		}
		Remove((OdbcParameter)value);
	}

	public override void RemoveAt(int index)
	{
		if (index >= list.Count || index < 0)
		{
			throw new IndexOutOfRangeException($"Invalid index {index} for this OdbcParameterCollection with count = {list.Count}");
		}
		this[index].Container = null;
		list.RemoveAt(index);
	}

	public override void RemoveAt(string parameterName)
	{
		RemoveAt(IndexOf(parameterName));
	}

	protected override DbParameter GetParameter(string name)
	{
		return this[name];
	}

	protected override DbParameter GetParameter(int index)
	{
		return this[index];
	}

	protected override void SetParameter(string name, DbParameter value)
	{
		this[name] = (OdbcParameter)value;
	}

	protected override void SetParameter(int index, DbParameter value)
	{
		this[index] = (OdbcParameter)value;
	}

	public override void AddRange(Array values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		foreach (OdbcParameter value2 in values)
		{
			if (value2 == null)
			{
				throw new ArgumentNullException("values", "The OdbcParameterCollection only accepts non-null OdbcParameter type objects");
			}
		}
		foreach (OdbcParameter value3 in values)
		{
			Add(value3);
		}
	}

	public void AddRange(OdbcParameter[] values)
	{
		AddRange((Array)values);
	}

	public void Insert(int index, OdbcParameter value)
	{
		if (index > list.Count || index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "The index must be non-negative and less than or equal to size of the collection");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.Container != null)
		{
			throw new ArgumentException("The OdbcParameter is already contained by another collection");
		}
		if (string.IsNullOrEmpty(value.ParameterName))
		{
			value.ParameterName = "Parameter" + nullParamCount;
			nullParamCount++;
		}
		value.Container = this;
		list.Insert(index, value);
	}

	public OdbcParameter AddWithValue(string parameterName, object value)
	{
		if (value == null)
		{
			return Add(new OdbcParameter(parameterName, OdbcType.NVarChar));
		}
		return Add(new OdbcParameter(parameterName, value));
	}

	public void Remove(OdbcParameter value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.Container != this)
		{
			throw new ArgumentException("values", "Attempted to remove an OdbcParameter that is not contained in this OdbcParameterCollection");
		}
		value.Container = null;
		list.Remove(value);
	}

	public bool Contains(OdbcParameter value)
	{
		if (value == null)
		{
			return false;
		}
		if (value.Container != this)
		{
			return false;
		}
		return Contains(value.ParameterName);
	}

	public int IndexOf(OdbcParameter value)
	{
		if (value == null)
		{
			return -1;
		}
		return IndexOf((object)value);
	}

	public void CopyTo(OdbcParameter[] array, int index)
	{
		list.CopyTo(array, index);
	}
}
