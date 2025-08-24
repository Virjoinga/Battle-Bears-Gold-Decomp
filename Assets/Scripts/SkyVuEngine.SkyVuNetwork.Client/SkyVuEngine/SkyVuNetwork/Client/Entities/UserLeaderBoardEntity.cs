using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class UserLeaderBoardEntity : BaseEntity
	{
		public int Score { get; set; }

		public int GameId { get; set; }

		public int LeaderBoardId { get; set; }

		public string GamerTag { get; set; }

		public UserLeaderBoardEntity()
		{
			base.EntityName = "userleaderboard";
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
			jsonWriter.WritePropertyName("s");
			jsonWriter.Write(Score);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("l");
			jsonWriter.Write(LeaderBoardId);
			jsonWriter.WritePropertyName("gt");
			jsonWriter.Write(GamerTag);
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
				while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
				{
					if (reader.Value == null)
					{
						continue;
					}
					switch (reader.Value.ToString())
					{
					case "s":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							Score = num.Value;
							break;
						}
						throw new JsonException("Invalid 'score' value in 'UserLeaderBoardEntity'");
					}
					case "g":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							GameId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'UserLeaderBoardEntity'");
					}
					case "l":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							LeaderBoardId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'leaderBoardId' value in 'UserLeaderBoardEntity'");
					}
					case "gt":
						reader.Read();
						GamerTag = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && Score >= 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
