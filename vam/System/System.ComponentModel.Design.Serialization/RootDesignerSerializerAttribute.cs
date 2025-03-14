namespace System.ComponentModel.Design.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
[Obsolete("Use DesignerSerializerAttribute instead")]
public sealed class RootDesignerSerializerAttribute : Attribute
{
	private string serializer;

	private string baseserializer;

	private bool reload;

	public bool Reloadable => reload;

	public string SerializerBaseTypeName => baseserializer;

	public string SerializerTypeName => serializer;

	public override object TypeId => ToString() + baseserializer;

	public RootDesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName, bool reloadable)
	{
		serializer = serializerTypeName;
		baseserializer = baseSerializerTypeName;
		reload = reloadable;
	}

	public RootDesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType, bool reloadable)
		: this(serializerTypeName, baseSerializerType.AssemblyQualifiedName, reloadable)
	{
	}

	public RootDesignerSerializerAttribute(Type serializerType, Type baseSerializerType, bool reloadable)
		: this(serializerType.AssemblyQualifiedName, baseSerializerType.AssemblyQualifiedName, reloadable)
	{
	}
}
