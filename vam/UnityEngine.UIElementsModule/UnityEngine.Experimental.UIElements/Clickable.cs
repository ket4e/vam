using System;

namespace UnityEngine.Experimental.UIElements;

public class Clickable : MouseManipulator
{
	private readonly long m_Delay;

	private readonly long m_Interval;

	private IVisualElementScheduledItem m_Repeater;

	public Vector2 lastMousePosition { get; private set; }

	public event Action clicked;

	public Clickable(Action handler, long delay, long interval)
		: this(handler)
	{
		m_Delay = delay;
		m_Interval = interval;
	}

	public Clickable(Action handler)
	{
		this.clicked = handler;
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
	}

	private void OnTimer(TimerState timerState)
	{
		if (this.clicked != null && IsRepeatable())
		{
			if (base.target.ContainsPoint(lastMousePosition))
			{
				this.clicked();
				base.target.pseudoStates |= PseudoStates.Active;
			}
			else
			{
				base.target.pseudoStates &= ~PseudoStates.Active;
			}
		}
	}

	private bool IsRepeatable()
	{
		return m_Delay > 0 || m_Interval > 0;
	}

	protected override void RegisterCallbacksOnTarget()
	{
		base.target.RegisterCallback<MouseDownEvent>(OnMouseDown);
		base.target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
		base.target.RegisterCallback<MouseUpEvent>(OnMouseUp);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		base.target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
		base.target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
		base.target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
	}

	protected void OnMouseDown(MouseDownEvent evt)
	{
		if (!CanStartManipulation(evt))
		{
			return;
		}
		base.target.TakeMouseCapture();
		lastMousePosition = evt.localMousePosition;
		if (IsRepeatable())
		{
			if (this.clicked != null && base.target.ContainsPoint(evt.localMousePosition))
			{
				this.clicked();
			}
			if (m_Repeater == null)
			{
				m_Repeater = base.target.schedule.Execute(OnTimer).Every(m_Interval).StartingIn(m_Delay);
			}
			else
			{
				m_Repeater.ExecuteLater(m_Delay);
			}
		}
		base.target.pseudoStates |= PseudoStates.Active;
		evt.StopPropagation();
	}

	protected void OnMouseMove(MouseMoveEvent evt)
	{
		if (base.target.HasMouseCapture())
		{
			lastMousePosition = evt.localMousePosition;
			evt.StopPropagation();
		}
	}

	protected void OnMouseUp(MouseUpEvent evt)
	{
		if (!CanStopManipulation(evt))
		{
			return;
		}
		base.target.ReleaseMouseCapture();
		if (IsRepeatable())
		{
			if (m_Repeater != null)
			{
				m_Repeater.Pause();
			}
		}
		else if (this.clicked != null && base.target.ContainsPoint(evt.localMousePosition))
		{
			this.clicked();
		}
		base.target.pseudoStates &= ~PseudoStates.Active;
		evt.StopPropagation();
	}
}
