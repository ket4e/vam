namespace UnityEngine.Experimental.Rendering;

/// <summary>
///   <para>Describes a material render queue range.</para>
/// </summary>
public struct RenderQueueRange
{
	/// <summary>
	///   <para>Inclusive lower bound for the range.</para>
	/// </summary>
	public int min;

	/// <summary>
	///   <para>Inclusive upper bound for the range.</para>
	/// </summary>
	public int max;

	/// <summary>
	///   <para>A range that includes all objects.</para>
	/// </summary>
	public static RenderQueueRange all
	{
		get
		{
			RenderQueueRange result = default(RenderQueueRange);
			result.min = 0;
			result.max = 5000;
			return result;
		}
	}

	/// <summary>
	///   <para>A range that includes only opaque objects.</para>
	/// </summary>
	public static RenderQueueRange opaque
	{
		get
		{
			RenderQueueRange result = default(RenderQueueRange);
			result.min = 0;
			result.max = 2500;
			return result;
		}
	}

	/// <summary>
	///   <para>A range that includes only transparent objects.</para>
	/// </summary>
	public static RenderQueueRange transparent
	{
		get
		{
			RenderQueueRange result = default(RenderQueueRange);
			result.min = 2501;
			result.max = 5000;
			return result;
		}
	}
}
