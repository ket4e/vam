using System;

namespace UnityEngine.Experimental.UIElements;

public class RepeatButton : BaseTextElement
{
	public RepeatButton(Action clickEvent, long delay, long interval)
	{
		this.AddManipulator(new Clickable(clickEvent, delay, interval));
	}
}
