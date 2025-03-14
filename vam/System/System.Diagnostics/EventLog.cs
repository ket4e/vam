using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Diagnostics;

[InstallerType(typeof(System.Diagnostics.EventLogInstaller))]
[MonitoringDescription("Represents an event log")]
[DefaultEvent("EntryWritten")]
public class EventLog : Component, ISupportInitialize
{
	internal const string LOCAL_FILE_IMPL = "local";

	private const string WIN32_IMPL = "win32";

	private const string NULL_IMPL = "null";

	internal const string EVENTLOG_TYPE_VAR = "MONO_EVENTLOG_TYPE";

	private string source;

	private string logName;

	private string machineName;

	private bool doRaiseEvents;

	private ISynchronizeInvoke synchronizingObject;

	private System.Diagnostics.EventLogImpl Impl;

	[DefaultValue(false)]
	[MonitoringDescription("If enabled raises event when a log is written.")]
	[Browsable(false)]
	public bool EnableRaisingEvents
	{
		get
		{
			return doRaiseEvents;
		}
		set
		{
			if (value != doRaiseEvents)
			{
				if (value)
				{
					Impl.EnableNotification();
				}
				else
				{
					Impl.DisableNotification();
				}
				doRaiseEvents = value;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[MonitoringDescription("The entries in the log.")]
	public EventLogEntryCollection Entries => new EventLogEntryCollection(Impl);

	[DefaultValue("")]
	[MonitoringDescription("Name of the log that is read and written.")]
	[TypeConverter("System.Diagnostics.Design.LogConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[RecommendedAsConfigurable(true)]
	[ReadOnly(true)]
	public string Log
	{
		get
		{
			if (source != null && source.Length > 0)
			{
				return GetLogName();
			}
			return logName;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (string.Compare(logName, value, ignoreCase: true) != 0)
			{
				logName = value;
				Reset();
			}
		}
	}

	[Browsable(false)]
	public string LogDisplayName => Impl.LogDisplayName;

	[ReadOnly(true)]
	[MonitoringDescription("Name of the machine that this log get written to.")]
	[RecommendedAsConfigurable(true)]
	[DefaultValue(".")]
	public string MachineName
	{
		get
		{
			return machineName;
		}
		set
		{
			if (value == null || value.Trim().Length == 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value {0} for property MachineName.", value));
			}
			if (string.Compare(machineName, value, ignoreCase: true) != 0)
			{
				Close();
				machineName = value;
			}
		}
	}

	[ReadOnly(true)]
	[MonitoringDescription("The application name that writes the log.")]
	[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[RecommendedAsConfigurable(true)]
	[DefaultValue("")]
	public string Source
	{
		get
		{
			return source;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (source == null || (source.Length == 0 && (logName == null || logName.Length == 0)))
			{
				source = value;
			}
			else if (string.Compare(source, value, ignoreCase: true) != 0)
			{
				source = value;
				Reset();
			}
		}
	}

	[MonitoringDescription("An object that synchronizes event handler calls.")]
	[Browsable(false)]
	[DefaultValue(null)]
	public ISynchronizeInvoke SynchronizingObject
	{
		get
		{
			return synchronizingObject;
		}
		set
		{
			synchronizingObject = value;
		}
	}

	[System.MonoTODO]
	[ComVisible(false)]
	[Browsable(false)]
	public OverflowAction OverflowAction => Impl.OverflowAction;

	[System.MonoTODO]
	[Browsable(false)]
	[ComVisible(false)]
	public int MinimumRetentionDays => Impl.MinimumRetentionDays;

	[System.MonoTODO]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[ComVisible(false)]
	public long MaximumKilobytes
	{
		get
		{
			return Impl.MaximumKilobytes;
		}
		set
		{
			Impl.MaximumKilobytes = value;
		}
	}

	private static bool Win32EventLogEnabled => Environment.OSVersion.Platform == PlatformID.Win32NT;

	private static string EventLogImplType
	{
		get
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_EVENTLOG_TYPE");
			if (environmentVariable == null)
			{
				if (Win32EventLogEnabled)
				{
					return "win32";
				}
				return "null";
			}
			if (Win32EventLogEnabled && string.Compare(environmentVariable, "win32", ignoreCase: true) == 0)
			{
				return "win32";
			}
			if (string.Compare(environmentVariable, "null", ignoreCase: true) == 0)
			{
				return "null";
			}
			if (string.Compare(environmentVariable, 0, "local", 0, "local".Length, ignoreCase: true) == 0)
			{
				return "local";
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Eventlog implementation '{0}' is not supported.", environmentVariable));
		}
	}

	[MonitoringDescription("Raised for each EventLog entry written.")]
	public event EntryWrittenEventHandler EntryWritten;

	public EventLog()
		: this(string.Empty)
	{
	}

	public EventLog(string logName)
		: this(logName, ".")
	{
	}

	public EventLog(string logName, string machineName)
		: this(logName, machineName, string.Empty)
	{
	}

	public EventLog(string logName, string machineName, string source)
	{
		if (logName == null)
		{
			throw new ArgumentNullException("logName");
		}
		if (machineName == null || machineName.Trim().Length == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", machineName));
		}
		this.source = source;
		this.machineName = machineName;
		this.logName = logName;
		Impl = CreateEventLogImpl(this);
	}

	[ComVisible(false)]
	[System.MonoTODO]
	public void ModifyOverflowPolicy(OverflowAction action, int retentionDays)
	{
		Impl.ModifyOverflowPolicy(action, retentionDays);
	}

	[System.MonoTODO]
	[ComVisible(false)]
	public void RegisterDisplayName(string resourceFile, long resourceId)
	{
		Impl.RegisterDisplayName(resourceFile, resourceId);
	}

	public void BeginInit()
	{
		Impl.BeginInit();
	}

	public void Clear()
	{
		string log = Log;
		if (log == null || log.Length == 0)
		{
			throw new ArgumentException("Log property value has not been specified.");
		}
		if (!Exists(log, MachineName))
		{
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Event Log '{0}' does not exist on computer '{1}'.", log, machineName));
		}
		Impl.Clear();
		Reset();
	}

	public void Close()
	{
		Impl.Close();
		EnableRaisingEvents = false;
	}

	internal void Reset()
	{
		bool enableRaisingEvents = EnableRaisingEvents;
		Close();
		EnableRaisingEvents = enableRaisingEvents;
	}

	public static void CreateEventSource(string source, string logName)
	{
		CreateEventSource(source, logName, ".");
	}

	[Obsolete("use CreateEventSource(EventSourceCreationData) instead")]
	public static void CreateEventSource(string source, string logName, string machineName)
	{
		CreateEventSource(new EventSourceCreationData(source, logName, machineName));
	}

	[System.MonoNotSupported("remote machine is not supported")]
	public static void CreateEventSource(EventSourceCreationData sourceData)
	{
		if (sourceData.Source == null || sourceData.Source.Length == 0)
		{
			throw new ArgumentException("Source property value has not been specified.");
		}
		if (sourceData.LogName == null || sourceData.LogName.Length == 0)
		{
			throw new ArgumentException("Log property value has not been specified.");
		}
		if (SourceExists(sourceData.Source, sourceData.MachineName))
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Source '{0}' already exists on '{1}'.", sourceData.Source, sourceData.MachineName));
		}
		System.Diagnostics.EventLogImpl eventLogImpl = CreateEventLogImpl(sourceData.LogName, sourceData.MachineName, sourceData.Source);
		eventLogImpl.CreateEventSource(sourceData);
	}

	public static void Delete(string logName)
	{
		Delete(logName, ".");
	}

	[System.MonoNotSupported("remote machine is not supported")]
	public static void Delete(string logName, string machineName)
	{
		if (machineName == null || machineName.Trim().Length == 0)
		{
			throw new ArgumentException("Invalid format for argument machineName.");
		}
		if (logName == null || logName.Length == 0)
		{
			throw new ArgumentException("Log to delete was not specified.");
		}
		System.Diagnostics.EventLogImpl eventLogImpl = CreateEventLogImpl(logName, machineName, string.Empty);
		eventLogImpl.Delete(logName, machineName);
	}

	public static void DeleteEventSource(string source)
	{
		DeleteEventSource(source, ".");
	}

	[System.MonoNotSupported("remote machine is not supported")]
	public static void DeleteEventSource(string source, string machineName)
	{
		if (machineName == null || machineName.Trim().Length == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", machineName));
		}
		System.Diagnostics.EventLogImpl eventLogImpl = CreateEventLogImpl(string.Empty, machineName, source);
		eventLogImpl.DeleteEventSource(source, machineName);
	}

	protected override void Dispose(bool disposing)
	{
		if (Impl != null)
		{
			Impl.Dispose(disposing);
		}
	}

	public void EndInit()
	{
		Impl.EndInit();
	}

	public static bool Exists(string logName)
	{
		return Exists(logName, ".");
	}

	[System.MonoNotSupported("remote machine is not supported")]
	public static bool Exists(string logName, string machineName)
	{
		if (machineName == null || machineName.Trim().Length == 0)
		{
			throw new ArgumentException("Invalid format for argument machineName.");
		}
		if (logName == null || logName.Length == 0)
		{
			return false;
		}
		System.Diagnostics.EventLogImpl eventLogImpl = CreateEventLogImpl(logName, machineName, string.Empty);
		return eventLogImpl.Exists(logName, machineName);
	}

	public static EventLog[] GetEventLogs()
	{
		return GetEventLogs(".");
	}

	[System.MonoNotSupported("remote machine is not supported")]
	public static EventLog[] GetEventLogs(string machineName)
	{
		System.Diagnostics.EventLogImpl eventLogImpl = CreateEventLogImpl(new EventLog());
		return eventLogImpl.GetEventLogs(machineName);
	}

	[System.MonoNotSupported("remote machine is not supported")]
	public static string LogNameFromSourceName(string source, string machineName)
	{
		if (machineName == null || machineName.Trim().Length == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'MachineName'.", machineName));
		}
		System.Diagnostics.EventLogImpl eventLogImpl = CreateEventLogImpl(string.Empty, machineName, source);
		return eventLogImpl.LogNameFromSourceName(source, machineName);
	}

	public static bool SourceExists(string source)
	{
		return SourceExists(source, ".");
	}

	[System.MonoNotSupported("remote machine is not supported")]
	public static bool SourceExists(string source, string machineName)
	{
		if (machineName == null || machineName.Trim().Length == 0)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for parameter 'machineName'.", machineName));
		}
		System.Diagnostics.EventLogImpl eventLogImpl = CreateEventLogImpl(string.Empty, machineName, source);
		return eventLogImpl.SourceExists(source, machineName);
	}

	public void WriteEntry(string message)
	{
		WriteEntry(message, EventLogEntryType.Information);
	}

	public void WriteEntry(string message, EventLogEntryType type)
	{
		WriteEntry(message, type, 0);
	}

	public void WriteEntry(string message, EventLogEntryType type, int eventID)
	{
		WriteEntry(message, type, eventID, 0);
	}

	public void WriteEntry(string message, EventLogEntryType type, int eventID, short category)
	{
		WriteEntry(message, type, eventID, category, null);
	}

	public void WriteEntry(string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
	{
		WriteEntry(new string[1] { message }, type, eventID, category, rawData);
	}

	public static void WriteEntry(string source, string message)
	{
		WriteEntry(source, message, EventLogEntryType.Information);
	}

	public static void WriteEntry(string source, string message, EventLogEntryType type)
	{
		WriteEntry(source, message, type, 0);
	}

	public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID)
	{
		WriteEntry(source, message, type, eventID, 0);
	}

	public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category)
	{
		WriteEntry(source, message, type, eventID, category, null);
	}

	public static void WriteEntry(string source, string message, EventLogEntryType type, int eventID, short category, byte[] rawData)
	{
		using EventLog eventLog = new EventLog();
		eventLog.Source = source;
		eventLog.WriteEntry(message, type, eventID, category, rawData);
	}

	[ComVisible(false)]
	public void WriteEvent(EventInstance instance, params object[] values)
	{
		WriteEvent(instance, null, values);
	}

	[ComVisible(false)]
	public void WriteEvent(EventInstance instance, byte[] data, params object[] values)
	{
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		string[] array = null;
		if (values != null)
		{
			array = new string[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				object obj = values[i];
				if (obj == null)
				{
					array[i] = string.Empty;
				}
				else
				{
					array[i] = values[i].ToString();
				}
			}
		}
		else
		{
			array = new string[0];
		}
		WriteEntry(array, instance.EntryType, instance.InstanceId, (short)instance.CategoryId, data);
	}

	public static void WriteEvent(string source, EventInstance instance, params object[] values)
	{
		WriteEvent(source, instance, null, values);
	}

	public static void WriteEvent(string source, EventInstance instance, byte[] data, params object[] values)
	{
		using EventLog eventLog = new EventLog();
		eventLog.Source = source;
		eventLog.WriteEvent(instance, data, values);
	}

	internal void OnEntryWritten(EventLogEntry newEntry)
	{
		if (doRaiseEvents && this.EntryWritten != null)
		{
			this.EntryWritten(this, new EntryWrittenEventArgs(newEntry));
		}
	}

	internal string GetLogName()
	{
		if (logName != null && logName.Length > 0)
		{
			return logName;
		}
		logName = LogNameFromSourceName(source, machineName);
		return logName;
	}

	private static System.Diagnostics.EventLogImpl CreateEventLogImpl(string logName, string machineName, string source)
	{
		EventLog eventLog = new EventLog(logName, machineName, source);
		return CreateEventLogImpl(eventLog);
	}

	private static System.Diagnostics.EventLogImpl CreateEventLogImpl(EventLog eventLog)
	{
		return EventLogImplType switch
		{
			"local" => new System.Diagnostics.LocalFileEventLog(eventLog), 
			"win32" => new System.Diagnostics.Win32EventLog(eventLog), 
			"null" => new System.Diagnostics.NullEventLog(eventLog), 
			_ => throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Eventlog implementation '{0}' is not supported.", EventLogImplType)), 
		};
	}

	private void WriteEntry(string[] replacementStrings, EventLogEntryType type, long instanceID, short category, byte[] rawData)
	{
		if (Source.Length == 0)
		{
			throw new ArgumentException("Source property was not setbefore writing to the event log.");
		}
		if (!Enum.IsDefined(typeof(EventLogEntryType), type))
		{
			throw new InvalidEnumArgumentException("type", (int)type, typeof(EventLogEntryType));
		}
		ValidateEventID(instanceID);
		if (!SourceExists(Source, MachineName))
		{
			if (Log == null || Log.Length == 0)
			{
				Log = "Application";
			}
			CreateEventSource(Source, Log, MachineName);
		}
		else if (logName != null && logName.Length != 0)
		{
			string text = LogNameFromSourceName(Source, MachineName);
			if (string.Compare(logName, text, ignoreCase: true, CultureInfo.InvariantCulture) != 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The source '{0}' is not registered in log '{1}' (it is registered in log '{2}'). The Source and Log properties must be matched, or you may set Log to the empty string, and it will automatically be matched to the Source property.", Source, logName, text));
			}
		}
		if (rawData == null)
		{
			rawData = new byte[0];
		}
		Impl.WriteEntry(replacementStrings, type, (uint)instanceID, category, rawData);
	}

	private void ValidateEventID(long instanceID)
	{
		int eventID = GetEventID(instanceID);
		if (eventID < 0 || eventID > 65535)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid eventID value '{0}'. It must be in the range between '{1}' and '{2}'.", instanceID, (ushort)0, ushort.MaxValue));
		}
	}

	internal static int GetEventID(long instanceID)
	{
		long num = ((instanceID >= 0) ? instanceID : (-instanceID));
		int num2 = (int)(num & 0x3FFFFFFF);
		return (instanceID >= 0) ? num2 : (-num2);
	}
}
