using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class NewsFeedEntity : BaseEntity
	{
		public int NewsFeedId { get; set; }

		public int NewsFeedTypeId { get; set; }

		public int NewsFeedFormatTypeId { get; set; }

		public string Title { get; set; }

		public string CreateDate { get; set; }

		public string Body { get; set; }

		public int Priority { get; set; }

		public int GameId { get; set; }

		public string Data { get; set; }

		public bool IsActive { get; set; }

		public int Limit { get; set; }

		public NewsFeedEntity()
		{
			base.EntityName = "newsfeed";
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
			jsonWriter.Write(NewsFeedId);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(NewsFeedTypeId);
			jsonWriter.WritePropertyName("f");
			jsonWriter.Write(NewsFeedFormatTypeId);
			jsonWriter.WritePropertyName("ti");
			jsonWriter.Write(Title);
			jsonWriter.WritePropertyName("c");
			jsonWriter.Write((CreateDate != null) ? CreateDate.Replace("/", "-") : null);
			jsonWriter.WritePropertyName("b");
			jsonWriter.Write(Body);
			jsonWriter.WritePropertyName("p");
			jsonWriter.Write(Priority);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Data);
			jsonWriter.WritePropertyName("a");
			jsonWriter.Write(IsActive);
			jsonWriter.WritePropertyName("lim");
			jsonWriter.Write(Limit);
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
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							NewsFeedId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'newsFeedId' value in 'NewsFeedEntity'");
					}
					case "t":
					{
						reader.Read();
						int? num6 = Parsers.ParseInt(reader.Value);
						if (num6.HasValue)
						{
							NewsFeedTypeId = num6.Value;
							break;
						}
						throw new JsonException("Invalid 'NewsFeedTypeId' value in 'NewsFeedEntity'");
					}
					case "f":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							NewsFeedFormatTypeId = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'NewsFeedFormatTypeId' value in 'NewsFeedEntity'");
					}
					case "ti":
						reader.Read();
						Title = (string)reader.Value;
						break;
					case "c":
						reader.Read();
						CreateDate = (string)reader.Value;
						break;
					case "b":
						reader.Read();
						Body = (string)reader.Value;
						break;
					case "p":
					{
						reader.Read();
						int? num5 = Parsers.ParseInt(reader.Value);
						if (num5.HasValue)
						{
							Priority = num5.Value;
							break;
						}
						throw new JsonException("Invalid 'priority' value in 'NewsFeedEntity'");
					}
					case "gi":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							GameId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'NewsFeedEntity'");
					}
					case "d":
						reader.Read();
						Data = (string)reader.Value;
						break;
					case "a":
						reader.Read();
						IsActive = (bool)reader.Value;
						break;
					case "lim":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							Limit = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'limit' value in 'NewsFeedEntity'");
					}
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
