using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeMemberEvent : CodeTypeMember
{
	private CodeTypeReferenceCollection implementationTypes;

	private CodeTypeReference privateImplementationType;

	private CodeTypeReference type;

	public CodeTypeReferenceCollection ImplementationTypes
	{
		get
		{
			if (implementationTypes == null)
			{
				implementationTypes = new CodeTypeReferenceCollection();
			}
			return implementationTypes;
		}
	}

	public CodeTypeReference PrivateImplementationType
	{
		get
		{
			return privateImplementationType;
		}
		set
		{
			privateImplementationType = value;
		}
	}

	public CodeTypeReference Type
	{
		get
		{
			if (type == null)
			{
				type = new CodeTypeReference(string.Empty);
			}
			return type;
		}
		set
		{
			type = value;
		}
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
