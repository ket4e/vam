using System.Reflection;

namespace System.Windows.Forms;

public abstract class FeatureSupport : IFeatureSupport
{
	private static IFeatureSupport FeatureObject(string class_name)
	{
		Type type = Type.GetType(class_name);
		if (type != null && typeof(IFeatureSupport).IsAssignableFrom(type))
		{
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor != null)
			{
				return (IFeatureSupport)constructor.Invoke(new object[0]);
			}
		}
		return null;
	}

	public static Version GetVersionPresent(string featureClassName, string featureConstName)
	{
		return FeatureObject(featureClassName)?.GetVersionPresent(featureConstName);
	}

	public static bool IsPresent(string featureClassName, string featureConstName)
	{
		return FeatureObject(featureClassName)?.IsPresent(featureConstName) ?? false;
	}

	public static bool IsPresent(string featureClassName, string featureConstName, Version minimumVersion)
	{
		return FeatureObject(featureClassName)?.IsPresent(featureConstName, minimumVersion) ?? false;
	}

	public abstract Version GetVersionPresent(object feature);

	public virtual bool IsPresent(object feature)
	{
		if (GetVersionPresent(feature) != null)
		{
			return true;
		}
		return false;
	}

	public virtual bool IsPresent(object feature, Version minimumVersion)
	{
		bool result = false;
		Version versionPresent = GetVersionPresent(feature);
		if (versionPresent != null && versionPresent >= minimumVersion)
		{
			result = true;
		}
		return result;
	}
}
