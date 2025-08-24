using System;
using System.Collections.Generic;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class DataTrackingEntity : BaseEntity
	{
		private string _data = string.Empty;

		public int DataTrackingTypeId { get; set; }

		public int GameId { get; set; }

		public string LabelName { get; set; }

		public Dictionary<string, string> Data { get; set; }

		public DataTrackingEntity()
		{
			_isSerialRequired = true;
			Data = new Dictionary<string, string>();
			base.EntityName = "datatracking";
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
			jsonWriter.WritePropertyName("tt");
			jsonWriter.Write(DataTrackingTypeId);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("l");
			jsonWriter.Write(LabelName);
			_data = string.Empty;
			foreach (KeyValuePair<string, string> datum in Data)
			{
				if (datum.Key.Contains("=") || datum.Key.Contains(";") || datum.Value.Contains("=") || datum.Value.Contains(";"))
				{
					throw new Exception("Data cannot contain '=' or ';'");
				}
				string data = _data;
				_data = data + datum.Key + "=" + datum.Value + ";";
			}
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(_data);
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
					case "tt":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							DataTrackingTypeId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'trackingType' value in 'DataTracking'");
					}
					case "gi":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GameId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'DataTracking'");
					}
					case "l":
						reader.Read();
						LabelName = (string)reader.Value;
						break;
					case "d":
						reader.Read();
						_data = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				Data.Clear();
				string[] array = _data.Split(';');
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split('=');
					if (array3.Length == 2)
					{
						Data.Add(array3[0], array3[1]);
					}
				}
				return base.IsPopulated && DataTrackingTypeId > 0 && LabelName != null && LabelName.Length > 0 && GameId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
