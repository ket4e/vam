using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace Unity.Jobs.LowLevel.Unsafe;

/// <summary>
///   <para>Static class containing functionality to create, run and debug jobs.</para>
/// </summary>
[NativeType(Header = "Runtime/Jobs/ScriptBindings/JobsBindings.h")]
public static class JobsUtility
{
	/// <summary>
	///   <para>Struct containing job parameters for scheduling.</para>
	/// </summary>
	public struct JobScheduleParameters
	{
		/// <summary>
		///   <para>A JobHandle to any dependency this job would have.</para>
		/// </summary>
		public JobHandle Dependency;

		/// <summary>
		///   <para>ScheduleMode option.</para>
		/// </summary>
		public int ScheduleMode;

		/// <summary>
		///   <para>Pointer to the reflection data.</para>
		/// </summary>
		public IntPtr ReflectionData;

		/// <summary>
		///   <para>Pointer to the job data.</para>
		/// </summary>
		public IntPtr JobDataPtr;

		/// <summary>
		///   <para>Constructor.</para>
		/// </summary>
		/// <param name="i_jobData"></param>
		/// <param name="i_reflectionData"></param>
		/// <param name="i_dependency"></param>
		/// <param name="i_scheduleMode"></param>
		public unsafe JobScheduleParameters(void* i_jobData, IntPtr i_reflectionData, JobHandle i_dependency, ScheduleMode i_scheduleMode)
		{
			Dependency = i_dependency;
			JobDataPtr = (IntPtr)i_jobData;
			ReflectionData = i_reflectionData;
			ScheduleMode = (int)i_scheduleMode;
		}
	}

	/// <summary>
	///   <para>Maximum job thread count.</para>
	/// </summary>
	public const int MaxJobThreadCount = 128;

	/// <summary>
	///   <para>Size of a cache line.</para>
	/// </summary>
	public const int CacheLineSize = 64;

	/// <summary>
	///   <para>Enables and disables the job debugger at runtime. Note that currently the job debugger is only supported in the Editor. Thus this only has effect in the editor.</para>
	/// </summary>
	public static extern bool JobDebuggerEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		set;
	}

	/// <summary>
	///   <para>When disabled, forces jobs that have already been compiled with burst to run in mono instead. For example if you want to debug the C# jobs or just want to compare behaviour or performance.</para>
	/// </summary>
	public static extern bool JobCompilerEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		set;
	}

	public unsafe static void GetJobRange(ref JobRanges ranges, int jobIndex, out int beginIndex, out int endIndex)
	{
		int* ptr = (int*)(void*)ranges.StartEndIndex;
		beginIndex = ptr[jobIndex * 2];
		endIndex = ptr[jobIndex * 2 + 1];
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsFreeFunction = true, IsThreadSafe = true)]
	public static extern bool GetWorkStealingRange(ref JobRanges ranges, int jobIndex, out int beginIndex, out int endIndex);

	[FreeFunction("ScheduleManagedJob")]
	public static JobHandle Schedule(ref JobScheduleParameters parameters)
	{
		Schedule_Injected(ref parameters, out var ret);
		return ret;
	}

	[FreeFunction("ScheduleManagedJobParallelFor")]
	public static JobHandle ScheduleParallelFor(ref JobScheduleParameters parameters, int arrayLength, int innerloopBatchCount)
	{
		ScheduleParallelFor_Injected(ref parameters, arrayLength, innerloopBatchCount, out var ret);
		return ret;
	}

	[FreeFunction("ScheduleManagedJobParallelForDeferArraySize")]
	public unsafe static JobHandle ScheduleParallelForDeferArraySize(ref JobScheduleParameters parameters, int innerloopBatchCount, void* listData, void* listDataAtomicSafetyHandle)
	{
		ScheduleParallelForDeferArraySize_Injected(ref parameters, innerloopBatchCount, listData, listDataAtomicSafetyHandle, out var ret);
		return ret;
	}

	[FreeFunction("ScheduleManagedJobParallelForTransform")]
	public static JobHandle ScheduleParallelForTransform(ref JobScheduleParameters parameters, IntPtr transfromAccesssArray)
	{
		ScheduleParallelForTransform_Injected(ref parameters, transfromAccesssArray, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true, IsFreeFunction = true)]
	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	public unsafe static extern void PatchBufferMinMaxRanges(IntPtr bufferRangePatchData, void* jobdata, int startIndex, int rangeSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern IntPtr CreateJobReflectionData(Type wrapperJobType, Type userJobType, JobType jobType, object managedJobFunction0, object managedJobFunction1, object managedJobFunction2);

	public static IntPtr CreateJobReflectionData(Type type, JobType jobType, object managedJobFunction0, object managedJobFunction1 = null, object managedJobFunction2 = null)
	{
		return CreateJobReflectionData(type, type, jobType, managedJobFunction0, managedJobFunction1, managedJobFunction2);
	}

	public static IntPtr CreateJobReflectionData(Type wrapperJobType, Type userJobType, JobType jobType, object managedJobFunction0)
	{
		return CreateJobReflectionData(wrapperJobType, userJobType, jobType, managedJobFunction0, null, null);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Schedule_Injected(ref JobScheduleParameters parameters, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ScheduleParallelFor_Injected(ref JobScheduleParameters parameters, int arrayLength, int innerloopBatchCount, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void ScheduleParallelForDeferArraySize_Injected(ref JobScheduleParameters parameters, int innerloopBatchCount, void* listData, void* listDataAtomicSafetyHandle, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ScheduleParallelForTransform_Injected(ref JobScheduleParameters parameters, IntPtr transfromAccesssArray, out JobHandle ret);
}
