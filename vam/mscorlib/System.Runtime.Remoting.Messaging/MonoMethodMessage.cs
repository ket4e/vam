using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Runtime.Remoting.Messaging;

[Serializable]
internal class MonoMethodMessage : IInternalMessage, IMessage, IMethodCallMessage, IMethodMessage, IMethodReturnMessage
{
	private MonoMethod method;

	private object[] args;

	private string[] names;

	private byte[] arg_types;

	public LogicalCallContext ctx;

	public object rval;

	public Exception exc;

	private AsyncResult asyncResult;

	private CallType call_type;

	private string uri;

	private MethodCallDictionary properties;

	private Type[] methodSignature;

	private Identity identity;

	Identity IInternalMessage.TargetIdentity
	{
		get
		{
			return identity;
		}
		set
		{
			identity = value;
		}
	}

	public IDictionary Properties
	{
		get
		{
			if (properties == null)
			{
				properties = new MethodCallDictionary(this);
			}
			return properties;
		}
	}

	public int ArgCount
	{
		get
		{
			if (CallType == CallType.EndInvoke)
			{
				return -1;
			}
			if (args == null)
			{
				return 0;
			}
			return args.Length;
		}
	}

	public object[] Args => args;

	public bool HasVarArgs => false;

	public LogicalCallContext LogicalCallContext
	{
		get
		{
			return ctx;
		}
		set
		{
			ctx = value;
		}
	}

	public MethodBase MethodBase => method;

	public string MethodName
	{
		get
		{
			if (method == null)
			{
				return string.Empty;
			}
			return method.Name;
		}
	}

	public object MethodSignature
	{
		get
		{
			if (methodSignature == null)
			{
				ParameterInfo[] parameters = method.GetParameters();
				methodSignature = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					methodSignature[i] = parameters[i].ParameterType;
				}
			}
			return methodSignature;
		}
	}

	public string TypeName
	{
		get
		{
			if (method == null)
			{
				return string.Empty;
			}
			return method.DeclaringType.AssemblyQualifiedName;
		}
	}

	public string Uri
	{
		get
		{
			return uri;
		}
		set
		{
			uri = value;
		}
	}

	public int InArgCount
	{
		get
		{
			if (CallType == CallType.EndInvoke)
			{
				return -1;
			}
			if (args == null)
			{
				return 0;
			}
			int num = 0;
			byte[] array = arg_types;
			foreach (byte b in array)
			{
				if (((uint)b & (true ? 1u : 0u)) != 0)
				{
					num++;
				}
			}
			return num;
		}
	}

	public object[] InArgs
	{
		get
		{
			int inArgCount = InArgCount;
			object[] array = new object[inArgCount];
			int num;
			int num2 = (num = 0);
			byte[] array2 = arg_types;
			foreach (byte b in array2)
			{
				if (((uint)b & (true ? 1u : 0u)) != 0)
				{
					array[num++] = args[num2];
				}
				num2++;
			}
			return array;
		}
	}

	public Exception Exception => exc;

	public int OutArgCount
	{
		get
		{
			if (args == null)
			{
				return 0;
			}
			int num = 0;
			byte[] array = arg_types;
			foreach (byte b in array)
			{
				if ((b & 2u) != 0)
				{
					num++;
				}
			}
			return num;
		}
	}

	public object[] OutArgs
	{
		get
		{
			if (args == null)
			{
				return null;
			}
			int outArgCount = OutArgCount;
			object[] array = new object[outArgCount];
			int num;
			int num2 = (num = 0);
			byte[] array2 = arg_types;
			foreach (byte b in array2)
			{
				if ((b & 2u) != 0)
				{
					array[num++] = args[num2];
				}
				num2++;
			}
			return array;
		}
	}

	public object ReturnValue => rval;

	public bool IsAsync => asyncResult != null;

	public AsyncResult AsyncResult => asyncResult;

	internal CallType CallType
	{
		get
		{
			if (call_type == CallType.Sync && RemotingServices.IsOneWay(method))
			{
				call_type = CallType.OneWay;
			}
			return call_type;
		}
	}

	public MonoMethodMessage(MethodBase method, object[] out_args)
	{
		if (method != null)
		{
			InitMessage((MonoMethod)method, out_args);
		}
		else
		{
			args = null;
		}
	}

	public MonoMethodMessage(Type type, string method_name, object[] in_args)
	{
		MethodInfo methodInfo = type.GetMethod(method_name);
		InitMessage((MonoMethod)methodInfo, null);
		int num = in_args.Length;
		for (int i = 0; i < num; i++)
		{
			args[i] = in_args[i];
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void InitMessage(MonoMethod method, object[] out_args);

	public object GetArg(int arg_num)
	{
		if (args == null)
		{
			return null;
		}
		return args[arg_num];
	}

	public string GetArgName(int arg_num)
	{
		if (args == null)
		{
			return string.Empty;
		}
		return names[arg_num];
	}

	public object GetInArg(int arg_num)
	{
		int num = 0;
		int num2 = 0;
		byte[] array = arg_types;
		foreach (byte b in array)
		{
			if (((uint)b & (true ? 1u : 0u)) != 0 && num2++ == arg_num)
			{
				return args[num];
			}
			num++;
		}
		return null;
	}

	public string GetInArgName(int arg_num)
	{
		int num = 0;
		int num2 = 0;
		byte[] array = arg_types;
		foreach (byte b in array)
		{
			if (((uint)b & (true ? 1u : 0u)) != 0 && num2++ == arg_num)
			{
				return names[num];
			}
			num++;
		}
		return null;
	}

	public object GetOutArg(int arg_num)
	{
		int num = 0;
		int num2 = 0;
		byte[] array = arg_types;
		foreach (byte b in array)
		{
			if ((b & 2u) != 0 && num2++ == arg_num)
			{
				return args[num];
			}
			num++;
		}
		return null;
	}

	public string GetOutArgName(int arg_num)
	{
		int num = 0;
		int num2 = 0;
		byte[] array = arg_types;
		foreach (byte b in array)
		{
			if ((b & 2u) != 0 && num2++ == arg_num)
			{
				return names[num];
			}
			num++;
		}
		return null;
	}

	public bool NeedsOutProcessing(out int outCount)
	{
		bool flag = false;
		outCount = 0;
		byte[] array = arg_types;
		foreach (byte b in array)
		{
			if ((b & 2u) != 0)
			{
				outCount++;
			}
			else if ((b & 4u) != 0)
			{
				flag = true;
			}
		}
		return outCount > 0 || flag;
	}
}
