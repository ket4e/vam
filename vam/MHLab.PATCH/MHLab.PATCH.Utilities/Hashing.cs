using System;
using System.IO;
using System.Security.Cryptography;

namespace MHLab.PATCH.Utilities;

internal class Hashing
{
	public static string SHA1(string filePath)
	{
		using SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
		return GetHash(filePath, hasher);
	}

	public static string SHA1(Stream s)
	{
		using SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
		return GetHash(s, hasher);
	}

	public static string MD5(string filePath)
	{
		using MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
		return GetHash(filePath, hasher);
	}

	public static string MD5(Stream s)
	{
		using MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();
		return GetHash(s, hasher);
	}

	public static string CRC32(string filename)
	{
		CRC32 cRC = new CRC32();
		string text = string.Empty;
		using FileStream inputStream = File.Open(filename, FileMode.Open);
		byte[] array = cRC.ComputeHash(inputStream);
		foreach (byte b in array)
		{
			text += b.ToString("x2").ToLower();
		}
		return text;
	}

	public static string CRC32(FileStream file)
	{
		CRC32 cRC = new CRC32();
		string text = string.Empty;
		using FileStream inputStream = file;
		byte[] array = cRC.ComputeHash(inputStream);
		foreach (byte b in array)
		{
			text += b.ToString("x2").ToLower();
		}
		return text;
	}

	private static string GetHash(string filePath, HashAlgorithm hasher)
	{
		using FileStream s = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		return GetHash(s, hasher);
	}

	private static string GetHash(Stream s, HashAlgorithm hasher)
	{
		return Convert.ToBase64String(hasher.ComputeHash(s)).TrimEnd('=');
	}
}
