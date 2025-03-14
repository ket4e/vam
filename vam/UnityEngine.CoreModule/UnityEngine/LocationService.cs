using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Interface into location functionality.</para>
/// </summary>
public sealed class LocationService
{
	/// <summary>
	///   <para>Specifies whether location service is enabled in user settings.</para>
	/// </summary>
	public extern bool isEnabledByUser
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Returns location service status.</para>
	/// </summary>
	public extern LocationServiceStatus status
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Last measured device geographical location.</para>
	/// </summary>
	public extern LocationInfo lastData
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	/// <summary>
	///   <para>Starts location service updates.  Last location coordinates could be.</para>
	/// </summary>
	/// <param name="desiredAccuracyInMeters"></param>
	/// <param name="updateDistanceInMeters"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Start([DefaultValue("10f")] float desiredAccuracyInMeters, [DefaultValue("10f")] float updateDistanceInMeters);

	/// <summary>
	///   <para>Starts location service updates.  Last location coordinates could be.</para>
	/// </summary>
	/// <param name="desiredAccuracyInMeters"></param>
	/// <param name="updateDistanceInMeters"></param>
	[ExcludeFromDocs]
	public void Start(float desiredAccuracyInMeters)
	{
		float updateDistanceInMeters = 10f;
		Start(desiredAccuracyInMeters, updateDistanceInMeters);
	}

	[ExcludeFromDocs]
	public void Start()
	{
		float updateDistanceInMeters = 10f;
		float desiredAccuracyInMeters = 10f;
		Start(desiredAccuracyInMeters, updateDistanceInMeters);
	}

	/// <summary>
	///   <para>Stops location service updates. This could be useful for saving battery life.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	public extern void Stop();
}
