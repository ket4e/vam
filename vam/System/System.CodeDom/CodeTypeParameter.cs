using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeTypeParameter : CodeObject
{
	private CodeTypeReferenceCollection constraints;

	private CodeAttributeDeclarationCollection customAttributes;

	private bool hasConstructorConstraint;

	private string name;

	public CodeTypeReferenceCollection Constraints
	{
		get
		{
			if (constraints == null)
			{
				constraints = new CodeTypeReferenceCollection();
			}
			return constraints;
		}
	}

	public CodeAttributeDeclarationCollection CustomAttributes
	{
		get
		{
			if (customAttributes == null)
			{
				customAttributes = new CodeAttributeDeclarationCollection();
			}
			return customAttributes;
		}
	}

	public bool HasConstructorConstraint
	{
		get
		{
			return hasConstructorConstraint;
		}
		set
		{
			hasConstructorConstraint = value;
		}
	}

	public string Name
	{
		get
		{
			if (name == null)
			{
				return string.Empty;
			}
			return name;
		}
		set
		{
			name = value;
		}
	}

	public CodeTypeParameter()
	{
	}

	public CodeTypeParameter(string name)
	{
		this.name = name;
	}
}
