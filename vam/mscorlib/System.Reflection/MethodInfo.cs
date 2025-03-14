using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[ComVisible(true)]
[ComDefaultInterface(typeof(_MethodInfo))]
[ClassInterface(ClassInterfaceType.None)]
public abstract class MethodInfo : MethodBase, _MethodInfo
{
	public override MemberTypes MemberType => MemberTypes.Method;

	public virtual Type ReturnType => null;

	public abstract ICustomAttributeProvider ReturnTypeCustomAttributes { get; }

	public override bool IsGenericMethod => false;

	public override bool IsGenericMethodDefinition => false;

	public override bool ContainsGenericParameters => false;

	public virtual ParameterInfo ReturnParameter
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	void _MethodInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
	{
		throw new NotImplementedException();
	}

	void _MethodInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
	{
		throw new NotImplementedException();
	}

	void _MethodInfo.GetTypeInfoCount(out uint pcTInfo)
	{
		throw new NotImplementedException();
	}

	void _MethodInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
	{
		throw new NotImplementedException();
	}

	public abstract MethodInfo GetBaseDefinition();

	[ComVisible(true)]
	public virtual MethodInfo GetGenericMethodDefinition()
	{
		throw new NotSupportedException();
	}

	public virtual MethodInfo MakeGenericMethod(params Type[] typeArguments)
	{
		throw new NotSupportedException(GetType().ToString());
	}

	[ComVisible(true)]
	public override Type[] GetGenericArguments()
	{
		return Type.EmptyTypes;
	}

	virtual Type _MethodInfo.GetType()
	{
		return GetType();
	}
}
