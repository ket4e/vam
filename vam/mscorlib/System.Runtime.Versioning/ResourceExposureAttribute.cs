using System.Diagnostics;

namespace System.Runtime.Versioning;

[Conditional("RESOURCE_ANNOTATION_WORK")]
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
public sealed class ResourceExposureAttribute : Attribute
{
	private ResourceScope exposure;

	public ResourceScope ResourceExposureLevel => exposure;

	public ResourceExposureAttribute(ResourceScope exposureLevel)
	{
		exposure = exposureLevel;
	}
}
