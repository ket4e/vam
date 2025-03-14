namespace System.ComponentModel;

public class NestedContainer : Container, IDisposable, IContainer, INestedContainer
{
	private class Site : IServiceProvider, INestedSite, ISite
	{
		private IComponent _component;

		private NestedContainer _nestedContainer;

		private string _siteName;

		public IComponent Component => _component;

		public IContainer Container => _nestedContainer;

		public bool DesignMode
		{
			get
			{
				if (_nestedContainer.Owner != null && _nestedContainer.Owner.Site != null)
				{
					return _nestedContainer.Owner.Site.DesignMode;
				}
				return false;
			}
		}

		public string Name
		{
			get
			{
				return _siteName;
			}
			set
			{
				_siteName = value;
			}
		}

		public string FullName
		{
			get
			{
				if (_siteName == null)
				{
					return null;
				}
				if (_nestedContainer.OwnerName == null)
				{
					return _siteName;
				}
				return _nestedContainer.OwnerName + "." + _siteName;
			}
		}

		public Site(IComponent component, NestedContainer container, string name)
		{
			_component = component;
			_nestedContainer = container;
			_siteName = name;
		}

		public virtual object GetService(Type service)
		{
			if (service == typeof(ISite))
			{
				return this;
			}
			return _nestedContainer.GetService(service);
		}
	}

	private IComponent _owner;

	public IComponent Owner => _owner;

	protected virtual string OwnerName
	{
		get
		{
			if (_owner.Site is INestedSite)
			{
				return ((INestedSite)_owner.Site).FullName;
			}
			if (_owner == null || _owner.Site == null)
			{
				return null;
			}
			return _owner.Site.Name;
		}
	}

	public NestedContainer(IComponent owner)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		_owner = owner;
		_owner.Disposed += OnOwnerDisposed;
	}

	protected override ISite CreateSite(IComponent component, string name)
	{
		if (component == null)
		{
			throw new ArgumentNullException("component");
		}
		return new Site(component, this, name);
	}

	protected override object GetService(Type service)
	{
		if (service == typeof(INestedContainer))
		{
			return this;
		}
		return base.GetService(service);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_owner.Disposed -= OnOwnerDisposed;
		}
		base.Dispose(disposing);
	}

	private void OnOwnerDisposed(object sender, EventArgs e)
	{
		Dispose();
	}
}
