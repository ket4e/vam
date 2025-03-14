namespace System.Security;

[MonoTODO("Only supported by the runtime when CoreCLR is enabled")]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
public sealed class SecurityCriticalAttribute : Attribute
{
	private SecurityCriticalScope _scope;

	public SecurityCriticalScope Scope => _scope;

	public SecurityCriticalAttribute()
	{
		_scope = SecurityCriticalScope.Explicit;
	}

	public SecurityCriticalAttribute(SecurityCriticalScope scope)
	{
		if (scope == SecurityCriticalScope.Everything)
		{
			_scope = SecurityCriticalScope.Everything;
		}
		else
		{
			_scope = SecurityCriticalScope.Explicit;
		}
	}
}
