using System.IO;

namespace System.Security.Cryptography.Xml;

internal class SymmetricKeyWrap
{
	public static byte[] AESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
	{
		SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
		symmetricAlgorithm.Mode = CipherMode.ECB;
		symmetricAlgorithm.IV = new byte[16];
		ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor(rgbKey, symmetricAlgorithm.IV);
		int num = rgbWrappedKeyData.Length / 8;
		byte[] array = new byte[16];
		byte[] array2 = new byte[8 * (num + 1)];
		if (num == 1)
		{
			byte[] buf = new byte[8] { 166, 166, 166, 166, 166, 166, 166, 166 };
			cryptoTransform.TransformBlock(Concatenate(buf, rgbWrappedKeyData), 0, 16, array, 0);
			Buffer.BlockCopy(MSB(array), 0, array2, 0, 8);
			Buffer.BlockCopy(LSB(array), 0, array2, 8, 8);
		}
		else
		{
			byte[] buf = new byte[8] { 166, 166, 166, 166, 166, 166, 166, 166 };
			byte[][] array3 = new byte[num + 1][];
			for (int i = 1; i <= num; i++)
			{
				array3[i] = new byte[8];
				Buffer.BlockCopy(rgbWrappedKeyData, 8 * (i - 1), array3[i], 0, 8);
			}
			for (int j = 0; j <= 5; j++)
			{
				for (int k = 1; k <= num; k++)
				{
					cryptoTransform.TransformBlock(Concatenate(buf, array3[k]), 0, 16, array, 0);
					byte[] bytes = BitConverter.GetBytes((long)(num * j + k));
					if (BitConverter.IsLittleEndian)
					{
						Array.Reverse(bytes);
					}
					buf = Xor(bytes, MSB(array));
					array3[k] = LSB(array);
				}
			}
			Buffer.BlockCopy(buf, 0, array2, 0, 8);
			for (int l = 1; l <= num; l++)
			{
				Buffer.BlockCopy(array3[l], 0, array2, 8 * l, 8);
			}
		}
		return array2;
	}

	public static byte[] AESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
	{
		SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("Rijndael");
		symmetricAlgorithm.Mode = CipherMode.ECB;
		symmetricAlgorithm.Key = rgbKey;
		int num = rgbEncryptedWrappedKeyData.Length / 8 - 1;
		byte[] array = new byte[8];
		Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 0, array, 0, 8);
		byte[] array2 = new byte[num * 8];
		Buffer.BlockCopy(rgbEncryptedWrappedKeyData, 8, array2, 0, rgbEncryptedWrappedKeyData.Length - 8);
		ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
		for (int num2 = 5; num2 >= 0; num2--)
		{
			for (int num3 = num; num3 >= 1; num3--)
			{
				byte[] bytes = BitConverter.GetBytes((long)num * (long)num2 + num3);
				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes);
				}
				byte[] array3 = new byte[16];
				byte[] array4 = new byte[8];
				Buffer.BlockCopy(array2, 8 * (num3 - 1), array4, 0, 8);
				byte[] inputBuffer = Concatenate(Xor(array, bytes), array4);
				cryptoTransform.TransformBlock(inputBuffer, 0, 16, array3, 0);
				array = MSB(array3);
				Buffer.BlockCopy(LSB(array3), 0, array2, 8 * (num3 - 1), 8);
			}
		}
		return array2;
	}

	public static byte[] TripleDESKeyWrapEncrypt(byte[] rgbKey, byte[] rgbWrappedKeyData)
	{
		SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("TripleDES");
		byte[] buf = ComputeCMSKeyChecksum(rgbWrappedKeyData);
		byte[] data = Concatenate(rgbWrappedKeyData, buf);
		symmetricAlgorithm.GenerateIV();
		symmetricAlgorithm.Mode = CipherMode.CBC;
		symmetricAlgorithm.Padding = PaddingMode.None;
		symmetricAlgorithm.Key = rgbKey;
		byte[] buf2 = Transform(data, symmetricAlgorithm.CreateEncryptor());
		byte[] array = Concatenate(symmetricAlgorithm.IV, buf2);
		Array.Reverse(array);
		symmetricAlgorithm.IV = new byte[8] { 74, 221, 162, 44, 121, 232, 33, 5 };
		return Transform(array, symmetricAlgorithm.CreateEncryptor());
	}

	public static byte[] TripleDESKeyWrapDecrypt(byte[] rgbKey, byte[] rgbEncryptedWrappedKeyData)
	{
		SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create("TripleDES");
		symmetricAlgorithm.Mode = CipherMode.CBC;
		symmetricAlgorithm.Padding = PaddingMode.None;
		symmetricAlgorithm.Key = rgbKey;
		symmetricAlgorithm.IV = new byte[8] { 74, 221, 162, 44, 121, 232, 33, 5 };
		byte[] array = Transform(rgbEncryptedWrappedKeyData, symmetricAlgorithm.CreateDecryptor());
		Array.Reverse(array);
		byte[] array2 = new byte[array.Length - 8];
		byte[] array3 = new byte[8];
		Buffer.BlockCopy(array, 0, array3, 0, 8);
		Buffer.BlockCopy(array, 8, array2, 0, array2.Length);
		symmetricAlgorithm.IV = array3;
		byte[] array4 = Transform(array2, symmetricAlgorithm.CreateDecryptor());
		byte[] dst = new byte[8];
		byte[] array5 = new byte[array4.Length - 8];
		Buffer.BlockCopy(array4, 0, array5, 0, array5.Length);
		Buffer.BlockCopy(array4, array5.Length, dst, 0, 8);
		return array5;
	}

	private static byte[] Transform(byte[] data, ICryptoTransform t)
	{
		MemoryStream memoryStream = new MemoryStream();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, t, CryptoStreamMode.Write);
		cryptoStream.Write(data, 0, data.Length);
		cryptoStream.FlushFinalBlock();
		byte[] result = memoryStream.ToArray();
		memoryStream.Close();
		cryptoStream.Close();
		return result;
	}

	private static byte[] ComputeCMSKeyChecksum(byte[] data)
	{
		byte[] src = HashAlgorithm.Create("SHA1").ComputeHash(data);
		byte[] array = new byte[8];
		Buffer.BlockCopy(src, 0, array, 0, 8);
		return array;
	}

	private static byte[] Concatenate(byte[] buf1, byte[] buf2)
	{
		byte[] array = new byte[buf1.Length + buf2.Length];
		Buffer.BlockCopy(buf1, 0, array, 0, buf1.Length);
		Buffer.BlockCopy(buf2, 0, array, buf1.Length, buf2.Length);
		return array;
	}

	private static byte[] MSB(byte[] input)
	{
		return MSB(input, 8);
	}

	private static byte[] MSB(byte[] input, int bytes)
	{
		byte[] array = new byte[bytes];
		Buffer.BlockCopy(input, 0, array, 0, bytes);
		return array;
	}

	private static byte[] LSB(byte[] input)
	{
		return LSB(input, 8);
	}

	private static byte[] LSB(byte[] input, int bytes)
	{
		byte[] array = new byte[bytes];
		Buffer.BlockCopy(input, bytes, array, 0, bytes);
		return array;
	}

	private static byte[] Xor(byte[] x, byte[] y)
	{
		if (x.Length != y.Length)
		{
			throw new CryptographicException("Error performing Xor: arrays different length.");
		}
		byte[] array = new byte[x.Length];
		for (int i = 0; i < x.Length; i++)
		{
			array[i] = (byte)(x[i] ^ y[i]);
		}
		return array;
	}
}
