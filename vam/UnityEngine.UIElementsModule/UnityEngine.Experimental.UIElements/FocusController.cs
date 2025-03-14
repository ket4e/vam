namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Class in charge of managing the focus inside a Panel.</para>
/// </summary>
public class FocusController
{
	private IFocusRing focusRing { get; set; }

	/// <summary>
	///   <para>The currently focused element.</para>
	/// </summary>
	public Focusable focusedElement { get; private set; }

	internal int imguiKeyboardControl { get; set; }

	/// <summary>
	///   <para>Constructor.</para>
	/// </summary>
	/// <param name="focusRing"></param>
	public FocusController(IFocusRing focusRing)
	{
		this.focusRing = focusRing;
		focusedElement = null;
		imguiKeyboardControl = 0;
	}

	private static void AboutToReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
	{
		using FocusOutEvent evt = FocusEventBase<FocusOutEvent>.GetPooled(focusable, willGiveFocusTo, direction);
		UIElementsUtility.eventDispatcher.DispatchEvent(evt, null);
	}

	private static void ReleaseFocus(Focusable focusable, Focusable willGiveFocusTo, FocusChangeDirection direction)
	{
		using BlurEvent evt = FocusEventBase<BlurEvent>.GetPooled(focusable, willGiveFocusTo, direction);
		UIElementsUtility.eventDispatcher.DispatchEvent(evt, null);
	}

	private static void AboutToGrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction)
	{
		using FocusInEvent evt = FocusEventBase<FocusInEvent>.GetPooled(focusable, willTakeFocusFrom, direction);
		UIElementsUtility.eventDispatcher.DispatchEvent(evt, null);
	}

	private static void GrabFocus(Focusable focusable, Focusable willTakeFocusFrom, FocusChangeDirection direction)
	{
		using FocusEvent evt = FocusEventBase<FocusEvent>.GetPooled(focusable, willTakeFocusFrom, direction);
		UIElementsUtility.eventDispatcher.DispatchEvent(evt, null);
	}

	internal void SwitchFocus(Focusable newFocusedElement)
	{
		SwitchFocus(newFocusedElement, FocusChangeDirection.unspecified);
	}

	private void SwitchFocus(Focusable newFocusedElement, FocusChangeDirection direction)
	{
		if (newFocusedElement == focusedElement)
		{
			return;
		}
		Focusable focusable = focusedElement;
		if (newFocusedElement == null || !newFocusedElement.canGrabFocus)
		{
			if (focusable != null)
			{
				AboutToReleaseFocus(focusable, newFocusedElement, direction);
				focusedElement = null;
				ReleaseFocus(focusable, newFocusedElement, direction);
			}
		}
		else if (newFocusedElement != focusable)
		{
			if (focusable != null)
			{
				AboutToReleaseFocus(focusable, newFocusedElement, direction);
			}
			AboutToGrabFocus(newFocusedElement, focusable, direction);
			focusedElement = newFocusedElement;
			if (focusable != null)
			{
				ReleaseFocus(focusable, newFocusedElement, direction);
			}
			GrabFocus(newFocusedElement, focusable, direction);
		}
	}

	/// <summary>
	///   <para>Ask the controller to change the focus according to the event. The focus controller will use its focus ring to choose the next element to be focused.</para>
	/// </summary>
	/// <param name="e"></param>
	public void SwitchFocusOnEvent(EventBase e)
	{
		FocusChangeDirection focusChangeDirection = focusRing.GetFocusChangeDirection(focusedElement, e);
		if (focusChangeDirection != FocusChangeDirection.none)
		{
			Focusable nextFocusable = focusRing.GetNextFocusable(focusedElement, focusChangeDirection);
			SwitchFocus(nextFocusable, focusChangeDirection);
		}
	}

	internal void SyncIMGUIFocus(int imguiKeyboardControlID, IMGUIContainer imguiContainerHavingKeyboardControl)
	{
		imguiKeyboardControl = imguiKeyboardControlID;
		if (imguiKeyboardControl != 0)
		{
			SwitchFocus(imguiContainerHavingKeyboardControl, FocusChangeDirection.unspecified);
		}
		else
		{
			SwitchFocus(null, FocusChangeDirection.unspecified);
		}
	}
}
