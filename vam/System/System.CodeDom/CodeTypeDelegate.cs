using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeTypeDelegate : CodeTypeDeclaration
{
	private CodeParameterDeclarationExpressionCollection parameters;

	private CodeTypeReference returnType;

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

	public CodeTypeReference ReturnType
	{
		get
		{
			if (returnType == null)
			{
				returnType = new CodeTypeReference(string.Empty);
			}
			return returnType;
		}
		set
		{
			returnType = value;
		}
	}

	public CodeTypeDelegate()
	{
		base.BaseTypes.Add(new CodeTypeReference("System.Delegate"));
	}

	public CodeTypeDelegate(string name)
		: this()
	{
		base.Name = name;
	}
}
