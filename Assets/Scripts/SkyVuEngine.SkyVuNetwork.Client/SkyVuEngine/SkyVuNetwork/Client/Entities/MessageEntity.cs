using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class MessageEntity : BaseEntity
	{
		public string Subject { get; set; }

		public string Message { get; set; }

		public int MessageId { get; set; }

		public int MessageType { get; set; }

		public int MessageStatus { get; set; }

		public int To { get; set; }

		public int From { get; set; }

		public string FromGamerTag { get; set; }

		public string Date { get; set; }

		public MessageEntity()
		{
			_isSerialRequired = true;
			base.EntityName = "message";
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
			jsonWriter.WritePropertyName("s");
			jsonWriter.Write(Subject);
			jsonWriter.WritePropertyName("m");
			jsonWriter.Write(Message);
			jsonWriter.WritePropertyName("mi");
			jsonWriter.Write(MessageId);
			jsonWriter.WritePropertyName("msgt");
			jsonWriter.Write(MessageType);
			jsonWriter.WritePropertyName("ms");
			jsonWriter.Write(MessageStatus);
			jsonWriter.WritePropertyName("to");
			jsonWriter.Write(To);
			jsonWriter.WritePropertyName("f");
			jsonWriter.Write(From);
			jsonWriter.WritePropertyName("fgt");
			jsonWriter.Write(FromGamerTag);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Date);
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
					case "s":
						reader.Read();
						Subject = (string)reader.Value;
						break;
					case "m":
						reader.Read();
						Message = (string)reader.Value;
						break;
					case "mi":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							MessageId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'messageId' value in 'message entity'");
					}
					case "msgt":
					{
						reader.Read();
						int? num5 = Parsers.ParseInt(reader.Value);
						if (num5.HasValue)
						{
							MessageType = num5.Value;
							break;
						}
						throw new JsonException("Invalid 'messageType' value in 'message entity'");
					}
					case "ms":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							MessageStatus = num.Value;
							break;
						}
						throw new JsonException("Invalid 'messageStatus' value in 'message entity'");
					}
					case "to":
					{
						reader.Read();
						int? num4 = Parsers.ParseInt(reader.Value);
						if (num4.HasValue)
						{
							To = num4.Value;
							break;
						}
						throw new JsonException("Invalid 'to' value in 'message entity'");
					}
					case "f":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							From = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'from' value in 'message entity'");
					}
					case "fgt":
						reader.Read();
						FromGamerTag = (string)reader.Value;
						break;
					case "d":
						reader.Read();
						Date = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				return base.IsPopulated && To > 0 && From > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}
