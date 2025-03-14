using System.Runtime.InteropServices;

namespace System.ComponentModel;

[DesignerCategory("Component")]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class Component : MarshalByRefObject, IDisposable, IComponent
{
	private EventHandlerList event_handlers;

	private ISite mySite;

	private object disposedEvent = new object();

	protected virtual bool CanRaiseEvents => false;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual ISite Site
	{
		get
		{
			return mySite;
		}
		set
		{
			mySite = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public IContainer Container
	{
		get
		{
			if (mySite == null)
			{
				return null;
			}
			return mySite.Container;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	protected bool DesignMode
	{
		get
		{
			if (mySite == null)
			{
				return false;
			}
			return mySite.DesignMode;
		}
	}

	protected EventHandlerList Events
	{
		get
		{
			if (event_handlers == null)
			{
				event_handlers = new EventHandlerList();
			}
			return event_handlers;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public event EventHandler Disposed
	{
		add
		{
			Events.AddHandler(disposedEvent, value);
		}
		remove
		{
			Events.RemoveHandler(disposedEvent, value);
		}
	}

	public Component()
	{
		event_handlers = null;
	}

	~Component()
	{
		Dispose(release_all: false);
	}

	public void Dispose()
	{
		Dispose(release_all: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool release_all)
	{
		if (release_all)
		{
			if (mySite != null && mySite.Container != null)
			{
				mySite.Container.Remove(this);
			}
			((EventHandler)Events[disposedEvent])?.Invoke(this, EventArgs.Empty);
		}
	}

	protected virtual object GetService(Type service)
	{
		if (mySite != null)
		{
			return mySite.GetService(service);
		}
		return null;
	}

	public override string ToString()
	{
		if (mySite == null)
		{
			return GetType().ToString();
		}
		return $"{mySite.Name} [{GetType().ToString()}]";
	}
}
