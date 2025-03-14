namespace UnityEngine.Experimental.UIElements;

public struct TimerState
{
	public long start;

	public long now;

	public long deltaTime => now - start;
}
