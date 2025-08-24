using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class SaleOfferEntity : BaseEntity
	{
		public int SaleOfferId { get; set; }

		public int Offer { get; set; }

		public int OfferType { get; set; }

		public int GameId { get; set; }

		public int GameItemId { get; set; }

		public string Expire { get; set; }

		public SaleOfferEntity()
		{
			base.EntityName = "saleoffer";
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
			jsonWriter.WritePropertyName("soi");
			jsonWriter.Write(SaleOfferId);
			jsonWriter.WritePropertyName("o");
			jsonWriter.Write(Offer);
			jsonWriter.WritePropertyName("ot");
			jsonWriter.Write(OfferType);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("gid");
			jsonWriter.Write(GameItemId);
			jsonWriter.WritePropertyName("e");
			jsonWriter.Write(Expire);
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
					case "soi":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							SaleOfferId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'saleOfferId' value in 'SaleOfferEntity'");
					}
					case "o":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							Offer = num.Value;
							break;
						}
						throw new JsonException("Invalid 'offer' value in 'SaleOfferEntity'");
					}
					case "ot":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							OfferType = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'offerType' value in 'SaleOfferEntity'");
					}
					case "gi":
					{
						reader.Read();
						int? num5 = Parsers.ParseInt(reader.Value);
						if (num5.HasValue)
						{
							GameId = num5.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'SaleOfferEntity'");
					}
					case "gid":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GameItemId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemId' value in 'SaleOfferEntity'");
					}
					case "e":
						reader.Read();
						Expire = (string)reader.Value;
						break;
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
