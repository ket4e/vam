namespace System.Windows.Forms.CarbonInternal;

internal abstract class EventHandlerBase
{
	internal XplatUICarbon Driver;

	public EventHandlerBase()
	{
	}

	public EventHandlerBase(XplatUICarbon driver)
	{
		Driver = driver;
	}
}
