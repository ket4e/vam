using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace System.Drawing.Design;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("4BACD258-DE64-4048-BC4E-FEDBEF9ACB76")]
public interface IToolboxService
{
	CategoryNameCollection CategoryNames { get; }

	string SelectedCategory { get; set; }

	void AddCreator(ToolboxItemCreatorCallback creator, string format);

	void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host);

	void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host);

	void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host);

	void AddToolboxItem(ToolboxItem toolboxItem, string category);

	void AddToolboxItem(ToolboxItem toolboxItem);

	ToolboxItem DeserializeToolboxItem(object serializedObject);

	ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host);

	ToolboxItem GetSelectedToolboxItem();

	ToolboxItem GetSelectedToolboxItem(IDesignerHost host);

	ToolboxItemCollection GetToolboxItems();

	ToolboxItemCollection GetToolboxItems(IDesignerHost host);

	ToolboxItemCollection GetToolboxItems(string category);

	ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host);

	bool IsSupported(object serializedObject, ICollection filterAttributes);

	bool IsSupported(object serializedObject, IDesignerHost host);

	bool IsToolboxItem(object serializedObject);

	bool IsToolboxItem(object serializedObject, IDesignerHost host);

	void Refresh();

	void RemoveCreator(string format);

	void RemoveCreator(string format, IDesignerHost host);

	void RemoveToolboxItem(ToolboxItem toolboxItem);

	void RemoveToolboxItem(ToolboxItem toolboxItem, string category);

	void SelectedToolboxItemUsed();

	object SerializeToolboxItem(ToolboxItem toolboxItem);

	bool SetCursor();

	void SetSelectedToolboxItem(ToolboxItem toolboxItem);
}
