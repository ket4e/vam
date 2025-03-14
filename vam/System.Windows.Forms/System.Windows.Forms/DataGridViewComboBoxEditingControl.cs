using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class DataGridViewComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
{
	private DataGridView editingControlDataGridView;

	private object editingControlFormattedValue;

	private int editingControlRowIndex;

	private bool editingControlValueChanged;

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
			return editingControlFormattedValue;
		}
		set
		{
			editingControlFormattedValue = value;
		}
	}

	public virtual int EditingControlRowIndex
	{
		get
		{
			return editingControlRowIndex;
		}
		set
		{
			editingControlRowIndex = value;
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

	public virtual bool RepositionEditingControlOnValueChange => false;

	public DataGridViewComboBoxEditingControl()
	{
		editingControlValueChanged = false;
	}

	public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
	{
	}

	public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
	{
		return IsInputKey(keyData);
	}

	public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
	{
		return Text;
	}

	public virtual void PrepareEditingControlForEdit(bool selectAll)
	{
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		base.OnSelectedIndexChanged(e);
	}
}
