using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace System.Runtime.Remoting.Messaging;

internal class CADMessageBase
{
	protected object[] _args;

	protected byte[] _serializedArgs;

	protected int _propertyCount;

	protected CADArgHolder _callContext;

	internal static int MarshalProperties(IDictionary dict, ref ArrayList args)
	{
		IDictionary dictionary = dict;
		int num = 0;
		if (dict is MethodDictionary methodDictionary)
		{
			if (methodDictionary.HasInternalProperties)
			{
				dictionary = methodDictionary.InternalProperties;
				if (dictionary != null)
				{
					foreach (DictionaryEntry item in dictionary)
					{
						if (args == null)
						{
							args = new ArrayList();
						}
						args.Add(item);
						num++;
					}
				}
			}
		}
		else if (dict != null)
		{
			foreach (DictionaryEntry item2 in dictionary)
			{
				if (args == null)
				{
					args = new ArrayList();
				}
				args.Add(item2);
				num++;
			}
		}
		return num;
	}

	internal static void UnmarshalProperties(IDictionary dict, int count, ArrayList args)
	{
		for (int i = 0; i < count; i++)
		{
			DictionaryEntry dictionaryEntry = (DictionaryEntry)args[i];
			dict[dictionaryEntry.Key] = dictionaryEntry.Value;
		}
	}

	private static bool IsPossibleToIgnoreMarshal(object obj)
	{
		Type type = obj.GetType();
		if (type.IsPrimitive || type == typeof(void))
		{
			return true;
		}
		if (type.IsArray && type.GetElementType().IsPrimitive && ((Array)obj).Rank == 1)
		{
			return true;
		}
		if (obj is string || obj is DateTime || obj is TimeSpan)
		{
			return true;
		}
		return false;
	}

	protected object MarshalArgument(object arg, ref ArrayList args)
	{
		if (arg == null)
		{
			return null;
		}
		if (IsPossibleToIgnoreMarshal(arg))
		{
			return arg;
		}
		if (arg is MarshalByRefObject marshalByRefObject && !RemotingServices.IsTransparentProxy(marshalByRefObject))
		{
			ObjRef o = RemotingServices.Marshal(marshalByRefObject);
			return new CADObjRef(o, Thread.GetDomainID());
		}
		if (args == null)
		{
			args = new ArrayList();
		}
		args.Add(arg);
		return new CADArgHolder(args.Count - 1);
	}

	protected object UnmarshalArgument(object arg, ArrayList args)
	{
		if (arg == null)
		{
			return null;
		}
		if (arg is CADArgHolder cADArgHolder)
		{
			return args[cADArgHolder.index];
		}
		if (arg is CADObjRef cADObjRef)
		{
			string typeName = string.Copy(cADObjRef.TypeName);
			string uri = string.Copy(cADObjRef.URI);
			int sourceDomain = cADObjRef.SourceDomain;
			ChannelInfo cinfo = new ChannelInfo(new CrossAppDomainData(sourceDomain));
			ObjRef objectRef = new ObjRef(typeName, uri, cinfo);
			return RemotingServices.Unmarshal(objectRef);
		}
		if (arg is Array)
		{
			Array array = (Array)arg;
			Array array2 = Type.GetTypeCode(arg.GetType().GetElementType()) switch
			{
				TypeCode.Boolean => new bool[array.Length], 
				TypeCode.Byte => new byte[array.Length], 
				TypeCode.Char => new char[array.Length], 
				TypeCode.Decimal => new decimal[array.Length], 
				TypeCode.Double => new double[array.Length], 
				TypeCode.Int16 => new short[array.Length], 
				TypeCode.Int32 => new int[array.Length], 
				TypeCode.Int64 => new long[array.Length], 
				TypeCode.SByte => new sbyte[array.Length], 
				TypeCode.Single => new float[array.Length], 
				TypeCode.UInt16 => new ushort[array.Length], 
				TypeCode.UInt32 => new uint[array.Length], 
				TypeCode.UInt64 => new ulong[array.Length], 
				_ => throw new NotSupportedException(), 
			};
			array.CopyTo(array2, 0);
			return array2;
		}
		switch (Type.GetTypeCode(arg.GetType()))
		{
		case TypeCode.Boolean:
			return (bool)arg;
		case TypeCode.Byte:
			return (byte)arg;
		case TypeCode.Char:
			return (char)arg;
		case TypeCode.Decimal:
			return (decimal)arg;
		case TypeCode.Double:
			return (double)arg;
		case TypeCode.Int16:
			return (short)arg;
		case TypeCode.Int32:
			return (int)arg;
		case TypeCode.Int64:
			return (long)arg;
		case TypeCode.SByte:
			return (sbyte)arg;
		case TypeCode.Single:
			return (float)arg;
		case TypeCode.UInt16:
			return (ushort)arg;
		case TypeCode.UInt32:
			return (uint)arg;
		case TypeCode.UInt64:
			return (ulong)arg;
		case TypeCode.String:
			return string.Copy((string)arg);
		case TypeCode.DateTime:
			return new DateTime(((DateTime)arg).Ticks);
		default:
			if (arg is TimeSpan timeSpan)
			{
				return new TimeSpan(timeSpan.Ticks);
			}
			if (arg is IntPtr)
			{
				return (IntPtr)arg;
			}
			throw new NotSupportedException(string.Concat("Parameter of type ", arg.GetType(), " cannot be unmarshalled"));
		}
	}

	internal object[] MarshalArguments(object[] arguments, ref ArrayList args)
	{
		object[] array = new object[arguments.Length];
		int num = arguments.Length;
		for (int i = 0; i < num; i++)
		{
			array[i] = MarshalArgument(arguments[i], ref args);
		}
		return array;
	}

	internal object[] UnmarshalArguments(object[] arguments, ArrayList args)
	{
		object[] array = new object[arguments.Length];
		int num = arguments.Length;
		for (int i = 0; i < num; i++)
		{
			array[i] = UnmarshalArgument(arguments[i], args);
		}
		return array;
	}

	protected void SaveLogicalCallContext(IMethodMessage msg, ref ArrayList serializeList)
	{
		if (msg.LogicalCallContext != null && msg.LogicalCallContext.HasInfo)
		{
			if (serializeList == null)
			{
				serializeList = new ArrayList();
			}
			_callContext = new CADArgHolder(serializeList.Count);
			serializeList.Add(msg.LogicalCallContext);
		}
	}

	internal LogicalCallContext GetLogicalCallContext(ArrayList args)
	{
		if (_callContext == null)
		{
			return null;
		}
		return (LogicalCallContext)args[_callContext.index];
	}
}
