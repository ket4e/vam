using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling;

/// <summary>
///   <para>Custom CPU Profiler label used for profiling arbitrary code blocks.</para>
/// </summary>
[UsedByNativeCode]
public sealed class CustomSampler : Sampler
{
	internal static CustomSampler s_InvalidCustomSampler = new CustomSampler();

	internal CustomSampler()
	{
	}

	/// <summary>
	///   <para>Creates a new CustomSampler for profiling parts of your code.</para>
	/// </summary>
	/// <param name="name">Name of the Sampler.</param>
	/// <returns>
	///   <para>CustomSampler object or null if a built-in Sampler with the same name exists.</para>
	/// </returns>
	public static CustomSampler Create(string name)
	{
		CustomSampler customSampler = CreateInternal(name);
		return customSampler ?? s_InvalidCustomSampler;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private static extern CustomSampler CreateInternal(string name);

	/// <summary>
	///   <para>Begin profiling a piece of code with a custom label defined by this instance of CustomSampler.</para>
	/// </summary>
	/// <param name="targetObject"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[Conditional("ENABLE_PROFILER")]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	public extern void Begin();

	/// <summary>
	///   <para>Begin profiling a piece of code with a custom label defined by this instance of CustomSampler.</para>
	/// </summary>
	/// <param name="targetObject"></param>
	[Conditional("ENABLE_PROFILER")]
	public void Begin(Object targetObject)
	{
		BeginWithObject(targetObject);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	[GeneratedByOldBindingsGenerator]
	private extern void BeginWithObject(Object targetObject);

	/// <summary>
	///   <para>End profiling a piece of code with a custom label.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[Conditional("ENABLE_PROFILER")]
	[ThreadAndSerializationSafe]
	public extern void End();
}
