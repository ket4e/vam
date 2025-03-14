using System.Reflection;
using System.Runtime.Remoting.Proxies;
using System.Threading;

namespace System.Runtime.Remoting.Messaging;

internal class StackBuilderSink : IMessageSink
{
	private MarshalByRefObject _target;

	private RealProxy _rp;

	public IMessageSink NextSink => null;

	public StackBuilderSink(MarshalByRefObject obj, bool forceInternalExecute)
	{
		_target = obj;
		if (!forceInternalExecute && RemotingServices.IsTransparentProxy(obj))
		{
			_rp = RemotingServices.GetRealProxy(obj);
		}
	}

	public IMessage SyncProcessMessage(IMessage msg)
	{
		CheckParameters(msg);
		if (_rp != null)
		{
			return _rp.Invoke(msg);
		}
		return RemotingServices.InternalExecuteMessage(_target, (IMethodCallMessage)msg);
	}

	public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
	{
		object[] state = new object[2] { msg, replySink };
		ThreadPool.QueueUserWorkItem(ExecuteAsyncMessage, state);
		return null;
	}

	private void ExecuteAsyncMessage(object ob)
	{
		object[] array = (object[])ob;
		IMethodCallMessage methodCallMessage = (IMethodCallMessage)array[0];
		IMessageSink messageSink = (IMessageSink)array[1];
		CheckParameters(methodCallMessage);
		IMessage msg = ((_rp == null) ? RemotingServices.InternalExecuteMessage(_target, methodCallMessage) : _rp.Invoke(methodCallMessage));
		messageSink.SyncProcessMessage(msg);
	}

	private void CheckParameters(IMessage msg)
	{
		IMethodCallMessage methodCallMessage = (IMethodCallMessage)msg;
		ParameterInfo[] parameters = methodCallMessage.MethodBase.GetParameters();
		int num = 0;
		ParameterInfo[] array = parameters;
		foreach (ParameterInfo parameterInfo in array)
		{
			object arg = methodCallMessage.GetArg(num++);
			Type type = parameterInfo.ParameterType;
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			if (arg != null && !type.IsInstanceOfType(arg))
			{
				throw new RemotingException("Cannot cast argument " + parameterInfo.Position + " of type '" + arg.GetType().AssemblyQualifiedName + "' to type '" + type.AssemblyQualifiedName + "'");
			}
		}
	}
}
