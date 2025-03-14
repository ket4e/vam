using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.CSSLayout;

[NativeHeader("External/CSSLayout/CSSLayout/Native.bindings.h")]
internal static class Native
{
	private const string DllName = "CSSLayout";

	private static LockDictionary<IntPtr, WeakReference> s_MeasureFunctions = new LockDictionary<IntPtr, WeakReference>();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern IntPtr CSSNodeNew();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeInit(IntPtr cssNode);

	public static void CSSNodeFree(IntPtr cssNode)
	{
		if (!(cssNode == IntPtr.Zero))
		{
			CSSNodeSetMeasureFunc(cssNode, null);
			CSSNodeFreeInternal(cssNode);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "CSSNodeFree", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void CSSNodeFreeInternal(IntPtr cssNode);

	public static void CSSNodeReset(IntPtr cssNode)
	{
		CSSNodeSetMeasureFunc(cssNode, null);
		CSSNodeResetInternal(cssNode);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "CSSNodeReset", IsFreeFunction = true)]
	private static extern void CSSNodeResetInternal(IntPtr cssNode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern int CSSNodeGetInstanceCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSLayoutSetExperimentalFeatureEnabled(CSSExperimentalFeature feature, bool enabled);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern bool CSSLayoutIsExperimentalFeatureEnabled(CSSExperimentalFeature feature);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeInsertChild(IntPtr node, IntPtr child, uint index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeRemoveChild(IntPtr node, IntPtr child);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern IntPtr CSSNodeGetChild(IntPtr node, uint index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern uint CSSNodeChildCount(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeCalculateLayout(IntPtr node, float availableWidth, float availableHeight, CSSDirection parentDirection);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeMarkDirty(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern bool CSSNodeIsDirty(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodePrint(IntPtr node, CSSPrintOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern bool CSSValueIsUndefined(float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeCopyStyle(IntPtr dstNode, IntPtr srcNode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeSetContext(IntPtr node, IntPtr context);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern IntPtr CSSNodeGetContext(IntPtr node);

	public static void CSSNodeSetMeasureFunc(IntPtr node, CSSMeasureFunc measureFunc)
	{
		if (measureFunc != null)
		{
			s_MeasureFunctions.Set(node, new WeakReference(measureFunc));
			CSSLayoutCallbacks.RegisterWrapper(node);
		}
		else if (s_MeasureFunctions.ContainsKey(node))
		{
			s_MeasureFunctions.Remove(node);
			CSSLayoutCallbacks.UnegisterWrapper(node);
		}
	}

	public static CSSMeasureFunc CSSNodeGetMeasureFunc(IntPtr node)
	{
		WeakReference cacheItem = null;
		if (s_MeasureFunctions.TryGetValue(node, out cacheItem) && cacheItem.IsAlive)
		{
			return cacheItem.Target as CSSMeasureFunc;
		}
		return null;
	}

	[RequiredByNativeCode]
	public unsafe static void CSSNodeMeasureInvoke(IntPtr node, float width, CSSMeasureMode widthMode, float height, CSSMeasureMode heightMode, IntPtr returnValueAddress)
	{
		CSSMeasureFunc cSSMeasureFunc = CSSNodeGetMeasureFunc(node);
		if (cSSMeasureFunc != null)
		{
			*(CSSSize*)(void*)returnValueAddress = cSSMeasureFunc(node, width, widthMode, height, heightMode);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeSetHasNewLayout(IntPtr node, bool hasNewLayout);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern bool CSSNodeGetHasNewLayout(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetDirection(IntPtr node, CSSDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSDirection CSSNodeStyleGetDirection(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetFlexDirection(IntPtr node, CSSFlexDirection flexDirection);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSFlexDirection CSSNodeStyleGetFlexDirection(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetJustifyContent(IntPtr node, CSSJustify justifyContent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSJustify CSSNodeStyleGetJustifyContent(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetAlignContent(IntPtr node, CSSAlign alignContent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSAlign CSSNodeStyleGetAlignContent(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetAlignItems(IntPtr node, CSSAlign alignItems);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSAlign CSSNodeStyleGetAlignItems(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetAlignSelf(IntPtr node, CSSAlign alignSelf);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSAlign CSSNodeStyleGetAlignSelf(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetPositionType(IntPtr node, CSSPositionType positionType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSPositionType CSSNodeStyleGetPositionType(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetFlexWrap(IntPtr node, CSSWrap flexWrap);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSWrap CSSNodeStyleGetFlexWrap(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetOverflow(IntPtr node, CSSOverflow flexWrap);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSOverflow CSSNodeStyleGetOverflow(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetFlex(IntPtr node, float flex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetFlexGrow(IntPtr node, float flexGrow);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetFlexGrow(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetFlexShrink(IntPtr node, float flexShrink);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetFlexShrink(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetFlexBasis(IntPtr node, float flexBasis);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetFlexBasis(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetWidth(IntPtr node, float width);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetWidth(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetHeight(IntPtr node, float height);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetHeight(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetMinWidth(IntPtr node, float minWidth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetMinWidth(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetMinHeight(IntPtr node, float minHeight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetMinHeight(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetMaxWidth(IntPtr node, float maxWidth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetMaxWidth(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetMaxHeight(IntPtr node, float maxHeight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetMaxHeight(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetAspectRatio(IntPtr node, float aspectRatio);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetAspectRatio(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetPosition(IntPtr node, CSSEdge edge, float position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetPosition(IntPtr node, CSSEdge edge);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetMargin(IntPtr node, CSSEdge edge, float margin);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetMargin(IntPtr node, CSSEdge edge);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetPadding(IntPtr node, CSSEdge edge, float padding);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetPadding(IntPtr node, CSSEdge edge);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void CSSNodeStyleSetBorder(IntPtr node, CSSEdge edge, float border);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeStyleGetBorder(IntPtr node, CSSEdge edge);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeLayoutGetLeft(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeLayoutGetTop(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeLayoutGetRight(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeLayoutGetBottom(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeLayoutGetWidth(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern float CSSNodeLayoutGetHeight(IntPtr node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern CSSDirection CSSNodeLayoutGetDirection(IntPtr node);
}
