using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Un4seen.Bass.AddOn.Tags;

[SuppressUnmanagedCodeSecurity]
internal class ID3v2Reader
{
	private Stream stream;

	private byte majorVersion;

	private byte minorVersion;

	private byte tagFlags;

	private int lastTagPos;

	private string frameId;

	private short frameFlags;

	private object frameValue;

	private int offset;

	private byte[] buffer;

	private byte DefaultMajorVersion = 3;

	private byte DefaultMinorVersion;

	public byte MajorVersion => majorVersion;

	public byte MinorVersion => minorVersion;

	public ID3v2Reader(IntPtr pID3v2)
	{
		if (Utils.IntPtrAsStringAnsi(pID3v2, 3) == "ID3")
		{
			offset += 3;
			majorVersion = Marshal.ReadByte(pID3v2, offset);
			offset++;
			minorVersion = Marshal.ReadByte(pID3v2, offset);
			offset++;
			tagFlags = Marshal.ReadByte(pID3v2, offset);
			offset++;
			int num = ReadSynchsafeInt32(pID3v2, offset);
			offset += 4;
			bool num2 = (tagFlags & 0x40) > 0;
			int num3 = 0;
			if (num2)
			{
				num3 = ReadSynchsafeInt32(pID3v2, offset);
			}
			buffer = new byte[num + 10];
			Marshal.Copy(pID3v2, buffer, 0, num + 10);
			stream = new MemoryStream(buffer);
			stream.Position = 10 + num3;
			int num4 = num + 10;
			lastTagPos = num4 - 10;
		}
		else
		{
			majorVersion = DefaultMajorVersion;
			minorVersion = DefaultMinorVersion;
			stream = null;
		}
		frameId = null;
		frameValue = null;
	}

	public void Close()
	{
		if (stream != null)
		{
			stream.Close();
			stream.Dispose();
			stream = null;
		}
	}

	public bool Read()
	{
		frameId = null;
		frameValue = null;
		if (stream == null)
		{
			return false;
		}
		if (stream.Position > lastTagPos)
		{
			return false;
		}
		frameId = ReadFrameId();
		int num = ReadFrameLength();
		if (num > 16777216)
		{
			return false;
		}
		frameFlags = 0;
		if (majorVersion > 2)
		{
			frameFlags = ReadFrameFlags();
		}
		if (num == 0)
		{
			frameValue = string.Empty;
		}
		else
		{
			frameValue = ReadFrameValue(num, frameFlags);
		}
		return true;
	}

	public string GetKey()
	{
		return frameId;
	}

	public object GetValue()
	{
		return frameValue;
	}

	public short GetFlags()
	{
		return frameFlags;
	}

	private string ReadMagic(IntPtr p)
	{
		byte[] bytes = new byte[3];
		stream.Read(bytes, 0, 3);
		return Encoding.ASCII.GetString(bytes, 0, 3);
	}

	private string ReadFrameId()
	{
		int num = 4;
		if (majorVersion == 2)
		{
			num = 3;
		}
		byte[] bytes = new byte[num];
		stream.Read(bytes, 0, num);
		return Encoding.ASCII.GetString(bytes, 0, num).TrimEnd(default(char));
	}

	private int ReadFrameLength()
	{
		if (majorVersion == 4)
		{
			return ReadSynchsafeInt32();
		}
		if (majorVersion == 3)
		{
			return ReadInt32();
		}
		if (majorVersion == 2)
		{
			return ReadInt24();
		}
		throw new NotSupportedException("Unsupported ID3v2 version detected. Don't know how to deal with this version.");
	}

	private short ReadFrameFlags()
	{
		int num = stream.ReadByte();
		int num2 = stream.ReadByte();
		return (short)((num << 8) | num2);
	}

	private byte[] UnsyncBuffer(byte[] buffer)
	{
		List<byte> list = new List<byte>(buffer);
		for (int num = list.Count - 2; num >= 0; num--)
		{
			if (list[num] == byte.MaxValue && list[num + 1] == 0)
			{
				list.RemoveAt(num + 1);
			}
		}
		return list.ToArray();
	}

	private object ReadFrameValue(int frameLength, short frameFlags)
	{
		byte[] array = new byte[frameLength];
		stream.Read(array, 0, frameLength);
		int num = 0;
		if (((uint)frameFlags & (true ? 1u : 0u)) != 0)
		{
			GetSynchsafeInt32(array[0], array[1], array[2], array[3]);
			num = 4;
		}
		if ((tagFlags & 0x80u) != 0 || ((uint)frameFlags & 2u) != 0)
		{
			array = UnsyncBuffer(array);
			frameLength = array.Length;
		}
		if (frameId == "COM" || frameId == "COMM" || frameId == "USER" || frameId == "ULT" || frameId == "USLT")
		{
			Encoding encoding = GetFrameEncoding(array[num]);
			num += 4;
			if (array[num - 4] == 1 && frameLength > 6)
			{
				if (array[num] == 254 && array[num + 1] == byte.MaxValue)
				{
					encoding = Encoding.BigEndianUnicode;
					num += 2;
				}
				else
				{
					if (array[num] != byte.MaxValue || array[num + 1] != 254)
					{
						return string.Empty;
					}
					encoding = Encoding.Unicode;
					num += 2;
				}
			}
			string text = encoding.GetString(array, num, frameLength - num).TrimEnd(default(char));
			string[] array2 = text.Split(default(char));
			if (array2 != null && array2.Length > 1)
			{
				text = ((array2[0].Trim().Length <= 0 || array2[1].Trim().Length <= 0) ? array2[1].Trim() : ("(" + array2[0].Trim() + "):" + array2[1].Trim()));
			}
			return text.Trim();
		}
		if (frameId == "WXXX" || frameId == "WXX" || frameId == "TXXX" || frameId == "TXX")
		{
			Encoding encoding2 = GetFrameEncoding(array[num]);
			num++;
			if (array[num - 1] == 1 && frameLength > 6)
			{
				if (array[num] == 254 && array[num + 1] == byte.MaxValue)
				{
					encoding2 = Encoding.BigEndianUnicode;
					num += 2;
				}
				else
				{
					if (array[num] != byte.MaxValue || array[num + 1] != 254)
					{
						return string.Empty;
					}
					encoding2 = Encoding.Unicode;
					num += 2;
				}
			}
			string text2 = encoding2.GetString(array, num, frameLength - num).TrimEnd(default(char));
			string[] array3 = text2.Split(default(char));
			if (array3 != null && array3.Length > 1)
			{
				text2 = ((array3[0].Trim().Length <= 0) ? array3[1].Trim() : (array3[0].Trim() + ":" + array3[1].Trim()));
			}
			return text2.Trim();
		}
		if (frameId[0] == 'T')
		{
			Encoding encoding3 = GetFrameEncoding(array[num]);
			num++;
			if (array[num - 1] == 1 && frameLength > 3)
			{
				if (array[num] == 254 && array[num + 1] == byte.MaxValue)
				{
					encoding3 = Encoding.BigEndianUnicode;
					num += 2;
				}
				else
				{
					if (array[num] != byte.MaxValue || array[num + 1] != 254)
					{
						return string.Empty;
					}
					encoding3 = Encoding.Unicode;
					num += 2;
				}
			}
			return encoding3.GetString(array, num, frameLength - num).TrimEnd(default(char)).Trim();
		}
		if (frameId[0] == 'W')
		{
			string text3 = Encoding.Default.GetString(array, num, frameLength).TrimEnd(default(char)).TrimEnd(default(char));
			string[] array4 = text3.Split(default(char));
			if (array4 != null && array4.Length > 1)
			{
				text3 = array4[0].Trim();
			}
			return text3.Trim();
		}
		if (frameId == "UFI" || frameId == "LNK" || frameId == "UFID" || frameId == "LINK")
		{
			string text4 = Encoding.Default.GetString(array, num, frameLength).TrimEnd(default(char));
			string[] array5 = text4.Split(default(char));
			if (array5 != null && array5.Length > 1)
			{
				text4 = ((array5[0].Trim().Length <= 0) ? array5[1].Trim() : (array5[0].Trim() + ":" + array5[1].Trim()));
			}
			return text4.Trim();
		}
		if (frameId == "POP" || frameId == "POPM")
		{
			byte b = 0;
			for (int i = 0; i < frameLength; i++)
			{
				if (array[i + num] == 0 && i < frameLength - 1)
				{
					b = array[i + num + 1];
					break;
				}
			}
			return b;
		}
		return array;
	}

	private int ReadSynchsafeInt32(IntPtr p, int offset)
	{
		byte[] array = new byte[4]
		{
			Marshal.ReadByte(p, offset),
			Marshal.ReadByte(p, offset + 1),
			Marshal.ReadByte(p, offset + 2),
			Marshal.ReadByte(p, offset + 3)
		};
		if ((array[0] & 0x80u) != 0 || (array[1] & 0x80u) != 0 || (array[2] & 0x80u) != 0 || (array[3] & 0x80u) != 0)
		{
			throw new FormatException("Found invalid syncsafe integer");
		}
		return (array[0] << 21) | (array[1] << 14) | (array[2] << 7) | array[3];
	}

	private int ReadSynchsafeInt32()
	{
		byte[] array = new byte[4];
		stream.Read(array, 0, 4);
		return GetSynchsafeInt32(array[0], array[1], array[2], array[3]);
	}

	private int GetSynchsafeInt32(byte first, byte second, byte third, byte forth)
	{
		if ((first & 0x80u) != 0 || (second & 0x80u) != 0 || (third & 0x80u) != 0 || (forth & 0x80u) != 0)
		{
			throw new FormatException("Found invalid syncsafe integer");
		}
		return (first << 21) | (second << 14) | (third << 7) | forth;
	}

	private int ReadInt32(IntPtr p, int offset)
	{
		byte[] array = new byte[4]
		{
			Marshal.ReadByte(p, offset),
			Marshal.ReadByte(p, offset + 1),
			Marshal.ReadByte(p, offset + 2),
			Marshal.ReadByte(p, offset + 3)
		};
		return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
	}

	private int ReadInt32()
	{
		byte[] array = new byte[4];
		stream.Read(array, 0, 4);
		return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
	}

	private int ReadInt24(IntPtr p, int offset)
	{
		byte[] array = new byte[3]
		{
			Marshal.ReadByte(p, offset),
			Marshal.ReadByte(p, offset + 1),
			Marshal.ReadByte(p, offset + 2)
		};
		return (array[0] << 16) | (array[1] << 8) | array[2];
	}

	private int ReadInt24()
	{
		byte[] array = new byte[3];
		stream.Read(array, 0, 3);
		return (array[0] << 16) | (array[1] << 8) | array[2];
	}

	private Encoding GetFrameEncoding(byte frameEncoding)
	{
		switch (frameEncoding)
		{
		case 0:
			if (!BassNet.UseBrokenLatin1Behavior)
			{
				return Encoding.GetEncoding("latin1");
			}
			return Encoding.Default;
		case 1:
			return Encoding.Unicode;
		case 2:
			return Encoding.BigEndianUnicode;
		case 3:
			return Encoding.UTF8;
		default:
			if (!BassNet.UseBrokenLatin1Behavior)
			{
				return Encoding.GetEncoding("latin1");
			}
			return Encoding.Default;
		}
	}

	private int GetDataLength()
	{
		return 0;
	}

	public TagPicture GetPicture(byte[] frameValue, short frameFlags, int index, bool v2)
	{
		if (frameValue == null)
		{
			return null;
		}
		TagPicture result = null;
		byte[] array = null;
		try
		{
			int num = 0;
			_ = frameValue.Length;
			if (((uint)frameFlags & (true ? 1u : 0u)) != 0)
			{
				GetSynchsafeInt32(frameValue[0], frameValue[1], frameValue[2], frameValue[3]);
				num = 4;
			}
			Encoding frameEncoding = GetFrameEncoding(frameValue[num]);
			num++;
			string mimeType;
			if (v2)
			{
				mimeType = "Unknown";
				if (frameValue[num] == 74 && frameValue[num + 1] == 80 && frameValue[num + 2] == 71)
				{
					mimeType = "image/jpeg";
				}
				else if (frameValue[num] == 71 && frameValue[num + 1] == 73 && frameValue[num + 2] == 70)
				{
					mimeType = "image/gif";
				}
				else if (frameValue[num] == 66 && frameValue[num + 1] == 80 && frameValue[num + 2] == 77)
				{
					mimeType = "image/bmp";
				}
				else if (frameValue[num] == 80 && frameValue[num + 1] == 78 && frameValue[num + 2] == 71)
				{
					mimeType = "image/png";
				}
				num += 3;
			}
			else
			{
				mimeType = ReadTextZero(frameValue, ref num);
				num++;
			}
			byte b = frameValue[num];
			TagPicture.PICTURE_TYPE pictureType;
			try
			{
				pictureType = (TagPicture.PICTURE_TYPE)b;
			}
			catch
			{
				pictureType = TagPicture.PICTURE_TYPE.Unknown;
			}
			num++;
			string description = ReadTextZero(frameValue, ref num, frameEncoding);
			num++;
			int num2 = frameValue.Length - num;
			if (num2 > 0)
			{
				array = new byte[num2];
				Array.Copy(frameValue, num, array, 0, num2);
				if (((uint)frameFlags & 2u) != 0)
				{
					List<byte> list = new List<byte>(num2);
					for (int i = 0; i < num2; i++)
					{
						list.Add(array[i]);
						if (i < num2 - 1 && array[i] == byte.MaxValue && array[i + 1] == 0)
						{
							i++;
						}
					}
					array = list.ToArray();
				}
				result = new TagPicture(index, mimeType, pictureType, description, array);
			}
		}
		catch
		{
		}
		return result;
	}

	private string ReadTextZero(byte[] frameValue, ref int offset)
	{
		StringBuilder stringBuilder = new StringBuilder();
		try
		{
			char value;
			while ((value = (char)frameValue[offset]) != 0)
			{
				stringBuilder.Append(value);
				offset++;
			}
		}
		catch
		{
		}
		return stringBuilder.ToString();
	}

	private string ReadTextZero(byte[] frameValue, ref int offset, Encoding encoding)
	{
		string result = string.Empty;
		try
		{
			if (frameValue[0] == 1)
			{
				if (frameValue[offset] == 254 && frameValue[offset + 1] == byte.MaxValue)
				{
					encoding = Encoding.BigEndianUnicode;
				}
				else
				{
					if (frameValue[offset] != byte.MaxValue || frameValue[offset + 1] != 254)
					{
						return result;
					}
					encoding = Encoding.Unicode;
				}
			}
			int num = 1;
			if (frameValue[0] == 1 || frameValue[0] == 2)
			{
				num = 2;
			}
			int num2 = offset;
			while (true)
			{
				switch (num)
				{
				default:
					continue;
				case 1:
					if (frameValue[num2] != 0)
					{
						num2++;
						continue;
					}
					break;
				case 2:
					if (frameValue[num2] == 0 && frameValue[num2 + 1] == 0)
					{
						num2++;
						break;
					}
					num2++;
					continue;
				}
				break;
			}
			result = encoding.GetString(frameValue, offset, num2 - offset);
			offset = num2;
			if (num == 2)
			{
				offset++;
			}
		}
		catch
		{
		}
		return result;
	}
}
