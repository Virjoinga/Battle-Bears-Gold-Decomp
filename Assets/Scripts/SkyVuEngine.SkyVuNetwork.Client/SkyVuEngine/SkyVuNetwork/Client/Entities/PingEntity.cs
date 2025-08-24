using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class PingEntity : BaseEntity
	{
		public string Response { get; set; }

		public PingEntity()
		{
			base.EntityName = "ping";
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
			jsonWriter.WritePropertyName("Response");
			jsonWriter.Write(Response);
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
					if (reader.Value != null)
					{
						Response += reader.Value.ToString();
					}
				}
				return base.IsPopulated && Response.Length > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
