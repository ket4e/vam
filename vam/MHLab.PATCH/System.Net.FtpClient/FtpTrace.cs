using System.Collections.Generic;
using System.Diagnostics;

namespace System.Net.FtpClient;

public static class FtpTrace
{
	private static List<TraceListener> m_listeners = new List<TraceListener>();

	private static bool m_flushOnWrite = false;

	public static bool FlushOnWrite
	{
		get
		{
			return m_flushOnWrite;
		}
		set
		{
			m_flushOnWrite = value;
		}
	}

	public static void AddListener(TraceListener listener)
	{
		lock (m_listeners)
		{
			m_listeners.Add(listener);
		}
	}

	public static void RemoveListener(TraceListener listener)
	{
		lock (m_listeners)
		{
			m_listeners.Remove(listener);
		}
	}

	public static void Write(string message, params object[] args)
	{
		Write(string.Format(message, args));
	}

	public static void Write(string message)
	{
		TraceListener[] array;
		lock (m_listeners)
		{
			array = m_listeners.ToArray();
		}
		TraceListener[] array2 = array;
		foreach (TraceListener traceListener in array2)
		{
			traceListener.Write(message);
			if (m_flushOnWrite)
			{
				traceListener.Flush();
			}
		}
	}

	public static void WriteLine(string message, params object[] args)
	{
		Write($"{string.Format(message, args)}{Environment.NewLine}");
	}

	public static void WriteLine(string message)
	{
		Write($"{message}{Environment.NewLine}");
	}
}
