using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.IO.Pipes;

[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
public sealed class PipeAuditRule : AuditRule
{
	[System.MonoNotSupported("ACL is not supported in Mono")]
	public PipeAccessRights PipeAccessRights { get; private set; }

	[System.MonoNotSupported("ACL is not supported in Mono")]
	public PipeAuditRule(IdentityReference identity, PipeAccessRights rights, AuditFlags flags)
		: base(identity, 0, isInherited: false, InheritanceFlags.None, PropagationFlags.None, flags)
	{
		throw new NotImplementedException("ACL is not supported in Mono");
	}

	[System.MonoNotSupported("ACL is not supported in Mono")]
	public PipeAuditRule(string identity, PipeAccessRights rights, AuditFlags flags)
		: this((IdentityReference)null, rights, flags)
	{
		throw new NotImplementedException("ACL is not supported in Mono");
	}
}
