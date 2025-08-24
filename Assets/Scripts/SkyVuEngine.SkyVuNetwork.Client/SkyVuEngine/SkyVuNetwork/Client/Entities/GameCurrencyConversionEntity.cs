using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class GameCurrencyConversionEntity : BaseEntity
	{
		public int GameCurrencyConversionId { get; set; }

		public int GameId { get; set; }

		public bool IsEnabled { get; set; }

		public int GasConsumed { get; set; }

		public int JoulesRewarded { get; set; }

		public string Name { get; set; }

		public GameCurrencyConversionEntity()
		{
			base.EntityName = "currencyconversion";
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
			jsonWriter.Write(GameCurrencyConversionId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("e");
			jsonWriter.Write(IsEnabled);
			jsonWriter.WritePropertyName("gc");
			jsonWriter.Write(GasConsumed);
			jsonWriter.WritePropertyName("jr");
			jsonWriter.Write(JoulesRewarded);
			jsonWriter.WritePropertyName("n");
			jsonWriter.Write(Name);
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
				GasConsumed = -1;
				JoulesRewarded = -1;
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
							GameCurrencyConversionId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameCurrencyConversionId' value in 'GameCurrencyConversionEntity'");
					}
					case "g":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GameId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'GameCurrencyConversionEntity'");
					}
					case "e":
						reader.Read();
						IsEnabled = (bool)reader.Value;
						break;
					case "gc":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							GasConsumed = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'gasConsumed' value in 'GameCurrencyConversionEntity'");
					}
					case "jr":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							JoulesRewarded = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'joulesRewarded' value in 'GameCurrencyConversionEntity'");
					}
					case "n":
						reader.Read();
						Name = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GasConsumed > -1 && JoulesRewarded > -1;
			}
			catch
			{
				return false;
			}
		}
	}
}
