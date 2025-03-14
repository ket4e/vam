using System;

namespace UnityEngine.Experimental.UIElements;

internal class ClampedDragger : Clickable
{
	[Flags]
	public enum DragDirection
	{
		None = 0,
		LowToHigh = 1,
		HighToLow = 2,
		Free = 4
	}

	public DragDirection dragDirection { get; set; }

	private Slider slider { get; set; }

	public Vector2 startMousePosition { get; private set; }

	public Vector2 delta => base.lastMousePosition - startMousePosition;

	public event Action dragging;

	public ClampedDragger(Slider slider, Action clickHandler, Action dragHandler)
		: base(clickHandler, 250L, 30L)
	{
		dragDirection = DragDirection.None;
		this.slider = slider;
		dragging += dragHandler;
	}

	protected override void RegisterCallbacksOnTarget()
	{
		base.target.RegisterCallback<MouseDownEvent>(OnMouseDown);
		base.target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
		base.target.RegisterCallback<MouseUpEvent>(base.OnMouseUp);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		base.target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
		base.target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
		base.target.UnregisterCallback<MouseUpEvent>(base.OnMouseUp);
	}

	private new void OnMouseDown(MouseDownEvent evt)
	{
		if (CanStartManipulation(evt))
		{
			startMousePosition = evt.localMousePosition;
			dragDirection = DragDirection.None;
			base.OnMouseDown(evt);
		}
	}

	private new void OnMouseMove(MouseMoveEvent evt)
	{
		if (base.target.HasMouseCapture())
		{
			base.OnMouseMove(evt);
			if (dragDirection == DragDirection.None)
			{
				dragDirection = DragDirection.Free;
			}
			if (dragDirection == DragDirection.Free && this.dragging != null)
			{
				this.dragging();
			}
		}
	}
}
