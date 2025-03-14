using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>A handler that lets you read or write a HumanPose from or to a humanoid avatar skeleton hierarchy.</para>
/// </summary>
[NativeHeader("Runtime/Animation/ScriptBindings/Animation.bindings.h")]
[NativeHeader("Runtime/Animation/HumanPoseHandler.h")]
public class HumanPoseHandler : IDisposable
{
	internal IntPtr m_Ptr;

	/// <summary>
	///   <para>Creates a human pose handler from an avatar and a root transform.</para>
	/// </summary>
	/// <param name="avatar">The avatar that defines the humanoid rig on skeleton hierarchy with root as the top most parent.</param>
	/// <param name="root">The top most node of the skeleton hierarchy defined in humanoid avatar.</param>
	public HumanPoseHandler(Avatar avatar, Transform root)
	{
		m_Ptr = IntPtr.Zero;
		if (root == null)
		{
			throw new ArgumentNullException("HumanPoseHandler root Transform is null");
		}
		if (avatar == null)
		{
			throw new ArgumentNullException("HumanPoseHandler avatar is null");
		}
		if (!avatar.isValid)
		{
			throw new ArgumentException("HumanPoseHandler avatar is invalid");
		}
		if (!avatar.isHuman)
		{
			throw new ArgumentException("HumanPoseHandler avatar is not human");
		}
		m_Ptr = Internal_Create(avatar, root);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationBindings::CreateHumanPoseHandler")]
	private static extern IntPtr Internal_Create(Avatar avatar, Transform root);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationBindings::DestroyHumanPoseHandler")]
	private static extern void Internal_Destroy(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetHumanPose(out Vector3 bodyPosition, out Quaternion bodyRotation, [Out] float[] muscles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetHumanPose(ref Vector3 bodyPosition, ref Quaternion bodyRotation, float[] muscles);

	public void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	public void GetHumanPose(ref HumanPose humanPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		humanPose.Init();
		GetHumanPose(out humanPose.bodyPosition, out humanPose.bodyRotation, humanPose.muscles);
	}

	public void SetHumanPose(ref HumanPose humanPose)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new NullReferenceException("HumanPoseHandler is not initialized properly");
		}
		humanPose.Init();
		SetHumanPose(ref humanPose.bodyPosition, ref humanPose.bodyRotation, humanPose.muscles);
	}
}
