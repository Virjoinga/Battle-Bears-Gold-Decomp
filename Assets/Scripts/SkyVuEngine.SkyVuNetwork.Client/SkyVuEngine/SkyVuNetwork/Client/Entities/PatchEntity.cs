using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class PatchEntity : BaseEntity
	{
		public int GameId { get; set; }

		public int GameVersion { get; set; }

		public string PatchLocation { get; set; }

		public int PatchSize { get; set; }

		public string Hash { get; set; }

		public int GamePlatformType { get; set; }

		public PatchEntity()
		{
			base.EntityName = "patch";
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
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("gv");
			jsonWriter.Write(GameVersion);
			jsonWriter.WritePropertyName("pl");
			jsonWriter.Write(PatchLocation);
			jsonWriter.WritePropertyName("ps");
			jsonWriter.Write(PatchSize);
			jsonWriter.WritePropertyName("h");
			jsonWriter.Write(Hash);
			jsonWriter.WritePropertyName("gpt");
			jsonWriter.Write(GamePlatformType);
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
					case "gi":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							GameId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'PatchEntity'");
					}
					case "gv":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GameVersion = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameVersion' value in 'PatchEntity'");
					}
					case "pl":
						reader.Read();
						PatchLocation = (string)reader.Value;
						break;
					case "ps":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							PatchSize = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'patchSize' value in 'PatchEntity'");
					}
					case "h":
						reader.Read();
						Hash = (string)reader.Value;
						break;
					case "gpt":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GamePlatformType = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'platform' value in 'PatchEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GameVersion >= 0 && GameId > 0 && GamePlatformType > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
