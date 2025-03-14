namespace UnityEngine;

/// <summary>
///   <para>Suspends the coroutine execution for the given amount of seconds using unscaled time.</para>
/// </summary>
public class WaitForSecondsRealtime : CustomYieldInstruction
{
	private float waitTime;

	public override bool keepWaiting => Time.realtimeSinceStartup < waitTime;

	/// <summary>
	///   <para>Creates a yield instruction to wait for a given number of seconds using unscaled time.</para>
	/// </summary>
	/// <param name="time"></param>
	public WaitForSecondsRealtime(float time)
	{
		waitTime = Time.realtimeSinceStartup + time;
	}
}
