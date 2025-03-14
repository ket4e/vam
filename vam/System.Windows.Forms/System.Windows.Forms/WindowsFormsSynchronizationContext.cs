using System.ComponentModel;
using System.Threading;

namespace System.Windows.Forms;

public sealed class WindowsFormsSynchronizationContext : SynchronizationContext, IDisposable
{
	private static bool auto_installed;

	private static Control invoke_control;

	private static SynchronizationContext previous_context;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static bool AutoInstall
	{
		get
		{
			return auto_installed;
		}
		set
		{
			auto_installed = value;
		}
	}

	static WindowsFormsSynchronizationContext()
	{
		invoke_control = new Control();
		invoke_control.CreateControl();
		auto_installed = true;
		previous_context = SynchronizationContext.Current;
	}

	public override SynchronizationContext CreateCopy()
	{
		return base.CreateCopy();
	}

	public void Dispose()
	{
	}

	public override void Post(SendOrPostCallback d, object state)
	{
		invoke_control.BeginInvoke(d, state);
	}

	public override void Send(SendOrPostCallback d, object state)
	{
		invoke_control.Invoke(d, state);
	}

	public static void Uninstall()
	{
		if (previous_context == null)
		{
			previous_context = new SynchronizationContext();
		}
		SynchronizationContext.SetSynchronizationContext(previous_context);
	}
}
