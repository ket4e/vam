using System.IO;
using System.Security.Cryptography;

namespace System.Net.FtpClient;

public class FtpHash
{
	private FtpHashAlgorithm m_algorithm;

	private string m_value;

	public FtpHashAlgorithm Algorithm
	{
		get
		{
			return m_algorithm;
		}
		internal set
		{
			m_algorithm = value;
		}
	}

	public string Value
	{
		get
		{
			return m_value;
		}
		internal set
		{
			m_value = value;
		}
	}

	public bool IsValid
	{
		get
		{
			if (m_algorithm != 0)
			{
				return !string.IsNullOrEmpty(m_value);
			}
			return false;
		}
	}

	public bool Verify(string file)
	{
		using FileStream istream = new FileStream(file, FileMode.Open, FileAccess.Read);
		return Verify(istream);
	}

	public bool Verify(Stream istream)
	{
		if (IsValid)
		{
			HashAlgorithm hashAlgorithm = null;
			hashAlgorithm = m_algorithm switch
			{
				FtpHashAlgorithm.SHA1 => new SHA1CryptoServiceProvider(), 
				FtpHashAlgorithm.SHA256 => new SHA256CryptoServiceProvider(), 
				FtpHashAlgorithm.SHA512 => new SHA512CryptoServiceProvider(), 
				FtpHashAlgorithm.MD5 => new MD5CryptoServiceProvider(), 
				FtpHashAlgorithm.CRC => throw new NotImplementedException("There is no built in support for computing CRC hashes."), 
				_ => throw new NotImplementedException("Unknown hash algorithm: " + m_algorithm), 
			};
			byte[] array = null;
			string text = "";
			array = hashAlgorithm.ComputeHash(istream);
			if (array != null)
			{
				byte[] array2 = array;
				foreach (byte b in array2)
				{
					text += b.ToString("x2");
				}
				return text.ToUpper() == m_value.ToUpper();
			}
		}
		return false;
	}

	internal FtpHash()
	{
	}
}
