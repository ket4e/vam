using System.Collections;
using System.ComponentModel;

namespace System.Xml.Serialization;

[XmlType("serializer")]
internal class SerializerInfo
{
	[XmlAttribute("class")]
	public string ClassName;

	[XmlAttribute("assembly")]
	public string Assembly;

	[XmlElement("reader")]
	public string ReaderClassName;

	[XmlElement("writer")]
	public string WriterClassName;

	[XmlElement("baseSerializer")]
	public string BaseSerializerClassName;

	[XmlElement("implementation")]
	public string ImplementationClassName;

	[XmlElement("noreader")]
	public bool NoReader;

	[XmlElement("nowriter")]
	public bool NoWriter;

	[XmlElement("generateAsInternal")]
	public bool GenerateAsInternal;

	[XmlElement("namespace")]
	public string Namespace;

	[XmlArrayItem("namespaceImport")]
	[XmlArray("namespaceImports")]
	public string[] NamespaceImports;

	[DefaultValue(SerializationFormat.Literal)]
	public SerializationFormat SerializationFormat = SerializationFormat.Literal;

	[XmlElement("outFileName")]
	public string OutFileName;

	[XmlArray("readerHooks")]
	public Hook[] ReaderHooks;

	[XmlArray("writerHooks")]
	public Hook[] WriterHooks;

	public ArrayList GetHooks(HookType hookType, XmlMappingAccess dir, HookAction action, Type type, string member)
	{
		if ((dir & XmlMappingAccess.Read) != 0)
		{
			return FindHook(ReaderHooks, hookType, action, type, member);
		}
		if ((dir & XmlMappingAccess.Write) != 0)
		{
			return FindHook(WriterHooks, hookType, action, type, member);
		}
		throw new Exception("INTERNAL ERROR");
	}

	private ArrayList FindHook(Hook[] hooks, HookType hookType, HookAction action, Type type, string member)
	{
		ArrayList arrayList = new ArrayList();
		if (hooks == null)
		{
			return arrayList;
		}
		foreach (Hook hook in hooks)
		{
			if ((action == HookAction.InsertBefore && (hook.InsertBefore == null || hook.InsertBefore == string.Empty)) || (action == HookAction.InsertAfter && (hook.InsertAfter == null || hook.InsertAfter == string.Empty)) || (action == HookAction.Replace && (hook.Replace == null || hook.Replace == string.Empty)) || hook.HookType != hookType)
			{
				continue;
			}
			if (hook.Select != null)
			{
				if ((hook.Select.TypeName != null && hook.Select.TypeName != string.Empty && hook.Select.TypeName != type.FullName) || (hook.Select.TypeMember != null && hook.Select.TypeMember != string.Empty && hook.Select.TypeMember != member))
				{
					continue;
				}
				if (hook.Select.TypeAttributes != null && hook.Select.TypeAttributes.Length > 0)
				{
					object[] customAttributes = type.GetCustomAttributes(inherit: true);
					bool flag = false;
					object[] array = customAttributes;
					foreach (object obj in array)
					{
						if (Array.IndexOf(hook.Select.TypeAttributes, obj.GetType().FullName) != -1)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
			}
			arrayList.Add(hook);
		}
		return arrayList;
	}
}
