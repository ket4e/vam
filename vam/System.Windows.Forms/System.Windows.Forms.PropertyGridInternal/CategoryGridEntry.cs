namespace System.Windows.Forms.PropertyGridInternal;

internal class CategoryGridEntry : GridEntry
{
	private string label;

	public override GridItemType GridItemType => GridItemType.Category;

	public override bool Expandable => GridItems.Count > 0;

	public override string Label => label;

	public override bool IsReadOnly => true;

	public override bool IsEditable => false;

	public override bool IsResetable => false;

	public CategoryGridEntry(PropertyGrid owner, string category, GridEntry parent)
		: base(owner, parent)
	{
		label = category;
	}
}
