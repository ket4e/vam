namespace UnityEngine.Experimental.UIElements;

internal abstract class ContextualMenuManager
{
	public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

	public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
	{
		ContextualMenu contextualMenu = new ContextualMenu();
		bool flag;
		using (ContextualMenuPopulateEvent contextualMenuPopulateEvent = ContextualMenuPopulateEvent.GetPooled(triggerEvent, contextualMenu, target))
		{
			UIElementsUtility.eventDispatcher.DispatchEvent(contextualMenuPopulateEvent, null);
			flag = !contextualMenuPopulateEvent.isDefaultPrevented;
		}
		if (flag)
		{
			contextualMenu.PrepareForDisplay(triggerEvent);
			DoDisplayMenu(contextualMenu, triggerEvent);
		}
	}

	protected abstract void DoDisplayMenu(ContextualMenu menu, EventBase triggerEvent);
}
