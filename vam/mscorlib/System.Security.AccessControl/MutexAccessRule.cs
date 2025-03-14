using System.Security.Principal;

namespace System.Security.AccessControl;

public sealed class MutexAccessRule : AccessRule
{
	private MutexRights rights;

	public MutexRights MutexRights => rights;

	public MutexAccessRule(IdentityReference identity, MutexRights eventRights, AccessControlType type)
		: base(identity, 0, isInherited: false, InheritanceFlags.None, PropagationFlags.None, type)
	{
		rights = eventRights;
	}

	public MutexAccessRule(string identity, MutexRights eventRights, AccessControlType type)
		: this(new SecurityIdentifier(identity), eventRights, type)
	{
	}
}
