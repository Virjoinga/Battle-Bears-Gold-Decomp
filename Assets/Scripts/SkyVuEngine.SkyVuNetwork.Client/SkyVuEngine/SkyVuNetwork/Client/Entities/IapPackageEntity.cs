using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class IapPackageEntity : BaseEntity
	{
		public int IapPackageId { get; set; }

		public int GameId { get; set; }

		public string Description { get; set; }

		public string ProductLabel { get; set; }

		public int QtyGas { get; set; }

		public int GamePlatformTypeId { get; set; }

		public IapPackageEntity()
		{
			base.EntityName = "iappackage";
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
			jsonWriter.Write(IapPackageId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Description);
			jsonWriter.WritePropertyName("l");
			jsonWriter.Write(ProductLabel);
			jsonWriter.WritePropertyName("gq");
			jsonWriter.Write(QtyGas);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(GamePlatformTypeId);
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
		}

		public override bool Populate(JsonReader reader)
		{
			return base.Populate(reader, true);
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
							IapPackageId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'iapPackageId' value in 'IapPackageEntity'");
					}
					case "g":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							GameId = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'IapPackageEntity'");
					}
					case "d":
						reader.Read();
						Description = (string)reader.Value;
						break;
					case "l":
						reader.Read();
						ProductLabel = (string)reader.Value;
						break;
					case "gq":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							QtyGas = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'qtyGas' value in 'IapPackageEntity'");
					}
					case "t":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GamePlatformTypeId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gamePlatformTypeId' value in 'IapPackageEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && Description.Length > 0 && ProductLabel.Length > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
