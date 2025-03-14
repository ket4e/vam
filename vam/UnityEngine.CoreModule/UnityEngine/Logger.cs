using System;

namespace UnityEngine;

/// <summary>
///   <para>Initializes a new instance of the Logger.</para>
/// </summary>
public class Logger : ILogger, ILogHandler
{
	private const string kNoTagFormat = "{0}";

	private const string kTagFormat = "{0}: {1}";

	/// <summary>
	///   <para>Set  Logger.ILogHandler.</para>
	/// </summary>
	public ILogHandler logHandler { get; set; }

	/// <summary>
	///   <para>To runtime toggle debug logging [ON/OFF].</para>
	/// </summary>
	public bool logEnabled { get; set; }

	/// <summary>
	///   <para>To selective enable debug log message.</para>
	/// </summary>
	public LogType filterLogType { get; set; }

	private Logger()
	{
	}

	/// <summary>
	///   <para>Create a custom Logger.</para>
	/// </summary>
	/// <param name="logHandler">Pass in default log handler or custom log handler.</param>
	public Logger(ILogHandler logHandler)
	{
		this.logHandler = logHandler;
		logEnabled = true;
		filterLogType = LogType.Log;
	}

	/// <summary>
	///   <para>Check logging is enabled based on the LogType.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <returns>
	///   <para>Retrun true in case logs of LogType will be logged otherwise returns false.</para>
	/// </returns>
	public bool IsLogTypeAllowed(LogType logType)
	{
		if (logEnabled)
		{
			if (logType == LogType.Exception)
			{
				return true;
			}
			if (filterLogType != LogType.Exception)
			{
				return logType <= filterLogType;
			}
		}
		return false;
	}

	private static string GetString(object message)
	{
		return (message == null) ? "Null" : message.ToString();
	}

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void Log(LogType logType, object message)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, null, "{0}", GetString(message));
		}
	}

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void Log(LogType logType, object message, Object context)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, context, "{0}", GetString(message));
		}
	}

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void Log(LogType logType, string tag, object message)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, null, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void Log(LogType logType, string tag, object message, Object context)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, context, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void Log(object message)
	{
		if (IsLogTypeAllowed(LogType.Log))
		{
			logHandler.LogFormat(LogType.Log, null, "{0}", GetString(message));
		}
	}

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void Log(string tag, object message)
	{
		if (IsLogTypeAllowed(LogType.Log))
		{
			logHandler.LogFormat(LogType.Log, null, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void Log(string tag, object message, Object context)
	{
		if (IsLogTypeAllowed(LogType.Log))
		{
			logHandler.LogFormat(LogType.Log, context, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>A variant of Logger.Log that logs an warning message.</para>
	/// </summary>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void LogWarning(string tag, object message)
	{
		if (IsLogTypeAllowed(LogType.Warning))
		{
			logHandler.LogFormat(LogType.Warning, null, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>A variant of Logger.Log that logs an warning message.</para>
	/// </summary>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void LogWarning(string tag, object message, Object context)
	{
		if (IsLogTypeAllowed(LogType.Warning))
		{
			logHandler.LogFormat(LogType.Warning, context, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>A variant of Logger.Log that logs an error message.</para>
	/// </summary>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void LogError(string tag, object message)
	{
		if (IsLogTypeAllowed(LogType.Error))
		{
			logHandler.LogFormat(LogType.Error, null, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>A variant of Logger.Log that logs an error message.</para>
	/// </summary>
	/// <param name="tag">Used to identify the source of a log message. It usually identifies the class where the log call occurs.</param>
	/// <param name="message">String or object to be converted to string representation for display.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void LogError(string tag, object message, Object context)
	{
		if (IsLogTypeAllowed(LogType.Error))
		{
			logHandler.LogFormat(LogType.Error, context, "{0}: {1}", tag, GetString(message));
		}
	}

	/// <summary>
	///   <para>Logs a formatted message.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="context">Object to which the message applies.</param>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public void LogFormat(LogType logType, string format, params object[] args)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, null, format, args);
		}
	}

	/// <summary>
	///   <para>A variant of Logger.Log that logs an exception message.</para>
	/// </summary>
	/// <param name="exception">Runtime Exception.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void LogException(Exception exception)
	{
		if (logEnabled)
		{
			logHandler.LogException(exception, null);
		}
	}

	/// <summary>
	///   <para>Logs a formatted message.</para>
	/// </summary>
	/// <param name="logType">The type of the log message.</param>
	/// <param name="context">Object to which the message applies.</param>
	/// <param name="format">A composite format string.</param>
	/// <param name="args">Format arguments.</param>
	public void LogFormat(LogType logType, Object context, string format, params object[] args)
	{
		if (IsLogTypeAllowed(logType))
		{
			logHandler.LogFormat(logType, context, format, args);
		}
	}

	/// <summary>
	///   <para>A variant of Logger.Log that logs an exception message.</para>
	/// </summary>
	/// <param name="exception">Runtime Exception.</param>
	/// <param name="context">Object to which the message applies.</param>
	public void LogException(Exception exception, Object context)
	{
		if (logEnabled)
		{
			logHandler.LogException(exception, context);
		}
	}
}
