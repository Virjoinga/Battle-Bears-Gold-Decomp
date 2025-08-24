using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class CategoryEntity : BaseEntity
	{
		public string GamerTag { get; set; }

		public string AccountToken { get; set; }

		public int AccountType { get; set; }

		public int GameId { get; set; }

		public string Email { get; set; }

		public string Aux1 { get; set; }

		public string Aux2 { get; set; }

		public int ErrorCode { get; set; }

		public CategoryEntity()
		{
			ErrorCode = 0;
			_isSerialRequired = true;
			base.EntityName = "category";
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
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GamerTag);
			jsonWriter.WritePropertyName("a");
			jsonWriter.Write(AccountToken);
			jsonWriter.WritePropertyName("at");
			jsonWriter.Write(AccountType);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("e");
			jsonWriter.Write(Email);
			jsonWriter.WritePropertyName("a1");
			jsonWriter.Write(Aux1);
			jsonWriter.WritePropertyName("a2");
			jsonWriter.Write(Aux2);
			jsonWriter.WritePropertyName("err");
			jsonWriter.Write(ErrorCode);
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
					case "g":
						reader.Read();
						GamerTag = (string)reader.Value;
						break;
					case "a":
						reader.Read();
						AccountToken = (string)reader.Value;
						break;
					case "at":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							AccountType = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'accountType' value in 'CategoryEntity'");
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
						throw new JsonException("Invalid 'gameId' value in 'CategoryEntity'");
					}
					case "err":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							ErrorCode = num.Value;
							break;
						}
						throw new JsonException("Invalid 'errorCode' value in 'CategoryEntity'");
					}
					case "e":
						reader.Read();
						Email = (string)reader.Value;
						break;
					case "a1":
						reader.Read();
						Aux1 = (string)reader.Value;
						break;
					case "a2":
						reader.Read();
						Aux2 = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && AccountType > 0 && GameId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
