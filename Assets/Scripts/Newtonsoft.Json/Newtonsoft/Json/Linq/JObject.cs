using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json.Utilities.LinqBridge;

namespace Newtonsoft.Json.Linq
{
	public class JObject : JContainer, IDictionary<string, JToken>, ICollection<KeyValuePair<string, JToken>>, IEnumerable<KeyValuePair<string, JToken>>, IEnumerable, INotifyPropertyChanged, ICustomTypeDescriptor
	{
		private readonly JPropertyKeyedCollection _properties = new JPropertyKeyedCollection();

		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return _properties;
			}
		}

		public override JTokenType Type
		{
			get
			{
				return JTokenType.Object;
			}
		}

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Accessed JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this[text];
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new ArgumentException("Set JObject values with invalid key value: {0}. Object property name expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this[text] = value;
			}
		}

		public JToken this[string propertyName]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
				JProperty jProperty = Property(propertyName);
				if (jProperty == null)
				{
					return null;
				}
				return jProperty.Value;
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

		ICollection<string> IDictionary<string, JToken>.Keys
		{
			get
			{
				return _properties.Keys;
			}
		}

		ICollection<JToken> IDictionary<string, JToken>.Values
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool ICollection<KeyValuePair<string, JToken>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

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
			JObject jObject = node as JObject;
			if (jObject != null)
			{
				return ContentsEqual(jObject);
			}
			return false;
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

		internal void InternalPropertyChanged(JProperty childProperty)
		{
			OnPropertyChanged(childProperty.Name);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, IndexOfItem(childProperty)));
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
			return ChildrenTokens.Cast<JProperty>();
		}

		public JProperty Property(string name)
		{
			if (name == null)
			{
				return null;
			}
			JToken value;
			_properties.TryGetValue(name, out value);
			return (JProperty)value;
		}

		public JEnumerable<JToken> PropertyValues()
		{
			return new JEnumerable<JToken>(from p in Properties()
				select p.Value);
		}

		public new static JObject Load(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw new Exception("Error reading JObject from JsonReader.");
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JObject jObject = new JObject();
			jObject.SetLineInfo(reader as IJsonLineInfo);
			jObject.ReadTokenFrom(reader);
			return jObject;
		}

		public new static JObject Parse(string json)
		{
			JsonReader jsonReader = new JsonTextReader(new StringReader(json));
			JObject result = Load(jsonReader);
			if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
			{
				throw new Exception("Additional text found in JSON string after parsing content.");
			}
			return result;
		}

		public new static JObject FromObject(object o)
		{
			return FromObject(o, new JsonSerializer());
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
			foreach (JProperty childrenToken in ChildrenTokens)
			{
				childrenToken.WriteTo(writer, converters);
			}
			writer.WriteEndObject();
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
			if (arrayIndex >= array.Length)
			{
				throw new ArgumentException("arrayIndex is equal to or greater than the length of array.");
			}
			if (base.Count > array.Length - arrayIndex)
			{
				throw new ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			foreach (JProperty childrenToken in ChildrenTokens)
			{
				array[arrayIndex + num] = new KeyValuePair<string, JToken>(childrenToken.Name, childrenToken.Value);
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
			foreach (JProperty property in ChildrenTokens)
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

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties((Attribute[])null);
		}

		private static Type GetTokenPropertyType(JToken token)
		{
			if (token is JValue)
			{
				JValue jValue = (JValue)token;
				if (jValue.Value == null)
				{
					return typeof(object);
				}
				return jValue.Value.GetType();
			}
			return token.GetType();
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			using (IEnumerator<KeyValuePair<string, JToken>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, JToken> current = enumerator.Current;
					propertyDescriptorCollection.Add(new JPropertyDescriptor(current.Key, GetTokenPropertyType(current.Value)));
				}
				return propertyDescriptorCollection;
			}
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return new TypeConverter();
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return null;
		}
	}
}
