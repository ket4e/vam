using System.Collections;
using System.Security.Permissions;

namespace System.Drawing.Design;

[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
public sealed class ToolboxItemCollection : ReadOnlyCollectionBase
{
	public ToolboxItem this[int index] => (ToolboxItem)base.InnerList[index];

	public ToolboxItemCollection(ToolboxItem[] value)
	{
		base.InnerList.AddRange(value);
	}

	public ToolboxItemCollection(ToolboxItemCollection value)
	{
		base.InnerList.AddRange(value);
	}

	public bool Contains(ToolboxItem value)
	{
		return base.InnerList.Contains(value);
	}

	public void CopyTo(ToolboxItem[] array, int index)
	{
		base.InnerList.CopyTo(array, index);
	}

	public int IndexOf(ToolboxItem value)
	{
		return base.InnerList.IndexOf(value);
	}
}
