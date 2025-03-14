using System.Collections;

namespace System.Data.Common;

internal abstract class DataContainer
{
	private BitArray null_values;

	private Type _type;

	private DataColumn _column;

	internal object this[int index]
	{
		get
		{
			return (!IsNull(index)) ? GetValue(index) : DBNull.Value;
		}
		set
		{
			if (value == null)
			{
				CopyValue(Column.Table.DefaultValuesRowIndex, index);
				return;
			}
			bool flag = value == DBNull.Value;
			if (flag)
			{
				ZeroOut(index);
			}
			else
			{
				SetValue(index, value);
			}
			null_values[index] = flag;
		}
	}

	internal int Capacity
	{
		get
		{
			return (null_values != null) ? null_values.Count : 0;
		}
		set
		{
			int capacity = Capacity;
			if (value != capacity)
			{
				if (null_values == null)
				{
					null_values = new BitArray(value);
				}
				else
				{
					null_values.Length = value;
				}
				Resize(value);
			}
		}
	}

	internal Type Type => _type;

	protected DataColumn Column => _column;

	protected abstract object GetValue(int index);

	internal abstract long GetInt64(int index);

	protected abstract void ZeroOut(int index);

	protected abstract void SetValue(int index, object value);

	protected abstract void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field);

	protected abstract void DoCopyValue(DataContainer from, int from_index, int to_index);

	protected abstract int DoCompareValues(int index1, int index2);

	protected abstract void Resize(int length);

	internal static DataContainer Create(Type type, DataColumn column)
	{
		DataContainer dataContainer = Type.GetTypeCode(type) switch
		{
			TypeCode.Int16 => new Int16DataContainer(), 
			TypeCode.Int32 => new Int32DataContainer(), 
			TypeCode.Int64 => new Int64DataContainer(), 
			TypeCode.String => new StringDataContainer(), 
			TypeCode.Boolean => new BitDataContainer(), 
			TypeCode.Byte => new ByteDataContainer(), 
			TypeCode.Char => new CharDataContainer(), 
			TypeCode.Double => new DoubleDataContainer(), 
			TypeCode.SByte => new SByteDataContainer(), 
			TypeCode.Single => new SingleDataContainer(), 
			TypeCode.UInt16 => new UInt16DataContainer(), 
			TypeCode.UInt32 => new UInt32DataContainer(), 
			TypeCode.UInt64 => new UInt64DataContainer(), 
			TypeCode.DateTime => new DateTimeDataContainer(), 
			TypeCode.Decimal => new DecimalDataContainer(), 
			_ => new ObjectDataContainer(), 
		};
		dataContainer._type = type;
		dataContainer._column = column;
		return dataContainer;
	}

	internal static object GetExplicitValue(object value)
	{
		Type type = value.GetType();
		return type.GetMethod("op_Explicit", new Type[1] { type })?.Invoke(value, new object[1] { value });
	}

	internal object GetContainerData(object value)
	{
		if (_type.IsInstanceOfType(value))
		{
			return value;
		}
		if (value is IConvertible)
		{
			return Type.GetTypeCode(_type) switch
			{
				TypeCode.Int16 => Convert.ToInt16(value), 
				TypeCode.Int32 => Convert.ToInt32(value), 
				TypeCode.Int64 => Convert.ToInt64(value), 
				TypeCode.String => Convert.ToString(value), 
				TypeCode.Boolean => Convert.ToBoolean(value), 
				TypeCode.Byte => Convert.ToByte(value), 
				TypeCode.Char => Convert.ToChar(value), 
				TypeCode.Double => Convert.ToDouble(value), 
				TypeCode.SByte => Convert.ToSByte(value), 
				TypeCode.Single => Convert.ToSingle(value), 
				TypeCode.UInt16 => Convert.ToUInt16(value), 
				TypeCode.UInt32 => Convert.ToUInt32(value), 
				TypeCode.UInt64 => Convert.ToUInt64(value), 
				TypeCode.DateTime => Convert.ToDateTime(value), 
				TypeCode.Decimal => Convert.ToDecimal(value), 
				_ => throw new InvalidCastException(), 
			};
		}
		object explicitValue;
		if ((explicitValue = GetExplicitValue(value)) != null)
		{
			return explicitValue;
		}
		throw new InvalidCastException();
	}

	internal bool IsNull(int index)
	{
		return null_values == null || null_values[index];
	}

	internal void FillValues(int fromIndex)
	{
		for (int i = 0; i < Capacity; i++)
		{
			CopyValue(fromIndex, i);
		}
	}

	internal void CopyValue(int from_index, int to_index)
	{
		CopyValue(this, from_index, to_index);
	}

	internal void CopyValue(DataContainer from, int from_index, int to_index)
	{
		DoCopyValue(from, from_index, to_index);
		null_values[to_index] = from.null_values[from_index];
	}

	internal void SetItemFromDataRecord(int index, IDataRecord record, int field)
	{
		if (record.IsDBNull(field))
		{
			this[index] = DBNull.Value;
		}
		else if (record is ISafeDataRecord)
		{
			SetValueFromSafeDataRecord(index, (ISafeDataRecord)record, field);
		}
		else
		{
			this[index] = record.GetValue(field);
		}
	}

	internal int CompareValues(int index1, int index2)
	{
		bool flag = IsNull(index1);
		bool flag2 = IsNull(index2);
		if (flag == flag2)
		{
			return (!flag) ? DoCompareValues(index1, index2) : 0;
		}
		return (!flag) ? 1 : (-1);
	}
}
