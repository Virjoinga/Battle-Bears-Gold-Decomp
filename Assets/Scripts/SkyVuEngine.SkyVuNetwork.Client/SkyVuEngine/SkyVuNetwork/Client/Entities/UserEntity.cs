using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class UserEntity : BaseEntity
	{
		public string LastLoggedIn { get; set; }

		public string GamerTag { get; set; }

		public string ImageUrl { get; set; }

		public UserEntity()
		{
			_isSerialRequired = true;
			base.EntityName = "user";
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
			jsonWriter.WritePropertyName("t");
			jsonWriter.Write(GamerTag);
			jsonWriter.WritePropertyName("l");
			jsonWriter.Write((LastLoggedIn != null) ? LastLoggedIn.Replace("/", "-") : null);
			jsonWriter.WritePropertyName("iu");
			jsonWriter.Write(ImageUrl);
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
					case "t":
						reader.Read();
						GamerTag = (string)reader.Value;
						continue;
					case "l":
						reader.Read();
						LastLoggedIn = (string)reader.Value;
						continue;
					case "iu":
						reader.Read();
						ImageUrl = (string)reader.Value;
						continue;
					}
					if (BaseData)
					{
						PopulateBase(reader);
					}
				}
				return base.IsPopulated && GamerTag.Length > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
