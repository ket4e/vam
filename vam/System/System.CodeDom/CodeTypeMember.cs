using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeTypeMember : CodeObject
{
	private string name;

	private MemberAttributes attributes;

	private CodeCommentStatementCollection comments;

	private CodeAttributeDeclarationCollection customAttributes;

	private CodeLinePragma linePragma;

	private CodeDirectiveCollection endDirectives;

	private CodeDirectiveCollection startDirectives;

	public MemberAttributes Attributes
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

	public CodeCommentStatementCollection Comments
	{
		get
		{
			if (comments == null)
			{
				comments = new CodeCommentStatementCollection();
			}
			return comments;
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
		set
		{
			customAttributes = value;
		}
	}

	public CodeLinePragma LinePragma
	{
		get
		{
			return linePragma;
		}
		set
		{
			linePragma = value;
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

	public CodeDirectiveCollection EndDirectives
	{
		get
		{
			if (endDirectives == null)
			{
				endDirectives = new CodeDirectiveCollection();
			}
			return endDirectives;
		}
	}

	public CodeDirectiveCollection StartDirectives
	{
		get
		{
			if (startDirectives == null)
			{
				startDirectives = new CodeDirectiveCollection();
			}
			return startDirectives;
		}
	}

	public CodeTypeMember()
	{
		attributes = (MemberAttributes)20482;
	}
}
