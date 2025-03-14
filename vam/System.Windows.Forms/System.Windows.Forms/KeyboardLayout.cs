namespace System.Windows.Forms;

[Serializable]
internal class KeyboardLayout
{
	public int Lcid;

	public string Name;

	public ScanTableIndex ScanIndex;

	public VKeyTableIndex VKeyIndex;

	public uint[][] Keys;

	public KeyboardLayout(int lcid, string name, ScanTableIndex scan_index, VKeyTableIndex vkey_index, uint[][] keys)
	{
		Lcid = lcid;
		Name = name;
		ScanIndex = scan_index;
		VKeyIndex = vkey_index;
		Keys = keys;
	}

	public KeyboardLayout(int lcid, string name, int scan_index, int vkey_index, uint[][] keys)
		: this(lcid, name, (ScanTableIndex)scan_index, (VKeyTableIndex)vkey_index, keys)
	{
	}
}
