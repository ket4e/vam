using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.Threading;

[Serializable]
public sealed class ExecutionContext : ISerializable
{
	private SecurityContext _sc;

	private bool _suppressFlow;

	private bool _capture;

	internal SecurityContext SecurityContext
	{
		get
		{
			if (_sc == null)
			{
				_sc = new SecurityContext();
			}
			return _sc;
		}
		set
		{
			_sc = value;
		}
	}

	internal bool FlowSuppressed
	{
		get
		{
			return _suppressFlow;
		}
		set
		{
			_suppressFlow = value;
		}
	}

	internal ExecutionContext()
	{
	}

	internal ExecutionContext(ExecutionContext ec)
	{
		if (ec._sc != null)
		{
			_sc = new SecurityContext(ec._sc);
		}
		_suppressFlow = ec._suppressFlow;
		_capture = true;
	}

	[MonoTODO]
	internal ExecutionContext(SerializationInfo info, StreamingContext context)
	{
		throw new NotImplementedException();
	}

	public static ExecutionContext Capture()
	{
		ExecutionContext executionContext = Thread.CurrentThread.ExecutionContext;
		if (executionContext.FlowSuppressed)
		{
			return null;
		}
		ExecutionContext executionContext2 = new ExecutionContext(executionContext);
		if (SecurityManager.SecurityEnabled)
		{
			executionContext2.SecurityContext = SecurityContext.Capture();
		}
		return executionContext2;
	}

	public ExecutionContext CreateCopy()
	{
		if (!_capture)
		{
			throw new InvalidOperationException();
		}
		return new ExecutionContext(this);
	}

	[MonoTODO]
	[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"MemberAccess\"/>\n</PermissionSet>\n")]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		throw new NotImplementedException();
	}

	public static bool IsFlowSuppressed()
	{
		return Thread.CurrentThread.ExecutionContext.FlowSuppressed;
	}

	public static void RestoreFlow()
	{
		ExecutionContext executionContext = Thread.CurrentThread.ExecutionContext;
		if (!executionContext.FlowSuppressed)
		{
			throw new InvalidOperationException();
		}
		executionContext.FlowSuppressed = false;
	}

	[MonoTODO("only the SecurityContext is considered")]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
	public static void Run(ExecutionContext executionContext, ContextCallback callback, object state)
	{
		if (executionContext == null)
		{
			throw new InvalidOperationException(Locale.GetText("Null ExecutionContext"));
		}
		SecurityContext.Run(executionContext.SecurityContext, callback, state);
	}

	public static AsyncFlowControl SuppressFlow()
	{
		Thread currentThread = Thread.CurrentThread;
		currentThread.ExecutionContext.FlowSuppressed = true;
		return new AsyncFlowControl(currentThread, AsyncFlowControlType.Execution);
	}
}
