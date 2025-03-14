using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows;

/// <summary>
///   <para>Exposes useful information related to crash reporting on Windows platforms.</para>
/// </summary>
public static class CrashReporting
{
	/// <summary>
	///   <para>Returns the path to the crash report folder on Windows.</para>
	/// </summary>
	[ThreadAndSerializationSafe]
	public static extern string crashReportFolder
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}
}
