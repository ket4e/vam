using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeConditionStatement : CodeStatement
{
	private CodeExpression condition;

	private CodeStatementCollection trueStatements;

	private CodeStatementCollection falseStatements;

	public CodeExpression Condition
	{
		get
		{
			return condition;
		}
		set
		{
			condition = value;
		}
	}

	public CodeStatementCollection FalseStatements
	{
		get
		{
			if (falseStatements == null)
			{
				falseStatements = new CodeStatementCollection();
			}
			return falseStatements;
		}
	}

	public CodeStatementCollection TrueStatements
	{
		get
		{
			if (trueStatements == null)
			{
				trueStatements = new CodeStatementCollection();
			}
			return trueStatements;
		}
	}

	public CodeConditionStatement()
	{
	}

	public CodeConditionStatement(CodeExpression condition, params CodeStatement[] trueStatements)
	{
		this.condition = condition;
		TrueStatements.AddRange(trueStatements);
	}

	public CodeConditionStatement(CodeExpression condition, CodeStatement[] trueStatements, CodeStatement[] falseStatements)
	{
		this.condition = condition;
		TrueStatements.AddRange(trueStatements);
		FalseStatements.AddRange(falseStatements);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
