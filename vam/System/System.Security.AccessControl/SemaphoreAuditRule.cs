using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl;

[ComVisible(false)]
public sealed class SemaphoreAuditRule : AuditRule
{
	private SemaphoreRights semaphoreRights;

	public SemaphoreRights SemaphoreRights => semaphoreRights;

	public SemaphoreAuditRule(IdentityReference identity, SemaphoreRights semaphoreRights, AuditFlags flags)
		: base(identity, 0, isInherited: false, InheritanceFlags.None, PropagationFlags.None, flags)
	{
		this.semaphoreRights = semaphoreRights;
	}
}
