using System.Drawing;

namespace System.Windows.Forms;

public class SearchForVirtualItemEventArgs : EventArgs
{
	private SearchDirectionHint direction;

	private bool include_sub_items_in_search;

	private int index;

	private bool is_prefix_search;

	private bool is_text_search;

	private int start_index;

	private Point starting_point;

	private string text;

	public SearchDirectionHint Direction => direction;

	public bool IncludeSubItemsInSearch => include_sub_items_in_search;

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public bool IsPrefixSearch => is_prefix_search;

	public bool IsTextSearch => is_text_search;

	public int StartIndex => start_index;

	public Point StartingPoint => starting_point;

	public string Text => text;

	public SearchForVirtualItemEventArgs(bool isTextSearch, bool isPrefixSearch, bool includeSubItemsInSearch, string text, Point startingPoint, SearchDirectionHint direction, int startIndex)
	{
		is_text_search = isTextSearch;
		is_prefix_search = isPrefixSearch;
		include_sub_items_in_search = includeSubItemsInSearch;
		this.text = text;
		starting_point = startingPoint;
		this.direction = direction;
		start_index = startIndex;
		index = -1;
	}
}
