using System.ComponentModel;

namespace System.Windows.Forms;

[ToolboxItemFilter("System.Windows.Forms")]
public abstract class CommonDialog : Component
{
	internal class DialogForm : Form
	{
		protected CommonDialog owner;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.Style |= -2134376448;
				return createParams;
			}
		}

		internal DialogForm(CommonDialog owner)
		{
			this.owner = owner;
			base.ControlBox = true;
			base.MinimizeBox = false;
			base.MaximizeBox = false;
			base.ShowInTaskbar = false;
			base.FormBorderStyle = FormBorderStyle.Sizable;
			base.StartPosition = FormStartPosition.CenterScreen;
		}

		internal DialogResult RunDialog()
		{
			owner.InitFormsSize(this);
			ShowDialog();
			return base.DialogResult;
		}
	}

	internal DialogForm form;

	private object tag;

	private static object HelpRequestEvent;

	[Bindable(true)]
	[Localizable(false)]
	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
	[MWFCategory("Data")]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public event EventHandler HelpRequest
	{
		add
		{
			base.Events.AddHandler(HelpRequestEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(HelpRequestEvent, value);
		}
	}

	public CommonDialog()
	{
	}

	static CommonDialog()
	{
		HelpRequest = new object();
	}

	internal virtual void InitFormsSize(Form form)
	{
		form.Width = 200;
		form.Height = 200;
	}

	public abstract void Reset();

	public DialogResult ShowDialog()
	{
		return ShowDialog(null);
	}

	public DialogResult ShowDialog(IWin32Window owner)
	{
		if (form == null)
		{
			if (RunDialog(owner?.Handle ?? IntPtr.Zero))
			{
				return DialogResult.OK;
			}
			return DialogResult.Cancel;
		}
		if (RunDialog(form.Handle))
		{
			form.ShowDialog(owner);
		}
		return form.DialogResult;
	}

	protected virtual IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
	{
		return IntPtr.Zero;
	}

	protected virtual void OnHelpRequest(EventArgs e)
	{
		((EventHandler)base.Events[HelpRequest])?.Invoke(this, e);
	}

	protected virtual IntPtr OwnerWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
	{
		return IntPtr.Zero;
	}

	protected abstract bool RunDialog(IntPtr hwndOwner);
}
