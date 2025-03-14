namespace System.Windows.Forms.CarbonInternal;

internal enum WindowAttributes : uint
{
	kWindowNoAttributes = 0u,
	kWindowCloseBoxAttribute = 1u,
	kWindowHorizontalZoomAttribute = 2u,
	kWindowVerticalZoomAttribute = 4u,
	kWindowFullZoomAttribute = 6u,
	kWindowCollapseBoxAttribute = 8u,
	kWindowResizableAttribute = 16u,
	kWindowSideTitlebarAttribute = 32u,
	kWindowToolbarButtonAttribute = 64u,
	kWindowMetalAttribute = 256u,
	kWindowNoUpdatesAttribute = 65536u,
	kWindowNoActivatesAttribute = 131072u,
	kWindowOpaqueForEventsAttribute = 262144u,
	kWindowCompositingAttribute = 524288u,
	kWindowNoShadowAttribute = 2097152u,
	kWindowHideOnSuspendAttribute = 16777216u,
	kWindowStandardHandlerAttribute = 33554432u,
	kWindowHideOnFullScreenAttribute = 67108864u,
	kWindowInWindowMenuAttribute = 134217728u,
	kWindowLiveResizeAttribute = 268435456u,
	kWindowIgnoreClicksAttribute = 536870912u,
	kWindowNoConstrainAttribute = 2147483648u,
	kWindowStandardDocumentAttributes = 31u,
	kWindowStandardFloatingAttributes = 9u
}
