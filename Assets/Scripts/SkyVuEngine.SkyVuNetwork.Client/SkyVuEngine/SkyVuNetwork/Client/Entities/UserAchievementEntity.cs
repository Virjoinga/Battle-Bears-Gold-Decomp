using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class UserAchievementEntity : BaseEntity
	{
		public int UserAchievementId { get; set; }

		public int AchievementId { get; set; }

		public int GameId { get; set; }

		public string CreateDate { get; set; }

		public UserAchievementEntity()
		{
			base.EntityName = "userachievement";
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
			jsonWriter.Write(UserAchievementId);
			jsonWriter.WritePropertyName("ac");
			jsonWriter.Write(AchievementId);
			jsonWriter.WritePropertyName("c");
			jsonWriter.Write(CreateDate);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
		}

		public override bool Populate(JsonReader reader)
		{
			return Populate(reader, true);
		}

		public override bool Populate(JsonReader reader, bool BaseData)
		{
			try
			{
				base.UserId = -1;
				UserAchievementId = -1;
				AchievementId = -1;
				GameId = -1;
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
							UserAchievementId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'userAchievementId' value in 'UserAchievementEntity'");
					}
					case "ac":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							AchievementId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'achievementId' value in 'UserAchievementEntity'");
					}
					case "c":
						reader.Read();
						CreateDate = (string)reader.Value;
						break;
					case "g":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GameId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'UserAchievementEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && base.UserId > -1 && UserAchievementId > -1 && AchievementId > -1 && GameId > -1;
			}
			catch
			{
				return false;
			}
		}
	}
}
