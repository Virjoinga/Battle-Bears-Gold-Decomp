using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class GameItemEntity : BaseEntity
	{
		public int GameItemId { get; set; }

		public int GameItemTypeId { get; set; }

		public int GameId { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public int GasCost { get; set; }

		public int JouleCost { get; set; }

		public string LabelName { get; set; }

		public decimal? SalesPercent { get; set; }

		public GameItemEntity()
		{
			base.EntityName = "gameitem";
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
			jsonWriter.Write(GameItemId);
			jsonWriter.WritePropertyName("git");
			jsonWriter.Write(GameItemTypeId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(Title);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Description);
			jsonWriter.WritePropertyName("gc");
			jsonWriter.Write(GasCost);
			jsonWriter.WritePropertyName("jc");
			jsonWriter.Write(JouleCost);
			jsonWriter.WritePropertyName("ln");
			jsonWriter.Write(LabelName);
			jsonWriter.WritePropertyName("s");
			jsonWriter.Write(SalesPercent.HasValue ? SalesPercent.Value : 0m);
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
							GameItemId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemId' value in 'GameItemEntity'");
					}
					case "git":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							GameItemTypeId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameItemType' value in 'GameItemEntity'");
					}
					case "g":
					{
						reader.Read();
						int? num5 = Parsers.ParseInt(reader.Value);
						if (num5.HasValue)
						{
							GameId = num5.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'GameItemEntity'");
					}
					case "t":
						reader.Read();
						Title = (string)reader.Value;
						break;
					case "d":
						reader.Read();
						Description = (string)reader.Value;
						break;
					case "gc":
					{
						reader.Read();
						int? num6 = Parsers.ParseInt(reader.Value);
						if (num6.HasValue)
						{
							GasCost = num6.Value;
							break;
						}
						throw new JsonException("Invalid 'gasCost' value in 'GameItemEntity'");
					}
					case "jc":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							JouleCost = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'jouleCost' value in 'GameItemEntity'");
					}
					case "ln":
						reader.Read();
						LabelName = (string)reader.Value;
						break;
					case "s":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							SalesPercent = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'SalesPercent' value in 'GameItemEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && Title.Length > 0 && Description.Length > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
