namespace Mono.Cecil.Cil;

public abstract class VariableReference
{
	internal int index = -1;

	protected TypeReference variable_type;

	public TypeReference VariableType
	{
		get
		{
			return variable_type;
		}
		set
		{
			variable_type = value;
		}
	}

	public int Index => index;

	internal VariableReference(TypeReference variable_type)
	{
		this.variable_type = variable_type;
	}

	public abstract VariableDefinition Resolve();

	public override string ToString()
	{
		if (index >= 0)
		{
			return "V_" + index;
		}
		return string.Empty;
	}
}
