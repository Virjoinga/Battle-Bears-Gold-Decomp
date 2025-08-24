using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class GameSaleEntity : BaseEntity
	{
		public int GameSaleId { get; set; }

		public int GameId { get; set; }

		public bool IsEnabled { get; set; }

		public decimal SalePercent { get; set; }

		public GameSaleEntity()
		{
			base.EntityName = "gamesales";
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
			jsonWriter.Write(GameSaleId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("e");
			jsonWriter.Write(IsEnabled);
			jsonWriter.WritePropertyName("s");
			jsonWriter.Write(SalePercent);
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
				SalePercent = -999m;
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
							GameSaleId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameSaleId' value in 'GameSaleEntity'");
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
						throw new JsonException("Invalid 'gameId' value in 'GameSaleEntity'");
					}
					case "e":
						reader.Read();
						IsEnabled = (bool)reader.Value;
						break;
					case "s":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							SalePercent = num.Value;
							break;
						}
						throw new JsonException("Invalid 'salePercent' value in 'GameSaleEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && SalePercent > -999m;
			}
			catch
			{
				return false;
			}
		}
	}
}
