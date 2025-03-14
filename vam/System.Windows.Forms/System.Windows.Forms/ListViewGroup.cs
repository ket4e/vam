using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

[Serializable]
[DefaultProperty("Header")]
[DesignTimeVisible(false)]
[ToolboxItem(false)]
[TypeConverter(typeof(ListViewGroupConverter))]
public sealed class ListViewGroup : ISerializable
{
	internal string header = string.Empty;

	private string name;

	private HorizontalAlignment header_alignment;

	private ListView list_view_owner;

	private ListView.ListViewItemCollection items;

	private object tag;

	private Rectangle header_bounds = Rectangle.Empty;

	internal int starting_row;

	internal int starting_item;

	internal int rows;

	internal int current_item;

	internal Point items_area_location;

	private bool is_default_group;

	private int item_count;

	public string Header
	{
		get
		{
			return header;
		}
		set
		{
			if (!header.Equals(value))
			{
				header = value;
				if (list_view_owner != null)
				{
					list_view_owner.Redraw(recalculate: true);
				}
			}
		}
	}

	[DefaultValue(HorizontalAlignment.Left)]
	public HorizontalAlignment HeaderAlignment
	{
		get
		{
			return header_alignment;
		}
		set
		{
			if (!header_alignment.Equals(value))
			{
				if (value != 0 && value != HorizontalAlignment.Right && value != HorizontalAlignment.Center)
				{
					throw new InvalidEnumArgumentException("HeaderAlignment", (int)value, typeof(HorizontalAlignment));
				}
				header_alignment = value;
				if (list_view_owner != null)
				{
					list_view_owner.Redraw(recalculate: true);
				}
			}
		}
	}

	[Browsable(false)]
	public ListView.ListViewItemCollection Items => items;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ListView ListView => list_view_owner;

	internal ListView ListViewOwner
	{
		get
		{
			return list_view_owner;
		}
		set
		{
			list_view_owner = value;
			if (!is_default_group)
			{
				items.Owner = value;
			}
		}
	}

	internal Rectangle HeaderBounds
	{
		get
		{
			Rectangle result = header_bounds;
			result.X -= list_view_owner.h_marker;
			result.Y -= list_view_owner.v_marker;
			return result;
		}
		set
		{
			if (list_view_owner != null)
			{
				list_view_owner.item_control.Invalidate(HeaderBounds);
			}
			header_bounds = value;
			if (list_view_owner != null)
			{
				list_view_owner.item_control.Invalidate(HeaderBounds);
			}
		}
	}

	internal bool IsDefault
	{
		get
		{
			return is_default_group;
		}
		set
		{
			is_default_group = value;
		}
	}

	internal int ItemCount
	{
		get
		{
			return (!is_default_group) ? items.Count : item_count;
		}
		set
		{
			if (!is_default_group)
			{
				throw new InvalidOperationException("ItemCount cannot be set for non-default groups.");
			}
			item_count = value;
		}
	}

	[DefaultValue("")]
	[Browsable(true)]
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
	[Localizable(false)]
	[Bindable(true)]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public ListViewGroup()
		: this("ListViewGroup", HorizontalAlignment.Left)
	{
	}

	public ListViewGroup(string header)
		: this(header, HorizontalAlignment.Left)
	{
	}

	public ListViewGroup(string key, string headerText)
		: this(headerText, HorizontalAlignment.Left)
	{
		name = key;
	}

	public ListViewGroup(string header, HorizontalAlignment headerAlignment)
	{
		this.header = header;
		header_alignment = headerAlignment;
		items = new ListView.ListViewItemCollection(list_view_owner, this);
	}

	private ListViewGroup(SerializationInfo info, StreamingContext context)
	{
		header = info.GetString("Header");
		name = info.GetString("Name");
		header_alignment = (HorizontalAlignment)info.GetInt32("HeaderAlignment");
		tag = info.GetValue("Tag", typeof(object));
		int @int = info.GetInt32("ListViewItemCount");
		if (@int > 0)
		{
			if (items == null)
			{
				items = new ListView.ListViewItemCollection(list_view_owner);
			}
			for (int i = 0; i < @int; i++)
			{
				items.Add((ListViewItem)info.GetValue($"ListViewItem_{i}", typeof(ListViewItem)));
			}
		}
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Header", header);
		info.AddValue("Name", name);
		info.AddValue("HeaderAlignment", header_alignment);
		info.AddValue("Tag", tag);
		info.AddValue("ListViewItemCount", items.Count);
		int num = 0;
		foreach (ListViewItem item in items)
		{
			info.AddValue($"ListViewItem_{num}", item);
			num++;
		}
	}

	internal int GetActualItemCount()
	{
		if (is_default_group)
		{
			return item_count;
		}
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].ListView != null)
			{
				num++;
			}
		}
		return num;
	}

	public override string ToString()
	{
		return header;
	}
}
