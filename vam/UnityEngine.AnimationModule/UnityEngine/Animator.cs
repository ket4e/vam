using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Interface to control the Mecanim animation system.</para>
/// </summary>
[NativeHeader("Runtime/Animation/Animator.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/Animator.bindings.h")]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorControllerParameter.bindings.h")]
[UsedByNativeCode]
public class Animator : Behaviour
{
	/// <summary>
	///   <para>Returns true if the current rig is optimizable with AnimatorUtility.OptimizeTransformHierarchy.</para>
	/// </summary>
	public extern bool isOptimizable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsOptimizable")]
		get;
	}

	/// <summary>
	///   <para>Returns true if the current rig is humanoid, false if it is generic.</para>
	/// </summary>
	public extern bool isHuman
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsHuman")]
		get;
	}

	/// <summary>
	///   <para>Returns true if the current rig has root motion.</para>
	/// </summary>
	public extern bool hasRootMotion
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("HasRootMotion")]
		get;
	}

	internal extern bool isRootPositionOrRotationControlledByCurves
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsRootTranslationOrRotationControllerByCurves")]
		get;
	}

	/// <summary>
	///   <para>Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).</para>
	/// </summary>
	public extern float humanScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Returns whether the animator is initialized successfully.</para>
	/// </summary>
	public extern bool isInitialized
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsInitialized")]
		get;
	}

	/// <summary>
	///   <para>Gets the avatar delta position for the last evaluated frame.</para>
	/// </summary>
	public Vector3 deltaPosition
	{
		get
		{
			get_deltaPosition_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Gets the avatar delta rotation for the last evaluated frame.</para>
	/// </summary>
	public Quaternion deltaRotation
	{
		get
		{
			get_deltaRotation_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Gets the avatar velocity  for the last evaluated frame.</para>
	/// </summary>
	public Vector3 velocity
	{
		get
		{
			get_velocity_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Gets the avatar angular velocity for the last evaluated frame.</para>
	/// </summary>
	public Vector3 angularVelocity
	{
		get
		{
			get_angularVelocity_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>The root position, the position of the game object.</para>
	/// </summary>
	public Vector3 rootPosition
	{
		[NativeMethod("GetAvatarPosition")]
		get
		{
			get_rootPosition_Injected(out var ret);
			return ret;
		}
		[NativeMethod("SetAvatarPosition")]
		set
		{
			set_rootPosition_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The root rotation, the rotation of the game object.</para>
	/// </summary>
	public Quaternion rootRotation
	{
		[NativeMethod("GetAvatarRotation")]
		get
		{
			get_rootRotation_Injected(out var ret);
			return ret;
		}
		[NativeMethod("SetAvatarRotation")]
		set
		{
			set_rootRotation_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Should root motion be applied?</para>
	/// </summary>
	public extern bool applyRootMotion
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>When linearVelocityBlending is set to true, the root motion velocity and angular velocity will be blended linearly.</para>
	/// </summary>
	public extern bool linearVelocityBlending
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>When turned on, animations will be executed in the physics loop. This is only useful in conjunction with kinematic rigidbodies.</para>
	/// </summary>
	[Obsolete("Animator.animatePhysics has been deprecated. Use Animator.updateMode instead.")]
	public bool animatePhysics
	{
		get
		{
			return updateMode == AnimatorUpdateMode.AnimatePhysics;
		}
		set
		{
			updateMode = (value ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal);
		}
	}

	/// <summary>
	///   <para>Specifies the update mode of the Animator.</para>
	/// </summary>
	public extern AnimatorUpdateMode updateMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns true if the object has a transform hierarchy.</para>
	/// </summary>
	public extern bool hasTransformHierarchy
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	internal extern bool allowConstantClipSamplingOptimization
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The current gravity weight based on current animations that are played.</para>
	/// </summary>
	public extern float gravityWeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The position of the body center of mass.</para>
	/// </summary>
	public Vector3 bodyPosition
	{
		get
		{
			CheckIfInIKPass();
			return bodyPositionInternal;
		}
		set
		{
			CheckIfInIKPass();
			bodyPositionInternal = value;
		}
	}

	internal Vector3 bodyPositionInternal
	{
		[NativeMethod("GetBodyPosition")]
		get
		{
			get_bodyPositionInternal_Injected(out var ret);
			return ret;
		}
		[NativeMethod("SetBodyPosition")]
		set
		{
			set_bodyPositionInternal_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>The rotation of the body center of mass.</para>
	/// </summary>
	public Quaternion bodyRotation
	{
		get
		{
			CheckIfInIKPass();
			return bodyRotationInternal;
		}
		set
		{
			CheckIfInIKPass();
			bodyRotationInternal = value;
		}
	}

	internal Quaternion bodyRotationInternal
	{
		[NativeMethod("GetBodyRotation")]
		get
		{
			get_bodyRotationInternal_Injected(out var ret);
			return ret;
		}
		[NativeMethod("SetBodyRotation")]
		set
		{
			set_bodyRotationInternal_Injected(ref value);
		}
	}

	/// <summary>
	///   <para>Automatic stabilization of feet during transition and blending.</para>
	/// </summary>
	public extern bool stabilizeFeet
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns the number of layers in the controller.</para>
	/// </summary>
	public extern int layerCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The AnimatorControllerParameter list used by the animator. (Read Only)</para>
	/// </summary>
	public extern AnimatorControllerParameter[] parameters
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "AnimatorBindings::GetParameters", HasExplicitThis = true)]
		get;
	}

	/// <summary>
	///   <para>Returns the number of parameters in the controller.</para>
	/// </summary>
	public extern int parameterCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Blends pivot point between body center of mass and feet pivot. At 0%, the blending point is body center of mass. At 100%, the blending point is feet pivot.</para>
	/// </summary>
	public extern float feetPivotActive
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Gets the pivot weight.</para>
	/// </summary>
	public extern float pivotWeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Get the current position of the pivot.</para>
	/// </summary>
	public Vector3 pivotPosition
	{
		get
		{
			get_pivotPosition_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>If automatic matching is active.</para>
	/// </summary>
	public extern bool isMatchingTarget
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("IsMatchingTarget")]
		get;
	}

	/// <summary>
	///   <para>The playback speed of the Animator. 1 is normal playback speed.</para>
	/// </summary>
	public extern float speed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns the position of the target specified by SetTarget.</para>
	/// </summary>
	public Vector3 targetPosition
	{
		get
		{
			get_targetPosition_Injected(out var ret);
			return ret;
		}
	}

	/// <summary>
	///   <para>Returns the rotation of the target specified by SetTarget.</para>
	/// </summary>
	public Quaternion targetRotation
	{
		get
		{
			get_targetRotation_Injected(out var ret);
			return ret;
		}
	}

	internal extern Transform avatarRoot
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Controls culling of this Animator component.</para>
	/// </summary>
	public extern AnimatorCullingMode cullingMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Sets the playback position in the recording buffer.</para>
	/// </summary>
	public extern float playbackTime
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Start time of the first frame of the buffer relative to the frame at which StartRecording was called.</para>
	/// </summary>
	public float recorderStartTime
	{
		get
		{
			return GetRecorderStartTime();
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>End time of the recorded clip relative to when StartRecording was called.</para>
	/// </summary>
	public float recorderStopTime
	{
		get
		{
			return GetRecorderStopTime();
		}
		set
		{
		}
	}

	/// <summary>
	///   <para>Gets the mode of the Animator recorder.</para>
	/// </summary>
	public extern AnimatorRecorderMode recorderMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>The runtime representation of AnimatorController that controls the Animator.</para>
	/// </summary>
	public extern RuntimeAnimatorController runtimeAnimatorController
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns true if Animator has any playables assigned to it.</para>
	/// </summary>
	public extern bool hasBoundPlayables
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("HasBoundPlayables")]
		get;
	}

	/// <summary>
	///   <para>Gets/Sets the current Avatar.</para>
	/// </summary>
	public extern Avatar avatar
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>The PlayableGraph created by the Animator.</para>
	/// </summary>
	public PlayableGraph playableGraph
	{
		get
		{
			PlayableGraph graph = default(PlayableGraph);
			GetCurrentGraph(ref graph);
			return graph;
		}
	}

	/// <summary>
	///   <para>Additional layers affects the center of mass.</para>
	/// </summary>
	public extern bool layersAffectMassCenter
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Get left foot bottom height.</para>
	/// </summary>
	public extern float leftFeetBottomHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	/// <summary>
	///   <para>Get right foot bottom height.</para>
	/// </summary>
	public extern float rightFeetBottomHeight
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeConditional("UNITY_EDITOR")]
	internal extern bool supportsOnAnimatorMove
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("SupportsOnAnimatorMove")]
		get;
	}

	public extern bool logWarnings
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Sets whether the Animator sends events of type AnimationEvent.</para>
	/// </summary>
	public extern bool fireEvents
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Controls the behaviour of the Animator component when a GameObject is disabled.</para>
	/// </summary>
	public extern bool keepAnimatorControllerStateOnDisable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	/// <summary>
	///   <para>Returns the value of the given float parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>The value of the parameter.</para>
	/// </returns>
	public float GetFloat(string name)
	{
		return GetFloatString(name);
	}

	/// <summary>
	///   <para>Returns the value of the given float parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>The value of the parameter.</para>
	/// </returns>
	public float GetFloat(int id)
	{
		return GetFloatID(id);
	}

	/// <summary>
	///   <para>Send float values to the Animator to affect transitions.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	/// <param name="dampTime">The damper total time.</param>
	/// <param name="deltaTime">The delta time to give to the damper.</param>
	public void SetFloat(string name, float value)
	{
		SetFloatString(name, value);
	}

	/// <summary>
	///   <para>Send float values to the Animator to affect transitions.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	/// <param name="dampTime">The damper total time.</param>
	/// <param name="deltaTime">The delta time to give to the damper.</param>
	public void SetFloat(string name, float value, float dampTime, float deltaTime)
	{
		SetFloatStringDamp(name, value, dampTime, deltaTime);
	}

	/// <summary>
	///   <para>Send float values to the Animator to affect transitions.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	/// <param name="dampTime">The damper total time.</param>
	/// <param name="deltaTime">The delta time to give to the damper.</param>
	public void SetFloat(int id, float value)
	{
		SetFloatID(id, value);
	}

	/// <summary>
	///   <para>Send float values to the Animator to affect transitions.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	/// <param name="dampTime">The damper total time.</param>
	/// <param name="deltaTime">The delta time to give to the damper.</param>
	public void SetFloat(int id, float value, float dampTime, float deltaTime)
	{
		SetFloatIDDamp(id, value, dampTime, deltaTime);
	}

	/// <summary>
	///   <para>Returns the value of the given boolean parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>The value of the parameter.</para>
	/// </returns>
	public bool GetBool(string name)
	{
		return GetBoolString(name);
	}

	/// <summary>
	///   <para>Returns the value of the given boolean parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>The value of the parameter.</para>
	/// </returns>
	public bool GetBool(int id)
	{
		return GetBoolID(id);
	}

	/// <summary>
	///   <para>Sets the value of the given boolean parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	public void SetBool(string name, bool value)
	{
		SetBoolString(name, value);
	}

	/// <summary>
	///   <para>Sets the value of the given boolean parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	public void SetBool(int id, bool value)
	{
		SetBoolID(id, value);
	}

	/// <summary>
	///   <para>Returns the value of the given integer parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>The value of the parameter.</para>
	/// </returns>
	public int GetInteger(string name)
	{
		return GetIntegerString(name);
	}

	/// <summary>
	///   <para>Returns the value of the given integer parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>The value of the parameter.</para>
	/// </returns>
	public int GetInteger(int id)
	{
		return GetIntegerID(id);
	}

	/// <summary>
	///   <para>Sets the value of the given integer parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	public void SetInteger(string name, int value)
	{
		SetIntegerString(name, value);
	}

	/// <summary>
	///   <para>Sets the value of the given integer parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <param name="value">The new parameter value.</param>
	public void SetInteger(int id, int value)
	{
		SetIntegerID(id, value);
	}

	/// <summary>
	///   <para>Sets the value of the given trigger parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	public void SetTrigger(string name)
	{
		SetTriggerString(name);
	}

	/// <summary>
	///   <para>Sets the value of the given trigger parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	public void SetTrigger(int id)
	{
		SetTriggerID(id);
	}

	/// <summary>
	///   <para>Resets the value of the given trigger parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	public void ResetTrigger(string name)
	{
		ResetTriggerString(name);
	}

	/// <summary>
	///   <para>Resets the value of the given trigger parameter.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	public void ResetTrigger(int id)
	{
		ResetTriggerID(id);
	}

	/// <summary>
	///   <para>Returns true if the parameter is controlled by a curve, false otherwise.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>True if the parameter is controlled by a curve, false otherwise.</para>
	/// </returns>
	public bool IsParameterControlledByCurve(string name)
	{
		return IsParameterControlledByCurveString(name);
	}

	/// <summary>
	///   <para>Returns true if the parameter is controlled by a curve, false otherwise.</para>
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="id">The parameter ID.</param>
	/// <returns>
	///   <para>True if the parameter is controlled by a curve, false otherwise.</para>
	/// </returns>
	public bool IsParameterControlledByCurve(int id)
	{
		return IsParameterControlledByCurveID(id);
	}

	/// <summary>
	///   <para>Gets the position of an IK goal.</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is queried.</param>
	/// <returns>
	///   <para>Return the current position of this IK goal in world space.</para>
	/// </returns>
	public Vector3 GetIKPosition(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalPosition(goal);
	}

	private Vector3 GetGoalPosition(AvatarIKGoal goal)
	{
		GetGoalPosition_Injected(goal, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Sets the position of an IK goal.</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is set.</param>
	/// <param name="goalPosition">The position in world space.</param>
	public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
	{
		CheckIfInIKPass();
		SetGoalPosition(goal, goalPosition);
	}

	private void SetGoalPosition(AvatarIKGoal goal, Vector3 goalPosition)
	{
		SetGoalPosition_Injected(goal, ref goalPosition);
	}

	/// <summary>
	///   <para>Gets the rotation of an IK goal.</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is is queried.</param>
	public Quaternion GetIKRotation(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalRotation(goal);
	}

	private Quaternion GetGoalRotation(AvatarIKGoal goal)
	{
		GetGoalRotation_Injected(goal, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Sets the rotation of an IK goal.</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is set.</param>
	/// <param name="goalRotation">The rotation in world space.</param>
	public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
	{
		CheckIfInIKPass();
		SetGoalRotation(goal, goalRotation);
	}

	private void SetGoalRotation(AvatarIKGoal goal, Quaternion goalRotation)
	{
		SetGoalRotation_Injected(goal, ref goalRotation);
	}

	/// <summary>
	///   <para>Gets the translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal).</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is queried.</param>
	public float GetIKPositionWeight(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalWeightPosition(goal);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetGoalWeightPosition(AvatarIKGoal goal);

	/// <summary>
	///   <para>Sets the translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal).</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is set.</param>
	/// <param name="value">The translative weight.</param>
	public void SetIKPositionWeight(AvatarIKGoal goal, float value)
	{
		CheckIfInIKPass();
		SetGoalWeightPosition(goal, value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetGoalWeightPosition(AvatarIKGoal goal, float value);

	/// <summary>
	///   <para>Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal).</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is queried.</param>
	public float GetIKRotationWeight(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalWeightRotation(goal);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetGoalWeightRotation(AvatarIKGoal goal);

	/// <summary>
	///   <para>Sets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal).</para>
	/// </summary>
	/// <param name="goal">The AvatarIKGoal that is set.</param>
	/// <param name="value">The rotational weight.</param>
	public void SetIKRotationWeight(AvatarIKGoal goal, float value)
	{
		CheckIfInIKPass();
		SetGoalWeightRotation(goal, value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetGoalWeightRotation(AvatarIKGoal goal, float value);

	/// <summary>
	///   <para>Gets the position of an IK hint.</para>
	/// </summary>
	/// <param name="hint">The AvatarIKHint that is queried.</param>
	/// <returns>
	///   <para>Return the current position of this IK hint in world space.</para>
	/// </returns>
	public Vector3 GetIKHintPosition(AvatarIKHint hint)
	{
		CheckIfInIKPass();
		return GetHintPosition(hint);
	}

	private Vector3 GetHintPosition(AvatarIKHint hint)
	{
		GetHintPosition_Injected(hint, out var ret);
		return ret;
	}

	/// <summary>
	///   <para>Sets the position of an IK hint.</para>
	/// </summary>
	/// <param name="hint">The AvatarIKHint that is set.</param>
	/// <param name="hintPosition">The position in world space.</param>
	public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
	{
		CheckIfInIKPass();
		SetHintPosition(hint, hintPosition);
	}

	private void SetHintPosition(AvatarIKHint hint, Vector3 hintPosition)
	{
		SetHintPosition_Injected(hint, ref hintPosition);
	}

	/// <summary>
	///   <para>Gets the translative weight of an IK Hint (0 = at the original animation before IK, 1 = at the hint).</para>
	/// </summary>
	/// <param name="hint">The AvatarIKHint that is queried.</param>
	/// <returns>
	///   <para>Return translative weight.</para>
	/// </returns>
	public float GetIKHintPositionWeight(AvatarIKHint hint)
	{
		CheckIfInIKPass();
		return GetHintWeightPosition(hint);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetHintWeightPosition(AvatarIKHint hint);

	/// <summary>
	///   <para>Sets the translative weight of an IK hint (0 = at the original animation before IK, 1 = at the hint).</para>
	/// </summary>
	/// <param name="hint">The AvatarIKHint that is set.</param>
	/// <param name="value">The translative weight.</param>
	public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
	{
		CheckIfInIKPass();
		SetHintWeightPosition(hint, value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetHintWeightPosition(AvatarIKHint hint, float value);

	/// <summary>
	///   <para>Sets the look at position.</para>
	/// </summary>
	/// <param name="lookAtPosition">The position to lookAt.</param>
	public void SetLookAtPosition(Vector3 lookAtPosition)
	{
		CheckIfInIKPass();
		SetLookAtPositionInternal(lookAtPosition);
	}

	[NativeMethod("SetLookAtPosition")]
	private void SetLookAtPositionInternal(Vector3 lookAtPosition)
	{
		SetLookAtPositionInternal_Injected(ref lookAtPosition);
	}

	/// <summary>
	///   <para>Set look at weights.</para>
	/// </summary>
	/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
	/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
	/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
	/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
	/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
	public void SetLookAtWeight(float weight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, 0f, 1f, 0f, 0.5f);
	}

	/// <summary>
	///   <para>Set look at weights.</para>
	/// </summary>
	/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
	/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
	/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
	/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
	/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
	public void SetLookAtWeight(float weight, float bodyWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, 1f, 0f, 0.5f);
	}

	/// <summary>
	///   <para>Set look at weights.</para>
	/// </summary>
	/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
	/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
	/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
	/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
	/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
	public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, headWeight, 0f, 0.5f);
	}

	/// <summary>
	///   <para>Set look at weights.</para>
	/// </summary>
	/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
	/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
	/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
	/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
	/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
	public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, 0.5f);
	}

	/// <summary>
	///   <para>Set look at weights.</para>
	/// </summary>
	/// <param name="weight">(0-1) the global weight of the LookAt, multiplier for other parameters.</param>
	/// <param name="bodyWeight">(0-1) determines how much the body is involved in the LookAt.</param>
	/// <param name="headWeight">(0-1) determines how much the head is involved in the LookAt.</param>
	/// <param name="eyesWeight">(0-1) determines how much the eyes are involved in the LookAt.</param>
	/// <param name="clampWeight">(0-1) 0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).</param>
	public void SetLookAtWeight(float weight, [DefaultValue("0.0f")] float bodyWeight, [DefaultValue("1.0f")] float headWeight, [DefaultValue("0.0f")] float eyesWeight, [DefaultValue("0.5f")] float clampWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetLookAtWeight")]
	private extern void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight);

	/// <summary>
	///   <para>Sets local rotation of a human bone during a IK pass.</para>
	/// </summary>
	/// <param name="humanBoneId">The human bone Id.</param>
	/// <param name="rotation">The local rotation.</param>
	public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
	{
		CheckIfInIKPass();
		SetBoneLocalRotationInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId), rotation);
	}

	[NativeMethod("SetBoneLocalRotation")]
	private void SetBoneLocalRotationInternal(int humanBoneId, Quaternion rotation)
	{
		SetBoneLocalRotationInternal_Injected(humanBoneId, ref rotation);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern ScriptableObject GetBehaviour([NotNull] Type type);

	public T GetBehaviour<T>() where T : StateMachineBehaviour
	{
		return GetBehaviour(typeof(T)) as T;
	}

	private static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T : StateMachineBehaviour
	{
		if (rawObjects == null)
		{
			return null;
		}
		T[] array = new T[rawObjects.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (T)rawObjects[i];
		}
		return array;
	}

	public T[] GetBehaviours<T>() where T : StateMachineBehaviour
	{
		return ConvertStateMachineBehaviour<T>(InternalGetBehaviours(typeof(T)));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::InternalGetBehaviours", HasExplicitThis = true)]
	internal extern ScriptableObject[] InternalGetBehaviours([NotNull] Type type);

	public StateMachineBehaviour[] GetBehaviours(int fullPathHash, int layerIndex)
	{
		return InternalGetBehavioursByKey(fullPathHash, layerIndex, typeof(StateMachineBehaviour)) as StateMachineBehaviour[];
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::InternalGetBehavioursByKey", HasExplicitThis = true)]
	internal extern ScriptableObject[] InternalGetBehavioursByKey(int fullPathHash, int layerIndex, [NotNull] Type type);

	/// <summary>
	///   <para>Returns the layer name.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>The layer name.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern string GetLayerName(int layerIndex);

	/// <summary>
	///   <para>Returns the index of the layer with the given name.</para>
	/// </summary>
	/// <param name="layerName">The layer name.</param>
	/// <returns>
	///   <para>The layer index.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern int GetLayerIndex(string layerName);

	/// <summary>
	///   <para>Returns the weight of the layer at the specified index.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>The layer weight.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern float GetLayerWeight(int layerIndex);

	/// <summary>
	///   <para>Sets the weight of the layer at the given index.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <param name="weight">The new layer weight.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetLayerWeight(int layerIndex, float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetAnimatorStateInfo(int layerIndex, StateInfoIndex stateInfoIndex, out AnimatorStateInfo info);

	/// <summary>
	///   <para>Returns an AnimatorStateInfo with the information on the current state.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>An AnimatorStateInfo with the information on the current state.</para>
	/// </returns>
	public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
	{
		GetAnimatorStateInfo(layerIndex, StateInfoIndex.CurrentState, out var info);
		return info;
	}

	/// <summary>
	///   <para>Returns an AnimatorStateInfo with the information on the next state.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>An AnimatorStateInfo with the information on the next state.</para>
	/// </returns>
	public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
	{
		GetAnimatorStateInfo(layerIndex, StateInfoIndex.NextState, out var info);
		return info;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetAnimatorTransitionInfo(int layerIndex, out AnimatorTransitionInfo info);

	/// <summary>
	///   <para>Returns an AnimatorTransitionInfo with the informations on the current transition.</para>
	/// </summary>
	/// <param name="layerIndex">The layer's index.</param>
	/// <returns>
	///   <para>An AnimatorTransitionInfo with the informations on the current transition.</para>
	/// </returns>
	public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
	{
		GetAnimatorTransitionInfo(layerIndex, out var info);
		return info;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern int GetAnimatorClipInfoCount(int layerIndex, bool current);

	/// <summary>
	///   <para>Returns the number of AnimatorClipInfo in the current state.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>The number of AnimatorClipInfo in the current state.</para>
	/// </returns>
	public int GetCurrentAnimatorClipInfoCount(int layerIndex)
	{
		return GetAnimatorClipInfoCount(layerIndex, current: true);
	}

	/// <summary>
	///   <para>Returns the number of AnimatorClipInfo in the next state.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>The number of AnimatorClipInfo in the next state.</para>
	/// </returns>
	public int GetNextAnimatorClipInfoCount(int layerIndex)
	{
		return GetAnimatorClipInfoCount(layerIndex, current: false);
	}

	/// <summary>
	///   <para>Returns an array of all the AnimatorClipInfo in the current state of the given layer.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>An array of all the AnimatorClipInfo in the current state.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetCurrentAnimatorClipInfo", HasExplicitThis = true)]
	public extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex);

	/// <summary>
	///   <para>Returns an array of all the AnimatorClipInfo in the next state of the given layer.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>An array of all the AnimatorClipInfo in the next state.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetNextAnimatorClipInfo", HasExplicitThis = true)]
	public extern AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex);

	public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
	{
		if (clips == null)
		{
			throw new ArgumentNullException("clips");
		}
		GetAnimatorClipInfoInternal(layerIndex, isCurrent: true, clips);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetAnimatorClipInfoInternal", HasExplicitThis = true)]
	private extern void GetAnimatorClipInfoInternal(int layerIndex, bool isCurrent, object clips);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("ENABLE_DOTNET")]
	[FreeFunction(Name = "AnimatorBindings::GetAnimatorClipInfoInternalWinRT", HasExplicitThis = true)]
	private extern AnimatorClipInfo[] GetAnimatorClipInfoInternalWinRT(int layerIndex, bool isCurrent);

	public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
	{
		if (clips == null)
		{
			throw new ArgumentNullException("clips");
		}
		GetAnimatorClipInfoInternal(layerIndex, isCurrent: false, clips);
	}

	/// <summary>
	///   <para>Returns true if there is a transition on the given layer, false otherwise.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <returns>
	///   <para>True if there is a transition on the given layer, false otherwise.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool IsInTransition(int layerIndex);

	/// <summary>
	///   <para>See AnimatorController.parameters.</para>
	/// </summary>
	/// <param name="index"></param>
	public AnimatorControllerParameter GetParameter(int index)
	{
		AnimatorControllerParameter[] array = parameters;
		if (index < 0 && index >= parameters.Length)
		{
			throw new IndexOutOfRangeException("Index must be between 0 and " + parameters.Length);
		}
		return array[index];
	}

	private void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, int targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime)
	{
		MatchTarget_Injected(ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
	}

	public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
	{
		MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, 1f);
	}

	/// <summary>
	///   <para>Automatically adjust the gameobject position and rotation so that the AvatarTarget reaches the matchPosition when the current state is at the specified progress.</para>
	/// </summary>
	/// <param name="matchPosition">The position we want the body part to reach.</param>
	/// <param name="matchRotation">The rotation in which we want the body part to be.</param>
	/// <param name="targetBodyPart">The body part that is involved in the match.</param>
	/// <param name="weightMask">Structure that contains weights for matching position and rotation.</param>
	/// <param name="startNormalizedTime">Start time within the animation clip (0 - beginning of clip, 1 - end of clip).</param>
	/// <param name="targetNormalizedTime">End time within the animation clip (0 - beginning of clip, 1 - end of clip), values greater than 1 can be set to trigger a match after a certain number of loops. Ex: 2.3 means at 30% of 2nd loop.</param>
	public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [DefaultValue("1")] float targetNormalizedTime)
	{
		MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, targetNormalizedTime);
	}

	/// <summary>
	///   <para>Interrupts the automatic target matching.</para>
	/// </summary>
	/// <param name="completeMatch"></param>
	public void InterruptMatchTarget()
	{
		InterruptMatchTarget(completeMatch: true);
	}

	/// <summary>
	///   <para>Interrupts the automatic target matching.</para>
	/// </summary>
	/// <param name="completeMatch"></param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void InterruptMatchTarget([DefaultValue("true")] bool completeMatch);

	[Obsolete("ForceStateNormalizedTime is deprecated. Please use Play or CrossFade instead.")]
	public void ForceStateNormalizedTime(float normalizedTime)
	{
		Play(0, 0, normalizedTime);
	}

	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		int layer = -1;
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, int layer, float fixedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	/// <summary>
	///   <para>Creates a crossfade from the current state to any other state using times in seconds.</para>
	/// </summary>
	/// <param name="stateName">The name of the state.</param>
	/// <param name="stateHashName">The hash name of the state.</param>
	/// <param name="fixedTransitionDuration">The duration of the transition (in seconds).</param>
	/// <param name="layer">The layer where the crossfade occurs.</param>
	/// <param name="fixedTimeOffset">The time of the state (in seconds).</param>
	/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime)
	{
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, int layer, float fixedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		int layer = -1;
		CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	/// <summary>
	///   <para>Creates a crossfade from the current state to any other state using times in seconds.</para>
	/// </summary>
	/// <param name="stateName">The name of the state.</param>
	/// <param name="stateHashName">The hash name of the state.</param>
	/// <param name="fixedTransitionDuration">The duration of the transition (in seconds).</param>
	/// <param name="layer">The layer where the crossfade occurs.</param>
	/// <param name="fixedTimeOffset">The time of the state (in seconds).</param>
	/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::CrossFadeInFixedTime", HasExplicitThis = true)]
	public extern void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime);

	public void CrossFade(string stateName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(string stateName, float normalizedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(string stateName, float normalizedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		int layer = -1;
		CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	/// <summary>
	///   <para>Creates a crossfade from the current state to any other state using normalized times.</para>
	/// </summary>
	/// <param name="stateName">The name of the state.</param>
	/// <param name="stateHashName">The hash name of the state.</param>
	/// <param name="normalizedTransitionDuration">The duration of the transition (normalized).</param>
	/// <param name="layer">The layer where the crossfade occurs.</param>
	/// <param name="normalizedTimeOffset">The time of the state (normalized).</param>
	/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
	public void CrossFade(string stateName, float normalizedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime)
	{
		CrossFade(StringToHash(stateName), normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	/// <summary>
	///   <para>Creates a crossfade from the current state to any other state using normalized times.</para>
	/// </summary>
	/// <param name="stateName">The name of the state.</param>
	/// <param name="stateHashName">The hash name of the state.</param>
	/// <param name="normalizedTransitionDuration">The duration of the transition (normalized).</param>
	/// <param name="layer">The layer where the crossfade occurs.</param>
	/// <param name="normalizedTimeOffset">The time of the state (normalized).</param>
	/// <param name="normalizedTransitionTime">The time of the transition (normalized).</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::CrossFade", HasExplicitThis = true)]
	public extern void CrossFade(int stateHashName, float normalizedTransitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float normalizedTimeOffset, [DefaultValue("0.0f")] float normalizedTransitionTime);

	public void CrossFade(int stateHashName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(int stateHashName, float normalizedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(int stateHashName, float normalizedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		int layer = -1;
		CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void PlayInFixedTime(string stateName, int layer)
	{
		float fixedTime = float.NegativeInfinity;
		PlayInFixedTime(stateName, layer, fixedTime);
	}

	public void PlayInFixedTime(string stateName)
	{
		float fixedTime = float.NegativeInfinity;
		int layer = -1;
		PlayInFixedTime(stateName, layer, fixedTime);
	}

	/// <summary>
	///   <para>Plays a state.</para>
	/// </summary>
	/// <param name="stateName">The state name.</param>
	/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
	/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
	/// <param name="fixedTime">The time offset (in seconds).</param>
	public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
	{
		PlayInFixedTime(StringToHash(stateName), layer, fixedTime);
	}

	/// <summary>
	///   <para>Plays a state.</para>
	/// </summary>
	/// <param name="stateName">The state name.</param>
	/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
	/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
	/// <param name="fixedTime">The time offset (in seconds).</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::PlayInFixedTime", HasExplicitThis = true)]
	public extern void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime);

	public void PlayInFixedTime(int stateNameHash, int layer)
	{
		float fixedTime = float.NegativeInfinity;
		PlayInFixedTime(stateNameHash, layer, fixedTime);
	}

	public void PlayInFixedTime(int stateNameHash)
	{
		float fixedTime = float.NegativeInfinity;
		int layer = -1;
		PlayInFixedTime(stateNameHash, layer, fixedTime);
	}

	public void Play(string stateName, int layer)
	{
		float normalizedTime = float.NegativeInfinity;
		Play(stateName, layer, normalizedTime);
	}

	public void Play(string stateName)
	{
		float normalizedTime = float.NegativeInfinity;
		int layer = -1;
		Play(stateName, layer, normalizedTime);
	}

	/// <summary>
	///   <para>Plays a state.</para>
	/// </summary>
	/// <param name="stateName">The state name.</param>
	/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
	/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
	/// <param name="normalizedTime">The time offset (in percentage).</param>
	public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
	{
		Play(StringToHash(stateName), layer, normalizedTime);
	}

	/// <summary>
	///   <para>Plays a state.</para>
	/// </summary>
	/// <param name="stateName">The state name.</param>
	/// <param name="stateNameHash">The state hash name. If statNameHash is 0, it changes the current state time.</param>
	/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name or hash.</param>
	/// <param name="normalizedTime">The time offset (in percentage).</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::Play", HasExplicitThis = true)]
	public extern void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

	public void Play(int stateNameHash, int layer)
	{
		float normalizedTime = float.NegativeInfinity;
		Play(stateNameHash, layer, normalizedTime);
	}

	public void Play(int stateNameHash)
	{
		float normalizedTime = float.NegativeInfinity;
		int layer = -1;
		Play(stateNameHash, layer, normalizedTime);
	}

	/// <summary>
	///   <para>Sets an AvatarTarget and a targetNormalizedTime for the current state.</para>
	/// </summary>
	/// <param name="targetIndex">The avatar body part that is queried.</param>
	/// <param name="targetNormalizedTime">The current state Time that is queried.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime);

	/// <summary>
	///   <para>Returns true if the transform is controlled by the Animator\.</para>
	/// </summary>
	/// <param name="transform">The transform that is queried.</param>
	[Obsolete("Use mask and layers to control subset of transfroms in a skeleton.", true)]
	public bool IsControlled(Transform transform)
	{
		return false;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern bool IsBoneTransform(Transform transform);

	/// <summary>
	///   <para>Returns Transform mapped to this human bone id.</para>
	/// </summary>
	/// <param name="humanBoneId">The human bone that is queried, see enum HumanBodyBones for a list of possible values.</param>
	public Transform GetBoneTransform(HumanBodyBones humanBoneId)
	{
		return GetBoneTransformInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetBoneTransform")]
	internal extern Transform GetBoneTransformInternal(int humanBoneId);

	/// <summary>
	///   <para>Sets the animator in playback mode.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void StartPlayback();

	/// <summary>
	///   <para>Stops the animator playback mode. When playback stops, the avatar resumes getting control from game logic.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void StopPlayback();

	/// <summary>
	///   <para>Sets the animator in recording mode, and allocates a circular buffer of size frameCount.</para>
	/// </summary>
	/// <param name="frameCount">The number of frames (updates) that will be recorded. If frameCount is 0, the recording will continue until the user calls StopRecording. The maximum value for frameCount is 10000.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void StartRecording(int frameCount);

	/// <summary>
	///   <para>Stops animator record mode.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void StopRecording();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetRecorderStartTime();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetRecorderStopTime();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void ClearInternalControllerPlayable();

	/// <summary>
	///   <para>Returns true if the state exists in this layer, false otherwise.</para>
	/// </summary>
	/// <param name="layerIndex">The layer index.</param>
	/// <param name="stateID">The state ID.</param>
	/// <returns>
	///   <para>True if the state exists in this layer, false otherwise.</para>
	/// </returns>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern bool HasState(int layerIndex, int stateID);

	/// <summary>
	///   <para>Generates an parameter id from a string.</para>
	/// </summary>
	/// <param name="name">The string to convert to Id.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "ScriptingStringToCRC32", IsThreadSafe = true)]
	public static extern int StringToHash(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern string GetStats();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetCurrentGraph", HasExplicitThis = true)]
	private extern void GetCurrentGraph(ref PlayableGraph graph);

	private void CheckIfInIKPass()
	{
		if (logWarnings && !IsInIKPass())
		{
			Debug.LogWarning("Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern bool IsInIKPass();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetFloatString", HasExplicitThis = true)]
	private extern void SetFloatString(string name, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetFloatID", HasExplicitThis = true)]
	private extern void SetFloatID(int id, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetFloatString", HasExplicitThis = true)]
	private extern float GetFloatString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetFloatID", HasExplicitThis = true)]
	private extern float GetFloatID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetBoolString", HasExplicitThis = true)]
	private extern void SetBoolString(string name, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetBoolID", HasExplicitThis = true)]
	private extern void SetBoolID(int id, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetBoolString", HasExplicitThis = true)]
	private extern bool GetBoolString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetBoolID", HasExplicitThis = true)]
	private extern bool GetBoolID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetIntegerString", HasExplicitThis = true)]
	private extern void SetIntegerString(string name, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetIntegerID", HasExplicitThis = true)]
	private extern void SetIntegerID(int id, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetIntegerString", HasExplicitThis = true)]
	private extern int GetIntegerString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::GetIntegerID", HasExplicitThis = true)]
	private extern int GetIntegerID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetTriggerString", HasExplicitThis = true)]
	private extern void SetTriggerString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetTriggerID", HasExplicitThis = true)]
	private extern void SetTriggerID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::ResetTriggerString", HasExplicitThis = true)]
	private extern void ResetTriggerString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::ResetTriggerID", HasExplicitThis = true)]
	private extern void ResetTriggerID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveString", HasExplicitThis = true)]
	private extern bool IsParameterControlledByCurveString(string name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveID", HasExplicitThis = true)]
	private extern bool IsParameterControlledByCurveID(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetFloatStringDamp", HasExplicitThis = true)]
	private extern void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimatorBindings::SetFloatIDDamp", HasExplicitThis = true)]
	private extern void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("UNITY_EDITOR")]
	internal extern void OnUpdateModeChanged();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("UNITY_EDITOR")]
	internal extern void OnCullingModeChanged();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("UNITY_EDITOR")]
	internal extern void WriteDefaultPose();

	/// <summary>
	///   <para>Evaluates the animator based on deltaTime.</para>
	/// </summary>
	/// <param name="deltaTime">The time delta.</param>
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("UpdateWithDelta")]
	public extern void Update(float deltaTime);

	/// <summary>
	///   <para>Rebind all the animated properties and mesh data with the Animator.</para>
	/// </summary>
	public void Rebind()
	{
		Rebind(writeDefaultValues: true);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void Rebind(bool writeDefaultValues);

	/// <summary>
	///   <para>Apply the default Root Motion.</para>
	/// </summary>
	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ApplyBuiltinRootMotion();

	[NativeConditional("UNITY_EDITOR")]
	internal void EvaluateController()
	{
		EvaluateController(0f);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void EvaluateController(float deltaTime);

	[NativeConditional("UNITY_EDITOR")]
	internal string GetCurrentStateName(int layerIndex)
	{
		return GetAnimatorStateName(layerIndex, current: true);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal string GetNextStateName(int layerIndex)
	{
		return GetAnimatorStateName(layerIndex, current: false);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("UNITY_EDITOR")]
	private extern string GetAnimatorStateName(int layerIndex, bool current);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern string ResolveHash(int hash);

	/// <summary>
	///   <para>Gets the value of a vector parameter.</para>
	/// </summary>
	/// <param name="name">The name of the parameter.</param>
	[Obsolete("GetVector is deprecated.")]
	public Vector3 GetVector(string name)
	{
		return Vector3.zero;
	}

	/// <summary>
	///   <para>Gets the value of a vector parameter.</para>
	/// </summary>
	/// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
	[Obsolete("GetVector is deprecated.")]
	public Vector3 GetVector(int id)
	{
		return Vector3.zero;
	}

	/// <summary>
	///   <para>Sets the value of a vector parameter.</para>
	/// </summary>
	/// <param name="name">The name of the parameter.</param>
	/// <param name="value">The new value for the parameter.</param>
	[Obsolete("SetVector is deprecated.")]
	public void SetVector(string name, Vector3 value)
	{
	}

	/// <summary>
	///   <para>Sets the value of a vector parameter.</para>
	/// </summary>
	/// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
	/// <param name="value">The new value for the parameter.</param>
	[Obsolete("SetVector is deprecated.")]
	public void SetVector(int id, Vector3 value)
	{
	}

	/// <summary>
	///   <para>Gets the value of a quaternion parameter.</para>
	/// </summary>
	/// <param name="name">The name of the parameter.</param>
	[Obsolete("GetQuaternion is deprecated.")]
	public Quaternion GetQuaternion(string name)
	{
		return Quaternion.identity;
	}

	/// <summary>
	///   <para>Gets the value of a quaternion parameter.</para>
	/// </summary>
	/// <param name="id">The id of the parameter. The id is generated using Animator::StringToHash.</param>
	[Obsolete("GetQuaternion is deprecated.")]
	public Quaternion GetQuaternion(int id)
	{
		return Quaternion.identity;
	}

	/// <summary>
	///   <para>Sets the value of a quaternion parameter.</para>
	/// </summary>
	/// <param name="name">The name of the parameter.</param>
	/// <param name="value">The new value for the parameter.</param>
	[Obsolete("SetQuaternion is deprecated.")]
	public void SetQuaternion(string name, Quaternion value)
	{
	}

	/// <summary>
	///   <para>Sets the value of a quaternion parameter.</para>
	/// </summary>
	/// <param name="id">Of the parameter. The id is generated using Animator::StringToHash.</param>
	/// <param name="value">The new value for the parameter.</param>
	[Obsolete("SetQuaternion is deprecated.")]
	public void SetQuaternion(int id, Quaternion value)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_deltaPosition_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_deltaRotation_Injected(out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_velocity_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_angularVelocity_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rootPosition_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rootPosition_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_rootRotation_Injected(out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_rootRotation_Injected(ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_bodyPositionInternal_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_bodyPositionInternal_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_bodyRotationInternal_Injected(out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_bodyRotationInternal_Injected(ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetGoalPosition_Injected(AvatarIKGoal goal, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetGoalPosition_Injected(AvatarIKGoal goal, ref Vector3 goalPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetGoalRotation_Injected(AvatarIKGoal goal, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetGoalRotation_Injected(AvatarIKGoal goal, ref Quaternion goalRotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetHintPosition_Injected(AvatarIKHint hint, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetHintPosition_Injected(AvatarIKHint hint, ref Vector3 hintPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetLookAtPositionInternal_Injected(ref Vector3 lookAtPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetBoneLocalRotationInternal_Injected(int humanBoneId, ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_pivotPosition_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void MatchTarget_Injected(ref Vector3 matchPosition, ref Quaternion matchRotation, int targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_targetPosition_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_targetRotation_Injected(out Quaternion ret);
}
