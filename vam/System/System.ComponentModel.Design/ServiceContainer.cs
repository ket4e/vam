using System.Collections;

namespace System.ComponentModel.Design;

public class ServiceContainer : IDisposable, IServiceProvider, IServiceContainer
{
	private IServiceProvider parentProvider;

	private Hashtable services;

	private bool _disposed;

	private Hashtable Services
	{
		get
		{
			if (services == null)
			{
				services = new Hashtable();
			}
			return services;
		}
	}

	protected virtual Type[] DefaultServices => new Type[2]
	{
		typeof(IServiceContainer),
		typeof(ServiceContainer)
	};

	public ServiceContainer()
		: this(null)
	{
	}

	public ServiceContainer(IServiceProvider parentProvider)
	{
		this.parentProvider = parentProvider;
	}

	public void AddService(Type serviceType, object serviceInstance)
	{
		AddService(serviceType, serviceInstance, promote: false);
	}

	public void AddService(Type serviceType, ServiceCreatorCallback callback)
	{
		AddService(serviceType, callback, promote: false);
	}

	public virtual void AddService(Type serviceType, object serviceInstance, bool promote)
	{
		if (promote && parentProvider != null)
		{
			IServiceContainer serviceContainer = (IServiceContainer)parentProvider.GetService(typeof(IServiceContainer));
			serviceContainer.AddService(serviceType, serviceInstance, promote);
			return;
		}
		if (serviceType == null)
		{
			throw new ArgumentNullException("serviceType");
		}
		if (serviceInstance == null)
		{
			throw new ArgumentNullException("serviceInstance");
		}
		if (Services.Contains(serviceType))
		{
			throw new ArgumentException($"The service {serviceType.ToString()} already exists in the service container.", "serviceType");
		}
		Services.Add(serviceType, serviceInstance);
	}

	public virtual void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
	{
		if (promote && parentProvider != null)
		{
			IServiceContainer serviceContainer = (IServiceContainer)parentProvider.GetService(typeof(IServiceContainer));
			serviceContainer.AddService(serviceType, callback, promote);
			return;
		}
		if (serviceType == null)
		{
			throw new ArgumentNullException("serviceType");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		if (Services.Contains(serviceType))
		{
			throw new ArgumentException($"The service {serviceType.ToString()} already exists in the service container.", "serviceType");
		}
		Services.Add(serviceType, callback);
	}

	public void RemoveService(Type serviceType)
	{
		RemoveService(serviceType, promote: false);
	}

	public virtual void RemoveService(Type serviceType, bool promote)
	{
		if (promote && parentProvider != null)
		{
			IServiceContainer serviceContainer = (IServiceContainer)parentProvider.GetService(typeof(IServiceContainer));
			serviceContainer.RemoveService(serviceType, promote);
			return;
		}
		if (serviceType == null)
		{
			throw new ArgumentNullException("serviceType");
		}
		Services.Remove(serviceType);
	}

	public virtual object GetService(Type serviceType)
	{
		object obj = null;
		Type[] defaultServices = DefaultServices;
		for (int i = 0; i < defaultServices.Length; i++)
		{
			if (defaultServices[i] == serviceType)
			{
				obj = this;
				break;
			}
		}
		if (obj == null)
		{
			obj = Services[serviceType];
		}
		if (obj == null && parentProvider != null)
		{
			obj = parentProvider.GetService(serviceType);
		}
		if (obj != null && obj is ServiceCreatorCallback serviceCreatorCallback)
		{
			obj = serviceCreatorCallback(this, serviceType);
			Services[serviceType] = obj;
		}
		return obj;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		if (disposing && services != null)
		{
			foreach (object service in services)
			{
				if (service is IDisposable)
				{
					((IDisposable)service).Dispose();
				}
			}
			services = null;
		}
		_disposed = true;
	}
}
