using System.Security.Principal;

namespace System.Security.AccessControl;

public abstract class AuthorizationRule
{
	private IdentityReference identity;

	private int accessMask;

	private bool isInherited;

	private InheritanceFlags inheritanceFlags;

	private PropagationFlags propagationFlags;

	public IdentityReference IdentityReference => identity;

	public InheritanceFlags InheritanceFlags => inheritanceFlags;

	public bool IsInherited => isInherited;

	public PropagationFlags PropagationFlags => propagationFlags;

	protected internal int AccessMask => accessMask;

	internal AuthorizationRule()
	{
	}

	protected internal AuthorizationRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
	{
		if (!(identity is SecurityIdentifier))
		{
			throw new ArgumentException("identity");
		}
		if (accessMask == 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		this.identity = identity;
		this.accessMask = accessMask;
		this.isInherited = isInherited;
		this.inheritanceFlags = inheritanceFlags;
		this.propagationFlags = propagationFlags;
	}
}
