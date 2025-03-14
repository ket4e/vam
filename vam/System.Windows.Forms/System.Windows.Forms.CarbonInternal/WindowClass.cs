namespace System.Windows.Forms.CarbonInternal;

internal enum WindowClass : uint
{
	kAlertWindowClass = 1u,
	kMovableAlertWindowClass = 2u,
	kModalWindowClass = 3u,
	kMovableModalWindowClass = 4u,
	kFloatingWindowClass = 5u,
	kDocumentWindowClass = 6u,
	kUtilityWindowClass = 8u,
	kHelpWindowClass = 10u,
	kSheetWindowClass = 11u,
	kToolbarWindowClass = 12u,
	kPlainWindowClass = 13u,
	kOverlayWindowClass = 14u,
	kSheetAlertWindowClass = 15u,
	kAltPlainWindowClass = 16u,
	kDrawerWindowClass = 20u,
	kAllWindowClasses = uint.MaxValue
}
