namespace System.Drawing.Printing;

[Serializable]
public enum PrintRange
{
	AllPages = 0,
	Selection = 1,
	SomePages = 2,
	CurrentPage = 0x400000
}
