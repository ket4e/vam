using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace System.Windows.Forms;

internal class FDOMimeConfigReader
{
	private bool fdo_mime_available;

	private StringCollection shared_mime_paths = new StringCollection();

	private BinaryReader br;

	private int max_offset_and_range;

	public int Init()
	{
		CheckFDOMimePaths();
		if (!fdo_mime_available)
		{
			return -1;
		}
		ReadMagicData();
		ReadGlobsData();
		ReadSubclasses();
		ReadAliases();
		shared_mime_paths = null;
		br = null;
		return max_offset_and_range;
	}

	private void CheckFDOMimePaths()
	{
		if (Directory.Exists("/usr/share/mime"))
		{
			shared_mime_paths.Add("/usr/share/mime/");
		}
		else if (Directory.Exists("/usr/local/share/mime"))
		{
			shared_mime_paths.Add("/usr/local/share/mime/");
		}
		if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/.local/share/mime"))
		{
			shared_mime_paths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/.local/share/mime/");
		}
		if (shared_mime_paths.Count != 0)
		{
			fdo_mime_available = true;
		}
	}

	private void ReadMagicData()
	{
		StringEnumerator enumerator = shared_mime_paths.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (!File.Exists(current + "/magic"))
				{
					continue;
				}
				try
				{
					FileStream fileStream = File.OpenRead(current + "/magic");
					br = new BinaryReader(fileStream);
					if (CheckMagicHeader())
					{
						MakeMatches();
					}
					br.Close();
					fileStream.Close();
				}
				catch (Exception)
				{
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void MakeMatches()
	{
		Matchlet[] array = new Matchlet[30];
		while (br.PeekChar() != -1)
		{
			int priority = -1;
			string text = ReadPriorityAndMimeType(ref priority);
			if (text == null)
			{
				continue;
			}
			Match match = new Match();
			match.Priority = priority;
			match.MimeType = text;
			do
			{
				int num = 0;
				if (br.PeekChar() != 62)
				{
					StringBuilder stringBuilder = new StringBuilder();
					while (br.PeekChar() != 62)
					{
						char value = br.ReadChar();
						stringBuilder.Append(value);
					}
					num = Convert.ToInt32(stringBuilder.ToString());
				}
				int offset = 0;
				if (br.PeekChar() == 62)
				{
					br.ReadChar();
					offset = ReadValue();
				}
				int count = 0;
				byte[] array2 = null;
				if (br.PeekChar() == 61)
				{
					br.ReadChar();
					byte b = br.ReadByte();
					byte b2 = br.ReadByte();
					count = b * 256 + b2;
					array2 = br.ReadBytes(count);
				}
				byte[] array3 = null;
				if (br.PeekChar() == 38)
				{
					br.ReadChar();
					array3 = br.ReadBytes(count);
				}
				int num2 = 1;
				if (br.PeekChar() == 126)
				{
					br.ReadChar();
					char value = br.ReadChar();
					num2 = Convert.ToInt32(value - 48);
					if (num2 > 1 && BitConverter.IsLittleEndian)
					{
						switch (num2)
						{
						case 2:
							if (array2 != null)
							{
								for (int k = 0; k < array2.Length; k += 2)
								{
									byte b11 = array2[k];
									byte b12 = array2[k + 1];
									array2[k] = b12;
									array2[k + 1] = b11;
								}
							}
							if (array3 != null)
							{
								for (int l = 0; l < array3.Length; l += 2)
								{
									byte b13 = array3[l];
									byte b14 = array3[l + 1];
									array3[l] = b14;
									array3[l + 1] = b13;
								}
							}
							break;
						case 4:
							if (array2 != null)
							{
								for (int i = 0; i < array2.Length; i += 4)
								{
									byte b3 = array2[i];
									byte b4 = array2[i + 1];
									byte b5 = array2[i + 2];
									byte b6 = array2[i + 3];
									array2[i] = b6;
									array2[i + 1] = b5;
									array2[i + 2] = b4;
									array2[i + 3] = b3;
								}
							}
							if (array3 != null)
							{
								for (int j = 0; j < array3.Length; j += 4)
								{
									byte b7 = array3[j];
									byte b8 = array3[j + 1];
									byte b9 = array3[j + 2];
									byte b10 = array3[j + 3];
									array3[j] = b10;
									array3[j + 1] = b9;
									array3[j + 2] = b8;
									array3[j + 3] = b7;
								}
							}
							break;
						}
					}
				}
				int offsetLength = 1;
				if (br.PeekChar() == 43)
				{
					br.ReadChar();
					offsetLength = ReadValue();
				}
				br.ReadChar();
				array[num] = new Matchlet();
				array[num].Offset = offset;
				array[num].OffsetLength = offsetLength;
				array[num].ByteValue = array2;
				if (array3 != null)
				{
					array[num].Mask = array3;
				}
				if (num == 0)
				{
					match.Matchlets.Add(array[num]);
				}
				else
				{
					array[num - 1].Matchlets.Add(array[num]);
				}
				if (max_offset_and_range < array[num].Offset + array[num].OffsetLength + array[num].ByteValue.Length + 1)
				{
					max_offset_and_range = array[num].Offset + array[num].OffsetLength + array[num].ByteValue.Length + 1;
				}
			}
			while (br.PeekChar() != 91);
			if (priority < 80)
			{
				Mime.MatchesBelow80.Add(match);
			}
			else
			{
				Mime.Matches80Plus.Add(match);
			}
		}
	}

	private void ReadGlobsData()
	{
		StringEnumerator enumerator = shared_mime_paths.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (!File.Exists(current + "/globs"))
				{
					continue;
				}
				try
				{
					StreamReader streamReader = new StreamReader(current + "/globs");
					while (streamReader.Peek() != -1)
					{
						string text = streamReader.ReadLine().Trim();
						if (text.StartsWith("#"))
						{
							continue;
						}
						string[] array = text.Split(':');
						if (array[1].IndexOf('*') > -1 && array[1].IndexOf('.') == -1)
						{
							Mime.GlobalSufPref.Add(array[1], array[0]);
							continue;
						}
						if (array[1].IndexOf('*') == -1)
						{
							Mime.GlobalLiterals.Add(array[1], array[0]);
							continue;
						}
						string[] array2 = array[1].Split('.');
						if (array2.Length > 2)
						{
							Mime.GlobalPatternsLong.Add(array[1].Remove(0, 1), array[0]);
						}
						else
						{
							Mime.GlobalPatternsShort.Add(array[1].Remove(0, 1), array[0]);
						}
					}
					streamReader.Close();
				}
				catch (Exception)
				{
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void ReadSubclasses()
	{
		StringEnumerator enumerator = shared_mime_paths.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (!File.Exists(current + "/subclasses"))
				{
					continue;
				}
				try
				{
					StreamReader streamReader = new StreamReader(current + "/subclasses");
					while (streamReader.Peek() != -1)
					{
						string text = streamReader.ReadLine().Trim();
						if (!text.StartsWith("#"))
						{
							string[] array = text.Split(' ');
							Mime.SubClasses.Add(array[0], array[1]);
						}
					}
					streamReader.Close();
				}
				catch (Exception)
				{
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void ReadAliases()
	{
		StringEnumerator enumerator = shared_mime_paths.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				if (!File.Exists(current + "/aliases"))
				{
					continue;
				}
				try
				{
					StreamReader streamReader = new StreamReader(current + "/aliases");
					while (streamReader.Peek() != -1)
					{
						string text = streamReader.ReadLine().Trim();
						if (!text.StartsWith("#"))
						{
							string[] array = text.Split(' ');
							Mime.Aliases.Add(array[0], array[1]);
						}
					}
					streamReader.Close();
				}
				catch (Exception)
				{
				}
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private int ReadValue()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		while (br.PeekChar() != 61 && br.PeekChar() != 10)
		{
			char value = br.ReadChar();
			stringBuilder.Append(value);
		}
		return Convert.ToInt32(stringBuilder.ToString());
	}

	private string ReadPriorityAndMimeType(ref int priority)
	{
		if (br.ReadChar() == '[')
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				char c = br.ReadChar();
				if (c == ':')
				{
					break;
				}
				stringBuilder.Append(c);
			}
			priority = Convert.ToInt32(stringBuilder.ToString());
			StringBuilder stringBuilder2 = new StringBuilder();
			while (true)
			{
				char c2 = br.ReadChar();
				if (c2 == ']')
				{
					break;
				}
				stringBuilder2.Append(c2);
			}
			if (br.ReadChar() == '\n')
			{
				return stringBuilder2.ToString();
			}
		}
		return null;
	}

	private bool CheckMagicHeader()
	{
		char[] value = br.ReadChars(10);
		string text = new string(value);
		if (text != "MIME-Magic")
		{
			return false;
		}
		if (br.ReadByte() != 0)
		{
			return false;
		}
		if (br.ReadChar() != '\n')
		{
			return false;
		}
		return true;
	}
}
