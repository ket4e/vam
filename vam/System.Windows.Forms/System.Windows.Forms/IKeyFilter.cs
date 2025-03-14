namespace System.Windows.Forms;

internal interface IKeyFilter
{
	bool PreFilterKey(KeyFilterData data);
}
