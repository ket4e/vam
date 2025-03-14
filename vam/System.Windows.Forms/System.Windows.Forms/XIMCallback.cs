using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
internal class XIMCallback
{
	public IntPtr client_data;

	public XIMProc callback;

	[NonSerialized]
	private GCHandle gch;

	public XIMCallback(IntPtr clientData, XIMProc proc)
	{
		client_data = clientData;
		gch = GCHandle.Alloc(proc);
		callback = proc;
	}

	~XIMCallback()
	{
		gch.Free();
	}
}
