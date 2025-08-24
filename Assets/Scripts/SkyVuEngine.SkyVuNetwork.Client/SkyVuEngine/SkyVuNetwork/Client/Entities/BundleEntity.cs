using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class BundleEntity : BaseEntity
	{
		public int BundleId { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string LabelName { get; set; }

		public int GameId { get; set; }

		public int Gas { get; set; }

		public int Joules { get; set; }

		public int? Order { get; set; }

		public string ImageUrl { get; set; }

		public string StartDate { get; set; }

		public string EndDate { get; set; }

		public List<BundleItemEntity> _bundleItems { get; set; }

		public BundleEntity()
		{
			base.EntityName = "bundle";
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
			jsonWriter.WritePropertyName("bid");
			jsonWriter.Write(BundleId);
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(Title);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Description);
			jsonWriter.WritePropertyName("l");
			jsonWriter.Write(LabelName);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(Gas);
			jsonWriter.WritePropertyName("j");
			jsonWriter.Write(Joules);
			jsonWriter.WritePropertyName("o");
			jsonWriter.Write(Order.HasValue ? Order.Value : 0);
			jsonWriter.WritePropertyName("i");
			jsonWriter.Write(ImageUrl);
			jsonWriter.WritePropertyName("sd");
			jsonWriter.Write(StartDate);
			jsonWriter.WritePropertyName("ed");
			jsonWriter.Write(EndDate);
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
					case "bid":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							BundleId = new int?(num3.Value).Value;
							break;
						}
						throw new JsonException("Invalid 'bundleId' value in 'BundleEntity'");
					}
					case "t":
						reader.Read();
						Title = (string)reader.Value;
						break;
					case "d":
						reader.Read();
						Description = (string)reader.Value;
						break;
					case "l":
						reader.Read();
						LabelName = (string)reader.Value;
						break;
					case "g":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							Gas = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gas' value in 'BundleEntity'");
					}
					case "o":
						reader.Read();
						Order = Parsers.ParseInt(reader.Value);
						break;
					case "j":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							Joules = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'joules' value in 'BundleEntity'");
					}
					case "i":
						reader.Read();
						ImageUrl = (string)reader.Value;
						break;
					case "gi":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GameId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'BundleEntity'");
					}
					case "sd":
						reader.Read();
						StartDate = (string)reader.Value;
						break;
					case "ed":
						reader.Read();
						EndDate = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && GameId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
