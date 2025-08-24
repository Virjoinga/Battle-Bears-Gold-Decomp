using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class CustomGameDataEntity : BaseEntity
	{
		private List<string> _CustomGameDataList;

		public int GameId { get; set; }

		public List<string> CustomGameData
		{
			get
			{
				return _CustomGameDataList;
			}
			set
			{
				_CustomGameDataList = value;
			}
		}

		public CustomGameDataEntity()
		{
			_CustomGameDataList = new List<string>();
			base.EntityName = "customgamedata";
		}

		public override string Serialize()
		{
			return Serialize(true);
		}

		public override string Serialize(bool baseData)
		{
			JsonWriter jsonWriter = new JsonWriter();
			jsonWriter.WriteObjectStart();
			SerializeBase(jsonWriter);
			Serialize(_CustomGameDataList, jsonWriter, "cgd");
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WriteObjectEnd();
			return jsonWriter.ToString();
		}

		public override bool Populate(string json, bool baseData)
		{
			try
			{
				if (json == null)
				{
					return false;
				}
				json = StripUnusedCharacters(json);
				JsonReader jsonReader = new JsonReader(json);
				jsonReader.Read();
				if (jsonReader.Token != JsonToken.ObjectStart)
				{
					return false;
				}
				while (jsonReader.Read() && jsonReader.Token != JsonToken.ObjectEnd)
				{
					if (jsonReader.Value == null)
					{
						continue;
					}
					switch (jsonReader.Value.ToString())
					{
					case "gi":
					{
						jsonReader.Read();
						int? num = Parsers.ParseInt(jsonReader.Value);
						if (num.HasValue)
						{
							GameId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'customGameDataEntity'");
					}
					case "cgd":
						while (jsonReader.Read() && jsonReader.Token != JsonToken.ArrayEnd)
						{
							string item = (string)jsonReader.Value;
							_CustomGameDataList.Add(item);
						}
						break;
					default:
						PopulateBase(jsonReader);
						break;
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
