using System.Security.Principal;

namespace System.Security.AccessControl;

public sealed class CryptoKeyAccessRule : AccessRule
{
	private CryptoKeyRights rights;

	public CryptoKeyRights CryptoKeyRights => rights;

	public CryptoKeyAccessRule(IdentityReference identity, CryptoKeyRights cryptoKeyRights, AccessControlType type)
		: base(identity, 0, isInherited: false, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow)
	{
		rights = cryptoKeyRights;
	}

	public CryptoKeyAccessRule(string identity, CryptoKeyRights cryptoKeyRights, AccessControlType type)
		: this(new SecurityIdentifier(identity), cryptoKeyRights, type)
	{
	}
}
