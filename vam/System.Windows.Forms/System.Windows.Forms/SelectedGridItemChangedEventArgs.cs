namespace System.Windows.Forms;

public class SelectedGridItemChangedEventArgs : EventArgs
{
	private GridItem new_selection;

	private GridItem old_selection;

	public GridItem NewSelection => new_selection;

	public GridItem OldSelection => old_selection;

	public SelectedGridItemChangedEventArgs(GridItem oldSel, GridItem newSel)
	{
		old_selection = oldSel;
		new_selection = newSel;
	}
}
