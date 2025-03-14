using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.VisualStyles;

internal class GtkPlus
{
	private abstract class Painter
	{
		public virtual void AttachStyle(WidgetType widgetType, IntPtr drawable, GtkPlus gtkPlus)
		{
			ref IntPtr reference = ref gtkPlus.styles[(int)widgetType];
			reference = gtk_style_attach(gtkPlus.styles[(int)widgetType], drawable);
		}

		public abstract void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus);
	}

	private enum TransparencyType
	{
		None,
		Color,
		Alpha
	}

	private enum DeviceContextType
	{
		Unknown,
		Graphics,
		Native
	}

	private class ButtonPainter : Painter
	{
		private bool @default;

		private GtkPlusState state;

		public void Configure(bool @default, GtkPlusState state)
		{
			this.@default = @default;
			this.state = state;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			if (@default)
			{
				gtk_window_set_default(gtkPlus.window, widget);
				gtk_paint_box(style, window, GtkStateType.GTK_STATE_NORMAL, GtkShadowType.GTK_SHADOW_IN, ref area, widget, "buttondefault", x, y, width, height);
				gtk_window_set_default(gtkPlus.window, IntPtr.Zero);
			}
			else
			{
				gtk_paint_box(style, window, (GtkStateType)state, (state == GtkPlusState.Pressed) ? GtkShadowType.GTK_SHADOW_IN : GtkShadowType.GTK_SHADOW_OUT, ref area, widget, "button", x, y, width, height);
			}
		}
	}

	private abstract class ToggleButtonPainter : Painter
	{
		private GtkPlusState state;

		private GtkPlusToggleButtonValue value;

		protected abstract string Detail { get; }

		protected abstract ToggleButtonPaintFunction PaintFunction { get; }

		public void Configure(GtkPlusState state, GtkPlusToggleButtonValue value)
		{
			this.state = state;
			this.value = value;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			PaintFunction(style, window, (GtkStateType)state, (GtkShadowType)value, ref area, widget, Detail, x, y, width, height);
		}
	}

	private class CheckBoxPainter : ToggleButtonPainter
	{
		protected override string Detail => "checkbutton";

		protected override ToggleButtonPaintFunction PaintFunction => gtk_paint_check;
	}

	private class RadioButtonPainter : ToggleButtonPainter
	{
		protected override string Detail => "radiobutton";

		protected override ToggleButtonPaintFunction PaintFunction => gtk_paint_option;
	}

	private class ComboBoxDropDownButtonPainter : Painter
	{
		private GtkPlusState state;

		public void Configure(GtkPlusState state)
		{
			this.state = state;
		}

		public override void AttachStyle(WidgetType widgetType, IntPtr drawable, GtkPlus gtkPlus)
		{
			gtkPlus.combo_box_drop_down_toggle_button_style = gtk_style_attach(gtkPlus.combo_box_drop_down_toggle_button_style, drawable);
			gtkPlus.combo_box_drop_down_arrow_style = gtk_style_attach(gtkPlus.combo_box_drop_down_arrow_style, drawable);
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(shadow_type: state switch
			{
				GtkPlusState.Disabled => GtkShadowType.GTK_SHADOW_ETCHED_IN, 
				GtkPlusState.Pressed => GtkShadowType.GTK_SHADOW_IN, 
				_ => GtkShadowType.GTK_SHADOW_OUT, 
			}, style: gtkPlus.combo_box_drop_down_toggle_button_style, window: window, state_type: (GtkStateType)state, area: ref area, widget: gtkPlus.combo_box_drop_down_toggle_button, detail: "button", x: x, y: y, width: width, height: height);
			GtkMisc gtkMisc = (GtkMisc)Marshal.PtrToStructure(gtkPlus.combo_box_drop_down_arrow, typeof(GtkMisc));
			int num = (int)((float)Math.Min(width - gtkMisc.xpad * 2, height - gtkMisc.ypad * 2) * GetWidgetStyleSingle(gtkPlus.combo_box_drop_down_arrow, "arrow-scaling"));
			gtk_paint_arrow(gtkPlus.combo_box_drop_down_arrow_style, window, (GtkStateType)state, GtkShadowType.GTK_SHADOW_NONE, ref area, gtkPlus.combo_box_drop_down_arrow, "arrow", GtkArrowType.GTK_ARROW_DOWN, fill: true, (int)Math.Floor((float)(x + gtkMisc.xpad) + (float)(width - num) * gtkMisc.xalign), (int)Math.Floor((float)(y + gtkMisc.ypad) + (float)(height - num) * gtkMisc.yalign), num, num);
		}
	}

	private class ComboBoxBorderPainter : Painter
	{
		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_shadow(style, window, GtkStateType.GTK_STATE_NORMAL, GtkShadowType.GTK_SHADOW_IN, ref area, widget, "combobox", x, y, width, height);
		}
	}

	private class GroupBoxPainter : Painter
	{
		private GtkPlusState state;

		public void Configure(GtkPlusState state)
		{
			this.state = state;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_shadow(style, window, (GtkStateType)state, GtkShadowType.GTK_SHADOW_ETCHED_IN, ref area, widget, "frame", x, y, width, height);
		}
	}

	private class HeaderPainter : Painter
	{
		private GtkPlusState state;

		public void Configure(GtkPlusState state)
		{
			this.state = state;
		}

		public override void AttachStyle(WidgetType widgetType, IntPtr drawable, GtkPlus gtkPlus)
		{
			gtkPlus.tree_view_column_button_style = gtk_style_attach(gtkPlus.tree_view_column_button_style, drawable);
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(gtkPlus.tree_view_column_button_style, window, (GtkStateType)state, (state == GtkPlusState.Pressed) ? GtkShadowType.GTK_SHADOW_IN : GtkShadowType.GTK_SHADOW_OUT, ref area, gtkPlus.tree_view_column_button, "button", x, y, width, height);
		}
	}

	private class ProgressBarBarPainter : Painter
	{
		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(style, window, GtkStateType.GTK_STATE_NORMAL, GtkShadowType.GTK_SHADOW_IN, ref area, widget, "trough", x, y, width, height);
		}
	}

	private class ProgressBarChunkPainter : Painter
	{
		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(style, window, GtkStateType.GTK_STATE_PRELIGHT, GtkShadowType.GTK_SHADOW_OUT, ref area, widget, "bar", x, y, width, height);
		}
	}

	private class ScrollBarArrowButtonPainter : Painter
	{
		private GtkPlusState state;

		private bool horizontal;

		private bool up_or_left;

		public void Configure(GtkPlusState state, bool horizontal, bool upOrLeft)
		{
			this.state = state;
			this.horizontal = horizontal;
			up_or_left = upOrLeft;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			g_object_get(widget, "can-focus", out var value, IntPtr.Zero);
			if (value)
			{
				gtk_widget_style_get(widget, "focus-line-width", out var value2, "focus-padding", out var value3, IntPtr.Zero);
				int num = value2 + value3;
				if (horizontal)
				{
					y -= num;
					height -= 2 * num;
				}
				else
				{
					x -= num;
					width -= 2 * num;
				}
			}
			GtkShadowType shadow_type = ((state == GtkPlusState.Pressed) ? GtkShadowType.GTK_SHADOW_IN : GtkShadowType.GTK_SHADOW_OUT);
			string detail = ((!horizontal) ? "vscrollbar" : "hscrollbar");
			gtk_paint_box(style, window, (GtkStateType)state, shadow_type, ref area, widget, detail, x, y, width, height);
			width /= 2;
			height /= 2;
			x += width / 2;
			y += height / 2;
			if (state == GtkPlusState.Pressed)
			{
				gtk_widget_style_get(widget, "arrow-displacement-x", out var value4, "arrow-displacement-y", out var value5, IntPtr.Zero);
				x += value4;
				y += value5;
			}
			gtk_paint_arrow(style, window, (GtkStateType)state, shadow_type, ref area, widget, detail, horizontal ? ((!up_or_left) ? GtkArrowType.GTK_ARROW_RIGHT : GtkArrowType.GTK_ARROW_LEFT) : ((!up_or_left) ? GtkArrowType.GTK_ARROW_DOWN : GtkArrowType.GTK_ARROW_UP), fill: true, x, y, width, height);
		}
	}

	private abstract class RangeThumbButtonPainter : Painter
	{
		private GtkPlusState state;

		private bool horizontal;

		protected bool Horizontal => horizontal;

		protected abstract string Detail { get; }

		public void Configure(GtkPlusState state, bool horizontal)
		{
			this.state = state;
			this.horizontal = horizontal;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_slider(style, window, (GtkStateType)state, (state == GtkPlusState.Pressed && GetWidgetStyleBoolean(widget, "activate-slider")) ? GtkShadowType.GTK_SHADOW_IN : GtkShadowType.GTK_SHADOW_OUT, ref area, widget, Detail, x, y, width, height, (!horizontal) ? GtkOrientation.GTK_ORIENTATION_VERTICAL : GtkOrientation.GTK_ORIENTATION_HORIZONTAL);
		}
	}

	private class ScrollBarThumbButtonPainter : RangeThumbButtonPainter
	{
		protected override string Detail => "slider";
	}

	private class ScrollBarTrackPainter : Painter
	{
		private GtkPlusState state;

		private bool up_or_left;

		public void Configure(GtkPlusState state, bool upOrLeft)
		{
			this.state = state;
			up_or_left = upOrLeft;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(style, window, (state == GtkPlusState.Pressed) ? GtkStateType.GTK_STATE_ACTIVE : GtkStateType.GTK_STATE_INSENSITIVE, GtkShadowType.GTK_SHADOW_IN, ref area, widget, (!GetWidgetStyleBoolean(widget, "trough-side-details")) ? "trough" : ((!up_or_left) ? "trough-lower" : "trough-upper"), x, y, width, height);
		}
	}

	private class StatusBarGripperPainter : Painter
	{
		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_resize_grip(style, window, GtkStateType.GTK_STATE_NORMAL, ref area, widget, "statusbar", GdkWindowEdge.GDK_WINDOW_EDGE_SOUTH_EAST, x, y, width, height);
		}
	}

	private class TabControlPanePainter : Painter
	{
		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box_gap(style, window, GtkStateType.GTK_STATE_NORMAL, GtkShadowType.GTK_SHADOW_OUT, ref area, widget, "notebook", x, y, width, height, GtkPositionType.GTK_POS_TOP, 0, 0);
		}
	}

	private class TabControlTabItemPainter : Painter
	{
		private GtkPlusState state;

		public void Configure(GtkPlusState state)
		{
			this.state = state;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_extension(style, window, (GtkStateType)state, GtkShadowType.GTK_SHADOW_OUT, ref area, widget, "tab", x, y, width, height, GtkPositionType.GTK_POS_BOTTOM);
		}
	}

	private class TextBoxPainter : Painter
	{
		private GtkPlusState state;

		public void Configure(GtkPlusState state)
		{
			this.state = state;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_shadow(style, window, GtkStateType.GTK_STATE_NORMAL, GtkShadowType.GTK_SHADOW_IN, ref area, widget, "entry", x, y, width, height);
			GtkStyle gtkStyle = (GtkStyle)Marshal.PtrToStructure(style, typeof(GtkStyle));
			x += gtkStyle.xthickness;
			y += gtkStyle.ythickness;
			width -= 2 * gtkStyle.xthickness;
			height -= 2 * gtkStyle.ythickness;
			gtk_paint_flat_box(style, window, (GtkStateType)state, GtkShadowType.GTK_SHADOW_NONE, ref area, widget, "entry_bg", x, y, width, height);
		}
	}

	private class ToolBarPainter : Painter
	{
		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(style, window, GtkStateType.GTK_STATE_NORMAL, GetWidgetStyleShadowType(widget), ref area, widget, "toolbar", x, y, width, height);
		}
	}

	private class ToolBarButtonPainter : Painter
	{
		private GtkPlusState state;

		public void Configure(GtkPlusState state)
		{
			this.state = state;
		}

		public override void AttachStyle(WidgetType widgetType, IntPtr drawable, GtkPlus gtkPlus)
		{
			gtkPlus.tool_bar_button_style = gtk_style_attach(gtkPlus.tool_bar_button_style, drawable);
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(gtkPlus.tool_bar_button_style, window, (GtkStateType)state, (state == GtkPlusState.Pressed) ? GtkShadowType.GTK_SHADOW_IN : GtkShadowType.GTK_SHADOW_OUT, ref area, gtkPlus.tool_bar_button, "button", x, y, width, height);
		}
	}

	private class ToolBarCheckedButtonPainter : Painter
	{
		public override void AttachStyle(WidgetType widgetType, IntPtr drawable, GtkPlus gtkPlus)
		{
			gtkPlus.tool_bar_toggle_button_style = gtk_style_attach(gtkPlus.tool_bar_toggle_button_style, drawable);
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(gtkPlus.tool_bar_toggle_button_style, window, GtkStateType.GTK_STATE_ACTIVE, GtkShadowType.GTK_SHADOW_IN, ref area, gtkPlus.tool_bar_toggle_button, "button", x, y, width, height);
		}
	}

	private class TrackBarTrackPainter : Painter
	{
		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_box(style, window, GtkStateType.GTK_STATE_ACTIVE, GtkShadowType.GTK_SHADOW_IN, ref area, widget, "trough", x, y, width, height);
		}
	}

	private class TrackBarThumbPainter : RangeThumbButtonPainter
	{
		protected override string Detail => (!base.Horizontal) ? "vscale" : "hscale";
	}

	private class TreeViewGlyphPainter : Painter
	{
		private bool closed;

		public void Configure(bool closed)
		{
			this.closed = closed;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			gtk_paint_expander(style, window, GtkStateType.GTK_STATE_NORMAL, ref area, widget, "treeview", x + width / 2, y + height / 2, (!closed) ? GtkExpanderStyle.GTK_EXPANDER_EXPANDED : GtkExpanderStyle.GTK_EXPANDER_COLLAPSED);
		}
	}

	private class UpDownPainter : Painter
	{
		private bool up;

		private GtkPlusState state;

		public void Configure(bool up, GtkPlusState state)
		{
			this.up = up;
			this.state = state;
		}

		public override void Paint(IntPtr style, IntPtr window, GdkRectangle area, IntPtr widget, int x, int y, int width, int height, GtkPlus gtkPlus)
		{
			GtkShadowType widgetStyleShadowType = GetWidgetStyleShadowType(widget);
			if (widgetStyleShadowType != 0)
			{
				gtk_paint_box(style, window, GtkStateType.GTK_STATE_NORMAL, widgetStyleShadowType, ref area, widget, "spinbutton", x, y - ((!up) ? height : 0), width, height * 2);
			}
			widgetStyleShadowType = ((state == GtkPlusState.Pressed) ? GtkShadowType.GTK_SHADOW_IN : GtkShadowType.GTK_SHADOW_OUT);
			gtk_paint_box(style, window, (GtkStateType)state, widgetStyleShadowType, ref area, widget, (!up) ? "spinbutton_down" : "spinbutton_up", x, y, width, height);
			if (up)
			{
				y += 2;
			}
			height -= 2;
			width -= 3;
			x++;
			int num = width / 2;
			num -= num % 2 - 1;
			int num2 = (num + 1) / 2;
			x += (width - num) / 2;
			y += (height - num2) / 2;
			height = num2;
			width = num;
			gtk_paint_arrow(style, window, (GtkStateType)state, widgetStyleShadowType, ref area, widget, "spinbutton", (!up) ? GtkArrowType.GTK_ARROW_DOWN : GtkArrowType.GTK_ARROW_UP, fill: true, x, y, width, height);
		}
	}

	private enum WidgetType
	{
		Button,
		CheckBox,
		ComboBox,
		GroupBox,
		ProgressBar,
		RadioButton,
		HScrollBar,
		VScrollBar,
		StatusBar,
		TabControl,
		TextBox,
		ToolBar,
		HorizontalTrackBar,
		VerticalTrackBar,
		TreeView,
		UpDown
	}

	private static class GetFirstChildWidgetOfType
	{
		private static IntPtr Type;

		private static IntPtr Result;

		private static ArrayList ContainersToSearch;

		public static IntPtr Get(IntPtr parent, IntPtr childType)
		{
			Type = childType;
			Result = IntPtr.Zero;
			ContainersToSearch = new ArrayList();
			ContainersToSearch.Add(parent);
			do
			{
				ArrayList containersToSearch = ContainersToSearch;
				ContainersToSearch = new ArrayList();
				foreach (IntPtr item in containersToSearch)
				{
					gtk_widget_realize(item);
					gtk_container_forall(item, Callback, IntPtr.Zero);
					if (Result != IntPtr.Zero)
					{
						return Result;
					}
				}
			}
			while (ContainersToSearch.Count != 0);
			return IntPtr.Zero;
		}

		private static void Callback(IntPtr widget, IntPtr data)
		{
			if (!(Result != IntPtr.Zero))
			{
				if (g_type_check_instance_is_a(widget, Type))
				{
					Result = widget;
				}
				else if (g_type_check_instance_is_a(widget, gtk_container_get_type()))
				{
					ContainersToSearch.Add(widget);
				}
			}
		}
	}

	private struct GdkColor
	{
		public uint pixel;

		public ushort red;

		public ushort green;

		public ushort blue;

		public GdkColor(Color value)
		{
			pixel = 0u;
			red = (ushort)(value.R << 8);
			green = (ushort)(value.G << 8);
			blue = (ushort)(value.B << 8);
		}
	}

	internal struct GdkRectangle
	{
		public int x;

		public int y;

		public int width;

		public int height;

		public GdkRectangle(Rectangle value)
		{
			x = value.X;
			y = value.Y;
			width = value.Width;
			height = value.Height;
		}
	}

	private enum GdkColorspace
	{
		GDK_COLORSPACE_RGB
	}

	internal enum GtkShadowType
	{
		GTK_SHADOW_NONE,
		GTK_SHADOW_IN,
		GTK_SHADOW_OUT,
		GTK_SHADOW_ETCHED_IN,
		GTK_SHADOW_ETCHED_OUT
	}

	private enum GtkStateType
	{
		GTK_STATE_NORMAL,
		GTK_STATE_ACTIVE,
		GTK_STATE_PRELIGHT,
		GTK_STATE_SELECTED,
		GTK_STATE_INSENSITIVE
	}

	private enum GtkWindowType
	{
		GTK_WINDOW_TOPLEVEL,
		GTK_WINDOW_POPUP
	}

	private enum GtkArrowType
	{
		GTK_ARROW_UP,
		GTK_ARROW_DOWN,
		GTK_ARROW_LEFT,
		GTK_ARROW_RIGHT,
		GTK_ARROW_NONE
	}

	private enum GtkOrientation
	{
		GTK_ORIENTATION_HORIZONTAL,
		GTK_ORIENTATION_VERTICAL
	}

	private enum GtkExpanderStyle
	{
		GTK_EXPANDER_COLLAPSED,
		GTK_EXPANDER_SEMI_COLLAPSED,
		GTK_EXPANDER_SEMI_EXPANDED,
		GTK_EXPANDER_EXPANDED
	}

	private enum GtkPositionType
	{
		GTK_POS_LEFT,
		GTK_POS_RIGHT,
		GTK_POS_TOP,
		GTK_POS_BOTTOM
	}

	private enum GtkWidgetFlags : uint
	{
		GTK_CAN_DEFAULT = 0x2000u
	}

	private enum GdkWindowEdge
	{
		GDK_WINDOW_EDGE_NORTH_WEST,
		GDK_WINDOW_EDGE_NORTH,
		GDK_WINDOW_EDGE_NORTH_EAST,
		GDK_WINDOW_EDGE_WEST,
		GDK_WINDOW_EDGE_EAST,
		GDK_WINDOW_EDGE_SOUTH_WEST,
		GDK_WINDOW_EDGE_SOUTH,
		GDK_WINDOW_EDGE_SOUTH_EAST
	}

	private struct GtkStyle
	{
		private GObject parent_instance;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] fg;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] bg;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] light;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] dark;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] mid;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] text;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] @base;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		private GdkColor[] text_aa;

		private GdkColor black;

		private GdkColor white;

		private IntPtr font_desc;

		public int xthickness;

		public int ythickness;
	}

	private struct GtkWidget
	{
		private GtkObject @object;

		private ushort private_flags;

		private byte state;

		private byte saved_state;

		private string name;

		private IntPtr style;

		private GtkRequisition requisition;

		public GdkRectangle allocation;

		private IntPtr window;

		private IntPtr parent;
	}

	private struct GtkObject
	{
		private GObject parent_instance;

		public uint flags;
	}

	private struct GtkRequisition
	{
		private int width;

		private int height;
	}

	private struct GtkMisc
	{
		private GtkWidget widget;

		public float xalign;

		public float yalign;

		public ushort xpad;

		public ushort ypad;
	}

	private struct GtkTreeViewColumn
	{
		private GtkObject parent;

		private IntPtr tree_view;

		public IntPtr button;
	}

	private enum G_TYPE
	{

	}

	private struct GTypeInstance
	{
		private IntPtr g_class;
	}

	internal struct GObject
	{
		private GTypeInstance g_type_instance;

		private uint ref_count;

		private IntPtr qdata;
	}

	private delegate void ToggleButtonPaintFunction(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate void GtkCallback(IntPtr widget, IntPtr data);

	private const WidgetType WidgetTypeNotNeeded = WidgetType.Button;

	private const string GobjectLibraryName = "libgobject-2.0.so";

	private const string GdkLibraryName = "libgdk-x11-2.0.so";

	private const string GdkPixbufLibraryName = "libgdk_pixbuf-2.0.so";

	private const string GtkLibraryName = "libgtk-x11-2.0.so";

	private const int G_TYPE_FUNDAMENTAL_SHIFT = 2;

	private static GtkPlus instance;

	private readonly int WidgetTypeCount = Enum.GetNames(typeof(WidgetType)).Length;

	private readonly IntPtr[] widgets;

	private readonly IntPtr window;

	private readonly IntPtr @fixed;

	private readonly IntPtr[] styles;

	private readonly IntPtr combo_box_drop_down_toggle_button;

	private readonly IntPtr combo_box_drop_down_arrow;

	private IntPtr combo_box_drop_down_toggle_button_style;

	private IntPtr combo_box_drop_down_arrow_style;

	private readonly IntPtr tool_bar_button;

	private readonly IntPtr tool_bar_toggle_button;

	private IntPtr tool_bar_button_style;

	private IntPtr tool_bar_toggle_button_style;

	private readonly IntPtr tree_view_column;

	private readonly IntPtr tree_view_column_button;

	private IntPtr tree_view_column_button_style;

	private readonly ButtonPainter button_painter = new ButtonPainter();

	private readonly CheckBoxPainter check_box_painter = new CheckBoxPainter();

	private readonly RadioButtonPainter radio_button_painter = new RadioButtonPainter();

	private readonly ComboBoxDropDownButtonPainter combo_box_drop_down_button_painter = new ComboBoxDropDownButtonPainter();

	private readonly ComboBoxBorderPainter combo_box_border_painter = new ComboBoxBorderPainter();

	private readonly GroupBoxPainter group_box_painter = new GroupBoxPainter();

	private readonly HeaderPainter header_painter = new HeaderPainter();

	private readonly ProgressBarBarPainter progress_bar_bar_painter = new ProgressBarBarPainter();

	private readonly ProgressBarChunkPainter progress_bar_chunk_painter = new ProgressBarChunkPainter();

	private readonly ScrollBarArrowButtonPainter scroll_bar_arrow_button_painter = new ScrollBarArrowButtonPainter();

	private readonly ScrollBarThumbButtonPainter scroll_bar_thumb_button_painter = new ScrollBarThumbButtonPainter();

	private readonly ScrollBarTrackPainter scroll_bar_track_painter = new ScrollBarTrackPainter();

	private readonly StatusBarGripperPainter status_bar_gripper_painter = new StatusBarGripperPainter();

	private readonly TabControlPanePainter tab_control_pane_painter = new TabControlPanePainter();

	private readonly TabControlTabItemPainter tab_control_tab_item_painter = new TabControlTabItemPainter();

	private readonly TextBoxPainter text_box_painter = new TextBoxPainter();

	private readonly ToolBarPainter tool_bar_painter = new ToolBarPainter();

	private readonly ToolBarButtonPainter tool_bar_button_painter = new ToolBarButtonPainter();

	private readonly ToolBarCheckedButtonPainter tool_bar_checked_button_painter = new ToolBarCheckedButtonPainter();

	private readonly TrackBarTrackPainter track_bar_track_painter = new TrackBarTrackPainter();

	private readonly TrackBarThumbPainter track_bar_thumb_painter = new TrackBarThumbPainter();

	private readonly TreeViewGlyphPainter tree_view_glyph_painter = new TreeViewGlyphPainter();

	private readonly UpDownPainter up_down_painter = new UpDownPainter();

	public static GtkPlus Instance => instance;

	protected GtkPlus()
	{
		widgets = new IntPtr[WidgetTypeCount];
		styles = new IntPtr[WidgetTypeCount];
		window = gtk_window_new(GtkWindowType.GTK_WINDOW_TOPLEVEL);
		@fixed = gtk_fixed_new();
		gtk_container_add(window, @fixed);
		IntPtr container = @fixed;
		ref IntPtr reference = ref widgets[0];
		gtk_container_add(container, reference = gtk_button_new());
		GTK_WIDGET_SET_FLAGS(widgets[0], GtkWidgetFlags.GTK_CAN_DEFAULT);
		IntPtr container2 = @fixed;
		ref IntPtr reference2 = ref widgets[1];
		gtk_container_add(container2, reference2 = gtk_check_button_new());
		IntPtr container3 = @fixed;
		ref IntPtr reference3 = ref widgets[2];
		gtk_container_add(container3, reference3 = gtk_combo_box_entry_new());
		gtk_widget_realize(widgets[2]);
		combo_box_drop_down_toggle_button = GetFirstChildWidgetOfType.Get(widgets[2], gtk_toggle_button_get_type());
		gtk_widget_realize(combo_box_drop_down_toggle_button);
		combo_box_drop_down_arrow = GetFirstChildWidgetOfType.Get(combo_box_drop_down_toggle_button, gtk_arrow_get_type());
		g_object_ref(combo_box_drop_down_toggle_button_style = GetWidgetStyle(combo_box_drop_down_toggle_button));
		g_object_ref(combo_box_drop_down_arrow_style = GetWidgetStyle(combo_box_drop_down_arrow));
		IntPtr container4 = @fixed;
		ref IntPtr reference4 = ref widgets[3];
		gtk_container_add(container4, reference4 = gtk_frame_new(null));
		IntPtr container5 = @fixed;
		ref IntPtr reference5 = ref widgets[4];
		gtk_container_add(container5, reference5 = gtk_progress_bar_new());
		IntPtr container6 = @fixed;
		ref IntPtr reference6 = ref widgets[5];
		gtk_container_add(container6, reference6 = gtk_radio_button_new(IntPtr.Zero));
		IntPtr container7 = @fixed;
		ref IntPtr reference7 = ref widgets[6];
		gtk_container_add(container7, reference7 = gtk_hscrollbar_new(IntPtr.Zero));
		IntPtr container8 = @fixed;
		ref IntPtr reference8 = ref widgets[7];
		gtk_container_add(container8, reference8 = gtk_vscrollbar_new(IntPtr.Zero));
		IntPtr container9 = @fixed;
		ref IntPtr reference9 = ref widgets[8];
		gtk_container_add(container9, reference9 = gtk_statusbar_new());
		IntPtr container10 = @fixed;
		ref IntPtr reference10 = ref widgets[9];
		gtk_container_add(container10, reference10 = gtk_notebook_new());
		IntPtr container11 = @fixed;
		ref IntPtr reference11 = ref widgets[10];
		gtk_container_add(container11, reference11 = gtk_entry_new());
		IntPtr container12 = @fixed;
		ref IntPtr reference12 = ref widgets[11];
		gtk_container_add(container12, reference12 = gtk_toolbar_new());
		IntPtr intPtr = gtk_tool_button_new(IntPtr.Zero, null);
		gtk_toolbar_insert(widgets[11], intPtr, -1);
		tool_bar_button = gtk_bin_get_child(intPtr);
		g_object_ref(tool_bar_button_style = GetWidgetStyle(tool_bar_button));
		IntPtr intPtr2 = gtk_toggle_tool_button_new();
		gtk_toolbar_insert(widgets[11], intPtr2, -1);
		tool_bar_toggle_button = gtk_bin_get_child(intPtr2);
		g_object_ref(tool_bar_toggle_button_style = GetWidgetStyle(tool_bar_toggle_button));
		IntPtr container13 = @fixed;
		ref IntPtr reference13 = ref widgets[12];
		gtk_container_add(container13, reference13 = gtk_hscale_new_with_range(0.0, 1.0, 1.0));
		IntPtr container14 = @fixed;
		ref IntPtr reference14 = ref widgets[13];
		gtk_container_add(container14, reference14 = gtk_vscale_new_with_range(0.0, 1.0, 1.0));
		IntPtr container15 = @fixed;
		ref IntPtr reference15 = ref widgets[14];
		gtk_container_add(container15, reference15 = gtk_tree_view_new());
		tree_view_column = gtk_tree_view_column_new();
		gtk_tree_view_insert_column(widgets[14], tree_view_column, -1);
		tree_view_column_button = ((GtkTreeViewColumn)Marshal.PtrToStructure(tree_view_column, typeof(GtkTreeViewColumn))).button;
		g_object_ref(tree_view_column_button_style = GetWidgetStyle(tree_view_column_button));
		IntPtr adjustment = gtk_adjustment_new(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
		IntPtr container16 = @fixed;
		ref IntPtr reference16 = ref widgets[15];
		gtk_container_add(container16, reference16 = gtk_spin_button_new(adjustment, 0.0, 0u));
		for (int i = 0; i < WidgetTypeCount; i++)
		{
			ref IntPtr reference17 = ref styles[i];
			g_object_ref(reference17 = GetWidgetStyle(widgets[i]));
		}
	}

	public static bool Initialize()
	{
		try
		{
			if (gtk_check_version(2u, 10u, 0u) != IntPtr.Zero)
			{
				return false;
			}
			int argc = 0;
			string[] argv = new string[1];
			bool flag = gtk_init_check(ref argc, ref argv);
			if (flag)
			{
				instance = new GtkPlus();
			}
			return flag;
		}
		catch (DllNotFoundException)
		{
			return false;
		}
	}

	~GtkPlus()
	{
		gtk_object_destroy(window);
		int i = 0;
		for (; i < WidgetTypeCount; i++)
		{
			g_object_unref(styles[i]);
		}
		g_object_unref(combo_box_drop_down_toggle_button_style);
		g_object_unref(combo_box_drop_down_arrow_style);
		g_object_unref(tool_bar_button_style);
		g_object_unref(tool_bar_toggle_button_style);
		g_object_unref(tree_view_column_button_style);
	}

	public void ButtonPaint(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, bool @default, GtkPlusState state)
	{
		button_painter.Configure(@default, state);
		Paint(WidgetType.Button, bounds, dc, clippingArea, button_painter);
	}

	public void CheckBoxPaint(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state, GtkPlusToggleButtonValue value)
	{
		check_box_painter.Configure(state, value);
		Paint(WidgetType.CheckBox, bounds, dc, clippingArea, check_box_painter);
	}

	private Size GetGtkCheckButtonIndicatorSize(WidgetType widgetType)
	{
		int widgetStyleInteger = GetWidgetStyleInteger(widgets[(int)widgetType], "indicator-size");
		return new Size(widgetStyleInteger, widgetStyleInteger);
	}

	public Size CheckBoxGetSize()
	{
		return GetGtkCheckButtonIndicatorSize(WidgetType.CheckBox);
	}

	public void ComboBoxPaintDropDownButton(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state)
	{
		combo_box_drop_down_button_painter.Configure(state);
		Paint(WidgetType.ComboBox, bounds, dc, clippingArea, combo_box_drop_down_button_painter);
	}

	public void ComboBoxPaintBorder(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea)
	{
		Paint(WidgetType.ComboBox, bounds, dc, clippingArea, combo_box_border_painter);
	}

	public void GroupBoxPaint(IDeviceContext dc, Rectangle bounds, Rectangle excludedArea, GtkPlusState state)
	{
		group_box_painter.Configure(state);
		PaintExcludingArea(WidgetType.GroupBox, bounds, dc, excludedArea, group_box_painter);
	}

	public void HeaderPaint(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state)
	{
		header_painter.Configure(state);
		Paint(WidgetType.TreeView, bounds, dc, clippingArea, header_painter);
	}

	public void ProgressBarPaintBar(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea)
	{
		Paint(WidgetType.ProgressBar, bounds, dc, clippingArea, progress_bar_bar_painter);
	}

	public void ProgressBarPaintChunk(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea)
	{
		Paint(WidgetType.ProgressBar, bounds, dc, clippingArea, progress_bar_chunk_painter);
	}

	public Rectangle ProgressBarGetBackgroundContentRectagle(Rectangle bounds)
	{
		GtkStyle gtkStyle = (GtkStyle)Marshal.PtrToStructure(gtk_widget_get_style(widgets[4]), typeof(GtkStyle));
		bounds.Inflate(-gtkStyle.xthickness, -gtkStyle.ythickness);
		return bounds;
	}

	public void RadioButtonPaint(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state, GtkPlusToggleButtonValue value)
	{
		radio_button_painter.Configure(state, value);
		Paint(WidgetType.RadioButton, bounds, dc, clippingArea, radio_button_painter);
	}

	public Size RadioButtonGetSize()
	{
		return GetGtkCheckButtonIndicatorSize(WidgetType.RadioButton);
	}

	public void ScrollBarPaintArrowButton(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state, bool horizontal, bool upOrLeft)
	{
		scroll_bar_arrow_button_painter.Configure(state, horizontal, upOrLeft);
		Paint((!horizontal) ? WidgetType.VScrollBar : WidgetType.HScrollBar, bounds, dc, clippingArea, scroll_bar_arrow_button_painter);
	}

	public void ScrollBarPaintThumbButton(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state, bool horizontal)
	{
		scroll_bar_thumb_button_painter.Configure(state, horizontal);
		Paint((!horizontal) ? WidgetType.VScrollBar : WidgetType.HScrollBar, bounds, dc, clippingArea, scroll_bar_thumb_button_painter);
	}

	public void ScrollBarPaintTrack(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state, bool horizontal, bool upOrLeft)
	{
		scroll_bar_track_painter.Configure(state, upOrLeft);
		Paint((!horizontal) ? WidgetType.VScrollBar : WidgetType.HScrollBar, bounds, dc, clippingArea, scroll_bar_track_painter);
	}

	public void StatusBarPaintGripper(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea)
	{
		Paint(WidgetType.StatusBar, bounds, dc, clippingArea, status_bar_gripper_painter);
	}

	public void TabControlPaintPane(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea)
	{
		Paint(WidgetType.TabControl, bounds, dc, clippingArea, tab_control_pane_painter);
	}

	public void TabControlPaintTabItem(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state)
	{
		tab_control_tab_item_painter.Configure(state);
		Paint(WidgetType.TabControl, bounds, dc, clippingArea, tab_control_tab_item_painter);
	}

	public void TextBoxPaint(IDeviceContext dc, Rectangle bounds, Rectangle excludedArea, GtkPlusState state)
	{
		text_box_painter.Configure(state);
		PaintExcludingArea(WidgetType.TextBox, bounds, dc, excludedArea, text_box_painter);
	}

	public void ToolBarPaint(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea)
	{
		Paint(WidgetType.ToolBar, bounds, dc, clippingArea, tool_bar_painter);
	}

	public void ToolBarPaintButton(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state)
	{
		tool_bar_button_painter.Configure(state);
		Paint(WidgetType.Button, bounds, dc, clippingArea, tool_bar_button_painter);
	}

	public void ToolBarPaintCheckedButton(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea)
	{
		Paint(WidgetType.Button, bounds, dc, clippingArea, tool_bar_checked_button_painter);
	}

	public void TrackBarPaintTrack(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, bool horizontal)
	{
		Paint((!horizontal) ? WidgetType.VerticalTrackBar : WidgetType.HorizontalTrackBar, bounds, dc, clippingArea, track_bar_track_painter);
	}

	public void TrackBarPaintThumb(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, GtkPlusState state, bool horizontal)
	{
		track_bar_thumb_painter.Configure(state, horizontal);
		Paint((!horizontal) ? WidgetType.VerticalTrackBar : WidgetType.HorizontalTrackBar, bounds, dc, clippingArea, track_bar_thumb_painter);
	}

	public void TreeViewPaintGlyph(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, bool closed)
	{
		tree_view_glyph_painter.Configure(closed);
		Paint(WidgetType.TreeView, bounds, dc, clippingArea, tree_view_glyph_painter);
	}

	public void UpDownPaint(IDeviceContext dc, Rectangle bounds, Rectangle clippingArea, bool up, GtkPlusState state)
	{
		up_down_painter.Configure(up, state);
		Paint(WidgetType.UpDown, bounds, dc, clippingArea, up_down_painter);
	}

	private void Paint(WidgetType widgetType, Rectangle bounds, IDeviceContext dc, Rectangle clippingArea, Painter painter)
	{
		Paint(widgetType, bounds, dc, TransparencyType.Alpha, Color.Black, DeviceContextType.Native, clippingArea, painter, Rectangle.Empty);
	}

	private void PaintExcludingArea(WidgetType widgetType, Rectangle bounds, IDeviceContext dc, Rectangle excludedArea, Painter painter)
	{
		Paint(widgetType, bounds, dc, TransparencyType.Alpha, Color.Black, DeviceContextType.Native, bounds, painter, excludedArea);
	}

	private unsafe void Paint(WidgetType widgetType, Rectangle bounds, IDeviceContext dc, TransparencyType transparencyType, Color background, DeviceContextType deviceContextType, Rectangle clippingArea, Painter painter, Rectangle excludedArea)
	{
		Rectangle clippingArea2 = Rectangle.Intersect(bounds, clippingArea);
		if (clippingArea2.Width == 0 || clippingArea2.Height == 0)
		{
			return;
		}
		clippingArea2.Offset(-bounds.X, -bounds.Y);
		excludedArea.Offset(-bounds.X, -bounds.Y);
		IntPtr intPtr = gdk_pixmap_new(IntPtr.Zero, bounds.Width, bounds.Height, 24);
		painter.AttachStyle(widgetType, intPtr, this);
		IntPtr intPtr2 = gdk_gc_new(intPtr);
		GdkColor color = new GdkColor(background);
		gdk_gc_set_rgb_fg_color(intPtr2, ref color);
		Paint(intPtr, intPtr2, bounds, widgetType, out var pixbuf, out var pixelData, out var rowstride, clippingArea2, painter, excludedArea);
		IntPtr pixbuf2 = IntPtr.Zero;
		IntPtr pixelData2 = IntPtr.Zero;
		int rowstride2 = 0;
		GdkColor color2 = default(GdkColor);
		if (transparencyType == TransparencyType.Alpha)
		{
			color2.red = ushort.MaxValue;
			color2.green = ushort.MaxValue;
			color2.blue = ushort.MaxValue;
			gdk_gc_set_rgb_fg_color(intPtr2, ref color2);
			Paint(intPtr, intPtr2, bounds, widgetType, out pixbuf2, out pixelData2, out rowstride2, clippingArea2, painter, excludedArea);
		}
		g_object_unref(intPtr2);
		byte* ptr = (byte*)(void*)pixelData;
		byte* ptr2 = (byte*)(void*)pixelData2;
		for (int i = 0; i < clippingArea2.Height; i++)
		{
			byte* ptr3 = ptr;
			byte* ptr4 = ptr2;
			for (int j = 0; j < clippingArea2.Width; j++)
			{
				switch (transparencyType)
				{
				case TransparencyType.Alpha:
					ptr3[3] = (byte)(*ptr3 - *ptr4 + 255);
					break;
				case TransparencyType.Color:
					if (*ptr3 == background.R && ptr3[1] == background.G && ptr3[2] == background.B)
					{
						ptr3[3] = 0;
					}
					break;
				}
				byte b = *ptr3;
				*ptr3 = ptr3[2];
				ptr3[2] = b;
				ptr3 += 4;
				ptr4 += 4;
			}
			ptr += rowstride;
			ptr2 += rowstride2;
		}
		if (transparencyType == TransparencyType.Alpha)
		{
			g_object_unref(pixbuf2);
		}
		g_object_unref(intPtr);
		Bitmap bitmap = new Bitmap(clippingArea2.Width, clippingArea2.Height, rowstride, PixelFormat.Format32bppPArgb, pixelData);
		bool flag = false;
		Graphics graphics;
		switch (deviceContextType)
		{
		case DeviceContextType.Graphics:
			graphics = (Graphics)dc;
			break;
		case DeviceContextType.Native:
			graphics = Graphics.FromHdc(dc.GetHdc());
			break;
		default:
			graphics = dc as Graphics;
			if (graphics == null)
			{
				flag = true;
				graphics = Graphics.FromHdc(dc.GetHdc());
			}
			else
			{
				flag = false;
			}
			break;
		}
		clippingArea2.Offset(bounds.X, bounds.Y);
		graphics.DrawImage(bitmap, clippingArea2.Location);
		switch (deviceContextType)
		{
		case DeviceContextType.Native:
			graphics.Dispose();
			dc.ReleaseHdc();
			break;
		default:
			if (flag)
			{
				graphics.Dispose();
				dc.ReleaseHdc();
			}
			break;
		case DeviceContextType.Graphics:
			break;
		}
		bitmap.Dispose();
		g_object_unref(pixbuf);
	}

	private void Paint(IntPtr drawable, IntPtr gc, Rectangle rectangle, WidgetType widgetType, out IntPtr pixbuf, out IntPtr pixelData, out int rowstride, Rectangle clippingArea, Painter painter, Rectangle excludedArea)
	{
		gdk_draw_rectangle(drawable, gc, filled: true, clippingArea.X, clippingArea.Y, clippingArea.Width, clippingArea.Height);
		painter.Paint(styles[(int)widgetType], drawable, new GdkRectangle(clippingArea), widgets[(int)widgetType], 0, 0, rectangle.Width, rectangle.Height, this);
		if (excludedArea.Width != 0)
		{
			gdk_draw_rectangle(drawable, gc, filled: true, excludedArea.X, excludedArea.Y, excludedArea.Width, excludedArea.Height);
		}
		if ((pixbuf = gdk_pixbuf_new(GdkColorspace.GDK_COLORSPACE_RGB, has_alpha: true, 8, clippingArea.Width, clippingArea.Height)) == IntPtr.Zero || gdk_pixbuf_get_from_drawable(pixbuf, drawable, IntPtr.Zero, clippingArea.X, clippingArea.Y, 0, 0, clippingArea.Width, clippingArea.Height) == IntPtr.Zero)
		{
			throw new OutOfMemoryException();
		}
		pixelData = gdk_pixbuf_get_pixels(pixbuf);
		rowstride = gdk_pixbuf_get_rowstride(pixbuf);
	}

	private static GtkShadowType GetWidgetStyleShadowType(IntPtr widget)
	{
		gtk_widget_style_get(widget, "shadow-type", out GtkShadowType value, IntPtr.Zero);
		return value;
	}

	private static int GetWidgetStyleInteger(IntPtr widget, string propertyName)
	{
		gtk_widget_style_get(widget, propertyName, out int value, IntPtr.Zero);
		return value;
	}

	private static float GetWidgetStyleSingle(IntPtr widget, string propertyName)
	{
		gtk_widget_style_get(widget, propertyName, out float value, IntPtr.Zero);
		return value;
	}

	private static bool GetWidgetStyleBoolean(IntPtr widget, string propertyName)
	{
		gtk_widget_style_get(widget, propertyName, out bool value, IntPtr.Zero);
		return value;
	}

	private static IntPtr GetWidgetStyle(IntPtr widget)
	{
		return gtk_rc_get_style(widget);
	}

	[DllImport("libgdk-x11-2.0.so")]
	private static extern void gdk_draw_rectangle(IntPtr drawable, IntPtr gc, bool filled, int x, int y, int width, int height);

	[DllImport("libgdk-x11-2.0.so")]
	private static extern IntPtr gdk_gc_new(IntPtr drawable);

	[DllImport("libgdk-x11-2.0.so")]
	private static extern void gdk_gc_set_rgb_fg_color(IntPtr gc, ref GdkColor color);

	[DllImport("libgdk-x11-2.0.so")]
	private static extern IntPtr gdk_pixbuf_get_from_drawable(IntPtr dest, IntPtr src, IntPtr cmap, int src_x, int src_y, int dest_x, int dest_y, int width, int height);

	[DllImport("libgdk-x11-2.0.so")]
	private static extern IntPtr gdk_pixmap_new(IntPtr drawable, int width, int height, int depth);

	[DllImport("libgdk_pixbuf-2.0.so")]
	private static extern IntPtr gdk_pixbuf_get_pixels(IntPtr pixbuf);

	[DllImport("libgdk_pixbuf-2.0.so")]
	private static extern int gdk_pixbuf_get_rowstride(IntPtr pixbuf);

	[DllImport("libgdk_pixbuf-2.0.so")]
	private static extern IntPtr gdk_pixbuf_new(GdkColorspace colorspace, bool has_alpha, int bits_per_sample, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern bool gtk_init_check(ref int argc, ref string[] argv);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_check_version(uint required_major, uint required_minor, uint required_micro);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_container_add(IntPtr container, IntPtr widget);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_container_forall(IntPtr container, GtkCallback callback, IntPtr callback_data);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_object_destroy(IntPtr @object);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_rc_get_style(IntPtr widget);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_style_attach(IntPtr style, IntPtr window);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_widget_realize(IntPtr widget);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_widget_style_get(IntPtr widget, string property, out int value, IntPtr nullTerminator);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_widget_style_get(IntPtr widget, string property, out float value, IntPtr nullTerminator);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_widget_style_get(IntPtr widget, string property1, out int value1, string property2, out int value2, IntPtr nullTerminator);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_widget_style_get(IntPtr widget, string property, out GtkShadowType value, IntPtr nullTerminator);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_widget_style_get(IntPtr widget, string property, out bool value, IntPtr nullTerminator);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_window_new(GtkWindowType type);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_window_set_default(IntPtr window, IntPtr default_widget);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_adjustment_new(double value, double lower, double upper, double step_increment, double page_increment, double page_size);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_widget_get_style(IntPtr widget);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_tree_view_column_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern int gtk_tree_view_insert_column(IntPtr tree_view, IntPtr column, int position);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_toolbar_insert(IntPtr toolbar, IntPtr item, int pos);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_bin_get_child(IntPtr bin);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_arrow_get_type();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_container_get_type();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_toggle_button_get_type();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_button_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_check_button_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_combo_box_entry_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_entry_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_fixed_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_frame_new(string label);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_hscale_new_with_range(double min, double max, double step);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_hscrollbar_new(IntPtr adjustment);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_notebook_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_progress_bar_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_radio_button_new(IntPtr group);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_spin_button_new(IntPtr adjustment, double climb_rate, uint digits);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_statusbar_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_toggle_tool_button_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_toolbar_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_tool_button_new(IntPtr icon_widget, string label);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_tree_view_new();

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_vscale_new_with_range(double min, double max, double step);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern IntPtr gtk_vscrollbar_new(IntPtr adjustment);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_arrow(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, GtkArrowType arrow_type, bool fill, int x, int y, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_box(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_box_gap(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height, GtkPositionType gap_side, int gap_x, int gap_width);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_check(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_expander(IntPtr style, IntPtr window, GtkStateType state_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, GtkExpanderStyle expander_style);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_extension(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height, GtkPositionType gap_side);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_flat_box(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_option(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_resize_grip(IntPtr style, IntPtr window, GtkStateType state_type, ref GdkRectangle area, IntPtr widget, string detail, GdkWindowEdge edge, int x, int y, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_shadow(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height);

	[DllImport("libgtk-x11-2.0.so")]
	private static extern void gtk_paint_slider(IntPtr style, IntPtr window, GtkStateType state_type, GtkShadowType shadow_type, ref GdkRectangle area, IntPtr widget, string detail, int x, int y, int width, int height, GtkOrientation orientation);

	private static void GTK_WIDGET_SET_FLAGS(IntPtr wid, GtkWidgetFlags flag)
	{
		GtkObject gtkObject = (GtkObject)Marshal.PtrToStructure(wid, typeof(GtkObject));
		gtkObject.flags |= (uint)flag;
		Marshal.StructureToPtr(gtkObject, wid, fDeleteOld: false);
	}

	[DllImport("libgobject-2.0.so")]
	private static extern IntPtr g_object_ref(IntPtr @object);

	[DllImport("libgobject-2.0.so")]
	private static extern void g_object_unref(IntPtr @object);

	[DllImport("libgobject-2.0.so")]
	private static extern bool g_type_check_instance_is_a(IntPtr type_instance, IntPtr iface_type);

	[DllImport("libgobject-2.0.so")]
	private static extern void g_object_get(IntPtr @object, string property_name, out bool value, IntPtr nullTerminator);
}
