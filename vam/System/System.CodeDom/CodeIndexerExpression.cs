using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeIndexerExpression : CodeExpression
{
	private CodeExpression targetObject;

	private CodeExpressionCollection indices;

	public CodeExpressionCollection Indices
	{
		get
		{
			if (indices == null)
			{
				indices = new CodeExpressionCollection();
			}
			return indices;
		}
	}

	public CodeExpression TargetObject
	{
		get
		{
			return targetObject;
		}
		set
		{
			targetObject = value;
		}
	}

	public CodeIndexerExpression()
	{
	}

	public CodeIndexerExpression(CodeExpression targetObject, params CodeExpression[] indices)
	{
		this.targetObject = targetObject;
		Indices.AddRange(indices);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
