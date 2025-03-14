namespace System.Runtime.InteropServices.ComTypes;

[Serializable]
[Flags]
public enum IDLFLAG
{
	IDLFLAG_NONE = 0,
	IDLFLAG_FIN = 1,
	IDLFLAG_FOUT = 2,
	IDLFLAG_FLCID = 4,
	IDLFLAG_FRETVAL = 8
}
