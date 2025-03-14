using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public sealed class AssemblyFlagsAttribute : Attribute
{
	private uint flags;

	[CLSCompliant(false)]
	[Obsolete("")]
	public uint Flags => flags;

	public int AssemblyFlags => (int)flags;

	[Obsolete("")]
	[CLSCompliant(false)]
	public AssemblyFlagsAttribute(uint flags)
	{
		this.flags = flags;
	}

	[Obsolete("")]
	public AssemblyFlagsAttribute(int assemblyFlags)
	{
		flags = (uint)assemblyFlags;
	}

	public AssemblyFlagsAttribute(AssemblyNameFlags assemblyFlags)
	{
		flags = (uint)assemblyFlags;
	}
}
