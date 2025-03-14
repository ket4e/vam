namespace System.IO.Pipes;

[Serializable]
[Flags]
public enum PipeOptions
{
	None = 0,
	WriteThrough = 1,
	Asynchronous = 2
}
