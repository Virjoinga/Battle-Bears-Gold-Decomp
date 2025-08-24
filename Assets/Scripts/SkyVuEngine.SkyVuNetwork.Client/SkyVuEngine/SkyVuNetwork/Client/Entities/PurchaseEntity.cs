using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class PurchaseEntity : BaseEntity
	{
		public int GameId { get; set; }

		public int GameItemId { get; set; }

		public PurchaseEntity()
		{
			_isSerialRequired = true;
			base.EntityName = "purchase";
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
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameItemId);
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
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GameId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'PurchaseEntity'");
					}
					case "gi":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GameItemId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemId' value in 'PurchaseEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GameItemId > 0 && GameId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
