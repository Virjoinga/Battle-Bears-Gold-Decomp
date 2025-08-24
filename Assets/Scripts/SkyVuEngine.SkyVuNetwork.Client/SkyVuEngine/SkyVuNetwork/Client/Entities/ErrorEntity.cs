using SkyVu.Common;
using SkyVu.Common.Enums;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class ErrorEntity : BaseEntity
	{
		public int ErrorCode { get; set; }

		public ErrorEntity()
		{
			ErrorCode = -999;
			base.EntityName = "error";
		}

		public ErrorEntity(ErrorCodes errorCodes)
		{
			ErrorCode = (int)errorCodes;
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
			jsonWriter.WritePropertyName("err");
			jsonWriter.Write(ErrorCode);
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
					string text = reader.Value.ToString();
					if (text != null && text == "err")
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (!num.HasValue)
						{
							throw new JsonException("Invalid 'errorCode' value in 'ErrorEntity'");
						}
						ErrorCode = num.Value;
					}
					else if (BaseData)
					{
						PopulateBase(reader);
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
