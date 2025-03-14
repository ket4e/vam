using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeConstructor : CodeMemberMethod
{
	private CodeExpressionCollection baseConstructorArgs;

	private CodeExpressionCollection chainedConstructorArgs;

	public CodeExpressionCollection BaseConstructorArgs
	{
		get
		{
			if (baseConstructorArgs == null)
			{
				baseConstructorArgs = new CodeExpressionCollection();
			}
			return baseConstructorArgs;
		}
	}

	public CodeExpressionCollection ChainedConstructorArgs
	{
		get
		{
			if (chainedConstructorArgs == null)
			{
				chainedConstructorArgs = new CodeExpressionCollection();
			}
			return chainedConstructorArgs;
		}
	}

	public CodeConstructor()
	{
		base.Name = ".ctor";
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
