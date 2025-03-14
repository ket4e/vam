using System.Reflection;

namespace System.Diagnostics;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
[System.MonoLimitation("This attribute is not considered in trace support.")]
public sealed class SwitchAttribute : Attribute
{
	private string name;

	private string desc = string.Empty;

	private Type type;

	public string SwitchName
	{
		get
		{
			return name;
		}
		set
		{
			if (name == null)
			{
				throw new ArgumentNullException("value");
			}
			name = value;
		}
	}

	public string SwitchDescription
	{
		get
		{
			return desc;
		}
		set
		{
			if (desc == null)
			{
				throw new ArgumentNullException("value");
			}
			desc = value;
		}
	}

	public Type SwitchType
	{
		get
		{
			return type;
		}
		set
		{
			if (type == null)
			{
				throw new ArgumentNullException("value");
			}
			type = value;
		}
	}

	public SwitchAttribute(string switchName, Type switchType)
	{
		if (switchName == null)
		{
			throw new ArgumentNullException("switchName");
		}
		if (switchType == null)
		{
			throw new ArgumentNullException("switchType");
		}
		name = switchName;
		type = switchType;
	}

	public static SwitchAttribute[] GetAll(Assembly assembly)
	{
		object[] customAttributes = assembly.GetCustomAttributes(typeof(SwitchAttribute), inherit: false);
		SwitchAttribute[] array = new SwitchAttribute[customAttributes.Length];
		for (int i = 0; i < customAttributes.Length; i++)
		{
			array[i] = (SwitchAttribute)customAttributes[i];
		}
		return array;
	}
}
