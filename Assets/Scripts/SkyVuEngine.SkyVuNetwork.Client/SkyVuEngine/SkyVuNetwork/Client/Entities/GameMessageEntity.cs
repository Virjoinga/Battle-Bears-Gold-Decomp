using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class GameMessageEntity : BaseEntity
	{
		public string Subject { get; set; }

		public string Body { get; set; }

		public string To { get; set; }

		public string From { get; set; }

		public int GameId { get; set; }

		public GameMessageEntity()
		{
			base.EntityName = "gamemessage";
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
			jsonWriter.Write(Subject);
			jsonWriter.WritePropertyName("b");
			jsonWriter.Write(Body);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(To);
			jsonWriter.WritePropertyName("f");
			jsonWriter.Write(From);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
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
					case "s":
						reader.Read();
						Subject = (string)reader.Value;
						break;
					case "b":
						reader.Read();
						Body = (string)reader.Value;
						break;
					case "t":
						reader.Read();
						To = (string)reader.Value;
						break;
					case "f":
						reader.Read();
						From = (string)reader.Value;
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
						throw new JsonException("Invalid 'GameId' value in 'GameMessageEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && Subject.Length > 0 && Body.Length > 0 && To.Length > 0 && From.Length > 0 && GameId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
