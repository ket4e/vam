using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Net.FtpClient;

public class FtpListItem : IFtpListItem
{
	public delegate FtpListItem Parser(string line, FtpCapability capabilities);

	private FtpFileSystemObjectType m_type;

	private string m_path;

	private string m_name;

	private string m_linkTarget;

	private FtpListItem m_linkObject;

	private DateTime m_modified = DateTime.MinValue;

	private DateTime m_created = DateTime.MinValue;

	private long m_size = -1L;

	private FtpSpecialPermissions m_specialPermissions;

	private FtpPermission m_ownerPermissions;

	private FtpPermission m_groupPermissions;

	private FtpPermission m_otherPermissions;

	private string m_input;

	private static object m_parserLock = new object();

	private static List<Parser> m_parsers = null;

	public FtpFileSystemObjectType Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	public string FullName
	{
		get
		{
			return m_path;
		}
		set
		{
			m_path = value;
		}
	}

	public string Name
	{
		get
		{
			if (m_name == null && m_path != null)
			{
				return m_path.GetFtpFileName();
			}
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	public string LinkTarget
	{
		get
		{
			return m_linkTarget;
		}
		set
		{
			m_linkTarget = value;
		}
	}

	public FtpListItem LinkObject
	{
		get
		{
			return m_linkObject;
		}
		set
		{
			m_linkObject = value;
		}
	}

	public DateTime Modified
	{
		get
		{
			return m_modified;
		}
		set
		{
			m_modified = value;
		}
	}

	public DateTime Created
	{
		get
		{
			return m_created;
		}
		set
		{
			m_created = value;
		}
	}

	public long Size
	{
		get
		{
			return m_size;
		}
		set
		{
			m_size = value;
		}
	}

	public FtpSpecialPermissions SpecialPermissions
	{
		get
		{
			return m_specialPermissions;
		}
		set
		{
			m_specialPermissions = value;
		}
	}

	public FtpPermission OwnerPermissions
	{
		get
		{
			return m_ownerPermissions;
		}
		set
		{
			m_ownerPermissions = value;
		}
	}

	public FtpPermission GroupPermissions
	{
		get
		{
			return m_groupPermissions;
		}
		set
		{
			m_groupPermissions = value;
		}
	}

	public FtpPermission OthersPermissions
	{
		get
		{
			return m_otherPermissions;
		}
		set
		{
			m_otherPermissions = value;
		}
	}

	public string Input
	{
		get
		{
			return m_input;
		}
		private set
		{
			m_input = value;
		}
	}

	private static Parser[] Parsers
	{
		get
		{
			lock (m_parserLock)
			{
				if (m_parsers == null)
				{
					InitParsers();
				}
				return m_parsers.ToArray();
			}
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		PropertyInfo[] properties = GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			stringBuilder.AppendLine($"{propertyInfo.Name}: {propertyInfo.GetValue(this, null)}");
		}
		return stringBuilder.ToString();
	}

	public static FtpListItem Parse(string path, string buf, FtpCapability capabilities)
	{
		if (buf != null && buf.Length > 0)
		{
			Parser[] parsers = Parsers;
			foreach (Parser parser in parsers)
			{
				FtpListItem ftpListItem;
				if ((ftpListItem = parser(buf, capabilities)) == null)
				{
					continue;
				}
				if (parser == new Parser(ParseVaxList))
				{
					ftpListItem.FullName = path + ftpListItem.Name;
				}
				else
				{
					FtpTrace.WriteLine(ftpListItem.Name);
					if (path.GetFtpFileName().Contains("*"))
					{
						path = path.GetFtpDirectoryName();
					}
					if (ftpListItem.Name != null)
					{
						if (ftpListItem.Name.StartsWith("/") || ftpListItem.Name.StartsWith("./") || ftpListItem.Name.StartsWith("../"))
						{
							ftpListItem.FullName = ftpListItem.Name;
							ftpListItem.Name = ftpListItem.Name.GetFtpFileName();
						}
						else if (path != null)
						{
							ftpListItem.FullName = path.GetFtpPath(ftpListItem.Name);
						}
						else
						{
							FtpTrace.WriteLine("Couldn't determine the full path of this object:{0}{1}", Environment.NewLine, ftpListItem.ToString());
						}
					}
					if (ftpListItem.LinkTarget != null && !ftpListItem.LinkTarget.StartsWith("/"))
					{
						if (ftpListItem.LinkTarget.StartsWith("./"))
						{
							ftpListItem.LinkTarget = path.GetFtpPath(ftpListItem.LinkTarget.Remove(0, 2));
						}
						else
						{
							ftpListItem.LinkTarget = path.GetFtpPath(ftpListItem.LinkTarget);
						}
					}
				}
				ftpListItem.Input = buf;
				return ftpListItem;
			}
		}
		return null;
	}

	private static void InitParsers()
	{
		lock (m_parserLock)
		{
			if (m_parsers == null)
			{
				m_parsers = new List<Parser>();
				m_parsers.Add(ParseMachineList);
				m_parsers.Add(ParseUnixList);
				m_parsers.Add(ParseDosList);
				m_parsers.Add(ParseVaxList);
			}
		}
	}

	public static void AddParser(Parser parser)
	{
		lock (m_parserLock)
		{
			if (m_parsers == null)
			{
				InitParsers();
			}
			m_parsers.Add(parser);
		}
	}

	public static void ClearParsers()
	{
		lock (m_parserLock)
		{
			if (m_parsers == null)
			{
				InitParsers();
			}
			m_parsers.Clear();
		}
	}

	public static void RemoveParser(Parser parser)
	{
		lock (m_parserLock)
		{
			if (m_parsers == null)
			{
				InitParsers();
			}
			m_parsers.Remove(parser);
		}
	}

	private static FtpListItem ParseMachineList(string buf, FtpCapability capabilities)
	{
		FtpListItem ftpListItem = new FtpListItem();
		Match match;
		if (!(match = Regex.Match(buf, "type=(?<type>.+?);", RegexOptions.IgnoreCase)).Success)
		{
			return null;
		}
		switch (match.Groups["type"].Value.ToLower())
		{
		case "dir":
		case "pdir":
		case "cdir":
			ftpListItem.Type = FtpFileSystemObjectType.Directory;
			break;
		case "file":
			ftpListItem.Type = FtpFileSystemObjectType.File;
			break;
		default:
			return null;
		}
		if ((match = Regex.Match(buf, "; (?<name>.*)$", RegexOptions.IgnoreCase)).Success)
		{
			ftpListItem.Name = match.Groups["name"].Value;
			if ((match = Regex.Match(buf, "modify=(?<modify>.+?);", RegexOptions.IgnoreCase)).Success)
			{
				ftpListItem.Modified = match.Groups["modify"].Value.GetFtpDate(DateTimeStyles.AssumeUniversal);
			}
			if ((match = Regex.Match(buf, "created?=(?<create>.+?);", RegexOptions.IgnoreCase)).Success)
			{
				ftpListItem.Created = match.Groups["create"].Value.GetFtpDate(DateTimeStyles.AssumeUniversal);
			}
			if ((match = Regex.Match(buf, "size=(?<size>\\d+);", RegexOptions.IgnoreCase)).Success && long.TryParse(match.Groups["size"].Value, out var result))
			{
				ftpListItem.Size = result;
			}
			if ((match = Regex.Match(buf, "unix.mode=(?<mode>\\d+);", RegexOptions.IgnoreCase)).Success)
			{
				if (match.Groups["mode"].Value.Length == 4)
				{
					ftpListItem.SpecialPermissions = (FtpSpecialPermissions)int.Parse(match.Groups["mode"].Value[0].ToString());
					ftpListItem.OwnerPermissions = (FtpPermission)int.Parse(match.Groups["mode"].Value[1].ToString());
					ftpListItem.GroupPermissions = (FtpPermission)int.Parse(match.Groups["mode"].Value[2].ToString());
					ftpListItem.OthersPermissions = (FtpPermission)int.Parse(match.Groups["mode"].Value[3].ToString());
				}
				else if (match.Groups["mode"].Value.Length == 3)
				{
					ftpListItem.OwnerPermissions = (FtpPermission)int.Parse(match.Groups["mode"].Value[0].ToString());
					ftpListItem.GroupPermissions = (FtpPermission)int.Parse(match.Groups["mode"].Value[1].ToString());
					ftpListItem.OthersPermissions = (FtpPermission)int.Parse(match.Groups["mode"].Value[2].ToString());
				}
			}
			return ftpListItem;
		}
		return null;
	}

	private static FtpListItem ParseUnixList(string buf, FtpCapability capabilities)
	{
		string pattern = "(?<permissions>.+)\\s+(?<objectcount>\\d+)\\s+(?<user>.+)\\s+(?<group>.+)\\s+(?<size>\\d+)\\s+(?<modify>\\w+\\s+\\d+\\s+\\d+:\\d+|\\w+\\s+\\d+\\s+\\d+)\\s(?<name>.*)$";
		FtpListItem ftpListItem = new FtpListItem();
		Match match;
		if (!(match = Regex.Match(buf, pattern, RegexOptions.IgnoreCase)).Success)
		{
			return null;
		}
		if (match.Groups["permissions"].Value.Length == 0)
		{
			return null;
		}
		switch (match.Groups["permissions"].Value[0])
		{
		case 'd':
			ftpListItem.Type = FtpFileSystemObjectType.Directory;
			break;
		case '-':
		case 's':
			ftpListItem.Type = FtpFileSystemObjectType.File;
			break;
		case 'l':
			ftpListItem.Type = FtpFileSystemObjectType.Link;
			break;
		default:
			return null;
		}
		if (match.Groups["name"].Value.Length < 1)
		{
			return null;
		}
		ftpListItem.Name = match.Groups["name"].Value;
		switch (ftpListItem.Type)
		{
		case FtpFileSystemObjectType.Directory:
			if (ftpListItem.Name == "." || ftpListItem.Name == "..")
			{
				return null;
			}
			break;
		case FtpFileSystemObjectType.Link:
			if (!ftpListItem.Name.Contains(" -> "))
			{
				return null;
			}
			ftpListItem.LinkTarget = ftpListItem.Name.Remove(0, ftpListItem.Name.IndexOf("-> ") + 3);
			ftpListItem.Name = ftpListItem.Name.Remove(ftpListItem.Name.IndexOf(" -> "));
			break;
		}
		if (((capabilities & FtpCapability.MDTM) != FtpCapability.MDTM || ftpListItem.Type == FtpFileSystemObjectType.Directory) && match.Groups["modify"].Value.Length > 0)
		{
			ftpListItem.Modified = match.Groups["modify"].Value.GetFtpDate(DateTimeStyles.AssumeLocal);
			if (ftpListItem.Modified == DateTime.MinValue)
			{
				FtpTrace.WriteLine("GetFtpDate() failed on {0}", match.Groups["modify"].Value);
			}
		}
		else if (match.Groups["modify"].Value.Length == 0)
		{
			FtpTrace.WriteLine("RegEx failed to parse modified date from {0}.", buf);
		}
		else if (ftpListItem.Type == FtpFileSystemObjectType.Directory)
		{
			FtpTrace.WriteLine("Modified times of directories are ignored in UNIX long listings.");
		}
		else if ((capabilities & FtpCapability.MDTM) == FtpCapability.MDTM)
		{
			FtpTrace.WriteLine("Ignoring modified date because MDTM feature is present. If you aren't already, pass FtpListOption.Modify or FtpListOption.SizeModify to GetListing() to retrieve the modification time.");
		}
		if (match.Groups["size"].Value.Length > 0 && long.TryParse(match.Groups["size"].Value, out var result))
		{
			ftpListItem.Size = result;
		}
		if (match.Groups["permissions"].Value.Length > 0)
		{
			Match match2 = Regex.Match(match.Groups["permissions"].Value, "[\\w-]{1}(?<owner>[\\w-]{3})(?<group>[\\w-]{3})(?<others>[\\w-]{3})", RegexOptions.IgnoreCase);
			if (match2.Success)
			{
				if (match2.Groups["owner"].Value.Length == 3)
				{
					if (match2.Groups["owner"].Value[0] == 'r')
					{
						ftpListItem.OwnerPermissions |= FtpPermission.Read;
					}
					if (match2.Groups["owner"].Value[1] == 'w')
					{
						ftpListItem.OwnerPermissions |= FtpPermission.Write;
					}
					if (match2.Groups["owner"].Value[2] == 'x' || match2.Groups["owner"].Value[2] == 's')
					{
						ftpListItem.OwnerPermissions |= FtpPermission.Execute;
					}
					if (match2.Groups["owner"].Value[2] == 's' || match2.Groups["owner"].Value[2] == 'S')
					{
						ftpListItem.SpecialPermissions |= FtpSpecialPermissions.SetUserID;
					}
				}
				if (match2.Groups["group"].Value.Length == 3)
				{
					if (match2.Groups["group"].Value[0] == 'r')
					{
						ftpListItem.GroupPermissions |= FtpPermission.Read;
					}
					if (match2.Groups["group"].Value[1] == 'w')
					{
						ftpListItem.GroupPermissions |= FtpPermission.Write;
					}
					if (match2.Groups["group"].Value[2] == 'x' || match2.Groups["group"].Value[2] == 's')
					{
						ftpListItem.GroupPermissions |= FtpPermission.Execute;
					}
					if (match2.Groups["group"].Value[2] == 's' || match2.Groups["group"].Value[2] == 'S')
					{
						ftpListItem.SpecialPermissions |= FtpSpecialPermissions.SetGroupID;
					}
				}
				if (match2.Groups["others"].Value.Length == 3)
				{
					if (match2.Groups["others"].Value[0] == 'r')
					{
						ftpListItem.OthersPermissions |= FtpPermission.Read;
					}
					if (match2.Groups["others"].Value[1] == 'w')
					{
						ftpListItem.OthersPermissions |= FtpPermission.Write;
					}
					if (match2.Groups["others"].Value[2] == 'x' || match2.Groups["others"].Value[2] == 't')
					{
						ftpListItem.OthersPermissions |= FtpPermission.Execute;
					}
					if (match2.Groups["others"].Value[2] == 't' || match2.Groups["others"].Value[2] == 'T')
					{
						ftpListItem.SpecialPermissions |= FtpSpecialPermissions.Sticky;
					}
				}
			}
		}
		return ftpListItem;
	}

	private static FtpListItem ParseDosList(string buf, FtpCapability capabilities)
	{
		FtpListItem ftpListItem = new FtpListItem();
		string[] formats = new string[2] { "MM-dd-yy  hh:mmtt", "MM-dd-yyyy  hh:mmtt" };
		Match match;
		if ((match = Regex.Match(buf, "(?<modify>\\d+-\\d+-\\d+\\s+\\d+:\\d+\\w+)\\s+<DIR>\\s+(?<name>.*)$", RegexOptions.IgnoreCase)).Success)
		{
			ftpListItem.Type = FtpFileSystemObjectType.Directory;
			ftpListItem.Name = match.Groups["name"].Value;
			if (DateTime.TryParseExact(match.Groups["modify"].Value, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
			{
				ftpListItem.Modified = result;
			}
		}
		else
		{
			if (!(match = Regex.Match(buf, "(?<modify>\\d+-\\d+-\\d+\\s+\\d+:\\d+\\w+)\\s+(?<size>\\d+)\\s+(?<name>.*)$", RegexOptions.IgnoreCase)).Success)
			{
				return null;
			}
			ftpListItem.Type = FtpFileSystemObjectType.File;
			ftpListItem.Name = match.Groups["name"].Value;
			if (long.TryParse(match.Groups["size"].Value, out var result2))
			{
				ftpListItem.Size = result2;
			}
			if (DateTime.TryParseExact(match.Groups["modify"].Value, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result3))
			{
				ftpListItem.Modified = result3;
			}
		}
		return ftpListItem;
	}

	private static FtpListItem ParseVaxList(string buf, FtpCapability capabilities)
	{
		string pattern = "(?<name>.+)\\.(?<extension>.+);(?<version>\\d+)\\s+(?<size>\\d+)\\s+(?<modify>\\d+-\\w+-\\d+\\s+\\d+:\\d+)";
		Match match;
		if ((match = Regex.Match(buf, pattern)).Success)
		{
			FtpListItem ftpListItem = new FtpListItem();
			ftpListItem.m_name = string.Format("{0}.{1};{2}", match.Groups["name"].Value, match.Groups["extension"].Value, match.Groups["version"].Value);
			if (match.Groups["extension"].Value.ToUpper() == "DIR")
			{
				ftpListItem.m_type = FtpFileSystemObjectType.Directory;
			}
			else
			{
				ftpListItem.m_type = FtpFileSystemObjectType.File;
			}
			if (!long.TryParse(match.Groups["size"].Value, out ftpListItem.m_size))
			{
				ftpListItem.m_size = -1L;
			}
			if (!DateTime.TryParse(match.Groups["modify"].Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out ftpListItem.m_modified))
			{
				ftpListItem.m_modified = DateTime.MinValue;
			}
			return ftpListItem;
		}
		return null;
	}
}
