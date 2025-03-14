using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Windows.Forms;

internal class Mime
{
	private const string octet_stream = "application/octet-stream";

	private const string text_plain = "text/plain";

	private const string zero_file = "application/x-zerosize";

	private const int mime_file_cache_max_size = 3000;

	public static Mime Instance = new Mime();

	private string current_file_name;

	private string global_result = "application/octet-stream";

	private FileStream file_stream;

	private byte[] buffer;

	private StringDictionary mime_file_cache = new StringDictionary();

	private string search_string;

	private static object lock_object = new object();

	private bool is_zero_file;

	private int bytes_read;

	private bool mime_available;

	public static NameValueCollection Aliases;

	public static NameValueCollection SubClasses;

	public static NameValueCollection GlobalPatternsShort;

	public static NameValueCollection GlobalPatternsLong;

	public static NameValueCollection GlobalLiterals;

	public static NameValueCollection GlobalSufPref;

	public static ArrayList Matches80Plus;

	public static ArrayList MatchesBelow80;

	public static bool MimeAvailable => Instance.mime_available;

	private Mime()
	{
		Aliases = new NameValueCollection(StringComparer.CurrentCultureIgnoreCase);
		SubClasses = new NameValueCollection(StringComparer.CurrentCultureIgnoreCase);
		GlobalPatternsShort = new NameValueCollection(StringComparer.CurrentCultureIgnoreCase);
		GlobalPatternsLong = new NameValueCollection(StringComparer.CurrentCultureIgnoreCase);
		GlobalLiterals = new NameValueCollection(StringComparer.CurrentCultureIgnoreCase);
		GlobalSufPref = new NameValueCollection(StringComparer.CurrentCultureIgnoreCase);
		Matches80Plus = new ArrayList();
		MatchesBelow80 = new ArrayList();
		FDOMimeConfigReader fDOMimeConfigReader = new FDOMimeConfigReader();
		int num = fDOMimeConfigReader.Init();
		if (num >= 32)
		{
			buffer = new byte[num];
			mime_available = true;
		}
	}

	public static string GetMimeTypeForFile(string filename)
	{
		lock (lock_object)
		{
			Instance.StartByFileName(filename);
		}
		return Instance.global_result;
	}

	public static string GetMimeTypeForData(byte[] data)
	{
		lock (lock_object)
		{
			Instance.StartDataLookup(data);
		}
		return Instance.global_result;
	}

	public static string GetMimeTypeForString(string input)
	{
		lock (lock_object)
		{
			Instance.StartStringLookup(input);
		}
		return Instance.global_result;
	}

	public static string GetMimeAlias(string mimetype)
	{
		return Aliases[mimetype];
	}

	public static string GetMimeSubClass(string mimetype)
	{
		return SubClasses[mimetype];
	}

	public static void CleanFileCache()
	{
		lock (lock_object)
		{
			Instance.mime_file_cache.Clear();
		}
	}

	private void StartByFileName(string filename)
	{
		if (mime_file_cache.ContainsKey(filename))
		{
			global_result = mime_file_cache[filename];
			return;
		}
		current_file_name = filename;
		is_zero_file = false;
		global_result = "application/octet-stream";
		GoByFileName();
		mime_file_cache.Add(current_file_name, global_result);
		if (mime_file_cache.Count <= 3000)
		{
			return;
		}
		IEnumerator enumerator = mime_file_cache.GetEnumerator();
		int num = 2500;
		while (enumerator.MoveNext())
		{
			mime_file_cache.Remove(enumerator.Current.ToString());
			num--;
			if (num == 0)
			{
				break;
			}
		}
	}

	private void StartDataLookup(byte[] data)
	{
		global_result = "application/octet-stream";
		Array.Clear(buffer, 0, buffer.Length);
		if (data.Length > buffer.Length)
		{
			Array.Copy(data, buffer, buffer.Length);
		}
		else
		{
			Array.Copy(data, buffer, data.Length);
		}
		if (!CheckMatch80Plus() && !CheckMatchBelow80())
		{
			CheckForBinaryOrText();
		}
	}

	private void StartStringLookup(string input)
	{
		global_result = "text/plain";
		search_string = input;
		if (!CheckForContentTypeString())
		{
		}
	}

	private void GoByFileName()
	{
		if (!MimeAvailable || !OpenFile())
		{
			CheckGlobalPatterns();
		}
		else if ((is_zero_file || !CheckMatch80Plus()) && !CheckGlobalPatterns() && !is_zero_file && !CheckMatchBelow80())
		{
			CheckForBinaryOrText();
		}
	}

	private bool CheckMatch80Plus()
	{
		foreach (Match matches80Plu in Matches80Plus)
		{
			if (TestMatch(matches80Plu))
			{
				global_result = matches80Plu.MimeType;
				return true;
			}
		}
		return false;
	}

	private bool FastEndsWidth(string input, string value)
	{
		if (value.Length > input.Length)
		{
			return false;
		}
		int num = input.Length - 1;
		for (int num2 = value.Length - 1; num2 > -1; num2--)
		{
			if (value[num2] != input[num])
			{
				return false;
			}
			num--;
		}
		return true;
	}

	private bool FastStartsWith(string input, string value)
	{
		if (value.Length > input.Length)
		{
			return false;
		}
		for (int i = 0; i < value.Length; i++)
		{
			if (value[i] != input[i])
			{
				return false;
			}
		}
		return true;
	}

	private int FastIndexOf(string input, char value)
	{
		if (input.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < input.Length; i++)
		{
			if (input[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	private int FastIndexOf(string input, string value)
	{
		if (input.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < input.Length - value.Length; i++)
		{
			if (input[i] == value[0])
			{
				int num = 0;
				for (int j = 1; j < value.Length && input[i + j] == value[j]; j++)
				{
					num++;
				}
				if (num == value.Length - 1)
				{
					return i;
				}
			}
		}
		return -1;
	}

	private void CheckGlobalResult()
	{
		int num = FastIndexOf(global_result, ',');
		if (num != -1)
		{
			global_result = global_result.Substring(0, num);
		}
	}

	private bool CheckGlobalPatterns()
	{
		string fileName = Path.GetFileName(current_file_name);
		for (int i = 0; i < GlobalLiterals.Count; i++)
		{
			string key = GlobalLiterals.GetKey(i);
			if (FastIndexOf(key, '[') == -1)
			{
				if (FastIndexOf(fileName, key) != -1)
				{
					global_result = GlobalLiterals[i];
					CheckGlobalResult();
					return true;
				}
			}
			else if (Regex.IsMatch(fileName, key))
			{
				global_result = GlobalLiterals[i];
				CheckGlobalResult();
				return true;
			}
		}
		if (FastIndexOf(fileName, '.') != -1)
		{
			for (int j = 0; j < GlobalPatternsLong.Count; j++)
			{
				string key2 = GlobalPatternsLong.GetKey(j);
				if (FastEndsWidth(fileName, key2))
				{
					global_result = GlobalPatternsLong[j];
					CheckGlobalResult();
					return true;
				}
				if (FastEndsWidth(fileName.ToLower(), key2))
				{
					global_result = GlobalPatternsLong[j];
					CheckGlobalResult();
					return true;
				}
			}
			string extension = Path.GetExtension(current_file_name);
			if (extension.Length != 0)
			{
				string text = GlobalPatternsShort[extension];
				if (text != null)
				{
					global_result = text;
					CheckGlobalResult();
					return true;
				}
				text = GlobalPatternsShort[extension.ToLower()];
				if (text != null)
				{
					global_result = text;
					CheckGlobalResult();
					return true;
				}
			}
		}
		for (int k = 0; k < GlobalSufPref.Count; k++)
		{
			string key3 = GlobalSufPref.GetKey(k);
			if (key3[0] == '*')
			{
				if (FastEndsWidth(fileName, key3.Replace("*", string.Empty)))
				{
					global_result = GlobalSufPref[k];
					CheckGlobalResult();
					return true;
				}
			}
			else if (FastStartsWith(fileName, key3.Replace("*", string.Empty)))
			{
				global_result = GlobalSufPref[k];
				CheckGlobalResult();
				return true;
			}
		}
		return false;
	}

	private bool CheckMatchBelow80()
	{
		foreach (Match item in MatchesBelow80)
		{
			if (TestMatch(item))
			{
				global_result = item.MimeType;
				return true;
			}
		}
		return false;
	}

	private void CheckForBinaryOrText()
	{
		for (int i = 0; i < 32; i++)
		{
			char c = Convert.ToChar(buffer[i]);
			if (c != '\t' && c != '\n' && c != '\r' && c != '\f' && c < ' ')
			{
				global_result = "application/octet-stream";
				return;
			}
		}
		global_result = "text/plain";
	}

	private bool TestMatch(Match match)
	{
		foreach (Matchlet matchlet in match.Matchlets)
		{
			if (TestMatchlet(matchlet))
			{
				return true;
			}
		}
		return false;
	}

	private bool TestMatchlet(Matchlet matchlet)
	{
		if (matchlet.Offset + matchlet.ByteValue.Length > bytes_read)
		{
			return false;
		}
		for (int i = 0; i < matchlet.OffsetLength; i++)
		{
			if (matchlet.Offset + i + matchlet.ByteValue.Length > bytes_read)
			{
				return false;
			}
			if (matchlet.Mask == null)
			{
				if (buffer[matchlet.Offset + i] != matchlet.ByteValue[0])
				{
					continue;
				}
				if (matchlet.ByteValue.Length == 1)
				{
					if (matchlet.Matchlets.Count <= 0)
					{
						return true;
					}
					foreach (Matchlet matchlet6 in matchlet.Matchlets)
					{
						if (TestMatchlet(matchlet6))
						{
							return true;
						}
					}
				}
				int num = 0;
				if (matchlet.ByteValue.Length > 2)
				{
					if (buffer[matchlet.Offset + i + matchlet.ByteValue.Length - 1] != matchlet.ByteValue[matchlet.ByteValue.Length - 1])
					{
						return false;
					}
					num = 1;
				}
				for (int j = 1; j < matchlet.ByteValue.Length - num; j++)
				{
					if (buffer[matchlet.Offset + i + j] != matchlet.ByteValue[j])
					{
						return false;
					}
				}
				if (matchlet.Matchlets.Count <= 0)
				{
					return true;
				}
				foreach (Matchlet matchlet7 in matchlet.Matchlets)
				{
					if (TestMatchlet(matchlet7))
					{
						return true;
					}
				}
			}
			else
			{
				if ((buffer[matchlet.Offset + i] & matchlet.Mask[0]) != (matchlet.ByteValue[0] & matchlet.Mask[0]))
				{
					continue;
				}
				if (matchlet.ByteValue.Length == 1)
				{
					if (matchlet.Matchlets.Count <= 0)
					{
						return true;
					}
					foreach (Matchlet matchlet8 in matchlet.Matchlets)
					{
						if (TestMatchlet(matchlet8))
						{
							return true;
						}
					}
				}
				int num2 = 0;
				if (matchlet.ByteValue.Length > 2)
				{
					if ((buffer[matchlet.Offset + i + matchlet.ByteValue.Length - 1] & matchlet.Mask[matchlet.ByteValue.Length - 1]) != (matchlet.ByteValue[matchlet.ByteValue.Length - 1] & matchlet.Mask[matchlet.ByteValue.Length - 1]))
					{
						return false;
					}
					num2 = 1;
				}
				for (int k = 1; k < matchlet.ByteValue.Length - num2; k++)
				{
					if ((buffer[matchlet.Offset + i + k] & matchlet.Mask[k]) != (matchlet.ByteValue[k] & matchlet.Mask[k]))
					{
						return false;
					}
				}
				if (matchlet.Matchlets.Count <= 0)
				{
					return true;
				}
				foreach (Matchlet matchlet9 in matchlet.Matchlets)
				{
					if (TestMatchlet(matchlet9))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool OpenFile()
	{
		try
		{
			file_stream = new FileStream(current_file_name, FileMode.Open, FileAccess.Read);
			if (file_stream.Length == 0L)
			{
				global_result = "application/x-zerosize";
				is_zero_file = true;
			}
			else
			{
				bytes_read = file_stream.Read(buffer, 0, buffer.Length);
				if (bytes_read < buffer.Length)
				{
					Array.Clear(buffer, bytes_read, buffer.Length - bytes_read);
				}
			}
			file_stream.Close();
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	private bool CheckForContentTypeString()
	{
		int num = search_string.IndexOf("Content-type:");
		if (num != -1)
		{
			num += 13;
			global_result = string.Empty;
			while (search_string[num] != ';')
			{
				global_result += search_string[num++];
			}
			global_result.Trim();
			return true;
		}
		byte[] bytes = new ASCIIEncoding().GetBytes(search_string);
		Array.Clear(buffer, 0, buffer.Length);
		if (bytes.Length > buffer.Length)
		{
			Array.Copy(bytes, buffer, buffer.Length);
		}
		else
		{
			Array.Copy(bytes, buffer, bytes.Length);
		}
		if (CheckMatch80Plus())
		{
			return true;
		}
		if (CheckMatchBelow80())
		{
			return true;
		}
		return false;
	}
}
