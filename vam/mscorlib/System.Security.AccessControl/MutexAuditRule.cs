using System.Security.Principal;

namespace System.Security.AccessControl;

public sealed class MutexAuditRule : AuditRule
{
	private MutexRights rights;

	public MutexRights MutexRights => rights;

	public MutexAuditRule(IdentityReference identity, MutexRights eventRights, AuditFlags flags)
		: base(identity, 0, isInherited: false, InheritanceFlags.None, PropagationFlags.None, flags)
	{
		rights = eventRights;
	}
}
