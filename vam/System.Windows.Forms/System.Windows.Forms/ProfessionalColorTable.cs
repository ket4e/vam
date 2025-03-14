using System.Drawing;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public class ProfessionalColorTable
{
	private enum ColorSchemes
	{
		Classic,
		NormalColor,
		HomeStead,
		Metallic,
		MediaCenter,
		Aero
	}

	private bool use_system_colors;

	private Color button_checked_gradient_begin;

	private Color button_checked_gradient_end;

	private Color button_checked_gradient_middle;

	private Color button_checked_highlight;

	private Color button_checked_highlight_border;

	private Color button_pressed_border;

	private Color button_pressed_gradient_begin;

	private Color button_pressed_gradient_end;

	private Color button_pressed_gradient_middle;

	private Color button_pressed_highlight;

	private Color button_pressed_highlight_border;

	private Color button_selected_border;

	private Color button_selected_gradient_begin;

	private Color button_selected_gradient_end;

	private Color button_selected_gradient_middle;

	private Color button_selected_highlight;

	private Color button_selected_highlight_border;

	private Color check_background;

	private Color check_pressed_background;

	private Color check_selected_background;

	private Color grip_dark;

	private Color grip_light;

	private Color image_margin_gradient_begin;

	private Color image_margin_gradient_end;

	private Color image_margin_gradient_middle;

	private Color image_margin_revealed_gradient_begin;

	private Color image_margin_revealed_gradient_end;

	private Color image_margin_revealed_gradient_middle;

	private Color menu_border;

	private Color menu_item_border;

	private Color menu_item_pressed_gradient_begin;

	private Color menu_item_pressed_gradient_end;

	private Color menu_item_pressed_gradient_middle;

	private Color menu_item_selected;

	private Color menu_item_selected_gradient_begin;

	private Color menu_item_selected_gradient_end;

	private Color menu_strip_gradient_begin;

	private Color menu_strip_gradient_end;

	private Color overflow_button_gradient_begin;

	private Color overflow_button_gradient_end;

	private Color overflow_button_gradient_middle;

	private Color rafting_container_gradient_begin;

	private Color rafting_container_gradient_end;

	private Color separator_dark;

	private Color separator_light;

	private Color status_strip_gradient_begin;

	private Color status_strip_gradient_end;

	private Color tool_strip_border;

	private Color tool_strip_content_panel_gradient_begin;

	private Color tool_strip_content_panel_gradient_end;

	private Color tool_strip_drop_down_background;

	private Color tool_strip_gradient_begin;

	private Color tool_strip_gradient_end;

	private Color tool_strip_gradient_middle;

	private Color tool_strip_panel_gradient_begin;

	private Color tool_strip_panel_gradient_end;

	public virtual Color ButtonCheckedGradientBegin => button_checked_gradient_begin;

	public virtual Color ButtonCheckedGradientEnd => button_checked_gradient_end;

	public virtual Color ButtonCheckedGradientMiddle => button_checked_gradient_middle;

	public virtual Color ButtonCheckedHighlight => button_checked_highlight;

	public virtual Color ButtonCheckedHighlightBorder => button_checked_highlight_border;

	public virtual Color ButtonPressedBorder => button_pressed_border;

	public virtual Color ButtonPressedGradientBegin => button_pressed_gradient_begin;

	public virtual Color ButtonPressedGradientEnd => button_pressed_gradient_end;

	public virtual Color ButtonPressedGradientMiddle => button_pressed_gradient_middle;

	public virtual Color ButtonPressedHighlight => button_pressed_highlight;

	public virtual Color ButtonPressedHighlightBorder => button_pressed_highlight_border;

	public virtual Color ButtonSelectedBorder => button_selected_border;

	public virtual Color ButtonSelectedGradientBegin => button_selected_gradient_begin;

	public virtual Color ButtonSelectedGradientEnd => button_selected_gradient_end;

	public virtual Color ButtonSelectedGradientMiddle => button_selected_gradient_middle;

	public virtual Color ButtonSelectedHighlight => button_selected_highlight;

	public virtual Color ButtonSelectedHighlightBorder => button_selected_highlight_border;

	public virtual Color CheckBackground => check_background;

	public virtual Color CheckPressedBackground => check_pressed_background;

	public virtual Color CheckSelectedBackground => check_selected_background;

	public virtual Color GripDark => grip_dark;

	public virtual Color GripLight => grip_light;

	public virtual Color ImageMarginGradientBegin => image_margin_gradient_begin;

	public virtual Color ImageMarginGradientEnd => image_margin_gradient_end;

	public virtual Color ImageMarginGradientMiddle => image_margin_gradient_middle;

	public virtual Color ImageMarginRevealedGradientBegin => image_margin_revealed_gradient_begin;

	public virtual Color ImageMarginRevealedGradientEnd => image_margin_revealed_gradient_end;

	public virtual Color ImageMarginRevealedGradientMiddle => image_margin_revealed_gradient_middle;

	public virtual Color MenuBorder => menu_border;

	public virtual Color MenuItemBorder => menu_item_border;

	public virtual Color MenuItemPressedGradientBegin => menu_item_pressed_gradient_begin;

	public virtual Color MenuItemPressedGradientEnd => menu_item_pressed_gradient_end;

	public virtual Color MenuItemPressedGradientMiddle => menu_item_pressed_gradient_middle;

	public virtual Color MenuItemSelected => menu_item_selected;

	public virtual Color MenuItemSelectedGradientBegin => menu_item_selected_gradient_begin;

	public virtual Color MenuItemSelectedGradientEnd => menu_item_selected_gradient_end;

	public virtual Color MenuStripGradientBegin => menu_strip_gradient_begin;

	public virtual Color MenuStripGradientEnd => menu_strip_gradient_end;

	public virtual Color OverflowButtonGradientBegin => overflow_button_gradient_begin;

	public virtual Color OverflowButtonGradientEnd => overflow_button_gradient_end;

	public virtual Color OverflowButtonGradientMiddle => overflow_button_gradient_middle;

	public virtual Color RaftingContainerGradientBegin => rafting_container_gradient_begin;

	public virtual Color RaftingContainerGradientEnd => rafting_container_gradient_end;

	public virtual Color SeparatorDark => separator_dark;

	public virtual Color SeparatorLight => separator_light;

	public virtual Color StatusStripGradientBegin => status_strip_gradient_begin;

	public virtual Color StatusStripGradientEnd => status_strip_gradient_end;

	public virtual Color ToolStripBorder => tool_strip_border;

	public virtual Color ToolStripContentPanelGradientBegin => tool_strip_content_panel_gradient_begin;

	public virtual Color ToolStripContentPanelGradientEnd => tool_strip_content_panel_gradient_end;

	public virtual Color ToolStripDropDownBackground => tool_strip_drop_down_background;

	public virtual Color ToolStripGradientBegin => tool_strip_gradient_begin;

	public virtual Color ToolStripGradientEnd => tool_strip_gradient_end;

	public virtual Color ToolStripGradientMiddle => tool_strip_gradient_middle;

	public virtual Color ToolStripPanelGradientBegin => tool_strip_panel_gradient_begin;

	public virtual Color ToolStripPanelGradientEnd => tool_strip_panel_gradient_end;

	public bool UseSystemColors
	{
		get
		{
			return use_system_colors;
		}
		set
		{
			if (value != use_system_colors)
			{
				use_system_colors = value;
				CalculateColors();
			}
		}
	}

	public ProfessionalColorTable()
	{
		CalculateColors();
	}

	private void CalculateColors()
	{
		switch (GetCurrentStyle())
		{
		case ColorSchemes.Classic:
			button_checked_gradient_begin = Color.Empty;
			button_checked_gradient_end = Color.Empty;
			button_checked_gradient_middle = Color.Empty;
			button_checked_highlight = Color.FromArgb(184, 191, 211);
			button_checked_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_gradient_begin = Color.FromArgb(133, 146, 181);
			button_pressed_gradient_end = Color.FromArgb(133, 146, 181);
			button_pressed_gradient_middle = Color.FromArgb(133, 146, 181);
			button_pressed_highlight = Color.FromArgb(131, 144, 179);
			button_pressed_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_selected_border = Color.FromKnownColor(KnownColor.Highlight);
			button_selected_gradient_begin = Color.FromArgb(182, 189, 210);
			button_selected_gradient_end = Color.FromArgb(182, 189, 210);
			button_selected_gradient_middle = Color.FromArgb(182, 189, 210);
			button_selected_highlight = Color.FromArgb(184, 191, 211);
			button_selected_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			check_background = Color.FromKnownColor(KnownColor.Highlight);
			check_pressed_background = Color.FromArgb(133, 146, 181);
			check_selected_background = Color.FromArgb(133, 146, 181);
			grip_dark = Color.FromArgb(160, 160, 160);
			grip_light = SystemColors.Window;
			image_margin_gradient_begin = Color.FromArgb(245, 244, 242);
			image_margin_gradient_end = SystemColors.Control;
			image_margin_gradient_middle = Color.FromArgb(234, 232, 228);
			image_margin_revealed_gradient_begin = Color.FromArgb(238, 236, 233);
			image_margin_revealed_gradient_end = Color.FromArgb(216, 213, 206);
			image_margin_revealed_gradient_middle = Color.FromArgb(225, 222, 217);
			menu_border = Color.FromArgb(102, 102, 102);
			menu_item_border = SystemColors.Highlight;
			menu_item_pressed_gradient_begin = Color.FromArgb(245, 244, 242);
			menu_item_pressed_gradient_end = Color.FromArgb(234, 232, 228);
			menu_item_pressed_gradient_middle = Color.FromArgb(225, 222, 217);
			menu_item_selected = SystemColors.Window;
			menu_item_selected_gradient_begin = Color.FromArgb(182, 189, 210);
			menu_item_selected_gradient_end = Color.FromArgb(182, 189, 210);
			menu_strip_gradient_begin = SystemColors.ButtonFace;
			menu_strip_gradient_end = Color.FromArgb(246, 245, 244);
			overflow_button_gradient_begin = Color.FromArgb(225, 222, 217);
			overflow_button_gradient_end = SystemColors.ButtonShadow;
			overflow_button_gradient_middle = Color.FromArgb(216, 213, 206);
			rafting_container_gradient_begin = SystemColors.ButtonFace;
			rafting_container_gradient_end = Color.FromArgb(246, 245, 244);
			separator_dark = Color.FromArgb(166, 166, 166);
			separator_light = SystemColors.ButtonHighlight;
			status_strip_gradient_begin = SystemColors.ButtonFace;
			status_strip_gradient_end = Color.FromArgb(246, 245, 244);
			tool_strip_border = Color.FromArgb(219, 216, 209);
			tool_strip_content_panel_gradient_begin = SystemColors.ButtonFace;
			tool_strip_content_panel_gradient_end = Color.FromArgb(246, 245, 244);
			tool_strip_drop_down_background = SystemColors.Window;
			tool_strip_gradient_begin = Color.FromArgb(245, 244, 242);
			tool_strip_gradient_end = SystemColors.ButtonFace;
			tool_strip_gradient_middle = Color.FromArgb(234, 232, 228);
			tool_strip_panel_gradient_begin = SystemColors.ButtonFace;
			tool_strip_panel_gradient_end = Color.FromArgb(246, 245, 244);
			break;
		case ColorSchemes.NormalColor:
			button_checked_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 223, 154) : Color.Empty);
			button_checked_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 166, 76) : Color.Empty);
			button_checked_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 195, 116) : Color.Empty);
			button_checked_highlight = Color.FromArgb(195, 211, 237);
			button_checked_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_border = ((!use_system_colors) ? Color.FromArgb(0, 0, 128) : Color.FromKnownColor(KnownColor.Highlight));
			button_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(152, 181, 226));
			button_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 223, 154) : Color.FromArgb(152, 181, 226));
			button_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 177, 109) : Color.FromArgb(152, 181, 226));
			button_pressed_highlight = ((!use_system_colors) ? Color.FromArgb(150, 179, 225) : Color.FromArgb(150, 179, 225));
			button_pressed_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_selected_border = ((!use_system_colors) ? Color.FromArgb(0, 0, 128) : Color.FromKnownColor(KnownColor.Highlight));
			button_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 222) : Color.FromArgb(193, 210, 238));
			button_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 203, 136) : Color.FromArgb(193, 210, 238));
			button_selected_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 225, 172) : Color.FromArgb(193, 210, 238));
			button_selected_highlight = ((!use_system_colors) ? Color.FromArgb(195, 211, 237) : Color.FromArgb(195, 211, 237));
			button_selected_highlight_border = ((!use_system_colors) ? Color.FromArgb(0, 0, 128) : Color.FromKnownColor(KnownColor.Highlight));
			check_background = ((!use_system_colors) ? Color.FromArgb(255, 192, 111) : Color.FromKnownColor(KnownColor.Highlight));
			check_pressed_background = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(152, 181, 226));
			check_selected_background = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(152, 181, 226));
			grip_dark = ((!use_system_colors) ? Color.FromArgb(39, 65, 118) : Color.FromArgb(193, 190, 179));
			grip_light = ((!use_system_colors) ? Color.FromArgb(255, 255, 255) : SystemColors.Window);
			image_margin_gradient_begin = ((!use_system_colors) ? Color.FromArgb(227, 239, 255) : Color.FromArgb(251, 250, 246));
			image_margin_gradient_end = ((!use_system_colors) ? Color.FromArgb(123, 164, 224) : SystemColors.Control);
			image_margin_gradient_middle = ((!use_system_colors) ? Color.FromArgb(203, 225, 252) : Color.FromArgb(246, 244, 236));
			image_margin_revealed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(203, 221, 246) : Color.FromArgb(247, 246, 239));
			image_margin_revealed_gradient_end = ((!use_system_colors) ? Color.FromArgb(114, 155, 215) : Color.FromArgb(238, 235, 220));
			image_margin_revealed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(161, 197, 249) : Color.FromArgb(242, 240, 228));
			menu_border = ((!use_system_colors) ? Color.FromArgb(0, 45, 150) : Color.FromArgb(138, 134, 122));
			menu_item_border = ((!use_system_colors) ? Color.FromArgb(0, 0, 128) : SystemColors.Highlight);
			menu_item_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(227, 239, 255) : Color.FromArgb(251, 250, 246));
			menu_item_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(123, 164, 224) : Color.FromArgb(246, 244, 236));
			menu_item_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(161, 197, 249) : Color.FromArgb(242, 240, 228));
			menu_item_selected = ((!use_system_colors) ? Color.FromArgb(255, 238, 194) : SystemColors.Window);
			menu_item_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 222) : Color.FromArgb(193, 210, 238));
			menu_item_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 203, 136) : Color.FromArgb(193, 210, 238));
			menu_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(158, 190, 245) : SystemColors.ButtonFace);
			menu_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(196, 218, 250) : Color.FromArgb(251, 250, 247));
			overflow_button_gradient_begin = ((!use_system_colors) ? Color.FromArgb(127, 177, 250) : Color.FromArgb(242, 240, 228));
			overflow_button_gradient_end = ((!use_system_colors) ? Color.FromArgb(0, 53, 145) : SystemColors.ButtonShadow);
			overflow_button_gradient_middle = ((!use_system_colors) ? Color.FromArgb(82, 127, 208) : Color.FromArgb(238, 235, 220));
			rafting_container_gradient_begin = ((!use_system_colors) ? Color.FromArgb(158, 190, 245) : SystemColors.ButtonFace);
			rafting_container_gradient_end = ((!use_system_colors) ? Color.FromArgb(196, 218, 250) : Color.FromArgb(251, 250, 247));
			separator_dark = ((!use_system_colors) ? Color.FromArgb(106, 140, 203) : Color.FromArgb(197, 194, 184));
			separator_light = ((!use_system_colors) ? Color.FromArgb(241, 249, 255) : SystemColors.ButtonHighlight);
			status_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(158, 190, 245) : SystemColors.ButtonFace);
			status_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(196, 218, 250) : Color.FromArgb(251, 250, 247));
			tool_strip_border = ((!use_system_colors) ? Color.FromArgb(59, 97, 156) : Color.FromArgb(239, 237, 222));
			tool_strip_content_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(158, 190, 245) : SystemColors.ButtonFace);
			tool_strip_content_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(196, 218, 250) : Color.FromArgb(251, 250, 247));
			tool_strip_drop_down_background = ((!use_system_colors) ? Color.FromArgb(246, 246, 246) : Color.FromArgb(252, 252, 249));
			tool_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(227, 239, 255) : Color.FromArgb(251, 250, 246));
			tool_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(123, 164, 224) : SystemColors.ButtonFace);
			tool_strip_gradient_middle = ((!use_system_colors) ? Color.FromArgb(203, 225, 252) : Color.FromArgb(246, 244, 236));
			tool_strip_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(158, 190, 245) : SystemColors.ButtonFace);
			tool_strip_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(196, 218, 250) : Color.FromArgb(251, 250, 247));
			break;
		case ColorSchemes.HomeStead:
			button_checked_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 223, 154) : Color.Empty);
			button_checked_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 166, 76) : Color.Empty);
			button_checked_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 195, 116) : Color.Empty);
			button_checked_highlight = Color.FromArgb(223, 227, 213);
			button_checked_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_border = ((!use_system_colors) ? Color.FromArgb(63, 93, 56) : Color.FromKnownColor(KnownColor.Highlight));
			button_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(201, 208, 184));
			button_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 223, 154) : Color.FromArgb(201, 208, 184));
			button_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 177, 109) : Color.FromArgb(201, 208, 184));
			button_pressed_highlight = ((!use_system_colors) ? Color.FromArgb(200, 206, 182) : Color.FromArgb(200, 206, 182));
			button_pressed_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_selected_border = ((!use_system_colors) ? Color.FromArgb(63, 93, 56) : Color.FromKnownColor(KnownColor.Highlight));
			button_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 222) : Color.FromArgb(223, 227, 212));
			button_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 203, 136) : Color.FromArgb(223, 227, 212));
			button_selected_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 225, 172) : Color.FromArgb(223, 227, 212));
			button_selected_highlight = ((!use_system_colors) ? Color.FromArgb(223, 227, 213) : Color.FromArgb(223, 227, 213));
			button_selected_highlight_border = ((!use_system_colors) ? Color.FromArgb(63, 93, 56) : Color.FromKnownColor(KnownColor.Highlight));
			check_background = ((!use_system_colors) ? Color.FromArgb(255, 192, 111) : Color.FromKnownColor(KnownColor.Highlight));
			check_pressed_background = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(201, 208, 184));
			check_selected_background = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(201, 208, 184));
			grip_dark = ((!use_system_colors) ? Color.FromArgb(81, 94, 51) : Color.FromArgb(193, 190, 179));
			grip_light = ((!use_system_colors) ? Color.FromArgb(255, 255, 255) : SystemColors.Window);
			image_margin_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 237) : Color.FromArgb(251, 250, 246));
			image_margin_gradient_end = ((!use_system_colors) ? Color.FromArgb(181, 196, 143) : SystemColors.Control);
			image_margin_gradient_middle = ((!use_system_colors) ? Color.FromArgb(206, 220, 167) : Color.FromArgb(246, 244, 236));
			image_margin_revealed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(230, 230, 209) : Color.FromArgb(247, 246, 239));
			image_margin_revealed_gradient_end = ((!use_system_colors) ? Color.FromArgb(160, 177, 116) : Color.FromArgb(238, 235, 220));
			image_margin_revealed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(186, 201, 143) : Color.FromArgb(242, 240, 228));
			menu_border = ((!use_system_colors) ? Color.FromArgb(117, 141, 94) : Color.FromArgb(138, 134, 122));
			menu_item_border = ((!use_system_colors) ? Color.FromArgb(63, 93, 56) : SystemColors.Highlight);
			menu_item_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(237, 240, 214) : Color.FromArgb(251, 250, 246));
			menu_item_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(181, 196, 143) : Color.FromArgb(246, 244, 236));
			menu_item_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(186, 201, 143) : Color.FromArgb(242, 240, 228));
			menu_item_selected = ((!use_system_colors) ? Color.FromArgb(255, 238, 194) : SystemColors.Window);
			menu_item_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 222) : Color.FromArgb(223, 227, 212));
			menu_item_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 203, 136) : Color.FromArgb(223, 227, 212));
			menu_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(217, 217, 167) : SystemColors.ButtonFace);
			menu_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(242, 241, 228) : Color.FromArgb(251, 250, 247));
			overflow_button_gradient_begin = ((!use_system_colors) ? Color.FromArgb(186, 204, 150) : Color.FromArgb(242, 240, 228));
			overflow_button_gradient_end = ((!use_system_colors) ? Color.FromArgb(96, 119, 107) : SystemColors.ButtonShadow);
			overflow_button_gradient_middle = ((!use_system_colors) ? Color.FromArgb(141, 160, 107) : Color.FromArgb(238, 235, 220));
			rafting_container_gradient_begin = ((!use_system_colors) ? Color.FromArgb(217, 217, 167) : SystemColors.ButtonFace);
			rafting_container_gradient_end = ((!use_system_colors) ? Color.FromArgb(242, 241, 228) : Color.FromArgb(251, 250, 247));
			separator_dark = ((!use_system_colors) ? Color.FromArgb(96, 128, 88) : Color.FromArgb(197, 194, 184));
			separator_light = ((!use_system_colors) ? Color.FromArgb(244, 247, 222) : SystemColors.ButtonHighlight);
			status_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(217, 217, 167) : SystemColors.ButtonFace);
			status_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(242, 241, 228) : Color.FromArgb(251, 250, 247));
			tool_strip_border = ((!use_system_colors) ? Color.FromArgb(96, 128, 88) : Color.FromArgb(239, 237, 222));
			tool_strip_content_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(217, 217, 167) : SystemColors.ButtonFace);
			tool_strip_content_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(242, 241, 228) : Color.FromArgb(251, 250, 247));
			tool_strip_drop_down_background = ((!use_system_colors) ? Color.FromArgb(244, 244, 238) : Color.FromArgb(252, 252, 249));
			tool_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 237) : Color.FromArgb(251, 250, 246));
			tool_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(181, 196, 143) : SystemColors.ButtonFace);
			tool_strip_gradient_middle = ((!use_system_colors) ? Color.FromArgb(206, 220, 167) : Color.FromArgb(246, 244, 236));
			tool_strip_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(217, 217, 167) : SystemColors.ButtonFace);
			tool_strip_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(242, 241, 228) : Color.FromArgb(251, 250, 247));
			break;
		case ColorSchemes.Metallic:
			button_checked_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 223, 154) : Color.Empty);
			button_checked_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 166, 76) : Color.Empty);
			button_checked_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 195, 116) : Color.Empty);
			button_checked_highlight = Color.FromArgb(231, 232, 235);
			button_checked_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_border = ((!use_system_colors) ? Color.FromArgb(75, 75, 111) : Color.FromKnownColor(KnownColor.Highlight));
			button_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(217, 218, 223));
			button_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 223, 154) : Color.FromArgb(217, 218, 223));
			button_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 177, 109) : Color.FromArgb(217, 218, 223));
			button_pressed_highlight = ((!use_system_colors) ? Color.FromArgb(215, 216, 222) : Color.FromArgb(215, 216, 222));
			button_pressed_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_selected_border = ((!use_system_colors) ? Color.FromArgb(75, 75, 111) : Color.FromKnownColor(KnownColor.Highlight));
			button_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 222) : Color.FromArgb(232, 233, 236));
			button_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 203, 136) : Color.FromArgb(232, 233, 236));
			button_selected_gradient_middle = ((!use_system_colors) ? Color.FromArgb(255, 225, 172) : Color.FromArgb(232, 233, 236));
			button_selected_highlight = ((!use_system_colors) ? Color.FromArgb(231, 232, 235) : Color.FromArgb(231, 232, 235));
			button_selected_highlight_border = ((!use_system_colors) ? Color.FromArgb(75, 75, 111) : Color.FromKnownColor(KnownColor.Highlight));
			check_background = ((!use_system_colors) ? Color.FromArgb(255, 192, 111) : Color.FromKnownColor(KnownColor.Highlight));
			check_pressed_background = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(217, 218, 223));
			check_selected_background = ((!use_system_colors) ? Color.FromArgb(254, 128, 62) : Color.FromArgb(217, 218, 223));
			grip_dark = ((!use_system_colors) ? Color.FromArgb(84, 84, 117) : Color.FromArgb(182, 182, 185));
			grip_light = ((!use_system_colors) ? Color.FromArgb(255, 255, 255) : SystemColors.Window);
			image_margin_gradient_begin = ((!use_system_colors) ? Color.FromArgb(249, 249, 255) : Color.FromArgb(248, 248, 249));
			image_margin_gradient_end = ((!use_system_colors) ? Color.FromArgb(147, 145, 176) : SystemColors.Control);
			image_margin_gradient_middle = ((!use_system_colors) ? Color.FromArgb(225, 226, 236) : Color.FromArgb(240, 239, 241));
			image_margin_revealed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(215, 215, 226) : Color.FromArgb(243, 242, 244));
			image_margin_revealed_gradient_end = ((!use_system_colors) ? Color.FromArgb(118, 116, 151) : Color.FromArgb(227, 226, 230));
			image_margin_revealed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(184, 185, 202) : Color.FromArgb(233, 233, 235));
			menu_border = ((!use_system_colors) ? Color.FromArgb(124, 124, 148) : Color.FromArgb(126, 126, 129));
			menu_item_border = ((!use_system_colors) ? Color.FromArgb(75, 75, 111) : SystemColors.Highlight);
			menu_item_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(232, 233, 242) : Color.FromArgb(248, 248, 249));
			menu_item_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(172, 170, 194) : Color.FromArgb(240, 239, 241));
			menu_item_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(184, 185, 202) : Color.FromArgb(233, 233, 235));
			menu_item_selected = ((!use_system_colors) ? Color.FromArgb(255, 238, 194) : SystemColors.Window);
			menu_item_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(255, 255, 222) : Color.FromArgb(232, 233, 236));
			menu_item_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(255, 203, 136) : Color.FromArgb(232, 233, 236));
			menu_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(215, 215, 229) : SystemColors.ButtonFace);
			menu_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(243, 243, 247) : Color.FromArgb(249, 248, 249));
			overflow_button_gradient_begin = ((!use_system_colors) ? Color.FromArgb(186, 185, 206) : Color.FromArgb(233, 233, 235));
			overflow_button_gradient_end = ((!use_system_colors) ? Color.FromArgb(118, 116, 146) : SystemColors.ButtonShadow);
			overflow_button_gradient_middle = ((!use_system_colors) ? Color.FromArgb(156, 155, 180) : Color.FromArgb(227, 226, 230));
			rafting_container_gradient_begin = ((!use_system_colors) ? Color.FromArgb(215, 215, 229) : SystemColors.ButtonFace);
			rafting_container_gradient_end = ((!use_system_colors) ? Color.FromArgb(243, 243, 247) : Color.FromArgb(249, 248, 249));
			separator_dark = ((!use_system_colors) ? Color.FromArgb(110, 109, 143) : Color.FromArgb(186, 186, 189));
			separator_light = ((!use_system_colors) ? Color.FromArgb(255, 255, 255) : SystemColors.ButtonHighlight);
			status_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(215, 215, 229) : SystemColors.ButtonFace);
			status_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(243, 243, 247) : Color.FromArgb(249, 248, 249));
			tool_strip_border = ((!use_system_colors) ? Color.FromArgb(124, 124, 148) : Color.FromArgb(229, 228, 232));
			tool_strip_content_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(215, 215, 229) : SystemColors.ButtonFace);
			tool_strip_content_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(243, 243, 247) : Color.FromArgb(249, 248, 249));
			tool_strip_drop_down_background = ((!use_system_colors) ? Color.FromArgb(253, 250, 255) : Color.FromArgb(251, 250, 251));
			tool_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(249, 249, 255) : Color.FromArgb(248, 248, 249));
			tool_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(147, 145, 176) : SystemColors.ButtonFace);
			tool_strip_gradient_middle = ((!use_system_colors) ? Color.FromArgb(225, 226, 236) : Color.FromArgb(240, 239, 241));
			tool_strip_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(215, 215, 229) : SystemColors.ButtonFace);
			tool_strip_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(243, 243, 247) : Color.FromArgb(249, 248, 249));
			break;
		case ColorSchemes.MediaCenter:
			button_checked_gradient_begin = ((!use_system_colors) ? Color.FromArgb(226, 229, 238) : Color.Empty);
			button_checked_gradient_end = ((!use_system_colors) ? Color.FromArgb(226, 229, 238) : Color.Empty);
			button_checked_gradient_middle = ((!use_system_colors) ? Color.FromArgb(226, 229, 238) : Color.Empty);
			button_checked_highlight = Color.FromArgb(196, 208, 229);
			button_checked_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_border = ((!use_system_colors) ? Color.FromArgb(51, 94, 168) : Color.FromKnownColor(KnownColor.Highlight));
			button_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(153, 175, 212) : Color.FromArgb(153, 175, 212));
			button_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(153, 175, 212) : Color.FromArgb(153, 175, 212));
			button_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(153, 175, 212) : Color.FromArgb(153, 175, 212));
			button_pressed_highlight = ((!use_system_colors) ? Color.FromArgb(152, 173, 210) : Color.FromArgb(152, 173, 210));
			button_pressed_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_selected_border = ((!use_system_colors) ? Color.FromArgb(51, 94, 168) : Color.FromKnownColor(KnownColor.Highlight));
			button_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(194, 207, 229) : Color.FromArgb(194, 207, 229));
			button_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(194, 207, 229) : Color.FromArgb(194, 207, 229));
			button_selected_gradient_middle = ((!use_system_colors) ? Color.FromArgb(194, 207, 229) : Color.FromArgb(194, 207, 229));
			button_selected_highlight = ((!use_system_colors) ? Color.FromArgb(196, 208, 229) : Color.FromArgb(196, 208, 229));
			button_selected_highlight_border = ((!use_system_colors) ? Color.FromArgb(51, 94, 168) : Color.FromKnownColor(KnownColor.Highlight));
			check_background = ((!use_system_colors) ? Color.FromArgb(226, 229, 238) : Color.FromKnownColor(KnownColor.Highlight));
			check_pressed_background = ((!use_system_colors) ? Color.FromArgb(51, 94, 168) : Color.FromArgb(153, 175, 212));
			check_selected_background = ((!use_system_colors) ? Color.FromArgb(51, 94, 168) : Color.FromArgb(153, 175, 212));
			grip_dark = ((!use_system_colors) ? Color.FromArgb(189, 188, 191) : Color.FromArgb(189, 188, 191));
			grip_light = ((!use_system_colors) ? Color.FromArgb(255, 255, 255) : SystemColors.Window);
			image_margin_gradient_begin = ((!use_system_colors) ? Color.FromArgb(252, 252, 252) : Color.FromArgb(250, 250, 251));
			image_margin_gradient_end = ((!use_system_colors) ? Color.FromArgb(235, 233, 237) : SystemColors.Control);
			image_margin_gradient_middle = ((!use_system_colors) ? Color.FromArgb(245, 244, 246) : Color.FromArgb(245, 244, 246));
			image_margin_revealed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(247, 246, 248) : Color.FromArgb(247, 246, 248));
			image_margin_revealed_gradient_end = ((!use_system_colors) ? Color.FromArgb(228, 226, 230) : Color.FromArgb(237, 235, 239));
			image_margin_revealed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(241, 240, 242) : Color.FromArgb(241, 240, 242));
			menu_border = ((!use_system_colors) ? Color.FromArgb(134, 133, 136) : Color.FromArgb(134, 133, 136));
			menu_item_border = ((!use_system_colors) ? Color.FromArgb(51, 94, 168) : SystemColors.Highlight);
			menu_item_pressed_gradient_begin = ((!use_system_colors) ? Color.FromArgb(252, 252, 252) : Color.FromArgb(250, 250, 251));
			menu_item_pressed_gradient_end = ((!use_system_colors) ? Color.FromArgb(245, 244, 246) : Color.FromArgb(245, 244, 246));
			menu_item_pressed_gradient_middle = ((!use_system_colors) ? Color.FromArgb(241, 240, 242) : Color.FromArgb(241, 240, 242));
			menu_item_selected = ((!use_system_colors) ? Color.FromArgb(194, 207, 229) : SystemColors.Window);
			menu_item_selected_gradient_begin = ((!use_system_colors) ? Color.FromArgb(194, 207, 229) : Color.FromArgb(194, 207, 229));
			menu_item_selected_gradient_end = ((!use_system_colors) ? Color.FromArgb(194, 207, 229) : Color.FromArgb(194, 207, 229));
			menu_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(235, 233, 237) : SystemColors.ButtonFace);
			menu_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(251, 250, 251) : Color.FromArgb(251, 250, 251));
			overflow_button_gradient_begin = ((!use_system_colors) ? Color.FromArgb(242, 242, 242) : Color.FromArgb(241, 240, 242));
			overflow_button_gradient_end = ((!use_system_colors) ? Color.FromArgb(167, 166, 170) : SystemColors.ButtonShadow);
			overflow_button_gradient_middle = ((!use_system_colors) ? Color.FromArgb(224, 224, 225) : Color.FromArgb(237, 235, 239));
			rafting_container_gradient_begin = ((!use_system_colors) ? Color.FromArgb(235, 233, 237) : SystemColors.ButtonFace);
			rafting_container_gradient_end = ((!use_system_colors) ? Color.FromArgb(251, 250, 251) : Color.FromArgb(251, 250, 251));
			separator_dark = ((!use_system_colors) ? Color.FromArgb(193, 193, 196) : Color.FromArgb(193, 193, 196));
			separator_light = ((!use_system_colors) ? Color.FromArgb(255, 255, 255) : SystemColors.ButtonHighlight);
			status_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(235, 233, 237) : SystemColors.ButtonFace);
			status_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(251, 250, 251) : Color.FromArgb(251, 250, 251));
			tool_strip_border = ((!use_system_colors) ? Color.FromArgb(238, 237, 240) : Color.FromArgb(238, 237, 240));
			tool_strip_content_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(235, 233, 237) : SystemColors.ButtonFace);
			tool_strip_content_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(251, 250, 251) : Color.FromArgb(251, 250, 251));
			tool_strip_drop_down_background = ((!use_system_colors) ? Color.FromArgb(252, 252, 252) : Color.FromArgb(252, 252, 252));
			tool_strip_gradient_begin = ((!use_system_colors) ? Color.FromArgb(252, 252, 252) : Color.FromArgb(250, 250, 251));
			tool_strip_gradient_end = ((!use_system_colors) ? Color.FromArgb(235, 233, 237) : SystemColors.ButtonFace);
			tool_strip_gradient_middle = ((!use_system_colors) ? Color.FromArgb(245, 244, 246) : Color.FromArgb(245, 244, 246));
			tool_strip_panel_gradient_begin = ((!use_system_colors) ? Color.FromArgb(235, 233, 237) : SystemColors.ButtonFace);
			tool_strip_panel_gradient_end = ((!use_system_colors) ? Color.FromArgb(251, 250, 251) : Color.FromArgb(251, 250, 251));
			break;
		case ColorSchemes.Aero:
			button_checked_gradient_begin = Color.Empty;
			button_checked_gradient_end = Color.Empty;
			button_checked_gradient_middle = Color.Empty;
			button_checked_highlight = Color.FromArgb(196, 225, 255);
			button_checked_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_border = Color.FromKnownColor(KnownColor.Highlight);
			button_pressed_gradient_begin = Color.FromArgb(153, 204, 255);
			button_pressed_gradient_end = Color.FromArgb(153, 204, 255);
			button_pressed_gradient_middle = Color.FromArgb(153, 204, 255);
			button_pressed_highlight = Color.FromArgb(152, 203, 255);
			button_pressed_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			button_selected_border = ((!use_system_colors) ? Color.FromArgb(51, 94, 168) : Color.FromKnownColor(KnownColor.Highlight));
			button_selected_gradient_begin = Color.FromArgb(194, 224, 255);
			button_selected_gradient_end = Color.FromArgb(194, 224, 255);
			button_selected_gradient_middle = Color.FromArgb(194, 224, 255);
			button_selected_highlight = Color.FromArgb(196, 225, 255);
			button_selected_highlight_border = Color.FromKnownColor(KnownColor.Highlight);
			check_background = Color.FromKnownColor(KnownColor.Highlight);
			check_pressed_background = Color.FromArgb(153, 204, 255);
			check_selected_background = Color.FromArgb(153, 204, 255);
			grip_dark = Color.FromArgb(184, 184, 184);
			grip_light = SystemColors.Window;
			image_margin_gradient_begin = Color.FromArgb(252, 252, 252);
			image_margin_gradient_end = SystemColors.Control;
			image_margin_gradient_middle = Color.FromArgb(250, 250, 250);
			image_margin_revealed_gradient_begin = Color.FromArgb(251, 251, 251);
			image_margin_revealed_gradient_end = Color.FromArgb(245, 245, 245);
			image_margin_revealed_gradient_middle = Color.FromArgb(247, 247, 247);
			menu_border = Color.FromArgb(128, 128, 128);
			menu_item_border = SystemColors.Highlight;
			menu_item_pressed_gradient_begin = Color.FromArgb(252, 252, 252);
			menu_item_pressed_gradient_end = Color.FromArgb(250, 250, 250);
			menu_item_pressed_gradient_middle = Color.FromArgb(247, 247, 247);
			menu_item_selected = SystemColors.Window;
			menu_item_selected_gradient_begin = Color.FromArgb(194, 224, 255);
			menu_item_selected_gradient_end = Color.FromArgb(194, 224, 255);
			menu_strip_gradient_begin = SystemColors.ButtonFace;
			menu_strip_gradient_end = Color.FromArgb(253, 253, 253);
			overflow_button_gradient_begin = Color.FromArgb(247, 247, 247);
			overflow_button_gradient_end = SystemColors.ButtonShadow;
			overflow_button_gradient_middle = Color.FromArgb(245, 245, 245);
			rafting_container_gradient_begin = SystemColors.ButtonFace;
			rafting_container_gradient_end = Color.FromArgb(253, 253, 253);
			separator_dark = Color.FromArgb(189, 189, 189);
			separator_light = SystemColors.ButtonHighlight;
			status_strip_gradient_begin = SystemColors.ButtonFace;
			status_strip_gradient_end = Color.FromArgb(253, 253, 253);
			tool_strip_border = Color.FromArgb(246, 246, 246);
			tool_strip_content_panel_gradient_begin = SystemColors.ButtonFace;
			tool_strip_content_panel_gradient_end = Color.FromArgb(253, 253, 253);
			tool_strip_drop_down_background = Color.FromArgb(253, 253, 253);
			tool_strip_gradient_begin = Color.FromArgb(252, 252, 252);
			tool_strip_gradient_end = SystemColors.ButtonFace;
			tool_strip_gradient_middle = Color.FromArgb(250, 250, 250);
			tool_strip_panel_gradient_begin = SystemColors.ButtonFace;
			tool_strip_panel_gradient_end = Color.FromArgb(253, 253, 253);
			break;
		}
	}

	private ColorSchemes GetCurrentStyle()
	{
		if (!VisualStyleInformation.IsEnabledByUser || string.IsNullOrEmpty(VisualStylesEngine.Instance.VisualStyleInformationFileName))
		{
			return ColorSchemes.Classic;
		}
		return Path.GetFileNameWithoutExtension(VisualStylesEngine.Instance.VisualStyleInformationFileName).ToLowerInvariant() switch
		{
			"aero" => ColorSchemes.Aero, 
			"royale" => ColorSchemes.MediaCenter, 
			_ => VisualStyleInformation.ColorScheme switch
			{
				"NormalColor" => ColorSchemes.NormalColor, 
				"HomeStead" => ColorSchemes.HomeStead, 
				"Metallic" => ColorSchemes.Metallic, 
				_ => ColorSchemes.Classic, 
			}, 
		};
	}
}
