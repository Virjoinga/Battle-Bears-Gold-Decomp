using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class GameSettingEntity : BaseEntity
	{
		public int GameSettingId { get; set; }

		public int GameSettingCategoryId { get; set; }

		public string Description { get; set; }

		public int GameId { get; set; }

		public string LabelName { get; set; }

		public string Value { get; set; }

		public GameSettingEntity()
		{
			base.EntityName = "settings";
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
			jsonWriter.Write(GameSettingCategoryId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Description);
			jsonWriter.WritePropertyName("l");
			jsonWriter.Write(LabelName);
			jsonWriter.WritePropertyName("v");
			jsonWriter.Write(Value);
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
		}

		public override bool Populate(JsonReader json)
		{
			return base.Populate(json, true);
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
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GameSettingCategoryId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameSettingCategoryId' value in 'GameSettingEntity'");
					}
					case "g":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GameId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'GameSettingEntity'");
					}
					case "d":
						reader.Read();
						Description = (string)reader.Value;
						break;
					case "l":
						reader.Read();
						LabelName = (string)reader.Value;
						break;
					case "v":
						reader.Read();
						Value = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated;
			}
			catch
			{
				return false;
			}
		}
	}
}
