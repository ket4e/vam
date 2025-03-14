namespace System.Security.Cryptography;

[Serializable]
public sealed class CngAlgorithmGroup : IEquatable<CngAlgorithmGroup>
{
	private string group;

	private static CngAlgorithmGroup dh;

	private static CngAlgorithmGroup dsa;

	private static CngAlgorithmGroup ecdh;

	private static CngAlgorithmGroup ecdsa;

	private static CngAlgorithmGroup rsa;

	public string AlgorithmGroup => group;

	public static CngAlgorithmGroup DiffieHellman
	{
		get
		{
			if (dh == null)
			{
				dh = new CngAlgorithmGroup("DH");
			}
			return dh;
		}
	}

	public static CngAlgorithmGroup Dsa
	{
		get
		{
			if (dsa == null)
			{
				dsa = new CngAlgorithmGroup("DSA");
			}
			return dsa;
		}
	}

	public static CngAlgorithmGroup ECDiffieHellman
	{
		get
		{
			if (ecdh == null)
			{
				ecdh = new CngAlgorithmGroup("ECDH");
			}
			return ecdh;
		}
	}

	public static CngAlgorithmGroup ECDsa
	{
		get
		{
			if (ecdsa == null)
			{
				ecdsa = new CngAlgorithmGroup("ECDSA");
			}
			return ecdsa;
		}
	}

	public static CngAlgorithmGroup Rsa
	{
		get
		{
			if (rsa == null)
			{
				rsa = new CngAlgorithmGroup("RSA");
			}
			return rsa;
		}
	}

	public CngAlgorithmGroup(string algorithmGroup)
	{
		if (algorithmGroup == null)
		{
			throw new ArgumentNullException("algorithmGroup");
		}
		if (algorithmGroup.Length == 0)
		{
			throw new ArgumentException("algorithmGroup");
		}
		group = algorithmGroup;
	}

	public bool Equals(CngAlgorithmGroup other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as CngAlgorithmGroup);
	}

	public override int GetHashCode()
	{
		return group.GetHashCode();
	}

	public override string ToString()
	{
		return group;
	}

	public static bool operator ==(CngAlgorithmGroup left, CngAlgorithmGroup right)
	{
		if ((object)left == null)
		{
			return (object)right == null;
		}
		if ((object)right == null)
		{
			return false;
		}
		return left.group == right.group;
	}

	public static bool operator !=(CngAlgorithmGroup left, CngAlgorithmGroup right)
	{
		if ((object)left == null)
		{
			return (object)right != null;
		}
		if ((object)right == null)
		{
			return true;
		}
		return left.group != right.group;
	}
}
