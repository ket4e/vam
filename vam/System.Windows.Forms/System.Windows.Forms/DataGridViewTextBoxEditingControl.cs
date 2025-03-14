using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class DataGridViewTextBoxEditingControl : TextBox, IDataGridViewEditingControl
{
	private DataGridView editingControlDataGridView;

	private int rowIndex;

	private bool editingControlValueChanged;

	private bool repositionEditingControlOnValueChange;

	public virtual DataGridView EditingControlDataGridView
	{
		get
		{
			return editingControlDataGridView;
		}
		set
		{
			editingControlDataGridView = value;
		}
	}

	public virtual object EditingControlFormattedValue
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = (string)value;
		}
	}

	public virtual int EditingControlRowIndex
	{
		get
		{
			return rowIndex;
		}
		set
		{
			rowIndex = value;
		}
	}

	public virtual bool EditingControlValueChanged
	{
		get
		{
			return editingControlValueChanged;
		}
		set
		{
			editingControlValueChanged = value;
		}
	}

	public virtual Cursor EditingPanelCursor => Cursors.Default;

	public virtual bool RepositionEditingControlOnValueChange => repositionEditingControlOnValueChange;

	public DataGridViewTextBoxEditingControl()
	{
		repositionEditingControlOnValueChange = false;
	}

	public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
	{
		Font = dataGridViewCellStyle.Font;
		BackColor = dataGridViewCellStyle.BackColor;
		ForeColor = dataGridViewCellStyle.ForeColor;
	}

	public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
	{
		switch (keyData)
		{
		case Keys.Left:
			return base.SelectionStart != 0;
		case Keys.Right:
			return base.SelectionStart != TextLength;
		case Keys.Up:
		case Keys.Down:
			return false;
		default:
			return true;
		}
	}

	public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
	{
		return EditingControlFormattedValue;
	}

	public virtual void PrepareEditingControlForEdit(bool selectAll)
	{
		Focus();
		if (selectAll)
		{
			SelectAll();
		}
		editingControlValueChanged = false;
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		base.OnMouseWheel(e);
	}

	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
		editingControlValueChanged = true;
	}

	protected override bool ProcessKeyEventArgs(ref Message m)
	{
		return base.ProcessKeyEventArgs(ref m);
	}
}
