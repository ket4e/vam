using System;

namespace UnityEngine.Experimental.UIElements;

public class ScrollerButton : VisualElement
{
	public Clickable clickable;

	public ScrollerButton(Action clickEvent, long delay, long interval)
	{
		clickable = new Clickable(clickEvent, delay, interval);
		this.AddManipulator(clickable);
	}
}
