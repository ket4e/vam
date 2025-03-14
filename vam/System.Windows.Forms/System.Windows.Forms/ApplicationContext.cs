using System.ComponentModel;

namespace System.Windows.Forms;

public class ApplicationContext : IDisposable
{
	private Form main_form;

	private object tag;

	private bool thread_exit_raised;

	public Form MainForm
	{
		get
		{
			return main_form;
		}
		set
		{
			if (main_form != value)
			{
				if (main_form != null)
				{
					main_form.HandleDestroyed -= OnMainFormClosed;
				}
				main_form = value;
				if (main_form != null)
				{
					main_form.HandleDestroyed += OnMainFormClosed;
				}
			}
		}
	}

	[Bindable(true)]
	[Localizable(false)]
	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
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

	public event EventHandler ThreadExit;

	public ApplicationContext()
		: this(null)
	{
	}

	public ApplicationContext(Form mainForm)
	{
		MainForm = mainForm;
	}

	~ApplicationContext()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void ExitThread()
	{
		ExitThreadCore();
	}

	protected virtual void Dispose(bool disposing)
	{
		MainForm = null;
		tag = null;
	}

	protected virtual void ExitThreadCore()
	{
		if (Application.MWFThread.Current.Context == this)
		{
			XplatUI.PostQuitMessage(0);
		}
		if (!thread_exit_raised && this.ThreadExit != null)
		{
			thread_exit_raised = true;
			this.ThreadExit(this, EventArgs.Empty);
		}
	}

	protected virtual void OnMainFormClosed(object sender, EventArgs e)
	{
		if (!MainForm.RecreatingHandle)
		{
			ExitThreadCore();
		}
	}
}
