namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.All)]
public sealed class AmbientValueAttribute : Attribute
{
	private object AmbientValue;

	public object Value => AmbientValue;

	public AmbientValueAttribute(bool value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(byte value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(char value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(double value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(short value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(int value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(long value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(object value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(float value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(string value)
	{
		AmbientValue = value;
	}

	public AmbientValueAttribute(Type type, string value)
	{
		try
		{
			AmbientValue = Convert.ChangeType(value, type);
		}
		catch
		{
			AmbientValue = null;
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AmbientValueAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((AmbientValueAttribute)obj).Value == AmbientValue;
	}

	public override int GetHashCode()
	{
		return AmbientValue.GetHashCode();
	}
}
