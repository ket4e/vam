namespace System.ComponentModel.Design.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
public sealed class DesignerSerializerAttribute : Attribute
{
	private string serializerTypeName;

	private string baseSerializerTypeName;

	public string SerializerBaseTypeName => baseSerializerTypeName;

	public string SerializerTypeName => serializerTypeName;

	public override object TypeId => ToString() + baseSerializerTypeName;

	public DesignerSerializerAttribute(string serializerTypeName, string baseSerializerTypeName)
	{
		this.serializerTypeName = serializerTypeName;
		this.baseSerializerTypeName = baseSerializerTypeName;
	}

	public DesignerSerializerAttribute(string serializerTypeName, Type baseSerializerType)
		: this(serializerTypeName, baseSerializerType.AssemblyQualifiedName)
	{
	}

	public DesignerSerializerAttribute(Type serializerType, Type baseSerializerType)
		: this(serializerType.AssemblyQualifiedName, baseSerializerType.AssemblyQualifiedName)
	{
	}
}
