using System;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Base class for all user-defined UXML Element factories.</para>
/// </summary>
public abstract class UxmlFactory<T> : IUxmlFactory where T : VisualElement
{
	public Type CreatesType => typeof(T);

	public VisualElement Create(IUxmlAttributes bag, CreationContext cc)
	{
		return DoCreate(bag, cc);
	}

	protected abstract T DoCreate(IUxmlAttributes bag, CreationContext cc);
}
