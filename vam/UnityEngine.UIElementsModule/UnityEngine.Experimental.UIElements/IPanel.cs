using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

public interface IPanel
{
	VisualElement visualTree { get; }

	IEventDispatcher dispatcher { get; }

	ContextType contextType { get; }

	/// <summary>
	///   <para>Return the focus controller for this panel.</para>
	/// </summary>
	FocusController focusController { get; }

	VisualElement Pick(Vector2 point);

	VisualElement LoadTemplate(string path, Dictionary<string, VisualElement> slots = null);

	VisualElement PickAll(Vector2 point, List<VisualElement> picked);
}
