using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Used to communicate between scripting and the controller. Some parameters can be set in scripting and used by the controller, while other parameters are based on Custom Curves in Animation Clips and can be sampled using the scripting API.</para>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[NativeType(CodegenOptions.Custom, "MonoAnimatorControllerParameter")]
[UsedByNativeCode]
[NativeAsStruct]
[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorControllerParameter.bindings.h")]
[NativeHeader("Runtime/Animation/AnimatorControllerParameter.h")]
public class AnimatorControllerParameter
{
	internal string m_Name = "";

	internal AnimatorControllerParameterType m_Type;

	internal float m_DefaultFloat;

	internal int m_DefaultInt;

	internal bool m_DefaultBool;

	/// <summary>
	///   <para>The name of the parameter.</para>
	/// </summary>
	public string name => m_Name;

	/// <summary>
	///   <para>Returns the hash of the parameter based on its name.</para>
	/// </summary>
	public int nameHash => Animator.StringToHash(m_Name);

	/// <summary>
	///   <para>The type of the parameter.</para>
	/// </summary>
	public AnimatorControllerParameterType type
	{
		get
		{
			return m_Type;
		}
		set
		{
			m_Type = value;
		}
	}

	/// <summary>
	///   <para>The default float value for the parameter.</para>
	/// </summary>
	public float defaultFloat
	{
		get
		{
			return m_DefaultFloat;
		}
		set
		{
			m_DefaultFloat = value;
		}
	}

	/// <summary>
	///   <para>The default int value for the parameter.</para>
	/// </summary>
	public int defaultInt
	{
		get
		{
			return m_DefaultInt;
		}
		set
		{
			m_DefaultInt = value;
		}
	}

	/// <summary>
	///   <para>The default bool value for the parameter.</para>
	/// </summary>
	public bool defaultBool
	{
		get
		{
			return m_DefaultBool;
		}
		set
		{
			m_DefaultBool = value;
		}
	}

	public override bool Equals(object o)
	{
		return o is AnimatorControllerParameter animatorControllerParameter && m_Name == animatorControllerParameter.m_Name && m_Type == animatorControllerParameter.m_Type && m_DefaultFloat == animatorControllerParameter.m_DefaultFloat && m_DefaultInt == animatorControllerParameter.m_DefaultInt && m_DefaultBool == animatorControllerParameter.m_DefaultBool;
	}

	public override int GetHashCode()
	{
		return name.GetHashCode();
	}
}
