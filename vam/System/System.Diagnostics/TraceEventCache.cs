using System.Collections;
using System.Threading;

namespace System.Diagnostics;

public class TraceEventCache
{
	private DateTime started;

	private CorrelationManager manager;

	private string callstack;

	private string thread;

	private int process;

	private long timestamp;

	public string Callstack => callstack;

	public DateTime DateTime => started;

	public Stack LogicalOperationStack => manager.LogicalOperationStack;

	public int ProcessId => process;

	public string ThreadId => thread;

	public long Timestamp => timestamp;

	public TraceEventCache()
	{
		started = DateTime.Now;
		manager = Trace.CorrelationManager;
		callstack = Environment.StackTrace;
		timestamp = Stopwatch.GetTimestamp();
		thread = Thread.CurrentThread.Name;
		process = Process.GetCurrentProcess().Id;
	}
}
