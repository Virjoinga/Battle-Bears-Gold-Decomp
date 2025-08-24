using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class AchievementEntity : BaseEntity
	{
		public int AchievementId { get; set; }

		public int GameId { get; set; }

		public string Title { get; set; }

		public int Value { get; set; }

		public string ValueString { get; set; }

		public string LabelName { get; set; }

		public bool IsHidden { get; set; }

		public AchievementEntity()
		{
			base.EntityName = "achievement";
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
			jsonWriter.Write(AchievementId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(Title);
			jsonWriter.WritePropertyName("v");
			jsonWriter.Write(Value);
			jsonWriter.WritePropertyName("vs");
			jsonWriter.Write(ValueString);
			jsonWriter.WritePropertyName("ln");
			jsonWriter.Write(LabelName);
			jsonWriter.WritePropertyName("h");
			jsonWriter.Write(IsHidden);
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
				AchievementId = -1;
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
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							AchievementId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'achievementId' value in 'AchievementEntity'");
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
						throw new JsonException("Invalid 'gameId' value in 'AchievementEntity'");
					}
					case "t":
						reader.Read();
						Title = (string)reader.Value;
						break;
					case "v":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							Value = num.Value;
							break;
						}
						throw new JsonException("Invalid 'value' value in 'AchievementEntity'");
					}
					case "vs":
						reader.Read();
						ValueString = (string)reader.Value;
						break;
					case "ln":
						reader.Read();
						LabelName = (string)reader.Value;
						break;
					case "h":
						reader.Read();
						IsHidden = (bool)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GameId > 0 && AchievementId > -1;
			}
			catch
			{
				return false;
			}
		}
	}
}
