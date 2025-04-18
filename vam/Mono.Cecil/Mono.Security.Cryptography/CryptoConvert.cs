using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography;

internal static class CryptoConvert
{
	private static int ToInt32LE(byte[] bytes, int offset)
	{
		return (bytes[offset + 3] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 1] << 8) | bytes[offset];
	}

	private static uint ToUInt32LE(byte[] bytes, int offset)
	{
		return (uint)((bytes[offset + 3] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 1] << 8) | bytes[offset]);
	}

	private static byte[] Trim(byte[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0)
			{
				byte[] array2 = new byte[array.Length - i];
				Buffer.BlockCopy(array, i, array2, 0, array2.Length);
				return array2;
			}
		}
		return null;
	}

	private static RSA FromCapiPrivateKeyBlob(byte[] blob, int offset)
	{
		RSAParameters parameters = default(RSAParameters);
		try
		{
			if (blob[offset] != 7 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || ToUInt32LE(blob, offset + 8) != 843141970)
			{
				throw new CryptographicException("Invalid blob header");
			}
			int num = ToInt32LE(blob, offset + 12);
			byte[] array = new byte[4];
			Buffer.BlockCopy(blob, offset + 16, array, 0, 4);
			Array.Reverse(array);
			parameters.Exponent = Trim(array);
			int num2 = offset + 20;
			int num3 = num >> 3;
			parameters.Modulus = new byte[num3];
			Buffer.BlockCopy(blob, num2, parameters.Modulus, 0, num3);
			Array.Reverse(parameters.Modulus);
			num2 += num3;
			int num4 = num3 >> 1;
			parameters.P = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.P, 0, num4);
			Array.Reverse(parameters.P);
			num2 += num4;
			parameters.Q = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.Q, 0, num4);
			Array.Reverse(parameters.Q);
			num2 += num4;
			parameters.DP = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.DP, 0, num4);
			Array.Reverse(parameters.DP);
			num2 += num4;
			parameters.DQ = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.DQ, 0, num4);
			Array.Reverse(parameters.DQ);
			num2 += num4;
			parameters.InverseQ = new byte[num4];
			Buffer.BlockCopy(blob, num2, parameters.InverseQ, 0, num4);
			Array.Reverse(parameters.InverseQ);
			num2 += num4;
			parameters.D = new byte[num3];
			if (num2 + num3 + offset <= blob.Length)
			{
				Buffer.BlockCopy(blob, num2, parameters.D, 0, num3);
				Array.Reverse(parameters.D);
			}
		}
		catch (Exception inner)
		{
			throw new CryptographicException("Invalid blob.", inner);
		}
		RSA rSA = null;
		try
		{
			rSA = RSA.Create();
			rSA.ImportParameters(parameters);
		}
		catch (CryptographicException)
		{
			bool flag = false;
			try
			{
				rSA = new RSACryptoServiceProvider(new CspParameters
				{
					Flags = CspProviderFlags.UseMachineKeyStore
				});
				rSA.ImportParameters(parameters);
			}
			catch
			{
				flag = true;
			}
			if (flag)
			{
				throw;
			}
		}
		return rSA;
	}

	private static RSA FromCapiPublicKeyBlob(byte[] blob, int offset)
	{
		try
		{
			if (blob[offset] != 6 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || ToUInt32LE(blob, offset + 8) != 826364754)
			{
				throw new CryptographicException("Invalid blob header");
			}
			int num = ToInt32LE(blob, offset + 12);
			RSAParameters parameters = new RSAParameters
			{
				Exponent = new byte[3]
			};
			parameters.Exponent[0] = blob[offset + 18];
			parameters.Exponent[1] = blob[offset + 17];
			parameters.Exponent[2] = blob[offset + 16];
			int srcOffset = offset + 20;
			int num2 = num >> 3;
			parameters.Modulus = new byte[num2];
			Buffer.BlockCopy(blob, srcOffset, parameters.Modulus, 0, num2);
			Array.Reverse(parameters.Modulus);
			RSA rSA = null;
			try
			{
				rSA = RSA.Create();
				rSA.ImportParameters(parameters);
			}
			catch (CryptographicException)
			{
				rSA = new RSACryptoServiceProvider(new CspParameters
				{
					Flags = CspProviderFlags.UseMachineKeyStore
				});
				rSA.ImportParameters(parameters);
			}
			return rSA;
		}
		catch (Exception inner)
		{
			throw new CryptographicException("Invalid blob.", inner);
		}
	}

	public static RSA FromCapiKeyBlob(byte[] blob)
	{
		return FromCapiKeyBlob(blob, 0);
	}

	public static RSA FromCapiKeyBlob(byte[] blob, int offset)
	{
		if (blob == null)
		{
			throw new ArgumentNullException("blob");
		}
		if (offset >= blob.Length)
		{
			throw new ArgumentException("blob is too small.");
		}
		switch (blob[offset])
		{
		case 0:
			if (blob[offset + 12] == 6)
			{
				return FromCapiPublicKeyBlob(blob, offset + 12);
			}
			break;
		case 6:
			return FromCapiPublicKeyBlob(blob, offset);
		case 7:
			return FromCapiPrivateKeyBlob(blob, offset);
		}
		throw new CryptographicException("Unknown blob format.");
	}
}
