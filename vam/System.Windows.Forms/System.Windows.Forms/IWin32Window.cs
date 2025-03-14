using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("458AB8A2-A1EA-4d7b-8EBE-DEE5D3D9442C")]
[ComVisible(true)]
public interface IWin32Window
{
	IntPtr Handle { get; }
}
