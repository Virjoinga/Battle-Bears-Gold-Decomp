using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class GameItemAttributeEntity : BaseEntity
	{
		public int GameItemAttributeId { get; set; }

		public int GameItemAttributeType { get; set; }

		public int GameItemId { get; set; }

		public string StringValue { get; set; }

		public double? Value { get; set; }

		public GameItemAttributeEntity()
		{
			base.EntityName = "itemattribute";
		}

		public override string Serialize()
		{
			return Serialize(true);
		}

		public override string Serialize(bool BaseData)
		{
			JsonWriter jsonWriter = new JsonWriter();
			jsonWriter.WriteObjectStart();
			if (BaseData)
			{
				SerializeBase(jsonWriter);
			}
			jsonWriter.WritePropertyName("i");
			jsonWriter.Write(GameItemAttributeId);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(GameItemAttributeType);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameItemId);
			jsonWriter.WritePropertyName("s");
			jsonWriter.Write(StringValue);
			jsonWriter.WritePropertyName("v");
			jsonWriter.Write(Value.HasValue ? Value.Value : 0.0);
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
		}

		public override bool Populate(JsonReader json)
		{
			return Populate(json, true);
		}

		public override bool Populate(JsonReader reader, bool BaseData)
		{
			try
			{
				while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
				{
					if (reader.Value == null)
					{
						continue;
					}
					switch (reader.Value.ToString())
					{
					case "i":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							GameItemAttributeId = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemAttributeId' value in 'GameItemAttributeEntity'");
					}
					case "t":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							GameItemAttributeType = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemAttributeType' value in 'GameItemAttributeEntity'");
					}
					case "gi":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GameItemId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemId' value in 'GameItemAttributeEntity'");
					}
					case "s":
						reader.Read();
						StringValue = (string)reader.Value;
						break;
					case "v":
					{
						reader.Read();
						double? num2 = Parsers.ParseDouble(reader.Value);
						if (num2.HasValue)
						{
							Value = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'value' value in 'GameItemAttributeEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && Value.HasValue;
			}
			catch
			{
				return false;
			}
		}
	}
}
