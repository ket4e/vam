using System;

namespace UnityEngine;

public interface ILogger : ILogHandler
{
	/// <summary>
	///   <para>Set Logger.ILogHandler.</para>
	/// </summary>
	ILogHandler logHandler { get; set; }

	/// <summary>
	///   <para>To runtime toggle debug logging [ON/OFF].</para>
	/// </summary>
	bool logEnabled { get; set; }

	/// <summary>
	///   <para>To selective enable debug log message.</para>
	/// </summary>
	LogType filterLogType { get; set; }

	/// <summary>
	///   <para>Check logging is enabled based on the LogType.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <returns>
	///   <para>Retrun true in case logs of LogType will be logged otherwise returns false.</para>
	/// </returns>
	bool IsLogTypeAllowed(LogType logType);

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="tag"></param>
	void Log(LogType logType, object message);

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="tag"></param>
	void Log(LogType logType, object message, Object context);

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="tag"></param>
	void Log(LogType logType, string tag, object message);

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="tag"></param>
	void Log(LogType logType, string tag, object message, Object context);

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="tag"></param>
	void Log(object message);

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="tag"></param>
	void Log(string tag, object message);

	/// <summary>
	///   <para>Logs message to the Unity Console using default logger.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="tag"></param>
	void Log(string tag, object message, Object context);

	/// <summary>
	///   <para>A variant of Logger.Log that logs an warning message.</para>
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	void LogWarning(string tag, object message);

	/// <summary>
	///   <para>A variant of Logger.Log that logs an warning message.</para>
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	void LogWarning(string tag, object message, Object context);

	/// <summary>
	///   <para>A variant of ILogger.Log that logs an error message.</para>
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	void LogError(string tag, object message);

	/// <summary>
	///   <para>A variant of ILogger.Log that logs an error message.</para>
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	void LogError(string tag, object message, Object context);

	/// <summary>
	///   <para>Logs a formatted message.</para>
	/// </summary>
	/// <param name="logType"></param>
	/// <param name="format"></param>
	/// <param name="args"></param>
	void LogFormat(LogType logType, string format, params object[] args);

	/// <summary>
	///   <para>A variant of ILogger.Log that logs an exception message.</para>
	/// </summary>
	/// <param name="exception"></param>
	void LogException(Exception exception);
}
