using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Reflection;

[Serializable]
[ComDefaultInterface(typeof(_MemberInfo))]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.None)]
[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\"\n               Unrestricted=\"true\"/>\n")]
public abstract class MemberInfo : ICustomAttributeProvider, _MemberInfo
{
	public abstract Type DeclaringType { get; }

	public abstract MemberTypes MemberType { get; }

	public abstract string Name { get; }

	public abstract Type ReflectedType { get; }

	public virtual Module Module => DeclaringType.Module;

	public virtual extern int MetadataToken
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	void _MemberInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
	{
		throw new NotImplementedException();
	}

	void _MemberInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
	{
		throw new NotImplementedException();
	}

	void _MemberInfo.GetTypeInfoCount(out uint pcTInfo)
	{
		throw new NotImplementedException();
	}

	void _MemberInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
	{
		throw new NotImplementedException();
	}

	public abstract bool IsDefined(Type attributeType, bool inherit);

	public abstract object[] GetCustomAttributes(bool inherit);

	public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);

	virtual Type _MemberInfo.GetType()
	{
		return GetType();
	}
}
