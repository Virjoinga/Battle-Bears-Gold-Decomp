using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class BuddyRequestEntity : BaseEntity
	{
		public int BuddyRequestId { get; set; }

		public int BuddyRequestStatusId { get; set; }

		public string RequesterGamerTag { get; set; }

		public int GameId { get; set; }

		public string CreateDate { get; set; }

		public int RequestedUserId { get; set; }

		public int RequesterUserId { get; set; }

		public BuddyRequestEntity()
		{
			base.EntityName = "buddyrequest";
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
			jsonWriter.Write(BuddyRequestId);
			jsonWriter.WritePropertyName("b");
			jsonWriter.Write(BuddyRequestStatusId);
			jsonWriter.WritePropertyName("rgt");
			jsonWriter.Write(RequesterGamerTag);
			jsonWriter.WritePropertyName("g");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("c");
			jsonWriter.Write((CreateDate != null) ? CreateDate.Replace("/", "-") : null);
			jsonWriter.WritePropertyName("rd");
			jsonWriter.Write(RequestedUserId);
			jsonWriter.WritePropertyName("rr");
			jsonWriter.Write(RequesterUserId);
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
					case "i":
					{
						reader.Read();
						int? num5 = Parsers.ParseInt(reader.Value);
						if (num5.HasValue)
						{
							BuddyRequestId = num5.Value;
							break;
						}
						throw new JsonException("Invalid 'buddyRequestId' value in 'BuddyRequestEntity'");
					}
					case "b":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							BuddyRequestStatusId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'buddyRequestStatusId' value in 'BuddyRequestEntity'");
					}
					case "rgt":
						reader.Read();
						RequesterGamerTag = (string)reader.Value;
						break;
					case "g":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							GameId = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'BuddyRequestEntity'");
					}
					case "c":
						reader.Read();
						CreateDate = (string)reader.Value;
						break;
					case "rd":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							RequestedUserId = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'requestedUserId' value in 'BuddyRequestEntity'");
					}
					case "rr":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							RequesterUserId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'requesterUserId' value in 'BuddyRequestEntity'");
					}
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && RequestedUserId > 0 && RequesterUserId > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
