using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Messaging;

[Serializable]
[ComVisible(true)]
[CLSCompliant(false)]
public class MethodResponse : ISerializable, IInternalMessage, IMessage, IMethodMessage, IMethodReturnMessage, ISerializationRootObject
{
	private string _methodName;

	private string _uri;

	private string _typeName;

	private MethodBase _methodBase;

	private object _returnValue;

	private Exception _exception;

	private Type[] _methodSignature;

	private ArgInfo _inArgInfo;

	private object[] _args;

	private object[] _outArgs;

	private IMethodCallMessage _callMsg;

	private LogicalCallContext _callContext;

	private Identity _targetIdentity;

	protected IDictionary ExternalProperties;

	protected IDictionary InternalProperties;

	string IInternalMessage.Uri
	{
		get
		{
			return Uri;
		}
		set
		{
			Uri = value;
		}
	}

	Identity IInternalMessage.TargetIdentity
	{
		get
		{
			return _targetIdentity;
		}
		set
		{
			_targetIdentity = value;
		}
	}

	public int ArgCount
	{
		get
		{
			if (_args == null)
			{
				return 0;
			}
			return _args.Length;
		}
	}

	public object[] Args => _args;

	public Exception Exception => _exception;

	public bool HasVarArgs => (MethodBase.CallingConvention | CallingConventions.VarArgs) != (CallingConventions)0;

	public LogicalCallContext LogicalCallContext
	{
		get
		{
			if (_callContext == null)
			{
				_callContext = new LogicalCallContext();
			}
			return _callContext;
		}
	}

	public MethodBase MethodBase
	{
		get
		{
			if (_methodBase == null)
			{
				if (_callMsg != null)
				{
					_methodBase = _callMsg.MethodBase;
				}
				else if (MethodName != null && TypeName != null)
				{
					_methodBase = RemotingServices.GetMethodBaseFromMethodMessage(this);
				}
			}
			return _methodBase;
		}
	}

	public string MethodName
	{
		get
		{
			if (_methodName == null && _callMsg != null)
			{
				_methodName = _callMsg.MethodName;
			}
			return _methodName;
		}
	}

	public object MethodSignature
	{
		get
		{
			if (_methodSignature == null && _callMsg != null)
			{
				_methodSignature = (Type[])_callMsg.MethodSignature;
			}
			return _methodSignature;
		}
	}

	public int OutArgCount
	{
		get
		{
			if (_args == null || _args.Length == 0)
			{
				return 0;
			}
			if (_inArgInfo == null)
			{
				_inArgInfo = new ArgInfo(MethodBase, ArgInfoType.Out);
			}
			return _inArgInfo.GetInOutArgCount();
		}
	}

	public object[] OutArgs
	{
		get
		{
			if (_outArgs == null && _args != null)
			{
				if (_inArgInfo == null)
				{
					_inArgInfo = new ArgInfo(MethodBase, ArgInfoType.Out);
				}
				_outArgs = _inArgInfo.GetInOutArgs(_args);
			}
			return _outArgs;
		}
	}

	public virtual IDictionary Properties
	{
		get
		{
			if (ExternalProperties == null)
			{
				InternalProperties = ((MethodDictionary)(ExternalProperties = new MethodReturnDictionary(this))).GetInternalProperties();
			}
			return ExternalProperties;
		}
	}

	public object ReturnValue => _returnValue;

	public string TypeName
	{
		get
		{
			if (_typeName == null && _callMsg != null)
			{
				_typeName = _callMsg.TypeName;
			}
			return _typeName;
		}
	}

	public string Uri
	{
		get
		{
			if (_uri == null && _callMsg != null)
			{
				_uri = _callMsg.Uri;
			}
			return _uri;
		}
		set
		{
			_uri = value;
		}
	}

	public MethodResponse(Header[] h1, IMethodCallMessage mcm)
	{
		if (mcm != null)
		{
			_methodName = mcm.MethodName;
			_uri = mcm.Uri;
			_typeName = mcm.TypeName;
			_methodBase = mcm.MethodBase;
			_methodSignature = (Type[])mcm.MethodSignature;
			_args = mcm.Args;
		}
		if (h1 != null)
		{
			foreach (Header header in h1)
			{
				InitMethodProperty(header.Name, header.Value);
			}
		}
	}

	internal MethodResponse(Exception e, IMethodCallMessage msg)
	{
		_callMsg = msg;
		if (msg != null)
		{
			_uri = msg.Uri;
		}
		else
		{
			_uri = string.Empty;
		}
		_exception = e;
		_returnValue = null;
		_outArgs = new object[0];
	}

	internal MethodResponse(object returnValue, object[] outArgs, LogicalCallContext callCtx, IMethodCallMessage msg)
	{
		_callMsg = msg;
		_uri = msg.Uri;
		_exception = null;
		_returnValue = returnValue;
		_args = outArgs;
	}

	internal MethodResponse(IMethodCallMessage msg, CADMethodReturnMessage retmsg)
	{
		_callMsg = msg;
		_methodBase = msg.MethodBase;
		_uri = msg.Uri;
		_methodName = msg.MethodName;
		ArrayList arguments = retmsg.GetArguments();
		_exception = retmsg.GetException(arguments);
		_returnValue = retmsg.GetReturnValue(arguments);
		_args = retmsg.GetArgs(arguments);
		_callContext = retmsg.GetLogicalCallContext(arguments);
		if (_callContext == null)
		{
			_callContext = new LogicalCallContext();
		}
		if (retmsg.PropertiesCount > 0)
		{
			CADMessageBase.UnmarshalProperties(Properties, retmsg.PropertiesCount, arguments);
		}
	}

	internal MethodResponse(SerializationInfo info, StreamingContext context)
	{
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			InitMethodProperty(current.Name, current.Value);
		}
	}

	internal void InitMethodProperty(string key, object value)
	{
		switch (key)
		{
		case "__TypeName":
			_typeName = (string)value;
			break;
		case "__MethodName":
			_methodName = (string)value;
			break;
		case "__MethodSignature":
			_methodSignature = (Type[])value;
			break;
		case "__Uri":
			_uri = (string)value;
			break;
		case "__Return":
			_returnValue = value;
			break;
		case "__OutArgs":
			_args = (object[])value;
			break;
		case "__fault":
			_exception = (Exception)value;
			break;
		case "__CallContext":
			_callContext = (LogicalCallContext)value;
			break;
		default:
			Properties[key] = value;
			break;
		}
	}

	public object GetArg(int argNum)
	{
		if (_args == null)
		{
			return null;
		}
		return _args[argNum];
	}

	public string GetArgName(int index)
	{
		return MethodBase.GetParameters()[index].Name;
	}

	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (_exception == null)
		{
			info.AddValue("__TypeName", _typeName);
			info.AddValue("__MethodName", _methodName);
			info.AddValue("__MethodSignature", _methodSignature);
			info.AddValue("__Uri", _uri);
			info.AddValue("__Return", _returnValue);
			info.AddValue("__OutArgs", _args);
		}
		else
		{
			info.AddValue("__fault", _exception);
		}
		info.AddValue("__CallContext", _callContext);
		if (InternalProperties == null)
		{
			return;
		}
		foreach (DictionaryEntry internalProperty in InternalProperties)
		{
			info.AddValue((string)internalProperty.Key, internalProperty.Value);
		}
	}

	public object GetOutArg(int argNum)
	{
		if (_args == null)
		{
			return null;
		}
		if (_inArgInfo == null)
		{
			_inArgInfo = new ArgInfo(MethodBase, ArgInfoType.Out);
		}
		return _args[_inArgInfo.GetInOutArgIndex(argNum)];
	}

	public string GetOutArgName(int index)
	{
		if (_methodBase == null)
		{
			return "__method_" + index;
		}
		if (_inArgInfo == null)
		{
			_inArgInfo = new ArgInfo(MethodBase, ArgInfoType.Out);
		}
		return _inArgInfo.GetInOutArgName(index);
	}

	[MonoTODO]
	public virtual object HeaderHandler(Header[] h)
	{
		throw new NotImplementedException();
	}

	[MonoTODO]
	public void RootSetObjectData(SerializationInfo info, StreamingContext ctx)
	{
		throw new NotImplementedException();
	}
}
