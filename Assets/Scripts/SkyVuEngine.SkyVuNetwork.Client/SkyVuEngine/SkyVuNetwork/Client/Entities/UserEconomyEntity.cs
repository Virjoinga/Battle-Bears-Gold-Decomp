using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class UserEconomyEntity : BaseEntity
	{
		public int UserEconomyId { get; set; }

		public int GameId { get; set; }

		public int Joules { get; set; }

		public int Gas { get; set; }

		public UserEconomyEntity()
		{
			_isSerialRequired = true;
			base.EntityName = "usereconomy";
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
			jsonWriter.Write(UserEconomyId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("j");
			jsonWriter.Write(Joules);
			jsonWriter.WritePropertyName("gs");
			jsonWriter.Write(Gas);
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
							UserEconomyId = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'userEconomyId' value in 'UserEconomyEntity'");
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
						throw new JsonException("Invalid 'gameId' value in 'UserEconomyEntity'");
					}
					case "j":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							Joules = num.Value;
							break;
						}
						throw new JsonException("Invalid 'joules' value in 'UserEconomyEntity'");
					}
					case "gs":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							Gas = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gas' value in 'UserEconomyEntity'");
					}
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
