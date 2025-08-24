using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class RankingStatsEntity : BaseEntity
	{
		public class Entry
		{
			public int UserId { get; set; }

			public int Score { get; set; }
		}

		public List<Entry> Entries { get; set; }

		public int Count { get; set; }

		public int RankingType { get; set; }

		public int RankingSearchType { get; set; }

		public int Aux1 { get; set; }

		public RankingStatsEntity()
		{
			Entries = new List<Entry>();
			base.EntityName = "ping";
		}

		public override string Serialize()
		{
			return Serialize(true);
		}

		public override string Serialize(bool BaseData)
		{
			try
			{
				JsonWriter writer = new JsonWriter();
				writer.WriteObjectStart();
				if (BaseData)
				{
					SerializeBase(writer);
				}
				writer.WritePropertyName("c");
				writer.Write(Count);
				writer.WritePropertyName("t");
				writer.Write(RankingType);
				writer.WritePropertyName("st");
				writer.Write(RankingSearchType);
				writer.WritePropertyName("a1");
				writer.Write(Aux1);
				writer.WritePropertyName("entries");
				writer.WriteArrayStart();
				Entries.ForEach(delegate(Entry e)
				{
					writer.WriteObjectStart();
					writer.WritePropertyName("eu");
					writer.Write(e.UserId);
					writer.WritePropertyName("es");
					writer.Write(e.Score);
					writer.WriteObjectEnd();
				});
				writer.WriteArrayEnd();
				writer.WriteObjectEnd();
				return writer.ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public override bool Populate(JsonReader reader)
		{
			return Populate(reader, true);
		}

		public override bool Populate(JsonReader reader, bool BaseData)
		{
			Entries.Clear();
			Count = 0;
			Aux1 = 0;
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
					case "c":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							Count = num.Value;
							break;
						}
						throw new JsonException("Invalid 'count' value in 'RankingStatsEntity'");
					}
					case "t":
					{
						reader.Read();
						int? num6 = Parsers.ParseInt(reader.Value);
						if (num6.HasValue)
						{
							RankingType = num6.Value;
							break;
						}
						throw new JsonException("Invalid 'rankingType' value in 'RankingStatsEntity'");
					}
					case "st":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							RankingSearchType = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'rankingSearchType' value in 'RankingStatsEntity'");
					}
					case "a1":
					{
						reader.Read();
						int? num5 = Parsers.ParseInt(reader.Value);
						if (num5.HasValue)
						{
							Aux1 = num5.Value;
							break;
						}
						throw new JsonException("Invalid 'aux1' value in 'RankingStatsEntity'");
					}
					case "entries":
					{
						bool flag = false;
						bool flag2 = false;
						Entry entry = null;
						while (reader.Read() && reader.Token != JsonToken.ArrayEnd)
						{
							if (reader.Token == JsonToken.ObjectStart)
							{
								entry = new Entry();
							}
							else if (reader.Token == JsonToken.ObjectEnd)
							{
								if (flag && flag2)
								{
									Entries.Add(entry);
								}
								flag = false;
								flag2 = false;
							}
							if ("eu" == (string)reader.Value)
							{
								reader.Read();
								int? num2 = Parsers.ParseInt(reader.Value);
								if (!num2.HasValue)
								{
									throw new JsonException("Invalid 'userId' value in 'RankingStatsEntity'");
								}
								flag = true;
								entry.UserId = num2.Value;
							}
							else if ("es" == (string)reader.Value)
							{
								reader.Read();
								int? num3 = Parsers.ParseInt(reader.Value);
								if (!num3.HasValue)
								{
									throw new JsonException("Invalid 'score' value in 'RankingStatsEntity'");
								}
								flag2 = true;
								entry.Score = num3.Value;
							}
						}
						break;
					}
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
