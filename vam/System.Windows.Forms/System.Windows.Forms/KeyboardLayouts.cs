using System.Reflection;
using System.Resources;

namespace System.Windows.Forms;

internal class KeyboardLayouts
{
	private KeyboardLayout[] keyboard_layouts;

	public int[][] vkey_table;

	public short[][] scan_table;

	public KeyboardLayout[] Layouts
	{
		get
		{
			if (keyboard_layouts == null)
			{
				LoadLayouts();
			}
			return keyboard_layouts;
		}
	}

	public void LoadLayouts()
	{
		ResourceManager resourceManager = new ResourceManager("keyboards", Assembly.GetExecutingAssembly());
		keyboard_layouts = (KeyboardLayout[])resourceManager.GetObject("keyboard_table");
		vkey_table = (int[][])resourceManager.GetObject("vkey_table");
		scan_table = (short[][])resourceManager.GetObject("scan_table");
	}
}
