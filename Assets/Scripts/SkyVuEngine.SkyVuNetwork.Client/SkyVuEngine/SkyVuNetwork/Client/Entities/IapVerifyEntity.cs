using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class IapVerifyEntity : BaseEntity
	{
		public int GamePlatformType { get; set; }

		public string Token1 { get; set; }

		public string Token2 { get; set; }

		public IapVerifyEntity()
		{
			_isSerialRequired = true;
			base.EntityName = "iapverify";
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
			jsonWriter.WritePropertyName("p");
			jsonWriter.Write(GamePlatformType);
			jsonWriter.WritePropertyName("t1");
			jsonWriter.Write(Token1);
			jsonWriter.WritePropertyName("t2");
			jsonWriter.Write(Token2);
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
				GamePlatformType = -1;
				while (reader.Read() && reader.Token != JsonToken.ObjectEnd)
				{
					if (reader.Value == null)
					{
						continue;
					}
					switch (reader.Value.ToString())
					{
					case "p":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GamePlatformType = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gamePlatformType' value in 'IapVerifyEntity'");
					}
					case "t1":
						reader.Read();
						Token1 = (string)reader.Value;
						break;
					case "t2":
						reader.Read();
						Token2 = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GamePlatformType > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
