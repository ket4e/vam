using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Un4seen.Bass.AddOn.Enc;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 2)]
public sealed class ACMFORMAT
{
	public WAVEFORMATEX waveformatex;

	public byte[] extension;

	public int FormatLength
	{
		get
		{
			int num = 18;
			if (extension != null)
			{
				num += extension.Length;
			}
			return num;
		}
	}

	public ACMFORMAT()
	{
		int num = BassEnc.BASS_Encode_GetACMFormat(0, IntPtr.Zero, 0, null, BASSACMFormat.BASS_ACM_NONE);
		waveformatex = new WAVEFORMATEX();
		waveformatex.cbSize = (short)(num - 18);
		if (waveformatex.cbSize >= 0)
		{
			extension = new byte[waveformatex.cbSize];
		}
	}

	public ACMFORMAT(int length)
	{
		waveformatex = new WAVEFORMATEX();
		waveformatex.cbSize = (short)(length - 18);
		if (waveformatex.cbSize >= 0)
		{
			extension = new byte[waveformatex.cbSize];
		}
	}

	public unsafe ACMFORMAT(IntPtr codec)
	{
		waveformatex = (WAVEFORMATEX)Marshal.PtrToStructure(codec, typeof(WAVEFORMATEX));
		extension = new byte[waveformatex.cbSize];
		codec = new IntPtr((byte*)codec.ToPointer() + 18);
		Marshal.Copy(codec, extension, 0, waveformatex.cbSize);
	}

	public override string ToString()
	{
		return waveformatex.ToString();
	}

	public static bool SaveToFile(ACMFORMAT form, string fileName)
	{
		if (form == null)
		{
			return false;
		}
		bool result = false;
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		Stream stream = null;
		try
		{
			stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			binaryFormatter.Serialize(stream, form);
			result = true;
		}
		catch
		{
		}
		finally
		{
			if (stream != null)
			{
				stream.Flush();
				stream.Close();
			}
		}
		return result;
	}

	public static ACMFORMAT LoadFromFile(string fileName)
	{
		ACMFORMAT aCMFORMAT = null;
		IFormatter formatter = new BinaryFormatter();
		Stream stream = null;
		try
		{
			stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			bool flag = false;
			while (!flag)
			{
				try
				{
					aCMFORMAT = formatter.Deserialize(stream) as ACMFORMAT;
					if (aCMFORMAT != null)
					{
						flag = true;
					}
				}
				catch
				{
					flag = true;
				}
			}
		}
		catch
		{
		}
		finally
		{
			if (stream != null)
			{
				stream.Close();
				stream.Dispose();
				stream = null;
			}
		}
		return aCMFORMAT;
	}
}
