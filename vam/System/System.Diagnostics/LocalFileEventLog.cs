using System.Collections;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;

namespace System.Diagnostics;

internal class LocalFileEventLog : System.Diagnostics.EventLogImpl
{
	private const string DateFormat = "yyyyMMddHHmmssfff";

	private static readonly object lockObject = new object();

	private FileSystemWatcher file_watcher;

	private int last_notification_index;

	private bool _notifying;

	private bool RunningOnUnix
	{
		get
		{
			int platform = (int)Environment.OSVersion.Platform;
			return platform == 4 || platform == 128 || platform == 6;
		}
	}

	private string EventLogStore
	{
		get
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_EVENTLOG_TYPE");
			if (environmentVariable != null && environmentVariable.Length > "local".Length + 1)
			{
				return environmentVariable.Substring("local".Length + 1);
			}
			if (RunningOnUnix)
			{
				return "/var/lib/mono/eventlog";
			}
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "mono\\eventlog");
		}
	}

	public override OverflowAction OverflowAction => OverflowAction.DoNotOverwrite;

	public override int MinimumRetentionDays => int.MaxValue;

	public override long MaximumKilobytes
	{
		get
		{
			return long.MaxValue;
		}
		set
		{
			throw new NotSupportedException("This EventLog implementation does not support setting max kilobytes policy");
		}
	}

	public LocalFileEventLog(EventLog coreEventLog)
		: base(coreEventLog)
	{
	}

	public override void BeginInit()
	{
	}

	public override void Clear()
	{
		string path = FindLogStore(base.CoreEventLog.Log);
		if (Directory.Exists(path))
		{
			string[] files = Directory.GetFiles(path, "*.log");
			foreach (string path2 in files)
			{
				File.Delete(path2);
			}
		}
	}

	public override void Close()
	{
		if (file_watcher != null)
		{
			file_watcher.EnableRaisingEvents = false;
			file_watcher = null;
		}
	}

	public override void CreateEventSource(EventSourceCreationData sourceData)
	{
		string text = FindLogStore(sourceData.LogName);
		if (!Directory.Exists(text))
		{
			ValidateCustomerLogName(sourceData.LogName, sourceData.MachineName);
			Directory.CreateDirectory(text);
			Directory.CreateDirectory(Path.Combine(text, sourceData.LogName));
			if (RunningOnUnix)
			{
				ModifyAccessPermissions(text, "777");
				ModifyAccessPermissions(text, "+t");
			}
		}
		string path = Path.Combine(text, sourceData.Source);
		Directory.CreateDirectory(path);
	}

	public override void Delete(string logName, string machineName)
	{
		string path = FindLogStore(logName);
		if (!Directory.Exists(path))
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Event Log '{0}' does not exist on computer '{1}'.", logName, machineName));
		}
		Directory.Delete(path, recursive: true);
	}

	public override void DeleteEventSource(string source, string machineName)
	{
		if (!Directory.Exists(EventLogStore))
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source '{0}' is not registered on computer '{1}'.", source, machineName));
		}
		string text = FindSourceDirectory(source);
		if (text == null)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source '{0}' is not registered on computer '{1}'.", source, machineName));
		}
		Directory.Delete(text);
	}

	public override void Dispose(bool disposing)
	{
		Close();
	}

	public override void DisableNotification()
	{
		if (file_watcher != null)
		{
			file_watcher.EnableRaisingEvents = false;
		}
	}

	public override void EnableNotification()
	{
		if (file_watcher == null)
		{
			string path = FindLogStore(base.CoreEventLog.Log);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			file_watcher = new FileSystemWatcher();
			file_watcher.Path = path;
			file_watcher.Created += delegate
			{
				lock (this)
				{
					if (_notifying)
					{
						return;
					}
					_notifying = true;
				}
				Thread.Sleep(100);
				try
				{
					while (GetLatestIndex() > last_notification_index)
					{
						try
						{
							base.CoreEventLog.OnEntryWritten(GetEntry(last_notification_index++));
						}
						catch (Exception)
						{
						}
					}
				}
				finally
				{
					lock (this)
					{
						_notifying = false;
					}
				}
			};
		}
		last_notification_index = GetLatestIndex();
		file_watcher.EnableRaisingEvents = true;
	}

	public override void EndInit()
	{
	}

	public override bool Exists(string logName, string machineName)
	{
		string path = FindLogStore(logName);
		return Directory.Exists(path);
	}

	[System.MonoTODO("Use MessageTable from PE for lookup")]
	protected override string FormatMessage(string source, uint eventID, string[] replacementStrings)
	{
		return string.Join(", ", replacementStrings);
	}

	protected override int GetEntryCount()
	{
		string path = FindLogStore(base.CoreEventLog.Log);
		if (!Directory.Exists(path))
		{
			return 0;
		}
		string[] files = Directory.GetFiles(path, "*.log");
		return files.Length;
	}

	protected override EventLogEntry GetEntry(int index)
	{
		string path = FindLogStore(base.CoreEventLog.Log);
		string path2 = Path.Combine(path, (index + 1).ToString(CultureInfo.InvariantCulture) + ".log");
		using TextReader textReader = File.OpenText(path2);
		int index2 = int.Parse(Path.GetFileNameWithoutExtension(path2), CultureInfo.InvariantCulture);
		uint num = uint.Parse(textReader.ReadLine().Substring(12), CultureInfo.InvariantCulture);
		EventLogEntryType entryType = (EventLogEntryType)(int)Enum.Parse(typeof(EventLogEntryType), textReader.ReadLine().Substring(11));
		string source = textReader.ReadLine().Substring(8);
		string text = textReader.ReadLine().Substring(10);
		short categoryNumber = short.Parse(text, CultureInfo.InvariantCulture);
		string category = "(" + text + ")";
		DateTime timeGenerated = DateTime.ParseExact(textReader.ReadLine().Substring(15), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
		DateTime lastWriteTime = File.GetLastWriteTime(path2);
		int num2 = int.Parse(textReader.ReadLine().Substring(20));
		ArrayList arrayList = new ArrayList();
		StringBuilder stringBuilder = new StringBuilder();
		while (arrayList.Count < num2)
		{
			char c = (char)textReader.Read();
			if (c == '\0')
			{
				arrayList.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		string[] array = new string[arrayList.Count];
		arrayList.CopyTo(array, 0);
		string message = FormatMessage(source, num, array);
		int eventID = EventLog.GetEventID(num);
		byte[] data = Convert.FromBase64String(textReader.ReadToEnd());
		return new EventLogEntry(category, categoryNumber, index2, eventID, source, message, null, Environment.MachineName, entryType, timeGenerated, lastWriteTime, data, array, num);
	}

	[System.MonoTODO]
	protected override string GetLogDisplayName()
	{
		return base.CoreEventLog.Log;
	}

	protected override string[] GetLogNames(string machineName)
	{
		if (!Directory.Exists(EventLogStore))
		{
			return new string[0];
		}
		string[] directories = Directory.GetDirectories(EventLogStore, "*");
		string[] array = new string[directories.Length];
		for (int i = 0; i < directories.Length; i++)
		{
			array[i] = Path.GetFileName(directories[i]);
		}
		return array;
	}

	public override string LogNameFromSourceName(string source, string machineName)
	{
		if (!Directory.Exists(EventLogStore))
		{
			return string.Empty;
		}
		string text = FindSourceDirectory(source);
		if (text == null)
		{
			return string.Empty;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(text);
		return directoryInfo.Parent.Name;
	}

	public override bool SourceExists(string source, string machineName)
	{
		if (!Directory.Exists(EventLogStore))
		{
			return false;
		}
		string text = FindSourceDirectory(source);
		return text != null;
	}

	public override void WriteEntry(string[] replacementStrings, EventLogEntryType type, uint instanceID, short category, byte[] rawData)
	{
		lock (lockObject)
		{
			string path = FindLogStore(base.CoreEventLog.Log);
			string path2 = Path.Combine(path, (GetLatestIndex() + 1).ToString(CultureInfo.InvariantCulture) + ".log");
			try
			{
				using TextWriter textWriter = File.CreateText(path2);
				textWriter.WriteLine("InstanceID: {0}", instanceID.ToString(CultureInfo.InvariantCulture));
				textWriter.WriteLine("EntryType: {0}", (int)type);
				textWriter.WriteLine("Source: {0}", base.CoreEventLog.Source);
				textWriter.WriteLine("Category: {0}", category.ToString(CultureInfo.InvariantCulture));
				textWriter.WriteLine("TimeGenerated: {0}", DateTime.Now.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture));
				textWriter.WriteLine("ReplacementStrings: {0}", replacementStrings.Length.ToString(CultureInfo.InvariantCulture));
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value in replacementStrings)
				{
					stringBuilder.Append(value);
					stringBuilder.Append('\0');
				}
				textWriter.Write(stringBuilder.ToString());
				textWriter.Write(Convert.ToBase64String(rawData));
			}
			catch (IOException)
			{
				File.Delete(path2);
			}
		}
	}

	private string FindSourceDirectory(string source)
	{
		string result = null;
		string[] directories = Directory.GetDirectories(EventLogStore, "*");
		for (int i = 0; i < directories.Length; i++)
		{
			string[] directories2 = Directory.GetDirectories(directories[i], "*");
			for (int j = 0; j < directories2.Length; j++)
			{
				string fileName = Path.GetFileName(directories2[j]);
				if (string.Compare(fileName, source, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
				{
					result = directories2[j];
					break;
				}
			}
		}
		return result;
	}

	private string FindLogStore(string logName)
	{
		if (!Directory.Exists(EventLogStore))
		{
			return Path.Combine(EventLogStore, logName);
		}
		string[] directories = Directory.GetDirectories(EventLogStore, "*");
		for (int i = 0; i < directories.Length; i++)
		{
			string fileName = Path.GetFileName(directories[i]);
			if (string.Compare(fileName, logName, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
			{
				return directories[i];
			}
		}
		return Path.Combine(EventLogStore, logName);
	}

	private int GetLatestIndex()
	{
		int num = 0;
		string[] files = Directory.GetFiles(FindLogStore(base.CoreEventLog.Log), "*.log");
		for (int i = 0; i < files.Length; i++)
		{
			try
			{
				string path = files[i];
				int num2 = int.Parse(Path.GetFileNameWithoutExtension(path), CultureInfo.InvariantCulture);
				if (num2 > num)
				{
					num = num2;
				}
			}
			catch
			{
			}
		}
		return num;
	}

	private static void ModifyAccessPermissions(string path, string permissions)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "chmod";
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.RedirectStandardError = true;
		processStartInfo.UseShellExecute = false;
		processStartInfo.Arguments = $"{permissions} \"{path}\"";
		Process process = null;
		try
		{
			process = Process.Start(processStartInfo);
		}
		catch (Exception inner)
		{
			throw new SecurityException("Access permissions could not be modified.", inner);
		}
		process.WaitForExit();
		if (process.ExitCode != 0)
		{
			process.Close();
			throw new SecurityException("Access permissions could not be modified.");
		}
		process.Close();
	}

	public override void ModifyOverflowPolicy(OverflowAction action, int retentionDays)
	{
		throw new NotSupportedException("This EventLog implementation does not support modifying overflow policy");
	}

	public override void RegisterDisplayName(string resourceFile, long resourceId)
	{
		throw new NotSupportedException("This EventLog implementation does not support registering display name");
	}
}
