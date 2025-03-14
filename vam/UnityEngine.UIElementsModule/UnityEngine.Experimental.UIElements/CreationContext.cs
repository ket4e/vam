using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>This class is used during UXML template instantiation.</para>
/// </summary>
public struct CreationContext
{
	public static readonly CreationContext Default = default(CreationContext);

	public VisualElement target { get; private set; }

	public VisualTreeAsset visualTreeAsset { get; private set; }

	public Dictionary<string, VisualElement> slotInsertionPoints { get; private set; }

	internal CreationContext(Dictionary<string, VisualElement> slotInsertionPoints, VisualTreeAsset vta, VisualElement target)
	{
		this.target = target;
		this.slotInsertionPoints = slotInsertionPoints;
		visualTreeAsset = vta;
	}
}
