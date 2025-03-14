using System.Runtime.InteropServices;

namespace System.Security.Permissions;

[Serializable]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
[ComVisible(true)]
public abstract class CodeAccessSecurityAttribute : SecurityAttribute
{
	protected CodeAccessSecurityAttribute(SecurityAction action)
		: base(action)
	{
	}
}
