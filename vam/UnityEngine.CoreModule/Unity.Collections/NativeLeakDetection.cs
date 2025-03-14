namespace Unity.Collections;

/// <summary>
///   <para>Static class for native leak detection settings.</para>
/// </summary>
public static class NativeLeakDetection
{
	private static int s_NativeLeakDetectionMode;

	/// <summary>
	///   <para>Set whether native memory leak detection should be enabled or disabled.</para>
	/// </summary>
	public static NativeLeakDetectionMode Mode
	{
		get
		{
			return (NativeLeakDetectionMode)s_NativeLeakDetectionMode;
		}
		set
		{
			s_NativeLeakDetectionMode = (int)value;
		}
	}
}
