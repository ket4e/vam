using System.ComponentModel;

namespace System.Diagnostics;

[Designer("System.Diagnostics.Design.ProcessThreadDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public class ProcessThread : Component
{
	[System.MonoTODO]
	[MonitoringDescription("The base priority of this thread.")]
	public int BasePriority => 0;

	[System.MonoTODO]
	[MonitoringDescription("The current priority of this thread.")]
	public int CurrentPriority => 0;

	[System.MonoTODO]
	[MonitoringDescription("The ID of this thread.")]
	public int Id => 0;

	[Browsable(false)]
	[System.MonoTODO]
	public int IdealProcessor
	{
		set
		{
		}
	}

	[System.MonoTODO]
	[MonitoringDescription("Thread gets a priority boot when interactively used by a user.")]
	public bool PriorityBoostEnabled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[System.MonoTODO]
	[MonitoringDescription("The priority level of this thread.")]
	public ThreadPriorityLevel PriorityLevel
	{
		get
		{
			return ThreadPriorityLevel.Idle;
		}
		set
		{
		}
	}

	[System.MonoTODO]
	[MonitoringDescription("The amount of CPU time used in privileged mode.")]
	public TimeSpan PrivilegedProcessorTime => new TimeSpan(0L);

	[System.MonoTODO]
	[Browsable(false)]
	public IntPtr ProcessorAffinity
	{
		set
		{
		}
	}

	[MonitoringDescription("The start address in memory of this thread.")]
	[System.MonoTODO]
	public IntPtr StartAddress => (IntPtr)0;

	[MonitoringDescription("The time this thread was started.")]
	[System.MonoTODO]
	public DateTime StartTime => new DateTime(0L);

	[MonitoringDescription("The current state of this thread.")]
	[System.MonoTODO]
	public ThreadState ThreadState => ThreadState.Initialized;

	[MonitoringDescription("The total amount of CPU time used.")]
	[System.MonoTODO]
	public TimeSpan TotalProcessorTime => new TimeSpan(0L);

	[System.MonoTODO]
	[MonitoringDescription("The amount of CPU time used in user mode.")]
	public TimeSpan UserProcessorTime => new TimeSpan(0L);

	[MonitoringDescription("The reason why this thread is waiting.")]
	[System.MonoTODO]
	public ThreadWaitReason WaitReason => ThreadWaitReason.Executive;

	[System.MonoTODO("Parse parameters")]
	internal ProcessThread()
	{
	}

	[System.MonoTODO]
	public void ResetIdealProcessor()
	{
	}
}
