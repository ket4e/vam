namespace System.Security.Cryptography;

public sealed class SHA1Cng : SHA1
{
	private static byte[] Empty = new byte[0];

	private SHA1 hash;

	[SecurityCritical]
	public SHA1Cng()
	{
		hash = new SHA1Managed();
	}

	[SecurityCritical]
	public override void Initialize()
	{
		hash.Initialize();
	}

	[SecurityCritical]
	protected override void HashCore(byte[] array, int ibStart, int cbSize)
	{
		hash.TransformBlock(array, ibStart, cbSize, null, 0);
	}

	[SecurityCritical]
	protected override byte[] HashFinal()
	{
		hash.TransformFinalBlock(Empty, 0, 0);
		HashValue = hash.Hash;
		return HashValue;
	}

	[SecurityCritical]
	protected override void Dispose(bool disposing)
	{
		((IDisposable)hash).Dispose();
		base.Dispose(disposing);
	}
}
