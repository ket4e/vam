using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeMemberProperty : CodeTypeMember
{
	private CodeStatementCollection getStatements;

	private bool hasGet;

	private bool hasSet;

	private CodeTypeReferenceCollection implementationTypes;

	private CodeParameterDeclarationExpressionCollection parameters;

	private CodeTypeReference privateImplementationType;

	private CodeStatementCollection setStatements;

	private CodeTypeReference type;

	public CodeStatementCollection GetStatements
	{
		get
		{
			if (getStatements == null)
			{
				getStatements = new CodeStatementCollection();
			}
			return getStatements;
		}
	}

	public bool HasGet
	{
		get
		{
			return hasGet || (getStatements != null && getStatements.Count > 0);
		}
		set
		{
			hasGet = value;
			if (!hasGet && getStatements != null)
			{
				getStatements.Clear();
			}
		}
	}

	public bool HasSet
	{
		get
		{
			return hasSet || (setStatements != null && setStatements.Count > 0);
		}
		set
		{
			hasSet = value;
			if (!hasSet && setStatements != null)
			{
				setStatements.Clear();
			}
		}
	}

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

	public CodeParameterDeclarationExpressionCollection Parameters
	{
		get
		{
			if (parameters == null)
			{
				parameters = new CodeParameterDeclarationExpressionCollection();
			}
			return parameters;
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

	public CodeStatementCollection SetStatements
	{
		get
		{
			if (setStatements == null)
			{
				setStatements = new CodeStatementCollection();
			}
			return setStatements;
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
