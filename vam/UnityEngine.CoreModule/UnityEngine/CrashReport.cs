using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Holds data for a single application crash event and provides access to all gathered crash reports.</para>
/// </summary>
public sealed class CrashReport
{
	private static List<CrashReport> internalReports;

	private static object reportsLock = new object();

	private readonly string id;

	/// <summary>
	///   <para>Time, when the crash occured.</para>
	/// </summary>
	public readonly DateTime time;

	/// <summary>
	///   <para>Crash report data as formatted text.</para>
	/// </summary>
	public readonly string text;

	/// <summary>
	///   <para>Returns all currently available reports in a new array.</para>
	/// </summary>
	public static CrashReport[] reports
	{
		get
		{
			PopulateReports();
			lock (reportsLock)
			{
				return internalReports.ToArray();
			}
		}
	}

	/// <summary>
	///   <para>Returns last crash report, or null if no reports are available.</para>
	/// </summary>
	public static CrashReport lastReport
	{
		get
		{
			PopulateReports();
			lock (reportsLock)
			{
				if (internalReports.Count > 0)
				{
					return internalReports[internalReports.Count - 1];
				}
			}
			return null;
		}
	}

	private CrashReport(string id, DateTime time, string text)
	{
		this.id = id;
		this.time = time;
		this.text = text;
	}

	private static int Compare(CrashReport c1, CrashReport c2)
	{
		long ticks = c1.time.Ticks;
		long ticks2 = c2.time.Ticks;
		if (ticks > ticks2)
		{
			return 1;
		}
		if (ticks < ticks2)
		{
			return -1;
		}
		return 0;
	}

	private static void PopulateReports()
	{
		lock (reportsLock)
		{
			if (internalReports == null)
			{
				string[] array = GetReports();
				internalReports = new List<CrashReport>(array.Length);
				string[] array2 = array;
				foreach (string text in array2)
				{
					double secondsSinceUnixEpoch;
					string reportData = GetReportData(text, out secondsSinceUnixEpoch);
					DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(secondsSinceUnixEpoch);
					internalReports.Add(new CrashReport(text, dateTime, reportData));
				}
				internalReports.Sort(Compare);
			}
		}
	}

	/// <summary>
	///   <para>Remove all reports from available reports list.</para>
	/// </summary>
	public static void RemoveAll()
	{
		CrashReport[] array = reports;
		foreach (CrashReport crashReport in array)
		{
			crashReport.Remove();
		}
	}

	/// <summary>
	///   <para>Remove report from available reports list.</para>
	/// </summary>
	public void Remove()
	{
		if (RemoveReport(id))
		{
			lock (reportsLock)
			{
				internalReports.Remove(this);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private static extern string[] GetReports();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private static extern string GetReportData(string id, out double secondsSinceUnixEpoch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private static extern bool RemoveReport(string id);
}
