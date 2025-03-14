using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class KeyEvents
{
	protected List<Event> keyEvents = new List<Event>();

	protected List<Event> keyEventsLast = new List<Event>();

	private static readonly KeyCode[] keysToCheck = new KeyCode[2]
	{
		KeyCode.LeftShift,
		KeyCode.RightShift
	};

	private static readonly KeyCode[] keysToIgnore = new KeyCode[0];

	public List<Event> Events => keyEventsLast;

	public void InputUpdate()
	{
		List<Event> list = keyEvents;
		keyEvents = keyEventsLast;
		keyEventsLast = list;
		keyEvents.Clear();
		for (int i = 0; i < keysToCheck.Length; i++)
		{
			if (Input.GetKeyDown(keysToCheck[i]))
			{
				keyEventsLast.Insert(0, new Event
				{
					type = EventType.KeyDown,
					keyCode = keysToCheck[i]
				});
			}
			else if (Input.GetKeyUp(keysToCheck[i]))
			{
				keyEventsLast.Add(new Event
				{
					type = EventType.KeyUp,
					keyCode = keysToCheck[i]
				});
			}
		}
	}

	public void Feed(Event ev)
	{
		if (ev.type != EventType.KeyDown && ev.type != EventType.KeyUp)
		{
			return;
		}
		for (int i = 0; i < keysToIgnore.Length; i++)
		{
			if (ev.keyCode == keysToIgnore[i])
			{
				return;
			}
		}
		keyEvents.Add(new Event(ev));
	}

	public void Press(KeyCode key)
	{
		keyEvents.Add(new Event
		{
			type = EventType.KeyDown,
			keyCode = key
		});
	}

	public void Release(KeyCode key)
	{
		keyEvents.Add(new Event
		{
			type = EventType.KeyUp,
			keyCode = key
		});
	}

	public void Type(string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			keyEvents.Add(new Event
			{
				type = EventType.KeyDown,
				character = text[i]
			});
		}
	}
}
