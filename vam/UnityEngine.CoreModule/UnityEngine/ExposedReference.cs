using System;
using UnityEngine.Scripting;

namespace UnityEngine;

/// <summary>
///   <para>Creates a type whos value is resolvable at runtime.</para>
/// </summary>
[Serializable]
[UsedByNativeCode(Name = "ExposedReference")]
public struct ExposedReference<T> where T : Object
{
	[SerializeField]
	public PropertyName exposedName;

	[SerializeField]
	public Object defaultValue;

	public T Resolve(IExposedPropertyTable resolver)
	{
		if (resolver != null)
		{
			bool idValid;
			Object referenceValue = resolver.GetReferenceValue(exposedName, out idValid);
			if (idValid)
			{
				return referenceValue as T;
			}
		}
		return defaultValue as T;
	}
}
