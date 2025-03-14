using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb;

[ToolboxItem("Microsoft.VSDesigner.Data.VS.OleDbDataAdapterToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[Designer("Microsoft.VSDesigner.Data.VS.OleDbDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultEvent("RowUpdated")]
public sealed class OleDbDataAdapter : DbDataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
{
	private OleDbCommand deleteCommand;

	private OleDbCommand insertCommand;

	private OleDbCommand selectCommand;

	private OleDbCommand updateCommand;

	IDbCommand IDbDataAdapter.DeleteCommand
	{
		get
		{
			return DeleteCommand;
		}
		set
		{
			DeleteCommand = (OleDbCommand)value;
		}
	}

	IDbCommand IDbDataAdapter.InsertCommand
	{
		get
		{
			return InsertCommand;
		}
		set
		{
			InsertCommand = (OleDbCommand)value;
		}
	}

	IDbCommand IDbDataAdapter.SelectCommand
	{
		get
		{
			return SelectCommand;
		}
		set
		{
			SelectCommand = (OleDbCommand)value;
		}
	}

	IDbCommand IDbDataAdapter.UpdateCommand
	{
		get
		{
			return UpdateCommand;
		}
		set
		{
			UpdateCommand = (OleDbCommand)value;
		}
	}

	[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DataCategory("Update")]
	[DefaultValue(null)]
	public new OleDbCommand DeleteCommand
	{
		get
		{
			return deleteCommand;
		}
		set
		{
			deleteCommand = value;
		}
	}

	[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DataCategory("Update")]
	[DefaultValue(null)]
	public new OleDbCommand InsertCommand
	{
		get
		{
			return insertCommand;
		}
		set
		{
			insertCommand = value;
		}
	}

	[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultValue(null)]
	[DataCategory("Fill")]
	public new OleDbCommand SelectCommand
	{
		get
		{
			return selectCommand;
		}
		set
		{
			selectCommand = value;
		}
	}

	[DataCategory("Update")]
	[DefaultValue(null)]
	[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public new OleDbCommand UpdateCommand
	{
		get
		{
			return updateCommand;
		}
		set
		{
			updateCommand = value;
		}
	}

	[DataCategory("DataCategory_Update")]
	public event OleDbRowUpdatedEventHandler RowUpdated;

	[DataCategory("DataCategory_Update")]
	public event OleDbRowUpdatingEventHandler RowUpdating;

	public OleDbDataAdapter()
		: this(null)
	{
	}

	public OleDbDataAdapter(OleDbCommand selectCommand)
	{
		SelectCommand = selectCommand;
	}

	public OleDbDataAdapter(string selectCommandText, OleDbConnection selectConnection)
		: this(new OleDbCommand(selectCommandText, selectConnection))
	{
	}

	public OleDbDataAdapter(string selectCommandText, string selectConnectionString)
		: this(selectCommandText, new OleDbConnection(selectConnectionString))
	{
	}

	[System.MonoTODO]
	object ICloneable.Clone()
	{
		throw new NotImplementedException();
	}

	protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
	{
		return new OleDbRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
	}

	protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
	{
		return new OleDbRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
	}

	protected override void OnRowUpdated(RowUpdatedEventArgs value)
	{
		if (this.RowUpdated != null)
		{
			this.RowUpdated(this, (OleDbRowUpdatedEventArgs)value);
		}
	}

	protected override void OnRowUpdating(RowUpdatingEventArgs value)
	{
		if (this.RowUpdating != null)
		{
			this.RowUpdating(this, (OleDbRowUpdatingEventArgs)value);
		}
	}

	[System.MonoTODO]
	public int Fill(DataTable dataTable, object ADODBRecordSet)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public int Fill(DataSet dataSet, object ADODBRecordSet, string srcTable)
	{
		throw new NotImplementedException();
	}
}
