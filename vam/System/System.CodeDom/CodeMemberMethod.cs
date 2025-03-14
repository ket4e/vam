using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeMemberMethod : CodeTypeMember
{
	private CodeTypeReferenceCollection implementationTypes;

	private CodeParameterDeclarationExpressionCollection parameters;

	private CodeTypeReference privateImplements;

	private CodeTypeReference returnType;

	private CodeStatementCollection statements;

	private CodeAttributeDeclarationCollection returnAttributes;

	private int populated;

	private CodeTypeParameterCollection typeParameters;

	public CodeTypeReferenceCollection ImplementationTypes
	{
		get
		{
			if (implementationTypes == null)
			{
				implementationTypes = new CodeTypeReferenceCollection();
				if (this.PopulateImplementationTypes != null)
				{
					this.PopulateImplementationTypes(this, EventArgs.Empty);
				}
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
				if (this.PopulateParameters != null)
				{
					this.PopulateParameters(this, EventArgs.Empty);
				}
			}
			return parameters;
		}
	}

	public CodeTypeReference PrivateImplementationType
	{
		get
		{
			return privateImplements;
		}
		set
		{
			privateImplements = value;
		}
	}

	public CodeTypeReference ReturnType
	{
		get
		{
			if (returnType == null)
			{
				return new CodeTypeReference(typeof(void));
			}
			return returnType;
		}
		set
		{
			returnType = value;
		}
	}

	public CodeStatementCollection Statements
	{
		get
		{
			if (statements == null)
			{
				statements = new CodeStatementCollection();
				if (this.PopulateStatements != null)
				{
					this.PopulateStatements(this, EventArgs.Empty);
				}
			}
			return statements;
		}
	}

	public CodeAttributeDeclarationCollection ReturnTypeCustomAttributes
	{
		get
		{
			if (returnAttributes == null)
			{
				returnAttributes = new CodeAttributeDeclarationCollection();
			}
			return returnAttributes;
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

	public event EventHandler PopulateImplementationTypes;

	public event EventHandler PopulateParameters;

	public event EventHandler PopulateStatements;

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
