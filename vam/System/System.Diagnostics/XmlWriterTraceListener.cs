using System.IO;
using System.Threading;
using System.Xml;

namespace System.Diagnostics;

public class XmlWriterTraceListener : TextWriterTraceListener
{
	private static readonly string e2e_ns = "http://schemas.microsoft.com/2004/06/E2ETraceEvent";

	private static readonly string sys_ns = "http://schemas.microsoft.com/2004/06/windows/eventlog/system";

	private static readonly string default_name = "XmlWriter";

	private XmlWriter w;

	public XmlWriterTraceListener(string filename)
		: this(filename, default_name)
	{
	}

	public XmlWriterTraceListener(string filename, string name)
		: this(new StreamWriter(new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)), name)
	{
	}

	public XmlWriterTraceListener(Stream stream)
		: this(stream, default_name)
	{
	}

	public XmlWriterTraceListener(Stream writer, string name)
		: this(new StreamWriter(writer), name)
	{
	}

	public XmlWriterTraceListener(TextWriter writer)
		: this(writer, default_name)
	{
	}

	public XmlWriterTraceListener(TextWriter writer, string name)
		: base(name)
	{
		w = XmlWriter.Create(writer, new XmlWriterSettings
		{
			OmitXmlDeclaration = true
		});
	}

	public override void Close()
	{
		w.Close();
	}

	public override void Fail(string message, string detailMessage)
	{
		TraceEvent(null, null, TraceEventType.Error, 0, message + " " + detailMessage);
	}

	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
	{
		TraceCore(eventCache, source, eventType, id, false, Guid.Empty, 2, true, data);
	}

	[System.MonoLimitation("level is not always correct")]
	public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
	{
		TraceCore(eventCache, source, eventType, id, hasRelatedActivity: false, Guid.Empty, 2, wrapData: true, data);
	}

	[System.MonoLimitation("level is not always correct")]
	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
	{
		TraceCore(eventCache, source, TraceEventType.Transfer, id, false, Guid.Empty, 2, true, message);
	}

	[System.MonoLimitation("level is not always correct")]
	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
	{
		TraceCore(eventCache, source, TraceEventType.Transfer, id, false, Guid.Empty, 2, true, string.Format(format, args));
	}

	public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
	{
		TraceCore(eventCache, source, TraceEventType.Transfer, id, true, relatedActivityId, 255, true, message);
	}

	public override void Write(string message)
	{
		WriteLine(message);
	}

	[System.MonoLimitation("level is not always correct")]
	public override void WriteLine(string message)
	{
		TraceCore(null, "Trace", TraceEventType.Information, 0, false, Guid.Empty, 8, false, message);
	}

	private void TraceCore(TraceEventCache eventCache, string source, TraceEventType eventType, int id, bool hasRelatedActivity, Guid relatedActivity, int level, bool wrapData, params object[] data)
	{
		Process process = ((eventCache == null) ? Process.GetCurrentProcess() : Process.GetProcessById(eventCache.ProcessId));
		w.WriteStartElement("E2ETraceEvent", e2e_ns);
		w.WriteStartElement("System", sys_ns);
		w.WriteStartElement("EventID", sys_ns);
		w.WriteString(XmlConvert.ToString(id));
		w.WriteEndElement();
		w.WriteStartElement("Type", sys_ns);
		w.WriteString("3");
		w.WriteEndElement();
		w.WriteStartElement("SubType", sys_ns);
		w.WriteAttributeString("Name", eventType.ToString());
		w.WriteString("0");
		w.WriteEndElement();
		w.WriteStartElement("Level", sys_ns);
		w.WriteString(level.ToString());
		w.WriteEndElement();
		w.WriteStartElement("TimeCreated", sys_ns);
		w.WriteAttributeString("SystemTime", XmlConvert.ToString(eventCache?.DateTime ?? DateTime.Now));
		w.WriteEndElement();
		w.WriteStartElement("Source", sys_ns);
		w.WriteAttributeString("Name", source);
		w.WriteEndElement();
		w.WriteStartElement("Correlation", sys_ns);
		w.WriteAttributeString("ActivityID", string.Concat("{", Guid.Empty, "}"));
		w.WriteEndElement();
		w.WriteStartElement("Execution", sys_ns);
		w.WriteAttributeString("ProcessName", process.MainModule.ModuleName);
		w.WriteAttributeString("ProcessID", process.Id.ToString());
		w.WriteAttributeString("ThreadID", (eventCache == null) ? Thread.CurrentThread.ManagedThreadId.ToString() : eventCache.ThreadId);
		w.WriteEndElement();
		w.WriteStartElement("Channel", sys_ns);
		w.WriteEndElement();
		w.WriteStartElement("Computer");
		w.WriteString(process.MachineName);
		w.WriteEndElement();
		w.WriteEndElement();
		w.WriteStartElement("ApplicationData", e2e_ns);
		foreach (object obj in data)
		{
			if (wrapData)
			{
				w.WriteStartElement("TraceData", e2e_ns);
			}
			if (obj != null)
			{
				w.WriteString(obj.ToString());
			}
			if (wrapData)
			{
				w.WriteEndElement();
			}
		}
		w.WriteEndElement();
		w.WriteEndElement();
	}
}
