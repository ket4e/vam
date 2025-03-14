using System.Reflection;
using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeTypeDeclaration : CodeTypeMember
{
	private CodeTypeReferenceCollection baseTypes;

	private CodeTypeMemberCollection members;

	private TypeAttributes attributes = TypeAttributes.Public;

	private bool isEnum;

	private bool isStruct;

	private int populated;

	private bool isPartial;

	private CodeTypeParameterCollection typeParameters;

	public CodeTypeReferenceCollection BaseTypes
	{
		get
		{
			if (baseTypes == null)
			{
				baseTypes = new CodeTypeReferenceCollection();
				if (this.PopulateBaseTypes != null)
				{
					this.PopulateBaseTypes(this, EventArgs.Empty);
				}
			}
			return baseTypes;
		}
	}

	public bool IsClass
	{
		get
		{
			if ((attributes & TypeAttributes.ClassSemanticsMask) != 0)
			{
				return false;
			}
			if (isEnum)
			{
				return false;
			}
			if (isStruct)
			{
				return false;
			}
			return true;
		}
		set
		{
			if (value)
			{
				attributes &= ~TypeAttributes.ClassSemanticsMask;
				isEnum = false;
				isStruct = false;
			}
		}
	}

	public bool IsEnum
	{
		get
		{
			return isEnum;
		}
		set
		{
			if (value)
			{
				attributes &= ~TypeAttributes.ClassSemanticsMask;
				isEnum = true;
				isStruct = false;
			}
		}
	}

	public bool IsInterface
	{
		get
		{
			return (attributes & TypeAttributes.ClassSemanticsMask) != 0;
		}
		set
		{
			if (value)
			{
				attributes |= TypeAttributes.ClassSemanticsMask;
				isEnum = false;
				isStruct = false;
			}
		}
	}

	public bool IsStruct
	{
		get
		{
			return isStruct;
		}
		set
		{
			if (value)
			{
				attributes &= ~TypeAttributes.ClassSemanticsMask;
				isEnum = false;
				isStruct = true;
			}
		}
	}

	public CodeTypeMemberCollection Members
	{
		get
		{
			if (members == null)
			{
				members = new CodeTypeMemberCollection();
				if (this.PopulateMembers != null)
				{
					this.PopulateMembers(this, EventArgs.Empty);
				}
			}
			return members;
		}
	}

	public TypeAttributes TypeAttributes
	{
		get
		{
			return attributes;
		}
		set
		{
			attributes = value;
		}
	}

	public bool IsPartial
	{
		get
		{
			return isPartial;
		}
		set
		{
			isPartial = value;
		}
	}

	[ComVisible(false)]
	public CodeTypeParameterCollection TypeParameters
	{
		get
		{
			if (typeParameters == null)
			{
				typeParameters = new CodeTypeParameterCollection();
			}
			return typeParameters;
		}
	}

	public event EventHandler PopulateBaseTypes;

	public event EventHandler PopulateMembers;

	public CodeTypeDeclaration()
	{
	}

	public CodeTypeDeclaration(string name)
	{
		base.Name = name;
	}
}
