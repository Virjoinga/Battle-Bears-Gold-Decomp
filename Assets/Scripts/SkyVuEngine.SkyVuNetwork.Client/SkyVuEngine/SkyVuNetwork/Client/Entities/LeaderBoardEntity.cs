using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class LeaderBoardEntity : BaseEntity
	{
		public int LeaderBoardId { get; set; }

		public int GameId { get; set; }

		public int GameTypeId { get; set; }

		public string Title { get; set; }

		public int ScoreMin { get; set; }

		public int ScoreMax { get; set; }

		public int ScoreUnitTypeId { get; set; }

		public LeaderBoardEntity()
		{
			base.EntityName = "leadboard";
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
			jsonWriter.Write(LeaderBoardId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("gt");
			jsonWriter.Write(GameTypeId);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(Title);
			jsonWriter.WritePropertyName("min");
			jsonWriter.Write(ScoreMin);
			jsonWriter.WritePropertyName("max");
			jsonWriter.Write(ScoreMax);
			jsonWriter.WritePropertyName("sut");
			jsonWriter.Write(ScoreUnitTypeId);
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
					case "i":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							LeaderBoardId = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'leaderBoardId' value in 'LeaderBoardEntity'");
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
						throw new JsonException("Invalid 'gameId' value in 'LeaderBoardEntity'");
					}
					case "gt":
					{
						reader.Read();
						int? num6 = Parsers.ParseInt(reader.Value);
						if (num6.HasValue)
						{
							GameTypeId = num6.Value;
							break;
						}
						throw new JsonException("Invalid 'gameTypeId' value in 'LeaderBoardEntity'");
					}
					case "t":
						reader.Read();
						Title = (string)reader.Value;
						break;
					case "min":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							ScoreMin = num.Value;
							break;
						}
						throw new JsonException("Invalid 'scoreMin' value in 'LeaderBoardEntity'");
					}
					case "max":
					{
						reader.Read();
						int? num5 = Parsers.ParseInt(reader.Value);
						if (num5.HasValue)
						{
							ScoreMax = num5.Value;
							break;
						}
						throw new JsonException("Invalid 'scoreMax' value in 'LeaderBoardEntity'");
					}
					case "sut":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							ScoreUnitTypeId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'scoreUnitTypeId' value in 'LeaderBoardEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GameId > 0 && GameTypeId > 0 && ScoreUnitTypeId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
