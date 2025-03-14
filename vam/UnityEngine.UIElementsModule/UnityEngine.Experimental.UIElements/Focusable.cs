namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Base class for objects that can get the focus.</para>
/// </summary>
public abstract class Focusable : CallbackEventHandler
{
	private int m_FocusIndex;

	/// <summary>
	///   <para>Return the focus controller for this element.</para>
	/// </summary>
	public abstract FocusController focusController { get; }

	/// <summary>
	///   <para>An integer used to sort focusables in the focus ring. A negative value means that the element can not be focused.</para>
	/// </summary>
	public int focusIndex
	{
		get
		{
			return m_FocusIndex;
		}
		set
		{
			m_FocusIndex = value;
		}
	}

	/// <summary>
	///   <para>Return true if the element can be focused.</para>
	/// </summary>
	public virtual bool canGrabFocus => m_FocusIndex >= 0;

	protected Focusable()
	{
		m_FocusIndex = 0;
	}

	/// <summary>
	///   <para>Attempt to give the focus to this element.</para>
	/// </summary>
	public virtual void Focus()
	{
		if (focusController != null)
		{
			focusController.SwitchFocus((!canGrabFocus) ? null : this);
		}
	}

	/// <summary>
	///   <para>Tell the element to release the focus.</para>
	/// </summary>
	public virtual void Blur()
	{
		if (focusController != null && focusController.focusedElement == this)
		{
			focusController.SwitchFocus(null);
		}
	}

	protected internal override void ExecuteDefaultAction(EventBase evt)
	{
		base.ExecuteDefaultAction(evt);
		if (evt.GetEventTypeId() == EventBase<MouseDownEvent>.TypeId())
		{
			Focus();
		}
		if (focusController != null)
		{
			focusController.SwitchFocusOnEvent(evt);
		}
	}
}
