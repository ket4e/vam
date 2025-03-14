using System.Diagnostics;

namespace System.Runtime.Versioning;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
[Conditional("RESOURCE_ANNOTATION_WORK")]
public sealed class ResourceConsumptionAttribute : Attribute
{
	private ResourceScope resource;

	private ResourceScope consumption;

	public ResourceScope ConsumptionScope => consumption;

	public ResourceScope ResourceScope => resource;

	public ResourceConsumptionAttribute(ResourceScope resourceScope)
	{
		resource = resourceScope;
		consumption = resourceScope;
	}

	public ResourceConsumptionAttribute(ResourceScope resourceScope, ResourceScope consumptionScope)
	{
		resource = resourceScope;
		consumption = consumptionScope;
	}
}
