using System.ComponentModel;

namespace System.Data.Common;

[TypeConverter("System.Data.Common.DataTableMapping+DataTableMappingConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class DataTableMapping : MarshalByRefObject, ITableMapping, ICloneable
{
	private string sourceTable;

	private string dataSetTable;

	private DataColumnMappingCollection columnMappings;

	IColumnMappingCollection ITableMapping.ColumnMappings => ColumnMappings;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public DataColumnMappingCollection ColumnMappings => columnMappings;

	[DefaultValue("")]
	public string DataSetTable
	{
		get
		{
			return dataSetTable;
		}
		set
		{
			dataSetTable = value;
		}
	}

	[DefaultValue("")]
	public string SourceTable
	{
		get
		{
			return sourceTable;
		}
		set
		{
			sourceTable = value;
		}
	}

	public DataTableMapping()
	{
		dataSetTable = string.Empty;
		sourceTable = string.Empty;
		columnMappings = new DataColumnMappingCollection();
	}

	public DataTableMapping(string sourceTable, string dataSetTable)
		: this()
	{
		this.sourceTable = sourceTable;
		this.dataSetTable = dataSetTable;
	}

	public DataTableMapping(string sourceTable, string dataSetTable, DataColumnMapping[] columnMappings)
		: this(sourceTable, dataSetTable)
	{
		this.columnMappings.AddRange(columnMappings);
	}

	object ICloneable.Clone()
	{
		DataColumnMapping[] arr = new DataColumnMapping[columnMappings.Count];
		columnMappings.CopyTo(arr, 0);
		return new DataTableMapping(SourceTable, DataSetTable, arr);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DataColumnMapping GetColumnMappingBySchemaAction(string sourceColumn, MissingMappingAction mappingAction)
	{
		return DataColumnMappingCollection.GetColumnMappingBySchemaAction(columnMappings, sourceColumn, mappingAction);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO]
	public DataColumn GetDataColumn(string sourceColumn, Type dataType, DataTable dataTable, MissingMappingAction mappingAction, MissingSchemaAction schemaAction)
	{
		throw new NotImplementedException();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DataTable GetDataTableBySchemaAction(DataSet dataSet, MissingSchemaAction schemaAction)
	{
		if (dataSet.Tables.Contains(DataSetTable))
		{
			return dataSet.Tables[DataSetTable];
		}
		return schemaAction switch
		{
			MissingSchemaAction.Ignore => null, 
			MissingSchemaAction.Error => throw new InvalidOperationException($"Missing the '{DataSetTable} DataTable for the '{SourceTable}' SourceTable"), 
			_ => new DataTable(DataSetTable), 
		};
	}

	public override string ToString()
	{
		return SourceTable;
	}
}
