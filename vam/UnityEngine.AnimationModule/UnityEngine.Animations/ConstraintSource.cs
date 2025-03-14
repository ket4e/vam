using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

/// <summary>
///   <para>Represents a source for the constraint.</para>
/// </summary>
[Serializable]
[NativeType(CodegenOptions = CodegenOptions.Custom, Header = "Runtime/Animation/Constraints/ConstraintSource.h", IntermediateScriptingStructName = "MonoConstraintSource")]
[UsedByNativeCode]
[NativeHeader("Runtime/Animation/Constraints/Constraint.bindings.h")]
public struct ConstraintSource
{
	[NativeName("sourceTransform")]
	private Transform m_SourceTransform;

	[NativeName("weight")]
	private float m_Weight;

	/// <summary>
	///   <para>The transform component of the source object.</para>
	/// </summary>
	public Transform sourceTransform
	{
		get
		{
			return m_SourceTransform;
		}
		set
		{
			m_SourceTransform = value;
		}
	}

	/// <summary>
	///   <para>The weight of the source in the evaluation of the constraint.</para>
	/// </summary>
	public float weight
	{
		get
		{
			return m_Weight;
		}
		set
		{
			m_Weight = value;
		}
	}
}
