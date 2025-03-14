using System.ComponentModel;

namespace System.Data.Common;

[TypeConverter("System.Data.Common.DataColumnMapping+DataColumnMappingConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class DataColumnMapping : MarshalByRefObject, IColumnMapping, ICloneable
{
	private string sourceColumn;

	private string dataSetColumn;

	[DefaultValue("")]
	public string DataSetColumn
	{
		get
		{
			return dataSetColumn;
		}
		set
		{
			dataSetColumn = value;
		}
	}

	[DefaultValue("")]
	public string SourceColumn
	{
		get
		{
			return sourceColumn;
		}
		set
		{
			sourceColumn = value;
		}
	}

	public DataColumnMapping()
	{
		sourceColumn = string.Empty;
		dataSetColumn = string.Empty;
	}

	public DataColumnMapping(string sourceColumn, string dataSetColumn)
	{
		this.sourceColumn = sourceColumn;
		this.dataSetColumn = dataSetColumn;
	}

	object ICloneable.Clone()
	{
		return new DataColumnMapping(SourceColumn, DataSetColumn);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DataColumn GetDataColumnBySchemaAction(DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
	{
		if (dataTable.Columns.Contains(dataSetColumn))
		{
			return dataTable.Columns[dataSetColumn];
		}
		return schemaAction switch
		{
			MissingSchemaAction.Ignore => null, 
			MissingSchemaAction.Error => throw new InvalidOperationException($"Missing the DataColumn '{DataSetColumn}' in the DataTable '{dataTable.TableName}' for the SourceColumn '{SourceColumn}'"), 
			_ => new DataColumn(dataSetColumn, dataType), 
		};
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static DataColumn GetDataColumnBySchemaAction(string sourceColumn, string dataSetColumn, DataTable dataTable, Type dataType, MissingSchemaAction schemaAction)
	{
		if (dataTable.Columns.Contains(dataSetColumn))
		{
			return dataTable.Columns[dataSetColumn];
		}
		return schemaAction switch
		{
			MissingSchemaAction.Ignore => null, 
			MissingSchemaAction.Error => throw new InvalidOperationException($"Missing the DataColumn '{dataSetColumn}' in the DataTable '{dataTable.TableName}' for the SourceColumn '{sourceColumn}'"), 
			_ => new DataColumn(dataSetColumn, dataType), 
		};
	}

	public override string ToString()
	{
		return SourceColumn;
	}
}
