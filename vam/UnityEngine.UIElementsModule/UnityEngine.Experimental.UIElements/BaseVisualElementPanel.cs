using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements;

internal abstract class BaseVisualElementPanel : IPanel
{
	public abstract EventInterests IMGUIEventInterests { get; set; }

	public abstract ScriptableObject ownerObject { get; protected set; }

	public abstract SavePersistentViewData savePersistentViewData { get; set; }

	public abstract GetViewDataDictionary getViewDataDictionary { get; set; }

	public abstract int IMGUIContainersCount { get; set; }

	public abstract FocusController focusController { get; set; }

	internal virtual IStylePainter stylePainter { get; set; }

	internal virtual ICursorManager cursorManager { get; set; }

	internal virtual ContextualMenuManager contextualMenuManager { get; set; }

	public abstract VisualElement visualTree { get; }

	public abstract IEventDispatcher dispatcher { get; protected set; }

	internal abstract IScheduler scheduler { get; }

	internal abstract IDataWatchService dataWatch { get; }

	public abstract ContextType contextType { get; protected set; }

	public abstract bool keepPixelCacheOnWorldBoundChange { get; set; }

	public abstract void Repaint(Event e);

	public abstract void ValidateLayout();

	public abstract VisualElement Pick(Vector2 point);

	public abstract VisualElement PickAll(Vector2 point, List<VisualElement> picked);

	public abstract VisualElement LoadTemplate(string path, Dictionary<string, VisualElement> slots = null);
}
