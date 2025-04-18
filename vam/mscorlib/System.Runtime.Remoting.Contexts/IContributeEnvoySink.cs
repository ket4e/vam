using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Contexts;

[ComVisible(true)]
public interface IContributeEnvoySink
{
	IMessageSink GetEnvoySink(MarshalByRefObject obj, IMessageSink nextSink);
}
