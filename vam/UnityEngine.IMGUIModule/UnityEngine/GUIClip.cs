using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal sealed class GUIClip
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct ParentClipScope : IDisposable
	{
		private bool m_Disposed;

		public ParentClipScope(Matrix4x4 objectTransform, Rect clipRect)
		{
			m_Disposed = false;
			Internal_PushParentClip(objectTransform, clipRect);
		}

		public void Dispose()
		{
			if (!m_Disposed)
			{
				m_Disposed = true;
				Internal_PopParentClip();
			}
		}
	}

	public static extern bool enabled
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[GeneratedByOldBindingsGenerator]
		get;
	}

	public static Rect topmostRect
	{
		get
		{
			INTERNAL_get_topmostRect(out var value);
			return value;
		}
	}

	public static Rect visibleRect
	{
		get
		{
			INTERNAL_get_visibleRect(out var value);
			return value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void Internal_Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
	{
		INTERNAL_CALL_Internal_Push(ref screenRect, ref scrollOffset, ref renderOffset, resetOffset);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_Push(ref Rect screenRect, ref Vector2 scrollOffset, ref Vector2 renderOffset, bool resetOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Internal_Pop();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static extern int Internal_GetCount();

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static Rect GetTopRect()
	{
		INTERNAL_CALL_GetTopRect(out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetTopRect(out Rect value);

	private static void Unclip_Vector2(ref Vector2 pos)
	{
		INTERNAL_CALL_Unclip_Vector2(ref pos);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Unclip_Vector2(ref Vector2 pos);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_topmostRect(out Rect value);

	private static void Unclip_Rect(ref Rect rect)
	{
		INTERNAL_CALL_Unclip_Rect(ref rect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Unclip_Rect(ref Rect rect);

	private static void Clip_Vector2(ref Vector2 absolutePos)
	{
		INTERNAL_CALL_Clip_Vector2(ref absolutePos);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Clip_Vector2(ref Vector2 absolutePos);

	private static void Internal_Clip_Rect(ref Rect absoluteRect)
	{
		INTERNAL_CALL_Internal_Clip_Rect(ref absoluteRect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_Clip_Rect(ref Rect absoluteRect);

	private static void UnclipToWindow_Vector2(ref Vector2 pos)
	{
		INTERNAL_CALL_UnclipToWindow_Vector2(ref pos);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UnclipToWindow_Vector2(ref Vector2 pos);

	private static void UnclipToWindow_Rect(ref Rect rect)
	{
		INTERNAL_CALL_UnclipToWindow_Rect(ref rect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_UnclipToWindow_Rect(ref Rect rect);

	private static void ClipToWindow_Vector2(ref Vector2 absolutePos)
	{
		INTERNAL_CALL_ClipToWindow_Vector2(ref absolutePos);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ClipToWindow_Vector2(ref Vector2 absolutePos);

	private static void ClipToWindow_Rect(ref Rect absoluteRect)
	{
		INTERNAL_CALL_ClipToWindow_Rect(ref absoluteRect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_ClipToWindow_Rect(ref Rect absoluteRect);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[GeneratedByOldBindingsGenerator]
	internal static extern void Reapply();

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static Matrix4x4 GetMatrix()
	{
		INTERNAL_CALL_GetMatrix(out var value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_GetMatrix(out Matrix4x4 value);

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void SetMatrix(Matrix4x4 m)
	{
		INTERNAL_CALL_SetMatrix(ref m);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_SetMatrix(ref Matrix4x4 m);

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static void Internal_PushParentClip(Matrix4x4 objectTransform, Rect clipRect)
	{
		INTERNAL_CALL_Internal_PushParentClip(ref objectTransform, ref clipRect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_CALL_Internal_PushParentClip(ref Matrix4x4 objectTransform, ref Rect clipRect);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static extern void Internal_PopParentClip();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void INTERNAL_get_visibleRect(out Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[GeneratedByOldBindingsGenerator]
	private static extern void Internal_GetAbsoluteMousePosition(out Vector2 output);

	internal static void Push(Rect screenRect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
	{
		Internal_Push(screenRect, scrollOffset, renderOffset, resetOffset);
	}

	internal static void Pop()
	{
		Internal_Pop();
	}

	public static Vector2 Unclip(Vector2 pos)
	{
		Unclip_Vector2(ref pos);
		return pos;
	}

	public static Rect Unclip(Rect rect)
	{
		Unclip_Rect(ref rect);
		return rect;
	}

	public static Vector2 Clip(Vector2 absolutePos)
	{
		Clip_Vector2(ref absolutePos);
		return absolutePos;
	}

	public static Rect Clip(Rect absoluteRect)
	{
		Internal_Clip_Rect(ref absoluteRect);
		return absoluteRect;
	}

	public static Vector2 UnclipToWindow(Vector2 pos)
	{
		UnclipToWindow_Vector2(ref pos);
		return pos;
	}

	public static Rect UnclipToWindow(Rect rect)
	{
		UnclipToWindow_Rect(ref rect);
		return rect;
	}

	public static Vector2 ClipToWindow(Vector2 absolutePos)
	{
		ClipToWindow_Vector2(ref absolutePos);
		return absolutePos;
	}

	public static Rect ClipToWindow(Rect absoluteRect)
	{
		ClipToWindow_Rect(ref absoluteRect);
		return absoluteRect;
	}

	public static Vector2 GetAbsoluteMousePosition()
	{
		Internal_GetAbsoluteMousePosition(out var output);
		return output;
	}
}
