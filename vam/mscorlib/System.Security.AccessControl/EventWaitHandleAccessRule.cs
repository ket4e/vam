using System.Security.Principal;

namespace System.Security.AccessControl;

public sealed class EventWaitHandleAccessRule : AccessRule
{
	private EventWaitHandleRights rights;

	public EventWaitHandleRights EventWaitHandleRights => rights;

	public EventWaitHandleAccessRule(IdentityReference identity, EventWaitHandleRights eventRights, AccessControlType type)
		: base(identity, 0, isInherited: false, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow)
	{
		rights = eventRights;
	}

	public EventWaitHandleAccessRule(string identity, EventWaitHandleRights eventRights, AccessControlType type)
		: this(new SecurityIdentifier(identity), eventRights, type)
	{
	}
}
