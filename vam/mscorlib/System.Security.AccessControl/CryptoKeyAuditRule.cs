using System.Security.Principal;

namespace System.Security.AccessControl;

public sealed class CryptoKeyAuditRule : AuditRule
{
	private CryptoKeyRights rights;

	public CryptoKeyRights CryptoKeyRights => rights;

	public CryptoKeyAuditRule(IdentityReference identity, CryptoKeyRights cryptoKeyRights, AuditFlags flags)
		: base(identity, 0, isInherited: false, InheritanceFlags.None, PropagationFlags.None, flags)
	{
		rights = cryptoKeyRights;
	}

	public CryptoKeyAuditRule(string identity, CryptoKeyRights cryptoKeyRights, AuditFlags flags)
		: this(new SecurityIdentifier(identity), cryptoKeyRights, flags)
	{
	}
}
