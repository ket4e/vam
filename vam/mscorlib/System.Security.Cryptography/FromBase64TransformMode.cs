using System.Runtime.InteropServices;

namespace System.Security.Cryptography;

[Serializable]
[ComVisible(true)]
public enum FromBase64TransformMode
{
	IgnoreWhiteSpaces,
	DoNotIgnoreWhiteSpaces
}
