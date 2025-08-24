using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class BuddyListEntity : BaseEntity
	{
		public int BuddyListId { get; set; }

		public int SmallId { get; set; }

		public int BigId { get; set; }

		public string BuddyGamerTag { get; set; }

		public BuddyListEntity()
		{
			base.EntityName = "buddylist";
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
			jsonWriter.Write(BuddyListId);
			jsonWriter.WritePropertyName("s");
			jsonWriter.Write(SmallId);
			jsonWriter.WritePropertyName("b");
			jsonWriter.Write(BigId);
			jsonWriter.WritePropertyName("bgt");
			jsonWriter.Write(BuddyGamerTag);
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
							BuddyListId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'BuddyListId' value in 'BuddyListEntity'");
					}
					case "s":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							SmallId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'SmallId' value in 'BuddyListEntity'");
					}
					case "b":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							BigId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'BigId' value in 'BuddyListEntity'");
					}
					case "bgt":
						reader.Read();
						BuddyGamerTag = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && SmallId > 0 && BigId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
