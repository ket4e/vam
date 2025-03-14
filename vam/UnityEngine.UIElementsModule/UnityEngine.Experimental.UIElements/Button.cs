using System;

namespace UnityEngine.Experimental.UIElements;

public class Button : BaseTextElement
{
	public Clickable clickable;

	public Button(Action clickEvent)
	{
		clickable = new Clickable(clickEvent);
		this.AddManipulator(clickable);
	}
}
