namespace System.Windows.Forms.VisualStyles;

[Flags]
public enum HitTestOptions
{
	BackgroundSegment = 0,
	FixedBorder = 2,
	Caption = 4,
	ResizingBorderLeft = 0x10,
	ResizingBorderTop = 0x20,
	ResizingBorderRight = 0x40,
	ResizingBorderBottom = 0x80,
	ResizingBorder = 0xF0,
	SizingTemplate = 0x100,
	SystemSizingMargins = 0x200
}
