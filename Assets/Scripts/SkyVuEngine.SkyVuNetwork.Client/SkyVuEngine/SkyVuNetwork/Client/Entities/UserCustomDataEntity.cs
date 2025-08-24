using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class UserCustomDataEntity : BaseEntity
	{
		public int UserCustomDataId { get; set; }

		public int GameId { get; set; }

		public string Data { get; set; }

		public UserCustomDataEntity()
		{
			base.EntityName = "usercustomdata";
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
			jsonWriter.Write(UserCustomDataId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Data);
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
							UserCustomDataId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'userCustomDataId' value in 'UserCustomDataEntity'");
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
						throw new JsonException("Invalid 'gameId' value in 'UserCustomDataEntity'");
					}
					case "d":
						reader.Read();
						Data = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && base.UserId > 0 && GameId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
