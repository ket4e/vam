using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.Xml;

internal class XmlSignatureStreamReader : TextReader
{
	private TextReader source;

	private int cache = int.MinValue;

	public XmlSignatureStreamReader(TextReader input)
	{
		source = input;
	}

	public override void Close()
	{
		source.Close();
	}

	public override int Peek()
	{
		if (source.Peek() == -1)
		{
			return -1;
		}
		if (cache != int.MinValue)
		{
			return cache;
		}
		cache = source.Read();
		if (cache != 13)
		{
			return cache;
		}
		if (source.Peek() != 10)
		{
			return 13;
		}
		cache = int.MinValue;
		return 10;
	}

	public override int Read()
	{
		if (cache != int.MinValue)
		{
			int result = cache;
			cache = int.MinValue;
			return result;
		}
		int num = source.Read();
		if (num != 13)
		{
			return num;
		}
		cache = source.Read();
		if (cache != 10)
		{
			return 13;
		}
		cache = int.MinValue;
		return 10;
	}

	public override int ReadBlock([In][Out] char[] buffer, int index, int count)
	{
		char[] array = new char[count];
		source.ReadBlock(array, 0, count);
		int num = index;
		int num2 = 0;
		while (num2 < count)
		{
			if (array[num2] == '\r')
			{
				if (++num2 < array.Length && array[num2] == '\n')
				{
					buffer[num] = array[num2++];
				}
				else
				{
					buffer[num] = '\r';
				}
			}
			else
			{
				buffer[num] = array[num2];
			}
			num++;
		}
		while (num < count)
		{
			int num3 = Read();
			if (num3 < 0)
			{
				break;
			}
			buffer[num++] = (char)num3;
		}
		return num;
	}

	public override string ReadLine()
	{
		return source.ReadLine();
	}

	public override string ReadToEnd()
	{
		return source.ReadToEnd().Replace("\r\n", "\n");
	}
}
