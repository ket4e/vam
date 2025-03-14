using System;
using System.IO;
using MHLab.PATCH.Settings;

namespace MHLab.PATCH.Debugging;

public sealed class Debugger
{
	private static StreamWriter _writer = null;

	private static object locker = new object();

	public static void Clean()
	{
		lock (locker)
		{
			try
			{
				if (_writer != null)
				{
					_writer.Close();
					_writer = null;
				}
				if (File.Exists(SettingsManager.LOGS_ERROR_PATH))
				{
					File.Delete(SettingsManager.LOGS_ERROR_PATH);
				}
			}
			catch
			{
			}
		}
	}

	public static void Log(string message)
	{
		lock (locker)
		{
			try
			{
				if (_writer == null)
				{
					CheckLogsDirectory();
					_writer = new StreamWriter(SettingsManager.LOGS_ERROR_PATH, append: true);
				}
				string text = "[MVR Patcher - " + DateTime.Now.ToString() + "] " + message;
				if (_writer != null)
				{
					_writer.Write(text + "\r\n");
					_writer.Flush();
				}
			}
			catch
			{
			}
		}
	}

	public static void Close()
	{
		lock (locker)
		{
			try
			{
				if (_writer != null)
				{
					_writer.Close();
					_writer = null;
				}
			}
			catch
			{
			}
		}
	}

	private static bool CheckLogsDirectory()
	{
		try
		{
			if (!FileManager.DirectoryExists(SettingsManager.LOGS_ERROR_PATH))
			{
				FileManager.CreateDirectory(Path.GetDirectoryName(SettingsManager.LOGS_ERROR_PATH));
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
