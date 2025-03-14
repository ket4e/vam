using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices;

[Serializable]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Method)]
[ComVisible(true)]
public class CompilationRelaxationsAttribute : Attribute
{
	private int relax;

	public int CompilationRelaxations => relax;

	public CompilationRelaxationsAttribute(int relaxations)
	{
		relax = relaxations;
	}

	public CompilationRelaxationsAttribute(CompilationRelaxations relaxations)
	{
		relax = (int)relaxations;
	}
}
