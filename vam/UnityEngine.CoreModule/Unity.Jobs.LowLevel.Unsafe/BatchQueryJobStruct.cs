using System;
using System.Runtime.InteropServices;

namespace Unity.Jobs.LowLevel.Unsafe;

/// <summary>
///   <para>Struct used to schedule batch query jobs.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct BatchQueryJobStruct<T> where T : struct
{
	internal static IntPtr jobReflectionData;

	public static IntPtr Initialize()
	{
		if (jobReflectionData == IntPtr.Zero)
		{
			jobReflectionData = JobsUtility.CreateJobReflectionData(typeof(T), JobType.ParallelFor, IntPtr.Zero);
		}
		return jobReflectionData;
	}
}
