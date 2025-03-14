using System.Drawing;

namespace System.Windows.Forms.VisualStyles;

internal class VisualStylesGtkPlus : IVisualStyles
{
	private enum S
	{
		S_OK,
		S_FALSE
	}

	private enum ThemeHandle
	{
		BUTTON = 1,
		COMBOBOX,
		EDIT,
		HEADER,
		PROGRESS,
		REBAR,
		SCROLLBAR,
		SPIN,
		STATUS,
		TAB,
		TOOLBAR,
		TRACKBAR,
		TREEVIEW
	}

	private static GtkPlus GtkPlus => GtkPlus.Instance;

	public string VisualStyleInformationAuthor => null;

	public string VisualStyleInformationColorScheme => null;

	public string VisualStyleInformationCompany => null;

	public Color VisualStyleInformationControlHighlightHot => Color.Black;

	public string VisualStyleInformationCopyright => null;

	public string VisualStyleInformationDescription => null;

	public string VisualStyleInformationDisplayName => null;

	public string VisualStyleInformationFileName => null;

	public bool VisualStyleInformationIsSupportedByOS => true;

	public int VisualStyleInformationMinimumColorDepth => 0;

	public string VisualStyleInformationSize => null;

	public bool VisualStyleInformationSupportsFlatMenus => false;

	public Color VisualStyleInformationTextControlBorder => Color.Black;

	public string VisualStyleInformationUrl => null;

	public string VisualStyleInformationVersion => null;

	public static bool Initialize()
	{
		return GtkPlus.Initialize();
	}

	public int UxThemeCloseThemeData(IntPtr hTheme)
	{
		return 0;
	}

	public int UxThemeDrawThemeParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
	{
		return 1;
	}

	public int UxThemeDrawThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, Rectangle clipRectangle)
	{
		return (!DrawBackground((ThemeHandle)(int)hTheme, dc, iPartId, iStateId, bounds, clipRectangle, Rectangle.Empty)) ? 1 : 0;
	}

	public int UxThemeDrawThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds)
	{
		return UxThemeDrawThemeBackground(hTheme, dc, iPartId, iStateId, bounds, bounds);
	}

	private bool DrawBackground(ThemeHandle themeHandle, IDeviceContext dc, int part, int state, Rectangle bounds, Rectangle clipRectangle, Rectangle excludedArea)
	{
		switch (themeHandle)
		{
		case ThemeHandle.BUTTON:
			switch ((BUTTONPARTS)part)
			{
			case BUTTONPARTS.BP_PUSHBUTTON:
			{
				GtkPlusState state2;
				switch ((PUSHBUTTONSTATES)state)
				{
				case PUSHBUTTONSTATES.PBS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case PUSHBUTTONSTATES.PBS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case PUSHBUTTONSTATES.PBS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				case PUSHBUTTONSTATES.PBS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				case PUSHBUTTONSTATES.PBS_DEFAULTED:
					state2 = GtkPlusState.Normal;
					break;
				default:
					return false;
				}
				GtkPlus.ButtonPaint(dc, bounds, clipRectangle, state == 5, state2);
				return true;
			}
			case BUTTONPARTS.BP_RADIOBUTTON:
			{
				GtkPlusState state2;
				GtkPlusToggleButtonValue value;
				switch ((RADIOBUTTONSTATES)state)
				{
				case RADIOBUTTONSTATES.RBS_UNCHECKEDNORMAL:
					state2 = GtkPlusState.Normal;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case RADIOBUTTONSTATES.RBS_UNCHECKEDPRESSED:
					state2 = GtkPlusState.Pressed;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case RADIOBUTTONSTATES.RBS_UNCHECKEDHOT:
					state2 = GtkPlusState.Hot;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case RADIOBUTTONSTATES.RBS_UNCHECKEDDISABLED:
					state2 = GtkPlusState.Disabled;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case RADIOBUTTONSTATES.RBS_CHECKEDNORMAL:
					state2 = GtkPlusState.Normal;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				case RADIOBUTTONSTATES.RBS_CHECKEDPRESSED:
					state2 = GtkPlusState.Pressed;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				case RADIOBUTTONSTATES.RBS_CHECKEDHOT:
					state2 = GtkPlusState.Hot;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				case RADIOBUTTONSTATES.RBS_CHECKEDDISABLED:
					state2 = GtkPlusState.Disabled;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				default:
					return false;
				}
				GtkPlus.RadioButtonPaint(dc, bounds, clipRectangle, state2, value);
				return true;
			}
			case BUTTONPARTS.BP_CHECKBOX:
			{
				GtkPlusState state2;
				GtkPlusToggleButtonValue value;
				switch ((CHECKBOXSTATES)state)
				{
				case CHECKBOXSTATES.CBS_UNCHECKEDNORMAL:
					state2 = GtkPlusState.Normal;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case CHECKBOXSTATES.CBS_UNCHECKEDPRESSED:
					state2 = GtkPlusState.Pressed;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case CHECKBOXSTATES.CBS_UNCHECKEDHOT:
					state2 = GtkPlusState.Hot;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case CHECKBOXSTATES.CBS_UNCHECKEDDISABLED:
					state2 = GtkPlusState.Disabled;
					value = GtkPlusToggleButtonValue.Unchecked;
					break;
				case CHECKBOXSTATES.CBS_CHECKEDNORMAL:
					state2 = GtkPlusState.Normal;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				case CHECKBOXSTATES.CBS_CHECKEDPRESSED:
					state2 = GtkPlusState.Pressed;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				case CHECKBOXSTATES.CBS_CHECKEDHOT:
					state2 = GtkPlusState.Hot;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				case CHECKBOXSTATES.CBS_CHECKEDDISABLED:
					state2 = GtkPlusState.Disabled;
					value = GtkPlusToggleButtonValue.Checked;
					break;
				case CHECKBOXSTATES.CBS_MIXEDNORMAL:
					state2 = GtkPlusState.Normal;
					value = GtkPlusToggleButtonValue.Mixed;
					break;
				case CHECKBOXSTATES.CBS_MIXEDPRESSED:
					state2 = GtkPlusState.Pressed;
					value = GtkPlusToggleButtonValue.Mixed;
					break;
				case CHECKBOXSTATES.CBS_MIXEDHOT:
					state2 = GtkPlusState.Hot;
					value = GtkPlusToggleButtonValue.Mixed;
					break;
				case CHECKBOXSTATES.CBS_MIXEDDISABLED:
					state2 = GtkPlusState.Disabled;
					value = GtkPlusToggleButtonValue.Mixed;
					break;
				default:
					return false;
				}
				GtkPlus.CheckBoxPaint(dc, bounds, clipRectangle, state2, value);
				return true;
			}
			case BUTTONPARTS.BP_GROUPBOX:
			{
				GtkPlusState state2;
				switch ((GROUPBOXSTATES)state)
				{
				case GROUPBOXSTATES.GBS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case GROUPBOXSTATES.GBS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				default:
					return false;
				}
				GtkPlus.GroupBoxPaint(dc, bounds, excludedArea, state2);
				return true;
			}
			default:
				return false;
			}
		case ThemeHandle.COMBOBOX:
			switch ((COMBOBOXPARTS)part)
			{
			case COMBOBOXPARTS.CP_DROPDOWNBUTTON:
			{
				GtkPlusState state2;
				switch ((COMBOBOXSTYLESTATES)state)
				{
				case COMBOBOXSTYLESTATES.CBXS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case COMBOBOXSTYLESTATES.CBXS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case COMBOBOXSTYLESTATES.CBXS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				case COMBOBOXSTYLESTATES.CBXS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				default:
					return false;
				}
				GtkPlus.ComboBoxPaintDropDownButton(dc, bounds, clipRectangle, state2);
				return true;
			}
			case COMBOBOXPARTS.CP_BORDER:
				switch ((BORDERSTATES)state)
				{
				case BORDERSTATES.CBB_NORMAL:
				case BORDERSTATES.CBB_HOT:
				case BORDERSTATES.CBB_FOCUSED:
				case BORDERSTATES.CBB_DISABLED:
					GtkPlus.ComboBoxPaintBorder(dc, bounds, clipRectangle);
					return true;
				default:
					return false;
				}
			default:
				return false;
			}
		case ThemeHandle.EDIT:
			if (part == 1)
			{
				GtkPlusState state2;
				switch ((EDITTEXTSTATES)state)
				{
				case EDITTEXTSTATES.ETS_NORMAL:
				case EDITTEXTSTATES.ETS_HOT:
				case EDITTEXTSTATES.ETS_SELECTED:
				case EDITTEXTSTATES.ETS_FOCUSED:
				case EDITTEXTSTATES.ETS_READONLY:
				case EDITTEXTSTATES.ETS_ASSIST:
					state2 = GtkPlusState.Normal;
					break;
				case EDITTEXTSTATES.ETS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				default:
					return false;
				}
				GtkPlus.TextBoxPaint(dc, bounds, excludedArea, state2);
				return true;
			}
			return false;
		case ThemeHandle.HEADER:
			if (part == 1)
			{
				GtkPlusState state2;
				switch ((HEADERITEMSTATES)state)
				{
				case HEADERITEMSTATES.HIS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case HEADERITEMSTATES.HIS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case HEADERITEMSTATES.HIS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				default:
					return false;
				}
				GtkPlus.HeaderPaint(dc, bounds, clipRectangle, state2);
				return true;
			}
			return false;
		case ThemeHandle.PROGRESS:
			switch ((PROGRESSPARTS)part)
			{
			case PROGRESSPARTS.PP_BAR:
			case PROGRESSPARTS.PP_BARVERT:
				GtkPlus.ProgressBarPaintBar(dc, bounds, clipRectangle);
				return true;
			case PROGRESSPARTS.PP_CHUNK:
			case PROGRESSPARTS.PP_CHUNKVERT:
				GtkPlus.ProgressBarPaintChunk(dc, bounds, clipRectangle);
				return true;
			default:
				return false;
			}
		case ThemeHandle.REBAR:
			if (part == 3)
			{
				GtkPlus.ToolBarPaint(dc, bounds, clipRectangle);
				return true;
			}
			return false;
		case ThemeHandle.SCROLLBAR:
		{
			GtkPlusState state2;
			switch ((SCROLLBARPARTS)part)
			{
			case SCROLLBARPARTS.SBP_ARROWBTN:
			{
				bool horizontal;
				bool upOrLeft;
				switch ((ARROWBTNSTATES)state)
				{
				case ARROWBTNSTATES.ABS_UPNORMAL:
					state2 = GtkPlusState.Normal;
					horizontal = false;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_UPHOT:
					state2 = GtkPlusState.Hot;
					horizontal = false;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_UPPRESSED:
					state2 = GtkPlusState.Pressed;
					horizontal = false;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_UPDISABLED:
					state2 = GtkPlusState.Disabled;
					horizontal = false;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_DOWNNORMAL:
					state2 = GtkPlusState.Normal;
					horizontal = false;
					upOrLeft = false;
					break;
				case ARROWBTNSTATES.ABS_DOWNHOT:
					state2 = GtkPlusState.Hot;
					horizontal = false;
					upOrLeft = false;
					break;
				case ARROWBTNSTATES.ABS_DOWNPRESSED:
					state2 = GtkPlusState.Pressed;
					horizontal = false;
					upOrLeft = false;
					break;
				case ARROWBTNSTATES.ABS_DOWNDISABLED:
					state2 = GtkPlusState.Disabled;
					horizontal = false;
					upOrLeft = false;
					break;
				case ARROWBTNSTATES.ABS_LEFTNORMAL:
					state2 = GtkPlusState.Normal;
					horizontal = true;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_LEFTHOT:
					state2 = GtkPlusState.Hot;
					horizontal = true;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_LEFTPRESSED:
					state2 = GtkPlusState.Pressed;
					horizontal = true;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_LEFTDISABLED:
					state2 = GtkPlusState.Disabled;
					horizontal = true;
					upOrLeft = true;
					break;
				case ARROWBTNSTATES.ABS_RIGHTNORMAL:
					state2 = GtkPlusState.Normal;
					horizontal = true;
					upOrLeft = false;
					break;
				case ARROWBTNSTATES.ABS_RIGHTHOT:
					state2 = GtkPlusState.Hot;
					horizontal = true;
					upOrLeft = false;
					break;
				case ARROWBTNSTATES.ABS_RIGHTPRESSED:
					state2 = GtkPlusState.Pressed;
					horizontal = true;
					upOrLeft = false;
					break;
				case ARROWBTNSTATES.ABS_RIGHTDISABLED:
					state2 = GtkPlusState.Disabled;
					horizontal = true;
					upOrLeft = false;
					break;
				default:
					return false;
				}
				GtkPlus.ScrollBarPaintArrowButton(dc, bounds, clipRectangle, state2, horizontal, upOrLeft);
				return true;
			}
			case SCROLLBARPARTS.SBP_THUMBBTNHORZ:
			case SCROLLBARPARTS.SBP_THUMBBTNVERT:
				if (!GetGtkPlusState((SCROLLBARSTYLESTATES)state, out state2))
				{
					return false;
				}
				GtkPlus.ScrollBarPaintThumbButton(dc, bounds, clipRectangle, state2, part == 2);
				return true;
			case SCROLLBARPARTS.SBP_LOWERTRACKHORZ:
			case SCROLLBARPARTS.SBP_UPPERTRACKHORZ:
			case SCROLLBARPARTS.SBP_LOWERTRACKVERT:
			case SCROLLBARPARTS.SBP_UPPERTRACKVERT:
				if (!GetGtkPlusState((SCROLLBARSTYLESTATES)state, out state2))
				{
					return false;
				}
				GtkPlus.ScrollBarPaintTrack(dc, bounds, clipRectangle, state2, part == 4 || part == 5, part == 5 || part == 7);
				return true;
			default:
				return false;
			}
		}
		case ThemeHandle.SPIN:
		{
			bool up;
			GtkPlusState state2;
			switch ((SPINPARTS)part)
			{
			case SPINPARTS.SPNP_UP:
				up = true;
				switch ((UPSTATES)state)
				{
				case UPSTATES.UPS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case UPSTATES.UPS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case UPSTATES.UPS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				case UPSTATES.UPS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				default:
					return false;
				}
				break;
			case SPINPARTS.SPNP_DOWN:
				up = false;
				switch ((DOWNSTATES)state)
				{
				case DOWNSTATES.DNS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case DOWNSTATES.DNS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case DOWNSTATES.DNS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				case DOWNSTATES.DNS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				default:
					return false;
				}
				break;
			default:
				return false;
			}
			GtkPlus.UpDownPaint(dc, bounds, clipRectangle, up, state2);
			return true;
		}
		case ThemeHandle.STATUS:
			if (part == 3)
			{
				GtkPlus.StatusBarPaintGripper(dc, bounds, clipRectangle);
				return true;
			}
			return false;
		case ThemeHandle.TAB:
		{
			bool flag;
			switch ((TABPARTS)part)
			{
			case TABPARTS.TABP_TABITEM:
				switch ((TABITEMSTATES)state)
				{
				case TABITEMSTATES.TIS_SELECTED:
					flag = true;
					break;
				case TABITEMSTATES.TIS_NORMAL:
				case TABITEMSTATES.TIS_HOT:
				case TABITEMSTATES.TIS_DISABLED:
					flag = false;
					break;
				default:
					return false;
				}
				break;
			case TABPARTS.TABP_TABITEMLEFTEDGE:
				switch ((TABITEMLEFTEDGESTATES)state)
				{
				case TABITEMLEFTEDGESTATES.TILES_SELECTED:
					flag = true;
					break;
				case TABITEMLEFTEDGESTATES.TILES_NORMAL:
				case TABITEMLEFTEDGESTATES.TILES_HOT:
				case TABITEMLEFTEDGESTATES.TILES_DISABLED:
					flag = false;
					break;
				default:
					return false;
				}
				break;
			case TABPARTS.TABP_TABITEMRIGHTEDGE:
				switch ((TABITEMRIGHTEDGESTATES)state)
				{
				case TABITEMRIGHTEDGESTATES.TIRES_SELECTED:
					flag = true;
					break;
				case TABITEMRIGHTEDGESTATES.TIRES_NORMAL:
				case TABITEMRIGHTEDGESTATES.TIRES_HOT:
				case TABITEMRIGHTEDGESTATES.TIRES_DISABLED:
					flag = false;
					break;
				default:
					return false;
				}
				break;
			case TABPARTS.TABP_TABITEMBOTHEDGE:
				flag = false;
				break;
			case TABPARTS.TABP_TOPTABITEM:
				switch ((TOPTABITEMSTATES)state)
				{
				case TOPTABITEMSTATES.TTIS_SELECTED:
					flag = true;
					break;
				case TOPTABITEMSTATES.TTIS_NORMAL:
				case TOPTABITEMSTATES.TTIS_HOT:
				case TOPTABITEMSTATES.TTIS_DISABLED:
					flag = false;
					break;
				default:
					return false;
				}
				break;
			case TABPARTS.TABP_TOPTABITEMLEFTEDGE:
				switch ((TOPTABITEMLEFTEDGESTATES)state)
				{
				case TOPTABITEMLEFTEDGESTATES.TTILES_SELECTED:
					flag = true;
					break;
				case TOPTABITEMLEFTEDGESTATES.TTILES_NORMAL:
				case TOPTABITEMLEFTEDGESTATES.TTILES_HOT:
				case TOPTABITEMLEFTEDGESTATES.TTILES_DISABLED:
					flag = false;
					break;
				default:
					return false;
				}
				break;
			case TABPARTS.TABP_TOPTABITEMRIGHTEDGE:
				switch ((TOPTABITEMRIGHTEDGESTATES)state)
				{
				case TOPTABITEMRIGHTEDGESTATES.TTIRES_SELECTED:
					flag = true;
					break;
				case TOPTABITEMRIGHTEDGESTATES.TTIRES_NORMAL:
				case TOPTABITEMRIGHTEDGESTATES.TTIRES_HOT:
				case TOPTABITEMRIGHTEDGESTATES.TTIRES_DISABLED:
					flag = false;
					break;
				default:
					return false;
				}
				break;
			case TABPARTS.TABP_TOPTABITEMBOTHEDGE:
				flag = false;
				break;
			case TABPARTS.TABP_PANE:
				GtkPlus.TabControlPaintPane(dc, bounds, clipRectangle);
				return true;
			default:
				return false;
			}
			GtkPlus.TabControlPaintTabItem(dc, bounds, clipRectangle, flag ? GtkPlusState.Pressed : GtkPlusState.Normal);
			return true;
		}
		case ThemeHandle.TOOLBAR:
			if (part == 1)
			{
				GtkPlusState state2;
				switch ((TOOLBARSTYLESTATES)state)
				{
				case TOOLBARSTYLESTATES.TS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case TOOLBARSTYLESTATES.TS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case TOOLBARSTYLESTATES.TS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				case TOOLBARSTYLESTATES.TS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				case TOOLBARSTYLESTATES.TS_CHECKED:
				case TOOLBARSTYLESTATES.TS_HOTCHECKED:
					GtkPlus.ToolBarPaintCheckedButton(dc, bounds, clipRectangle);
					return true;
				default:
					return false;
				}
				GtkPlus.ToolBarPaintButton(dc, bounds, clipRectangle, state2);
				return true;
			}
			return false;
		case ThemeHandle.TRACKBAR:
			switch ((TRACKBARPARTS)part)
			{
			case TRACKBARPARTS.TKP_TRACK:
				if (state == 1)
				{
					GtkPlus.TrackBarPaintTrack(dc, bounds, clipRectangle, horizontal: true);
					return true;
				}
				return false;
			case TRACKBARPARTS.TKP_TRACKVERT:
				if (state == 1)
				{
					GtkPlus.TrackBarPaintTrack(dc, bounds, clipRectangle, horizontal: false);
					return true;
				}
				return false;
			case TRACKBARPARTS.TKP_THUMB:
			{
				GtkPlusState state2;
				switch ((THUMBSTATES)state)
				{
				case THUMBSTATES.TUS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case THUMBSTATES.TUS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case THUMBSTATES.TUS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				case THUMBSTATES.TUS_FOCUSED:
					state2 = GtkPlusState.Selected;
					break;
				case THUMBSTATES.TUS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				default:
					return false;
				}
				GtkPlus.TrackBarPaintThumb(dc, bounds, clipRectangle, state2, horizontal: true);
				return true;
			}
			case TRACKBARPARTS.TKP_THUMBVERT:
			{
				GtkPlusState state2;
				switch ((THUMBVERTSTATES)state)
				{
				case THUMBVERTSTATES.TUVS_NORMAL:
					state2 = GtkPlusState.Normal;
					break;
				case THUMBVERTSTATES.TUVS_HOT:
					state2 = GtkPlusState.Hot;
					break;
				case THUMBVERTSTATES.TUVS_PRESSED:
					state2 = GtkPlusState.Pressed;
					break;
				case THUMBVERTSTATES.TUVS_FOCUSED:
					state2 = GtkPlusState.Selected;
					break;
				case THUMBVERTSTATES.TUVS_DISABLED:
					state2 = GtkPlusState.Disabled;
					break;
				default:
					return false;
				}
				GtkPlus.TrackBarPaintThumb(dc, bounds, clipRectangle, state2, horizontal: false);
				return true;
			}
			default:
				return false;
			}
		case ThemeHandle.TREEVIEW:
			if (part == 2)
			{
				bool closed;
				switch ((GLYPHSTATES)state)
				{
				case GLYPHSTATES.GLPS_CLOSED:
					closed = true;
					break;
				case GLYPHSTATES.GLPS_OPENED:
					closed = false;
					break;
				default:
					return false;
				}
				GtkPlus.TreeViewPaintGlyph(dc, bounds, clipRectangle, closed);
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	private static bool GetGtkPlusState(SCROLLBARSTYLESTATES state, out GtkPlusState result)
	{
		switch (state)
		{
		case SCROLLBARSTYLESTATES.SCRBS_NORMAL:
			result = GtkPlusState.Normal;
			break;
		case SCROLLBARSTYLESTATES.SCRBS_HOT:
			result = GtkPlusState.Hot;
			break;
		case SCROLLBARSTYLESTATES.SCRBS_PRESSED:
			result = GtkPlusState.Pressed;
			break;
		case SCROLLBARSTYLESTATES.SCRBS_DISABLED:
			result = GtkPlusState.Disabled;
			break;
		default:
			result = GtkPlusState.Normal;
			return false;
		}
		return true;
	}

	public int UxThemeDrawThemeEdge(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects, out Rectangle result)
	{
		result = Rectangle.Empty;
		return 1;
	}

	public int UxThemeDrawThemeText(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string text, TextFormatFlags textFlags, Rectangle bounds)
	{
		return 1;
	}

	public int UxThemeGetThemeBackgroundContentRect(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, out Rectangle result)
	{
		return (!GetBackgroundContentRectangle((ThemeHandle)(int)hTheme, iPartId, iStateId, bounds, out result)) ? 1 : 0;
	}

	private bool GetBackgroundContentRectangle(ThemeHandle handle, int part, int state, Rectangle bounds, out Rectangle result)
	{
		if (handle == ThemeHandle.PROGRESS)
		{
			if (part == 1 || part == 2)
			{
				result = GtkPlus.ProgressBarGetBackgroundContentRectagle(bounds);
				return true;
			}
		}
		result = Rectangle.Empty;
		return false;
	}

	public int UxThemeGetThemeBackgroundExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle contentBounds, out Rectangle result)
	{
		result = Rectangle.Empty;
		return 1;
	}

	public int UxThemeGetThemeBackgroundRegion(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, out Region result)
	{
		result = null;
		return 1;
	}

	public int UxThemeGetThemeBool(IntPtr hTheme, int iPartId, int iStateId, BooleanProperty prop, out bool result)
	{
		result = false;
		return 1;
	}

	public int UxThemeGetThemeColor(IntPtr hTheme, int iPartId, int iStateId, ColorProperty prop, out Color result)
	{
		result = Color.Black;
		return 1;
	}

	public int UxThemeGetThemeEnumValue(IntPtr hTheme, int iPartId, int iStateId, EnumProperty prop, out int result)
	{
		result = 0;
		return 1;
	}

	public int UxThemeGetThemeFilename(IntPtr hTheme, int iPartId, int iStateId, FilenameProperty prop, out string result)
	{
		result = null;
		return 1;
	}

	public int UxThemeGetThemeInt(IntPtr hTheme, int iPartId, int iStateId, IntegerProperty prop, out int result)
	{
		return (!GetInteger((ThemeHandle)(int)hTheme, iPartId, iStateId, prop, out result)) ? 1 : 0;
	}

	private bool GetInteger(ThemeHandle handle, int part, int state, IntegerProperty property, out int result)
	{
		if (handle == ThemeHandle.PROGRESS)
		{
			if (part == 3 || part == 4)
			{
				switch (property)
				{
				case IntegerProperty.ProgressChunkSize:
					result = ThemeWin32Classic.ProgressBarGetChunkSize();
					return true;
				case IntegerProperty.ProgressSpaceSize:
					result = 2;
					return true;
				}
			}
		}
		result = 0;
		return false;
	}

	public int UxThemeGetThemeMargins(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, MarginProperty prop, out Padding result)
	{
		result = Padding.Empty;
		return 1;
	}

	public int UxThemeGetThemePartSize(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, ThemeSizeType type, out Size result)
	{
		return (!GetPartSize((ThemeHandle)(int)hTheme, dc, iPartId, iStateId, bounds, rectangleSpecified: true, type, out result)) ? 1 : 0;
	}

	public int UxThemeGetThemePartSize(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, ThemeSizeType type, out Size result)
	{
		return (!GetPartSize((ThemeHandle)(int)hTheme, dc, iPartId, iStateId, Rectangle.Empty, rectangleSpecified: false, type, out result)) ? 1 : 0;
	}

	private bool GetPartSize(ThemeHandle themeHandle, IDeviceContext dc, int part, int state, Rectangle bounds, bool rectangleSpecified, ThemeSizeType type, out Size result)
	{
		switch (themeHandle)
		{
		case ThemeHandle.BUTTON:
			switch ((BUTTONPARTS)part)
			{
			case BUTTONPARTS.BP_RADIOBUTTON:
				result = GtkPlus.RadioButtonGetSize();
				return true;
			case BUTTONPARTS.BP_CHECKBOX:
				result = GtkPlus.CheckBoxGetSize();
				return true;
			}
			break;
		case ThemeHandle.HEADER:
			if (part != 1)
			{
				break;
			}
			result = new Size(0, ThemeWin32Classic.ListViewGetHeaderHeight());
			return true;
		case ThemeHandle.TRACKBAR:
			switch ((TRACKBARPARTS)part)
			{
			case TRACKBARPARTS.TKP_TRACK:
				result = new Size(0, 4);
				return true;
			case TRACKBARPARTS.TKP_TRACKVERT:
				result = new Size(4, 0);
				return true;
			case TRACKBARPARTS.TKP_THUMB:
			case TRACKBARPARTS.TKP_THUMBVERT:
				result = ThemeWin32Classic.TrackBarGetThumbSize();
				if (part == 6)
				{
					int width = result.Width;
					result.Width = result.Height;
					result.Height = width;
				}
				return true;
			}
			break;
		}
		result = Size.Empty;
		return false;
	}

	public int UxThemeGetThemePosition(IntPtr hTheme, int iPartId, int iStateId, PointProperty prop, out Point result)
	{
		result = Point.Empty;
		return 1;
	}

	public int UxThemeGetThemeString(IntPtr hTheme, int iPartId, int iStateId, StringProperty prop, out string result)
	{
		result = null;
		return 1;
	}

	public int UxThemeGetThemeTextExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string textToDraw, TextFormatFlags flags, Rectangle bounds, out Rectangle result)
	{
		result = Rectangle.Empty;
		return 1;
	}

	public int UxThemeGetThemeTextExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string textToDraw, TextFormatFlags flags, out Rectangle result)
	{
		result = Rectangle.Empty;
		return 1;
	}

	public int UxThemeGetThemeTextMetrics(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, out TextMetrics result)
	{
		result = default(TextMetrics);
		return 1;
	}

	public int UxThemeHitTestThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, HitTestOptions options, Rectangle backgroundRectangle, IntPtr hrgn, Point pt, out HitTestCode result)
	{
		result = HitTestCode.Bottom;
		return 1;
	}

	public bool UxThemeIsAppThemed()
	{
		return true;
	}

	public bool UxThemeIsThemeActive()
	{
		return true;
	}

	public bool UxThemeIsThemeBackgroundPartiallyTransparent(IntPtr hTheme, int iPartId, int iStateId)
	{
		return true;
	}

	public bool UxThemeIsThemePartDefined(IntPtr hTheme, int iPartId)
	{
		switch ((ThemeHandle)(int)hTheme)
		{
		case ThemeHandle.BUTTON:
			switch ((BUTTONPARTS)iPartId)
			{
			case BUTTONPARTS.BP_PUSHBUTTON:
			case BUTTONPARTS.BP_RADIOBUTTON:
			case BUTTONPARTS.BP_CHECKBOX:
			case BUTTONPARTS.BP_GROUPBOX:
				return true;
			default:
				return false;
			}
		case ThemeHandle.COMBOBOX:
			switch ((COMBOBOXPARTS)iPartId)
			{
			case COMBOBOXPARTS.CP_DROPDOWNBUTTON:
			case COMBOBOXPARTS.CP_BORDER:
				return true;
			default:
				return false;
			}
		case ThemeHandle.EDIT:
			if (iPartId == 1)
			{
				return true;
			}
			return false;
		case ThemeHandle.HEADER:
			if (iPartId == 1)
			{
				return true;
			}
			return false;
		case ThemeHandle.PROGRESS:
			switch ((PROGRESSPARTS)iPartId)
			{
			case PROGRESSPARTS.PP_BAR:
			case PROGRESSPARTS.PP_BARVERT:
			case PROGRESSPARTS.PP_CHUNK:
			case PROGRESSPARTS.PP_CHUNKVERT:
				return true;
			default:
				return false;
			}
		case ThemeHandle.REBAR:
			if (iPartId == 3)
			{
				return true;
			}
			return false;
		case ThemeHandle.SCROLLBAR:
			switch ((SCROLLBARPARTS)iPartId)
			{
			case SCROLLBARPARTS.SBP_ARROWBTN:
			case SCROLLBARPARTS.SBP_THUMBBTNHORZ:
			case SCROLLBARPARTS.SBP_THUMBBTNVERT:
			case SCROLLBARPARTS.SBP_LOWERTRACKHORZ:
			case SCROLLBARPARTS.SBP_UPPERTRACKHORZ:
			case SCROLLBARPARTS.SBP_LOWERTRACKVERT:
			case SCROLLBARPARTS.SBP_UPPERTRACKVERT:
				return true;
			default:
				return false;
			}
		case ThemeHandle.SPIN:
			if (iPartId == 1 || iPartId == 2)
			{
				return true;
			}
			return false;
		case ThemeHandle.STATUS:
			if (iPartId == 3)
			{
				return true;
			}
			return false;
		case ThemeHandle.TAB:
			switch ((TABPARTS)iPartId)
			{
			case TABPARTS.TABP_TABITEM:
			case TABPARTS.TABP_TABITEMLEFTEDGE:
			case TABPARTS.TABP_TABITEMRIGHTEDGE:
			case TABPARTS.TABP_TABITEMBOTHEDGE:
			case TABPARTS.TABP_TOPTABITEM:
			case TABPARTS.TABP_TOPTABITEMLEFTEDGE:
			case TABPARTS.TABP_TOPTABITEMRIGHTEDGE:
			case TABPARTS.TABP_TOPTABITEMBOTHEDGE:
			case TABPARTS.TABP_PANE:
				return true;
			default:
				return false;
			}
		case ThemeHandle.TOOLBAR:
			if (iPartId == 1)
			{
				return true;
			}
			return false;
		case ThemeHandle.TRACKBAR:
			switch ((TRACKBARPARTS)iPartId)
			{
			case TRACKBARPARTS.TKP_TRACK:
			case TRACKBARPARTS.TKP_TRACKVERT:
			case TRACKBARPARTS.TKP_THUMB:
			case TRACKBARPARTS.TKP_THUMBVERT:
				return true;
			default:
				return false;
			}
		case ThemeHandle.TREEVIEW:
			if (iPartId == 2)
			{
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public IntPtr UxThemeOpenThemeData(IntPtr hWnd, string classList)
	{
		ThemeHandle themeHandle;
		try
		{
			themeHandle = (ThemeHandle)(int)Enum.Parse(typeof(ThemeHandle), classList);
		}
		catch (ArgumentException)
		{
			return IntPtr.Zero;
		}
		return (IntPtr)(int)themeHandle;
	}

	public void VisualStyleRendererDrawBackgroundExcludingArea(IntPtr theme, IDeviceContext dc, int part, int state, Rectangle bounds, Rectangle excludedArea)
	{
		DrawBackground((ThemeHandle)(int)theme, dc, part, state, bounds, bounds, excludedArea);
	}
}
