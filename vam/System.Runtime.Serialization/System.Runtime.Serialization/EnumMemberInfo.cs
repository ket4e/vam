namespace System.Runtime.Serialization;

internal struct EnumMemberInfo
{
	public readonly string XmlName;

	public readonly object Value;

	public EnumMemberInfo(string name, object value)
	{
		XmlName = name;
		Value = value;
	}
}
