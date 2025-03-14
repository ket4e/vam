using System.ComponentModel;
using System.Threading;

namespace System.Timers;

[DefaultProperty("Interval")]
[DefaultEvent("Elapsed")]
public class Timer : Component, ISupportInitialize
{
	private double interval;

	private bool autoReset;

	private System.Threading.Timer timer;

	private object _lock = new object();

	private ISynchronizeInvoke so;

	[Category("Behavior")]
	[DefaultValue(true)]
	[TimersDescription("Indicates whether the timer will be restarted when it is enabled.")]
	public bool AutoReset
	{
		get
		{
			return autoReset;
		}
		set
		{
			autoReset = value;
		}
	}

	[TimersDescription("Indicates whether the timer is enabled to fire events at a defined interval.")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool Enabled
	{
		get
		{
			lock (_lock)
			{
				return timer != null;
			}
		}
		set
		{
			lock (_lock)
			{
				bool flag = timer != null;
				if (flag != value)
				{
					if (value)
					{
						timer = new System.Threading.Timer(Callback, this, (int)interval, autoReset ? ((int)interval) : 0);
						return;
					}
					timer.Dispose();
					timer = null;
				}
			}
		}
	}

	[DefaultValue(100)]
	[TimersDescription("The number of milliseconds between timer events.")]
	[RecommendedAsConfigurable(true)]
	[Category("Behavior")]
	public double Interval
	{
		get
		{
			return interval;
		}
		set
		{
			if (value <= 0.0)
			{
				throw new ArgumentException("Invalid value: " + value);
			}
			lock (_lock)
			{
				interval = value;
				if (timer != null)
				{
					timer.Change((int)interval, autoReset ? ((int)interval) : 0);
				}
			}
		}
	}

	public override ISite Site
	{
		get
		{
			return base.Site;
		}
		set
		{
			base.Site = value;
		}
	}

	[Browsable(false)]
	[DefaultValue(null)]
	[TimersDescription("The object used to marshal the event handler calls issued when an interval has elapsed.")]
	public ISynchronizeInvoke SynchronizingObject
	{
		get
		{
			return so;
		}
		set
		{
			so = value;
		}
	}

	[Category("Behavior")]
	[TimersDescription("Occurs when the Interval has elapsed.")]
	public event ElapsedEventHandler Elapsed;

	public Timer()
		: this(100.0)
	{
	}

	public Timer(double interval)
	{
		if (interval > 2147483647.0)
		{
			throw new ArgumentException("Invalid value: " + interval, "interval");
		}
		autoReset = true;
		Interval = interval;
	}

	public void BeginInit()
	{
	}

	public void Close()
	{
		Enabled = false;
	}

	public void EndInit()
	{
	}

	public void Start()
	{
		Enabled = true;
	}

	public void Stop()
	{
		Enabled = false;
	}

	protected override void Dispose(bool disposing)
	{
		Close();
		base.Dispose(disposing);
	}

	private static void Callback(object state)
	{
		Timer timer = (Timer)state;
		if (!timer.Enabled)
		{
			return;
		}
		ElapsedEventHandler elapsed = timer.Elapsed;
		if (!timer.autoReset)
		{
			timer.Enabled = false;
		}
		if (elapsed == null)
		{
			return;
		}
		ElapsedEventArgs elapsedEventArgs = new ElapsedEventArgs(DateTime.Now);
		if (timer.so != null && timer.so.InvokeRequired)
		{
			timer.so.BeginInvoke(elapsed, new object[2] { timer, elapsedEventArgs });
			return;
		}
		try
		{
			elapsed(timer, elapsedEventArgs);
		}
		catch
		{
		}
	}
}
