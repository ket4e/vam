namespace System.Security.Cryptography;

[Serializable]
public sealed class CngAlgorithm : IEquatable<CngAlgorithm>
{
	private string algo;

	private static CngAlgorithm dh256;

	private static CngAlgorithm dh384;

	private static CngAlgorithm dh521;

	private static CngAlgorithm dsa256;

	private static CngAlgorithm dsa384;

	private static CngAlgorithm dsa521;

	private static CngAlgorithm md5;

	private static CngAlgorithm sha1;

	private static CngAlgorithm sha256;

	private static CngAlgorithm sha384;

	private static CngAlgorithm sha512;

	public string Algorithm => algo;

	public static CngAlgorithm ECDiffieHellmanP256
	{
		get
		{
			if (dh256 == null)
			{
				dh256 = new CngAlgorithm("ECDH_P256");
			}
			return dh256;
		}
	}

	public static CngAlgorithm ECDiffieHellmanP384
	{
		get
		{
			if (dh384 == null)
			{
				dh384 = new CngAlgorithm("ECDH_P384");
			}
			return dh384;
		}
	}

	public static CngAlgorithm ECDiffieHellmanP521
	{
		get
		{
			if (dh521 == null)
			{
				dh521 = new CngAlgorithm("ECDH_P521");
			}
			return dh521;
		}
	}

	public static CngAlgorithm ECDsaP256
	{
		get
		{
			if (dsa256 == null)
			{
				dsa256 = new CngAlgorithm("ECDSA_P256");
			}
			return dsa256;
		}
	}

	public static CngAlgorithm ECDsaP384
	{
		get
		{
			if (dsa384 == null)
			{
				dsa384 = new CngAlgorithm("ECDSA_P384");
			}
			return dsa384;
		}
	}

	public static CngAlgorithm ECDsaP521
	{
		get
		{
			if (dsa521 == null)
			{
				dsa521 = new CngAlgorithm("ECDSA_P521");
			}
			return dsa521;
		}
	}

	public static CngAlgorithm MD5
	{
		get
		{
			if (md5 == null)
			{
				md5 = new CngAlgorithm("MD5");
			}
			return md5;
		}
	}

	public static CngAlgorithm Sha1
	{
		get
		{
			if (sha1 == null)
			{
				sha1 = new CngAlgorithm("SHA1");
			}
			return sha1;
		}
	}

	public static CngAlgorithm Sha256
	{
		get
		{
			if (sha256 == null)
			{
				sha256 = new CngAlgorithm("SHA256");
			}
			return sha256;
		}
	}

	public static CngAlgorithm Sha384
	{
		get
		{
			if (sha384 == null)
			{
				sha384 = new CngAlgorithm("SHA384");
			}
			return sha384;
		}
	}

	public static CngAlgorithm Sha512
	{
		get
		{
			if (sha512 == null)
			{
				sha512 = new CngAlgorithm("SHA512");
			}
			return sha512;
		}
	}

	public CngAlgorithm(string algorithm)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		if (algorithm.Length == 0)
		{
			throw new ArgumentException("algorithm");
		}
		algo = algorithm;
	}

	public bool Equals(CngAlgorithm other)
	{
		if (other == null)
		{
			return false;
		}
		return algo == other.algo;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as CngAlgorithm);
	}

	public override int GetHashCode()
	{
		return algo.GetHashCode();
	}

	public override string ToString()
	{
		return algo;
	}

	public static bool operator ==(CngAlgorithm left, CngAlgorithm right)
	{
		if ((object)left == null)
		{
			return (object)right == null;
		}
		if ((object)right == null)
		{
			return false;
		}
		return left.algo == right.algo;
	}

	public static bool operator !=(CngAlgorithm left, CngAlgorithm right)
	{
		if ((object)left == null)
		{
			return (object)right != null;
		}
		if ((object)right == null)
		{
			return true;
		}
		return left.algo != right.algo;
	}
}
