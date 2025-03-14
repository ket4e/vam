using System;

namespace NAudio.Utils;

[AttributeUsage(AttributeTargets.Field)]
public class FieldDescriptionAttribute : Attribute
{
	public string Description { get; }

	public FieldDescriptionAttribute(string description)
	{
		Description = description;
	}

	public override string ToString()
	{
		return Description;
	}
}
