using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Playables;

/// <summary>
///   <para>An implementation of IPlayable that allows application of a Material shader to one or many texture inputs to produce a texture output.</para>
/// </summary>
[NativeHeader("Runtime/Export/Director/MaterialEffectPlayable.bindings.h")]
[NativeHeader("Runtime/Shaders/Director/MaterialEffectPlayable.h")]
[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[StaticAccessor("MaterialEffectPlayableBindings", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
public struct MaterialEffectPlayable : IPlayable, IEquatable<MaterialEffectPlayable>
{
	private PlayableHandle m_Handle;

	internal MaterialEffectPlayable(PlayableHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOfType<MaterialEffectPlayable>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an MaterialEffectPlayable.");
		}
		m_Handle = handle;
	}

	public static MaterialEffectPlayable Create(PlayableGraph graph, Material material, int pass = -1)
	{
		PlayableHandle handle = CreateHandle(graph, material, pass);
		return new MaterialEffectPlayable(handle);
	}

	private static PlayableHandle CreateHandle(PlayableGraph graph, Material material, int pass)
	{
		PlayableHandle handle = PlayableHandle.Null;
		if (!InternalCreateMaterialEffectPlayable(ref graph, material, pass, ref handle))
		{
			return PlayableHandle.Null;
		}
		return handle;
	}

	public PlayableHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator Playable(MaterialEffectPlayable playable)
	{
		return new Playable(playable.GetHandle());
	}

	public static explicit operator MaterialEffectPlayable(Playable playable)
	{
		return new MaterialEffectPlayable(playable.GetHandle());
	}

	public bool Equals(MaterialEffectPlayable other)
	{
		return GetHandle() == other.GetHandle();
	}

	public Material GetMaterial()
	{
		return GetMaterialInternal(ref m_Handle);
	}

	public void SetMaterial(Material value)
	{
		SetMaterialInternal(ref m_Handle, value);
	}

	public int GetPass()
	{
		return GetPassInternal(ref m_Handle);
	}

	public void SetPass(int value)
	{
		SetPassInternal(ref m_Handle, value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Material GetMaterialInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMaterialInternal(ref PlayableHandle hdl, Material material);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPassInternal(ref PlayableHandle hdl);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPassInternal(ref PlayableHandle hdl, int pass);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalCreateMaterialEffectPlayable(ref PlayableGraph graph, Material material, int pass, ref PlayableHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ValidateType(ref PlayableHandle hdl);
}
