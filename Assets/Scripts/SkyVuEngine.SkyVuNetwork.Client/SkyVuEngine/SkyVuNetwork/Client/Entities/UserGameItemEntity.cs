using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class UserGameItemEntity : BaseEntity
	{
		public int UserGameItemId { get; set; }

		public int GameItemId { get; set; }

		public int GameId { get; set; }

		public int Count { get; set; }

		public UserGameItemEntity()
		{
			base.EntityName = "usergameitem";
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
			jsonWriter.Write(UserGameItemId);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameItemId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("c");
			jsonWriter.Write(Count);
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
				GameItemId = -1;
				UserGameItemId = -1;
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
							UserGameItemId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'userGameItemId' value in 'UserGameItemEntity'");
					}
					case "gi":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							GameItemId = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemId' value in 'UserGameItemEntity'");
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
						throw new JsonException("Invalid 'gameId' value in 'UserGameItemEntity'");
					}
					case "c":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							if (!num2.HasValue)
							{
								Count = 0;
							}
							else
							{
								Count = num2.Value;
							}
							break;
						}
						throw new JsonException("Invalid 'count' value in 'UserGameItemEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GameItemId > -1 && UserGameItemId > -1;
			}
			catch
			{
				return false;
			}
		}
	}
}
