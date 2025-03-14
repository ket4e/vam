using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultProperty("GridEditName")]
[DesignTimeVisible(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ToolboxItem(false)]
public class DataGridTextBox : TextBox
{
	private bool isnavigating;

	private DataGrid grid;

	public bool IsInEditOrNavigateMode
	{
		get
		{
			return isnavigating;
		}
		set
		{
			isnavigating = value;
		}
	}

	public DataGridTextBox()
	{
		isnavigating = true;
		grid = null;
		accepts_tab = true;
		accepts_return = false;
		SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, value: false);
		SetStyle(ControlStyles.FixedHeight, value: true);
	}

	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		if (!base.ReadOnly)
		{
			isnavigating = false;
			grid.ColumnStartedEditing(Bounds);
		}
		base.OnKeyPress(e);
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
	}

	protected internal override bool ProcessKeyMessage(ref Message m)
	{
		Keys keys = (Keys)m.WParam.ToInt32();
		if (isnavigating && ProcessKeyPreview(ref m))
		{
			return true;
		}
		switch ((Msg)m.Msg)
		{
		case Msg.WM_CHAR:
		{
			Keys keys2 = keys;
			if (keys2 == Keys.Return)
			{
				isnavigating = true;
				return false;
			}
			return ProcessKeyEventArgs(ref m);
		}
		case Msg.WM_KEYDOWN:
			switch (keys)
			{
			case Keys.F2:
				base.SelectionStart = Text.Length;
				SelectionLength = 0;
				return false;
			case Keys.Left:
				if (base.SelectionStart == 0)
				{
					return ProcessKeyPreview(ref m);
				}
				return false;
			case Keys.Right:
				if (base.SelectionStart + SelectionLength >= Text.Length)
				{
					return ProcessKeyPreview(ref m);
				}
				return false;
			case Keys.Return:
				return true;
			case Keys.Tab:
			case Keys.Up:
			case Keys.Down:
				return ProcessKeyPreview(ref m);
			default:
				return ProcessKeyEventArgs(ref m);
			}
		default:
			return false;
		}
	}

	public void SetDataGrid(DataGrid parentGrid)
	{
		grid = parentGrid;
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_LBUTTONDOWN:
		case Msg.WM_LBUTTONDBLCLK:
			isnavigating = false;
			break;
		}
		base.WndProc(ref m);
	}
}
