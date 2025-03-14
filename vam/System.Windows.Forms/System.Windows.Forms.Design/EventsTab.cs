using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

public class EventsTab : PropertyTab
{
	private IServiceProvider serviceProvider;

	public override string HelpKeyword => TabName;

	public override string TabName => Locale.GetText("Events");

	private EventsTab()
	{
	}

	public EventsTab(IServiceProvider sp)
	{
		serviceProvider = sp;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
	{
		IEventBindingService eventBindingService = null;
		EventDescriptorCollection eventDescriptorCollection = null;
		if (serviceProvider != null)
		{
			eventBindingService = (IEventBindingService)serviceProvider.GetService(typeof(IEventBindingService));
		}
		if (eventBindingService == null)
		{
			return new PropertyDescriptorCollection(null);
		}
		eventDescriptorCollection = ((attributes == null) ? TypeDescriptor.GetEvents(component) : TypeDescriptor.GetEvents(component, attributes));
		return eventBindingService.GetEventProperties(eventDescriptorCollection);
	}

	public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
	{
		return GetProperties(null, component, attributes);
	}

	public override bool CanExtend(object extendee)
	{
		return false;
	}

	public override PropertyDescriptor GetDefaultProperty(object obj)
	{
		if (serviceProvider == null)
		{
			return null;
		}
		EventDescriptor defaultEvent = TypeDescriptor.GetDefaultEvent(obj);
		IEventBindingService eventBindingService = (IEventBindingService)serviceProvider.GetService(typeof(IEventBindingService));
		if (defaultEvent != null && eventBindingService != null)
		{
			return eventBindingService.GetEventProperty(defaultEvent);
		}
		return null;
	}
}
