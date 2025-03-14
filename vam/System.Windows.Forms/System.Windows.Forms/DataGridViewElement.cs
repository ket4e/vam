using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewElement
{
	private DataGridView dataGridView;

	private DataGridViewElementStates state;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public DataGridView DataGridView => dataGridView;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual DataGridViewElementStates State => state;

	public DataGridViewElement()
	{
		dataGridView = null;
	}

	protected virtual void OnDataGridViewChanged()
	{
	}

	protected void RaiseCellClick(DataGridViewCellEventArgs e)
	{
		if (dataGridView != null)
		{
			dataGridView.InternalOnCellClick(e);
		}
	}

	protected void RaiseCellContentClick(DataGridViewCellEventArgs e)
	{
		if (dataGridView != null)
		{
			dataGridView.InternalOnCellContentClick(e);
		}
	}

	protected void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e)
	{
		if (dataGridView != null)
		{
			dataGridView.InternalOnCellContentDoubleClick(e);
		}
	}

	protected void RaiseCellValueChanged(DataGridViewCellEventArgs e)
	{
		if (dataGridView != null)
		{
			dataGridView.InternalOnCellValueChanged(e);
		}
	}

	protected void RaiseDataError(DataGridViewDataErrorEventArgs e)
	{
		if (dataGridView != null)
		{
			dataGridView.InternalOnDataError(e);
		}
	}

	protected void RaiseMouseWheel(MouseEventArgs e)
	{
		if (dataGridView != null)
		{
			dataGridView.InternalOnMouseWheel(e);
		}
	}

	internal virtual void SetDataGridView(DataGridView dataGridView)
	{
		if (dataGridView != DataGridView)
		{
			this.dataGridView = dataGridView;
			OnDataGridViewChanged();
		}
	}

	internal virtual void SetState(DataGridViewElementStates state)
	{
		this.state = state;
	}
}
