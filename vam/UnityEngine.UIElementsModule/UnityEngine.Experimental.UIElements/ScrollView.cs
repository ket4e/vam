using System;

namespace UnityEngine.Experimental.UIElements;

public class ScrollView : VisualElement
{
	public static readonly Vector2 kDefaultScrollerValues = new Vector2(0f, 100f);

	private VisualElement m_ContentContainer;

	public Vector2 horizontalScrollerValues { get; set; }

	public Vector2 verticalScrollerValues { get; set; }

	public bool showHorizontal { get; set; }

	public bool showVertical { get; set; }

	public bool needsHorizontal => showHorizontal || contentContainer.layout.width - base.layout.width > 0f;

	public bool needsVertical => showVertical || contentContainer.layout.height - base.layout.height > 0f;

	public Vector2 scrollOffset
	{
		get
		{
			return new Vector2(horizontalScroller.value, verticalScroller.value);
		}
		set
		{
			if (value != scrollOffset)
			{
				horizontalScroller.value = value.x;
				verticalScroller.value = value.y;
				UpdateContentViewTransform();
			}
		}
	}

	private float scrollableWidth => contentContainer.layout.width - contentViewport.layout.width;

	private float scrollableHeight => contentContainer.layout.height - contentViewport.layout.height;

	public VisualElement contentViewport { get; private set; }

	[Obsolete("Please use contentContainer instead", false)]
	public VisualElement contentView => contentContainer;

	public Scroller horizontalScroller { get; private set; }

	public Scroller verticalScroller { get; private set; }

	public override VisualElement contentContainer => m_ContentContainer;

	public ScrollView()
		: this(kDefaultScrollerValues, kDefaultScrollerValues)
	{
	}

	public ScrollView(Vector2 horizontalScrollerValues, Vector2 verticalScrollerValues)
	{
		this.horizontalScrollerValues = horizontalScrollerValues;
		this.verticalScrollerValues = verticalScrollerValues;
		contentViewport = new VisualElement
		{
			name = "ContentViewport"
		};
		contentViewport.clippingOptions = ClippingOptions.ClipContents;
		base.shadow.Add(contentViewport);
		m_ContentContainer = new VisualElement
		{
			name = "ContentView"
		};
		contentViewport.Add(m_ContentContainer);
		horizontalScroller = new Scroller(horizontalScrollerValues.x, horizontalScrollerValues.y, delegate(float value)
		{
			scrollOffset = new Vector2(value, scrollOffset.y);
			UpdateContentViewTransform();
		}, Slider.Direction.Horizontal)
		{
			name = "HorizontalScroller",
			persistenceKey = "HorizontalScroller"
		};
		base.shadow.Add(horizontalScroller);
		verticalScroller = new Scroller(verticalScrollerValues.x, verticalScrollerValues.y, delegate(float value)
		{
			scrollOffset = new Vector2(scrollOffset.x, value);
			UpdateContentViewTransform();
		})
		{
			name = "VerticalScroller",
			persistenceKey = "VerticalScroller"
		};
		base.shadow.Add(verticalScroller);
		RegisterCallback<WheelEvent>(OnScrollWheel);
		contentContainer.RegisterCallback<PostLayoutEvent>(OnGeometryChanged);
	}

	private void UpdateContentViewTransform()
	{
		Vector3 position = contentContainer.transform.position;
		Vector2 vector = scrollOffset;
		position.x = 0f - vector.x;
		position.y = 0f - vector.y;
		contentContainer.transform.position = position;
		Dirty(ChangeType.Repaint);
	}

	/// <summary>
	///   <para>Scroll to a specific child element.</para>
	/// </summary>
	/// <param name="child">The child to scroll to.</param>
	public void ScrollTo(VisualElement child)
	{
		if (!contentContainer.Contains(child))
		{
			throw new ArgumentException("Cannot scroll to null child");
		}
		float num = contentContainer.layout.height - contentViewport.layout.height;
		float num2 = contentContainer.transform.position.y * -1f;
		float num3 = contentViewport.layout.yMin + num2;
		float num4 = contentViewport.layout.yMax + num2;
		float yMin = child.layout.yMin;
		float yMax = child.layout.yMax;
		if (!(yMin >= num3) || !(yMax <= num4))
		{
			bool flag = false;
			float num5 = yMax - num4;
			if (num5 < -1f)
			{
				num5 = num3 - yMin;
				flag = true;
			}
			float num6 = num5 * verticalScroller.highValue / num;
			scrollOffset.Set(scrollOffset.x, scrollOffset.y + ((!flag) ? num6 : (0f - num6)));
			verticalScroller.value = scrollOffset.y;
			UpdateContentViewTransform();
		}
	}

	protected internal override void ExecuteDefaultAction(EventBase evt)
	{
		base.ExecuteDefaultAction(evt);
		if (evt.GetEventTypeId() == EventBase<PostLayoutEvent>.TypeId())
		{
			OnGeometryChanged((PostLayoutEvent)evt);
		}
	}

	private void OnGeometryChanged(PostLayoutEvent evt)
	{
		if (evt.oldRect.size == evt.newRect.size)
		{
			return;
		}
		if (contentContainer.layout.width > Mathf.Epsilon)
		{
			horizontalScroller.Adjust(contentViewport.layout.width / contentContainer.layout.width);
		}
		if (contentContainer.layout.height > Mathf.Epsilon)
		{
			verticalScroller.Adjust(contentViewport.layout.height / contentContainer.layout.height);
		}
		horizontalScroller.SetEnabled(contentContainer.layout.width - base.layout.width > 0f);
		verticalScroller.SetEnabled(contentContainer.layout.height - base.layout.height > 0f);
		contentViewport.style.positionRight = ((!needsVertical) ? 0f : verticalScroller.layout.width);
		horizontalScroller.style.positionRight = ((!needsVertical) ? 0f : verticalScroller.layout.width);
		contentViewport.style.positionBottom = ((!needsHorizontal) ? 0f : horizontalScroller.layout.height);
		verticalScroller.style.positionBottom = ((!needsHorizontal) ? 0f : horizontalScroller.layout.height);
		if (needsHorizontal)
		{
			horizontalScroller.lowValue = 0f;
			horizontalScroller.highValue = scrollableWidth;
		}
		else
		{
			horizontalScroller.value = 0f;
		}
		if (needsVertical)
		{
			verticalScroller.lowValue = 0f;
			verticalScroller.highValue = scrollableHeight;
		}
		else
		{
			verticalScroller.value = 0f;
		}
		if (horizontalScroller.visible != needsHorizontal)
		{
			horizontalScroller.visible = needsHorizontal;
			if (needsHorizontal)
			{
				contentViewport.AddToClassList("HorizontalScroll");
			}
			else
			{
				contentViewport.RemoveFromClassList("HorizontalScroll");
			}
		}
		if (verticalScroller.visible != needsVertical)
		{
			verticalScroller.visible = needsVertical;
			if (needsVertical)
			{
				contentViewport.AddToClassList("VerticalScroll");
			}
			else
			{
				contentViewport.RemoveFromClassList("VerticalScroll");
			}
		}
		UpdateContentViewTransform();
	}

	private void OnScrollWheel(WheelEvent evt)
	{
		if (contentContainer.layout.height - base.layout.height > 0f)
		{
			if (evt.delta.y < 0f)
			{
				verticalScroller.ScrollPageUp();
			}
			else if (evt.delta.y > 0f)
			{
				verticalScroller.ScrollPageDown();
			}
		}
		evt.StopPropagation();
	}
}
