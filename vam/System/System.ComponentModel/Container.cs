using System.Collections.Generic;

namespace System.ComponentModel;

public class Container : IDisposable, IContainer
{
	private class DefaultSite : IServiceProvider, ISite
	{
		private readonly IComponent component;

		private readonly Container container;

		private string name;

		public IComponent Component => component;

		public IContainer Container => container;

		public bool DesignMode => false;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public DefaultSite(string name, IComponent component, Container container)
		{
			this.component = component;
			this.container = container;
			this.name = name;
		}

		public virtual object GetService(Type t)
		{
			if (typeof(ISite) == t)
			{
				return this;
			}
			return container.GetService(t);
		}
	}

	private List<IComponent> c = new List<IComponent>();

	public virtual ComponentCollection Components
	{
		get
		{
			IComponent[] components = c.ToArray();
			return new ComponentCollection(components);
		}
	}

	public virtual void Add(IComponent component)
	{
		Add(component, null);
	}

	public virtual void Add(IComponent component, string name)
	{
		if (component != null && (component.Site == null || component.Site.Container != this))
		{
			ValidateName(component, name);
			if (component.Site != null)
			{
				component.Site.Container.Remove(component);
			}
			component.Site = CreateSite(component, name);
			c.Add(component);
		}
	}

	protected virtual void ValidateName(IComponent component, string name)
	{
		if (component == null)
		{
			throw new ArgumentNullException("component");
		}
		if (name == null)
		{
			return;
		}
		foreach (IComponent item in c)
		{
			if (object.ReferenceEquals(component, item) || item.Site == null || string.Compare(item.Site.Name, name, ignoreCase: true) != 0)
			{
				continue;
			}
			throw new ArgumentException($"There already is a named component '{name}' in this container");
		}
	}

	protected virtual ISite CreateSite(IComponent component, string name)
	{
		return new DefaultSite(name, component, this);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			while (c.Count > 0)
			{
				int index = c.Count - 1;
				IComponent component = c[index];
				Remove(component);
				component.Dispose();
			}
		}
	}

	~Container()
	{
		Dispose(disposing: false);
	}

	protected virtual object GetService(Type service)
	{
		if (typeof(IContainer) != service)
		{
			return null;
		}
		return this;
	}

	public virtual void Remove(IComponent component)
	{
		Remove(component, unsite: true);
	}

	private void Remove(IComponent component, bool unsite)
	{
		if (component != null && component.Site != null && component.Site.Container == this)
		{
			if (unsite)
			{
				component.Site = null;
			}
			c.Remove(component);
		}
	}

	protected void RemoveWithoutUnsiting(IComponent component)
	{
		Remove(component, unsite: false);
	}
}
