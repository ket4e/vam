using System.Runtime.Serialization;

namespace System.Reflection;

[Serializable]
internal class MemberInfoSerializationHolder : ISerializable, IObjectReference
{
	private const BindingFlags DefaultBinding = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	private readonly string _memberName;

	private readonly string _memberSignature;

	private readonly MemberTypes _memberType;

	private readonly Type _reflectedType;

	private readonly Type[] _genericArguments;

	private MemberInfoSerializationHolder(SerializationInfo info, StreamingContext ctx)
	{
		string @string = info.GetString("AssemblyName");
		string string2 = info.GetString("ClassName");
		_memberName = info.GetString("Name");
		_memberSignature = info.GetString("Signature");
		_memberType = (MemberTypes)info.GetInt32("MemberType");
		try
		{
			_genericArguments = null;
		}
		catch (SerializationException)
		{
		}
		Assembly assembly = Assembly.Load(@string);
		_reflectedType = assembly.GetType(string2, throwOnError: true, ignoreCase: true);
	}

	public static void Serialize(SerializationInfo info, string name, Type klass, string signature, MemberTypes type)
	{
		Serialize(info, name, klass, signature, type, null);
	}

	public static void Serialize(SerializationInfo info, string name, Type klass, string signature, MemberTypes type, Type[] genericArguments)
	{
		info.SetType(typeof(MemberInfoSerializationHolder));
		info.AddValue("AssemblyName", klass.Module.Assembly.FullName, typeof(string));
		info.AddValue("ClassName", klass.FullName, typeof(string));
		info.AddValue("Name", name, typeof(string));
		info.AddValue("Signature", signature, typeof(string));
		info.AddValue("MemberType", (int)type);
		info.AddValue("GenericArguments", genericArguments, typeof(Type[]));
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new NotSupportedException();
	}

	public object GetRealObject(StreamingContext context)
	{
		switch (_memberType)
		{
		case MemberTypes.Constructor:
		{
			ConstructorInfo[] constructors = _reflectedType.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < constructors.Length; i++)
			{
				if (constructors[i].ToString().Equals(_memberSignature))
				{
					return constructors[i];
				}
			}
			throw new SerializationException($"Could not find constructor '{_memberSignature}' in type '{_reflectedType}'");
		}
		case MemberTypes.Method:
		{
			MethodInfo[] methods = _reflectedType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int j = 0; j < methods.Length; j++)
			{
				if (methods[j].ToString().Equals(_memberSignature))
				{
					return methods[j];
				}
				if (_genericArguments != null && methods[j].IsGenericMethod && methods[j].GetGenericArguments().Length == _genericArguments.Length)
				{
					MethodInfo methodInfo = methods[j].MakeGenericMethod(_genericArguments);
					if (methodInfo.ToString() == _memberSignature)
					{
						return methodInfo;
					}
				}
			}
			throw new SerializationException($"Could not find method '{_memberSignature}' in type '{_reflectedType}'");
		}
		case MemberTypes.Field:
		{
			FieldInfo field = _reflectedType.GetField(_memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				return field;
			}
			throw new SerializationException($"Could not find field '{_memberName}' in type '{_reflectedType}'");
		}
		case MemberTypes.Property:
		{
			PropertyInfo property = _reflectedType.GetProperty(_memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null)
			{
				return property;
			}
			throw new SerializationException($"Could not find property '{_memberName}' in type '{_reflectedType}'");
		}
		case MemberTypes.Event:
		{
			EventInfo @event = _reflectedType.GetEvent(_memberName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (@event != null)
			{
				return @event;
			}
			throw new SerializationException($"Could not find event '{_memberName}' in type '{_reflectedType}'");
		}
		default:
			throw new SerializationException($"Unhandled MemberType {_memberType}");
		}
	}
}
