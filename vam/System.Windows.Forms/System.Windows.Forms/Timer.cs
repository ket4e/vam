using System.ComponentModel;
using System.Threading;

namespace System.Windows.Forms;

[DefaultProperty("Interval")]
[DefaultEvent("Tick")]
[ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Allow)]
public class Timer : Component
{
	private bool enabled;

	private int interval = 100;

	private DateTime expires;

	internal Thread thread;

	internal bool Busy;

	internal IntPtr window;

	private object control_tag;

	internal static readonly int Minimum = 15;

	[DefaultValue(false)]
	public virtual bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			if (value != enabled)
			{
				enabled = value;
				if (value)
				{
					expires = DateTime.UtcNow.AddMilliseconds((interval <= Minimum) ? Minimum : interval);
					thread = Thread.CurrentThread;
					XplatUI.SetTimer(this);
				}
				else
				{
					XplatUI.KillTimer(this);
					thread = null;
				}
			}
		}
	}

	[DefaultValue(100)]
	public int Interval
	{
		get
		{
			return interval;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException("Interval", $"'{value}' is not a valid value for Interval. Interval must be greater than 0.");
			}
			if (interval != value)
			{
				interval = value;
				expires = DateTime.UtcNow.AddMilliseconds((interval <= Minimum) ? Minimum : interval);
				if (enabled)
				{
					XplatUI.KillTimer(this);
					XplatUI.SetTimer(this);
				}
			}
		}
	}

	[TypeConverter(typeof(StringConverter))]
	[MWFCategory("Data")]
	[Localizable(false)]
	[Bindable(true)]
	[DefaultValue(null)]
	public object Tag
	{
		get
		{
			return control_tag;
		}
		set
		{
			control_tag = value;
		}
	}

	internal DateTime Expires => expires;

	public event EventHandler Tick;

	public Timer()
	{
		enabled = false;
	}

	public Timer(IContainer container)
		: this()
	{
		container.Add(this);
	}

	public void Start()
	{
		Enabled = true;
	}

	public void Stop()
	{
		Enabled = false;
	}

	public override string ToString()
	{
		return base.ToString() + ", Interval: " + Interval;
	}

	internal void Update(DateTime update)
	{
		expires = update.AddMilliseconds((interval <= Minimum) ? Minimum : interval);
	}

	internal void FireTick()
	{
		OnTick(EventArgs.Empty);
	}

	protected virtual void OnTick(EventArgs e)
	{
		if (this.Tick != null)
		{
			this.Tick(this, e);
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		Enabled = false;
	}

	internal void TickHandler(object sender, EventArgs e)
	{
		OnTick(e);
	}
}
