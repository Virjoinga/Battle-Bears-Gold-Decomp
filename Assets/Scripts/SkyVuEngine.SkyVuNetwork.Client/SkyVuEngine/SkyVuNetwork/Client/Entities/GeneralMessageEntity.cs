using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class GeneralMessageEntity : BaseEntity
	{
		public string Message { get; set; }

		public GeneralMessageEntity()
		{
			_isSerialRequired = true;
			base.EntityName = "generalmessage";
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
			jsonWriter.WritePropertyName("m");
			jsonWriter.Write(Message);
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
						string text = reader.Value.ToString();
						if (text != null && text == "m")
						{
							reader.Read();
							Message = (string)reader.Value;
						}
						else if (BaseData)
						{
							PopulateBase(reader);
						}
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
