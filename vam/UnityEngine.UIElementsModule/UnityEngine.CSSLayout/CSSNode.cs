using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.CSSLayout;

internal class CSSNode : IEnumerable<CSSNode>, IEnumerable
{
	private IntPtr _cssNode;

	private WeakReference _parent;

	private List<CSSNode> _children;

	private MeasureFunction _measureFunction;

	private CSSMeasureFunc _cssMeasureFunc;

	private object _data;

	public bool IsDirty => Native.CSSNodeIsDirty(_cssNode);

	public bool HasNewLayout => Native.CSSNodeGetHasNewLayout(_cssNode);

	public CSSNode Parent => (_parent == null) ? null : (_parent.Target as CSSNode);

	public bool IsMeasureDefined => _measureFunction != null;

	public CSSDirection StyleDirection
	{
		get
		{
			return Native.CSSNodeStyleGetDirection(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetDirection(_cssNode, value);
		}
	}

	public CSSFlexDirection FlexDirection
	{
		get
		{
			return Native.CSSNodeStyleGetFlexDirection(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetFlexDirection(_cssNode, value);
		}
	}

	public CSSJustify JustifyContent
	{
		get
		{
			return Native.CSSNodeStyleGetJustifyContent(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetJustifyContent(_cssNode, value);
		}
	}

	public CSSAlign AlignItems
	{
		get
		{
			return Native.CSSNodeStyleGetAlignItems(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetAlignItems(_cssNode, value);
		}
	}

	public CSSAlign AlignSelf
	{
		get
		{
			return Native.CSSNodeStyleGetAlignSelf(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetAlignSelf(_cssNode, value);
		}
	}

	public CSSAlign AlignContent
	{
		get
		{
			return Native.CSSNodeStyleGetAlignContent(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetAlignContent(_cssNode, value);
		}
	}

	public CSSPositionType PositionType
	{
		get
		{
			return Native.CSSNodeStyleGetPositionType(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetPositionType(_cssNode, value);
		}
	}

	public CSSWrap Wrap
	{
		get
		{
			return Native.CSSNodeStyleGetFlexWrap(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetFlexWrap(_cssNode, value);
		}
	}

	public float Flex
	{
		set
		{
			Native.CSSNodeStyleSetFlex(_cssNode, value);
		}
	}

	public float FlexGrow
	{
		get
		{
			return Native.CSSNodeStyleGetFlexGrow(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetFlexGrow(_cssNode, value);
		}
	}

	public float FlexShrink
	{
		get
		{
			return Native.CSSNodeStyleGetFlexShrink(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetFlexShrink(_cssNode, value);
		}
	}

	public float FlexBasis
	{
		get
		{
			return Native.CSSNodeStyleGetFlexBasis(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetFlexBasis(_cssNode, value);
		}
	}

	public float Width
	{
		get
		{
			return Native.CSSNodeStyleGetWidth(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetWidth(_cssNode, value);
		}
	}

	public float Height
	{
		get
		{
			return Native.CSSNodeStyleGetHeight(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetHeight(_cssNode, value);
		}
	}

	public float MaxWidth
	{
		get
		{
			return Native.CSSNodeStyleGetMaxWidth(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetMaxWidth(_cssNode, value);
		}
	}

	public float MaxHeight
	{
		get
		{
			return Native.CSSNodeStyleGetMaxHeight(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetMaxHeight(_cssNode, value);
		}
	}

	public float MinWidth
	{
		get
		{
			return Native.CSSNodeStyleGetMinWidth(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetMinWidth(_cssNode, value);
		}
	}

	public float MinHeight
	{
		get
		{
			return Native.CSSNodeStyleGetMinHeight(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetMinHeight(_cssNode, value);
		}
	}

	public float AspectRatio
	{
		get
		{
			return Native.CSSNodeStyleGetAspectRatio(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetAspectRatio(_cssNode, value);
		}
	}

	public float LayoutX => Native.CSSNodeLayoutGetLeft(_cssNode);

	public float LayoutY => Native.CSSNodeLayoutGetTop(_cssNode);

	public float LayoutWidth => Native.CSSNodeLayoutGetWidth(_cssNode);

	public float LayoutHeight => Native.CSSNodeLayoutGetHeight(_cssNode);

	public CSSDirection LayoutDirection => Native.CSSNodeLayoutGetDirection(_cssNode);

	public CSSOverflow Overflow
	{
		get
		{
			return Native.CSSNodeStyleGetOverflow(_cssNode);
		}
		set
		{
			Native.CSSNodeStyleSetOverflow(_cssNode, value);
		}
	}

	public object Data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
		}
	}

	public CSSNode this[int index] => _children[index];

	public int Count => (_children != null) ? _children.Count : 0;

	public CSSNode()
	{
		CSSLogger.Initialize();
		_cssNode = Native.CSSNodeNew();
		if (_cssNode == IntPtr.Zero)
		{
			throw new InvalidOperationException("Failed to allocate native memory");
		}
	}

	~CSSNode()
	{
		Native.CSSNodeFree(_cssNode);
	}

	public void Reset()
	{
		_measureFunction = null;
		_data = null;
		Native.CSSNodeReset(_cssNode);
	}

	public virtual void MarkDirty()
	{
		Native.CSSNodeMarkDirty(_cssNode);
	}

	public void MarkHasNewLayout()
	{
		Native.CSSNodeSetHasNewLayout(_cssNode, hasNewLayout: true);
	}

	public void CopyStyle(CSSNode srcNode)
	{
		Native.CSSNodeCopyStyle(_cssNode, srcNode._cssNode);
	}

	public float GetMargin(CSSEdge edge)
	{
		return Native.CSSNodeStyleGetMargin(_cssNode, edge);
	}

	public void SetMargin(CSSEdge edge, float value)
	{
		Native.CSSNodeStyleSetMargin(_cssNode, edge, value);
	}

	public float GetPadding(CSSEdge edge)
	{
		return Native.CSSNodeStyleGetPadding(_cssNode, edge);
	}

	public void SetPadding(CSSEdge edge, float padding)
	{
		Native.CSSNodeStyleSetPadding(_cssNode, edge, padding);
	}

	public float GetBorder(CSSEdge edge)
	{
		return Native.CSSNodeStyleGetBorder(_cssNode, edge);
	}

	public void SetBorder(CSSEdge edge, float border)
	{
		Native.CSSNodeStyleSetBorder(_cssNode, edge, border);
	}

	public float GetPosition(CSSEdge edge)
	{
		return Native.CSSNodeStyleGetPosition(_cssNode, edge);
	}

	public void SetPosition(CSSEdge edge, float position)
	{
		Native.CSSNodeStyleSetPosition(_cssNode, edge, position);
	}

	public void MarkLayoutSeen()
	{
		Native.CSSNodeSetHasNewLayout(_cssNode, hasNewLayout: false);
	}

	public bool ValuesEqual(float f1, float f2)
	{
		if (float.IsNaN(f1) || float.IsNaN(f2))
		{
			return float.IsNaN(f1) && float.IsNaN(f2);
		}
		return Math.Abs(f2 - f1) < float.Epsilon;
	}

	public void Insert(int index, CSSNode node)
	{
		if (_children == null)
		{
			_children = new List<CSSNode>(4);
		}
		_children.Insert(index, node);
		node._parent = new WeakReference(this);
		Native.CSSNodeInsertChild(_cssNode, node._cssNode, (uint)index);
	}

	public void RemoveAt(int index)
	{
		CSSNode cSSNode = _children[index];
		cSSNode._parent = null;
		_children.RemoveAt(index);
		Native.CSSNodeRemoveChild(_cssNode, cSSNode._cssNode);
	}

	public void Clear()
	{
		if (_children != null)
		{
			while (_children.Count > 0)
			{
				RemoveAt(_children.Count - 1);
			}
		}
	}

	public int IndexOf(CSSNode node)
	{
		return (_children == null) ? (-1) : _children.IndexOf(node);
	}

	public void SetMeasureFunction(MeasureFunction measureFunction)
	{
		_measureFunction = measureFunction;
		_cssMeasureFunc = ((measureFunction == null) ? null : new CSSMeasureFunc(MeasureInternal));
		Native.CSSNodeSetMeasureFunc(_cssNode, _cssMeasureFunc);
	}

	public void CalculateLayout()
	{
		Native.CSSNodeCalculateLayout(_cssNode, float.NaN, float.NaN, Native.CSSNodeStyleGetDirection(_cssNode));
	}

	private CSSSize MeasureInternal(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode)
	{
		if (_measureFunction == null)
		{
			throw new InvalidOperationException("Measure function is not defined.");
		}
		long measureOutput = _measureFunction(this, width, widthMode, height, heightMode);
		CSSSize result = default(CSSSize);
		result.width = MeasureOutput.GetWidth(measureOutput);
		result.height = MeasureOutput.GetHeight(measureOutput);
		return result;
	}

	public string Print()
	{
		return Print((CSSPrintOptions)7);
	}

	public string Print(CSSPrintOptions options)
	{
		StringBuilder sb = new StringBuilder();
		CSSLogger.Func logger = CSSLogger.Logger;
		CSSLogger.Logger = delegate(CSSLogLevel level, string message)
		{
			sb.Append(message);
		};
		Native.CSSNodePrint(_cssNode, options);
		CSSLogger.Logger = logger;
		return sb.ToString();
	}

	public IEnumerator<CSSNode> GetEnumerator()
	{
		return (_children == null) ? Enumerable.Empty<CSSNode>().GetEnumerator() : ((IEnumerable<CSSNode>)_children).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return (_children == null) ? Enumerable.Empty<CSSNode>().GetEnumerator() : ((IEnumerable<CSSNode>)_children).GetEnumerator();
	}

	public static int GetInstanceCount()
	{
		return Native.CSSNodeGetInstanceCount();
	}

	public static void SetExperimentalFeatureEnabled(CSSExperimentalFeature feature, bool enabled)
	{
		Native.CSSLayoutSetExperimentalFeatureEnabled(feature, enabled);
	}

	public static bool IsExperimentalFeatureEnabled(CSSExperimentalFeature feature)
	{
		return Native.CSSLayoutIsExperimentalFeatureEnabled(feature);
	}
}
