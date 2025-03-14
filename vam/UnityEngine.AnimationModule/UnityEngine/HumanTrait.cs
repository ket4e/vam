using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

/// <summary>
///   <para>Details of all the human bone and muscle types defined by Mecanim.</para>
/// </summary>
[NativeHeader("Runtime/Animation/HumanTrait.h")]
public class HumanTrait
{
	/// <summary>
	///   <para>The number of human muscle types defined by Mecanim.</para>
	/// </summary>
	public static extern int MuscleCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Array of the names of all human muscle types defined by Mecanim.</para>
	/// </summary>
	public static extern string[] MuscleName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The number of human bone types defined by Mecanim.</para>
	/// </summary>
	public static extern int BoneCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Array of the names of all human bone types defined by Mecanim.</para>
	/// </summary>
	public static extern string[] BoneName
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("MonoBoneName")]
		get;
	}

	/// <summary>
	///   <para>The number of bone types that are required by Mecanim for any human model.</para>
	/// </summary>
	public static extern int RequiredBoneCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("RequiredBoneCount")]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetBoneIndexFromMono(int humanId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetBoneIndexToMono(int boneIndex);

	/// <summary>
	///   <para>Obtain the muscle index for a particular bone index and "degree of freedom".</para>
	/// </summary>
	/// <param name="i">Bone index.</param>
	/// <param name="dofIndex">Number representing a "degree of freedom": 0 for X-Axis, 1 for Y-Axis, 2 for Z-Axis.</param>
	public static int MuscleFromBone(int i, int dofIndex)
	{
		return Internal_MuscleFromBone(GetBoneIndexFromMono(i), dofIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("MuscleFromBone")]
	private static extern int Internal_MuscleFromBone(int i, int dofIndex);

	/// <summary>
	///   <para>Return the bone to which a particular muscle is connected.</para>
	/// </summary>
	/// <param name="i">Muscle index.</param>
	public static int BoneFromMuscle(int i)
	{
		return GetBoneIndexToMono(Internal_BoneFromMuscle(i));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("BoneFromMuscle")]
	private static extern int Internal_BoneFromMuscle(int i);

	/// <summary>
	///   <para>Is the bone a member of the minimal set of bones that Mecanim requires for a human model?</para>
	/// </summary>
	/// <param name="i">Index of the bone to test.</param>
	public static bool RequiredBone(int i)
	{
		return Internal_RequiredBone(GetBoneIndexFromMono(i));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("RequiredBone")]
	private static extern bool Internal_RequiredBone(int i);

	internal static bool HasCollider(Avatar avatar, int i)
	{
		return Internal_HasCollider(avatar, GetBoneIndexFromMono(i));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("HasCollider")]
	private static extern bool Internal_HasCollider(Avatar avatar, int i);

	/// <summary>
	///   <para>Get the default minimum value of rotation for a muscle in degrees.</para>
	/// </summary>
	/// <param name="i">Muscle index.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern float GetMuscleDefaultMin(int i);

	/// <summary>
	///   <para>Get the default maximum value of rotation for a muscle in degrees.</para>
	/// </summary>
	/// <param name="i">Muscle index.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern float GetMuscleDefaultMax(int i);

	/// <summary>
	///   <para>Returns parent humanoid bone index of a bone.</para>
	/// </summary>
	/// <param name="i">Humanoid bone index to get parent from.</param>
	/// <returns>
	///   <para>Humanoid bone index of parent.</para>
	/// </returns>
	public static int GetParentBone(int i)
	{
		int num = Internal_GetParent(GetBoneIndexFromMono(i));
		return (num == -1) ? (-1) : GetBoneIndexToMono(num);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetParent")]
	private static extern int Internal_GetParent(int i);
}
