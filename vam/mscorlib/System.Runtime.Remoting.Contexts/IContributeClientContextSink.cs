using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Contexts;

[ComVisible(true)]
public interface IContributeClientContextSink
{
	IMessageSink GetClientContextSink(IMessageSink nextSink);
}
