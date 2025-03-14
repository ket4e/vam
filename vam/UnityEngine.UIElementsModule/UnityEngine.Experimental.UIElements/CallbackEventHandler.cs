namespace UnityEngine.Experimental.UIElements;

/// <summary>
///   <para>Interface for classes capable of having callbacks to handle events.</para>
/// </summary>
public abstract class CallbackEventHandler : IEventHandler
{
	private EventCallbackRegistry m_CallbackRegistry;

	public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		if (m_CallbackRegistry == null)
		{
			m_CallbackRegistry = new EventCallbackRegistry();
		}
		m_CallbackRegistry.RegisterCallback(callback, useCapture);
	}

	public void RegisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		if (m_CallbackRegistry == null)
		{
			m_CallbackRegistry = new EventCallbackRegistry();
		}
		m_CallbackRegistry.RegisterCallback(callback, userArgs, useCapture);
	}

	public void UnregisterCallback<TEventType>(EventCallback<TEventType> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		if (m_CallbackRegistry != null)
		{
			m_CallbackRegistry.UnregisterCallback(callback, useCapture);
		}
	}

	public void UnregisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, Capture useCapture = Capture.NoCapture) where TEventType : EventBase<TEventType>, new()
	{
		if (m_CallbackRegistry != null)
		{
			m_CallbackRegistry.UnregisterCallback(callback, useCapture);
		}
	}

	/// <summary>
	///   <para>Handle an event, most often by executing the callbacks associated with the event.</para>
	/// </summary>
	/// <param name="evt">The event to handle.</param>
	public virtual void HandleEvent(EventBase evt)
	{
		if (evt.propagationPhase != PropagationPhase.DefaultAction)
		{
			if (!evt.isPropagationStopped && m_CallbackRegistry != null)
			{
				m_CallbackRegistry.InvokeCallbacks(evt);
			}
			if (evt.propagationPhase == PropagationPhase.AtTarget && !evt.isDefaultPrevented)
			{
				ExecuteDefaultActionAtTarget(evt);
			}
		}
		else if (!evt.isDefaultPrevented)
		{
			ExecuteDefaultAction(evt);
		}
	}

	/// <summary>
	///   <para>Return true if event handlers for the event propagation capture phase have been attached on this object.</para>
	/// </summary>
	/// <returns>
	///   <para>True if object has event handlers for the capture phase.</para>
	/// </returns>
	public bool HasCaptureHandlers()
	{
		return m_CallbackRegistry != null && m_CallbackRegistry.HasCaptureHandlers();
	}

	/// <summary>
	///   <para>Return true if event handlers for the event propagation bubble up phase have been attached on this object.</para>
	/// </summary>
	/// <returns>
	///   <para>True if object has event handlers for the bubble up phase.</para>
	/// </returns>
	public bool HasBubbleHandlers()
	{
		return m_CallbackRegistry != null && m_CallbackRegistry.HasBubbleHandlers();
	}

	protected internal virtual void ExecuteDefaultActionAtTarget(EventBase evt)
	{
	}

	protected internal virtual void ExecuteDefaultAction(EventBase evt)
	{
	}
}
