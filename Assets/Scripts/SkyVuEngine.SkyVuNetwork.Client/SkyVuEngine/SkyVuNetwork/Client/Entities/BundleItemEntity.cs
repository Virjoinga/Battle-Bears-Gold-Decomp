using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class BundleItemEntity : BaseEntity
	{
		public int BundleId { get; set; }

		public int BundleItemId { get; set; }

		public int GameItemId { get; set; }

		public BundleItemEntity()
		{
			base.EntityName = "bundleitem";
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
			jsonWriter.WritePropertyName("b");
			jsonWriter.Write(BundleId);
			jsonWriter.WritePropertyName("bi");
			jsonWriter.Write(BundleItemId);
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
					case "b":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							BundleId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'bundleId' value in 'BundleItemEntity'");
					}
					case "bi":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							BundleItemId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'bundleItemId' value in 'BundleItemEntity'");
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
						throw new JsonException("Invalid 'gameItemId' value in 'BundleItemEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && BundleId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
