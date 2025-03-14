using System.Runtime.InteropServices;

namespace System.Reflection;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
[ComVisible(true)]
public sealed class ObfuscateAssemblyAttribute : Attribute
{
	private bool is_private;

	private bool strip;

	public bool AssemblyIsPrivate => is_private;

	public bool StripAfterObfuscation
	{
		get
		{
			return strip;
		}
		set
		{
			strip = value;
		}
	}

	public ObfuscateAssemblyAttribute(bool assemblyIsPrivate)
	{
		strip = true;
		is_private = assemblyIsPrivate;
	}
}
