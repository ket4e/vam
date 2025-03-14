using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq;

public class JObject : JContainer, IDictionary<string, JToken>, ICollection<KeyValuePair<string, JToken>>, IEnumerable<KeyValuePair<string, JToken>>, IEnumerable, INotifyPropertyChanged
{
	private readonly JPropertyKeyedCollection _properties = new JPropertyKeyedCollection();

	protected override IList<JToken> ChildrenTokens => _properties;

	public override JTokenType Type => JTokenType.Object;

	public override JToken this[object key]
	{
		get
		{
			ValidationUtils.ArgumentNotNull(key, "key");
			if (!(key is string propertyName))
			{
				throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
			}
			return this[propertyName];
		}
		set
		{
			ValidationUtils.ArgumentNotNull(key, "key");
			if (!(key is string propertyName))
			{
				throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
			}
			this[propertyName] = value;
		}
	}

	public JToken this[string propertyName]
	{
		get
		{
			ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
			return Property(propertyName)?.Value;
		}
		set
		{
			JProperty jProperty = Property(propertyName);
			if (jProperty != null)
			{
				jProperty.Value = value;
				return;
			}
			Add(new JProperty(propertyName, value));
			OnPropertyChanged(propertyName);
		}
	}

	ICollection<string> IDictionary<string, JToken>.Keys => _properties.Keys;

	ICollection<JToken> IDictionary<string, JToken>.Values
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly => false;

	public event PropertyChangedEventHandler PropertyChanged;

	public JObject()
	{
	}

	public JObject(JObject other)
		: base(other)
	{
	}

	public JObject(params object[] content)
		: this((object)content)
	{
	}

	public JObject(object content)
	{
		Add(content);
	}

	internal override bool DeepEquals(JToken node)
	{
		if (!(node is JObject jObject))
		{
			return false;
		}
		return _properties.Compare(jObject._properties);
	}

	internal override int IndexOfItem(JToken item)
	{
		return _properties.IndexOfReference(item);
	}

	internal override void InsertItem(int index, JToken item, bool skipParentCheck)
	{
		if (item == null || item.Type != JTokenType.Comment)
		{
			base.InsertItem(index, item, skipParentCheck);
		}
	}

	internal override void ValidateToken(JToken o, JToken existing)
	{
		ValidationUtils.ArgumentNotNull(o, "o");
		if (o.Type != JTokenType.Property)
		{
			throw new ArgumentException("Can not add {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, o.GetType(), GetType()));
		}
		JProperty jProperty = (JProperty)o;
		if (existing != null)
		{
			JProperty jProperty2 = (JProperty)existing;
			if (jProperty.Name == jProperty2.Name)
			{
				return;
			}
		}
		if (_properties.TryGetValue(jProperty.Name, out existing))
		{
			throw new ArgumentException("Can not add property {0} to {1}. Property with the same name already exists on object.".FormatWith(CultureInfo.InvariantCulture, jProperty.Name, GetType()));
		}
	}

	internal override void MergeItem(object content, JsonMergeSettings settings)
	{
		if (!(content is JObject jObject))
		{
			return;
		}
		foreach (KeyValuePair<string, JToken> item in jObject)
		{
			JProperty jProperty = Property(item.Key);
			if (jProperty == null)
			{
				Add(item.Key, item.Value);
			}
			else
			{
				if (item.Value == null)
				{
					continue;
				}
				if (!(jProperty.Value is JContainer jContainer))
				{
					if (item.Value.Type != JTokenType.Null || (settings != null && settings.MergeNullValueHandling == MergeNullValueHandling.Merge))
					{
						jProperty.Value = item.Value;
					}
				}
				else if (jContainer.Type != item.Value.Type)
				{
					jProperty.Value = item.Value;
				}
				else
				{
					jContainer.Merge(item.Value, settings);
				}
			}
		}
	}

	internal void InternalPropertyChanged(JProperty childProperty)
	{
		OnPropertyChanged(childProperty.Name);
	}

	internal void InternalPropertyChanging(JProperty childProperty)
	{
	}

	internal override JToken CloneToken()
	{
		return new JObject(this);
	}

	public IEnumerable<JProperty> Properties()
	{
		return _properties.Cast<JProperty>();
	}

	public JProperty Property(string name)
	{
		if (name == null)
		{
			return null;
		}
		_properties.TryGetValue(name, out var value);
		return (JProperty)value;
	}

	public JEnumerable<JToken> PropertyValues()
	{
		return new JEnumerable<JToken>(from p in Properties()
			select p.Value);
	}

	public new static JObject Load(JsonReader reader)
	{
		return Load(reader, null);
	}

	public new static JObject Load(JsonReader reader, JsonLoadSettings settings)
	{
		ValidationUtils.ArgumentNotNull(reader, "reader");
		if (reader.TokenType == JsonToken.None && !reader.Read())
		{
			throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader.");
		}
		reader.MoveToContent();
		if (reader.TokenType != JsonToken.StartObject)
		{
			throw JsonReaderException.Create(reader, "Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}
		JObject jObject = new JObject();
		jObject.SetLineInfo(reader as IJsonLineInfo, settings);
		jObject.ReadTokenFrom(reader, settings);
		return jObject;
	}

	public new static JObject Parse(string json)
	{
		return Parse(json, null);
	}

	public new static JObject Parse(string json, JsonLoadSettings settings)
	{
		using JsonReader jsonReader = new JsonTextReader(new StringReader(json));
		JObject result = Load(jsonReader, settings);
		if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
		{
			throw JsonReaderException.Create(jsonReader, "Additional text found in JSON string after parsing content.");
		}
		return result;
	}

	public new static JObject FromObject(object o)
	{
		return FromObject(o, JsonSerializer.CreateDefault());
	}

	public new static JObject FromObject(object o, JsonSerializer jsonSerializer)
	{
		JToken jToken = JToken.FromObjectInternal(o, jsonSerializer);
		if (jToken != null && jToken.Type != JTokenType.Object)
		{
			throw new ArgumentException("Object serialized to {0}. JObject instance expected.".FormatWith(CultureInfo.InvariantCulture, jToken.Type));
		}
		return (JObject)jToken;
	}

	public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
	{
		writer.WriteStartObject();
		for (int i = 0; i < _properties.Count; i++)
		{
			_properties[i].WriteTo(writer, converters);
		}
		writer.WriteEndObject();
	}

	public JToken GetValue(string propertyName)
	{
		return GetValue(propertyName, StringComparison.Ordinal);
	}

	public JToken GetValue(string propertyName, StringComparison comparison)
	{
		if (propertyName == null)
		{
			return null;
		}
		JProperty jProperty = Property(propertyName);
		if (jProperty != null)
		{
			return jProperty.Value;
		}
		if (comparison != StringComparison.Ordinal)
		{
			foreach (JProperty property in _properties)
			{
				if (string.Equals(property.Name, propertyName, comparison))
				{
					return property.Value;
				}
			}
		}
		return null;
	}

	public bool TryGetValue(string propertyName, StringComparison comparison, out JToken value)
	{
		value = GetValue(propertyName, comparison);
		return value != null;
	}

	public void Add(string propertyName, JToken value)
	{
		Add(new JProperty(propertyName, value));
	}

	bool IDictionary<string, JToken>.ContainsKey(string key)
	{
		return _properties.Contains(key);
	}

	public bool Remove(string propertyName)
	{
		JProperty jProperty = Property(propertyName);
		if (jProperty == null)
		{
			return false;
		}
		jProperty.Remove();
		return true;
	}

	public bool TryGetValue(string propertyName, out JToken value)
	{
		JProperty jProperty = Property(propertyName);
		if (jProperty == null)
		{
			value = null;
			return false;
		}
		value = jProperty.Value;
		return true;
	}

	void ICollection<KeyValuePair<string, JToken>>.Add(KeyValuePair<string, JToken> item)
	{
		Add(new JProperty(item.Key, item.Value));
	}

	void ICollection<KeyValuePair<string, JToken>>.Clear()
	{
		RemoveAll();
	}

	bool ICollection<KeyValuePair<string, JToken>>.Contains(KeyValuePair<string, JToken> item)
	{
		JProperty jProperty = Property(item.Key);
		if (jProperty == null)
		{
			return false;
		}
		return jProperty.Value == item.Value;
	}

	void ICollection<KeyValuePair<string, JToken>>.CopyTo(KeyValuePair<string, JToken>[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
		}
		if (arrayIndex >= array.Length && arrayIndex != 0)
		{
			throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
		}
		if (base.Count > array.Length - arrayIndex)
		{
			throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
		}
		int num = 0;
		foreach (JProperty property in _properties)
		{
			array[arrayIndex + num] = new KeyValuePair<string, JToken>(property.Name, property.Value);
			num++;
		}
	}

	bool ICollection<KeyValuePair<string, JToken>>.Remove(KeyValuePair<string, JToken> item)
	{
		if (!((ICollection<KeyValuePair<string, JToken>>)this).Contains(item))
		{
			return false;
		}
		((IDictionary<string, JToken>)this).Remove(item.Key);
		return true;
	}

	internal override int GetDeepHashCode()
	{
		return ContentsHashCode();
	}

	public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
	{
		foreach (JProperty property in _properties)
		{
			yield return new KeyValuePair<string, JToken>(property.Name, property.Value);
		}
	}

	protected virtual void OnPropertyChanged(string propertyName)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
