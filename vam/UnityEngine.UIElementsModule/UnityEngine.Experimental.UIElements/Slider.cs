using System;

namespace UnityEngine.Experimental.UIElements;

public class Slider : VisualElement
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	[Serializable]
	private class SliderValue
	{
		public float m_Value = 0f;
	}

	private float m_LowValue;

	private float m_HighValue;

	private Rect m_DragElementStartPos;

	private SliderValue m_SliderValue;

	private Direction m_Direction;

	public VisualElement dragElement { get; private set; }

	public float lowValue
	{
		get
		{
			return m_LowValue;
		}
		set
		{
			if (!Mathf.Approximately(m_LowValue, value))
			{
				m_LowValue = value;
				ClampValue();
			}
		}
	}

	public float highValue
	{
		get
		{
			return m_HighValue;
		}
		set
		{
			if (!Mathf.Approximately(m_HighValue, value))
			{
				m_HighValue = value;
				ClampValue();
			}
		}
	}

	public float range => highValue - lowValue;

	public float pageSize { get; set; }

	internal ClampedDragger clampedDragger { get; private set; }

	public float value
	{
		get
		{
			return (m_SliderValue != null) ? m_SliderValue.m_Value : 0f;
		}
		set
		{
			if (m_SliderValue == null)
			{
				m_SliderValue = new SliderValue
				{
					m_Value = lowValue
				};
			}
			float b = Mathf.Clamp(value, lowValue, highValue);
			if (!Mathf.Approximately(m_SliderValue.m_Value, b))
			{
				m_SliderValue.m_Value = b;
				UpdateDragElementPosition();
				if (this.valueChanged != null)
				{
					this.valueChanged(m_SliderValue.m_Value);
				}
				Dirty(ChangeType.Repaint);
				SavePersistentData();
			}
		}
	}

	public Direction direction
	{
		get
		{
			return m_Direction;
		}
		set
		{
			m_Direction = value;
			if (m_Direction == Direction.Horizontal)
			{
				RemoveFromClassList("vertical");
				AddToClassList("horizontal");
			}
			else
			{
				RemoveFromClassList("horizontal");
				AddToClassList("vertical");
			}
		}
	}

	public event Action<float> valueChanged;

	public Slider(float start, float end, Action<float> valueChanged, Direction direction = Direction.Horizontal, float pageSize = 10f)
	{
		this.valueChanged = valueChanged;
		this.direction = direction;
		this.pageSize = pageSize;
		lowValue = start;
		highValue = end;
		Add(new VisualElement
		{
			name = "TrackElement"
		});
		dragElement = new VisualElement
		{
			name = "DragElement"
		};
		dragElement.RegisterCallback<PostLayoutEvent>(UpdateDragElementPosition);
		Add(dragElement);
		clampedDragger = new ClampedDragger(this, SetSliderValueFromClick, SetSliderValueFromDrag);
		this.AddManipulator(clampedDragger);
	}

	private void ClampValue()
	{
		value = value;
	}

	private void UpdateDragElementPosition(PostLayoutEvent evt)
	{
		if (!(evt.oldRect.size == evt.newRect.size))
		{
			UpdateDragElementPosition();
		}
	}

	/// <summary>
	///   <para>Called when the persistent data is accessible and/or when the data or persistence key have changed (VisualElement is properly parented).</para>
	/// </summary>
	public override void OnPersistentDataReady()
	{
		base.OnPersistentDataReady();
		string fullHierarchicalPersistenceKey = GetFullHierarchicalPersistenceKey();
		m_SliderValue = GetOrCreatePersistentData<SliderValue>(m_SliderValue, fullHierarchicalPersistenceKey);
	}

	private void SetSliderValueFromDrag()
	{
		if (clampedDragger.dragDirection == ClampedDragger.DragDirection.Free)
		{
			Vector2 delta = clampedDragger.delta;
			if (direction == Direction.Horizontal)
			{
				ComputeValueAndDirectionFromDrag(base.layout.width, dragElement.style.width, m_DragElementStartPos.x + delta.x);
			}
			else
			{
				ComputeValueAndDirectionFromDrag(base.layout.height, dragElement.style.height, m_DragElementStartPos.y + delta.y);
			}
		}
	}

	private void ComputeValueAndDirectionFromDrag(float sliderLength, float dragElementLength, float dragElementPos)
	{
		float num = sliderLength - dragElementLength;
		if (!(Mathf.Abs(num) < Mathf.Epsilon))
		{
			value = Mathf.Max(0f, Mathf.Min(dragElementPos, num)) / num * range + lowValue;
		}
	}

	private void SetSliderValueFromClick()
	{
		if (clampedDragger.dragDirection == ClampedDragger.DragDirection.Free)
		{
			return;
		}
		if (clampedDragger.dragDirection == ClampedDragger.DragDirection.None)
		{
			if (pageSize == 0f)
			{
				float num = ((direction != 0) ? dragElement.style.positionLeft.value : (clampedDragger.startMousePosition.x - (float)dragElement.style.width / 2f));
				float num2 = ((direction != 0) ? (clampedDragger.startMousePosition.y - (float)dragElement.style.height / 2f) : dragElement.style.positionTop.value);
				dragElement.style.positionLeft = num;
				dragElement.style.positionTop = num2;
				m_DragElementStartPos = new Rect(num, num2, dragElement.style.width, dragElement.style.height);
				clampedDragger.dragDirection = ClampedDragger.DragDirection.Free;
				if (direction == Direction.Horizontal)
				{
					ComputeValueAndDirectionFromDrag(base.layout.width, dragElement.style.width, m_DragElementStartPos.x);
				}
				else
				{
					ComputeValueAndDirectionFromDrag(base.layout.height, dragElement.style.height, m_DragElementStartPos.y);
				}
				return;
			}
			m_DragElementStartPos = new Rect(dragElement.style.positionLeft, dragElement.style.positionTop, dragElement.style.width, dragElement.style.height);
		}
		if (direction == Direction.Horizontal)
		{
			ComputeValueAndDirectionFromClick(base.layout.width, dragElement.style.width, dragElement.style.positionLeft, clampedDragger.lastMousePosition.x);
		}
		else
		{
			ComputeValueAndDirectionFromClick(base.layout.height, dragElement.style.height, dragElement.style.positionTop, clampedDragger.lastMousePosition.y);
		}
	}

	private void ComputeValueAndDirectionFromClick(float sliderLength, float dragElementLength, float dragElementPos, float dragElementLastPos)
	{
		float num = sliderLength - dragElementLength;
		if (!(Mathf.Abs(num) < Mathf.Epsilon))
		{
			if (dragElementLastPos < dragElementPos && clampedDragger.dragDirection != ClampedDragger.DragDirection.LowToHigh)
			{
				clampedDragger.dragDirection = ClampedDragger.DragDirection.HighToLow;
				value = Mathf.Max(0f, Mathf.Min(dragElementPos - pageSize, num)) / num * range + lowValue;
			}
			else if (dragElementLastPos > dragElementPos + dragElementLength && clampedDragger.dragDirection != ClampedDragger.DragDirection.HighToLow)
			{
				clampedDragger.dragDirection = ClampedDragger.DragDirection.LowToHigh;
				value = Mathf.Max(0f, Mathf.Min(dragElementPos + pageSize, num)) / num * range + lowValue;
			}
		}
	}

	public void AdjustDragElement(float factor)
	{
		bool flag = factor < 1f;
		dragElement.visible = flag;
		if (flag)
		{
			IStyle style = dragElement.style;
			dragElement.visible = true;
			if (direction == Direction.Horizontal)
			{
				float specifiedValueOrDefault = style.minWidth.GetSpecifiedValueOrDefault(0f);
				style.width = Mathf.Max(base.layout.width * factor, specifiedValueOrDefault);
			}
			else
			{
				float specifiedValueOrDefault2 = style.minHeight.GetSpecifiedValueOrDefault(0f);
				style.height = Mathf.Max(base.layout.height * factor, specifiedValueOrDefault2);
			}
		}
	}

	private void UpdateDragElementPosition()
	{
		if (base.panel != null)
		{
			float num = value - lowValue;
			float num2 = dragElement.style.width;
			float num3 = dragElement.style.height;
			if (direction == Direction.Horizontal)
			{
				float num4 = base.layout.width - num2;
				dragElement.style.positionLeft = num / range * num4;
			}
			else
			{
				float num5 = base.layout.height - num3;
				dragElement.style.positionTop = num / range * num5;
			}
		}
	}

	protected internal override void ExecuteDefaultAction(EventBase evt)
	{
		base.ExecuteDefaultAction(evt);
		if (evt.GetEventTypeId() == EventBase<PostLayoutEvent>.TypeId())
		{
			UpdateDragElementPosition((PostLayoutEvent)evt);
		}
	}
}
