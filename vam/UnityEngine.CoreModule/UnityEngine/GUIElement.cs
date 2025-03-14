using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Base class for images &amp; text strings displayed in a GUI.</para>
/// </summary>
[RequireComponent(typeof(Transform))]
public class GUIElement : Behaviour
{
	/// <summary>
	///   <para>Is a point on screen inside the element?</para>
	/// </summary>
	/// <param name="screenPosition"></param>
	/// <param name="camera"></param>
	public bool HitTest(Vector3 screenPosition, [DefaultValue("null")] Camera camera)
	{
		return INTERNAL_CALL_HitTest(this, ref screenPosition, camera);
	}

	/// <summary>
	///   <para>Is a point on screen inside the element?</para>
	/// </summary>
	/// <param name="screenPosition"></param>
	/// <param name="camera"></param>
	[ExcludeFromDocs]
	public bool HitTest(Vector3 screenPosition)
	{
		Camera camera = null;
		return INTERNAL_CALL_HitTest(this, ref screenPosition, camera);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern bool INTERNAL_CALL_HitTest(GUIElement self, ref Vector3 screenPosition, Camera camera);

	/// <summary>
	///   <para>Returns bounding rectangle of GUIElement in screen coordinates.</para>
	/// </summary>
	/// <param name="camera"></param>
	public Rect GetScreenRect([DefaultValue("null")] Camera camera)
	{
		INTERNAL_CALL_GetScreenRect(this, camera, out var value);
		return value;
	}

	[ExcludeFromDocs]
	public Rect GetScreenRect()
	{
		Camera camera = null;
		INTERNAL_CALL_GetScreenRect(this, camera, out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetScreenRect(GUIElement self, Camera camera, out Rect value);
}
