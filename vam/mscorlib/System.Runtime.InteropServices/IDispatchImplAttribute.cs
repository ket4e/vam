namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[ComVisible(true)]
[Obsolete]
public sealed class IDispatchImplAttribute : Attribute
{
	private IDispatchImplType Impl;

	public IDispatchImplType Value => Impl;

	public IDispatchImplAttribute(IDispatchImplType implType)
	{
		Impl = implType;
	}

	public IDispatchImplAttribute(short implType)
	{
		Impl = (IDispatchImplType)implType;
	}
}
