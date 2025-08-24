using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class RankingSystemEntity : BaseEntity
	{
		public int RankingId { get; set; }

		public int Value { get; set; }

		public RankingSystemEntity()
		{
			base.EntityName = "rankingsystem";
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
			jsonWriter.WritePropertyName("r");
			jsonWriter.Write(RankingId);
			jsonWriter.WritePropertyName("v");
			jsonWriter.Write(Value);
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
				Value = -1;
				while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
				{
					if (reader.Value == null)
					{
						continue;
					}
					switch (reader.Value.ToString())
					{
					case "r":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							RankingId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'rankingId' value in 'RankingSystemEntity'");
					}
					case "v":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							Value = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'value' value in 'RankingSystemEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && base.UserId > 0 && Value > -1;
			}
			catch
			{
				return false;
			}
		}
	}
}
