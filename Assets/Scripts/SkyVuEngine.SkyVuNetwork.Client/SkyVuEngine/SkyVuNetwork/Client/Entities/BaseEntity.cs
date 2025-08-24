using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public abstract class BaseEntity
	{
		protected bool _isSerialRequired = false;

		public int MethodType { get; set; }

		public string Catalog { get; set; }

		public int UserId { get; set; }

		public string DeviceSerial { get; set; }

		public int Version { get; set; }

		protected bool IsPopulated
		{
			get
			{
				if (Version <= 0)
				{
					return false;
				}
				return true;
			}
		}

		public string EntityName { get; set; }

		public BaseEntity()
		{
			MethodType = 0;
			Catalog = string.Empty;
			UserId = -1;
			Version = 1;
			EntityName = string.Empty;
		}

		protected void Serialize<T>(List<T> list, JsonWriter writer, string name) where T : BaseEntity
		{
			if (list != null && list.Count > 0)
			{
				writer.WritePropertyName(name);
				writer.WriteArrayStart();
				list.ForEach(delegate(T e)
				{
					writer.Write(e.Serialize(false));
				});
				writer.WriteArrayEnd();
			}
		}

		protected void Serialize(List<string> list, JsonWriter writer, string name)
		{
			if (list != null && list.Count > 0)
			{
				writer.WritePropertyName(name);
				writer.WriteArrayStart();
				list.ForEach(delegate(string d)
				{
					writer.Write(d);
				});
				writer.WriteArrayEnd();
			}
		}

		public virtual bool Populate(string json)
		{
			return Populate(json, true);
		}

		public virtual bool Populate(string json, bool BaseData)
		{
			if (string.IsNullOrEmpty(json) || json == "null")
			{
				return false;
			}
			json = StripUnusedCharacters(json);
			JsonReader jsonReader = new JsonReader(json);
			jsonReader.Read();
			if (jsonReader.Token != JsonToken.ObjectStart)
			{
				return false;
			}
			return Populate(jsonReader, BaseData);
		}

		public virtual bool Populate(JsonReader reader)
		{
			return Populate(reader, true);
		}

		public virtual bool Populate(JsonReader reader, bool BaseData)
		{
			return false;
		}

		public virtual string Serialize()
		{
			return string.Empty;
		}

		public virtual string Serialize(bool BaseData)
		{
			return string.Empty;
		}

		protected string StripUnusedCharacters(string json)
		{
			json = json.Replace("\\r", "\r");
			json = json.Replace("\\n", "\n");
			json = json.Replace("\\", "");
			json = json.Replace("\"{", "{");
			json = json.Replace("}\"", "}");
			return json;
		}

		protected void SerializeBase(JsonWriter writer)
		{
			writer.WritePropertyName("mt");
			writer.Write(MethodType);
			writer.WritePropertyName("cat");
			writer.Write(Catalog);
			writer.WritePropertyName("u");
			writer.Write(UserId);
			if (_isSerialRequired)
			{
				writer.WritePropertyName("ser");
				writer.Write(DeviceSerial);
			}
			writer.WritePropertyName("ver");
			writer.Write(Version);
			writer.WritePropertyName("entityname");
			writer.Write(EntityName);
		}

		protected void PopulateBase(JsonReader reader)
		{
			if (reader.Value == null)
			{
				return;
			}
			if ((string)reader.Value == "mt")
			{
				reader.Read();
				int? num = Parsers.ParseInt(reader.Value);
				if (!num.HasValue)
				{
					throw new JsonException("Invalid 'methodType' value in 'BaseEntity'");
				}
				MethodType = num.Value;
			}
			else if ((string)reader.Value == "cat")
			{
				reader.Read();
				Catalog = (string)reader.Value;
			}
			else if ((string)reader.Value == "u")
			{
				reader.Read();
				int? num2 = Parsers.ParseInt(reader.Value);
				if (!num2.HasValue)
				{
					throw new JsonException("Invalid 'userId' value in 'BaseEntity'");
				}
				UserId = num2.Value;
			}
			else if ((string)reader.Value == "ser" && _isSerialRequired)
			{
				reader.Read();
				DeviceSerial = (string)reader.Value;
			}
			else if ((string)reader.Value == "ver")
			{
				reader.Read();
				int? num3 = Parsers.ParseInt(reader.Value);
				if (!num3.HasValue)
				{
					throw new JsonException("Invalid 'version' value in 'BaseEntity'");
				}
				Version = num3.Value;
			}
			else
			{
				if (!((string)reader.Value == "entityname"))
				{
					throw new JsonException("Unrecognized property name: " + reader.Value.ToString());
				}
				reader.Read();
				EntityName = (string)reader.Value;
			}
		}
	}
}
