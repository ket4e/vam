using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine;

/// <summary>
///   <para>AvatarMask is used to mask out humanoid body parts and transforms.</para>
/// </summary>
[UsedByNativeCode]
[NativeHeader("Runtime/Animation/AvatarMask.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
[MovedFrom("UnityEditor.Animations", true)]
public sealed class AvatarMask : Object
{
	/// <summary>
	///   <para>The number of humanoid body parts.</para>
	/// </summary>
	[Obsolete("AvatarMask.humanoidBodyPartCount is deprecated, use AvatarMaskBodyPart.LastBodyPart instead.")]
	public int humanoidBodyPartCount => 13;

	/// <summary>
	///   <para>Number of transforms.</para>
	/// </summary>
	public extern int transformCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	internal extern bool hasFeetIK
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Creates a new AvatarMask.</para>
	/// </summary>
	public AvatarMask()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationBindings::CreateAvatarMask")]
	private static extern void Internal_Create([Writable] AvatarMask self);

	/// <summary>
	///   <para>Returns true if the humanoid body part at the given index is active.</para>
	/// </summary>
	/// <param name="index">The index of the humanoid body part.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetBodyPart")]
	public extern bool GetHumanoidBodyPartActive(AvatarMaskBodyPart index);

	/// <summary>
	///   <para>Sets the humanoid body part at the given index to active or not.</para>
	/// </summary>
	/// <param name="index">The index of the humanoid body part.</param>
	/// <param name="value">Active or not.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetBodyPart")]
	public extern void SetHumanoidBodyPartActive(AvatarMaskBodyPart index, bool value);

	public void AddTransformPath(Transform transform)
	{
		AddTransformPath(transform, recursive: true);
	}

	/// <summary>
	///   <para>Adds a transform path into the AvatarMask.</para>
	/// </summary>
	/// <param name="transform">The transform to add into the AvatarMask.</param>
	/// <param name="recursive">Whether to also add all children of the specified transform.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void AddTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive);

	public void RemoveTransformPath(Transform transform)
	{
		RemoveTransformPath(transform, recursive: true);
	}

	/// <summary>
	///   <para>Removes a transform path from the AvatarMask.</para>
	/// </summary>
	/// <param name="transform">The Transform that should be removed from the AvatarMask.</param>
	/// <param name="recursive">Whether to also remove all children of the specified transform.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void RemoveTransformPath([NotNull] Transform transform, [DefaultValue("true")] bool recursive);

	/// <summary>
	///   <para>Returns the path of the transform at the given index.</para>
	/// </summary>
	/// <param name="index">The index of the transform.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern string GetTransformPath(int index);

	/// <summary>
	///   <para>Sets the path of the transform at the given index.</para>
	/// </summary>
	/// <param name="index">The index of the transform.</param>
	/// <param name="path">The path of the transform.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetTransformPath(int index, string path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetTransformWeight(int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTransformWeight(int index, float weight);

	/// <summary>
	///   <para>Returns true if the transform at the given index is active.</para>
	/// </summary>
	/// <param name="index">The index of the transform.</param>
	public bool GetTransformActive(int index)
	{
		return GetTransformWeight(index) > 0.5f;
	}

	/// <summary>
	///   <para>Sets the tranform at the given index to active or not.</para>
	/// </summary>
	/// <param name="index">The index of the transform.</param>
	/// <param name="value">Active or not.</param>
	public void SetTransformActive(int index, bool value)
	{
		SetTransformWeight(index, (!value) ? 0f : 1f);
	}

	internal void Copy(AvatarMask other)
	{
		for (AvatarMaskBodyPart avatarMaskBodyPart = AvatarMaskBodyPart.Root; avatarMaskBodyPart < AvatarMaskBodyPart.LastBodyPart; avatarMaskBodyPart++)
		{
			SetHumanoidBodyPartActive(avatarMaskBodyPart, other.GetHumanoidBodyPartActive(avatarMaskBodyPart));
		}
		transformCount = other.transformCount;
		for (int i = 0; i < other.transformCount; i++)
		{
			SetTransformPath(i, other.GetTransformPath(i));
			SetTransformActive(i, other.GetTransformActive(i));
		}
	}
}
