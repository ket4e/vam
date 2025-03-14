using System.Collections;
using System.Drawing;
using System.IO;
using System.Xml;

namespace System.Windows.Forms;

internal class MWFConfig
{
	internal class MWFConfigInstance
	{
		internal class ClassEntry
		{
			private Hashtable classvalues_hashtable = new Hashtable();

			private string className;

			public string ClassName
			{
				get
				{
					return className;
				}
				set
				{
					className = value;
				}
			}

			public void SetValue(string value_name, object value)
			{
				ClassValue classValue = classvalues_hashtable[value_name] as ClassValue;
				if (classValue == null)
				{
					classValue = new ClassValue();
					classValue.Name = value_name;
					classvalues_hashtable[value_name] = classValue;
				}
				classValue.SetValue(value);
			}

			public object GetValue(string value_name)
			{
				if (!(classvalues_hashtable[value_name] is ClassValue classValue))
				{
					return null;
				}
				return classValue.GetValue();
			}

			public void RemoveAllClassValues()
			{
				classvalues_hashtable.Clear();
			}

			public void RemoveClassValue(string value_name)
			{
				if (classvalues_hashtable[value_name] is ClassValue)
				{
					classvalues_hashtable.Remove(value_name);
				}
			}

			public void ReadXml(XmlTextReader xtr)
			{
				while (xtr.Read())
				{
					switch (xtr.NodeType)
					{
					case XmlNodeType.Element:
					{
						string attribute = xtr.GetAttribute("name");
						ClassValue classValue = classvalues_hashtable[attribute] as ClassValue;
						if (classValue == null)
						{
							classValue = new ClassValue();
							classValue.Name = attribute;
							classvalues_hashtable[attribute] = classValue;
						}
						classValue.ReadXml(xtr);
						break;
					}
					case XmlNodeType.EndElement:
						return;
					}
				}
			}

			public void WriteXml(XmlTextWriter xtw)
			{
				if (classvalues_hashtable.Count == 0)
				{
					return;
				}
				xtw.WriteStartElement(className);
				foreach (DictionaryEntry item in classvalues_hashtable)
				{
					ClassValue classValue = item.Value as ClassValue;
					classValue.WriteXml(xtw);
				}
				xtw.WriteEndElement();
			}
		}

		internal class ClassValue
		{
			private object value;

			private string name;

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

			public void SetValue(object value)
			{
				this.value = value;
			}

			public object GetValue()
			{
				return value;
			}

			public void ReadXml(XmlTextReader xtr)
			{
				string attribute = xtr.GetAttribute("type");
				if (attribute == "byte_array" || attribute.IndexOf("-array") == -1)
				{
					string s = xtr.ReadString();
					switch (attribute)
					{
					case "string":
						value = s;
						break;
					case "int":
						value = int.Parse(s);
						break;
					case "byte":
						value = byte.Parse(s);
						break;
					case "color":
					{
						int argb = int.Parse(s);
						value = Color.FromArgb(argb);
						break;
					}
					case "byte-array":
					{
						byte[] array = Convert.FromBase64String(s);
						value = array;
						break;
					}
					}
				}
				else
				{
					ReadXmlArrayValues(xtr, attribute);
				}
			}

			private void ReadXmlArrayValues(XmlTextReader xtr, string type)
			{
				ArrayList arrayList = new ArrayList();
				while (xtr.Read())
				{
					switch (xtr.NodeType)
					{
					case XmlNodeType.Element:
					{
						string text = xtr.ReadString();
						if (type == "int-array")
						{
							int num = int.Parse(text);
							arrayList.Add(num);
						}
						else if (type == "string-array")
						{
							string text2 = text;
							arrayList.Add(text2);
						}
						break;
					}
					case XmlNodeType.EndElement:
						if (xtr.Name == "value")
						{
							if (type == "int-array")
							{
								value = arrayList.ToArray(typeof(int));
							}
							else if (type == "string-array")
							{
								value = arrayList.ToArray(typeof(string));
							}
							return;
						}
						break;
					}
				}
			}

			public void WriteXml(XmlTextWriter xtw)
			{
				xtw.WriteStartElement("value");
				xtw.WriteAttributeString("name", name);
				if (value is Array)
				{
					WriteArrayContent(xtw);
				}
				else
				{
					WriteSingleContent(xtw);
				}
				xtw.WriteEndElement();
			}

			private void WriteSingleContent(XmlTextWriter xtw)
			{
				string text = string.Empty;
				if (value is string)
				{
					text = "string";
				}
				else if (value is int)
				{
					text = "int";
				}
				else if (value is byte)
				{
					text = "byte";
				}
				else if (value is Color)
				{
					text = "color";
				}
				xtw.WriteAttributeString("type", text);
				if (value is Color)
				{
					xtw.WriteString(((Color)value).ToArgb().ToString());
				}
				else
				{
					xtw.WriteString(value.ToString());
				}
			}

			private void WriteArrayContent(XmlTextWriter xtw)
			{
				string text = string.Empty;
				string localName = string.Empty;
				if (value is string[])
				{
					text = "string-array";
					localName = "string";
				}
				else if (value is int[])
				{
					text = "int-array";
					localName = "int";
				}
				else if (value is byte[])
				{
					text = "byte-array";
					localName = "byte";
				}
				xtw.WriteAttributeString("type", text);
				if (text != "byte-array")
				{
					Array array = value as Array;
					{
						foreach (object item in array)
						{
							xtw.WriteStartElement(localName);
							xtw.WriteString(item.ToString());
							xtw.WriteEndElement();
						}
						return;
					}
				}
				byte[] array2 = value as byte[];
				xtw.WriteString(Convert.ToBase64String(array2, 0, array2.Length));
			}
		}

		private Hashtable classes_hashtable = new Hashtable();

		private static string full_file_name;

		private static string default_file_name;

		private readonly string configName = "MWFConfig";

		public MWFConfigInstance()
		{
			Open(default_file_name);
		}

		public MWFConfigInstance(string filename)
		{
			string directoryName = Path.GetDirectoryName(filename);
			if (directoryName == null || directoryName == string.Empty)
			{
				directoryName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				full_file_name = Path.Combine(directoryName, filename);
			}
			else
			{
				full_file_name = filename;
			}
			Open(full_file_name);
		}

		static MWFConfigInstance()
		{
			string path = "mwf_config";
			string text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (XplatUI.RunningOnUnix)
			{
				text = Path.Combine(text, ".mono");
				try
				{
					Directory.CreateDirectory(text);
				}
				catch
				{
				}
			}
			default_file_name = Path.Combine(text, path);
			full_file_name = default_file_name;
		}

		~MWFConfigInstance()
		{
			Flush();
		}

		public object GetValue(string class_name, string value_name)
		{
			if (classes_hashtable[class_name] is ClassEntry classEntry)
			{
				return classEntry.GetValue(value_name);
			}
			return null;
		}

		public void SetValue(string class_name, string value_name, object value)
		{
			ClassEntry classEntry = classes_hashtable[class_name] as ClassEntry;
			if (classEntry == null)
			{
				classEntry = new ClassEntry();
				classEntry.ClassName = class_name;
				classes_hashtable[class_name] = classEntry;
			}
			classEntry.SetValue(value_name, value);
		}

		private void Open(string filename)
		{
			try
			{
				XmlTextReader xmlTextReader = new XmlTextReader(filename);
				ReadConfig(xmlTextReader);
				xmlTextReader.Close();
			}
			catch (Exception)
			{
			}
		}

		public void Flush()
		{
			try
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(full_file_name, null);
				xmlTextWriter.Formatting = Formatting.Indented;
				WriteConfig(xmlTextWriter);
				xmlTextWriter.Close();
				if (!XplatUI.RunningOnUnix)
				{
					File.SetAttributes(full_file_name, FileAttributes.Hidden);
				}
			}
			catch (Exception)
			{
			}
		}

		public void RemoveClass(string class_name)
		{
			if (classes_hashtable[class_name] is ClassEntry classEntry)
			{
				classEntry.RemoveAllClassValues();
				classes_hashtable.Remove(class_name);
			}
		}

		public void RemoveClassValue(string class_name, string value_name)
		{
			if (classes_hashtable[class_name] is ClassEntry classEntry)
			{
				classEntry.RemoveClassValue(value_name);
			}
		}

		public void RemoveAllClassValues(string class_name)
		{
			if (classes_hashtable[class_name] is ClassEntry classEntry)
			{
				classEntry.RemoveAllClassValues();
			}
		}

		private void ReadConfig(XmlTextReader xtr)
		{
			if (!CheckForMWFConfig(xtr))
			{
				return;
			}
			while (xtr.Read())
			{
				XmlNodeType nodeType = xtr.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					ClassEntry classEntry = classes_hashtable[xtr.Name] as ClassEntry;
					if (classEntry == null)
					{
						classEntry = new ClassEntry();
						classEntry.ClassName = xtr.Name;
						classes_hashtable[xtr.Name] = classEntry;
					}
					classEntry.ReadXml(xtr);
				}
			}
		}

		private bool CheckForMWFConfig(XmlTextReader xtr)
		{
			if (xtr.Read() && xtr.NodeType == XmlNodeType.Element && xtr.Name == configName)
			{
				return true;
			}
			return false;
		}

		private void WriteConfig(XmlTextWriter xtw)
		{
			if (classes_hashtable.Count == 0)
			{
				return;
			}
			xtw.WriteStartElement(configName);
			foreach (DictionaryEntry item in classes_hashtable)
			{
				ClassEntry classEntry = item.Value as ClassEntry;
				classEntry.WriteXml(xtw);
			}
			xtw.WriteEndElement();
		}
	}

	private static MWFConfigInstance Instance = new MWFConfigInstance();

	private static object lock_object = new object();

	public static object GetValue(string class_name, string value_name)
	{
		lock (lock_object)
		{
			return Instance.GetValue(class_name, value_name);
		}
	}

	public static void SetValue(string class_name, string value_name, object value)
	{
		lock (lock_object)
		{
			Instance.SetValue(class_name, value_name, value);
		}
	}

	public static void Flush()
	{
		lock (lock_object)
		{
			Instance.Flush();
		}
	}

	public static void RemoveClass(string class_name)
	{
		lock (lock_object)
		{
			Instance.RemoveClass(class_name);
		}
	}

	public static void RemoveClassValue(string class_name, string value_name)
	{
		lock (lock_object)
		{
			Instance.RemoveClassValue(class_name, value_name);
		}
	}

	public static void RemoveAllClassValues(string class_name)
	{
		lock (lock_object)
		{
			Instance.RemoveAllClassValues(class_name);
		}
	}
}
