using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>AnimationEvent lets you call a script function similar to SendMessage as part of playing back an animation.</para>
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
public sealed class AnimationEvent
{
	internal float m_Time;

	internal string m_FunctionName;

	internal string m_StringParameter;

	internal Object m_ObjectReferenceParameter;

	internal float m_FloatParameter;

	internal int m_IntParameter;

	internal int m_MessageOptions;

	internal AnimationEventSource m_Source;

	internal AnimationState m_StateSender;

	internal AnimatorStateInfo m_AnimatorStateInfo;

	internal AnimatorClipInfo m_AnimatorClipInfo;

	[Obsolete("Use stringParameter instead")]
	public string data
	{
		get
		{
			return m_StringParameter;
		}
		set
		{
			m_StringParameter = value;
		}
	}

	/// <summary>
	///   <para>String parameter that is stored in the event and will be sent to the function.</para>
	/// </summary>
	public string stringParameter
	{
		get
		{
			return m_StringParameter;
		}
		set
		{
			m_StringParameter = value;
		}
	}

	/// <summary>
	///   <para>Float parameter that is stored in the event and will be sent to the function.</para>
	/// </summary>
	public float floatParameter
	{
		get
		{
			return m_FloatParameter;
		}
		set
		{
			m_FloatParameter = value;
		}
	}

	/// <summary>
	///   <para>Int parameter that is stored in the event and will be sent to the function.</para>
	/// </summary>
	public int intParameter
	{
		get
		{
			return m_IntParameter;
		}
		set
		{
			m_IntParameter = value;
		}
	}

	/// <summary>
	///   <para>Object reference parameter that is stored in the event and will be sent to the function.</para>
	/// </summary>
	public Object objectReferenceParameter
	{
		get
		{
			return m_ObjectReferenceParameter;
		}
		set
		{
			m_ObjectReferenceParameter = value;
		}
	}

	/// <summary>
	///   <para>The name of the function that will be called.</para>
	/// </summary>
	public string functionName
	{
		get
		{
			return m_FunctionName;
		}
		set
		{
			m_FunctionName = value;
		}
	}

	/// <summary>
	///   <para>The time at which the event will be fired off.</para>
	/// </summary>
	public float time
	{
		get
		{
			return m_Time;
		}
		set
		{
			m_Time = value;
		}
	}

	/// <summary>
	///   <para>Function call options.</para>
	/// </summary>
	public SendMessageOptions messageOptions
	{
		get
		{
			return (SendMessageOptions)m_MessageOptions;
		}
		set
		{
			m_MessageOptions = (int)value;
		}
	}

	/// <summary>
	///   <para>Returns true if this Animation event has been fired by an Animation component.</para>
	/// </summary>
	public bool isFiredByLegacy => m_Source == AnimationEventSource.Legacy;

	/// <summary>
	///   <para>Returns true if this Animation event has been fired by an Animator component.</para>
	/// </summary>
	public bool isFiredByAnimator => m_Source == AnimationEventSource.Animator;

	/// <summary>
	///   <para>The animation state that fired this event (Read Only).</para>
	/// </summary>
	public AnimationState animationState
	{
		get
		{
			if (!isFiredByLegacy)
			{
				Debug.LogError("AnimationEvent was not fired by Animation component, you shouldn't use AnimationEvent.animationState");
			}
			return m_StateSender;
		}
	}

	/// <summary>
	///   <para>The animator state info related to this event (Read Only).</para>
	/// </summary>
	public AnimatorStateInfo animatorStateInfo
	{
		get
		{
			if (!isFiredByAnimator)
			{
				Debug.LogError("AnimationEvent was not fired by Animator component, you shouldn't use AnimationEvent.animatorStateInfo");
			}
			return m_AnimatorStateInfo;
		}
	}

	/// <summary>
	///   <para>The animator clip info related to this event (Read Only).</para>
	/// </summary>
	public AnimatorClipInfo animatorClipInfo
	{
		get
		{
			if (!isFiredByAnimator)
			{
				Debug.LogError("AnimationEvent was not fired by Animator component, you shouldn't use AnimationEvent.animatorClipInfo");
			}
			return m_AnimatorClipInfo;
		}
	}

	/// <summary>
	///   <para>Creates a new animation event.</para>
	/// </summary>
	public AnimationEvent()
	{
		m_Time = 0f;
		m_FunctionName = "";
		m_StringParameter = "";
		m_ObjectReferenceParameter = null;
		m_FloatParameter = 0f;
		m_IntParameter = 0;
		m_MessageOptions = 0;
		m_Source = AnimationEventSource.NoSource;
		m_StateSender = null;
	}

	internal int GetHash()
	{
		int num = 0;
		num = functionName.GetHashCode();
		return 33 * num + time.GetHashCode();
	}
}
