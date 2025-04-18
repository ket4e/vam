using System.Runtime.InteropServices;

namespace System.Security.Cryptography;

[ComVisible(true)]
public abstract class SHA384 : HashAlgorithm
{
	protected SHA384()
	{
		HashSizeValue = 384;
	}

	public new static SHA384 Create()
	{
		return Create("System.Security.Cryptography.SHA384");
	}

	public new static SHA384 Create(string hashName)
	{
		return (SHA384)CryptoConfig.CreateFromName(hashName);
	}
}
