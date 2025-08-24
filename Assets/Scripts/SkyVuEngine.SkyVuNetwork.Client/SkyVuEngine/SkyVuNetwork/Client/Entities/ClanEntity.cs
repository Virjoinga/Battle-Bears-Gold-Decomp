using System;
using System.Collections.Generic;
using System.Linq;
using SkyVu.Common;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.SkyVuNetwork.Client.Entities
{
	public class ClanEntity : BaseEntity
	{
		public int ClanId { get; set; }

		public int GameId { get; set; }

		public int TopLimit { get; set; }

		public List<string> Members { get; set; }

		public string Description { get; set; }

		public string Name { get; set; }

		public bool IsInviteOnly { get; set; }

		public bool IsRequest { get; set; }

		public string LastActive { get; set; }

		private string _members { get; set; }

		public ClanEntity()
		{
			Members = new List<string>();
			base.EntityName = "clan";
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
			jsonWriter.WritePropertyName("cid");
			jsonWriter.Write(ClanId);
			jsonWriter.WritePropertyName("gi");
			jsonWriter.Write(GameId);
			jsonWriter.WritePropertyName("tl");
			jsonWriter.Write(TopLimit);
			Members.ForEach(delegate(string b)
			{
				_members = _members + b.ToString() + ";";
			});
			if (!string.IsNullOrEmpty(_members))
			{
				_members = _members.Substring(0, _members.Length - 1);
			}
			jsonWriter.WritePropertyName("mem");
			jsonWriter.Write(_members);
			jsonWriter.WritePropertyName("d");
			jsonWriter.Write(Description);
			jsonWriter.WritePropertyName("n");
			jsonWriter.Write(Name);
			jsonWriter.WritePropertyName("iv");
			jsonWriter.Write(IsInviteOnly);
			jsonWriter.WritePropertyName("rq");
			jsonWriter.Write(IsRequest);
			jsonWriter.WritePropertyName("l");
			jsonWriter.Write(LastActive);
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
					case "cid":
					{
						reader.Read();
						int? num = Parsers.ParseInt(reader.Value);
						if (num.HasValue)
						{
							ClanId = num.Value;
							break;
						}
						throw new JsonException("Invalid 'clanId' value in 'ClanEntity'");
					}
					case "gi":
					{
						reader.Read();
						int? num3 = Parsers.ParseInt(reader.Value);
						if (num3.HasValue)
						{
							GameId = num3.Value;
							break;
						}
						throw new JsonException("Invalid 'gameId' value in 'ClanEntity'");
					}
					case "tl":
					{
						reader.Read();
						int? num2 = Parsers.ParseInt(reader.Value);
						if (num2.HasValue)
						{
							TopLimit = num2.Value;
							break;
						}
						throw new JsonException("Invalid 'topLimit' value in 'ClanEntity'");
					}
					case "mem":
						reader.Read();
						_members = (string)reader.Value;
						break;
					case "d":
						reader.Read();
						Description = (string)reader.Value;
						break;
					case "n":
						reader.Read();
						Name = (string)reader.Value;
						break;
					case "iv":
						reader.Read();
						IsInviteOnly = (bool)reader.Value;
						break;
					case "rq":
						reader.Read();
						IsRequest = (bool)reader.Value;
						break;
					case "l":
						reader.Read();
						LastActive = (string)reader.Value;
						break;
					default:
						if (BaseData)
						{
							PopulateBase(reader);
						}
						break;
					}
				}
				if (!string.IsNullOrEmpty(_members))
				{
					_members.Split(';').ToList().ForEach(delegate(string m)
					{
						if (!string.IsNullOrEmpty(m))
						{
							Members.Add(m);
						}
					});
				}
				return base.IsPopulated && GameId > 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}
	}
}
