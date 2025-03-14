using System.Collections;

namespace System.Windows.Forms;

internal class DataGridDataSource
{
	public DataGrid owner;

	public CurrencyManager list_manager;

	public object view;

	public string data_member;

	public object data_source;

	public DataGridCell current;

	private DataGridRelationshipRow[] rows;

	private Hashtable selected_rows;

	private int selection_start;

	public DataGridRelationshipRow[] Rows
	{
		get
		{
			return rows;
		}
		set
		{
			rows = value;
		}
	}

	public Hashtable SelectedRows
	{
		get
		{
			return selected_rows;
		}
		set
		{
			selected_rows = value;
		}
	}

	public int SelectionStart
	{
		get
		{
			return selection_start;
		}
		set
		{
			selection_start = value;
		}
	}

	public DataGridDataSource(DataGrid owner, CurrencyManager list_manager, object data_source, string data_member, object view_data, DataGridCell current)
	{
		this.owner = owner;
		this.list_manager = list_manager;
		view = view_data;
		this.data_source = data_source;
		this.data_member = data_member;
		this.current = current;
	}
}
