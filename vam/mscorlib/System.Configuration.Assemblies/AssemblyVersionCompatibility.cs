using System.Runtime.InteropServices;

namespace System.Configuration.Assemblies;

[Serializable]
[ComVisible(true)]
public enum AssemblyVersionCompatibility
{
	SameMachine = 1,
	SameProcess,
	SameDomain
}
