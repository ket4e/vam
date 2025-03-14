using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.XR.WSA;

/// <summary>
///   <para>The Holographic Settings contain functions which effect the performance and presentation of Holograms on Windows Holographic platforms.</para>
/// </summary>
[NativeHeader("Runtime/VR/HoloLens/HolographicSettings.h")]
[StaticAccessor("HolographicSettings::GetInstance()", StaticAccessorType.Dot)]
public class HolographicSettings
{
	/// <summary>
	///   <para>Represents the kind of reprojection an app is requesting to stabilize its holographic rendering relative to the user's head motion.</para>
	/// </summary>
	public enum HolographicReprojectionMode
	{
		/// <summary>
		///   <para>The image should be stabilized for changes to both the user's head position and orientation. This is best for world-locked content that should remain physically stationary as the user walks around.</para>
		/// </summary>
		PositionAndOrientation,
		/// <summary>
		///   <para>The image should be stabilized only for changes to the user's head orientation, ignoring positional changes. This is best for body-locked content that should tag along with the user as they walk around, such as 360-degree video.</para>
		/// </summary>
		OrientationOnly,
		/// <summary>
		///   <para>The image should not be stabilized for the user's head motion, instead remaining fixed in the display. This is generally discouraged, as it is only comfortable for users when used sparingly, such as when the only visible content is a small cursor.</para>
		/// </summary>
		Disabled
	}

	/// <summary>
	///   <para>Whether the app is displaying protected content.</para>
	/// </summary>
	[NativeConditional("ENABLE_HOLOLENS_MODULE")]
	public static extern bool IsContentProtectionEnabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The kind of reprojection the app is requesting to stabilize its holographic rendering relative to the user's head motion.</para>
	/// </summary>
	public static HolographicReprojectionMode ReprojectionMode
	{
		get
		{
			return HolographicReprojectionMode.Disabled;
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Returns true if Holographic rendering is currently running with Latent Frame Presentation.  Default value is false.</para>
	/// </summary>
	[Obsolete("Support for toggling latent frame presentation has been removed, and IsLatentFramePresentation will always return true", false)]
	public static bool IsLatentFramePresentation => true;

	/// <summary>
	///   <para>Option to allow developers to achieve higher framerate at the cost of high latency.  By default this option is off.</para>
	/// </summary>
	/// <param name="activated">True to enable or false to disable Low Latent Frame Presentation.</param>
	[Obsolete("Support for toggling latent frame presentation has been removed", true)]
	public static void ActivateLatentFramePresentation(bool activated)
	{
	}
}
