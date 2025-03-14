using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc;

[DefaultEvent("RowUpdated")]
[Designer("Microsoft.VSDesigner.Data.VS.OdbcDataAdapterDesigner, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ToolboxItem("Microsoft.VSDesigner.Data.VS.OdbcDataAdapterToolboxItem, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public sealed class OdbcDataAdapter : DbDataAdapter, IDataAdapter, IDbDataAdapter, ICloneable
{
	private OdbcCommand deleteCommand;

	private OdbcCommand insertCommand;

	private OdbcCommand selectCommand;

	private OdbcCommand updateCommand;

	IDbCommand IDbDataAdapter.DeleteCommand
	{
		get
		{
			return DeleteCommand;
		}
		set
		{
			DeleteCommand = (OdbcCommand)value;
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
			InsertCommand = (OdbcCommand)value;
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
			SelectCommand = (OdbcCommand)value;
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
			UpdateCommand = (OdbcCommand)value;
		}
	}

	[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[OdbcDescription("Used during Update for deleted rows in DataSet.")]
	[OdbcCategory("Update")]
	[DefaultValue(null)]
	public new OdbcCommand DeleteCommand
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
	[OdbcDescription("Used during Update for new rows in DataSet.")]
	[OdbcCategory("Update")]
	[DefaultValue(null)]
	public new OdbcCommand InsertCommand
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
	[OdbcCategory("Fill")]
	[OdbcDescription("Used during Fill/FillSchema.")]
	[DefaultValue(null)]
	public new OdbcCommand SelectCommand
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

	[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[OdbcCategory("Update")]
	[OdbcDescription("Used during Update for modified rows in DataSet.")]
	[DefaultValue(null)]
	public new OdbcCommand UpdateCommand
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

	public event OdbcRowUpdatedEventHandler RowUpdated;

	public event OdbcRowUpdatingEventHandler RowUpdating;

	public OdbcDataAdapter()
		: this(null)
	{
	}

	public OdbcDataAdapter(OdbcCommand selectCommand)
	{
		SelectCommand = selectCommand;
	}

	public OdbcDataAdapter(string selectCommandText, OdbcConnection selectConnection)
		: this(new OdbcCommand(selectCommandText, selectConnection))
	{
	}

	public OdbcDataAdapter(string selectCommandText, string selectConnectionString)
		: this(selectCommandText, new OdbcConnection(selectConnectionString))
	{
	}

	[System.MonoTODO]
	object ICloneable.Clone()
	{
		throw new NotImplementedException();
	}

	protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
	{
		return new OdbcRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
	}

	protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
	{
		return new OdbcRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
	}

	protected override void OnRowUpdated(RowUpdatedEventArgs value)
	{
		if (this.RowUpdated != null)
		{
			this.RowUpdated(this, (OdbcRowUpdatedEventArgs)value);
		}
	}

	protected override void OnRowUpdating(RowUpdatingEventArgs value)
	{
		if (this.RowUpdating != null)
		{
			this.RowUpdating(this, (OdbcRowUpdatingEventArgs)value);
		}
	}
}
