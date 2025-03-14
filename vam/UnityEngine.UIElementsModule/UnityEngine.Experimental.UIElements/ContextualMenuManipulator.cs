using System;

namespace UnityEngine.Experimental.UIElements;

internal class ContextualMenuManipulator : MouseManipulator
{
	private Action<ContextualMenuPopulateEvent> m_MenuBuilder;

	public ContextualMenuManipulator(Action<ContextualMenuPopulateEvent> menuBuilder)
	{
		m_MenuBuilder = menuBuilder;
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.RightMouse
		});
	}

	protected override void RegisterCallbacksOnTarget()
	{
		base.target.RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
		base.target.RegisterCallback<KeyUpEvent>(OnKeyUpEvent);
		base.target.RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		base.target.UnregisterCallback<MouseUpEvent>(OnMouseUpEvent);
		base.target.UnregisterCallback<KeyUpEvent>(OnKeyUpEvent);
		base.target.UnregisterCallback<ContextualMenuPopulateEvent>(OnContextualMenuEvent);
	}

	private void OnMouseUpEvent(MouseUpEvent evt)
	{
		if (CanStartManipulation(evt) && base.target.elementPanel != null && base.target.elementPanel.contextualMenuManager != null)
		{
			base.target.elementPanel.contextualMenuManager.DisplayMenu(evt, base.target);
			evt.StopPropagation();
			evt.PreventDefault();
		}
	}

	private void OnKeyUpEvent(KeyUpEvent evt)
	{
		if (evt.keyCode == KeyCode.Menu && base.target.elementPanel != null && base.target.elementPanel.contextualMenuManager != null)
		{
			base.target.elementPanel.contextualMenuManager.DisplayMenu(evt, base.target);
			evt.StopPropagation();
			evt.PreventDefault();
		}
	}

	private void OnContextualMenuEvent(ContextualMenuPopulateEvent evt)
	{
		if (m_MenuBuilder != null)
		{
			m_MenuBuilder(evt);
		}
	}
}
