using System;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace UnityEngine.Experimental.UIElements;

public class Scroller : VisualElement
{
	public Slider slider { get; private set; }

	public ScrollerButton lowButton { get; private set; }

	public ScrollerButton highButton { get; private set; }

	public float value
	{
		get
		{
			return slider.value;
		}
		set
		{
			slider.value = value;
		}
	}

	public float lowValue
	{
		get
		{
			return slider.lowValue;
		}
		set
		{
			slider.lowValue = value;
		}
	}

	public float highValue
	{
		get
		{
			return slider.highValue;
		}
		set
		{
			slider.highValue = value;
		}
	}

	public Slider.Direction direction
	{
		get
		{
			return ((FlexDirection)base.style.flexDirection != FlexDirection.Row) ? Slider.Direction.Vertical : Slider.Direction.Horizontal;
		}
		set
		{
			if (value == Slider.Direction.Horizontal)
			{
				base.style.flexDirection = FlexDirection.Row;
				AddToClassList("horizontal");
			}
			else
			{
				base.style.flexDirection = FlexDirection.Column;
				AddToClassList("vertical");
			}
		}
	}

	public event Action<float> valueChanged;

	public Scroller(float lowValue, float highValue, Action<float> valueChanged, Slider.Direction direction = Slider.Direction.Vertical)
	{
		this.direction = direction;
		this.valueChanged = valueChanged;
		slider = new Slider(lowValue, highValue, OnSliderValueChange, direction)
		{
			name = "Slider",
			persistenceKey = "Slider"
		};
		Add(slider);
		lowButton = new ScrollerButton(ScrollPageUp, 250L, 30L)
		{
			name = "LowButton"
		};
		Add(lowButton);
		highButton = new ScrollerButton(ScrollPageDown, 250L, 30L)
		{
			name = "HighButton"
		};
		Add(highButton);
	}

	public void Adjust(float factor)
	{
		SetEnabled(factor < 1f);
		slider.AdjustDragElement(factor);
	}

	private void OnSliderValueChange(float newValue)
	{
		value = newValue;
		if (this.valueChanged != null)
		{
			this.valueChanged(slider.value);
		}
		Dirty(ChangeType.Repaint);
	}

	public void ScrollPageUp()
	{
		value -= slider.pageSize * ((!(slider.lowValue < slider.highValue)) ? (-1f) : 1f);
	}

	public void ScrollPageDown()
	{
		value += slider.pageSize * ((!(slider.lowValue < slider.highValue)) ? (-1f) : 1f);
	}
}
